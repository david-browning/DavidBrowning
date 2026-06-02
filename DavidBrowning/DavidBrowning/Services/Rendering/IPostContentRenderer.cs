// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DavidBrowning.Models;
using DavidBrowning.Models.ViewModels;
using DavidBrowning.Models.Writing;

namespace DavidBrowning.Services.Rendering;

public interface IPostContentRenderer
{
   Task<RenderedContent> RenderAsync(
      PostRevision revision,
      IReadOnlyCollection<SiteAssetLink> assetLinks,
      CancellationToken cancellationToken = default);
}
