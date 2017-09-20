namespace MarvelBot.Classes
{
    public static class Helpers
    {
        //public static string ImagePathBuilder(string path, string extension, string imageFormat)
        //{
        //    StringBuilder imageBuilder = new StringBuilder();
        //    imageBuilder.Append(path);
        //    imageBuilder.Append("/" + imageFormat);
        //    imageBuilder.Append("." + extension);

        //    return imageBuilder.ToString();
        //}

        //public static string ImagePathBuilder(string path, string extension, string imageFormat) =>
        //    path + "/" + imageFormat + "." + extension;

        public static string ImagePathBuilder(string path, string extension, string imageFormat) =>
           $"{path}/{imageFormat}.{extension}";
    }
}