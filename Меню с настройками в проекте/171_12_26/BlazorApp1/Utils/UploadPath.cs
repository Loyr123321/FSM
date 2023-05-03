namespace BlazorApp1.Utils
{
    public static class UploadPath
    {
        public const long MAX_FILE_SIZE = 104857600; //100mb

        public static readonly IConfiguration configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
        public static readonly string BaseUrl = configuration.GetValue<string>("BaseUrl");
        public static readonly string UploadUrl = BaseUrl + @"/UploadDir/";

        public static readonly string RootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
        public static readonly string TempDir = Path.Combine(RootPath, "TempDir");
        public static readonly string UploadDir = Path.Combine(RootPath, "UploadDir");

        public static void CreateDirectories()
        {
            Directory.CreateDirectory(TempDir);
            Directory.CreateDirectory(UploadDir);
        }

        public static string GetFilePath(string fileName)
        {
            return UploadDir + fileName;
        }

        public static void MoveFiles(string[] files)
        {
            foreach (var file in files)
            {
                if (File.Exists(file))
                {
                    try
                    {
                        //Копирование файлов начинать после успешного апдейта Orederа со статусом ЗАВЕРШЕНО
                        //CopyFile
                        File.Delete(file);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("RemoveFiles_Exception: " + ex.Message);
                    }
                }
            }
        }

        public static bool IsImage(string file)
        {
            var imageFileTypes = new List<string> { ".tiff", ".jfif", ".bmp", ".pjp", ".apng", ".gif", ".svg", ".png", ".xbm", ".dib", ".jxl", ".jpeg", ".svgz", ".jpg", ".webp", ".ico", ".tif", ".pjpeg", ".avif" };
            bool result = imageFileTypes.Any(x => x == Path.GetExtension(file));
            return result;
        }
    }
}
