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

        private readonly ISet<string> ActiveTags;
        private readonly RouteSet RouteSet;
        private readonly Context Context;
        private readonly List<IRouteSelectedListener> RouteSelectedListeners = new List<IRouteSelectedListener> ();

        /// <summary>
        ///     Constructor for the RouteRecyclerAdapter
        /// </summary>
        /// <param name="exhibitSet">The displayed exhibitSet.</param>
        /// <param name="location">The location of the device.</param>
        /// <param name="context"></param>
        public RouteRecyclerAdapter (RouteSet RouteSet, Context Context, ISet<string> ActiveTags)
        {
            this.RouteSet = RouteSet;
            this.Context = Context;
            this.ActiveTags = ActiveTags;
        }


        public override int ItemCount {
            get { throw new NotImplementedException (); }
        }

        public Filter Filter {
            get { return new FilterImpl (); }
        }

        public override void OnBindViewHolder (RecyclerView.ViewHolder holder, int position)
        {
            Route route = getFilteredRoutes ().getRouteByPosition (position);

            ((ViewHolder) holder).Title.Text = route.Title;

            ((ViewHolder) holder).Description.Text = route.Description;

            int durationInMinutes = route.Duration / 60;
            ((ViewHolder) holder).Duration.Text = Context.Resources.GetQuantityString (
                Resource.Plurals.route_activity_duration_minutes,
                durationInMinutes,
                durationInMinutes
                );

            ((ViewHolder) holder).Distance.Text = Java.Lang.String.Format (Context.Resources.
                                                                                   GetString (Resource.String.route_activity_distance_kilometer), route.Distance);

            // Check if there are actually tags for this route
            if (route.RouteTags != null)
            {
                ((ViewHolder) holder).TagsLayout.RemoveAllViews ();
                foreach (RouteTag tag in route.RouteTags)
                {
                    ImageView tagImageView = new ImageView (Context);
                    tagImageView.SetImageDrawable (tag.Image.GetDrawable ());
                    ((ViewHolder) holder).TagsLayout.AddView (tagImageView);
                }
            }

            ((ViewHolder) holder).Image.SetImageDrawable (route.Image.GetDrawable ());
            ((ViewHolder) holder).View.Id = route.Id.GetHashCode ();
        }

        /**
 * Filters routes for activeTags
 *
 * @return routes which contains activeTags
 */

        public RouteSet getFilteredRoutes ()
        {
            List<Route> result = new List<Route> ();

            //routeLoop:
            foreach (Route route in this.RouteSet.Routes)
            {
                foreach (RouteTag tag in route.RouteTags)
                {
                    if (ActiveTags.Contains (tag.Tag))
                    {
                        result.Add (route);
                        //continue routeLoop;
                    }
                }
            }

            RouteSet set = new RouteSet ();
            set.Routes = result;

            return set;
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder (ViewGroup parent, int viewType)
        {
            View v = LayoutInflater.From (parent.Context)
                                   .Inflate (Resource.Layout.activity_route_row_item, parent, false);
            return new ViewHolder (v);
        }


        /**
         * Register the listener for routeSelection
         *
         * @param listener RouteSelectedListener that should be added
         */

        public void registerRouteSelectedListener (IRouteSelectedListener listener)
        {
            RouteSelectedListeners.Add (listener);
        }

        /**
         * Notifies listeners when a route was selected
         *
         * @param route Route for which the listeners should be notified
         */

        private void notifyRouteSelectedListeners (Route route)
        {
            foreach (IRouteSelectedListener listener in RouteSelectedListeners)
            {
                listener.OnRouteSelected (route);
            }
        }

    }


/**
    * Interface for the routeSelectedListener
    */

    public interface IRouteSelectedListener {

        void OnRouteSelected (Route route);

    }

    /// <summary>
    ///     Provide a reference to the views for each data item
    ///     Complex data items may need more than one view per item, and
    ///     you provide access to all the views for a data item in a view holder
    /// </summary>
    public class ViewHolder : RecyclerView.ViewHolder {

        public ViewHolder (View v) : base (v)
        {
            View = v;
            Image = (ImageView) v.FindViewById (Resource.Id.routeRowItemImage);
            Title = (TextView) v.FindViewById (Resource.Id.routeRowItemTitle);
            Description = (TextView) v.FindViewById (Resource.Id.routeRowItemDescription);
            Duration = (TextView) v.FindViewById (Resource.Id.routeRowItemDuration);
            Distance = (TextView) v.FindViewById (Resource.Id.routeRowItemDistance);
            TagsLayout = (LinearLayout) v.FindViewById (Resource.Id.routeRowItemTagsLayout);

            /**
             * TODO: Call NotifyRoute SelectedListeners
             * 
             */
        }

        public View View { get; set; }
        public string RouteId { get; set; }
        public ImageView Image { get; set; }
        public TextView Title { get; set; }
        public TextView Description { get; set; }
        public TextView Duration { get; set; }
        public TextView Distance { get; set; }
        public LinearLayout TagsLayout { get; set; }

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
}