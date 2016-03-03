using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace Niv
{
    class Controller
    {
        // Reference to the window and transformer
        private Transformer transformer;
        private NivWindow window;

        // The mouse postion when moving.
        private Point mousePos;

        // The mouse position when mouse button is down
        private Point mouseDownAt;

        // The left mouse button state.
        public bool isLeftButtonDown = false;

        // If the user moved the mouse in the process of click
        // but the distance is within this threshold. Then we also think it's a valid click.
        private static int CLICK_MOVE_THRESHOLD = 16;

        private static int DOUBLE_CLICK_TIME_THRESHOLD = 750;

        // The mouse moved distance during the click.
        private double movedDistanceOfClick = 0;

        // Count the click times to invoke double click event.
        private int clickCount = 0;

        // Timestamp of last mousedown or mouseup
        private int lastMouseUpTimestamp = 0;

        // Constructor
        public Controller(NivWindow window,Transformer transformer)
        {
            this.window = window;
            this.transformer = transformer;
        }

        public void onMouseMove(Point pos)
        {
            movedDistanceOfClick += Math.Abs(mousePos.X - pos.X);
            movedDistanceOfClick += Math.Abs(mousePos.Y - pos.Y);
            mousePos = pos;
        }

        public void onMouseLeftDown(int timestamp)
        {
            isLeftButtonDown = true;
            mouseDownAt = mousePos;
            if (clickCount == 0) movedDistanceOfClick = 0;

            // Abort the double click if too late
            if (timestamp - lastMouseUpTimestamp > DOUBLE_CLICK_TIME_THRESHOLD) clickCount = 0;
        }

        public void onMouseLeftUp(int timestamp)
        {
            if (movedDistanceOfClick < CLICK_MOVE_THRESHOLD)
            {
                clickCount++;
                if (clickCount == 2)
                {
                    clickCount = 0;
                    window.toggleFullscreen();
                }
            }
            else
            {
                clickCount = 0;
            }
            isLeftButtonDown = false;
            lastMouseUpTimestamp = timestamp;
        }

        public void onMouseDoubleClick()
        {
            if (transformer.isFitWindow)
                transformer.zoomTo(1, mousePos).animate();
            else
                transformer.fitWindow().animate();
        }

        public void onDragMove()
        {
            double dX = mousePos.X - mouseDownAt.X;
            double dY = mousePos.Y - mouseDownAt.Y;

            transformer.pan(dX, dY).apply().setScaleCenterWithImage();


            mouseDownAt.X = mousePos.X;
            mouseDownAt.Y = mousePos.Y;
        }

        public void onMouseWheel(bool up)
        {
            double dS = up ? 1.25 : 0.8;
            transformer.zoomBy(dS, mousePos).animate();
        }

        // EOC
    }
}
