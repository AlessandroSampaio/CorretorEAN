using System;
using System.IO;

namespace CorretorEAN
{
    public class Log
    {
        private readonly static string basePath = AppDomain.CurrentDomain.BaseDirectory + "log";

        private static FileStream CreateFile(DateTime time)
        {
            string Path = basePath + time.Day + time.Month + time.Year + "_" + time.Hour + time.Minute + ".txt";
            if (File.Exists(Path))
            {
                return new FileStream(Path, FileMode.Append, FileAccess.Write);
            }
            else
            {
                return new FileStream(Path, FileMode.Create, FileAccess.Write);
            }
        }

        public static void AppendFile(Produto produto, DateTime time)
        {
            using (FileStream fs = CreateFile(time))
            {
                using (StreamWriter sw = new StreamWriter(fs))
                {
                    sw.WriteLine(produto.ToString());
                }
            }
        }
    }
}
