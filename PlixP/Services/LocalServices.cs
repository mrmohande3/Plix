using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace PlixP.Services
{

    public class LocalServices
    {
        private string[] extentions = { ".mp4", ".mkv", ".avi" };
        private List<DirectoryInfo> _baseDires = new List<DirectoryInfo>();
        private List<string> paths = new List<string>();
        public LocalServices()
        {

            var pathsJson = Plugin.Settings.CrossSettings.Current.GetValueOrDefault("Paths", string.Empty);
            paths = Newtonsoft.Json.JsonConvert.DeserializeObject<List<string>>(pathsJson);
            if (paths == null)
                paths = new List<string>();
            if (paths.Count > 0)
                paths.ForEach(p => _baseDires.Add(new DirectoryInfo(p)));
        }

        public void ResetFolder()
        {
            RenameAllFile(GetMovieFiles());
            CreateUnFolders(GetMovieFiles());
            RenameAllFolder();
        }

        public string GetFolderNameByMovieFileName(string movieN)
        {
            string movieName = string.Empty;
            bool yeared = false;
            bool qualited = false;
            movieN = movieN.Replace('-', '.');
            movieN = movieN.Replace('_', '.');
            foreach (var str in movieN.Split('.'))
            {
                int year = 0;
                if (int.TryParse(str, out year) && (year >= 1920 && year <= 2142))
                {
                    movieName += "(" + year + ")" + " ";
                    yeared = true;
                }
                else if (str.Contains("720") || str.Contains("1080") || str.ToLower().Contains("dvdsrc") || str.ToLower().Contains("hdrip") || str.ToLower().Contains("dvdrip"))
                {
                    movieName += "[" + str + "]";
                    qualited = true;
                }
                else if (!yeared && !qualited)
                {
                    movieName += str + " ";
                }
            }

            return movieName;
        }
        private void RenameAllFolder()
        {
            try
            {
                if (paths.Count > 0)
                {
                    foreach (var path in paths)
                    {
                        DirectoryInfo directoryInfo = new DirectoryInfo(path);
                        string baseDire = directoryInfo.Name;
                        var dires = GetDirectoryInfos(directoryInfo);
                        foreach (var dir in dires)
                        {
                            string movieN = string.Empty;
                            string movieName = string.Empty;
                            foreach (var fileInfo in dir.GetFiles())
                            {
                                if (fileInfo.Name.Contains(extentions[0]) || fileInfo.Name.Contains(extentions[1]) || fileInfo.Name.Contains(extentions[2]))
                                    movieN = fileInfo.Name;
                            }

                            if (!string.IsNullOrEmpty(movieN))
                            {
                                movieName = GetFolderNameByMovieFileName(movieN);
                                DirectoryInfo di = new DirectoryInfo(string.Concat(dir.Parent, "\\", movieName));
                                if (!di.Exists)
                                    Directory.Move(dir.FullName, string.Concat(dir.Parent, "\\", movieName));
                            }

                        }
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }
        public List<FileInfo> GetMovieFiles()
        {
            if (paths.Count > 0)
            {
                var allFiles = new List<FileInfo>();
                foreach (var path in paths)
                {
                    DirectoryInfo dir = new DirectoryInfo(path);
                    string baseDire = dir.Name;
                    var dires = GetDirectoryInfos(dir);
                    List<FileInfo> files = new List<FileInfo>();
                    foreach (var directoryInfo in dires)
                    {
                        files.AddRange(directoryInfo.GetFiles());
                    }
                    allFiles.AddRange(files);
                }
                return allFiles.Where(f => extentions.Contains(f.Extension)).ToList();
            }
            else
                return new List<FileInfo>();
        }
        public void RenameAllFile(List<FileInfo> fileInfos)
        {
            fileInfos.ForEach(m =>
            {
                string newName = "";
                foreach (var str in m.DirectoryName.Split("\\"))
                {
                    newName += str + "\\";
                }
                var name = m.Name.Replace('_', '.');
                name = name.Replace(' ', '.');
                newName += name;
                File.Move(m.FullName, newName);
            });
        }
        public void CreateUnFolders(List<FileInfo> movies)
        {
            foreach (var baseDire in _baseDires)
            {

                movies.Where(m => m.DirectoryName.Split('\\').Last() == baseDire.Name).ToList().ForEach(m =>
                {
                    try
                    {
                        string movieName = "";
                        bool yeared = false;
                        bool qualited = false;
                        foreach (var str in m.Name.Split('.'))
                        {
                            int year = 0;
                            if (int.TryParse(str, out year) && (year >= 1920 && year <= 2142))
                            {
                                movieName += "(" + year + ")" + " ";
                                yeared = true;
                            }
                            else if (str.Contains("720") || str.Contains("1080") || str.ToLower().Contains("dvdsrc") || str.ToLower().Contains("hdrip") || str.ToLower().Contains("dvdrip"))
                            {
                                movieName += "[" + str + "]";
                                qualited = true;
                            }
                            else if (!yeared && !qualited && !extentions.Contains(str))
                            {
                                movieName += str + " ";

                            }
                        }

                        Directory.CreateDirectory(m.DirectoryName + "\\" + movieName);
                        File.Move(m.FullName, m.DirectoryName + "\\" + movieName + "\\" + m.Name);

                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                        MessageBox.Show(e.Message);
                    }
                });
            }
        }
        public List<DirectoryInfo> GetDirectoryInfos(DirectoryInfo baseDirectoryInfo)
        {
            try
            {
                List<DirectoryInfo> directoryInfos = new List<DirectoryInfo>();
                var dires = baseDirectoryInfo.GetDirectories();
                if (dires.Length > 0)
                {
                    foreach (var directory in dires)
                    {
                        directoryInfos.AddRange(GetDirectoryInfos(directory));
                    }
                }
                directoryInfos.Add(baseDirectoryInfo);
                return directoryInfos;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return new List<DirectoryInfo>();
            }
        }
        public void RenameFile(FileInfo oldFile, FileInfo newFile)
        {
            File.Move(oldFile.FullName, newFile.FullName);
        }
        public void RenameDirection(DirectoryInfo oldDirectory, DirectoryInfo newDirectory)
        {
            Directory.Move(oldDirectory.FullName, newDirectory.FullName);
        }
    }
}
