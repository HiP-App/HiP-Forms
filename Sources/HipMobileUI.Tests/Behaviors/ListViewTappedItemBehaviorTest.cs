using System.Windows.Input;
using HipMobileUI.Behaviors;
using NSubstitute;
using NUnit.Framework;
using Xamarin.Forms;

namespace HipMobileUI.Tests.Behaviors {

    [TestFixture]
    public class ListViewTappedItemBehaviorTest {

        [Test, Category ("UnitTest")]
        public void OnListViewItemTapped_CommandExecuted ()
        {
            var sut = CreateSystemUnderTest ();

            var valueConverter = Substitute.For<IValueConverter>();
            var command = Substitute.For<ICommand>();
            command.CanExecute(Arg.Any<object>()).Returns(true);

            sut.Command = command;
            sut.Converter = valueConverter;

            var listView = CreateListView ();
            listView.Behaviors.Add (sut);
            
            sut.OnListViewItemTapped (null, new ItemTappedEventArgs (null, null));

            command.Received(1).Execute (Arg.Any<object> ());
        }


        private ListView CreateListView ()
        {
            return new ListView();
        }

        private ListViewTappedItemBehavior CreateSystemUnderTest ()
        {
            return new ListViewTappedItemBehavior ();
        }

    }
}