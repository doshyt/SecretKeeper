using System.IO;


namespace SecretKeeper.Engine
{
    public static class FileOperator
    {
        public static int DeleteUploadedFile(string path)
        {
            if (File.Exists(path))
            {
                try
                {
                    File.Delete(path);
                }
                catch
                {
                    return -1;
                }
            }
            return 0;
        }
    }
}
