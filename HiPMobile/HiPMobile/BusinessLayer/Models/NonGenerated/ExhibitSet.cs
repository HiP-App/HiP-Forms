// Copyright (C) 2016 History in Paderborn App - Universität Paderborn
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//      http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Realms;

namespace de.upb.hip.mobile.pcl.BusinessLayer.Models {
    public partial class ExhibitSet : IEnumerable<Exhibit>
    {
        #region IEnumerable interface
        public Exhibit this[int index]
        {
            get { return ActiveSet.ElementAt<Exhibit>(index); }
            set { ActiveSet.Insert(index, value); }
        }

        public IEnumerator<Exhibit> GetEnumerator()
        {
            return ActiveSet.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        #endregion

        /// <summary>
        /// Update the currently active categories.
        /// </summary>
        /// <param name="categories">The categories to set, as strings.</param>
        public void UpdateCategories (List<string> categories)
        {
            foreach (Exhibit exhibit in ActiveSet)
            {
                ActiveSet.Remove (exhibit);
            }

            foreach (string category in categories)
            {
                foreach (Exhibit exhibit in InitSet)
                {
                    if (exhibit.Categories.Count (element => element.Value == category) > 0)
                    {
                        this.ActiveSet.Add (exhibit);
                    }
                }
            }

            this.OrderByDistance ();
        }

        /// <summary>
        /// Order the set of active exhibits by distance.
        /// </summary>
        private void OrderByDistance ()
        {
            List<Exhibit> tmpList = new List<Exhibit> ();

            double minDistance = 0;
            var minPosition = 0;
            double currentDistance;
            var i = 0;

            while (ActiveSet.Count > 0)
            {
                currentDistance = ActiveSet [i].GetDistance (Position);
                if (minDistance == 0)
                {
                    minDistance = currentDistance;
                    minPosition = i;
                }
                if (currentDistance < minDistance)
                {
                    minDistance = currentDistance;
                    minPosition = i;
                }
                if (i == ActiveSet.Count - 1)
                {
                    tmpList.Add (ActiveSet [minPosition]);
                    ActiveSet.RemoveAt (minPosition);
                    minDistance = 0;
                    i = 0;
                }
                else
                    i++;
            }

            foreach (Exhibit exhibit in ActiveSet)
            {
                ActiveSet.Remove (exhibit);
            }
            foreach (Exhibit exhibit in tmpList)
            {
                ActiveSet.Add (exhibit);
            }
        }

        public void UpdatePosition (GeoLocation position)
        {
            this.Position = position;

            this.OrderByDistance ();
        }

    }
}