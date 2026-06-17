// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.
using System.Collections;
using System.ComponentModel.DataAnnotations;

namespace DavidBrowning.Infrastructure.Validators;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
public sealed class CollectionCountAttribute : ValidationAttribute
{
   private const int _noMaximum = -1;

   private int _maximumCount = _noMaximum;

   public CollectionCountAttribute(int minimumCount)
   {
      if (minimumCount < 0)
      {
         throw new ArgumentOutOfRangeException(
            nameof(minimumCount),
            "Minimum count cannot be negative.");
      }

      MinimumCount = minimumCount;
   }

   public int MinimumCount { get; }

   public int MaximumCount
   {
      get => _maximumCount;
      set
      {
         if (value < 0)
         {
            throw new ArgumentOutOfRangeException(
               nameof(value),
               "Maximum count cannot be negative.");
         }

         _maximumCount = value;
      }
   }

   public override bool IsValid(object? value)
   {
      if (MaximumCount != _noMaximum && MaximumCount < MinimumCount)
      {
         throw new InvalidOperationException(
            "Maximum count cannot be less than minimum count.");
      }

      int count = value is null ? 0 : GetCount(value);

      if (count < MinimumCount)
      {
         return false;
      }

      if (MaximumCount != _noMaximum && count > MaximumCount)
      {
         return false;
      }

      return true;
   }

   public override string FormatErrorMessage(string name)
   {
      if (!string.IsNullOrEmpty(ErrorMessage))
      {
         return ErrorMessage;
      }

      if (MaximumCount == _noMaximum)
      {
         return MinimumCount == 1
            ? $"{name} must contain at least one item."
            : $"{name} must contain at least {MinimumCount} items.";
      }

      return $"{name} must contain between {MinimumCount} and " +
         $"{MaximumCount} items.";
   }

   private static int GetCount(object value)
   {
      if (value is ICollection collection)
      {
         return collection.Count;
      }

      if (value is IEnumerable enumerable)
      {
         int count = 0;

         foreach (object? _ in enumerable)
         {
            count++;
         }

         return count;
      }

      throw new InvalidOperationException(
         $"{nameof(CollectionCountAttribute)} can only validate collections.");
   }
}