using DesktopClient.Helpers;
using DesktopClient.Models;
using SubDBSharp;
using SubDBSharp.Http;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DesktopClient
{
    public partial class Main : Form
    {
        // Selected directory from folder browse dialog
        private string _rootDirectory;

        // Contains list of movie/tv-show to be downloaded
        private readonly IList<MediaInfo> _mediaFiles;

        // SubDbSharp api client
        private readonly SubDBClient _client;

        private readonly HashSet<string> IgnoreExtensions;

        public Main()
        {
            InitializeComponent();

            // initialize client
            _client = new SubDBClient(new System.Net.Http.Headers.ProductHeaderValue("Desktop-Client", "1.0"));

            IgnoreExtensions = new HashSet<string>()
            {
                ".srt", ".txt", ".nfo", ".mp3", ".jpg", ".rar", ".zip", ".7zip", ".png"
            };

            _mediaFiles = new List<MediaInfo>();

            // initialize encoding combobox
            InitializeEncoding();

            // initialize combobox language
            InitializeLanguageCombobox();
        }

        private void InitializeLanguageCombobox()
        {
            // .Result can produce a deadlock in certain scenario's 
            Response languages = _client.GetAvailableLanguagesAsync().GetAwaiter().GetResult();

            byte[] buffer = (byte[])languages.Body;
            string csvLanguage = Encoding.UTF8.GetString(buffer, 0, buffer.Length);

            // e.g: en, pt, fr...
            foreach (string language in csvLanguage.Split(','))
            {
                LanguageItem cbi = new LanguageItem(language);
                // TODO: Review why after .ToString() return EnglishName (e.g: English) it keeps showing "en" instead...
                comboBoxLanguage.Items.Add(cbi);
            }

            // english
            comboBoxLanguage.SelectedIndex = 0;
        }

        private void InitializeEncoding()
        {
            int utf8Idx = -1;
            foreach (EncodingInfo encoding in Encoding.GetEncodings())
            {
                EncodingItem ei = new EncodingItem
                {
                    Encoding = encoding.GetEncoding()
                };
                if (encoding.Name.Equals("utf-8", StringComparison.OrdinalIgnoreCase))
                {
                    utf8Idx = comboBoxEncoding.Items.Count;
                }
                comboBoxEncoding.Items.Add(ei);
            }
            if (utf8Idx != -1)
            {
                comboBoxEncoding.SelectedIndex = utf8Idx;
            }
        }

        private void ButtonBrowse_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog folderDialog = new FolderBrowserDialog { ShowNewFolderButton = true, SelectedPath = _rootDirectory })
            {
                if (folderDialog.ShowDialog() == DialogResult.OK)
                {
                    _rootDirectory = folderDialog.SelectedPath;
                    textBoxPath.Text = folderDialog.SelectedPath;
                }
            }
        }

        private int LoadMedias()
        {
            _mediaFiles.Clear();
            foreach (string file in Directory.GetFiles(_rootDirectory))
            {
                // filter extension
                string lowerExtension = Path.GetExtension(file);
                if (IgnoreExtensions.Contains(lowerExtension))
                {
                    continue;
                }

                _mediaFiles.Add(MediaInfo.FromFile(file));
            }
            return _mediaFiles.Count;
        }

        private async void ButtonDownload_Click(object sender, EventArgs e)
        {
            // validation fails
            if (!IsValid())
            {
                return;
            }

            // reset progress bar
            progressBar1.Value = 0;
            progressBar1.Maximum = _mediaFiles.Count;

            //disable all controls
            ChangeControlsState(false);

            await DownloadAsync().ConfigureAwait(true);

            ChangeControlsState(true);
            // force async 
            //await Task.Yield();
            //ChangeControlsStates(false);
        }

        private async Task DownloadAsync()
        {
            LanguageItem cbi = (LanguageItem)comboBoxLanguage.SelectedItem;

            // encoding used to write content in file
            Encoding writeEncoding = ((EncodingItem)comboBoxEncoding.SelectedItem).Encoding;

            // report download progress
            Progress<string> progressReporter = new Progress<string>(value =>
            {
                //progressBar1.Value += value;
                progressBar1.Value += 1;
                if (File.Exists(value))
                {
                    Trace.WriteLine("file downloaded!");
                }
                if (progressBar1.Value == progressBar1.Maximum)
                {
                    MessageBox.Show("Download completed!");
                    ChangeControlsState(true);
                }
            });

            // this need to happen because Progress<T> implement IProgress interface explicitly
            IProgress<string> reporter = progressReporter;

            for (int i = 0; i < _mediaFiles.Count; ++i)
            {
                MediaInfo mediaInfo = _mediaFiles[i];

                Response response = await _client.DownloadSubtitleAsync(mediaInfo.Hash,
                    cbi.CultureInfo.TwoLetterISOLanguageName).ConfigureAwait(false);

                // download failed
                if (response.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    Trace.WriteLine($"Failed downloadeding: {_mediaFiles[i].FileInfo.Name}");
                    continue;
                }

                // generate file output location
                string path = Path.Combine(Path.GetDirectoryName(mediaInfo.FileInfo.FullName),
                    Path.GetFileNameWithoutExtension(mediaInfo.FileInfo.Name) + ".srt");

                byte[] buffer = (byte[])response.Body;

                // read content using guessed encoding
                string content = EncodingDetector.DetectAnsiEncoding(buffer).GetString(buffer, 0, buffer.Length);

                // write downloaded file to disk
                File.WriteAllText(path, content, writeEncoding);

                // report progress
                reporter?.Report(path);
            }
        }

        private void ChangeControlsState(bool enabled)
        {
            // combobox
            comboBoxEncoding.Enabled = enabled;
            comboBoxLanguage.Enabled = enabled;

            // button
            buttonDownload.Enabled = enabled;
        }

        private bool IsValid()
        {
            if (!Path.IsPathRooted(_rootDirectory))
            {
                MessageBox.Show("Invalid path!");
                return false;
            }
            if (!Directory.Exists(_rootDirectory))
            {
                MessageBox.Show("Selected directory doesn't exits!");
                return false;
            }
            // get media file content from selected path from textbox
            if (LoadMedias() == 0)
            {
                MessageBox.Show("Media files not loaded!");
                return false;
            }
            return true;
        }

        private void TextBox1_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = e.Data.GetDataPresent(DataFormats.FileDrop) ? DragDropEffects.Copy : DragDropEffects.None;
        }

        private void textBox1_DragDrop(object sender, DragEventArgs e)
        {
            string[] directory = (string[])e.Data.GetData(DataFormats.FileDrop);
            if (directory.Length > 0)
            {
                textBoxPath.Text = directory.First();
            }
        }
    }

}
