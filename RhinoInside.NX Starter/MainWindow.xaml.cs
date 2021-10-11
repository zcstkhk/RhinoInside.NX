using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace RhinoInside_Starter
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public UserPath NxInstallPath = new UserPath() { UserPathType = UserPathType.NxInstallPath };
        public UserPath RhinoInsidePath = new UserPath() { UserPathType = UserPathType.RhinoInsidePath };
        public UserPath RhinoPath = new UserPath() { UserPathType = UserPathType.RhinoPath };

        public static ResourceDictionary LanguageDictionary;

        public MainWindow()
        {
            InitializeComponent();

            textBoxNxInstallPath.SetBinding(TextBox.TextProperty, new Binding() { Source = NxInstallPath, Path = new PropertyPath("SelectedPath") });
            textBoxRhinoInsidePath.SetBinding(TextBox.TextProperty, new Binding() { Source = RhinoInsidePath, Path = new PropertyPath("SelectedPath") });
            textBoxRhinoPath.SetBinding(TextBox.TextProperty, new Binding() { Source = RhinoPath, Path = new PropertyPath("SelectedPath") });

            string configPath = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "RhinoInside Starter Config.config");

            if (System.IO.File.Exists(configPath))
            {
                string[] configContent = System.IO.File.ReadAllLines(configPath);

                comboBoxSelectLanguage.SelectedIndex = Convert.ToInt32(configContent[0].Substring(2));
                NxInstallPath.SelectedPath = configContent[1].Substring(2);
                RhinoInsidePath.SelectedPath = configContent[2].Substring(2);
                RhinoPath.SelectedPath = configContent[3].Substring(2);
            }
            else
            {
                if (Environment.GetEnvironmentVariable("UGII_BASE_DIR") != null)
                {
                    NxInstallPath.SelectedPath = Environment.GetEnvironmentVariable("UGII_BASE_DIR");
                }
            }
        }

        private void buttonBrowseNX_Click(object sender, RoutedEventArgs e)
        {
            SelectPath(NxInstallPath);
        }

        private void comboBoxSelectLanguage_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string requestedCulture;

            int selectedIndex = (sender as ComboBox).SelectedIndex;

            if (selectedIndex == 0)
                requestedCulture = @"Resources\ZH-CN.xaml";
            else if (selectedIndex == 1)
                requestedCulture = @"Resources\EN-US.xaml";
            else
                throw new NotImplementedException("未实现的语言种类");

            LanguageDictionary = Application.Current.Resources.MergedDictionaries.FirstOrDefault(obj => obj.Source.OriginalString.Equals(requestedCulture));
            Application.Current.Resources.MergedDictionaries.Remove(LanguageDictionary);
            Application.Current.Resources.MergedDictionaries.Add(LanguageDictionary);
        }

        private void buttonBrowseRhinoInside_Click(object sender, RoutedEventArgs e)
        {
            SelectPath(RhinoInsidePath);
        }

        private void buttonBrowseRhino_Click(object sender, RoutedEventArgs e)
        {
            SelectPath(RhinoPath);
        }

        private void SelectPath(UserPath path)
        {
            System.Windows.Forms.FolderBrowserDialog folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();

            var result = folderBrowserDialog.ShowDialog();

            if (result != System.Windows.Forms.DialogResult.Cancel)
                path.SelectedPath = folderBrowserDialog.SelectedPath;
        }

        private void buttonStart_Click(object sender, RoutedEventArgs e)
        {
            var configFilePath = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "RhinoInside Starter Config.config");

            using (var stream = System.IO.File.CreateText(configFilePath))
            {
                stream.WriteLine("# " + comboBoxSelectLanguage.SelectedIndex.ToString());
                stream.WriteLine("# " + textBoxNxInstallPath.Text);
                stream.WriteLine("# " + textBoxRhinoInsidePath.Text);
                stream.WriteLine("# " + textBoxRhinoPath.Text);
            }

            Process nxProcess = new Process();

            if (comboBoxSelectLanguage.SelectedIndex == 0)
                Environment.SetEnvironmentVariable("UGII_LANG", "simpl_chinese");
            else
                Environment.SetEnvironmentVariable("UGII_LANG", "english");

            if (System.IO.Directory.Exists(@"E:\Documents\Programming\Repos\NXOpen Debug Utilities\00~Program"))
            {
                using (var writer = System.IO.File.AppendText(configFilePath))
                {
                    writer.WriteLine(System.IO.Path.Combine( @"E:\Documents\Programming\Repos\NXOpen Debug Utilities\00~Program", textBoxNxInstallPath.Text.Split('\\').Last()));

                    writer.WriteLine(textBoxRhinoInsidePath.Text);
                }
                
                Environment.SetEnvironmentVariable("UGII_CUSTOM_DIRECTORY_FILE", configFilePath);
            }
            else
            {
                Environment.SetEnvironmentVariable("UGII_CUSTOM_DIRECTORY_FILE", "");

                Environment.SetEnvironmentVariable("UGII_USER_DIR", textBoxRhinoInsidePath.Text);
            }

            Environment.SetEnvironmentVariable("UGII_RhinoInside_Dir", textBoxRhinoInsidePath.Text);

            var allProcesses = Process.GetProcesses();
            if (allProcesses.Any(obj => obj.ProcessName == "vmware" || obj.ProcessName == "vmplayer"))
                Environment.SetEnvironmentVariable("SPLM_LICENSE_SERVER", "28000@WIN-KOUCIUBMHSM");
            else
                Environment.SetEnvironmentVariable("SPLM_LICENSE_SERVER", $"28000@{Environment.MachineName}");

            nxProcess.StartInfo.FileName = System.IO.Path.Combine(textBoxNxInstallPath.Text, "NXBIN", "ugraf.exe");

            nxProcess.StartInfo.Arguments = "-nx";

            nxProcess.Start();

            Close();
        }
    }

    public enum UserPathType
    {
        NxInstallPath,
        RhinoInsidePath,
        RhinoPath,
    }

    public class UserPath : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public UserPathType UserPathType;

        private string _selectedPath;

        public string SelectedPath
        {
            get => _selectedPath;

            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    switch (UserPathType)
                    {
                        case UserPathType.NxInstallPath:
                            var ugrafPath = System.IO.Path.Combine(value, "NXBIN", "ugraf.exe");

                            if (!System.IO.File.Exists(ugrafPath))
                            {
                                MessageBox.Show(MainWindow.LanguageDictionary["invalidNxPathMsg"].ToString());
                                return;
                            }
                            break;
                        case UserPathType.RhinoInsidePath:
                            var rhinoInsidePath = System.IO.Path.Combine(value, "Startup", "RhinoInside.NX.Core.dll");

                            if (!System.IO.File.Exists(rhinoInsidePath))
                            {
                                MessageBox.Show(MainWindow.LanguageDictionary["invalidRhinoInsidePathMsg"].ToString());
                                return;
                            }
                            break;
                        case UserPathType.RhinoPath:
                            var rhinoPath = System.IO.Path.Combine(value, "System", "Rhino.exe");

                            if (!System.IO.File.Exists(rhinoPath))
                            {
                                MessageBox.Show(MainWindow.LanguageDictionary["invalidRhinoPathMsg"].ToString());
                                return;
                            }
                            break;
                        default:
                            break;
                    }
                }

                _selectedPath = value;

                if (PropertyChanged != null)
                {
                    PropertyChanged.Invoke(this, new PropertyChangedEventArgs("SelectedPath"));
                }
            }
        }
    }
}
