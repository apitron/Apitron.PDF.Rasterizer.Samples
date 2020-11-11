using System;
using System.Collections.Generic;
using System.Text;
using Apitron.PDF.Rasterizer;
using Apitron.PDF.Rasterizer.Configuration;
using Apitron.PDF.Rasterizer.Navigation;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;


namespace BookmarksUsage
{
    /// <summary>
    /// Demonstrates bookmarks enumeration in PDF file.
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            // open and load the file
            using (FileStream fs = new FileStream(@"..\..\..\Documents\development guide.pdf", FileMode.Open))
            {
                // this object represents a PDF document
                using(Document document = new Document(fs))
                {
                   EnumerateBookmarksAndPrint(document.Bookmarks);                 
                }
            }

            Console.ReadLine();
        }

        /// <summary>
        /// Enumerates the bookmarks and prints their title.
        /// </summary>
        /// <param name="bookmark">The bookmark.</param>
        private static void EnumerateBookmarksAndPrint(Bookmark bookmark)
        {
            if(bookmark!=null)
            {
                Console.WriteLine(bookmark.Title);   

                for(int i=0;i<bookmark.Children.Count;i++)
                {
                    EnumerateBookmarksAndPrint(bookmark.Children[i]);
                }
            }
        }        
    }
}
