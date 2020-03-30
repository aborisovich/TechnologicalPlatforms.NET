using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

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
            {
                // work-around for .NET failures to remove Read-Only folders
                DirectoryInfo directoryInfo = new DirectoryInfo(directoryPath);
                directoryInfo.Attributes = FileAttributes.Normal;
                foreach(var folder in directoryInfo.GetDirectories())
                    folder.Attributes = FileAttributes.Normal;
                foreach (var file in directoryInfo.GetFiles())
                    file.Attributes = FileAttributes.Normal;
                Directory.Delete(directoryPath, true);
            }
                
        }

        public static void CreateFile(string filePath, FileAttributes attributes)
        {
            var stream = File.Create(filePath);
            stream.Close();
            new FileInfo(filePath).Attributes = attributes;
        }

        public static void CreateDirectory(string directoryPath, FileAttributes attributes)
        {
            Directory.CreateDirectory(directoryPath);
            new DirectoryInfo(directoryPath).Attributes = attributes;
        }

        public List<FileAttributes> GetFileAttributes(string filePath)
        {
            FileAttributes attributes = File.GetAttributes(filePath);
            return AttributesToList(attributes);
        }

        public List<FileAttributes> GetFolderAttributes(string directoryPath)
        {
            FileAttributes attributes = new DirectoryInfo(directoryPath).Attributes;
            return AttributesToList(attributes);
        }

        private List<FileAttributes> AttributesToList(FileAttributes attributes)
        {
            List<FileAttributes> attributeList = new List<FileAttributes>();
            if ((attributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
                attributeList.Add(FileAttributes.ReadOnly);
            if ((attributes & FileAttributes.Archive) == FileAttributes.Archive)
                attributeList.Add(FileAttributes.Archive);
            if ((attributes & FileAttributes.Hidden) == FileAttributes.Hidden)
                attributeList.Add(FileAttributes.Hidden);
            if ((attributes & FileAttributes.System) == FileAttributes.System)
                attributeList.Add(FileAttributes.System);
            return attributeList;
        }
    }
}
