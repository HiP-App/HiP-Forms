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
using System.IO;
using Android.Content.Res;
using de.upb.hip.mobile.pcl.BusinessLayer.Models;

namespace de.upb.hip.mobile.droid.Helpers {
    public class DbDummyDataFiller {

        private readonly AssetManager manager;

        public DbDummyDataFiller (AssetManager manager)
        {
            this.manager = manager;
        }

        public void InsertData ()
        {
            var dom = CreateExhibit ("Paderborner Dom",
                                     "Der Hohe Dom Ss. Maria, Liborius und Kilian ist die Kathedralkirche des Erzbistums Paderborn und liegt im Zentrum der Paderborner Innenstadt, oberhalb der Paderquellen.",
                                     51.718953, 8.75583, "dom.jpg", new[] {"Kirche"}, new[] {"Dom"});
            var uni = CreateExhibit ("Universität Paderborn",
                                     "Die Universität Paderborn in Paderborn, Deutschland, ist eine 1972 gegründete Universität in Nordrhein-Westfalen.",
                                     51.706768, 8.771104, "uni.jpg", new[] {"Uni"}, new[] {"Universität"});
            var exhibits = BusinessEntitiyFactory.CreateBusinessObject<ExhibitSet> ();
            exhibits.InitSet.Add (dom);
            exhibits.InitSet.Add (uni);
        }

        private Exhibit CreateExhibit (string name, string description, double latitude, double longitude,
                                       string imagePath, string[] tags, string[] categories)
        {
            var exhibit = BusinessEntitiyFactory.CreateBusinessObject<Exhibit> ();
            exhibit.Name = name;
            exhibit.Description = description;
            var position = BusinessEntitiyFactory.CreateBusinessObject<GeoLocation> ();
            position.Latitude = latitude;
            position.Longitude = longitude;
            exhibit.Location = position;
            var image = BusinessEntitiyFactory.CreateBusinessObject<Image> ();
            image.Data = LoadImageAsset (imagePath);
            exhibit.Image = image;
            foreach (var tag in tags)
            {
                var stringelement = BusinessEntitiyFactory.CreateBusinessObject<StringElement> ();
                stringelement.Value = tag;
                exhibit.Tags.Add (stringelement);
            }
            foreach (var category in categories)
            {
                var stringelement = BusinessEntitiyFactory.CreateBusinessObject<StringElement> ();
                stringelement.Value = category;
                exhibit.Categories.Add (stringelement);
            }
            return exhibit;
        }

        private byte[] LoadImageAsset (string name)
        {
            var buffer = new byte[16 * 1024];
            byte[] data;
            using (var input = manager.Open (name))
            {
                using (var ms = new MemoryStream ())
                {
                    int read;
                    while ((read = input.Read (buffer, 0, buffer.Length)) > 0)
                    {
                        ms.Write (buffer, 0, read);
                    }
                    data = ms.ToArray ();
                }
            }
            return data;
        }

    }
}