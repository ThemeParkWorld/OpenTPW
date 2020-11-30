using Engine.Utils.DebugUtils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Engine.Utils.FileUtils.FileSystems
{
    public class ArchiveFileSystem : IFileSystem
    {
        private Dictionary<string, FileArchive> MountPoints { get; set; } = new Dictionary<string, FileArchive>();

        public void Init(string contentPath)
        {
            // Load asset packages in parallel. 1 thread for each
            var archiveList = JsonConvert.DeserializeObject<List<string>>(File.ReadAllText($"{contentPath}/archives.json"));
            
            // Next, spawn a read thread for each archive
            foreach (var archive in archiveList)
            {
                //var loadThread = new Thread(LoadThread);
                //loadThread.Start($"Content/{archive}.alex");

                LoadThread($"Content/{archive}.alex");
            }
        }

        private void LoadThread(object archivePath_)
        {
            var archivePath = (string)archivePath_;

            var mountPoint = Path.GetFileNameWithoutExtension(archivePath);
            if (MountPoints.ContainsKey(mountPoint))
            {
                Logging.Log($"{mountPoint} already exists as a mount point.", Logging.Severity.High);
                return;
            }

            MountPoints.Add(mountPoint, FileArchive.LoadFromFile(archivePath));
        }

        private (string, string) ParseMountPoint(string path)
        {
            /*
             * Mount point is always /mountpoint/path
             * Examples:
             *  - /Models/rainier/scene.gltf
             *  - /Scripts/Player.cs
             */

            if (path.StartsWith("/"))
                path = path.Substring(1);

            var mountPoint = path.Substring(0, path.IndexOf("/"));
            var fsPath = path.Substring(path.IndexOf("/") + 1);

            return (mountPoint, fsPath);
        }

        public Asset GetAsset(string path)
        {
            var parsedMountPoint = ParseMountPoint(path.Replace("\\", "/"));
            var mountPoint = parsedMountPoint.Item1;
            var fsPath = parsedMountPoint.Item2;
            if (!MountPoints.ContainsKey(mountPoint))
            {
                Logging.Log($"{parsedMountPoint} isn't mounted.", Logging.Severity.High);
                return Asset.Empty;
            }

            var mountedArchive = MountPoints[mountPoint];

            var files = mountedArchive.Files.Where(file => file.FileName.Replace("\\", "/").Equals(fsPath, StringComparison.CurrentCultureIgnoreCase));
            if (files.Count() == 0)
            {
                Logging.Log($"{path} wasn't found.", Logging.Severity.High);
                return Asset.Empty;
            }

            var file = files.First();
            return new Asset(path, file.FileData);
        }
    }
}
