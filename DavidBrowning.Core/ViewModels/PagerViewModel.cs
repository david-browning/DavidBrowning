// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.

namespace DavidBrowning.ViewModels;

public sealed class PagerViewModel
{
   public PagerViewModel(
      int currentPage,
      int totalPages,
      string controller,
      string indexAction,
      string pageAction)
   {
      if (currentPage < 1)
      {
         throw new ArgumentOutOfRangeException(
            nameof(currentPage),
            "Current page must be greater than or equal to 1.");
      }

      if (totalPages < 0)
      {
         throw new ArgumentOutOfRangeException(
            nameof(totalPages), "Total pages cannot be negative.");
      }

      ArgumentException.ThrowIfNullOrWhiteSpace(controller);
      ArgumentException.ThrowIfNullOrWhiteSpace(indexAction);
      ArgumentException.ThrowIfNullOrWhiteSpace(pageAction);

      CurrentPage = currentPage;
      TotalPages = totalPages;
      Controller = controller;
      IndexAction = indexAction;
      PageAction = pageAction;
      VisiblePages = GetVisiblePages(currentPage, totalPages);
   }

   public int CurrentPage { get; }

   public int TotalPages { get; }

   public string Controller { get; }

   public string IndexAction { get; }

   public string PageAction { get; }

   public IReadOnlyList<int> VisiblePages { get; }

   public bool HasPreviousPage => CurrentPage > 1;

   public bool HasNextPage => CurrentPage < TotalPages;

   public bool ShowLeadingEllipsis =>
      VisiblePages.Count > 0 && VisiblePages[0] > 1;

   public bool ShowTrailingEllipsis =>
      VisiblePages.Count > 0 && VisiblePages[^1] < TotalPages;

   private static IReadOnlyList<int> GetVisiblePages(
      int currentPage,
      int totalPages)
   {
      if (totalPages == 0)
      {
         return Array.Empty<int>();
      }

      const int visiblePageCount = 5;
      var firstPage = Math.Max(1, currentPage - (visiblePageCount / 2));
      var lastPage = Math.Min(totalPages, firstPage + visiblePageCount - 1);
      firstPage = Math.Max(1, lastPage - visiblePageCount + 1);
      return Enumerable.Range(firstPage, lastPage - firstPage + 1).ToList();
   }
}