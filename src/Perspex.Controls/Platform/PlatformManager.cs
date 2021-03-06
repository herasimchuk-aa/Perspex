﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Text;
using System.Threading.Tasks;
using Perspex.Input;
using Perspex.Input.Raw;
using Perspex.Media;
using Perspex.Media.Imaging;
using Perspex.Platform;

namespace Perspex.Controls.Platform
{
    public static partial class PlatformManager
    {

        static IPlatformSettings GetSettings()
            => PerspexLocator.Current.GetService<IPlatformSettings>();

        static bool s_designerMode;
        private static double _designerScalingFactor = 1;

        public static IRenderTarget CreateRenderTarget(ITopLevelImpl window)
        {
            return
                new RenderTargetDecorator(
                    PerspexLocator.Current.GetService<IPlatformRenderInterface>().CreateRenderer(window.Handle), window);
        }

        public static IDisposable DesignerMode()
        {
            s_designerMode = true;
            return Disposable.Create(() => s_designerMode = false);
        }

        public static void SetDesignerScalingFactor(double factor)
        {
            _designerScalingFactor = factor;
        }

        class RenderTargetDecorator : IRenderTarget
        {
            private readonly IRenderTarget _target;
            private readonly ITopLevelImpl _window;

            public RenderTargetDecorator(IRenderTarget target, ITopLevelImpl window)
            {
                _target = target;
                var dec = window as WindowDecorator;
                if (dec != null)
                    window = dec.TopLevel;
                _window = window;
            }

            public void Dispose() => _target.Dispose();

            public DrawingContext CreateDrawingContext()
            {
                var cs = _window.ClientSize;
                var ctx = _target.CreateDrawingContext();
                var factor = _window.Scaling;
                if (factor != 1)
                {
                    ctx.PushPostTransform(Matrix.CreateScale(factor, factor));
                    ctx.PushTransformContainer();
                }
                return ctx;
            }
        }

        class WindowDecorator : IPopupImpl, IWindowImpl
        {
            private readonly ITopLevelImpl _tl;
            private readonly IWindowImpl _window;
            private readonly IPopupImpl _popup;

            public ITopLevelImpl TopLevel => _tl;

            public WindowDecorator(ITopLevelImpl tl)
            {
                _tl = tl;
                _window = tl as IWindowImpl;
                _popup = tl as IPopupImpl;
                _tl.Input = OnInput;
                _tl.Paint = OnPaint;
                _tl.Resized = OnResized;
            }

            private void OnResized(Size size)
            {
                Resized?.Invoke(size/Scaling);
            }

            private void OnPaint(Rect rc)
            {
                var f = Scaling;
                Paint?.Invoke(new Rect(rc.X/f, rc.Y/f, rc.Width/f, rc.Height/f));
            }

            private void OnInput(RawInputEventArgs obj)
            {
                var mouseEvent = obj as RawMouseEventArgs;
                if (mouseEvent != null)
                    mouseEvent.Position /= Scaling;
                //TODO: Transform event coordinates
                Input?.Invoke(obj);
            }

            public Point PointToClient(Point point)
            {
                return _tl.PointToClient(point / Scaling) * Scaling;
            }

            public Point PointToScreen(Point point)
            {
                return _tl.PointToScreen(point * Scaling) / Scaling;
            }

            public void Invalidate(Rect rc)
            {
                var f = Scaling;
                _tl.Invalidate(new Rect(rc.X*f, rc.Y*f, (rc.Width + 1)*f, (rc.Height + 1)*f));
            } 

            public Size ClientSize
            {
                get { return _tl.ClientSize/Scaling; }
                set { _tl.ClientSize = value*Scaling; }
            }

            public Size MaxClientSize => _window.MaxClientSize/Scaling;
            public double Scaling => _tl.Scaling;
            public Action<RawInputEventArgs> Input { get; set; }
            public Action<Rect> Paint { get; set; }
            public Action<Size> Resized { get; set; }

            public Action Activated
            {
                get { return _tl.Activated; }
                set { _tl.Activated = value; }
            }

            public Action Closed
            {
                get { return _tl.Closed; }
                set { _tl.Closed = value; }
            }

            public Action Deactivated
            {
                get { return _tl.Deactivated; }
                set { _tl.Deactivated = value; }
            }

            public WindowState WindowState
            {
                get { return _window.WindowState; }
                set { _window.WindowState = value; }
            }

            public Action<double> ScalingChanged
            {
                get { return _tl.ScalingChanged; }
                set { _tl.ScalingChanged = value; }
            }

            public void Dispose() => _tl.Dispose();

            public IPlatformHandle Handle => _tl.Handle;
            public void Activate() => _tl.Activate();
            public void SetInputRoot(IInputRoot inputRoot) => _tl.SetInputRoot(inputRoot);
            
            public void SetCursor(IPlatformHandle cursor) => _tl.SetCursor(cursor);

            public void SetTitle(string title) => _window.SetTitle(title);

            public void Show() => _tl.Show();
            public void BeginMoveDrag() => _tl.BeginMoveDrag();
            public void BeginResizeDrag(WindowEdge edge) => _tl.BeginResizeDrag(edge);

            public Point Position
            {
                get { return _tl.Position; }
                set { _tl.Position = value; }
            }

            public IDisposable ShowDialog() => _window.ShowDialog();

            public void Hide() => _popup.Hide();
            public void SetSystemDecorations(bool enabled) => _window.SetSystemDecorations(enabled);
        }

        public static IWindowImpl CreateWindow()
        {
            var platform = PerspexLocator.Current.GetService<IWindowingPlatform>();
            
            if (platform == null)
            {
                throw new Exception("Could not CreateWindow(): IWindowingPlatform is not registered.");
            }

            var window = s_designerMode ? platform.CreateEmbeddableWindow() : platform.CreateWindow();
            return new WindowDecorator(window);
        }

        public static IPopupImpl CreatePopup()
        {
            return new WindowDecorator(PerspexLocator.Current.GetService<IWindowingPlatform>().CreatePopup());
        }
    }
}
