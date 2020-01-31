using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Media;

namespace KryptrGUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 
    public partial class MainWindow : Window
    {

        public MainWindow()
        {
            InitializeComponent();

        }

        private string[] filesToEncode;
        private bool containsEncrypted;
        private bool containsPlain;
        private int compressionLevel = 5;

        private void CheckSelectionContents()
        {
            containsEncrypted = false;
            containsPlain = false;
            foreach (string s in filesToEncode)
            {
                FileAttributes attr = File.GetAttributes(s);

                if ((attr & FileAttributes.Directory) == FileAttributes.Directory)
                {
                    foreach (string file in Directory.GetFiles(s))
                    {
                        if (file.Contains(".kv2"))
                        {
                            containsEncrypted = true;
                        }
                        else
                        {
                            containsPlain = true;
                        }
                    }
                }
                else
                {
                    if (s.Contains(".kv2"))
                    {
                        containsEncrypted = true;
                    }
                    else
                    {
                        containsPlain = true;
                    }
                }
            }
        }

        private void UpdateSelections()
        {
            if (filesToEncode.Length >= 1)
            {
                runButton.IsEnabled = true;
                selectionBox.IsEnabled = true;
                CheckSelectionContents();
                if (containsEncrypted)
                {
                    pubBox.IsEnabled = true;
                    pubHelp.IsEnabled = true;
                    pubLabel.IsEnabled = true;
                }
                else
                {
                    pubBox.IsEnabled = false;
                    pubBox.Password = "";
                    pubHelp.IsEnabled = false;
                    pubLabel.IsEnabled = false;
                }
                if (containsPlain)
                {
                    useCompressionBox.IsEnabled = true;
                    compressionHelp.IsEnabled = true;
                    seedBox.IsEnabled = true;
                    seedHelp.IsEnabled = true;
                    seedLabel.IsEnabled = true;
                    compDropdown.IsEnabled = true;
                    compLevelHelp.IsEnabled = true;
                    useScrambleBox.IsEnabled = true;
                    scrambleHelp.IsEnabled = true;
                }
                else
                {
                    seedBox.IsEnabled = false;
                    seedBox.Password = "";
                    seedLabel.IsEnabled = false;
                    seedHelp.IsEnabled = false;
                    useCompressionBox.IsEnabled = false;
                    compressionHelp.IsEnabled = false;
                    compDropdown.IsEnabled = false;
                    compLevelHelp.IsEnabled = false;
                    useScrambleBox.IsEnabled = false;
                    scrambleHelp.IsEnabled = false;
                }
                errorLog.Visibility = Visibility.Hidden;
            }
            else
            {
                errorLog.Text = "ERROR: Please select at least 1 file.";
                errorLog.Visibility = Visibility.Visible;
            }
        }

        private void SelectFilesClicked(object sender, RoutedEventArgs e)
        {
            selectButton.ClearValue(BorderThicknessProperty);
            selectButton.ClearValue(BorderBrushProperty);
            OpenFileDialog ofd = new OpenFileDialog
            {
                Multiselect = true,
                Title = "Select files to encrypt/decrypt"
            };
            ofd.ShowDialog(this);
            filesToEncode = ofd.FileNames;
            UpdateSelections();
        }

        private void RunClicked(object sender, RoutedEventArgs e)
        {
            if (filesToEncode != Array.Empty<string>())
            {

                foreach (string s in filesToEncode)
                {
                    if (!Directory.Exists(s) && !File.Exists(s))
                    {
                        errorLog.Text = "ERROR: At least one of your selected files no longer exists.";
                        errorLog.Visibility = Visibility.Visible;
                        return;
                    }
                }

                if (passBox.Password == "")
                {
                    errorLog.Text = "ERROR: You must always supply a password.";
                    errorLog.Visibility = Visibility.Visible;
                    return;
                }
                string inputs = "";
                string pubValue;
                string seedValue;
                foreach (string s in filesToEncode)
                {
                    inputs += s + ((s != filesToEncode.Last()) ? "," : "");
                }

                if (seedBox.Password != "")
                {
                    seedValue = pubBox.Password;
                }
                else if (!containsPlain)
                {
                    seedValue = "$NOSEED$";
                }
                else
                {
                    errorLog.Text = "ERROR: You are encrypting at least 1 file, please supply a SEED.";
                    errorLog.Visibility = Visibility.Visible;
                    return;
                }
                if (pubBox.Password != "")
                {
                    pubValue = pubBox.Password;
                }
                else if (!containsEncrypted)
                {
                    pubValue = "$NOPUB$";
                }
                else
                {
                    errorLog.Text = "ERROR: You are decrypting at least 1 file, please supply a PUBLIC KEY";
                    errorLog.Visibility = Visibility.Visible;
                    return;
                }
                errorLog.Visibility = Visibility.Hidden;

                string useCompression;
                if ((bool)useCompressionBox.IsChecked)
                {
                    useCompression = "NULL";
                }
                else
                {
                    useCompression = "$NOCOMPRESS$";
                }

                string useProcessing;
                if ((bool)useProcessingBox.IsChecked)
                {
                    useProcessing = "NULL";
                }
                else
                {
                    useProcessing = "$NOFULL$";
                }

                string keepOriginals;
                if ((bool)keepOriginalBox.IsChecked)
                {
                    keepOriginals = "$KEEP$";
                }
                else
                {
                    keepOriginals = "NULL";
                }

                string doScramble;
                if ((bool)useScrambleBox.IsChecked)
                {
                    doScramble = "$SCRAMBLE$";
                }
                else
                {
                    doScramble = "NULL";
                }

                ProcessStartInfo psi = new ProcessStartInfo($@"{System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location)}\Kryptr V2.exe", $"GUIHOOK \"{inputs}\" \"{seedValue}\" \"{passBox.Password}\" \"{pubValue}\" {useCompression} {useProcessing} {keepOriginals} {compressionLevel} {doScramble}");
                Process.Start(psi);
            }
        }

        private void ShowCompressionHelp(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Double-layered compression enables Kryptr to compress the file before the encryption process begins, which can be useful when encrypting larger files. If you are encrypting a smaller file (<5MB), you may wish to turn this off to conserve space.\n\nNOTE: This setting does not effect DECRYPTION in any way.", "What is double-layered compression?", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void ShowPubHelp(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("The public key is a unique string of letters and numbers generated by Kryptr after a file is encrypted. It is unique to that specific encryption and is required to decrypt the files that were encrypted. If you have lost the public key, you can generate a new one by opening Kryptr and pressing the button that says \"Recover a public key\". Just note, you need the PRIVATE KEY and PASSWORD to regenerate the public key.", "What is the public key?", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void ShowPassHelp(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("The password is a user-supplied string that is used during both encryption and decryption. You must supply the same password the file was encrypted with during decryption in order to decrypt the file. Just like any other password, you should keep this safe.", "What is the password?", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void ShowSeedHelp(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("The seed is a user-supplied string used to increase the \"randomness\" of the encryption process. As such, it is only used during encryption and doesn't need to be remembered. For a stronger encryption, try to make this as random as possible!", "What is the seed?", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void ShowProcessingHelp(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Allows KV2 to utilize more processor threads, which will make the process of encryption/decryption faster but at the cost of using a large amount of your computer's processing power and slowing other programs down when encrypting/decrypting very large groups of files.", "What is it to utilize full processing power?", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void ShowKeepOriginalHelp(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("This checkbox tells KV2 whether you'd like to keep the original files after they've been encrypted or decrypted. This is left unchecked by default so as to not keep both an unecrypted copy and an encrypted copy of each file which may be unsafe and take up storage space.", "What does \"keep original files\" do?", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void ShowCompLevelHelp(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("This dropdown tells KV2 how much you'd like it to compress files. Higher levels may make files significantly smaller, but will make the encryption and decryption process longer. Lower levels will encrypt and decrypt faster, but will leave larger files.\n\nNOTE: If double-layered encryption is enabled, this setting applies to both the automatic compression and the second layer of compression.", "What does the compression level dropdown do?", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void ShowScrambleHelp(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("The scramble filenames option will cause KV2 to randomize the filenames and remove the extensions of encrypted files so as to make it harder for the contents to be guessed. This is off by default so that you can still easily tell what the file originally was.", "What does scramble filenames do?", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void FileDropped(object sender, DragEventArgs e)
        {
            if (!e.Data.GetFormats().Contains(DataFormats.FileDrop))
            {
                MessageBox.Show("Please make sure that you're only dropping a file or directory onto KV2's window!", "Drag-and-drop error detected", MessageBoxButton.OK, MessageBoxImage.Error);
                FileLeft(sender, e);
                return;
            }
            filesToEncode = (string[])e.Data.GetData(DataFormats.FileDrop, true);
            UpdateSelections();
            FileLeft(sender, e);
        }

        private void FileDragged(object sender, DragEventArgs e)
        {
            selectButton.BorderThickness = new Thickness(2);
            selectButton.BorderBrush = Brushes.Red;
        }

        private void FileLeft(object sender, DragEventArgs e)
        {
            selectButton.ClearValue(BorderThicknessProperty);
            selectButton.ClearValue(BorderBrushProperty);
        }

        private void RecoverClicked(object sender, RoutedEventArgs e)
        {
            TextOrRecover tor = new TextOrRecover(this);
            tor.ShowDialog();
        }

        public void EncryptTextClicked(object sender, RoutedEventArgs e)
        {

            ProcessStartInfo psi = new ProcessStartInfo($@"{System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location)}\Kryptr V2.exe", "STRINGHOOK");
            Process.Start(psi);
        }

        public void RecoverSelected(object sender, RoutedEventArgs e)
        {
            ProcessStartInfo psi = new ProcessStartInfo($@"{System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location)}\Kryptr V2.exe", "RECOVERYHOOK");
            Process.Start(psi);
        }

        private void ComplevelChanged(object sender, SelectionChangedEventArgs e)
        {
            int[] values = { 1, 3, 5, 7, 9 };
            if (compDropdown.SelectedIndex == 0)
            {
                compressionLevel = 1;
            }
            else
            {
                compressionLevel = values[compDropdown.SelectedIndex];
            }
        }

        private void RevealMusicButton(object sender, System.Windows.Input.MouseEventArgs e)
        {
            MusicButton.Opacity = 100;
        }

        private void HideMusicButton(object sender, System.Windows.Input.MouseEventArgs e)
        {
            MusicButton.Opacity = 0;
        }

        bool musicIsOn = false;
        SoundPlayer sp = new SoundPlayer();
        private void PlayMusic(object sender, RoutedEventArgs e)
        {
            if (!musicIsOn)
            {
                sp.Stream = Properties.Resources.MU;
                sp.PlayLooping();
            }
            else
            {
                sp.Stop();
            }
            musicIsOn = !musicIsOn;
        }
    }
}
