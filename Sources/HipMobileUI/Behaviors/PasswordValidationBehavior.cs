// Copyright (C) 2017 History in Paderborn App - Universität Paderborn
//  
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//  
//      http://www.apache.org/licenses/LICENSE-2.0
//  
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using System.Windows.Input;
using Xamarin.Forms;

namespace PaderbornUniversity.SILab.Hip.Mobile.UI.Behaviors
{
public class ComparisonBehavior : Behavior<Entry>
{
	private Entry thisEntry;

	static readonly BindablePropertyKey IsValidPropertyKey = BindableProperty.CreateReadOnly("IsValid", typeof(bool), typeof(ComparisonBehavior), false);
	public static readonly BindableProperty IsValidProperty = IsValidPropertyKey.BindableProperty;

	public static readonly BindableProperty CompareToEntryProperty = BindableProperty.Create("CompareToEntry", typeof(Entry), typeof(ComparisonBehavior), null);

	public Entry CompareToEntry
	{
		get { return (Entry)base.GetValue(CompareToEntryProperty); }
		set
		{
			base.SetValue(CompareToEntryProperty, value);
			if (CompareToEntry != null)
				CompareToEntry.TextChanged -= baseValue_changed;
			value.TextChanged += baseValue_changed;
		}
	}

	void baseValue_changed(object sender, TextChangedEventArgs e)
	{
		IsValid = ((Entry)sender).Text.Equals(thisEntry.Text);
		thisEntry.TextColor = IsValid ? Color.Green : Color.Red;
	}


	public bool IsValid
	{
		get { return (bool)base.GetValue(IsValidProperty); }
		private set { base.SetValue(IsValidPropertyKey, value); }
	}
	protected override void OnAttachedTo(Entry bindable)
	{
		thisEntry = bindable;

		if (CompareToEntry != null)
			CompareToEntry.TextChanged += baseValue_changed;

		bindable.TextChanged += HandleTextChanged;
		base.OnAttachedTo(bindable);
	}

	protected override void OnDetachingFrom(Entry bindable)
	{
		bindable.TextChanged -= HandleTextChanged;
		if (CompareToEntry != null)
			CompareToEntry.TextChanged -= baseValue_changed;
		base.OnDetachingFrom(bindable);
	}

	void HandleTextChanged(object sender, TextChangedEventArgs e)
	{
		string theBase = CompareToEntry.Text;
		string confirmation = e.NewTextValue;
		IsValid = (bool)theBase?.Equals(confirmation);

		((Entry)sender).TextColor = IsValid ? Color.Green : Color.Red;
	}
}   
}