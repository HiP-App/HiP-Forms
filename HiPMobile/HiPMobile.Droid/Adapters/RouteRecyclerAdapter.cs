using System;
using System.Collections.Generic;
using Android.Content;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using de.upb.hip.mobile.droid;
using de.upb.hip.mobile.droid.Helpers;
using de.upb.hip.mobile.pcl.BusinessLayer.Models;
using Java.Lang;

namespace de.upb.hip.mobile.droid.Adapters {
    public class RouteRecyclerAdapter : RecyclerView.Adapter, IFilterable {

        private readonly ISet<string> activeTags;
        private readonly ISet<Route> routeSet;
        private readonly Context context;
        private readonly List<IRouteSelectedListener> routeSelectedListeners = new List<IRouteSelectedListener> ();

        /// <summary>
        ///     Constructor for the RouteRecyclerAdapter
        /// </summary>
        /// <param name="routeSet">The displayed set of routes</param>
        /// <param name="context">The location of the device.</param>
        /// <param name="activeTags"></param>
        public RouteRecyclerAdapter (ISet<Route> routeSet, Context context, ISet<string> activeTags)
        {
            this.routeSet = routeSet;
            this.context = context;
            this.activeTags = activeTags;
        }


        public override int ItemCount {
            get { return GetFilteredRoutes ().Count; }
        }

        public Filter Filter {
            get { return new FilterImpl (); }
        }

        public override void OnBindViewHolder (RecyclerView.ViewHolder holder, int position)
        {
            Route route = GetFilteredRoutes () [position];
            var viewHolder = ((ViewHolder)holder);

            viewHolder.Title.Text = route.Title;

            viewHolder.Description.Text = route.Description;

            int durationInMinutes = route.Duration / 60;
            viewHolder.Duration.Text = context.Resources.GetQuantityString (
                Resource.Plurals.route_activity_duration_minutes,
                durationInMinutes,
                durationInMinutes
                );

            viewHolder.Distance.Text = Java.Lang.String.Format (context.Resources.
                                                                                   GetString (Resource.String.route_activity_distance_kilometer), route.Distance);

            // Check if there are actually tags for this route
            if (route.RouteTags != null)
            {
                viewHolder.TagsLayout.RemoveAllViews ();
                foreach (RouteTag tag in route.RouteTags)
                {
                    ImageView tagImageView = new ImageView (context);
                    tagImageView.SetImageDrawable (tag.Image.GetDrawable (context, tagImageView.Width, tagImageView.Height));
                    viewHolder.TagsLayout.AddView (tagImageView);
                }
            }


            viewHolder.Image.SetImageDrawable (route.Image.GetDrawable (context, viewHolder.Image.Width, viewHolder.Image.Height));
            viewHolder.View.Id = route.Id.GetHashCode ();
        }


        /// <summary>
        ///     Returns only the routes which match the current filters
        /// </summary>
        public IList<Route> GetFilteredRoutes ()
        {
            List<Route> result = new List<Route> ();

            foreach (Route route in this.routeSet)
            {
                foreach (RouteTag tag in route.RouteTags)
                {
                    if (activeTags.Contains (tag.Tag))
                    {
                        result.Add (route);
                        //Is implemented like this in Java, maybe should be made nicer
                        goto ROUTELOOP;
                    }
                }
                ROUTELOOP:
                ;
            }
            return result;
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder (ViewGroup parent, int viewType)
        {
            View v = LayoutInflater.From (parent.Context)
                                   .Inflate (Resource.Layout.activity_route_row_item, parent, false);
            return new ViewHolder (v, this);
        }


        public void RegisterRouteSelectedListener (IRouteSelectedListener listener)
        {
            routeSelectedListeners.Add (listener);
        }


        /// <summary>
        ///     For notifying the route selected listeners about a new route selection
        /// </summary>
        /// <param name="routeId">The hash code of the id of the selected route</param>
        public void NotifyRouteSelectedListeners (int routeId)
        {
            //Find the route
            Route route = null;
            foreach (Route routeIt in routeSet)
            {
                if (routeIt.Id.GetHashCode () == routeId)
                    route = routeIt;
            }
            if (route == null)
            {
                return;
            }
            foreach (IRouteSelectedListener listener in routeSelectedListeners)
            {
                listener.OnRouteSelected (route);
            }
        }

    }


    /// <summary>
    ///     An interface for all objects which want to be notified when a route is selected.
    /// </summary>
    public interface IRouteSelectedListener {

        void OnRouteSelected (Route route);

    }

    /// <summary>
    ///     Provide a reference to the views for each data item
    ///     Complex data items may need more than one view per item, and
    ///     you provide access to all the views for a data item in a view holder
    /// </summary>
    public class ViewHolder : RecyclerView.ViewHolder {

        public ViewHolder (View v, RouteRecyclerAdapter adapter) : base (v)
        {
            View = v;
            Image = (ImageView) v.FindViewById (Resource.Id.routeRowItemImage);
            Title = (TextView) v.FindViewById (Resource.Id.routeRowItemTitle);
            Description = (TextView) v.FindViewById (Resource.Id.routeRowItemDescription);
            Duration = (TextView) v.FindViewById (Resource.Id.routeRowItemDuration);
            Distance = (TextView) v.FindViewById (Resource.Id.routeRowItemDistance);
            TagsLayout = (LinearLayout) v.FindViewById (Resource.Id.routeRowItemTagsLayout);
            this.adapter = adapter;

            v.SetOnClickListener (new ViewOnClickListener (adapter));
        }

        public View View { get; set; }
        public string RouteId { get; set; }
        public ImageView Image { get; set; }
        public TextView Title { get; set; }
        public TextView Description { get; set; }
        public TextView Duration { get; set; }
        public TextView Distance { get; set; }
        public LinearLayout TagsLayout { get; set; }
        public RouteRecyclerAdapter adapter { get; }

    }


    public class FilterImpl : Filter {

        protected override FilterResults PerformFiltering (ICharSequence constraint)
        {
            return null;
        }

        protected override void PublishResults (ICharSequence constraint, FilterResults results)
        {
            //Empty on purpose
        }

    }

    public class ViewOnClickListener : Java.Lang.Object, View.IOnClickListener {

        public ViewOnClickListener (RouteRecyclerAdapter adapter)
        {
            this.adapter = adapter;
        }

        public RouteRecyclerAdapter adapter { get; }

        public void OnClick (View v)
        {
            adapter.NotifyRouteSelectedListeners (v.Id);
        }

    }
}