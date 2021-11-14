using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace ControlCatalog.Pages
{
    public class AcrylicPage : UserControl
    {
        public AcrylicPage()
        {
            this.InitializeComponent();

            // START == najak- Handle PointerPressed for all Panels - so that they will block MOuse INput from the Scene View beneath it.
            //_HandleMouse<ExperimentalAcrylicBorder>("topPanel");
            var collection = Avalonia.LogicalTree.LogicalExtensions.GetSelfAndLogicalDescendants(this);
            foreach (var dec in collection)
            {
                if (dec is ExperimentalAcrylicBorder)
                {
                    ExperimentalAcrylicBorder element = dec as ExperimentalAcrylicBorder;
                    element.PointerPressed += Stack_PointerPressed;
                }
            }
            // END == najak
        }

        // START == najak- Handle PointerPressed for all Panels - so that they will block MOuse INput from the Scene View beneath it.
        private void _HandleMouse<T>(params string[] names) where T : class, IControl
        {
            foreach (var name in names)
			{
                var element = this.FindControl<T>(name);
                if (element == null) continue; // not found
                element.PointerPressed += Stack_PointerPressed;
            }
        }
        private void Stack_PointerPressed(object sender, Avalonia.Input.PointerPressedEventArgs e) { e.Handled = true; }
        // END ==  najak- Handle PointerPressed for all Panels - so that they will block MOuse INput from the Scene View beneath it.


        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
