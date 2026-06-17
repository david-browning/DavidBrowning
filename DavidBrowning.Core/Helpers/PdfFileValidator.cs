using Microsoft.AspNetCore.Http;

namespace DavidBrowning.Helpers;
public sealed class PdfFileValidator
{
   public static async Task<bool> HasPdfSignatureAsync(
      IFormFile file,
      CancellationToken cancellationToken = default)
   {
      ArgumentNullException.ThrowIfNull(file);
      if (file.Length < _pdfSignature.Length)
      {
         return false;
      }

      var buffer = new byte[_pdfSignature.Length];
      await using var stream = file.OpenReadStream();
      await stream.ReadExactlyAsync(buffer, cancellationToken);
      return buffer.SequenceEqual(_pdfSignature);
   }

   private static readonly byte[] _pdfSignature =
   [
      0x25, // %
      0x50, // P
      0x44, // D
      0x46, // F
      0x2D, // -
   ];
}
