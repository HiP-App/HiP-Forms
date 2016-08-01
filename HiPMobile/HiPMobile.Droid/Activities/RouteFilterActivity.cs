using System.Collections.Generic;
using Android.Annotation;
using Android.Content;
using Android.OS;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using de.upb.hip.mobile.pcl.BusinessLayer.Models;
using Java.Lang;

namespace de.upb.hip.mobile.droid.Activities {
    public class RouteFilterActivity : AppCompatActivity
    {

        public static readonly int RETURN_SAVE = 1;
        public static readonly int RETURN_NOSAVE = 2;

    protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_route_filter);
            Intent intent = Intent;

            // Get routes
            RouteSet routeSet = (RouteSet)intent.GetSerializableExtra("RouteSet");
                HashSet<String> activeTags = (HashSet<String>)intent.getSerializableExtra("activeTags");

            //There will be duplicates in the route set so we have to remove them
            HashMap<String, RouteTagHolder> uniqueTags = new HashMap<>();
            for (Route route : routeSet.getRoutes())
            {
                for (RouteTag tag : route.getTags())
                {
                    if (!uniqueTags.containsKey(tag.getTag()))
                    {
                        uniqueTags.put(tag.getTag(),
                                new RouteTagHolder(activeTags.contains(tag.getTag()), tag));
                    }
                }
            }

            // Add tags
            ListView listView = (ListView)findViewById(R.id.routeFilterTagList);
            final ArrayAdapter< RouteTagHolder > adapter =
                    new RouteTagArrayAdapter(getApplicationContext(),
                            new ArrayList<>(uniqueTags.values()));
            listView.setAdapter(adapter);

            // Add buttons
            Button closeWithoutSave = (Button)findViewById(R.id.routeFilterCloseWithoutSaveButton);
            Button closeWithSave = (Button)findViewById(R.id.routeFilterCloseWithSaveButton);

            closeWithoutSave.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v)
        {
            setResult(RETURN_NOSAVE);
            finish();
        }
    });

        closeWithSave.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v)
    {
        HashSet<String> activeTags = new HashSet<>();
        for (int i = 0; i < adapter.getCount(); i++)
        {
            RouteTagHolder tagHolder = adapter.getItem(i);
            if (tagHolder.mIsSelected)
            {
                activeTags.add(tagHolder.getRouteTag().getTag());
            }
        }
        Intent intent = new Intent();
        intent.putExtra("activeTags", activeTags);
        setResult(RETURN_SAVE, intent);
        finish();
    }
});

        // Set back button on actionbar
        ActionBar actionBar = getSupportActionBar();
        if (actionBar != null) {
            actionBar.setDisplayHomeAsUpEnabled(true);
        }
    }

    @Override
    public boolean onSupportNavigateUp()
{
    finish();
    return true;
}

/**
 * Helper class for one RouteTagView
 */
private static class RouteTagViewHolder
{
    private CheckBox mCheckBox;
    private ImageView mImageView;

    /**
     * Constructor for RouteTagViewHolder
     *
     * @param checkBox  The checkbox for this tag
     * @param imageView The image for this tag
     */
    public RouteTagViewHolder(CheckBox checkBox, ImageView imageView)
    {
        this.mCheckBox = checkBox;
        this.mImageView = imageView;
    }

    public CheckBox getCheckBox()
    {
        return mCheckBox;
    }

    public ImageView getImageView()
    {
        return mImageView;
    }

}

/**
 * Helper class for a route tag
 */
private static class RouteTagHolder
{
    private boolean mIsSelected;
    private RouteTag mRouteTag;

    /**
     * Constructor for a RouteTagHolder
     *
     * @param isSelected if the tag is currently selected
     * @param routeTag   the tag
     */
    public RouteTagHolder(boolean isSelected, RouteTag routeTag)
    {
        this.mIsSelected = isSelected;
        this.mRouteTag = routeTag;
    }

    public boolean isSelected()
    {
        return mIsSelected;
    }

    public void setSelected(boolean isSelected)
    {
        this.mIsSelected = isSelected;
    }

    public RouteTag getRouteTag()
    {
        return mRouteTag;
    }
}

/**
 * Helper class for a route tag array
 */
private static class RouteTagArrayAdapter extends ArrayAdapter<RouteTagHolder> {

        private LayoutInflater mInflater;

/**
 * Constructor for RouteTagArrayAdapter
 *
 * @param context current context
 * @param tags    list of tags as RouteTagHolder
 */
public RouteTagArrayAdapter(Context context, List<RouteTagHolder> tags)
{
    super(context, R.layout.activity_route_filter_row_item, tags);
    mInflater = LayoutInflater.from(context);
}

        @SuppressLint("InflateParams") /* there are no view parameters on the root element,
        passing null to the inflater is valid */
        @Override
        public View getView(int position, View convertView, ViewGroup parent)
{
    RouteTagHolder tagHolder = this.getItem(position);
    RouteTag tag = tagHolder.getRouteTag();

    CheckBox checkBox;
    ImageView imageView;

    if (convertView == null)
    {
        convertView = mInflater.inflate(R.layout.activity_route_filter_row_item, null);

        checkBox = (CheckBox)convertView.findViewById(R.id.routeFilterRowItemCheckBox);
        imageView = (ImageView)convertView.findViewById(
                R.id.routeFilterRowItemImage);

        convertView.setTag(new RouteTagViewHolder(checkBox, imageView));

        checkBox.setOnClickListener(new View.OnClickListener()
        {
                    public void onClick(View v)
{
    CheckBox cb = (CheckBox)v;
    RouteTagHolder tagHolder = (RouteTagHolder)v.getTag();
    tagHolder.setSelected(cb.isChecked());
}
                });
            } else {
                RouteTagViewHolder viewHolder = (RouteTagViewHolder)convertView.getTag();
checkBox = viewHolder.getCheckBox();
                imageView = viewHolder.getImageView();
            }

            checkBox.setTag(tagHolder);
            checkBox.setText(tag.getName());
            checkBox.setChecked(tagHolder.isSelected());
            imageView.setImageDrawable(tag.getImage().getDawableImage(getContext()));

            return convertView;
        }
    }
}

}