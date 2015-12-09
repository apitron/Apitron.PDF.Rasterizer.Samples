using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Apitron.PDF.Rasterizer;
using Apitron.PDF.Rasterizer.Configuration;

namespace LowMemoryModeUsageSample
{
    class Program
    {
        static void Main(string[] args)
        {          
                using (Stream fileStream = File.Open("../../Data/document.pdf",FileMode.Open))
                {
                // You can turn on the low memory mode globally: 
                // EngineSettings.GlobalSettings.MemoryAllocationMode = MemoryAllocationMode.ResourcesLowMemory;
                //  or
                // apply this setting only to the specific file as shown below.
                // This mode can be useful in situations where you have PDF files containing huge image
                // resources or embedded fonts and running out of memory because of this.
                EngineSettings engineSettings = new EngineSettings()
                {
                    // set the resouce size limit to 2MB,
                    // everything bigger than that will be saved to disk
                    ResourceSizeLimit = 2097152,
                    MemoryAllocationMode = MemoryAllocationMode.ResourcesLowMemory
                };

                using (Document doc = new Document(fileStream, engineSettings))
                {
                    // render page and save the result
                    Bitmap bitmap = doc.Pages[0].Render(new Resolution(72, 72), new RenderingSettings());
                    bitmap.Save("page0.png");
                }
            }

            Console.ReadLine();
        }
    }
}
