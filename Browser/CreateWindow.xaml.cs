using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Browser
{
    /// <summary>
    /// Interaction logic for CreateWindow.xaml
    /// </summary>
    public partial class CreateWindow : Window
    {
        public string DirectoryPath { get; }
        private Dictionary<CheckBox, FileAttributes> FileAttributesMap { get; }
        private List<CheckBox> AttributesCheckboxes { get; }

        public CreateWindow(string directoryPath)
        {
            InitializeComponent();
            if (!Directory.Exists(directoryPath))
            {
                MessageBox.Show("Error, selected directory does not exist", "Directory not found", MessageBoxButton.OK, MessageBoxImage.Error);
                Close();
            }

            DirectoryPath = directoryPath;
            FileAttributesMap = new Dictionary<CheckBox, FileAttributes>()
            {
                { ReadOnlyCheckbox, FileAttributes.ReadOnly },
                { ArchiveCheckbox, FileAttributes.Archive },
                { HiddenCheckbox, FileAttributes.Hidden },
                { SystemCheckbox, FileAttributes.System }
            };
            AttributesCheckboxes = new List<CheckBox>()
            {
                ReadOnlyCheckbox,
                ArchiveCheckbox,
                HiddenCheckbox,
                SystemCheckbox
            };
        }

        private void OnConfirmButtonClick(object sender, RoutedEventArgs e)
        {
            if (sender != e.OriginalSource)
                return;

            if (FileSelection.IsChecked.Value)
            {
                FileAttributes attributes = FileAttributes.Normal;
                AttributesCheckboxes.ForEach(checkbox =>
                {
                    if (checkbox.IsChecked.Value)
                        attributes |= FileAttributesMap[checkbox];
                });
                Filesystem.CreateFile(System.IO.Path.Combine(DirectoryPath, CreateWindowTextbox.Text), attributes);
                Close();
            }
            else if (DirectorySelection.IsChecked.Value)
            {
                FileAttributes attributes = FileAttributes.Normal;
                AttributesCheckboxes.ForEach(checkbox =>
                {
                    if (checkbox.IsChecked.Value)
                        attributes |= FileAttributesMap[checkbox];
                });
                Filesystem.CreateDirectory(System.IO.Path.Combine(DirectoryPath, CreateWindowTextbox.Text), attributes);
                Close();
            }
            else
                MessageBox.Show("Please choose file or directory to create.", "Create type not specified", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void OnCancelButtonClick(object sender, RoutedEventArgs e)
        {
            if (sender != e.OriginalSource)
                return;
            Close();
        }
    }
}
