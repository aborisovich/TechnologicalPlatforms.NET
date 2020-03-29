using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Browser
{
    public class Filesystem
    {
        public DirectoryInfo RootDirectory { get; }

        public Filesystem(string rootDirectoryPath)
        {
            if (!Directory.Exists(rootDirectoryPath))
                throw new DirectoryNotFoundException(rootDirectoryPath);
            RootDirectory = new DirectoryInfo(rootDirectoryPath);
        }

        public DirectoryInfo[] GetSubdirectories(string directoryPath)
        {
            DirectoryInfo directory = new DirectoryInfo(directoryPath);
            return directory.GetDirectories("*", SearchOption.TopDirectoryOnly);
        }

        public FileInfo[] GetFiles(string directoryPath)
        {
            DirectoryInfo directory = new DirectoryInfo(directoryPath);
            return directory.GetFiles("*", SearchOption.TopDirectoryOnly);
        }

        public void DeleteFile(string filePath)
        {
            if(File.Exists(filePath))
                File.Delete(filePath);
        }

        public void DeleteFolder(string directoryPath)
        {
            if (Directory.Exists(directoryPath))
                Directory.Delete(directoryPath, true);
        }
    }
}
