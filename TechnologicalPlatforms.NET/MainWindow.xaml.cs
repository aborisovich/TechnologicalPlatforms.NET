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
            List<MenuItem> menuItems = new List<MenuItem>()
            {
                new MenuItem
                {
                    Header = "Create File",
                    Icon = new Image { Source = new BitmapImage(new Uri(@".\Resources\icons\NewFile_16x.png", UriKind.Relative)) } 
                },
                new MenuItem
                {
                    Header = "Create Folder",
                    Icon = new Image { Source = new BitmapImage(new Uri(@".\Resources\icons\FolderOpened_grey_16x.png", UriKind.Relative)) }
                },
                new MenuItem
                {
                    Header = "Delete Folder",
                    Icon = new Image { Source = new BitmapImage(new Uri(@".\Resources\icons\Trash_16x.png", UriKind.Relative)) }
                }
            };
            menuItems.ForEach(item => ContextMenu.Items.Add(item));
        }

        protected virtual void ContextMenuFile(object sender, RoutedEventArgs args)
        {
            if (sender != args.OriginalSource)
                return;
            ContextMenu = new ContextMenu();
            List<MenuItem> menuItems = new List<MenuItem>()
            {
                new MenuItem
                {
                    Header = "Open File",
                    Icon = new Image { Source = new BitmapImage(new Uri(@".\Resources\icons\OpenFile_16x.png", UriKind.Relative)) }
                },
                new MenuItem
                {
                    Header = "Delete File",
                    Icon = new Image { Source = new BitmapImage(new Uri(@".\Resources\icons\Trash_16x.png", UriKind.Relative)) }
                }
            };
            menuItems.ForEach(item => ContextMenu.Items.Add(item));
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
