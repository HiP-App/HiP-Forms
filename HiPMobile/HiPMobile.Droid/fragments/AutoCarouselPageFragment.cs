using Android.OS;
using Android.Support.V4.App;
using Android.Views;
using Android.Widget;

namespace de.upb.hip.mobile.droid.fragments {
    public class AutoCarouselPageFragment : Fragment {

        public const string PAGE_NUMBER = "PageNumber";

        public int pageNum;

        public override void OnCreate (Bundle savedInstanceState)
        {
            base.OnCreate (savedInstanceState);

            // Create your fragment here
            pageNum = Arguments.GetInt (PAGE_NUMBER);
        }

        public static AutoCarouselPageFragment Create (int pageNumber)
        {
            var fragment = new AutoCarouselPageFragment ();
            var args = new Bundle ();
            args.PutInt (PAGE_NUMBER, pageNumber);
            fragment.Arguments = args;
            return fragment;
        }


        public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Use this to return your custom view for this Fragment
            // return inflater.Inflate(Resource.Layout.YourFragment, container, false);
            // Inflate the layout containing a title and body text.
            var rootView = (ViewGroup) inflater.Inflate (Resource.Layout.fragment_auto_carousel_page, container, false);

            var image = rootView.FindViewById<ImageView> (Resource.Id.image_holder);
            var title = rootView.FindViewById<TextView> (Resource.Id.title_text_holder);
            var subtitle = rootView.FindViewById<TextView> (Resource.Id.subtitle_text_holder);

            if (pageNum == 0)
            {
                title.Text = Resources.GetText (Resource.String.slide_1_title);
                subtitle.Text = Resources.GetText (Resource.String.slide_1_desc);
                image.SetBackgroundResource (Resource.Drawable.ic_launcher);
            }

            if (pageNum == 1)
            {
                title.Text = Resources.GetText (Resource.String.slide_2_title);
                subtitle.Text = Resources.GetText (Resource.String.slide_2_desc);
                image.SetBackgroundResource (Resource.Drawable.ic_2);
            }

            if (pageNum == 2)
            {
                title.Text = Resources.GetText (Resource.String.slide_3_title);
                subtitle.Text = Resources.GetText (Resource.String.slide_3_desc);
                image.SetBackgroundResource (Resource.Drawable.ic_3);
            }

            return rootView;
        }

    }
}