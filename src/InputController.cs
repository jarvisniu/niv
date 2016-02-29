using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace Niv
{
    class InputController
    {
        MarginManager marginManager;  // TODO margin manager => image transformer
        Point mousePos;
        Point mouseDownAt;

        public bool isLeftButtonDown = false;  // todo rename to leftButtonDown

        public InputController(MarginManager manager)
        {
            marginManager = manager;
        }

        public void onMouseMove(Point pos)
        {
            mousePos = pos;
        }

        public void onMouseLeftDown()
        {
            isLeftButtonDown = true;
            mouseDownAt = mousePos;
        }

        public void onMouseLeftUp()
        {
            isLeftButtonDown = false;
        }

        public void onMouseDoubleClick()
        {
            if (marginManager.isFullwindow)
                marginManager.zoomTo(1, mousePos).animate();
            else
                marginManager.fullwindow().animate();
        }

        public void onDragMove()
        {
            double dX = mousePos.X - mouseDownAt.X;
            double dY = mousePos.Y - mouseDownAt.Y;

            marginManager.pan(dX, dY).apply().setScaleCenterWithImage();


            mouseDownAt.X = mousePos.X;
            mouseDownAt.Y = mousePos.Y;
        }

        public void onMouseWheel(bool up)
        {
            double dS = up ? 1.25 : 0.8;
            marginManager.zoomBy(dS, mousePos).animate();
        }

        // TODO delete this?
        private void zoomByAtMousePos(double deltaScale)
        {
            marginManager.zoomBy(deltaScale, mousePos).animate();
        }

        // EOC
    }
}
