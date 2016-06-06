using Foundation;
using System;
using System.CodeDom.Compiler;
using UIKit;
using MaterialControls;

namespace MaterialControlsSample
{
	partial class TimePickerViewController : UIViewController
	{
		public TimePickerViewController (IntPtr handle) : base (handle)
		{
		}

		partial void buttonShowHandle (MaterialControls.MDButton sender)
		{
			var dialog = new MDTimePickerDialog();
			dialog.Show();
		}
	}
}
