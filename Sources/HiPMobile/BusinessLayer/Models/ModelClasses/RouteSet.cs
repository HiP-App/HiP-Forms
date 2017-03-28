﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool
//     Changes to this file will be lost if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
/*Copyright (C) 2016 History in Paderborn App - Universit�t Paderborn
 
  Licensed under the Apache License, Version 2.0 (the "License");
  you may not use this file except in compliance with the License.
  You may obtain a copy of the License at
 
       http://www.apache.org/licenses/LICENSE-2.0
 
  Unless required by applicable law or agreed to in writing, software
  distributed under the License is distributed on an "AS IS" BASIS,
  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
  See the License for the specific language governing permissions and
  limitations under the License.*/
namespace PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.Models
{
	using Realms;
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;

	public class RouteSet : RealmObject, IIdentifiable
	{
		//Attributes
		[PrimaryKey]
		public string Id{ get; set; }

		//Associations
		public virtual IList<Route> Routes{ get; }

		// Contructor
		public RouteSet(){
		}
	}
}

