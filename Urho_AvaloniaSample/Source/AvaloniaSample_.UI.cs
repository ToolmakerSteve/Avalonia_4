using Urho;
using System;
using Urho.Gui;
using Urho.Avalonia;
using Urho.IO;
using Avalonia.Styling;
using Avalonia.Markup.Xaml.Styling;
using AC = Avalonia.Controls;
using Urho.Urho2D;
using System.Collections.Generic;

namespace AvaloniaSample
{
	public partial class AvaloniaSample : Sample
	{
        private AvaloniaUrhoContext avaloniaContext;

        private void _SetupAvaloniaUI()
        {
            _SetAvaloniaContent<ControlCatalog.App>(2.0);
            //_SetAvaloniaContent<TopHUD.App>(2.0);

            _topHUD = CreateWindow(InitializeTopHUD, _DockWindowTop);
            _btmHUD = CreateWindow(InitializeAvaloniaControlCatalogDemo, _DockWindowBottom);
            //CreateWindow(InitializeRenderDemo);
            _DockWindows();
        }

        private void _SetAvaloniaContent<T>(double uiRenderScale) where T : global::Avalonia.Application, new()
        {
            avaloniaContext = Context.ConfigureAvalonia<T>();
            avaloniaContext.RenderScaling = uiRenderScale;
            _pixelScale = (1.0 / uiRenderScale);
        }
        private double _pixelScale;

        private void _SetWindowSizePos(AC.Window window, double width, double height, double x, double y)
        {
            window.Width = width * _pixelScale;
            window.Height = height * _pixelScale;

            double posScale = 1.0;
            window.Position = new Avalonia.PixelPoint((int)(x * posScale), (int)(y * posScale));
        }


        private AC.Window CreateWindow(Func<AC.Window> createMethod, Action<AC.Window> dockWindowMethod)
        {
            var window = createMethod();
            window.SystemDecorations = AC.SystemDecorations.BorderOnly;
            dockWindowMethod(window);

            // najak - Needed to make Window Default Background be Transparent, rather than solid-white!
            var avRoot = Avalonia.VisualTree.VisualExtensions.GetVisualRoot(window) as global::Avalonia.Controls.TopLevel;
            avRoot.TransparencyBackgroundFallback = Avalonia.Media.Brushes.Transparent;

            AvaloniaUrhoContext.MainWindow = window;
            window.Show(UI.Root);

            return window;
        }

        private AC.Window _topHUD;
        private AC.Window _btmHUD;

        private void _DockWindows()
        {
            if (_topHUD != null)
                _DockWindowTop(_topHUD);
            if (_btmHUD != null)
                _DockWindowBottom(_btmHUD);
        }

        private void _DockWindowTop(AC.Window window)
        {
            _SetWindowSizePos(window, Graphics.Width, 0.15 * Graphics.Height, 0, 0);
        }
        private void _DockWindowBottom(AC.Window window)
        {
            _SetWindowSizePos(window, Graphics.Width, 0.3 * Graphics.Height, 0, 0.7 * Graphics.Height);
            //window.Width = this.Graphics.Width;
            //window.Height = (this.Graphics.Height / 4);
            //window.Position = new Avalonia.PixelPoint(0, (int)(Graphics.Height - window.Height));   // TMS
        }

        Avalonia.Controls.Window InitializeTopHUD()
        {
            var window = new TopHUD.MainWindow();

            //var panel = new AC.Panel();
            //panel.Parent = window;
            //window.Content = panel;



            return window;
        }
        //Avalonia.Controls.Window InitializeRenderDemo()
        //{
        //    avaloniaContext = Context.ConfigureAvalonia<RenderDemo.App>();
        //    avaloniaContext.RenderScaling = 2.0;
        //    return new RenderDemo.MainWindow();
        //}

        Avalonia.Controls.Window InitializeAvaloniaControlCatalogDemo()
        {
            var mainWindow = new ControlCatalog.MainWindow();
            //// najak - Needed to make Window Default Background be Transparent, rather than solid-white!
            //var avRoot = Avalonia.VisualTree.VisualExtensions.GetVisualRoot(mainWindow) as global::Avalonia.Controls.TopLevel;
            //avRoot.TransparencyBackgroundFallback = Avalonia.Media.Brushes.Transparent;

            var content = mainWindow.Content;

            AvaloniaUrhoContext.MainWindow = mainWindow;
            //mainWindow.Show(UI.Root);

            ////mainWindow.Position = new Avalonia.PixelPoint(100, 100);
            //mainWindow.Position = new Avalonia.PixelPoint(0,0);   // TMS
            //// TBD whether one HUD over both viewports, or each viewport has own HUD.
            //if (false)//ShowTwoViewports)
            //{   // Left half-screen.
            //    var width0DI = mainWindow.Width;
            //    var width1DI = ToDIUnits(Graphics.Width);
            //    mainWindow.Width = (int)(width1DI / 2);
            //    mainWindow.Height = (int)ToDIUnits(Graphics.Height);
            //}
            //else
            //{   // Full screen.
            //    mainWindow.Width = (int)ToDIUnits(Graphics.Width);
            //    mainWindow.Height = (int)ToDIUnits(Graphics.Height);
            //}

            return new ControlCatalog.MainWindow();

        }
    }
}
