using PaderbornUniversity.SILab.Hip.Mobile.Shared.Common;
using PaderbornUniversity.SILab.Hip.Mobile.UI.Contracts;
using PaderbornUniversity.SILab.Hip.Mobile.UI.Helpers;
using PaderbornUniversity.SILab.Hip.Mobile.UI.Navigation;
using PaderbornUniversity.SILab.Hip.Mobile.UI.ViewModels.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkiaSharp;
using SkiaSharp.Views.Forms;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PaderbornUniversity.SILab.Hip.Mobile.UI.Pages
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class CharacterSelectionPage : IViewFor<CharacterSelectionPageViewModel>
	{
		private double thisWidth, thisHeight;
		private DeviceOrientation deviceOrientation;

		private CharacterSelectionPageViewModel ViewModel => ((CharacterSelectionPageViewModel)BindingContext);

		public CharacterSelectionPage()
		{
			InitializeComponent();

			deviceOrientation = DeviceOrientation.Undefined;

			// hide the status bar for this page
			IStatusBarController statusBarController = IoCManager.Resolve<IStatusBarController>();
			statusBarController.HideStatusBar();
		}

		protected override void OnSizeAllocated(double width, double height)
		{
			base.OnSizeAllocated(width, height);

			if (!(Math.Abs(width - thisWidth) > 0.4) && !(Math.Abs(height - thisHeight) > 0.4))
				return;

			thisWidth = width;
			thisHeight = height;

			if (width <= height)
			{
				//Portrait
				if (deviceOrientation == DeviceOrientation.Portrait)
					return;



				deviceOrientation = DeviceOrientation.Portrait;
			}
			else if (width > height)
			{
				//Landscape
				if (deviceOrientation == DeviceOrientation.Landscape)
					return;


				deviceOrientation = DeviceOrientation.Landscape;
			}
		}
		private void OnPaintSample(object sender, SKPaintSurfaceEventArgs e)
		{
			int surfaceWidth = e.Info.Width;
			int surfaceHeight = e.Info.Height;

			SKCanvas canvas = e.Surface.Canvas;


			using (SKPaint paint = new SKPaint())
			{
				canvas.Clear(Color.Blue.ToSKColor()); //paint it blue


				var pathStroke2 = new SKPaint
				{
					IsAntialias = true,
					Style = SKPaintStyle.StrokeAndFill,
					Color = new SKColor(255, 165, 0),
					StrokeWidth = 5
				};

				var path2 = new SKPath { FillType = SKPathFillType.EvenOdd };
				path2.MoveTo(0, 0);
				path2.LineTo(0, surfaceHeight);
				path2.LineTo(surfaceWidth, surfaceHeight);
				path2.LineTo(0, 0);
				path2.Close();
				canvas.DrawPath(path2, pathStroke2);

			}
		}
		protected override bool OnBackButtonPressed()
		{
			//The user cannot go back when he has to select the character on the first app start
			if (ViewModel.ParentViewModel.GetType() != typeof(UserOnboardingPageViewModel))
			{
				ViewModel.SwitchToNextPage();
			}
			return true;
		}
	}
}