using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Apitron.PDF.Kit.FixedLayout;
using Apitron.PDF.Kit.FixedLayout.Resources.Fonts;
using Apitron.PDF.Kit.FixedLayout.Resources.XObjects;
using Apitron.PDF.Kit.Styles;
using Apitron.PDF.Kit.Styles.Appearance;
using Font = Apitron.PDF.Kit.Styles.Text.Font;

namespace Apitron.WpfPdfViewer.Annotations
{
    public static class AnnotationsHelper
    {
        /// <summary>
        /// Creates a fixed content object that contains
        /// drawing instructions for normal annotation state.       
        /// </summary>       
        public static FixedContent CreateNormalAppearance(string text, double width,
            double height)
        {
            // create fixed content object, set its unique ID using guid.
            // this object will be implicitly added to page resources using this ID.
            FixedContent fixedContent = new FixedContent(Guid.NewGuid().ToString("N"), new Boundary(0, 0, width, height));

            // use text block from flow layout API subset,
            // to quickly draw text in a fixed content container.
            Apitron.PDF.Kit.FlowLayout.Content.TextBlock textBlock = new Apitron.PDF.Kit.FlowLayout.Content.TextBlock(text);
            textBlock.Font = new Font(StandardFonts.Helvetica, 12);
            textBlock.Display = Display.Block;
            textBlock.Color = RgbColors.White;
            textBlock.Width = width;
            textBlock.Height = height;
            textBlock.Background = RgbColors.Green;

            fixedContent.Content.AppendContentElement(textBlock, width, height);
            return fixedContent;
        }
    }
}
