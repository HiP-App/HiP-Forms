using Foundation;
using System;
using System.CodeDom.Compiler;
using UIKit;
using MaterialControls;

namespace MaterialControlsSample
{
	partial class ProgressViewController : UIViewController
	{
		public ProgressViewController (IntPtr handle) : base (handle)
		{			
		}

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);

			progressDeterminate.Progress = 0;

			PerformSelector (new ObjCRuntime.Selector ("simulateProgress"), null, 0.2);
		}

		[Export ("simulateProgress")]
		public void SimulateProgress()
		{
			var val = 4 / 100f;
			var progress = progressDeterminate.Progress + val;
			if (progress < 1) {
				progressDeterminate.Progress = progress;
				progressLinearDeterminate.Progress = progress;

				PerformSelector (new ObjCRuntime.Selector ("simulateProgress"), null, 0.2);
			} else {
				progressDeterminate.Progress = 1;
				progressLinearDeterminate.Progress = 1;

				PerformSelector (new ObjCRuntime.Selector ("startProgress"), null, 3.0);
			}
		}

		[Export ("startProgress")]
		public void StartProgress()
		{
			progressDeterminate.Progress = 0;
			progressLinearDeterminate.Progress = 0;

			SimulateProgress ();
		}
	}
}
