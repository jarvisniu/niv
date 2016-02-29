/**
 * ButtonAnimation - Easily add background easing transform effect to you buttons
 * Jarvis Niu(牛俊为) - jarvisniu.com
 * MIT Licence
 * 
 * TODO add usage
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
        class ThemeColors
        {
            public Color bgColorDefault;
            public Color bgColorHover;
            public Color bgColorDown;
        };

        private Dictionary<string, ThemeColors> themes = new Dictionary<string, ThemeColors>();
        private string theme;
        private List<Border> appliedBorders = new List<Border>();

        private double TIME_HOVER = 0.1;
        private double TIME_DOWN = 0.01;
        private double TIME_UP = 0.2;
        private double TIME_LEAVE = 0.3;

        private SolidColorBrush brush = new SolidColorBrush();

        private PropertyPath pathBorderBgColor = new PropertyPath(Border.BackgroundProperty
            + "." + SolidColorBrush.ColorProperty);
        private PropertyPath pathBorderBorderColor = new PropertyPath(Border.BorderBrushProperty
            + "." + SolidColorBrush.ColorProperty);

        private ColorAnimation colorAnimationMouseHover;
        private ColorAnimation colorAnimationMouseDown;
        private ColorAnimation colorAnimationMouseUp;
        private ColorAnimation colorAnimationMouseLeave;

        private Storyboard storyboardMouseHover = new Storyboard();
        private Storyboard storyboardMouseDown = new Storyboard();
        private Storyboard storyboardMouseUp = new Storyboard();
        private Storyboard storyboardMouseLeave = new Storyboard();

        public ButtonAnimator()
        {
            loadThemeColors();
            initAnimations();
        }

        private void loadThemeColors()
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


            Storyboard.SetTargetProperty(colorAnimationMouseHover, pathBorderBgColor);
            Storyboard.SetTargetProperty(colorAnimationMouseDown, pathBorderBgColor);
            Storyboard.SetTargetProperty(colorAnimationMouseUp, pathBorderBgColor);
            Storyboard.SetTargetProperty(colorAnimationMouseLeave, pathBorderBgColor);

            storyboardMouseHover.Children.Add(colorAnimationMouseHover);
            storyboardMouseDown.Children.Add(colorAnimationMouseDown);
            storyboardMouseUp.Children.Add(colorAnimationMouseUp);
            storyboardMouseLeave.Children.Add(colorAnimationMouseLeave);
        }

        public void setTheme(string theme)
        {
            this.theme = theme;

            colorAnimationMouseHover.To = themes[theme].bgColorHover;
            colorAnimationMouseLeave.To = themes[theme].bgColorDefault;
            colorAnimationMouseDown.To = themes[theme].bgColorDown;
            colorAnimationMouseUp.To = themes[theme].bgColorHover;

            foreach (Border border in appliedBorders)
            {
                border.Background = new SolidColorBrush(themes[theme].bgColorDefault);
            }
        }

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

        public ButtonAnimator init(Border border)
        {
            border.Background.SetCurrentValue(
                SolidColorBrush.ColorProperty,
                themes[theme].bgColorDefault);

            return this;
        }

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

        private void border_MouseEnter(object sender, MouseEventArgs e)
        {
            Storyboard.SetTarget(colorAnimationMouseHover, ((FrameworkElement)sender));
            storyboardMouseHover.Begin();
        }

        private void border_MouseLeave(object sender, MouseEventArgs e)
        {
            Storyboard.SetTarget(colorAnimationMouseLeave, (FrameworkElement)sender);
            storyboardMouseLeave.Begin();
        }

        private void border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Storyboard.SetTarget(colorAnimationMouseDown, (FrameworkElement)sender);
            storyboardMouseDown.Begin();
        }

        private void border_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Storyboard.SetTarget(colorAnimationMouseUp, (FrameworkElement)sender);
            storyboardMouseUp.Begin();
        }

        private void border_TouchDown(object sender, TouchEventArgs e)
        {
            Storyboard.SetTarget(colorAnimationMouseDown, (FrameworkElement)sender);
            storyboardMouseDown.Begin();
        }

        private void border_TouchUp(object sender, TouchEventArgs e)
        {
            Storyboard.SetTarget(colorAnimationMouseLeave, (FrameworkElement)sender);
            storyboardMouseLeave.Begin();
        }
        
        // EOC
    }
}
