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

	public partial class Exhibit : RealmObject, IIdentifiable
	{
		//Attributes
		[ObjectId]
		private string _id{ get; set; }
		public string Id{
			get{ return _id; }
			set{ Realm.GetInstance ().Write (() => _id = value); }
		}

		private string _name{ get; set; }
		public string Name{
			get{ return _name; }
			set{ Realm.GetInstance ().Write (() => _name = value); }
		}

		private string _description{ get; set; }
		public string Description{
			get{ return _description; }
			set{ Realm.GetInstance ().Write (() => _description = value); }
		}

		private GeoPoint _location{ get; set; }
		public GeoPoint Location{
			get{ return _location; }
			set{ Realm.GetInstance ().Write (() => _location = value); }
		}

		private RealmList<StringElement> _categories{ get; }
		public IList<StringElement> Categories{
			get{ return _categories; }
		}

		private RealmList<StringElement> _tags{ get; }
		public IList<StringElement> Tags{
			get{ return _tags; }
		}

		//Associations
		private RealmList<Page> _pages;
		public IList<Page> Pages{
			 get{ return _pages; }
		}

		public virtual Image Image{ get; set; }

		// Contructor
		public Exhibit(){
		}
	}
}

