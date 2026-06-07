// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.
using System.Text;
using System.Xml;

namespace DavidBrowning.Infrastructure.Seo;
public sealed class SitemapBuilder
{
   public SitemapBuilder(UrlBuilder urlBuilder)
   {
      _urlBuilder = urlBuilder;
   }

   public string GenerateXml(IEnumerable<SitemapEntry> entries)
   {
      var settings = new XmlWriterSettings
      {
         Encoding = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false),
         Indent = true,
         OmitXmlDeclaration = false,
      };

      var strBuilder = new StringBuilder();
      using (var writer = XmlWriter.Create(strBuilder, settings))
      {
         writer.WriteStartDocument();
         writer.WriteStartElement(
            prefix: null,
            localName: "urlset",
            ns: "http://www.sitemaps.org/schemas/sitemap/0.9");

         foreach (var entry in entries)
         {
            writer.WriteStartElement("url");
            writer.WriteElementString(
               "loc", _urlBuilder.GetAbsoluteUrl(entry.RelativePath));

            if (entry.LastModifiedUtc is not null)
            {
               writer.WriteElementString(
                  "lastmod",
                  entry.LastModifiedUtc.Value.ToUniversalTime()
                     .ToString("yyyy-MM-dd'T'HH:mm:ss'Z'"));
            }

            writer.WriteEndElement();
         }

         writer.WriteEndElement();
         writer.WriteEndDocument();
         writer.Flush();
      }

      return strBuilder.ToString();
   }

   private readonly UrlBuilder _urlBuilder;
}
