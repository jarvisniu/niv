using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace Niv
{
    class Controller
    {
        private Transformer transformer;
        private Point mousePos;
        private Point mouseDownAt;

        public bool isLeftButtonDown = false;

        public Controller(Transformer transformer)
        {
            this.transformer = transformer;
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
            if (transformer.isFullwindow)
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

        // TODO delete this?
        private void zoomByAtMousePos(double deltaScale)
        {
            transformer.zoomBy(deltaScale, mousePos).animate();
        }

        // EOC
    }
}
