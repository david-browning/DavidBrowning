// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.
using DavidBrowning.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DavidBrowning.Infrastructure.Data.Stores;

public sealed class SqlUncategorizedStore : IUncategorizedStore
{
   public SqlUncategorizedStore(
      ILogger<SqlUncategorizedStore> logger,
      SiteDbContext context)
   {
      _logger = logger;
      _context = context;
   }

   public async Task<IReadOnlyList<Interest>> GetAllInterestsAsync(
      CancellationToken cancellationToken = default)
   {
      return await _context.Interests
         .AsNoTracking()
         .Include(interest => interest.FeaturedPost)
         .OrderBy(interest => interest.SortOrder)
         .ThenBy(interest => interest.DisplayName)
         .ToListAsync(cancellationToken);
   }

   public async Task<IReadOnlyList<Interest>> GetInterestsAsync(
      CancellationToken cancellationToken = default)
   {
      return await _context.Interests
         .AsNoTracking()
         .Include(interest => interest.FeaturedPost)
         .OrderBy(interest => interest.SortOrder)
         .ThenBy(interest => interest.DisplayName)
         .Where(i => i.IsActive)
         .ToListAsync(cancellationToken);
   }

   public async Task<Interest?> GetInterestAsync(
      int id,
      CancellationToken cancellationToken)
   {
      return await _context.Interests
         .AsNoTracking()
         .SingleOrDefaultAsync(
            interest => interest.Id == id,
            cancellationToken);
   }

   public async Task InsertInterestAsync(
      Interest interest,
      CancellationToken cancellationToken)
   {
      ArgumentNullException.ThrowIfNull(interest);
      if (await _context.SlugExistsAsync<Interest>(
         interest.Slug, cancellationToken: cancellationToken))
      {
         throw new DuplicateSlugException(interest.Slug);
      }

      await ValidateFeaturedPostAsync(interest.FeaturedPostId, cancellationToken);
      interest.FeaturedPost = null;
      _context.Interests.Add(interest);
      await _context.SaveChangesAsync(cancellationToken);
      _logger.LogInformation(
         "Created interest {InterestId}: {DisplayName}",
         interest.Id, interest.DisplayName);
   }

   public async Task<bool> UpdateInterestAsync(
    Interest interest,
    CancellationToken cancellationToken)
   {
      ArgumentNullException.ThrowIfNull(interest);
      if (await _context.SlugExistsAsync<Interest>(
         interest.Slug, excludedId: interest.Id, cancellationToken))
      {
         throw new DuplicateSlugException(interest.Slug);
      }

      await ValidateFeaturedPostAsync(
         interest.FeaturedPostId, cancellationToken);

      Interest? storedInterest = await _context.Interests
         .SingleOrDefaultAsync(
            existingInterest => existingInterest.Id == interest.Id,
            cancellationToken);

      if (storedInterest is null)
      {
         return false;
      }

      storedInterest.Slug = interest.Slug;
      storedInterest.DisplayName = interest.DisplayName;
      storedInterest.Summary = interest.Summary;
      storedInterest.IconCssClass = interest.IconCssClass;
      storedInterest.IsActive = interest.IsActive;
      storedInterest.FeaturedPostId = interest.FeaturedPostId;

      await _context.SaveChangesAsync(cancellationToken);
      _logger.LogInformation(
         "Updated interest {InterestId}: {DisplayName}",
         storedInterest.Id, storedInterest.DisplayName);
      return true;
   }

   public async Task<bool> DeleteInterestAsync(
      int id,
      CancellationToken cancellationToken = default)
   {
      Interest? interest = await _context.Interests
         .SingleOrDefaultAsync(
            interest => interest.Id == id,
            cancellationToken);

      if (interest is null)
      {
         return false;
      }

      _context.Interests.Remove(interest);
      await _context.SaveChangesAsync(cancellationToken);
      _logger.LogInformation(
         "Deleted interest {InterestId}: {DisplayName}",
         interest.Id, interest.DisplayName);

      return true;
   }

   public async Task ReorderInterestsAsync(
      IReadOnlyList<int> idsInDisplayOrder,
      CancellationToken cancellationToken)
   {
      int changedRecordCount = await _context.ApplySortOrderAsync<Interest>(
         idsInDisplayOrder, cancellationToken);
      if (changedRecordCount == 0)
      {
         return;
      }

      await _context.SaveChangesAsync(cancellationToken);
      _logger.LogInformation(
         "Updated the sort order for {InterestCount} interests",
         changedRecordCount);
   }

   public async Task<IReadOnlyList<SiteAsset>> GetSiteAssetsAsync(
      CancellationToken cancellationToken = default)
   {
      return await _context.SiteAssets
         .AsNoTracking()
         .ToListAsync(cancellationToken);
   }

   public async Task<SiteAsset?> GetAssetAsync(
      int id,
      CancellationToken cancellationToken = default)
   {
      return await _context.SiteAssets
         .AsNoTracking()
         .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
   }

   public async Task<bool> DeleteAssetAsync(
      int id,
      CancellationToken cancellationToken = default)
   {
      var asset = await _context.SiteAssets
         .SingleOrDefaultAsync(asset => asset.Id == id, cancellationToken);
      if (asset is null)
      {
         return false;
      }

      _context.SiteAssets.Remove(asset);
      await _context.SaveChangesAsync(cancellationToken);
      return true;
   }

   public async Task<bool> UpdateAssetAsync(
      SiteAsset asset,
      CancellationToken cancellationToken = default)
   {
      ArgumentNullException.ThrowIfNull(asset);
      var existing = await _context.SiteAssets
         .SingleOrDefaultAsync(e => e.Id == asset.Id, cancellationToken);
      if (existing is null)
      {
         return false;
      }

      existing.AltText = asset.AltText;
      existing.AssetKey = asset.AssetKey;
      existing.ContentType = asset.ContentType;
      existing.HeightPixels = asset.HeightPixels;
      existing.WidthPixels = asset.WidthPixels;
      existing.OriginalFileName = asset.OriginalFileName;
      existing.SizeBytes = asset.SizeBytes;

      await _context.SaveChangesAsync(cancellationToken);
      return true;
   }

   public async Task InsertAssetAsync(
      SiteAsset asset,
      CancellationToken cancellationToken = default)
   {
      ArgumentNullException.ThrowIfNull(asset);
      await _context.SiteAssets.AddAsync(asset, cancellationToken);
      await _context.SaveChangesAsync(cancellationToken);
   }

   private async Task ValidateFeaturedPostAsync(
      int? featuredPostId,
      CancellationToken cancellationToken)
   {
      if (featuredPostId is null)
      {
         return;
      }

      bool postExists = await _context.Posts.AnyAsync(
         post => post.Id == featuredPostId.Value, cancellationToken);

      if (!postExists)
      {
         throw new InvalidOperationException(
            "The selected featured post does not exist.");
      }
   }

   private readonly ILogger<SqlUncategorizedStore> _logger;
   private readonly SiteDbContext _context;

}
