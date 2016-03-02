/**
 * AnimationJar - A transition animation effect collection for WPF controls.
 * Jarvis Niu(牛俊为) - http://jarvisniu.com/
 * MIT Licence
 *
 * ## API
 *     AnimatorJar animator = new AnimatorJar();
 *
 *     animator.fadeIn(borderHome);
 *     animator.fadeOut(borderHome);
 *     animator.translateLeftTo(borderHome);
 *     animator.translateLeftBy(borderHome);
 *     animator.translateTopTo(borderHome);
 *     animator.translateTopBy(borderHome);
 *     animator.rotateTo(borderHome);
 *     animator.heightTo(borderHome);
 * 
 *     Every effect has a instant version with no transition animation effect.
 *     Their names are the same but with a additional letter `I` in the end.
 * 
 */

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace com.jarvisniu
{

    public class AnimatorJar
    {
        /// Variables ----------------------------------------------------------

        // The destinatoin value of fade-in animation
        private double FADE_IN_VALUE = 1.0;

        // The destinatoin value of fade-out animation
        private double FADE_OUT_VALUE = 0.0;

        // The animation duration constant
        private static Duration DURITION_ZERO = new Duration(TimeSpan.FromSeconds(0));
        private static Duration DURITION_DOT2 = new Duration(TimeSpan.FromSeconds(0.2));

        // The Animation objects of every effects
        private DoubleAnimation opacityAnimationFadeIn;
        private DoubleAnimation opacityAnimationFadeOut;
        private ThicknessAnimation translateAnimation;
        private DoubleAnimation rotateAnimation;
        private DoubleAnimation heightAnimation;

        // The `PropertyPath` objects of every effects
        private PropertyPath pathBorderOpacity = new PropertyPath(FrameworkElement.OpacityProperty);
        private PropertyPath pathBorderMargin = new PropertyPath(FrameworkElement.MarginProperty);
        private PropertyPath pathRotation = new PropertyPath("RenderTransform.Angle");
        private PropertyPath pathHeight = new PropertyPath(FrameworkElement.HeightProperty);

        // The `Storyboard` objects of every effects
        private Storyboard boardFadeIn = new Storyboard();
        private Storyboard boardFadeOut = new Storyboard();
        private Storyboard boardTranslate = new Storyboard();
        private Storyboard boardRotate = new Storyboard();
        private Storyboard boardHeight = new Storyboard();


        // Constructor
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

        /// All transition animation effects -----------------------------------

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
            if (Math.Abs(angle - currDegree) > 180)
            {
                if (angle > currDegree) angle -= 360;
                else angle += 360;
            }
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

        // EOC
    }
}
