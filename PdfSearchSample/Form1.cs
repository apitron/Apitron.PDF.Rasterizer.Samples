#region

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using Apitron.PDF.Rasterizer;
using Apitron.PDF.Rasterizer.Configuration;
using Apitron.PDF.Rasterizer.Search;

#endregion

namespace PdfSearchSample
{
    public partial class Form1 : Form
    {
        #region Fields

        private string selectedFolder;

        private Thread thread;

        private bool CancelSearching = false;

        private string currentSearchFileName;

        private Apitron.PDF.Rasterizer.Document document = null;

        private string currentWorkingFileName;

        private readonly object selectionLock = new object();

        private ListViewGroup currentGroup;

        #endregion

        #region Ctors

        public Form1()
        {
            InitializeComponent();
            this.DoubleBuffered = true;
            this.Closing += OnClosing;
        }

        #endregion

        #region Event Handlers

        /// <summary>
        ///   Called when [closing].
        /// </summary>
        /// <param name="sender"> The sender. </param>
        /// <param name="cancelEventArgs"> The <see cref="System.ComponentModel.CancelEventArgs" /> instance containing the event data. </param>
        private void OnClosing(object sender, CancelEventArgs cancelEventArgs)
        {
            if (thread != null)
            {
                CancelSearching = true;
                thread.Join(5000);
            }
        }

        /// <summary>
        ///   Called when folder with pdf files is selected.
        /// </summary>
        /// <param name="sender"> The sender. </param>
        /// <param name="e"> The <see cref="System.EventArgs" /> instance containing the event data. </param>
        private void OnSelectStorage(object sender, EventArgs e)
        {
            DialogResult result = pdfStorageSelector.ShowDialog();
            if (result == DialogResult.OK)
            {
                selectedFolder = folderNameLabel.Text = pdfStorageSelector.SelectedPath;
            }
            else
            {
                selectedFolder = string.Empty;
                folderNameLabel.Text = "(is not specified)";
            }
        }

        /// <summary>
        ///   Called when we clicked search.
        /// </summary>
        /// <param name="sender"> The sender. </param>
        /// <param name="e"> The <see cref="System.EventArgs" /> instance containing the event data. </param>
        private void OnSearch(object sender, EventArgs e)
        {
            if (thread != null)
            {
                CancelSearching = true;
                thread.Join(5000);
                CancelSearching = false;
            }

            resultsView.Items.Clear();
            resultsView.Groups.Clear();
            thread = new Thread(StartSearching);
            thread.Start();
        }

        /// <summary>
        ///   Called when the digest item is selected.
        /// </summary>
        /// <param name="sender"> The sender. </param>
        /// <param name="e"> The <see cref="System.Windows.Forms.ListViewItemSelectionChangedEventArgs" /> instance containing the event data. </param>
        private void OnSelectResult(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            lock (selectionLock)
            {
                if (resultsView.SelectedItems.Count != 0)
                {
                    ResultItem item = (ResultItem) resultsView.SelectedItems[0].Tag;
                    if (item.FileName != currentWorkingFileName || document == null)
                    {
                        if (document != null)
                        {
                            document.Dispose();
                        }

                        FileStream stream = new FileStream(item.FileName, FileMode.Open, FileAccess.Read, FileShare.Read);

                        // create document used for search and rendering
                        document = new Document(stream);
                        currentWorkingFileName = item.FileName;
                    }

                    Page page = document.Pages[item.SearchResult.PageIndex];
                    RenderingSettings renderingSettings = new RenderingSettings();

                    Resolution resolution = new Resolution(72, 72);
                    Bitmap bmp = page.Render(resolution, renderingSettings);


                    // Highlights the search result
                    Graphics gr = Graphics.FromImage(bmp);
                    SearchResultRegion searchResultRegion = page.TransformRegion(item.SearchResult.Region, resolution, renderingSettings);

                    foreach (double[] block in searchResultRegion.Blocks)
                    {
                        PointF[] points = new PointF[block.Length/2];
                        for (int i = 0; i < block.Length/2; i++)
                        {
                            points[i] = new PointF((float) block[i*2], (float) block[i*2 + 1]);
                        }
                        gr.FillPolygon(new SolidBrush(Color.FromArgb(50, Color.Yellow)), points);
                    }

                    pictureBox1.Image = bmp;
                }
            }
        }

        #endregion

        #region Private Members

        /// <summary>
        ///   Starts the searching.
        /// </summary>
        private void StartSearching()
        {
            UseWaitCursor = true;
            try
            {
                if (!string.IsNullOrEmpty(searchText.Text) && !string.IsNullOrEmpty(selectedFolder))
                {
                    string[] pdfFiles = Directory.GetFiles(selectedFolder, "*.pdf", SearchOption.TopDirectoryOnly);
                    foreach (string pdfFile in pdfFiles)
                    {
                        currentSearchFileName = pdfFile;
                        using (FileStream stream = new FileStream(pdfFile, FileMode.Open, FileAccess.Read, FileShare.Read))
                        {
                            SearchIndex searchIndex = new SearchIndex(stream);

                            // search text in PDF document and render pages containg results
                            searchIndex.Search(OnSearchItem, searchText.Text);
                        }

                        if (CancelSearching)
                        {
                            UseWaitCursor = false;
                            break;
                        }
                    }
                }
            }
            finally
            {
                //Nothing was found
                currentSearchFileName = string.Empty;
                UseWaitCursor = false;
            }
        }

        /// <summary>
        ///   Called when generate a digest of found items.
        /// </summary>
        /// <param name="handlerargs"> The handler args. </param>
        private void OnSearchItem(SearchHandlerArgs handlerargs)
        {
            if (!(handlerargs.CancelSearch = CancelSearching))
            {
                try
                {
                    Invoke(new Action(delegate
                    {
                        if (handlerargs.ResultItems.Count != 0)
                        {
                            string fileName = Path.GetFileName(currentSearchFileName);
                            ListViewGroup group = currentGroup;
                            if (currentGroup == null || currentGroup.Header != fileName)
                            {
                                group = currentGroup = new ListViewGroup(fileName);
                                resultsView.Groups.Add(group);
                                group.HeaderAlignment = HorizontalAlignment.Left;
                            }

                            foreach (SearchResultItem searchResultItem in handlerargs.ResultItems)
                            {
                                ListViewItem item = new ListViewItem(string.Format("'{0}' on the page {1}", searchResultItem.Title, searchResultItem.PageIndex), group);
                                item.Tag = new ResultItem(searchResultItem, currentSearchFileName);
                                resultsView.Items.Add(item);
                            }
                        }
                    }));
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e);
                }
            }
            else
            {
                Debug.WriteLine("Canceled");
            }
        }

        #endregion
    }
}
