namespace DavidBrowning.Admin.ViewModels;

public static class AdminOffcanvasIds
{
   public static string Body(string id)
   {
      return $"{id}-body";
   }

   public static string Overlay(string id)
   {
      return $"{id}-overlay";
   }

   public static string Title(string id)
   {
      return $"{id}-title";
   }

   public static string Selector(string id)
   {
      return $"#{id}";
   }

   public static string BodySelector(string id)
   {
      return $"#{Body(id)}";
   }

   public static string OverlaySelector(string id)
   {
      return $"#{Overlay(id)}";
   }
}