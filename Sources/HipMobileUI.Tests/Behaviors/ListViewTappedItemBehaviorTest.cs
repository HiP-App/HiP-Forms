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

using System.Windows.Input;
using NSubstitute;
using NUnit.Framework;
using PaderbornUniversity.SILab.Hip.Mobile.UI.Behaviors;
using Xamarin.Forms;

namespace PaderbornUniversity.SILab.Hip.Mobile.HipMobileUITests.Behaviors
{
    [TestFixture]
    public class ListViewTappedItemBehaviorTest
    {
        [Test, Category("UnitTest")]
        public void OnListViewItemTapped_CommandExecuted()
        {
            var sut = CreateSystemUnderTest();

            var valueConverter = Substitute.For<IValueConverter>();
            var command = Substitute.For<ICommand>();
            command.CanExecute(Arg.Any<object>()).Returns(true);

            sut.Command = command;
            sut.Converter = valueConverter;

            var listView = CreateListView();
            listView.Behaviors.Add(sut);

            sut.OnListViewItemTapped(null, new ItemTappedEventArgs(null, null));

            command.Received(1).Execute(Arg.Any<object>());
        }

        private ListView CreateListView()
        {
            return new ListView();
        }

        private ListViewTappedItemBehavior CreateSystemUnderTest()
        {
            return new ListViewTappedItemBehavior();
        }
    }
}