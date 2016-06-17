﻿//------------------------------------------------------------------------------
// <auto-generated>
//     Dieser Code wurde von einem Tool generiert.
//     Wenn der Code neu generiert wird, gehen alle Änderungen an dieser Datei verloren
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
namespace de.upb.hip.mobile.pcl.BusinessLayer.Models
{
	using Realms;
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;

	public class TextPage : RealmObject, IIdentifiable
	{
		//Attributes
		[ObjectId]
		private string _id{ get; set; }
		public string Id{
			get{ return _id; }
			set{ Realm.GetInstance ().Write (() => _id = value); }
		}

		private string _string{ get; set; }
		public string String{
			get{ return _string; }
			set{ Realm.GetInstance ().Write (() => _string = value); }
		}

		//Associations
		private Audio _audio{ get; set; }
		public Audio Audio{
			get{ return _audio; }
			set{ Realm.GetInstance ().Write (() => _audio = value); }
		}

		// Contructor
		public TextPage(){
		}
	}
}

