// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.
namespace DavidBrowning.Models.Writing
{
   /// <summary>
   /// What kind of content the writing source is. The content can be rendered 
   /// differently by choosing a rendering mode.
   /// </summary>
   public enum PostContentFormat : byte
   {
      PlainText = 0,
      Markdown = 1,
      Html = 2,
      Latex = 3,
      ExternalBlob = 4
   }
}
