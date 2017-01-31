/////////////////////////////////////////////////
// This file has been generated automatically! //
/////////////////////////////////////////////////


// Copyright (C) 2017 History in Paderborn App - Universität Paderborn
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

using de.upb.hip.mobile.pcl.BusinessLayer.Managers;
using de.upb.hip.mobile.pcl.BusinessLayer.Models;

namespace de.upb.hip.mobile.pcl.Common
{
    public partial class DbDummyDataFiller
    {

        public static int DatabaseVersion { get; } = 28065;

        public void InsertData()
        {
            using (DbManager.StartTransaction())
            {
                var audio1 = CreateAudio(
    "sprechertext.mp3",
    "Audio Caption",
    "Audio Title"
  );
                var kaiserPfalzImage = CreateImage(
                    "Die Pfalz Karls des Großen",
                    "Die Überreste der Kaiserpflaz von Westen aus betrachtet.",
                    "kaiserpfalz_teaser.jpg"
                  );
                var theoTeaser = CreateImage(
                    null,
                    null,
                    "theo_teaser.jpg"
                  );
                var kaiserpfalz = CreateExhibit(
                    "Die Pfalz Karls des Großen",
                    "",
                    51.7189826,
                    8.754652599999986,
                    new string[] { "Kirche" },
                    new string[] { "Dom" },
                    kaiserPfalzImage
                  );
                kaiserpfalz.Pages.Add(CreateAppetizerPage("Ein befestigter Stützpunkt in Sachsen – Aufbau und Entwicklung.", kaiserPfalzImage));
                var karlsrouteSet = DbManager.CreateBusinessObject<ExhibitSet>();
                karlsrouteSet.ActiveSet.Add(kaiserpfalz);
                var karlsroute = CreateRoute(
                    "Karlsroute",
                    "Auf der Spur Karls des Großen!",
                    1800,
                    4.2,
                    theoTeaser
                  );

                foreach (var exhibit in karlsrouteSet.ActiveSet)
                {
                    karlsroute.Waypoints.Add(CreateWayPoint(exhibit));
                }


            }
        }
    }
}