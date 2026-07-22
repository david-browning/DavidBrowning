// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.

using System.Diagnostics.CodeAnalysis;
using DavidBrowning.Models;
using DavidBrowning.Models.Projects;
using DavidBrowning.Models.Writing;

namespace DavidBrowning.Models.Published;

public sealed class PublishedLookup
{
   public required string Slug { get; set; }

   public required string DisplayName { get; set; }

   public string? Description { get; set; }

   public PublishedLookup()
   {

   }

   [SetsRequiredMembers]
   public PublishedLookup(ProjectStatus status)
      : this(status.Slug, status.DisplayName, status.Description)
   {

   }

   [SetsRequiredMembers]
   public PublishedLookup(ProjectType type)
      : this(type.Slug, type.DisplayName, type.Description)
   {

   }

   [SetsRequiredMembers]
   public PublishedLookup(ProjectOrigin origin)
      : this(origin.Slug, origin.DisplayName, origin.Description)
   {

   }

   [SetsRequiredMembers]
   public PublishedLookup(ProjectTag tag)
      : this(tag.Slug, tag.DisplayName, tag.Description)
   {

   }

   [SetsRequiredMembers]
   public PublishedLookup(ProjectStackTag tag)
      : this(tag.Slug, tag.DisplayName, tag.Description)
   {

   }

   [SetsRequiredMembers]
   public PublishedLookup(PostStyle style)
      : this(style.Slug, style.DisplayName, style.Description)
   {

   }

   [SetsRequiredMembers]
   public PublishedLookup(WritingTag tag)
      : this(tag.Slug, tag.DisplayName, null)
   {

   }

   [SetsRequiredMembers]
   private PublishedLookup(
      string slug,
      string displayName,
      string? description)
   {
      Slug = slug;
      DisplayName = displayName;
      Description = description;
   }
}

public sealed class PublishedProjectLinkType
{
   public required string Slug { get; set; }

   public required string DisplayName { get; set; }

   public string? Description { get; set; }

   public string? IconCssClass { get; set; }

   public PublishedProjectLinkType()
   {

   }

   [SetsRequiredMembers]
   public PublishedProjectLinkType(ProjectLinkType linkType)
   {
      ArgumentNullException.ThrowIfNull(linkType);

      Slug = linkType.Slug;
      DisplayName = linkType.DisplayName;
      Description = linkType.Description;
      IconCssClass = linkType.IconCssClass;
   }
}

public sealed class PublishedProjectLink
{
   public required PublishedProjectLinkType ProjectLinkType { get; set; }

   public required string Label { get; set; }

   public required string Url { get; set; }

   public int SortOrder { get; set; }

   public PublishedProjectLink()
   {

   }

   [SetsRequiredMembers]
   public PublishedProjectLink(ProjectLink link)
   {
      ArgumentNullException.ThrowIfNull(link);

      ProjectLinkType = new PublishedProjectLinkType(
         link.ProjectLinkType ?? throw new InvalidOperationException(
            $"Project link '{link.Label}' is missing its link type."));
      Label = link.Label;
      Url = link.Url;
      SortOrder = link.SortOrder;
   }
}

public sealed class PublishedProjectTagLink
{
   public required PublishedLookup ProjectTag { get; set; }

   public PublishedProjectTagLink()
   {

   }

   [SetsRequiredMembers]
   public PublishedProjectTagLink(ProjectTagLink link)
   {
      ArgumentNullException.ThrowIfNull(link);

      ProjectTag = new PublishedLookup(
         link.ProjectTag ?? throw new InvalidOperationException(
            "Project tag link is missing its project tag."));
   }
}

public sealed class PublishedProjectStackTagLink
{
   public required PublishedLookup ProjectStackTag { get; set; }

   public PublishedProjectStackTagLink()
   {

   }

   [SetsRequiredMembers]
   public PublishedProjectStackTagLink(ProjectStackTagLink link)
   {
      ArgumentNullException.ThrowIfNull(link);

      ProjectStackTag = new PublishedLookup(
         link.ProjectStackTag ?? throw new InvalidOperationException(
            "Project stack tag link is missing its stack tag."));
   }
}

public sealed class PublishedPostTag
{
   public required PublishedLookup WritingTag { get; set; }

   public PublishedPostTag()
   {

   }

   [SetsRequiredMembers]
   public PublishedPostTag(PostTag tag)
   {
      ArgumentNullException.ThrowIfNull(tag);

      WritingTag = new PublishedLookup(
         tag.WritingTag ?? throw new InvalidOperationException(
            "Post tag is missing its writing tag."));
   }
}

public sealed class PublishedAssetReference
{
   public required string ReferenceKey { get; set; }

   public required string AssetKey { get; set; }

   public string? Caption { get; set; }

   public string? AltText { get; set; }

   public PublishedAssetReference()
   {

   }

   [SetsRequiredMembers]
   public PublishedAssetReference(PostRevisionAssetLink link)
   {
      ArgumentNullException.ThrowIfNull(link);

      var asset = link.SiteAsset ?? throw new InvalidOperationException(
         "Post revision asset link is missing its site asset.");

      ReferenceKey = link.ReferenceKey;
      AssetKey = asset.AssetKey;
      Caption = link.Caption;
      AltText = link.AltTextOverride ?? asset.AltText;
   }

