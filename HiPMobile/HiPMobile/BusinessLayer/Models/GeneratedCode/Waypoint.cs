﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool
//     Changes to this file will be lost if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
namespace de.upb.hip.mobile.pcl.BusinessLayer.Models
{
	using Realms;
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;

	public class Waypoint : RealmObject, IIdentifiable
	{
		//Attributes
		[ObjectId]
		private string _id{ get; set; }
		public string Id{
			get{ return _id; }
			set{ Realm.GetInstance ().Write (() => _id = value); }
		}

		private GeoPoint _location{ get; set; }
		public GeoPoint Location{
			get{ return _location; }
			set{ Realm.GetInstance ().Write (() => _location = value); }
		}

		private string _exhibitid{ get; set; }
		public string ExhibitId{
			get{ return _exhibitid; }
			set{ Realm.GetInstance ().Write (() => _exhibitid = value); }
		}

		//Associations
		// Contructor
		public Waypoint(){
		}
	}
}

