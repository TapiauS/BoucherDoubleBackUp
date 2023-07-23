namespace Boucher_Double_Back_End.Models.ServerModel
{
    public class UtilitaryClass
    {
        public static string GetContentType(string extension)
        {
            switch (extension.ToLower())
            {
                case ".jpg":
                case ".jpeg":
                    return "image/jpeg";
                case ".png":
                    return "image/png";
                case ".gif":
                    return "image/gif";
                // Add more mappings for other image formats as needed
                default:
                    return "application/octet-stream"; // Default content type
            }
        }

    }
}
