using System.Threading;
using System.Threading.Tasks;
using DavidBrowning.Models.ViewModels;
using DavidBrowning.Services.Assets;

namespace DavidBrowning.Services.Rendering
{
   public sealed class BasicContentRenderer : IContentRenderer
   {
      public Task<RenderedContent> RenderAsync(
         StoredContent content,
         CancellationToken cancellationToken = default)
      {
         var ret = new RenderedContent()
         {
            AssetKey = content.AssetKey,
            Html = content.SourceText,
            OriginalSourceFormat = ContentSourceFormat.PlainText
         };

         return Task.FromResult(ret);
      }
   }
}
