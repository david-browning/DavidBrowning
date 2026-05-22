// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.
using System;
using System.Linq;
using System.Collections;
using System.Reflection;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using System.Threading.Tasks;
using System.IO;
using System.Collections.Generic;
using System.Threading;
using Microsoft.EntityFrameworkCore.Storage;

namespace DavidBrowning.Data.Seeding
{
   /// <summary>
   /// Reflection-driven JSON seed loader for EF Core.
   /// 
   /// The seeder scans a folder of JSON files, matches each file to an EF entity type,
   /// deserializes the file into List&lt;TEntity&gt;, and inserts the rows through the supplied DbContext.
   /// 
   /// File names may match any of:
   /// - DbSet property name, e.g. Projects.json
   /// - EF table name, e.g. db_Projects.json
   /// - EF table name without the configured prefix, e.g. Projects.json when table is db_Projects
   /// - CLR entity type name, e.g. Project.json
   /// 
   /// Principal tables are inserted before dependent tables by inspecting EF foreign-key metadata.
   /// </summary>
   public sealed class JsonSeedDatabaseSeeder<TDbContext>
       where TDbContext : DbContext
   {
      public JsonSeedDatabaseSeeder(
          TDbContext dbContext,
          JsonSeedDatabaseSeederOptions? options = null,
          JsonSerializerOptions? serializerOptions = null)
      {
         this._dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
         this._options = options ?? new JsonSeedDatabaseSeederOptions();
         this._serializerOptions = serializerOptions ?? JsonSeedSerializerOptions.CreateDefault();
      }

      public async Task<JsonSeedResult> SeedAsync(
          string seedRootFolder,
          CancellationToken cancellationToken = default)
      {
         ArgumentException.ThrowIfNullOrWhiteSpace(seedRootFolder);
         if (!Directory.Exists(seedRootFolder))
         {
            throw new DirectoryNotFoundException($"Seed folder not found: {seedRootFolder}");
         }

         IReadOnlyList<SeedFileMatch> matches = MatchSeedFiles(seedRootFolder);
         IReadOnlyList<SeedFileMatch> orderedMatches = OrderByForeignKeyDependencies(matches);

         JsonSeedResult result = new();
         foreach (var match in orderedMatches)
         {
            var fileResult = await SeedFileAsync(match, cancellationToken);
            result.Files.Add(fileResult);
         }

         if (_options.ThrowOnUnmatchedJsonFiles)
         {
            IReadOnlySet<string> matchedPaths = matches
                .Select(match => Path.GetFullPath(match.FilePath))
                .ToHashSet(StringComparer.OrdinalIgnoreCase);

            var unmatchedFiles = Directory
                .EnumerateFiles(seedRootFolder, "*.json", SearchOption.TopDirectoryOnly)
                .Where(filePath => !matchedPaths.Contains(Path.GetFullPath(filePath)))
                .ToArray();

            if (unmatchedFiles.Length > 0)
            {
               var fileList = string.Join(Environment.NewLine, unmatchedFiles.Select(path => $" - {path}"));
               throw new InvalidOperationException($"Unmatched seed JSON files were found:{Environment.NewLine}{fileList}");
            }
         }

         return result;
      }

      private async Task<JsonSeedFileResult> SeedFileAsync(
          SeedFileMatch match,
          CancellationToken cancellationToken)
      {
         if (_options.SkipFileWhenTargetTableHasRows)
         {
            bool hasAnyRows = await HasAnyRowsAsync(match.EntityType.ClrType, cancellationToken);
            if (hasAnyRows)
            {
               return JsonSeedFileResult.Skipped(match.FilePath, match.EntityType.ClrType, "Target table already has rows.");
            }
         }

         Type listType = typeof(List<>).MakeGenericType(match.EntityType.ClrType);
         await using FileStream stream = File.OpenRead(match.FilePath);
         // And exception here likely means that the JSON property name does not
         // match the C# type or an enum value is mismatched.
         object? deserialized = await JsonSerializer.DeserializeAsync(
             stream,
             listType,
             _serializerOptions,
             cancellationToken);

         if (deserialized is not IEnumerable enumerable)
         {
            throw new InvalidOperationException($"Seed file did not deserialize to an enumerable: {match.FilePath}");
         }

         object[] rows = enumerable.Cast<object>().ToArray();
         if (rows.Length == 0)
         {
            return JsonSeedFileResult.Skipped(match.FilePath, match.EntityType.ClrType, "Seed file had no rows.");
         }

         if (!_dbContext.Database.IsRelational())
         {
            _dbContext.AddRange(rows);
            int savedRows = await _dbContext.SaveChangesAsync(cancellationToken);
            return JsonSeedFileResult.Inserted(match.FilePath, match.EntityType.ClrType, rows.Length, savedRows);
         }

         bool useSqlServerIdentityInsert = ShouldUseSqlServerIdentityInsert(match, rows);
         IExecutionStrategy executionStrategy = _dbContext.Database.CreateExecutionStrategy();
         return await executionStrategy.ExecuteAsync(async () =>
         {
            await using Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction transaction =
               await _dbContext.Database.BeginTransactionAsync(cancellationToken);

            if (useSqlServerIdentityInsert)
            {
               await SetSqlServerIdentityInsertAsync(match, enabled: true, cancellationToken);
            }

            _dbContext.AddRange(rows);
            int savedRows = await _dbContext.SaveChangesAsync(cancellationToken);

            if (useSqlServerIdentityInsert)
            {
               await SetSqlServerIdentityInsertAsync(match, enabled: false, cancellationToken);
            }

            await transaction.CommitAsync(cancellationToken);

            return JsonSeedFileResult.Inserted(match.FilePath, match.EntityType.ClrType, rows.Length, savedRows);
         });
      }

      private IReadOnlyList<SeedFileMatch> MatchSeedFiles(string seedRootFolder)
      {
         var entityTypesBySeedName = BuildEntityTypeNameIndex();
         List<SeedFileMatch> matches = [];
         foreach (string filePath in Directory.EnumerateFiles(seedRootFolder, "*.json", SearchOption.TopDirectoryOnly))
         {
            string fileName = Path.GetFileNameWithoutExtension(filePath);
            string normalizedFileName = NormalizeSeedName(RemoveConfiguredTablePrefix(fileName));

            if (!entityTypesBySeedName.TryGetValue(normalizedFileName, out IEntityType? entityType))
            {
               if (_options.ThrowOnUnmatchedJsonFiles)
               {
                  throw new InvalidOperationException(
                      $"Could not match seed file '{Path.GetFileName(filePath)}' to an EF entity type.");
               }

               continue;
            }

            matches.Add(new SeedFileMatch(filePath, entityType));
         }

         return matches;
      }

      private IReadOnlyDictionary<string, IEntityType> BuildEntityTypeNameIndex()
      {
         Dictionary<Type, string> dbSetNamesByClrType = typeof(TDbContext)
             .GetProperties(BindingFlags.Instance | BindingFlags.Public)
             .Where(property =>
                 property.PropertyType.IsGenericType &&
                 property.PropertyType.GetGenericTypeDefinition() == typeof(DbSet<>))
             .ToDictionary(
                 property => property.PropertyType.GetGenericArguments()[0],
                 property => property.Name);

         Dictionary<string, IEntityType> entityTypesBySeedName = new(StringComparer.OrdinalIgnoreCase);

         foreach (IEntityType entityType in _dbContext.Model.GetEntityTypes())
         {
            if (entityType.ClrType is null)
            {
               continue;
            }

            List<string> candidateNames =
            [
                entityType.ClrType.Name
            ];

            if (dbSetNamesByClrType.TryGetValue(entityType.ClrType, out string? dbSetName))
            {
               candidateNames.Add(dbSetName);
            }

            string? tableName = entityType.GetTableName();

            if (!string.IsNullOrWhiteSpace(tableName))
            {
               candidateNames.Add(tableName);
               candidateNames.Add(RemoveConfiguredTablePrefix(tableName));
            }

            foreach (string candidateName in candidateNames)
            {
               string normalizedCandidateName = NormalizeSeedName(candidateName);

               if (entityTypesBySeedName.TryGetValue(normalizedCandidateName, out IEntityType? existingEntityType) &&
                   existingEntityType.ClrType != entityType.ClrType)
               {
                  throw new InvalidOperationException(
                      $"Ambiguous seed name '{candidateName}' maps to both '{existingEntityType.ClrType.Name}' and '{entityType.ClrType.Name}'. " +
                      "Rename the JSON file or add a DbSet property with a unique name.");
               }

               entityTypesBySeedName[normalizedCandidateName] = entityType;
            }
         }

         return entityTypesBySeedName;
      }

      private IReadOnlyList<SeedFileMatch> OrderByForeignKeyDependencies(IReadOnlyList<SeedFileMatch> matches)
      {
         Dictionary<IEntityType, SeedFileMatch> matchesByEntityType = matches
             .ToDictionary(match => match.EntityType);

         Dictionary<IEntityType, VisitState> visitStates = [];
         List<SeedFileMatch> orderedMatches = [];

         foreach (SeedFileMatch match in matches)
         {
            Visit(match.EntityType);
         }

         return orderedMatches;

         void Visit(IEntityType entityType)
         {
            if (visitStates.TryGetValue(entityType, out VisitState visitState))
            {
               if (visitState == VisitState.Visiting)
               {
                  // Cycles can happen with self-references or mutually-dependent optional relationships.
                  // Leave those to EF/database constraints rather than pretending we can solve every cycle here.
                  return;
               }

               return;
            }

            visitStates[entityType] = VisitState.Visiting;

            foreach (IForeignKey foreignKey in entityType.GetForeignKeys())
            {
               IEntityType principalEntityType = foreignKey.PrincipalEntityType;

               if (principalEntityType == entityType)
               {
                  continue;
               }

               if (matchesByEntityType.ContainsKey(principalEntityType))
               {
                  Visit(principalEntityType);
               }
            }

            visitStates[entityType] = VisitState.Visited;
            if (matchesByEntityType.TryGetValue(entityType, out SeedFileMatch? match))
            {
               orderedMatches.Add(match);
            }
         }
      }

      private async Task<bool> HasAnyRowsAsync(
          Type entityClrType,
          CancellationToken cancellationToken)
      {
         object? dbSet = _dbContextSetMethod
             .MakeGenericMethod(entityClrType)
             .Invoke(_dbContext, parameters: null);

         object? task = _anyAsyncMethod
             .MakeGenericMethod(entityClrType)
             .Invoke(null, [dbSet!, cancellationToken]);

         if (task is not Task<bool> typedTask)
         {
            throw new InvalidOperationException($"Could not execute AnyAsync for entity type '{entityClrType.Name}'.");
         }

         return await typedTask;
      }

      private bool ShouldUseSqlServerIdentityInsert(
          SeedFileMatch match,
          object[] rows)
      {
         if (!_options.UseSqlServerIdentityInsertWhenNeeded)
         {
            return false;
         }

         string? providerName = _dbContext.Database.ProviderName;

         if (providerName is null ||
             !providerName.Contains("SqlServer", StringComparison.OrdinalIgnoreCase))
         {
            return false;
         }

         IProperty? identityKeyProperty = match.EntityType
             .FindPrimaryKey()
             ?.Properties
             .SingleOrDefault(property =>
                 property.ValueGenerated == ValueGenerated.OnAdd &&
                 IsIntegerType(property.ClrType));

         if (identityKeyProperty is null)
         {
            return false;
         }

         PropertyInfo? propertyInfo = identityKeyProperty.PropertyInfo;

         if (propertyInfo is null)
         {
            return false;
         }

         return rows.Any(row =>
         {
            object? value = propertyInfo.GetValue(row);
            return value is not null && !value.Equals(Activator.CreateInstance(propertyInfo.PropertyType));
         });
      }

      private async Task SetSqlServerIdentityInsertAsync(
          SeedFileMatch match,
          bool enabled,
          CancellationToken cancellationToken)
      {
         string? tableName = match.EntityType.GetTableName();
         if (string.IsNullOrWhiteSpace(tableName))
         {
            throw new InvalidOperationException($"Could not determine table name for entity type '{match.EntityType.ClrType.Name}'.");
         }

         string? schema = match.EntityType.GetSchema();
         string delimitedTableName = string.IsNullOrWhiteSpace(schema)
             ? DelimitSqlServerIdentifier(tableName)
             : $"{DelimitSqlServerIdentifier(schema)}.{DelimitSqlServerIdentifier(tableName)}";

         string enabledText = enabled ? "ON" : "OFF";
#pragma warning disable EF1002 // Risk of vulnerability to SQL injection.
         // This is only used to seed the database.
         await _dbContext.Database.ExecuteSqlRawAsync(
             $"SET IDENTITY_INSERT {delimitedTableName} {enabledText};",
             cancellationToken);
#pragma warning restore EF1002 // Risk of vulnerability to SQL injection.
      }

      private string RemoveConfiguredTablePrefix(string name)
      {
         if (string.IsNullOrWhiteSpace(_options.TablePrefix))
         {
            return name;
         }

         return name.StartsWith(_options.TablePrefix, StringComparison.OrdinalIgnoreCase)
             ? name[_options.TablePrefix.Length..]
             : name;
      }

      private static string NormalizeSeedName(string name)
      {
         return new string(
             name
                 .Where(char.IsLetterOrDigit)
                 .Select(char.ToLowerInvariant)
                 .ToArray());
      }

      private static bool IsIntegerType(Type type)
      {
         Type underlyingType = Nullable.GetUnderlyingType(type) ?? type;

         return underlyingType == typeof(byte) ||
             underlyingType == typeof(short) ||
             underlyingType == typeof(int) ||
             underlyingType == typeof(long);
      }

      private static string DelimitSqlServerIdentifier(string identifier)
      {
         return $"[{identifier.Replace("]", "]]", StringComparison.Ordinal)}]";
      }

      private sealed record SeedFileMatch(
          string FilePath,
          IEntityType EntityType);

      private enum VisitState
      {
         Visiting,
         Visited
      }

      private static readonly MethodInfo _dbContextSetMethod = typeof(DbContext)
          .GetMethods()
          .Single(method =>
              method.Name == nameof(DbContext.Set) &&
              method.IsGenericMethodDefinition &&
              method.GetParameters().Length == 0);

      private static readonly MethodInfo _anyAsyncMethod = typeof(EntityFrameworkQueryableExtensions)
          .GetMethods()
          .Single(method =>
              method.Name == nameof(EntityFrameworkQueryableExtensions.AnyAsync) &&
              method.IsGenericMethodDefinition &&
              method.GetParameters().Length == 2 &&
              method.GetParameters()[0].ParameterType.IsGenericType &&
              method.GetParameters()[0].ParameterType.GetGenericTypeDefinition() == typeof(IQueryable<>));

      private readonly TDbContext _dbContext;
      private readonly JsonSeedDatabaseSeederOptions _options;
      private readonly JsonSerializerOptions _serializerOptions;
   }
}