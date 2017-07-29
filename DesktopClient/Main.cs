using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SubDBSharp.Http;
using SubDBSharp;
using System.IO;
using System.Globalization;
using DesktopClient.Models;

namespace DesktopClient
{
    public partial class Main : Form
    {
        // Selected directory from folder browse dialog
        private string _rootDirctory;

        // Contains list of movie/tv-show to be downloaded
        private IList<MediaInfo> _mediaFiles;

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
                ".srt", ".txt", ".nfo", ".mp3", ".jpg", ".rar", ".zip"
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
            Response languages = _client.GetAvailableLanguagesAsync().Result;
            string csvLanguage = languages.Body;

            // e.g: en, pt, fr...
            foreach (string language in csvLanguage.Split(','))
            {
                var cbi = new LanguageItem(language);
                // TODO: Review why after .ToString()  return EnglishName (e.g: English) it keeps
                // showing "en" instead...
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
                var ei = new EncodingItem
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
            using (var fbd = new FolderBrowserDialog())
            {
                // Configure folder dialog


                if (fbd.ShowDialog() == DialogResult.OK)
                {
                    _rootDirctory = fbd.SelectedPath;
                    textBox1.Text = _rootDirctory;
                    LoadMedias();
                    // get all movie files and store them in a list for later access...
                }
            }
        }

        private void LoadMedias()
        {
            if (!Directory.Exists(_rootDirctory))
            {
                return;
            }
            _mediaFiles.Clear();
            foreach (string file in Directory.GetFiles(_rootDirctory))
            {
                if (IgnoreExtensions.Contains(Path.GetExtension(file)))
                {
                    continue;
                }
                var mf = new MediaInfo(file);
                _mediaFiles.Add(mf);
            }
        }

        private async void ButtonDownload_Click(object sender, EventArgs e)
        {
            // validation fails
            if (!IsValid())
            {
                return;
            }
            await DownloadSubtitleAsync();

            MessageBox.Show("Download completed!");
        }

        private async Task DownloadSubtitleAsync()
        {
            var cbi = (LanguageItem)comboBoxLanguage.SelectedItem;
            Encoding encoding = ((EncodingItem)comboBoxEncoding.SelectedItem).Encoding;
            foreach (MediaInfo mediaInfo in _mediaFiles)
            {
                Response response = await _client.DownloadSubtitleAsync(mediaInfo.Hash, cbi.CultureInfo.TwoLetterISOLanguageName);
                if (response.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    continue;
                }
                string path = Path.Combine(Path.GetDirectoryName(mediaInfo.FileInfo.FullName), Path.GetFileNameWithoutExtension(mediaInfo.FileInfo.Name) + ".srt");
                File.WriteAllText(path, response.Body, encoding);
            }
        }

        private bool IsValid()
        {
            if (string.IsNullOrEmpty(_rootDirctory))
            {
                MessageBox.Show("No directory selected");
                return false;
            }

            if (!Directory.Exists(_rootDirctory))
            {
                MessageBox.Show("Selected directory doesn't exits!");
                return false;
            }

            if (_mediaFiles.Count == 0)
            {
                MessageBox.Show("Media files not loaded!");
                return false;
            }

            return true;
        }

        private void textBox1_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.Copy;
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }

        private void textBox1_DragDrop(object sender, DragEventArgs e)
        {
            string[] directory = (string[])e.Data.GetData(DataFormats.FileDrop);
            if (directory.Length > 0)
            {
                _rootDirctory = directory.First();
                textBox1.Text = _rootDirctory;
                LoadMedias();
            }
        }
    }
}
