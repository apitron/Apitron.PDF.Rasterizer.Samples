// This code is a part of samples package for Apitron PDF Rasterizer .NET
// © 2014 Apitron LTD 

using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using Android.Views;
using System.Collections.Generic;
using Apitron.PDF.Rasterizer;
using Android.Content;
using System.IO;
using Android.Graphics;
using Android.Widget;
using Android.Util;
using Apitron.PDF.Rasterizer.Configuration;
using Java.Nio;
using Android.Support.V4.View;

namespace AndroidSampleWithViewPager
{
    /// <summary>
    /// Simple pager adapter, uses disk cache to store PDF doc rendered pages.
    /// The rendering is being performed on separate thread with optional progressive view update.
    /// See the <see cref="ProgressiveRendering"/> property for details.
    /// </summary>
    internal class SimplePagerAdapter : PagerAdapter
    {
        #region fields

        private Document document;
        private Context baseContext;

        // This field holds all the currently displayable views, in order from left to right.
        private ImageView[] views;

        // holds bitmaps, can be replaced with single field if no further caching is needed.
        private Bitmap[] bitmaps;

        private string cacheDirName;

        private object syncObject;

        #endregion

        #region properties

        /// <summary>
        /// Gets or sets a value indicating whether progressive rendering should be used, so the resulting view will be updated continously until the page is fully rendered.
        /// </summary>
        /// <value>
        ///   <c>true</c> if progressive rendering is enabled; otherwise, <c>false</c>.
        /// </value>
        public bool ProgressiveRendering { get; set; }

        #endregion

        #region ctor

        /// <summary>
        /// Initializes a new instance of the <see cref="SimplePagerAdapter"/> class able to present PDF documents.
        /// </summary>
        /// <param name="baseContext">The base context.</param>
        /// <param name="doc">The document.</param>
        public SimplePagerAdapter(Context baseContext, Document doc)
        {
            this.document = doc;
            this.baseContext = baseContext;

            views = new ImageView[doc.Pages.Count];
            bitmaps = new Bitmap[doc.Pages.Count];

            cacheDirName = "PagesCache";

            syncObject = new object();

            InitializeCache();
        }

        #endregion

        #region members

        public override int GetItemPosition(Java.Lang.Object @object)
        {
            int index = -1;

            for (int i = 0; i < views.Length; ++i)
            {
                if (views[i] == @object)
                {
                    index = i;
                    break;
                }
            }

            if (index == -1)
                return PositionNone;
            else
                return index;
        }

