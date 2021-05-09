using System;
using System.Collections.Generic;
using System.IO;
using Engine.Assets;
using ImGuiNET;

namespace Engine.Gui.Managers.ImGuiWindows
{
    public class FileBrowserWindow : ImGuiWindow
    {
        private struct DirListEntry
        {
            public DirListEntry(string path, Type entryType)
            {
                Path = path;
                EntryType = entryType;
            }

            public string Path { get; set; }

            public enum Type
            {
                File,
                Folder
            }
            
            public Type EntryType { get; set; }
        }
        
        public override bool Render { get; set; } = true;
        public override string Title { get; }
        public override string IconGlyph { get; }

        private string currentDirectory = @"C:\";
        
        public string CurrentDirectory
        {
            get => currentDirectory;
            set
            {
                currentDirectory = value;   
                RefreshListings();
            }
        }
        
        private int currentItem;

        private DirListEntry[] dirListing;
        private string[] displayDirListing;

        public FileBrowserWindow(string defaultDirectory = @"C:\")
        {
            currentDirectory = defaultDirectory;
            
            RefreshListings();
        }

        private void RefreshListings()
        {
            dirListing = GetDirListing();
            displayDirListing = new string[dirListing.Length];
            for (int i = 0; i < dirListing.Length; i++)
            {
                var entry = dirListing[i];
                var icon = (entry.EntryType == DirListEntry.Type.File) ? FontAwesome5.File : FontAwesome5.Folder;
                displayDirListing[i] = $"{icon} {Path.GetFileName(entry.Path)}";
            }
        }
        
        public override void Draw()
        {
            ImGui.Text(CurrentDirectory);
            ImGui.BeginChild(CurrentDirectory);
            
            ImGui.SetNextItemWidth(-1);
            // File / folder list
            if (ImGui.ListBox("##hideLabel#dirListing", ref currentItem, displayDirListing, dirListing.Length, dirListing.Length))
            {
                string target = dirListing[currentItem].Path;
                if (target == "..")
                    target = Directory.GetParent(CurrentDirectory)?.FullName ?? @"C:\";
                else 
                    target = Path.Combine(CurrentDirectory, target);
                
                if (Directory.Exists(target))
                    CurrentDirectory = target;
            }
            
            ImGui.EndChild();
        }

        private DirListEntry[] GetDirListing()
        {
            List<DirListEntry> newDirListing = new()
            {
                new DirListEntry("..", DirListEntry.Type.Folder)
            };
            
            foreach (var directory in Directory.GetDirectories(CurrentDirectory))
                newDirListing.Add(new DirListEntry(directory, DirListEntry.Type.Folder));
            
            foreach (var file in Directory.GetFiles(CurrentDirectory))
                newDirListing.Add(new DirListEntry(file, DirListEntry.Type.File));

            return newDirListing.ToArray();
        }
    }
}