   [SetsRequiredMembers]
   public PublishedAssetReference(ProjectAssetLink link)
   {
      ArgumentNullException.ThrowIfNull(link);

      string referenceKey = link.ReferenceKey ??
         throw new InvalidOperationException(
            "Project asset link does not have a reference key.");

      if (string.IsNullOrWhiteSpace(referenceKey))
      {
         throw new InvalidOperationException(
            "Project asset link has an empty reference key.");
      }

      var asset = link.SiteAsset ?? throw new InvalidOperationException(
         "Project asset link is missing its site asset.");

      ReferenceKey = referenceKey;
      AssetKey = asset.AssetKey;
      Caption = link.Caption;
      AltText = link.AltTextOverride ?? asset.AltText;
   }
}

public sealed class PublishedAssetBlock
{
   public required string AssetKey { get; set; }

   public required string ContentType { get; set; }

   public string? Caption { get; set; }

   public string? AltText { get; set; }

   public int? WidthPixels { get; set; }

   public int? HeightPixels { get; set; }

   public PublishedAssetBlock()
   {

   }

   [SetsRequiredMembers]
   public PublishedAssetBlock(ProjectAssetLink link)
   {
      ArgumentNullException.ThrowIfNull(link);

      var asset = link.SiteAsset ?? throw new InvalidOperationException(
         "Project asset link is missing its site asset.");

      AssetKey = asset.AssetKey;
      ContentType = asset.ContentType;
      Caption = link.Caption;
      AltText = link.AltTextOverride ?? asset.AltText;
      WidthPixels = asset.WidthPixels;
      HeightPixels = asset.HeightPixels;
   }

   [SetsRequiredMembers]
   public PublishedAssetBlock(PostRevisionAssetLink link)
   {
      ArgumentNullException.ThrowIfNull(link);

      var asset = link.SiteAsset ?? throw new InvalidOperationException(
         "Post revision asset link is missing its site asset.");

      AssetKey = asset.AssetKey;
      ContentType = asset.ContentType;
      Caption = link.Caption;
      AltText = link.AltTextOverride ?? asset.AltText;
      WidthPixels = asset.WidthPixels;
      HeightPixels = asset.HeightPixels;
   }
}

public sealed class PublishedWritingSummary
{
   public required string Slug { get; set; }

   public required string Title { get; set; }

   public string? Subtitle { get; set; }

   public string? Summary { get; set; }

   public required PublishedLookup PostStyle { get; set; }

   public IReadOnlyList<PublishedPostTag> Tags { get; set; } =
      Array.Empty<PublishedPostTag>();

   public PublishedWritingSummary()
   {

   }

   [SetsRequiredMembers]
   public PublishedWritingSummary(Post post)
   {
      ArgumentNullException.ThrowIfNull(post);

      Slug = post.Slug;
      Title = post.Title;
      Subtitle = post.Subtitle;
      Summary = post.Summary;
      PostStyle = new PublishedLookup(
         post.PostStyle ?? throw new InvalidOperationException(
            $"Writing '{post.Slug}' is missing its post style."));
      Tags = post.Tags
         .Where(tag => tag.WritingTag is not null)
         .OrderBy(tag => tag.WritingTag!.DisplayName)
         .Select(tag => new PublishedPostTag(tag))
         .ToArray();
   }
}

public sealed class PublishedProjectPost
{
   public required PublishedWritingSummary Post { get; set; }

   public string? RelationshipLabel { get; set; }

   public int SortOrder { get; set; }

   public PublishedProjectPost()
   {

   }

   [SetsRequiredMembers]
   public PublishedProjectPost(ProjectPost link)
   {
      ArgumentNullException.ThrowIfNull(link);

      Post = new PublishedWritingSummary(
         link.Post ?? throw new InvalidOperationException(
            "Project-post link is missing its writing post."));
      RelationshipLabel = link.RelationshipLabel;
      SortOrder = link.SortOrder;
   }
}

public sealed class PublishedTextContent
{
   public required string CacheKey { get; set; }

   public ContentFormat ContentFormat { get; set; }

   public string? CreatedBy { get; set; }

   public string? Content { get; set; }

   public IReadOnlyList<PublishedAssetReference> AssetLinks { get; set; } =
      Array.Empty<PublishedAssetReference>();

   public PublishedTextContent()
   {

   }

   [SetsRequiredMembers]
   public PublishedTextContent(PostRevision revision)
   {
      ArgumentNullException.ThrowIfNull(revision);

      CacheKey = $"post-revision:{revision.Id}";
      ContentFormat = revision.ContentFormat;
      CreatedBy = revision.CreatedBy;
      Content = revision.Content;
      AssetLinks = revision.AssetLinks
         .Where(link => link.SiteAsset is not null)
         .Select(link => new PublishedAssetReference(link))
         .ToArray();
   }

   [SetsRequiredMembers]
   public PublishedTextContent(
      Project project,
      string content,
      string publicationVersion)
   {
      ArgumentNullException.ThrowIfNull(project);
      ArgumentNullException.ThrowIfNull(content);
      ArgumentException.ThrowIfNullOrWhiteSpace(
         publicationVersion);

      CacheKey =
         $"published:{publicationVersion}:project:{project.Slug}";

      ContentFormat = DavidBrowning.Models.ContentFormat.Markdown;
      CreatedBy = null;
      Content = content;

      AssetLinks = project.AssetLinks
         .Where(link =>
            !string.IsNullOrWhiteSpace(link.ReferenceKey) &&
            link.SiteAsset is not null)
         .OrderBy(link => link.SortOrder)
         .Select(link => new PublishedAssetReference(link))
         .ToArray();
   }
}
