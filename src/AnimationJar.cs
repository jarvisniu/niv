/**
 * AnimationJar 0.3.0 - transition animation manager
 * 牛俊为(Jarvis Niu) - jarvisniu.com
 * Licence MIT
 * Log:
 *   v0.3.0 - 20150919
 *     - rotateTo()
 */
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace com.jarvisniu
{
    /**
     * AnimationJar动画瓶
     * AnimatorJar animator = new AnimatorJar();
     * animator.fadeIn(borderHome);
     * animator.fadeOut(borderHome);
     * animator.translateLeftTo(borderHome);
     * animator.translateLeftBy(borderHome);
     * animator.translateTopTo(borderHome);
     * animator.translateTopBy(borderHome);
     * 
     */

    public class AnimatorJar
    {
        // variables

        private double FADE_IN_VALUE = 1.0;
        private double FADE_OUT_VALUE = 0.0;

        // durations
        Duration DURITION_ZERO = new Duration(TimeSpan.FromSeconds(0));
        //Duration DURITION_DOT1 = new Duration(TimeSpan.FromSeconds(0.1));
        //Duration DURITION_DOT15 = new Duration(TimeSpan.FromSeconds(0.15));
        Duration DURITION_DOT2 = new Duration(TimeSpan.FromSeconds(0.2));
        //Duration DURITION_DOT25 = new Duration(TimeSpan.FromSeconds(0.25));

        private DoubleAnimation opacityAnimationFadeIn;
        private DoubleAnimation opacityAnimationFadeOut;
        private ThicknessAnimation translateAnimation;
        private DoubleAnimation rotateAnimation;
        private DoubleAnimation heightAnimation;

        private PropertyPath pathBorderOpacity = new PropertyPath(FrameworkElement.OpacityProperty);
        private PropertyPath pathBorderMargin = new PropertyPath(FrameworkElement.MarginProperty);
        private PropertyPath pathRotation = new PropertyPath("RenderTransform.Angle");
        private PropertyPath pathHeight = new PropertyPath(FrameworkElement.HeightProperty);

        private Storyboard boardFadeIn = new Storyboard();
        private Storyboard boardFadeOut = new Storyboard();
        private Storyboard boardTranslate = new Storyboard();
        private Storyboard boardRotate = new Storyboard();
        private Storyboard boardHeight = new Storyboard();

        // init

        public AnimatorJar()
        {
            opacityAnimationFadeIn = new DoubleAnimation();
            opacityAnimationFadeIn.To = FADE_IN_VALUE;
            opacityAnimationFadeIn.Duration = DURITION_DOT2;

            opacityAnimationFadeOut = new DoubleAnimation();
            opacityAnimationFadeOut.To = FADE_OUT_VALUE;
            opacityAnimationFadeOut.Duration = DURITION_DOT2;

            translateAnimation = new ThicknessAnimation();
            translateAnimation.Duration = DURITION_DOT2;

            rotateAnimation = new DoubleAnimation();
            rotateAnimation.Duration = DURITION_DOT2;
            rotateAnimation.AccelerationRatio = 0.5;

            heightAnimation = new DoubleAnimation();
            heightAnimation.Duration = DURITION_DOT2;
            heightAnimation.AccelerationRatio = 0.5;

            Storyboard.SetTargetProperty(opacityAnimationFadeIn, pathBorderOpacity);
            Storyboard.SetTargetProperty(opacityAnimationFadeOut, pathBorderOpacity);
            Storyboard.SetTargetProperty(translateAnimation, pathBorderMargin);
            Storyboard.SetTargetProperty(rotateAnimation, pathRotation);
            Storyboard.SetTargetProperty(heightAnimation, pathHeight);

            boardFadeIn.Children.Add(opacityAnimationFadeIn);
            boardFadeOut.Children.Add(opacityAnimationFadeOut);
            boardTranslate.Children.Add(translateAnimation);
            boardRotate.Children.Add(rotateAnimation);
            boardHeight.Children.Add(heightAnimation);
        }

        // APIs

        public AnimatorJar fadeIn(FrameworkElement target)
        {
            Storyboard.SetTarget(opacityAnimationFadeIn, target);
            boardFadeIn.Begin();

            return this;
        }

        public AnimatorJar fadeOut(FrameworkElement target)
        {
            Storyboard.SetTarget(opacityAnimationFadeOut, target);
            boardFadeOut.Begin();

            return this;
        }

        public AnimatorJar marginTo(FrameworkElement target, Thickness m)
        {
            translateAnimation.To = new Thickness(m.Left, m.Top, m.Right, m.Bottom);
            Storyboard.SetTarget(translateAnimation, target);
            boardTranslate.Begin();

            return this;
        }

        public AnimatorJar marginTopTo(FrameworkElement target, double des)
        {
            Thickness m = target.Margin;
            translateAnimation.To = new Thickness(m.Left, des, m.Right, m.Bottom);
            Storyboard.SetTarget(translateAnimation, target);
            boardTranslate.Begin();

            return this;
        }

        public AnimatorJar marginBottomTo(FrameworkElement target, double des)
        {
            Thickness m = target.Margin;
            translateAnimation.To = new Thickness(m.Left, m.Top, m.Right, des);
            Storyboard.SetTarget(translateAnimation, target);
            boardTranslate.Begin();

            return this;
        }

        public AnimatorJar translateTopTo(FrameworkElement target, double desTop)
        {
            Thickness m = target.Margin;
            translateAnimation.To = new Thickness(m.Left, desTop, m.Right, m.Bottom);
            Storyboard.SetTarget(translateAnimation, target);
            boardTranslate.Begin();

            return this;
        }

        public AnimatorJar translateTopToI(FrameworkElement target, double desTop)
        {
            translateAnimation.Duration = DURITION_ZERO;
            translateTopTo(target, desTop);
            translateAnimation.Duration = DURITION_DOT2;

            return this;
        }

        public AnimatorJar translateTopBy(FrameworkElement target, double deltaTop)
        {
            Thickness m = target.Margin;
            translateAnimation.To = new Thickness(m.Left, m.Top + deltaTop, m.Right, m.Bottom);
            Storyboard.SetTarget(translateAnimation, target);
            boardTranslate.Begin();

            return this;
        }

        public AnimatorJar translateTopByI(FrameworkElement target, double delta)
        {
            translateAnimation.Duration = DURITION_ZERO;
            translateTopBy(target, delta);
            translateAnimation.Duration = DURITION_DOT2;

            return this;
        }

        public AnimatorJar translateLeftTo(FrameworkElement target, double desLeft)
        {
            Thickness m = target.Margin;
            translateAnimation.To = new Thickness(desLeft, m.Top, m.Right, m.Bottom);
            Storyboard.SetTarget(translateAnimation, target);
            boardTranslate.Begin();

            return this;
        }

        public AnimatorJar translateLeftToI(FrameworkElement target, double delta)
        {
            translateAnimation.Duration = DURITION_ZERO;
            translateLeftTo(target, delta);
            translateAnimation.Duration = DURITION_DOT2;

            return this;
        }

        public AnimatorJar translateLeftBy(FrameworkElement target, double deltaLeft)
        {
            Thickness m = target.Margin;
            translateAnimation.To = new Thickness(m.Left + deltaLeft, m.Top, m.Right, m.Bottom);
            Storyboard.SetTarget(translateAnimation, target);
            boardTranslate.Begin();

            return this;
        }

        public AnimatorJar translateLeftByI(FrameworkElement target, double delta)
        {
            translateAnimation.Duration = DURITION_ZERO;
            translateLeftBy(target, delta);
            translateAnimation.Duration = DURITION_DOT2;

            return this;
        }

        public AnimatorJar rotateTo(FrameworkElement target, double angle)
        {
            if (!(target.RenderTransform is RotateTransform))
                target.RenderTransform = new RotateTransform(0);
            double currDegree = (double)target.RenderTransform.GetValue(RotateTransform.AngleProperty);
            rotateAnimation.To = angle;
            Storyboard.SetTarget(rotateAnimation, target);
            boardRotate.Begin();

            return this;
        }

        public AnimatorJar rotateToI(FrameworkElement target, double angle)
        {
            rotateAnimation.Duration = DURITION_ZERO;
            rotateTo(target, angle);
            rotateAnimation.Duration = DURITION_DOT2;

            return this;
        }

        public AnimatorJar heightTo(FrameworkElement target, double height)
        {
            heightAnimation.To = height;
            Storyboard.SetTarget(heightAnimation, target);
            boardHeight.Begin();

            return this;
        }

        //  end of class
    }
}
