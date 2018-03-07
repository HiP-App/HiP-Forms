using PaderbornUniversity.SILab.Hip.Mobile.UI.Helpers;
using PaderbornUniversity.SILab.Hip.Mobile.UI.Navigation;
using PaderbornUniversity.SILab.Hip.Mobile.UI.ViewModels.Pages;
using SkiaSharp;
using SkiaSharp.Views.Forms;
using Xamarin.Forms.Xaml;

namespace PaderbornUniversity.SILab.Hip.Mobile.UI.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CharacterSelectionPage : IViewFor<CharacterSelectionPageViewModel>
    {
        private DeviceOrientation deviceOrientation;

        private CharacterSelectionPageViewModel ViewModel => (CharacterSelectionPageViewModel) BindingContext;

        public CharacterSelectionPage()
        {
            InitializeComponent();

            deviceOrientation = DeviceOrientation.Undefined;
        }

        protected override void OnSizeAllocated(double width, double height)
        {
            base.OnSizeAllocated(width, height);

            if (width > height && deviceOrientation != DeviceOrientation.Landscape)
            {
                // landscape mode

                deviceOrientation = DeviceOrientation.Landscape;
            }
            else if (width <= height && deviceOrientation != DeviceOrientation.Portrait)
            {
                // portrait mode

                deviceOrientation = DeviceOrientation.Portrait;
            }
        }

        private void OnPaintSample(object sender, SKPaintSurfaceEventArgs e)
        {

            var surfaceWidth = e.Info.Width;
            var surfaceHeight = e.Info.Height;

            var canvas = e.Surface.Canvas;

            using (var paint = new SKPaint())
            {
                canvas.Clear(new SKColor(127,172,255)); //paint it blue
                using (var pathStroke = new SKPaint
                {
                    IsAntialias = true,
                    Style = SKPaintStyle.StrokeAndFill,
                    Color = new SKColor(255, 229, 127),
                    StrokeWidth = 5
                })
                {
                    using (var path = new SKPath { FillType = SKPathFillType.EvenOdd })
                    {
                        path.MoveTo(surfaceWidth, 0);
                        path.LineTo(0, 0);
                        path.LineTo(0, surfaceHeight);
                        path.LineTo(surfaceWidth, 0);
                        path.Close();
                        canvas.DrawPath(path, pathStroke);
                    }
                }
            }
        }

        protected override bool OnBackButtonPressed()
        {
            //The user cannot go back when he has to select the character on the first app start
            if (ViewModel.ParentViewModel.GetType() != typeof(UserOnboardingPageViewModel))
            {
                ViewModel.ReturnToPreviousPage();
            }

            return true;
        }
    }
}