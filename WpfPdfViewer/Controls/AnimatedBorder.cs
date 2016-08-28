using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace Apitron.WpfPdfViewer.Controls
{
    [TemplatePart(Name = "PART_Rectangle", Type = typeof(Rectangle))]
    public class AnimatedBorder : ContentControl
    {
        private DoubleAnimation borderAnimation;

        static AnimatedBorder()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(AnimatedBorder),
                new FrameworkPropertyMetadata(typeof(AnimatedBorder)));
        }

        public AnimatedBorder()
        {
            borderAnimation = new DoubleAnimation();
            borderAnimation.From = 0.0;
            borderAnimation.To = StrokeDashArray.Single() * 2;
            borderAnimation.Duration = new Duration(TimeSpan.FromSeconds(1.0));
            borderAnimation.AutoReverse = false;
            borderAnimation.RepeatBehavior = RepeatBehavior.Forever;
        }

        public Brush BorderBrush
        {
            get { return (Brush) GetValue(BorderBrushProperty); }
            set { SetValue(BorderBrushProperty, value); }
        }

        public static readonly DependencyProperty BorderBrushProperty = DependencyProperty.Register("BorderBrush",
            typeof(Brush), typeof(AnimatedBorder), new UIPropertyMetadata(Brushes.Red));

        public double BorderThickness
        {
            get { return (double) GetValue(BorderThicknessProperty); }
            set { SetValue(BorderThicknessProperty, value); }
        }

        public static readonly DependencyProperty BorderThicknessProperty =
            DependencyProperty.Register("BorderThickness", typeof(double), typeof(AnimatedBorder),
                new UIPropertyMetadata(1D));

        // Only DoubleCollections with one single item are supported.
        public DoubleCollection StrokeDashArray
        {
            get { return (DoubleCollection) GetValue(StrokeDashArrayProperty); }
            set { SetValue(StrokeDashArrayProperty, value); }
        }

        public static readonly DependencyProperty StrokeDashArrayProperty =
            DependencyProperty.Register("StrokeDashArray", typeof(DoubleCollection), typeof(AnimatedBorder),
                new UIPropertyMetadata(new DoubleCollection() {2}));

        protected override void OnMouseEnter(MouseEventArgs e)
        {
            if (!IsFocused)
            {
                StartAnimation();
            }

            base.OnMouseEnter(e);
        }

        protected override void OnMouseLeave(MouseEventArgs e)
        {
            if (!this.IsFocused)
            {
                StopAnimation();
            }

            base.OnMouseLeave(e);
        }

        protected override void OnGotFocus(RoutedEventArgs e)
        {
            StartAnimation();

            base.OnGotFocus(e);
        }

        private void StartAnimation()
        {
            Rectangle PART_Rectangle = this.GetTemplateChild("PART_Rectangle") as Rectangle;
            if (PART_Rectangle != null)
            {
                PART_Rectangle.Visibility = Visibility.Visible;
                PART_Rectangle.BeginAnimation(Shape.StrokeDashOffsetProperty, borderAnimation);
            }
        }

        protected override void OnLostFocus(RoutedEventArgs e)
        {
            StopAnimation();

            base.OnLostFocus(e);
        }

        private void StopAnimation()
        {
            Rectangle PART_Rectangle = this.GetTemplateChild("PART_Rectangle") as Rectangle;
            if (PART_Rectangle != null)
            {
                    PART_Rectangle.Visibility = Visibility.Hidden;
                    PART_Rectangle.BeginAnimation(Shape.StrokeDashOffsetProperty, null);
            }
        }
    }
}
