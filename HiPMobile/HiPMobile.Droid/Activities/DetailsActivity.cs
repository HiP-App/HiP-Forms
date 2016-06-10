using Android.App;
using Android.OS;
using Android.Support.V4.View;
using Android.Widget;

namespace de.upb.hip.mobile.droid.Activities {
    [Activity (Theme = "@style/AppTheme.WithActionBar", Label = "DetailsActivity")]
    public class DetailsActivity : Activity {

        public static  string INTENT_EXHIBIT_ID = "exhibit-id";

        // View name of the header image. Used for activity scene transitions
        public static  string VIEW_NAME_IMAGE = "detail:image";
        // View name of the header title. Used for activity scene transitions
        public static  string VIEW_NAME_TITLE = "detail:title";

        private ActionBar mActionBar;

        private int mExhibitId;
        private string mImageName;

        private ImageView mImageView;
        private bool mIsSlider;
        private TextView mTextView;


        /**
         * Set up the Details. Load the correct image and text.
         * Add a transitionListener, if necessary
         * @param savedInstanceState
         */

        protected void onCreate (Bundle savedInstanceState)
        {
            OnCreate (savedInstanceState);
            SetContentView (Resource.Layout.activity_details);

            //  openDatabase();

            mImageView = (ImageView) FindViewById (Resource.Id.detailsImageView);
            //TODO: Get rid of this hardcoded constant
            mImageName = "image.jpg";
            //mImageView.setImageDrawable(mDatabase.getImage(1, mImageName));
            mTextView = (TextView) FindViewById (Resource.Id.detailsName);

            if (Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop)
            {
                /**
                 * Set the name of the view's which will be transitioned to,
                 * using the static values above.
                 * This could be done in the layout XML,
                 * but exposing it via static variables allows easy
                 * querying from other Activities
                 */
                ViewCompat.SetTransitionName (mImageView, VIEW_NAME_IMAGE);
                ViewCompat.SetTransitionName (mTextView, VIEW_NAME_TITLE);

                //addTransitionListener ();
            }

            mExhibitId = Intent.GetIntExtra (INTENT_EXHIBIT_ID, 0);

            //TODO: Remove hardcoded string constant
            //  Drawable d = mDatabase.getImage(mExhibitId, "image.jpg");
            //  mImageView.SetImageDrawable(d);

            //DocumentsContract.Document document = mDatabase.getDocument(mExhibitId);
            //Exhibit exhibit = new Exhibit(document);
            /*mTextView.setText(exhibit.getName());
            if (exhibit.getSliderId() != -1)
            {
                mIsSlider = true;
            }
            else
            {
                mIsSlider = false;
            }*/

            /* TextView txtDescription = (TextView)FindViewById(Resource.Id.detailsDescription);
            txtDescription.Text = (exhibit.Description);*/

            //TODO: is this needed?
            //        // Set ActionBar
            //        final Toolbar toolbar = (Toolbar) findViewById(R.id.toolbar);
            //        if (toolbar != null) {
            //            toolbar.setTitle(exhibit.name);
            //            setSupportActionBar(toolbar);
            //        }

            // Set back button on actionbar
            /*mActionBar = GetSupportActionBar();
            if (mActionBar != null)
            {
                mActionBar.setDisplayHomeAsUpEnabled(true);
                mActionBar.setTitle(exhibit.getName());
            }*/

            //TODO: is this needed?
            //ImageButton fab = (ImageButton) findViewById(R.id.fab);
            //        ViewOutlineProvider viewOutlineProvider = new ViewOutlineProvider() {
            //            @Override
            //            public void getOutline(View view, Outline outline) {
            //                // Or read size directly from the view's width/height
            //                int size = getResources().getDimensionPixelSize(R.dimen.fab_size);
            //                outline.setOval(0, 0, size, size);
            //            }
            //        };
            //        fab.setOutlineProvider(viewOutlineProvider);
        }

/*public void onClick_detailsImageView(View view)
{
    if (mIsSlider)
    {
        Intent intent = new Intent(this, DisplayImageSliderActivity.class);
            intent.putExtra(DisplayImageSliderActivity.INTENT_EXHIBIT_ID, mExhibitId);
            intent.putExtra(DisplayImageSliderActivity.INTENT_IMAGE_NAME, mImageName);
            startActivity(intent);
        } else {
            Intent intent = new Intent(this, DisplaySingleImageActivity.class);
            intent.putExtra(DisplaySingleImageActivity.INTENT_EXHIBIT_ID, mExhibitId);
            intent.putExtra(DisplaySingleImageActivity.INTENT_IMAGE_NAME, mImageName);
            startActivity(intent);
        }
    }

    @Override
    public boolean onSupportNavigateUp()
{
    finish();
    return true;
}

@Override
    protected void onResume()
{
    super.onResume();
}

@Override
    protected void onDestroy()
{
    super.onDestroy();
}

private void openDatabase()
{
    mDatabase = new DBAdapter(this);
}

@Override
    public void onBackPressed()
{
    this.finish();
}

    //TODO: is this needed?
    /*  Check later if this is needed
    public void onStop() {
        mImageView.destroyDrawingCache();
        super.onStop();
    }
    */

        /*public void onClick_back(View view)
{
    this.finish();
}

/**
 * Opens a bigger version of the image when pressed
 *
 * @param view
 */
        /* private boolean addTransitionListener()
        {
            final Transition transition = getWindow().getSharedElementEnterTransition();

            if (transition != null)
            {
                // There is an entering shared element transition so add a listener to it
                transition.addListener(new Transition.TransitionListener() {
                @Override
                public void onTransitionEnd(Transition transition)
        {
            // As the transition has ended, we can now load the full-size image
            Drawable d = mDatabase.getImage(mExhibitId, "image.jpg");
            mImageView.setImageDrawable(d);

            // Make sure we remove ourselves as a listener
            transition.removeListener(this);
        }

        @Override
                public void onTransitionStart(Transition transition)
        {
            // No-op
        }

        @Override
                public void onTransitionCancel(Transition transition)
        {
            // Make sure we remove ourselves as a listener
            transition.removeListener(this);
        }

        @Override
                public void onTransitionPause(Transition transition)
        {
            // No-op
        }

        @Override
                public void onTransitionResume(Transition transition)
        {
            // No-op
        }
    });
            return true;
        }

        // If we reach this then we have not added a listener
        return false;
    }

    /**
     * Closes the DetailsActivity
     */

        /**
         * Try and add a {@link android.transition.Transition.TransitionListener}
         * to the entering shared element
         * {@link android.transition.Transition}.
         * We do this so that we can load the full-size image after the transition
         * has completed.
         *
         * @return true if we were successful in adding a listener to the entered transition
         */

    }
}