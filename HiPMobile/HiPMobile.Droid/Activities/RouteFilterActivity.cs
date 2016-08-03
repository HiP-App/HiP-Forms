using System.Collections.Generic;
using Android.Annotation;
using Android.Content;
using Android.OS;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using de.upb.hip.mobile.pcl.BusinessLayer.Managers;
using de.upb.hip.mobile.pcl.BusinessLayer.Models;
using Java.Lang;
using Java.Util;
using de.upb.hip.mobile.droid;
using System;

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

            
            ISet<Route> routes = new HashSet<Route>();
            //Init the available routes
            foreach (Route route in RouteManager.GetRoutes())
            {
                routes.Add(route);
            }

            ISet<string> activeTags = new HashSet<string>();
            string[] tags = intent.GetStringArrayExtra ("activeTags");
            foreach (string tag in tags)
            {
                activeTags.Add (tag);
            }

            //There will be duplicates in the route set so we have to remove them
            var uniqueTags = new Dictionary<string, RouteTagHolder>();
            foreach (Route route in routes)
            {
                foreach (RouteTag tag in route.RouteTags)
                {
                    if (!uniqueTags.ContainsKey(tag.Tag))
                    {
                        uniqueTags.Add (tag.Tag,
                                new RouteTagHolder(activeTags.Contains(tag.Tag), tag));
                    }
                }
            }

            // Add tags
            ListView listView = (ListView)FindViewById(Resource.Id.routeFilterTagList);
            ArrayAdapter< RouteTagHolder > adapter =
                    new RouteTagArrayAdapter(ApplicationContext,
                            new ArrayList<>(uniqueTags.Values.));
            listView.Adapter = adapter;

            // Add buttons
            Button closeWithoutSave = (Button)FindViewById(Resource.Id.routeFilterCloseWithoutSaveButton);
            Button closeWithSave = (Button)FindViewById(Resource.Id.routeFilterCloseWithSaveButton);

            closeWithoutSave.SetOnClickListener(new CloseWithoutSaveOnClickListener ());

        closeWithSave.SetOnClickListener(new CloseWithSaveOnClickListener ());

        // Set back button on actionbar
        ActionBar actionBar = getSupportActionBar();
        if (actionBar != null) {
            actionBar.setDisplayHomeAsUpEnabled(true);
        }
    }

    public override bool OnSupportNavigateUp()
{
    Finish();
    return true;
}

/**
 * Helper class for one RouteTagView
 */
class RouteTagViewHolder
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
 class RouteTagHolder
{
    private bool mIsSelected;
    private RouteTag mRouteTag;

    /**
     * Constructor for a RouteTagHolder
     *
     * @param isSelected if the tag is currently selected
     * @param routeTag   the tag
     */
    public RouteTagHolder(bool isSelected, RouteTag routeTag)
    {
        this.mIsSelected = isSelected;
        this.mRouteTag = routeTag;
    }

    public bool isSelected()
    {
        return mIsSelected;
    }

    public void setSelected(bool isSelected)
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
class RouteTagArrayAdapter : ArrayAdapter<RouteTagHolder> {

        private LayoutInflater mInflater;

/**
 * Constructor for RouteTagArrayAdapter
 *
 * @param context current context
 * @param tags    list of tags as RouteTagHolder
 */
public RouteTagArrayAdapter(Context context, List<RouteTagHolder> tags) : base (context, Resource.Layout.activity_route_filter_row_item, tags)
{
    mInflater = LayoutInflater.From(context);
}
        public override View GetView(int position, View convertView, ViewGroup parent)
{
    RouteTagHolder tagHolder = this.GetItem(position);
    RouteTag tag = tagHolder.getRouteTag();

    CheckBox checkBox;
    ImageView imageView;

    if (convertView == null)
    {
        convertView = mInflater.Inflate(Resource.Layout.activity_route_filter_row_item, null);

        checkBox = (CheckBox)convertView.FindViewById(Resource.Id.routeFilterRowItemCheckBox);
        imageView = (ImageView)convertView.FindViewById(
                Resource.Id.routeFilterRowItemImage);

        convertView.SetTag(new RouteTagViewHolder(checkBox, imageView));

        checkBox.SetOnClickListener(new CheckBoxViewOnClickListener());
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


    class CheckBoxViewOnClickListener : Java.Lang.Object, View.IOnClickListener
{


    public void OnClick(View v)
    {
        CheckBox cb = (CheckBox)v;
        RouteTagHolder tagHolder = (RouteTagHolder)v.getTag();
        tagHolder.setSelected(cb.isChecked());
    }
}

class CloseWithoutSaveOnClickListener : Java.Lang.Object, View.IOnClickListener
{
    public void OnClick(View v)
    {
        SetResult(RETURN_NOSAVE);
        Finish();
    }
}

class CloseWithSaveOnClickListener : Java.Lang.Object, View.IOnClickListener
{
    public void OnClick(View v)
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
}

}