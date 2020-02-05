﻿using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace Fasetto.Word
{
    /// <summary>
    /// Helpers to animate framework elements in specific ways
    /// </summary>
    public static class FrameworkElementAnimations
    {
        /// <summary>
        /// Slide an element in from the right
        /// </summary>
        /// <param name="element"> The element to animate </param>
        /// <param name="seconds"> The time the animation will take </param>
        /// <param name="keepMargin"> Whether to keep the elemnt at the same widyh during animation </param>
        /// <returns></returns>
        public static async Task SlideAndFadeInFromRightAsync(this FrameworkElement element, float seconds = 0.3f, bool keepMargin = true)
        {
            // Create the storyboard
            var sb = new Storyboard();

            // Add slide from right animation
            sb.AddSlideFromRight(seconds, element.ActualWidth, keepMargin: keepMargin);

            // Add fade in animation
            sb.AddFadeIn(seconds);

            // Start animating 
            sb.Begin(element);

            // Make page visible
            element.Visibility = Visibility.Visible;

            // Wait fo it to finish
            await Task.Delay((int)(seconds * 1000));
        }

        /// <summary>
        /// Slide an element out to the right
        /// </summary>
        /// <param name="element"> The element to animate </param>
        /// <param name="seconds"> The time the animation will take </param>
        /// <param name="keepMargin"> Whether to keep the elemnt at the same widyh during animation </param>
        /// <returns></returns>
        public static async Task SlideAndFadeOutToRightAsync(this FrameworkElement element, float seconds = 0.3f, bool keepMargin = true)
        {
            // Create the storyboard
            var sb = new Storyboard();

            // Add slide to left animation
            sb.AddSlideToRight(seconds, element.ActualWidth, keepMargin: keepMargin);

            // Add fade out animation
            sb.AddFadeOut(seconds);

            // Start animating 
            sb.Begin(element);

            // Make page visible
            element.Visibility = Visibility.Visible;

            // Wait fo it to finish
            await Task.Delay((int)(seconds * 1000));
        }

        /// <summary>
        /// Slide an element out to the left
        /// </summary>
        /// <param name="element"> The element to animate </param>
        /// <param name="seconds"> The time the animation will take </param>
        /// <param name="keepMargin"> Whether to keep the elemnt at the same widyh during animation </param>
        /// <returns></returns>
        public static async Task SlideAndFadeOutToLeftAsync(this FrameworkElement element, float seconds = 0.3f, bool keepMargin = true)
        {
            // Create the storyboard
            var sb = new Storyboard();

            // Add slide to left animation
            sb.AddSlideToLeft(seconds, element.ActualWidth, keepMargin: keepMargin);

            // Add fade out animation
            sb.AddFadeOut(seconds);

            // Start animating 
            sb.Begin(element);

            // Make page visible
            element.Visibility = Visibility.Visible;

            // Wait fo it to finish
            await Task.Delay((int)(seconds * 1000));
        }

        /// <summary>
        /// Slide an element in from the left
        /// </summary>
        /// <param name="element"> The element to animate </param>
        /// <param name="seconds"> The time the animation will take </param>
        /// <param name="keepMargin"> Whether to keep the elemnt at the same widyh during animation </param>
        /// <returns></returns>
        public static async Task SlideAndFadeInFromLeftAsync(this FrameworkElement element, float seconds = 0.3f, bool keepMargin = true)
        {
            // Create the storyboard
            var sb = new Storyboard();

            // Add slide from right animation
            sb.AddSlideFromLeft(seconds, element.ActualWidth, keepMargin: keepMargin);

            // Add fade in animation
            sb.AddFadeIn(seconds);

            // Start animating 
            sb.Begin(element);

            // Make page visible
            element.Visibility = Visibility.Visible;

            // Wait fo it to finish
            await Task.Delay((int)(seconds * 1000));
        }
    }
}
