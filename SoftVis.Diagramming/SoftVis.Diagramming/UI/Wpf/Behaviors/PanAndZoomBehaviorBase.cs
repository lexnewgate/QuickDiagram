﻿using System.Windows;
using System.Windows.Input;
using System.Windows.Interactivity;
using Codartis.SoftVis.UI.Common;
using Codartis.SoftVis.UI.Wpf.Commands;

namespace Codartis.SoftVis.UI.Wpf.Behaviors
{
    internal abstract class PanAndZoomBehaviorBase : Behavior<FrameworkElement>
    {
        public static readonly DependencyProperty PanCommandProperty =
            DependencyProperty.Register("PanCommand", typeof(ICommand), typeof(PanAndZoomBehaviorBase));

        public static readonly DependencyProperty ZoomCommandProperty =
            DependencyProperty.Register("ZoomCommand", typeof(ICommand), typeof(PanAndZoomBehaviorBase));

        public static readonly DependencyProperty MinZoomProperty =
            DependencyProperty.Register("MinZoom", typeof(double), typeof(PanAndZoomBehaviorBase));

        public static readonly DependencyProperty MaxZoomProperty =
            DependencyProperty.Register("MaxZoom", typeof(double), typeof(PanAndZoomBehaviorBase));

        public static readonly DependencyProperty ZoomValueProperty =
            DependencyProperty.Register("ZoomValue", typeof(double), typeof(PanAndZoomBehaviorBase));

        public ICommand PanCommand
        {
            get { return (ICommand)GetValue(PanCommandProperty); }
            set { SetValue(PanCommandProperty, value); }
        }

        public ICommand ZoomCommand
        {
            get { return (ICommand)GetValue(ZoomCommandProperty); }
            set { SetValue(ZoomCommandProperty, value); }
        }

        public double MinZoom
        {
            get { return (double)GetValue(MinZoomProperty); }
            set { SetValue(MinZoomProperty, value); }
        }

        public double MaxZoom
        {
            get { return (double)GetValue(MaxZoomProperty); }
            set { SetValue(MaxZoomProperty, value); }
        }

        public double ZoomValue
        {
            get { return (double)GetValue(ZoomValueProperty); }
            set { SetValue(ZoomValueProperty, value); }
        }

        protected void Pan(Vector vector)
        {
            PanCommand?.Execute(vector);
        }

        protected void Zoom(ZoomDirection direction, double amount, Point center)
        {
            var commandParameters = new ZoomCommandParameters(direction, amount, center);
            ZoomCommand?.Execute(commandParameters);
        }

        protected bool IsZoomLimitReached(ZoomDirection zoomDirection)
        {
            return (zoomDirection == ZoomDirection.In && ZoomValue >= MaxZoom) ||
                  (zoomDirection == ZoomDirection.Out && ZoomValue <= MinZoom);
        }

    }
}
