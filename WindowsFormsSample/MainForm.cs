using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace WindowsFormsSample
{
    using System.Diagnostics;
    using System.Globalization;
    using System.IO;

    using Apitron.PDF.Rasterizer;
    using Apitron.PDF.Rasterizer.Configuration;
    using Apitron.PDF.Rasterizer.ErrorHandling;
    using Apitron.PDF.Rasterizer.Fonts;

    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void openToolStripMenuItem_Click( object sender, EventArgs e )
        {
            // get the dialog ready
            OpenFileDialog dlg = new OpenFileDialog();

            dlg.Filter = "(*.pdf)|*.pdf";
            dlg.Multiselect = true;

            if(dlg.ShowDialog()== DialogResult.OK)
            {
                Cursor oldCursor = Cursor.Current;

                Cursor.Current = Cursors.WaitCursor;

                try
                {
                    // open document and load its info, render first page then
                    using ( FileStream fs = new FileStream( dlg.FileName, FileMode.Open ) )
                    {
                        Document doc = new Document(fs);

                        LoadDocumentInfo(doc);

                        // render image
                        Page page = doc.Pages[0];

                        Bitmap bm = page.Render((int)page.Width, (int)page.Height, new RenderingSettings(), new ErrorLogger());
                        myImage.Image = bm;
                    }
                }
                catch ( Exception ex)
                {
                    MessageBox.Show(ex.Message);                   
                }

                Cursor.Current = oldCursor;
            }
        }

        private void LoadDocumentInfo( Document doc )
        {
            // clean
            txtFileName.Text = txtSubject.Text = txtPages.Text = string.Empty;
            fontList.Items.Clear();

            txtFonts.Text = "Fonts:";

            // load new data
            txtFileName.Text = doc.DocumentInfo.Title;
            txtSubject.Text = doc.DocumentInfo.Subject;

            txtPages.Text = doc.Pages.Count.ToString(CultureInfo.InvariantCulture);

            txtFonts.Text = string.Format("Fonts: {0}", doc.Fonts.Length);

            foreach (Apitron.PDF.Rasterizer.Fonts.Font font in doc.Fonts)
            {
                if ( font.State != FontState.Embedded )
                {
                    fontList.Items.Add(string.Format("{0},{1},{2},{3},{4}", font.Name, font.Type, font.State, font.ActualFontName, font.ActualFontType));
                }
                else
                {
                    fontList.Items.Add( string.Format( "{0},{1},{2}", font.Name, font.Type, font.State) );
                }
            }
        }
    }
}