        /// <summary>
        /// Initializes the cache. Removes cached data or creates cache dir if needed.
        /// </summary>
        private void InitializeCache()
        {
            try
            {
                string cacheDirectory = this.GetCacheDirName();

                // remove all files and go
                if (Directory.Exists(cacheDirectory))
                {
                    foreach (string fileName in Directory.GetFiles(cacheDirectory))
                    {
                        File.Delete(fileName);
                    }
                }
                else
                {
                    // create the dir
                    Directory.CreateDirectory(cacheDirectory);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("cache initialization: " + e.Message);
            }
        }

        public override Java.Lang.Object InstantiateItem(ViewGroup container, int position)
        {
            ImageView imageView;

            lock (syncObject)
            {
                // create view
                imageView = new ImageView(baseContext);
                views[position] = imageView;

                // prepare page bitmap
                Bitmap bm = GetPageBitmap(position);

                ReplaceBitmap(position, bm);

                imageView.SetImageBitmap(bm);

                container.AddView(imageView);
            }

            // proceed with rendering
            SchedulePageRendering(position);

            return imageView;
        }

        public override void DestroyItem(ViewGroup container, int position, Java.Lang.Object @object)
        {
            Console.WriteLine("Destroying item");

            lock (syncObject)
            {
                // get the view and remove from container
                ImageView currentView = views[position];

                container.RemoveView(currentView);

                // clean view
                currentView.SetImageBitmap(null);

                views[position] = null;

                // dispose the corresponding bitmap
                ReplaceBitmap(position, null);
            }
        }

        /// <summary>
        /// Writes the bitmap to cache.
        /// </summary>
        /// <param name="bitmap">Current bitmap.</param>
        /// <param name="position">Position.</param>
        private void WriteBitmapToCache(Bitmap bitmap, int position)
        {
            if (bitmap != null)
            {
                try
                {
                    using (Stream outputStream = File.Create(this.GetCacheFileName(position)))
                    {
                        // tune it for better results
                        bitmap.Compress(Bitmap.CompressFormat.Png, 90, outputStream);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
        }

        public override int Count
        {
            get { return document.Pages.Count; }
        }


        public override bool IsViewFromObject(View view, Java.Lang.Object obj)
        {
            return view == obj;
        }       

        /// <summary>
        /// Gets the name of the cache dir, uses subkey to store pages' data.
        /// </summary>
        /// <returns>The cache dir name.</returns>
        private string GetCacheDirName()
        {
            return System.IO.Path.Combine(baseContext.CacheDir.AbsolutePath, cacheDirName);
        }

        /// <summary>
        /// Gets the name of the cache file for given page number.
        /// </summary>
        /// <returns>The cache file name.</returns>
        /// <param name="pos">Position.</param>
        private string GetCacheFileName(int pos)
        {
            return System.IO.Path.Combine(GetCacheDirName(), pos.ToString());
        }

        /// <summary>
        /// Gets the page bitmap base. A white page that will be used to provide an instant feedback to the user.
        /// </summary>
        /// <returns>The page bitmap.</returns>
        /// <param name="pos">page number.</param>
        private Bitmap GetPageBitmap(int pos)
        {
            // create an empty white page
            Page currentPage = document.Pages[pos];

            int width = (int) currentPage.Width;
            int height = (int) currentPage.Height;

            Bitmap result = BitmapHelper.CreateMutableBitmapARGB32(width, height, 0xFFFFFFFF);

            return result;
        }

        private void SchedulePageRendering(int pos)
        {
            // schedule rendering or load from cache procedure
            ThreadPool.QueueUserWorkItem((p) =>
                                             {
                                                 if (pos >= 0)
                                                 {
                                                     Page currentPage = document.Pages[pos];

                                                     int width = (int) currentPage.Width;
                                                     int height = (int) currentPage.Height;

                                                     // Phase I, try to load the file first
                                                     try
                                                     {
                                                         string cachedFilePath = GetCacheFileName(pos);

                                                         if (File.Exists(cachedFilePath))
                                                         {
                                                             using (FileStream fs = File.OpenRead(cachedFilePath))
                                                             {
                                                                 Bitmap loadedBitmap = BitmapFactory.DecodeStream(fs);

                                                                 if (loadedBitmap != null)
                                                                 {
                                                                     lock (syncObject)
                                                                     {
                                                                         Console.WriteLine("loaded cached instance");

                                                                         ImageView currentView = views[pos];

                                                                         // if view exists update it, otherwise dismiss the bitmap
                                                                         if (currentView != null)
                                                                         {
                                                                             currentView.Post(() =>
                                                                                                  {
                                                                                                      lock (syncObject)
                                                                                                      {
                                                                                                          ReplaceBitmap(pos,loadedBitmap);
                                                                                                          currentView.SetImageBitmap(loadedBitmap);
                                                                                                          currentView.Invalidate();
                                                                                                      }
                                                                                                  });
                                                                         }
                                                                         else
                                                                         {
                                                                             DismissBitmap(loadedBitmap);
                                                                         }
                                                                     }

                                                                     return;
                                                                 }
                                                             }
                                                         }
                                                     }
                                                     catch (Exception e)
                                                     {
                                                         Console.WriteLine("can't load cached bitmap, re-rendering: " +
                                                                           e.Message);
                                                     }

                                                     try
                                                     {
                                                         Console.WriteLine("creating new instance");

                                                         // phase II, re-rendering
                                                         // render the page using default settings
                                                         int[] bitmapData = new int[width*height];

                                                         ManualResetEvent me = null;

                                                         if (ProgressiveRendering)
                                                         {
                                                             me = new ManualResetEvent(false);

                                                             ThreadPool.QueueUserWorkItem((a) =>
                                                                                              {
                                                                                                  Thread.Sleep(500);

                                                                                                  while (!me.WaitOne(0))
                                                                                                  {
                                                                                                      lock (syncObject)
                                                                                                      {
                                                                                                          ImageView currentView = views[pos];

                                                                                                          if (currentView !=null)
                                                                                                          {
                                                                                                              Bitmap targetBitmap = bitmaps[pos];

                                                                                                              BitmapHelper.CopyPixelDataToBitmap(targetBitmap,bitmapData,width);

                                                                                                              currentView.PostInvalidate();
                                                                                                          }
                                                                                                          else
                                                                                                          {
                                                                                                              return;
                                                                                                          }
                                                                                                      }

                                                                                                      Thread.Sleep(200);
                                                                                                  }
                                                                                              });
                                                         }


                                                         int[] renderedImageData = currentPage.RenderAsInts(width,height,bitmapData,new RenderingSettings());

                                                         if (me != null)
                                                         {
                                                             me.Set();
                                                         }

                                                         lock (syncObject)
                                                         {
                                                             ImageView currentView = views[pos];

                                                             if (currentView != null && renderedImageData != null)
                                                             {
                                                                 Bitmap targetBitmap = bitmaps[pos];

                                                                 BitmapHelper.CopyPixelDataToBitmap(targetBitmap, renderedImageData,
                                                                                        width);

                                                                 currentView.PostInvalidate();

                                                                 WriteBitmapToCache(targetBitmap, pos);
                                                             }
                                                         }
                                                     }
                                                     catch (Exception e)
                                                     {
                                                         Console.WriteLine("page rendering error: " + e.Message);
                                                     }
                                                 }
                                             });
        }

        private void DismissBitmap(Bitmap bitmap)
        {
            if (bitmap != null)
            {
                bitmap.Recycle();
                bitmap.Dispose();

                GC.Collect();
            }
        }

        private void ReplaceBitmap(int position, Bitmap replacementBitmap)
        {
            DismissBitmap(bitmaps[position]);

            bitmaps[position] = replacementBitmap;
        }               
       
        #endregion
    }
}