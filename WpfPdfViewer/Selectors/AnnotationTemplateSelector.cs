using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using Apitron.WpfPdfViewer.Annotations;

namespace Apitron.WpfPdfViewer.Selectors
{
    public class AnnotationTemplateSelector:DataTemplateSelector
    {
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            FrameworkElement element = container as FrameworkElement;

            if (element != null && item != null && item is AnnotationViewModel)
            {

                if (item is StarAnnotationViewModel || item is SmileAnnotationViewModel)
                    return
                        element.FindResource("ImageBasedAnnotationDataTemplate") as DataTemplate;
                else if(item is TextAnnotationViewModel)
                    return
                        element.FindResource("TextBasedAnnotationDataTemplate") as DataTemplate;
            }

            return null;
        }
    }
}
