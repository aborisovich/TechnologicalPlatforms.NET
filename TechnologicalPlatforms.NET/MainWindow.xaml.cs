using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Cache;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Browser
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Filesystem filesystem;

        public MainWindow()
        {
            InitializeComponent();
        }

        protected virtual void MenuExitClick(object sender, RoutedEventArgs eventArgs)
        {
            this.Close();
        }

        protected virtual void MenuOpenClick(object sender, RoutedEventArgs eventArgs)
        {
            System.Windows.Forms.FolderBrowserDialog folderDialog = new System.Windows.Forms.FolderBrowserDialog()
                { Description = "Select directory to open" };
            folderDialog.ShowDialog();
            filesystem = new Filesystem(folderDialog.SelectedPath);

            var rootItem = new TreeViewItem
            {
                Header = filesystem.RootDirectory.Name,
                Tag = filesystem.RootDirectory.FullName
            };

            Tree.Items.Add(rootItem);
            CreateTreeItemChildren(rootItem);
            rootItem.Expanded += OnExpanded;
            rootItem.Selected += ContextMenuFolder;
        }

        /// <summary>
        /// Represents single call event reading children of the sender <see cref="TreeViewItem"/>.
        /// </summary>
        /// <param name="sender"><see cref="TreeViewItem"/> expanded by user.</param>
        /// <param name="args">Event args.</param>
        protected virtual void OnExpanded(object sender, RoutedEventArgs args)
        {
            if (sender != args.OriginalSource)
                return;
            foreach (var item in (sender as TreeViewItem).Items)
                CreateTreeItemChildren(item as TreeViewItem);
        }

        protected virtual void ContextMenuFolder(object sender, RoutedEventArgs args)
        {
            if (sender != args.OriginalSource)
                return;
            ContextMenu = new ContextMenu();
            MenuItem createFile = new MenuItem
            {
                Header = "Create File",
                Icon = new Image { Source = new BitmapImage(new Uri(@".\Resources\icons\NewFile_16x.png", UriKind.Relative)) }
            };
            MenuItem createFolder = new MenuItem
            {
                Header = "Create Folder",
                Icon = new Image { Source = new BitmapImage(new Uri(@".\Resources\icons\FolderOpened_grey_16x.png", UriKind.Relative)) }
            };
            MenuItem deleteFolder = new MenuItem
            {
                Header = "Delete Folder",
                Icon = new Image { Source = new BitmapImage(new Uri(@".\Resources\icons\Trash_16x.png", UriKind.Relative)) },
            };

            createFile.Click += (s, a) => OnFileCreated(sender, args);
            createFolder.Click += (s, a) => OnFolderCreated(sender, args);
            deleteFolder.Click += (s, a) => OnFolderDeleted(sender, args);

            ContextMenu.Items.Add(createFile);
            ContextMenu.Items.Add(createFolder);
            ContextMenu.Items.Add(deleteFolder);
        }

        protected virtual void ContextMenuFile(object sender, RoutedEventArgs args)
        {
            if (sender != args.OriginalSource)
                return;
            ContextMenu = new ContextMenu();
            MenuItem openFile = new MenuItem
            {
                Header = "Open File",
                Icon = new Image { Source = new BitmapImage(new Uri(@".\Resources\icons\OpenFile_16x.png", UriKind.Relative)) }
            };
            MenuItem deleteFile = new MenuItem
            {
                Header = "Delete File",
                Icon = new Image { Source = new BitmapImage(new Uri(@".\Resources\icons\Trash_16x.png", UriKind.Relative)) }
            };

            openFile.Click += (s, a) => OnFileOpened(sender, args);
            deleteFile.Click += (s, a) => OnFileDeleted(sender, args);

            ContextMenu.Items.Add(openFile);
            ContextMenu.Items.Add(deleteFile);
        }

        protected virtual void OnFileDeleted(object sender, RoutedEventArgs args)
        {
            if (sender != args.OriginalSource)
                return;
            ((sender as TreeViewItem).Parent as TreeViewItem).Items.Remove((sender as TreeViewItem));
            filesystem.DeleteFile((sender as TreeViewItem).Tag.ToString());
        }

        protected virtual void OnFileOpened(object sender, RoutedEventArgs args)
        {
            if (sender != args.OriginalSource)
                return;
            TreeViewItem item = (sender as TreeViewItem);
            using (var textReader = System.IO.File.OpenText(item.Tag.ToString()))
            {
                string text = textReader.ReadToEnd();
                TextBlock.Text = text;
            }
        }

        protected virtual void OnFileCreated(object sender, RoutedEventArgs args)
        {
            if (sender != args.OriginalSource)
                return;
        }

        protected virtual void OnFolderCreated(object sender, RoutedEventArgs args)
        {
            if (sender != args.OriginalSource)
                return;
        }

        protected virtual void OnFolderDeleted(object sender, RoutedEventArgs args)
        {
            if (sender != args.OriginalSource)
                return;
            TreeViewItem item = (sender as TreeViewItem);
            if (item.Parent != null && item.Parent.GetType() != typeof(TreeView))
                (item.Parent as TreeViewItem).Items.Remove(item);
            else
                Tree.Items.Remove(item);
            filesystem.DeleteFolder((sender as TreeViewItem).Tag.ToString());
        }



        /// <summary>
        /// Creates item children (files and folders) and registers child directories <see cref="TreeViewItem.Expanded"/> event.
        /// </summary>
        /// <param name="parent">Parent for whom children shall be found.</param>
        private void CreateTreeItemChildren(TreeViewItem parent)
        {
            if (File.Exists((string)parent.Tag))
                return;

            foreach (var directory in filesystem.GetSubdirectories(parent.Tag.ToString()))
            {
                TreeViewItem item = new TreeViewItem
                {
                    Header = directory.Name,
                    Tag = directory.FullName,
                };
                item.Expanded += OnExpanded;
                item.Selected += ContextMenuFolder;
                parent.Items.Add(item);
            }

            foreach (var file in filesystem.GetFiles(parent.Tag.ToString()))
            {
                TreeViewItem item = new TreeViewItem
                {
                    Header = file.Name,
                    Tag = file.FullName
                };
                item.Selected += ContextMenuFile;
                parent.Items.Add(item);
            }
        }
    }
}
