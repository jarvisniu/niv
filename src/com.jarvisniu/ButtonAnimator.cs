/**
 * ButtonAnimation - Easily add background easing transform effect to you buttons.
 * Jarvis Niu(牛俊为) - http://jarvisniu.com/
 * MIT Licence
 * 
 * ## Usage
 *     1. Create a instance of ButtonAnimator
 *         private ButtonAnimator buttonAnimator = new ButtonAnimator();
 *     2. Apply it to a button (instance of `System.Windows.Controls.Border` class)
 *         buttonAnimator.apply(btnOpen);
 *     3. Change the theme predefined in `setThemes()`
 *         buttonAnimator.setTheme(themeName);
 */

using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace com.jarvisniu
{
    class ButtonAnimator
    {
        // Save colors of themes
        private class ThemeColors
        {
            public Color bgColorDefault;
            public Color bgColorHover;
            public Color bgColorDown;
        };

        // Colors of each themes
        private Dictionary<string, ThemeColors> themes = new Dictionary<string, ThemeColors>();

        // The current using theme
        private string theme;

        // Record the Borders (buttons) that have been applied to.
        private List<Border> appliedBorders = new List<Border>();

        // Animation duration times
        private double TIME_HOVER = 0.1;
        private double TIME_DOWN = 0.01;
        private double TIME_UP = 0.2;
        private double TIME_LEAVE = 0.3;

        // `PropertyPath` of background color used in `StoryBoard`
        private static PropertyPath PROPERTY_PATH_BACKGROUND_COLOR = new PropertyPath(Border.BackgroundProperty
            + "." + SolidColorBrush.ColorProperty);

        // `PropertyPath` of border color used in `StoryBoard`
        private static PropertyPath PROPERTY_PATH_BORDER_COLOR = new PropertyPath(Border.BorderBrushProperty
            + "." + SolidColorBrush.ColorProperty);

        // Animations of every actions
        private ColorAnimation colorAnimationMouseHover;
        private ColorAnimation colorAnimationMouseDown;
        private ColorAnimation colorAnimationMouseUp;
        private ColorAnimation colorAnimationMouseLeave;

        // StoryBoards of every actions
        private Storyboard storyboardMouseHover = new Storyboard();
        private Storyboard storyboardMouseDown = new Storyboard();
        private Storyboard storyboardMouseUp = new Storyboard();
        private Storyboard storyboardMouseLeave = new Storyboard();

        // Constructor
        public ButtonAnimator()
        {
            setThemes();
            initAnimations();
        }

        // Set the Theme colors
        private void setThemes()
        {
            var darkTheme = themes["dark"] = new ThemeColors();
            darkTheme.bgColorDefault = Color.FromArgb(0, 0, 0, 0);
            darkTheme.bgColorHover = Color.FromArgb(128, 192, 192, 192);
            darkTheme.bgColorDown = Color.FromArgb(128, 64, 64, 64);

            var lightTheme = themes["light"] = new ThemeColors();
            lightTheme.bgColorDefault = Color.FromArgb(255, 220, 220, 220);
            lightTheme.bgColorHover = Color.FromArgb(255, 168, 168, 168);
            lightTheme.bgColorDown = Color.FromArgb(255, 112, 112, 112);
        }

        // Initialize the `Animations` and `StoryBoards`
        private void initAnimations()
        {
            colorAnimationMouseHover = new ColorAnimation();
            colorAnimationMouseHover.Duration = new Duration(TimeSpan.FromSeconds(TIME_HOVER));

            colorAnimationMouseDown = new ColorAnimation();
            colorAnimationMouseDown.Duration = new Duration(TimeSpan.FromSeconds(TIME_DOWN));

            colorAnimationMouseUp = new ColorAnimation();
            colorAnimationMouseUp.Duration = new Duration(TimeSpan.FromSeconds(TIME_UP));

            colorAnimationMouseLeave = new ColorAnimation();
            colorAnimationMouseLeave.Duration = new Duration(TimeSpan.FromSeconds(TIME_LEAVE));

            Storyboard.SetTargetProperty(colorAnimationMouseHover, PROPERTY_PATH_BACKGROUND_COLOR);
            Storyboard.SetTargetProperty(colorAnimationMouseDown, PROPERTY_PATH_BACKGROUND_COLOR);
            Storyboard.SetTargetProperty(colorAnimationMouseUp, PROPERTY_PATH_BACKGROUND_COLOR);
            Storyboard.SetTargetProperty(colorAnimationMouseLeave, PROPERTY_PATH_BACKGROUND_COLOR);

            storyboardMouseHover.Children.Add(colorAnimationMouseHover);
            storyboardMouseDown.Children.Add(colorAnimationMouseDown);
            storyboardMouseUp.Children.Add(colorAnimationMouseUp);
            storyboardMouseLeave.Children.Add(colorAnimationMouseLeave);
        }

        // Set or change the theme of buttons
        public void setTheme(string theme)
        {
            this.theme = theme;

            colorAnimationMouseHover.To = themes[theme].bgColorHover;
            colorAnimationMouseLeave.To = themes[theme].bgColorDefault;
            colorAnimationMouseDown.To = themes[theme].bgColorDown;
            colorAnimationMouseUp.To = themes[theme].bgColorHover;

            foreach (Border border in appliedBorders)
                border.Background = new SolidColorBrush(themes[theme].bgColorDefault);
        }

        // Apply animation effect to a button(`Border` class).
        public ButtonAnimator apply(Border border)
        {
            if (border.Background == null)
                border.Background = new SolidColorBrush(themes[theme].bgColorDefault);

            border.MouseEnter += border_MouseEnter;
            border.MouseLeave += border_MouseLeave;
            border.MouseDown += border_MouseDown;
            border.MouseUp += border_MouseUp;
            border.TouchDown += border_TouchDown;
            border.TouchUp += border_TouchUp;

            appliedBorders.Add(border);

            return this;
        }

        // You can have two `ButtonAnimator` and switch between them.
        public ButtonAnimator cancel(Border border)
        {
            border.MouseEnter -= border_MouseEnter;
            border.MouseLeave -= border_MouseLeave;
            border.MouseDown -= border_MouseDown;
            border.MouseUp -= border_MouseUp;
            border.TouchDown -= border_TouchDown;
            border.TouchUp -= border_TouchUp;

            if (appliedBorders.Contains(border)) appliedBorders.Remove(border);

            return this;
        }

        // The public `MouseEnter` event handler of every buttons.
        private void border_MouseEnter(object sender, MouseEventArgs e)
        {
            Storyboard.SetTarget(colorAnimationMouseHover, ((FrameworkElement)sender));
            storyboardMouseHover.Begin();
        }

        // The public `MouseLeave` event handler of every buttons.
        private void border_MouseLeave(object sender, MouseEventArgs e)
        {
            Storyboard.SetTarget(colorAnimationMouseLeave, (FrameworkElement)sender);
            storyboardMouseLeave.Begin();
        }

        // The public `MouseDown` event handler of every buttons.
        private void border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Storyboard.SetTarget(colorAnimationMouseDown, (FrameworkElement)sender);
            storyboardMouseDown.Begin();
        }

        // The public `MouseUp` event handler of every buttons.
        private void border_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Storyboard.SetTarget(colorAnimationMouseUp, (FrameworkElement)sender);
            storyboardMouseUp.Begin();
        }

        // The public `TouchDown` event handler of every buttons.
        private void border_TouchDown(object sender, TouchEventArgs e)
        {
            Storyboard.SetTarget(colorAnimationMouseDown, (FrameworkElement)sender);
            storyboardMouseDown.Begin();
        }

        // The public `TouchUp` event handler of every buttons.
        private void border_TouchUp(object sender, TouchEventArgs e)
        {
            Storyboard.SetTarget(colorAnimationMouseLeave, (FrameworkElement)sender);
            storyboardMouseLeave.Begin();
        }

        // EOC
    }
}
