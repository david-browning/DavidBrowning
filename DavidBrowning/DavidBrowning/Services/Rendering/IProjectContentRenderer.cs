using System.Threading;
using System.Threading.Tasks;
using DavidBrowning.Models.Projects;
using DavidBrowning.Models.ViewModels;

namespace DavidBrowning.Services.Rendering;

public interface IProjectContentRenderer
{
   Task<RenderedContent?> RenderAsync(
      Project project,
      CancellationToken cancellationToken = default);
}