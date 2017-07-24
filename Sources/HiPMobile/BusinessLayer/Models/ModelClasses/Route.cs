﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool
//     Changes to this file will be lost if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
/*Copyright (C) 2016 History in Paderborn App - Universität Paderborn
 
  Licensed under the Apache License, Version 2.0 (the "License");
  you may not use this file except in compliance with the License.
  You may obtain a copy of the License at
 
       http://www.apache.org/licenses/LICENSE-2.0
 
  Unless required by applicable law or agreed to in writing, software
  distributed under the License is distributed on an "AS IS" BASIS,
  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
  See the License for the specific language governing permissions and
  limitations under the License.*/

using PaderbornUniversity.SILab.Hip.Mobile.Shared.ServiceAccessLayer;

namespace PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.Models
{
	using Realms;
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;

	public partial class Route : RealmObject, IIdentifiable, IDownloadable
	{
		//Attributes
		[PrimaryKey]
		public string Id{ get; set; }

		public virtual string Title{ get; set; }

        public string Name {
            get { return Title; }
            set { Title = value; }
        }

	    public virtual string Description{ get; set; }

		public virtual int Duration{ get; set; }

		public virtual double Distance{ get; set; }

	    public virtual DateTimeOffset? LastTimeDismissed{ get; set; }

        public virtual Boolean DetailsDataLoaded { get; set; }
        //Associations
        public virtual IList<RouteTag> RouteTags{ get; }

		public virtual Image Image{ get; set; }

		public virtual IList<Waypoint> Waypoints{ get; }

		public virtual Audio Audio{ get; set; }

        public DownloadableType Type { get { return DownloadableType.Route; } }

        // Constructor
        public Route ()
	    {
	        
	    }

	    public virtual int IdForRestApi { get; set; }
	    public virtual DateTimeOffset Timestamp { get; set; }

	}
}

