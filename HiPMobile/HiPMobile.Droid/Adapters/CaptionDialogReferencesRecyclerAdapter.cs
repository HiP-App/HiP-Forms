// Copyright (C) 2016 History in Paderborn App - Universität Paderborn
//  
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//  
//       http://www.apache.org/licenses/LICENSE-2.0
//  
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System.Collections.Generic;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using de.upb.hip.mobile.droid.Helpers.InteractiveSources;

namespace de.upb.hip.mobile.droid.Adapters
{
    public class CaptionDialogReferencesRecyclerAdapter : RecyclerView.Adapter
    {
        private readonly List<Source> references;

        public CaptionDialogReferencesRecyclerAdapter(List<Source> references)
        {
            this.references = references;
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            var reference = references[position];

            var viewHolder = (ReferenceViewHolder)holder;

            viewHolder.TitleTextView.Text = reference.SubstituteText + ":";
            viewHolder.ReferenceTextView.Text = reference.Text;
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            View v = LayoutInflater.From(parent.Context)
                                   .Inflate(Resource.Layout.fragment_exhibit_details_caption_dialog_references_item, parent, false);

            return new ReferenceViewHolder(v);
        }

        public override int ItemCount => references.Count;

        private class ReferenceViewHolder : RecyclerView.ViewHolder
        {

            public ReferenceViewHolder(View v) : base(v)
            {
                TitleTextView = (TextView)v.FindViewById(Resource.Id.captionTitleTextView);
                ReferenceTextView = (TextView)v.FindViewById(Resource.Id.captionReferencesTextView);
            }

            public TextView TitleTextView { get; }
            public TextView ReferenceTextView { get; }

        }

    }
}