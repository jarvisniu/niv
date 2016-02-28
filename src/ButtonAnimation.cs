/**
 * BgColorAnimator 0.1.0 - WPF背景色渐变动画
 * 牛俊为 | Jarvis Niu - www.jarvisniu.com
 * Licence MIT
 */
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace com.jarvisniu
{
    class ButtonAnimator
    {
        private Color BG_COLOR_DEFAULT = Color.FromArgb(0, 0, 0, 0);
        private Color BG_COLOR_HOVER = Color.FromArgb(128, 192, 192, 192);
        private Color BG_COLOR_DOWN = Color.FromArgb(128, 64, 64, 64);
        private Color BG_COLOR_DEFAULT_ON = Color.FromRgb(160, 160, 160);

        private double TIME_HOVER = 0.2;
        private double TIME_DOWN = 0.01;
        private double TIME_UP = 0.3;

        private bool on = true;

        private SolidColorBrush brush = new SolidColorBrush();

        private ColorAnimation colorAnimationMouseHover;
        private ColorAnimation colorAnimationMouseLeave;
        private ColorAnimation colorAnimationMouseDown;
        private ColorAnimation colorAnimationMouseUp;

        private PropertyPath pathBorderBgColor = new PropertyPath(Border.BackgroundProperty
            + "." + SolidColorBrush.ColorProperty);
        private PropertyPath pathBorderBorderColor = new PropertyPath(Border.BorderBrushProperty
            + "." + SolidColorBrush.ColorProperty
            );

        private Storyboard storyboardMouseOn = new Storyboard();
        private Storyboard storyboardMouseOff = new Storyboard();
        private Storyboard storyboardMouseDown = new Storyboard();
        private Storyboard storyboardMouseUp = new Storyboard();

        public ButtonAnimator(bool _on)
        {
            on = _on;

            colorAnimationMouseHover = new ColorAnimation();
            colorAnimationMouseHover.To = BG_COLOR_HOVER;
            colorAnimationMouseHover.Duration = new Duration(TimeSpan.FromSeconds(TIME_HOVER));

            colorAnimationMouseLeave = new ColorAnimation();
            colorAnimationMouseLeave.To = on ? BG_COLOR_DEFAULT_ON : BG_COLOR_DEFAULT;
            colorAnimationMouseLeave.Duration = new Duration(TimeSpan.FromSeconds(TIME_HOVER));

            colorAnimationMouseDown = new ColorAnimation();
            colorAnimationMouseDown.To = BG_COLOR_DOWN;
            colorAnimationMouseDown.Duration = new Duration(TimeSpan.FromSeconds(TIME_DOWN));

            colorAnimationMouseUp = new ColorAnimation();
            colorAnimationMouseUp.To = BG_COLOR_HOVER;
            colorAnimationMouseUp.Duration = new Duration(TimeSpan.FromSeconds(TIME_UP));

            Storyboard.SetTargetProperty(colorAnimationMouseHover, pathBorderBgColor);
            Storyboard.SetTargetProperty(colorAnimationMouseLeave, pathBorderBgColor);
            Storyboard.SetTargetProperty(colorAnimationMouseDown, pathBorderBgColor);
            Storyboard.SetTargetProperty(colorAnimationMouseUp, pathBorderBgColor);

            storyboardMouseOn.Children.Add(colorAnimationMouseHover);
            storyboardMouseOff.Children.Add(colorAnimationMouseLeave);
            storyboardMouseDown.Children.Add(colorAnimationMouseDown);
            storyboardMouseUp.Children.Add(colorAnimationMouseUp);

        }

        public ButtonAnimator apply(Border border)
        {
            if (border.Background == null) border.Background = new SolidColorBrush(BG_COLOR_DEFAULT);

            border.MouseEnter += borderGradient1_MouseEnter;
            border.MouseLeave += borderGradient1_MouseLeave;
            border.MouseDown += borderGradient1_MouseDown;
            border.MouseUp += borderGradient1_MouseUp;
            border.TouchDown += borderGradient1_TouchDown;
            border.TouchUp += borderGradient1_TouchUp;

            return this;
        }

        public ButtonAnimator init(Border border)
        {
            border.Background.SetCurrentValue(
                SolidColorBrush.ColorProperty,
                on ? BG_COLOR_DEFAULT_ON : BG_COLOR_DEFAULT
            );

            return this;
        }

        public ButtonAnimator cancel(Border border)
        {
            border.MouseEnter -= borderGradient1_MouseEnter;
            border.MouseLeave -= borderGradient1_MouseLeave;
            border.MouseDown -= borderGradient1_MouseDown;
            border.MouseUp -= borderGradient1_MouseUp;
            border.TouchDown -= borderGradient1_TouchDown;
            border.TouchUp -= borderGradient1_TouchUp;

            return this;
        }

        private void borderGradient1_MouseEnter(object sender, MouseEventArgs e)
        {

            Storyboard.SetTarget(colorAnimationMouseHover, ((FrameworkElement)sender));
            storyboardMouseOn.Begin();
        }

        private void borderGradient1_MouseLeave(object sender, MouseEventArgs e)
        {
            Storyboard.SetTarget(colorAnimationMouseLeave, (FrameworkElement)sender);
            storyboardMouseOff.Begin();
        }

        private void borderGradient1_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Storyboard.SetTarget(colorAnimationMouseDown, (FrameworkElement)sender);
            storyboardMouseDown.Begin();
        }

        private void borderGradient1_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Storyboard.SetTarget(colorAnimationMouseUp, (FrameworkElement)sender);
            storyboardMouseUp.Begin();
        }

        private void borderGradient1_TouchDown(object sender, TouchEventArgs e)
        {
            Storyboard.SetTarget(colorAnimationMouseDown, (FrameworkElement)sender);
            storyboardMouseDown.Begin();
        }

        private void borderGradient1_TouchUp(object sender, TouchEventArgs e)
        {
            Storyboard.SetTarget(colorAnimationMouseLeave, (FrameworkElement)sender);
            storyboardMouseOff.Begin();
        }
        //  end of class
    }
}
