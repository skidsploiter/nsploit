using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace Interface
{
    public class Animations
    {
        public static void Expand(Border border)
        {
            var originalWidth = border.ActualWidth;
            var originalHeight = border.ActualHeight;
            var originalOpacity = border.Opacity;

            border.Opacity = 0;
            border.Width = 0;
            border.Height = 0;
            border.Visibility = Visibility.Visible;

            var scaleTransform = new ScaleTransform(0, 0, originalWidth / 2, originalHeight / 2);
            border.LayoutTransform = scaleTransform;

            var widthAnimation = new DoubleAnimation
            {
                To = originalWidth,
                Duration = TimeSpan.FromSeconds(1.5),
                EasingFunction = new QuarticEase { EasingMode = EasingMode.EaseInOut },
            };

            var heightAnimation = new DoubleAnimation
            {
                To = originalHeight,
                Duration = TimeSpan.FromSeconds(1.5),
                EasingFunction = new QuarticEase { EasingMode = EasingMode.EaseInOut },
            };

            var opacityAnimation = new DoubleAnimation
            {
                To = originalOpacity,
                Duration = TimeSpan.FromSeconds(1.0),
                BeginTime = TimeSpan.FromSeconds(0.5),
            };

            opacityAnimation.Completed += (s, e) => {
                border.BeginAnimation(FrameworkElement.WidthProperty, null);
                border.BeginAnimation(FrameworkElement.HeightProperty, null);
                border.BeginAnimation(UIElement.OpacityProperty, null);
                border.Width = originalWidth;
                border.Height = originalHeight;
                border.Opacity = originalOpacity;
            };

            var scaleAnimationX = new DoubleAnimation
            {
                To = 1,
                Duration = TimeSpan.FromSeconds(1.5),
                EasingFunction = new QuarticEase { EasingMode = EasingMode.EaseInOut },
            };

            var scaleAnimationY = new DoubleAnimation
            {
                To = 1,
                Duration = TimeSpan.FromSeconds(1.5),
                EasingFunction = new QuarticEase { EasingMode = EasingMode.EaseInOut },
            };

            scaleTransform.BeginAnimation(ScaleTransform.ScaleXProperty, scaleAnimationX);
            scaleTransform.BeginAnimation(ScaleTransform.ScaleYProperty, scaleAnimationY);

            border.BeginAnimation(FrameworkElement.WidthProperty, widthAnimation);
            border.BeginAnimation(FrameworkElement.HeightProperty, heightAnimation);
            border.BeginAnimation(UIElement.OpacityProperty, opacityAnimation);
        }

        public static void Collapse(Border border)
        {
            var originalWidth = border.ActualWidth;
            var originalHeight = border.ActualHeight;
            var originalOpacity = border.Opacity;

            var scaleTransform = new ScaleTransform(1, 1, originalWidth / 2, originalHeight / 2);
            border.LayoutTransform = scaleTransform;

            var widthAnimation = new DoubleAnimation
            {
                To = 0,
                Duration = TimeSpan.FromSeconds(1.5),
                EasingFunction = new QuarticEase { EasingMode = EasingMode.EaseInOut },
            };

            var heightAnimation = new DoubleAnimation
            {
                To = 0,
                Duration = TimeSpan.FromSeconds(1.5),
                EasingFunction = new QuarticEase { EasingMode = EasingMode.EaseInOut },
            };

            var opacityAnimation = new DoubleAnimation
            {
                To = 0,
                Duration = TimeSpan.FromSeconds(1.0),
                BeginTime = TimeSpan.FromSeconds(0.5),
            };

            opacityAnimation.Completed += (s, e) => {
                border.BeginAnimation(FrameworkElement.WidthProperty, null);
                border.BeginAnimation(FrameworkElement.HeightProperty, null);
                border.BeginAnimation(UIElement.OpacityProperty, null);
                border.Visibility = Visibility.Hidden;
                border.Width = originalWidth;
                border.Height = originalHeight;
                border.Opacity = originalOpacity;
            };

            var scaleAnimationX = new DoubleAnimation
            {
                To = 0,
                Duration = TimeSpan.FromSeconds(1.5),
                EasingFunction = new QuarticEase { EasingMode = EasingMode.EaseInOut },
            };

            var scaleAnimationY = new DoubleAnimation
            {
                To = 0,
                Duration = TimeSpan.FromSeconds(1.5),
                EasingFunction = new QuarticEase { EasingMode = EasingMode.EaseInOut },
            };

            scaleTransform.BeginAnimation(ScaleTransform.ScaleXProperty, scaleAnimationX);
            scaleTransform.BeginAnimation(ScaleTransform.ScaleYProperty, scaleAnimationY);

            border.BeginAnimation(FrameworkElement.WidthProperty, widthAnimation);
            border.BeginAnimation(FrameworkElement.HeightProperty, heightAnimation);
            border.BeginAnimation(UIElement.OpacityProperty, opacityAnimation);
        }
    }
}