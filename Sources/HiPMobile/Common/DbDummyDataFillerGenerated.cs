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

        public static int DatabaseVersion { get; } = 90004;

        public void InsertData()
        {
            using (DbManager.StartTransaction())
            {
                var sprechertext_1 = CreateAudio(
    "sprechertext_1.mp3",
    "Während des bisherigen Rundgangs haben Sie erfahren, wie wichtig das Gebiet zwischen Lippe und Pader für die Politik Karls des Großen ab den 770er Jahren war. Erinnern wir uns nur an die große Reichsversammlung im Jahre 777! Zu diesem Anlass fanden sich Franken, Sachsen, aber auch arabische Gesandte aus Spanien hier in Paderborn zusammen.\nAber was fanden diese Personen hier vor? Wie hat man sich das damalige Paderborn, die sogenannte „urbs Karoli“, eigentlich vorzustellen? Lange Zeit fragten sich Historiker und Archäologen, ob die „urbs Karoli“ tatsächlich in Paderborn existierte und wenn ja, wo genau? <fn> Einen Überblick über die hierzu geäußerten Vermutungen bietet Birgit Mecke: Der Stand der Forschungen vor den Grabungskampagnen Winkelmanns, in: Sveva Gai / Birgit Mecke: Est locus insignis…: Die Pfalz Karls des Großen in Paderborn und ihre bauliche Entwicklung bis zum Jahr 1002. Die Neuauswertung der Ausgrabungen Wilhelm Winkelmanns in den Jahren 1964-1978 (Denkmalpflege und Forschung in Westfalen 40,2), Mainz 2004, Bd. 1, S. 1-8. </fn> Die karolingischen Geschichtswerke sprechen für Paderborn nicht von palatium, dem lateinischen Wort für „Pfalz“. Und es gab auch keine archäologischen Anhaltspunkte für die Pfalz Karls des Großen in Paderborn. Noch Mitte des 20. Jahrhunderts schrieb der Akademieprofessor und Domkapitular Alois Fuchs, dass „für eine [karolingische] Pfalz in Paderborn nicht nur alle urkundlichen Bezeugungen fehlen, sondern auch alle Baureste, die für die charakteristischen Pfalzgebäude, den Reichssaal und die Pfalzkapelle, sprechen könnten <fn>Alois A. Fuchs: Zur Frage der Bautätigkeit des Bischofs Badurad am Paderborner Dom, in: Westfälische Zeitschrift 97 (1947), S. 3-34, hier S. 5. </fn>.“  Sichtbar waren einzig verbaute Überreste der Domburg Bischof Meinwerks aus dem 11. Jahrhundert. Diese Überreste hatten bereits Mitte des 19. Jahrhunderts das Interesse von Lokalforschern geweckt <fn>Vgl. J. B. Johann Bernhard Greve: Der kaiserliche und bischöfliche Palast in Paderborn, in: Blätter zur näheren Kunde Westfalens 6/4 (1868), S. 33-38.</fn>.\nJetzt stehen Sie zwischen dem Dom und dem Museum in der Kaiserpfalz. Dieses große und repräsentative Gebäude mit den Rundbogenfenstern sieht so aus, wie man sich eine Kaiserpfalz vorstellt. Doch handelt es sich dabei um die Pfalz Karls des Großen? Nein! Es ist die archäologische Rekonstruktion der Pfalz Bischof Meinwerks aus dem frühen 11. Jahrhundert. \n Aber wo befand sich nun die karolingische Kaiserpfalz? Sehen Sie die etwa 31 mal 10 m große, rechteckige Fläche zwischen Ihnen und dem Museum? Sie ist durch Bruchsteinmauern abgegrenzt. Das sind die aus konservatorischen Gründen aufgemauerten Fundamente der sog. aula regia, der Königshalle Karls des Großen. Wenn Sie genau hinschauen, sehen Sie ein rotes Ziegelband. Dieses trennt das originale Bruchsteinmauerwerk von später, im Zuge der Rekonstruktion ergänzten Steinen.",
    ""
  );
                var sprechertext_2_1 = CreateAudio(
                    "sprechertext_2_1.mp3",
                    "Falls Sie Schwierigkeiten haben sollten, sich in diesem Mauergewirr zurechtzufinden, blicken Sie auf Ihr Display. Hier werden die Mauern der aula regia rot hervorgehoben.",
                    ""
                  );
                var sprechertext_2_2 = CreateAudio(
                    "sprechertext_2_2.mp3",
                    "Vor hundert Jahren hätten Sie davon noch nichts sehen können. Denn dieses Gelände war damals mit Fachwerkhäusern bebaut, die im 2. Weltkrieg zerstört wurden. Einen Eindruck, wie sich das Gelände nördlich des Doms verändert hat, bietet der Fotoslider auf Ihrem Display. Die Kriegszerstörungen boten aber auch neue Möglichkeiten. So legte man ab den 1950er Jahren Teile der karolingischen Befestigungsmauer frei und konnte so ihren Verlauf rekonstruieren.",
                    ""
                  );
                var kaiserPfalzImage = CreateImage(
                    "Die Pfalz Karls des Großen",
                    "Die Überreste der Kaiserpflaz von Westen aus betrachtet.",
                    "kaiserpfalz_teaser.jpg"
                  );
                var kaiserPfalzImage1 = CreateImage(
                    "Die Pfalz Karls des Großen",
                    "Die Überreste der Kaiserpflaz von Westen aus betrachtet.",
                    "kaiserpfalz_image_1.jpg"
                  );
                var kaiserpfalzImage2 = CreateImage(
                    "Von Westen aus betrachtet",
                    "Die Überreste der Kaiserpflaz mit rot eingefärbten Mauern der aula regia von Westen aus betrachtet.",
                    "kaiserpfalz_image_2.jpg"
                  );
                var kaiserpfalz_Slider1_1 = CreateImage(
                    "",
                    "Blick von Norden auf die Paderquellen vor 1945",
                    "kaiserpfalz_slider_1_1.jpg"
                  );
                var kaiserpfalz_Slider1_2 = CreateImage(
                    "",
                    "Blick von Norden über die Treppe vor 1945",
                    "kaiserpfalz_slider_1_2.jpg"
                  );
                var kaiserpfalz_Slider1_3 = CreateImage(
                    "",
                    "Blick von Südwesten vor 1935",
                    "kaiserpfalz_slider_1_3.jpg"
                  );
                var kaiserpfalz_Slider1_4 = CreateImage(
                    "",
                    "Blick von Norden vor 1945",
                    "kaiserpfalz_slider_1_4.jpg"
                  );
                var kaiserpfalz_Slider1_5 = CreateImage(
                    "",
                    "Blick von Norden um 1945",
                    "kaiserpfalz_slider_1_5.jpg"
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
                    new[] { "Kirche" },
                    new[] { "Dom" },
                    kaiserPfalzImage
                  );
                kaiserpfalz.Pages.Add(CreateAppetizerPage("Ein befestigter Stützpunkt in Sachsen – Aufbau und Entwicklung.", kaiserPfalzImage));
                kaiserpfalz.Pages.Add(CreateImagePage(kaiserPfalzImage1, null, null, sprechertext_1));
                kaiserpfalz.Pages.Add(CreateImagePage(kaiserpfalzImage2, null, null, sprechertext_2_1));
                kaiserpfalz.Pages.Add(CreateTimeSliderPage("", "", new long[] { 0, 1, 2, 3, 4 }, new[] { kaiserpfalz_Slider1_1, kaiserpfalz_Slider1_2, kaiserpfalz_Slider1_3, kaiserpfalz_Slider1_4, kaiserpfalz_Slider1_5 }, true, sprechertext_2_2));
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