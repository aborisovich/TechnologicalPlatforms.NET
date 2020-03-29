using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
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

        private void MenuExitClick(object sender, RoutedEventArgs eventArgs)
        {
            this.Close();
        }

        private void MenuOpenClick(object sender, RoutedEventArgs eventArgs)
        {
            FolderBrowserDialog folderDialog = new FolderBrowserDialog() { Description = "Select directory to open" };
            folderDialog.ShowDialog();
            filesystem = new Filesystem(folderDialog.SelectedPath);

            var rootItem = new TreeViewItem
            {
                Header = filesystem.RootDirectory.Name,
                Tag = filesystem.RootDirectory.FullName
            };

            Tree.Items.Add(rootItem);
            CreateItemChildren(rootItem);
            rootItem.Expanded += OnExpanded;
        }

        /// <summary>
        /// Represents single call event reading children of the sender <see cref="TreeViewItem"/>.
        /// </summary>
        /// <param name="sender"><see cref="TreeViewItem"/> expanded by user.</param>
        /// <param name="args">Event args.</param>
        private void OnExpanded(object sender, RoutedEventArgs args)
        {
            foreach (var item in (sender as TreeViewItem).Items)
                CreateItemChildren(item as TreeViewItem);
            (sender as TreeViewItem).Expanded -= OnExpanded;
        }

        /// <summary>
        /// Creates item children (files and folders) and registers child directories <see cref="TreeViewItem.Expanded"/> event.
        /// </summary>
        /// <param name="parent"></param>
        private void CreateItemChildren(TreeViewItem parent)
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
                parent.Items.Add(item);
            }

            foreach (var file in filesystem.GetFiles(parent.Tag.ToString()))
                parent.Items.Add(new TreeViewItem
                {
                    Header = file.Name,
                    Tag = file.FullName
                });
        }
    }
}
