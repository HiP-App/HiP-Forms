using System;
using System.Text.RegularExpressions;
using Xamarin.Forms;

namespace PaderbornUniversity.SILab.Hip.Mobile.UI.Behaviors
{
    public class EmailValidationBehavior : Behavior<Entry>
    {
        protected override void OnAttachedTo(Entry bindable)
        {
            bindable.TextChanged += HandleTextChanged;
            base.OnAttachedTo(bindable);
        }

        void HandleTextChanged(object sender, TextChangedEventArgs e)
        {
            ((Entry) sender).TextColor = e.NewTextValue.Contains("@") ? Color.Green : Color.Red;
        }

        protected override void OnDetachingFrom(Entry bindable)
        {
            bindable.TextChanged -= HandleTextChanged;
            base.OnDetachingFrom(bindable);
        }
    }
}