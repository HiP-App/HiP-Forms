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

using System.Collections.Generic;
using System.IO;
using Android.Content.Res;
using de.upb.hip.mobile.pcl.BusinessLayer.Models;
using Java.Lang;
using Realms;

namespace de.upb.hip.mobile.droid.Helpers {
    public class DbDummyDataFiller {

        private readonly string lorem =
            "Lorem ipsum dolor sit amet, consetetur sadipscing elitr, sed diam nonumy eirmod tempor invidunt ut labore et dolore magna aliquyam erat, sed diam voluptua. At vero eos et accusam et justo duo dolores et ea rebum. Stet clita kasd gubergren, no sea takimata sanctus est Lorem ipsum dolor sit amet. Lorem ipsum dolor sit amet, consetetur sadipscing elitr, sed diam nonumy eirmod tempor invidunt ut labore et dolore magna aliquyam erat, sed diam voluptua. At vero eos et accusam et justo duo dolores et ea rebum. Stet clita kasd gubergren, no sea takimata sanctus est Lorem ipsum dolor sit amet. Lorem ipsum dolor sit amet, consetetur sadipscing elitr, sed diam nonumy eirmod tempor invidunt ut labore et dolore magna aliquyam erat, sed diam voluptua. At vero eos et accusam et justo duo dolores et ea rebum. Stet clita kasd gubergren, no sea takimata sanctus est Lorem ipsum dolor sit amet." +
            "Duis autem vel eum iriure dolor in hendrerit in vulputate velit esse molestie consequat, vel illum dolore eu feugiat nulla facilisis at vero eros et accumsan et iusto odio dignissim qui blandit praesent luptatum zzril delenit augue duis dolore te feugait nulla facilisi.Lorem ipsum dolor sit amet, consectetuer adipiscing elit, sed diam nonummy nibh euismod tincidunt ut laoreet dolore magna aliquam erat volutpat." +
            "Ut wisi enim ad minim veniam, quis nostrud exerci tation ullamcorper suscipit lobortis nisl ut aliquip ex ea commodo consequat.Duis autem vel eum iriure dolor in hendrerit in vulputate velit esse molestie consequat, vel illum dolore eu feugiat nulla facilisis at vero eros et accumsan et iusto odio dignissim qui blandit praesent luptatum zzril delenit augue duis dolore te feugait nulla facilisi." +
            "Nam liber tempor cum soluta nobis eleifend option congue nihil imperdiet doming id quod mazim placerat facer possim assum.Lorem ipsum dolor sit amet, consectetuer adipiscing elit, sed diam nonummy nibh euismod tincidunt ut laoreet dolore magna aliquam erat volutpat.Ut wisi enim ad minim veniam, quis nostrud exerci tation ullamcorper suscipit lobortis nisl ut aliquip ex ea commodo consequat." +
            "Duis autem vel eum iriure dolor in hendrerit in vulputate velit esse molestie consequat, vel illum dolore eu feugiat nulla facilisis." +
            "At vero eos et accusam et justo duo dolores et ea rebum.Stet clita kasd gubergren, no sea takimata sanctus est Lorem ipsum dolor sit amet. Lorem ipsum dolor sit amet, consetetur sadipscing elitr, sed diam nonumy eirmod tempor invidunt ut labore et dolore magna aliquyam erat, sed diam voluptua.At vero eos et accusam et justo duo dolores et ea rebum. Stet clita kasd gubergren, no sea takimata sanctus est Lorem ipsum dolor sit amet. Lorem ipsum dolor sit amet, consetetur sadipscing elitr, At accusam aliquyam diam diam dolore dolores duo eirmod eos erat, et nonumy sed tempor et et invidunt justo labore Stet clita ea et gubergren, kasd magna no rebum. sanctus sea sed takimata ut vero voluptua.est Lorem ipsum dolor sit amet. Lorem ipsum dolor sit amet, consetetur sadipscing elitr, sed diam nonumy eirmod tempor invidunt ut labore et dolore magna aliquyam erat." +
            "Consetetur sadipscing elitr, sed diam nonumy eirmod tempor invidunt ut labore et dolore magna aliquyam erat, sed diam voluptua.At vero eos et accusam et justo duo dolores et ea rebum. Stet clita kasd gubergren, no sea takimata sanctus est Lorem ipsum dolor sit amet. Lorem ipsum dolor sit amet, consetetur sadipscing elitr, sed diam nonumy eirmod tempor invidunt ut labore et dolore magna aliquyam erat, sed diam voluptua.At vero eos et accusam et justo duo dolores et ea rebum. Stet clita";



        private readonly AssetManager manager;

        public DbDummyDataFiller (AssetManager manager)
        {
            this.manager = manager;
        }

        public void InsertData ()
        {
            ExhibitSet karlsrouteSet = BusinessEntitiyFactory.CreateBusinessObject<ExhibitSet>();
            Image kaiserPfalzImage = CreateImage("Die Pfalz Karls des Großen", "Die Überreste der Kaiserpflaz von Westen aus betrachtet.", "kaiserpfalz_teaser.jpg");
            Exhibit kaiserpfalz = CreateExhibit( "Die Pfalz Karls des Großen", "", 51.7189826, 8.754652599999986,
                    new [] { "Kirche" }, new [] { "Dom" }, kaiserPfalzImage);


            kaiserpfalz.Pages.Add(CreateAppetizerPage("Ein befestigter Stützpunkt in Sachsen – Aufbau und Entwicklung.",
                    kaiserPfalzImage));

            var audio1 = CreateAudio("sprechertext_1.mp3",
                "Während des bisherigen Rundgangs haben Sie erfahren, wie wichtig das Gebiet zwischen Lippe und Pader für die Politik Karls des Großen ab den 770er Jahren war. Erinnern wir uns nur an die große Reichsversammlung im Jahre 777! Zu diesem Anlass fanden sich Franken, Sachsen, aber auch arabische Gesandte aus Spanien hier in Paderborn zusammen.\\n\" +\r\n            \"Aber was fanden diese Personen hier vor? Wie hat man sich das damalige Paderborn, die sogenannte „urbs Karoli“, eigentlich vorzustellen? Lange Zeit fragten sich Historiker und Archäologen, ob die „urbs Karoli“ tatsächlich in Paderborn existierte und wenn ja, wo genau? <fn> Einen Überblick über die hierzu geäußerten Vermutungen bietet Birgit Mecke: Der Stand der Forschungen vor den Grabungskampagnen Winkelmanns, in: Sveva Gai / Birgit Mecke: Est locus insignis…: Die Pfalz Karls des Großen in Paderborn und ihre bauliche Entwicklung bis zum Jahr 1002. Die Neuauswertung der Ausgrabungen Wilhelm Winkelmanns in den Jahren 1964-1978 (Denkmalpflege und Forschung in Westfalen 40,2), Mainz 2004, Bd. 1, S. 1-8. </fn> Die karolingischen Geschichtswerke sprechen für Paderborn nicht von palatium,  \\n\" +\r\n            \"dem  \\n\" +\r\n            \"lateinischen Wort für „Pfalz“. Und es gab auch keine archäologischen Anhaltspunkte für die Pfalz Karls des Großen in Paderborn. Noch Mitte des 20. Jahrhunderts schrieb der Akademieprofessor und Domkapitular Alois Fuchs, dass „für eine [karolingische] Pfalz in Paderborn nicht nur alle urkundlichen Bezeugungen fehlen, sondern auch alle Baureste, die für die charakteristischen Pfalzgebäude, den Reichssaal und die Pfalzkapelle, sprechen könnten <fn>Alois A. Fuchs: Zur Frage der Bautätigkeit des Bischofs Badurad am Paderborner Dom, in: Westfälische Zeitschrift 97 (1947), S. 3-34, hier S. 5. </fn>.“  Sichtbar waren einzig verbaute Überreste der Domburg Bischof Meinwerks aus dem 11. Jahrhundert. Diese Überreste hatten bereits Mitte des 19. Jahrhunderts das Interesse von Lokalforschern geweckt <fn>Vgl. J. B. Johann Bernhard Greve: Der kaiserliche und bischöfliche Palast in Paderborn, in: Blätter zur näheren Kunde Westfalens 6/4 (1868), S. 33-38.</fn>. \\n\" +\r\n            \"Jetzt stehen Sie zwischen dem Dom und dem Museum in der Kaiserpfalz. Dieses große und repräsentative Gebäude mit den Rundbogenfenstern sieht so aus, wie man sich eine Kaiserpfalz vorstellt. Doch handelt es sich dabei um die Pfalz Karls des Großen? Nein! Es ist die archäologische Rekonstruktion der Pfalz Bischof Meinwerks aus dem frühen 11. Jahrhundert. \\n Aber wo befand sich nun die karolingische Kaiserpfalz? Sehen Sie die etwa 31 mal 10 m große, rechteckige Fläche zwischen Ihnen und dem Museum? Sie ist durch Bruchsteinmauern abgegrenzt. Das sind die aus konservatorischen Gründen aufgemauerten Fundamente der sog. aula regia, der Königshalle Karls des Großen. Wenn Sie genau hinschauen, sehen Sie ein rotes Ziegelband. Dieses trennt das originale Bruchsteinmauerwerk von später, im Zuge der Rekonstruktion ergänzten Steinen.",
                "");
            kaiserpfalz.Pages.Add(CreateImagePage(kaiserPfalzImage, null, null, audio1));
            var audio2_1 = CreateAudio("sprechertext_2_1.mp3",
                "Falls Sie Schwierigkeiten haben sollten, sich in diesem Mauergewirr zurechtzufinden, blicken Sie auf Ihr Display. Hier werden die Mauern der aula regia rot hervorgehoben.",
                "");
            kaiserpfalz.Pages.Add(CreateImagePage(CreateImage("Von Westen aus betrachtet", "Die Überreste der Kaiserpflaz mit rot eingefärbten Mauern der aula regia von Westen aus betrachtet.", "kaiserpfalz_image_2.jpg"), null, null, audio2_1));

            List<Image> slider1 = new List<Image>();
            List<long> sliderTimes1 = new List<long>();
            slider1.Add(CreateImage("", "Blick von Norden auf die Paderquellen vor 1945", "kaiserpfalz_slider_1_1.jpg"));
            sliderTimes1.Add(0);
            slider1.Add(CreateImage("", "Blick von Norden über die Treppe vor 1945 ", "kaiserpfalz_slider_1_2.jpg"));
            sliderTimes1.Add(1);
            slider1.Add(CreateImage("", "Blick von Südwesten vor 1935", "kaiserpfalz_slider_1_3.jpg"));
            sliderTimes1.Add(2);
            slider1.Add(CreateImage("", "Blick von Norden vor 1945", "kaiserpfalz_slider_1_4.jpg"));
            sliderTimes1.Add(3);
            slider1.Add(CreateImage("", "Blick von Norden um 1945", "kaiserpfalz_slider_1_5.jpg"));
            sliderTimes1.Add(4);
            var audio2_2 = CreateAudio("sprechertext_2_2.mp3",
                "Vor hundert Jahren hätten Sie davon noch nichts sehen können. Denn dieses Gelände war damals mit Fachwerkhäusern bebaut, die im 2. Weltkrieg zerstört wurden. Einen Eindruck, wie sich das Gelände nördlich des Doms verändert hat, bietet der Fotoslider auf Ihrem Display. Die Kriegszerstörungen boten aber auch neue Möglichkeiten. So legte man ab den 1950er Jahren Teile der karolingischen Befestigungsmauer frei und konnte so ihren Verlauf rekonstruieren.",
                "");
            kaiserpfalz.Pages.Add(CreateTimeSliderPage("Gelände nördlich des Doms", "Fotografien des Geländes nördlich des Domes, zu Beginn des 20. Jh., in den 1930er Jahren, in den 1950er Jahren und während der Grabung. \n[Bildnachweis: Gai / Mecke 2004, S. 8, Abb. 5 und 6, S. 9, Abb. 7, S. 14, Abb. 11, S. 15, Abb. 12]", sliderTimes1.ToArray(), slider1.ToArray(), true, audio2_2));


            kaiserpfalz.Pages.Add(CreateImagePage(CreateImage("Befestigungsanlage", "Die vermutliche Ausdehnung der Befestigungsanlage in karolingischer Zeit. \n[Bildnachweis: Gai / Mecke 2004, Bd. 1, S. 102, Abb. 51]", "kaiserpfalz_image_3_1.jpg"), null, null, CreateAudio("sprechertext_3_1.mp3", "Auf Ihrem Display sehen Sie nun einen Plan der Paderborner Innenstadt. Hier ist der Verlauf der karolingischen Befestigungsmauer unter Berücksichtigung der Grabungsergebnisse von Marianne Moser eingezeichnet <fn>Vgl. Sveva Gai: Die Errichtung einer Befestigungsanlage, in: Sveva Gai / Birgit Mecke: Est locus insignis…: Die Pfalz Karls des Großen in Paderborn und ihre bauliche Entwicklung bis zum Jahr 1002. Die Neuauswertung der Ausgrabungen Wilhelm Winkelmanns in den Jahren 1964-1978 (Denkmalpflege und Forschung in Westfalen 40,2), Mainz 2004, Bd. 1, S. 95-102, unter Berücksichtigung von Marianne Moser: Neue Beobachtungen zu Struktur und Entwicklung der Domburgbefestigung. Eine kritische Betrachtung bisheriger Interpretationen aufgrund der Zusammenschau zahlreicher Hinweise. Unveröffentlichtes Manuskript.</fn>.  Sie sehen, dass die Mauer ein annähernd quadratisches Areal von 280 mal 250 m um den heutigen Dom einfasste. Diese Kalkbruchsteinmauer ersetzte die frühere Holz-Erde-Konstruktion nach der Zerstörung der urbs Karoli durch die Sachsen im Jahre 778. Eine solche gemauerte Befestigung war zu dieser Zeit unüblich. In Paderborn war sie aber aufgrund der Sachsenkriege zwingend notwendig.\n" +
            "Aber zurück zu unserem stark zerstörten Gelände der Nachkriegszeit. Es sollte 1963 neu erschlossen werden. Bei Baggerarbeiten stieß man auf ältere Gebäudemauern. Deshalb entschloss man sich, das gesamte Areal archäologisch zu untersuchen. Die systematische Freilegung begann 1964 und wurde 14 Jahre lang unter der Leitung des Archäologen Wilhelm Winkelmann fortgeführt <fn> Einen Überblick über den Fortgang und die Dokumentation der Ausgrabungen vermittelt Birgit Mecke: Die Ausgrabungen der Jahre 1963 bis 1978, in: Sveva Gai / Birgit Mecke: Est locus insignis…: Die Pfalz Karls des Großen in Paderborn und ihre bauliche Entwicklung bis zum Jahr 1002. Die Neuauswertung der Ausgrabungen Wilhelm Winkelmanns in den Jahren 1964-1978 (Denkmalpflege und Forschung in Westfalen 40,2), Mainz 2004, Bd. 1, S. 9-44. </fn>.", "")));
            kaiserpfalz.Pages.Add(CreateImagePage(CreateImage("Blick auf die Grabungsfläche", "Blick vom Dom auf die Grabungsfläche der Paderborner Pfalz. \n[Bildnachweis: 799. Kunst und Kultur der Karolingerzeit. Karl der Große und Papst Leo III. in Paderborn. Beiträge zum Katalog der Ausstellung Paderborn 1999, Mainz 1999, S. 178, Abb. 3]", "kaiserpfalz_image_3_2.jpg"), null, null, CreateAudio("sprechertext_3_2.mp3", "Einen Überblick über die verschiedenen mittelalterlichen Gebäudereste aus unterschiedlichen Jahrhunderten bietet die Fotografie, die Sie nun auf Ihrem Display sehen können. Sie wurde Mitte der 60er Jahre während der Ausgrabungen vom Domturm herab gemacht und zeigt die gesamte Grabungsfläche nördlich des Domes. Die Ausgräber legten nicht nur die ottonisch-salische Pfalz frei - hier gelb markiert -, sondern sie fanden auch die Überreste der karolingischen Pfalz - hier rot markiert.\n" +
            "Und genau inmitten dieses Areals stehen Sie nun. Eine Abbildung in Ihrer App hilft Ihnen, die im Folgenden beschriebenen baulichen Elemente der Karlspfalz zu identifizieren.", "")));
            kaiserpfalz.Pages.Add(CreateImagePage(CreateImage("Plan der Bauphasen", "Plan der Bauphasen der Paderborner Kaiserpfalz. \n[Bildnachweis: Gai / Mecke 2004, Bd. 1, S. 108]", "kaiserpfalz_image_4.jpg"), null, null, CreateAudio("sprechertext_4.mp3", "Kommen wir wieder auf die aula regia Karls des Großen zurück. Wie Sie anhand der Bruchsteinmauern sehen, handelte es sich um ein einfaches rechteckiges Steingebäude. Auf der Ihnen zugewandten Seite, der Südseite, befanden sich zwei Eingänge, die zu den Wirtschaftsräumen führten <fn> Vgl. Sveva Gai: Der Bau der Aula (776), in: Sveva Gai / Birgit Mecke: Est locus insignis…: Die Pfalz Karls des Großen in Paderborn und ihre bauliche Entwicklung bis zum Jahr 1002. Die Neuauswertung der Ausgrabungen Wilhelm Winkelmanns in den Jahren 1964-1978 (Denkmalpflege und Forschung in Westfalen 40,2), Mainz 2004, Bd. 1, S. 103-114, hier S. 103. </fn>. Trotz der geringen Größe wird es seiner Zeit die Sachsen vor Ort in Staunen versetzt haben, waren Steinbauten in dieser Region doch eine absolute Ausnahme <fn> Vgl. Sveva Gai: Die Pfalzenarchitektur der Karolingerzeit, in: Sveva Gai / Birgit Mecke: Est locus insignis…: Die Pfalz Karls des Großen in Paderborn und ihre bauliche Entwicklung bis zum Jahr 1002. Die Neuauswertung der Ausgrabungen Wilhelm Winkelmanns in den Jahren 1964-1978 (Denkmalpflege und Forschung in Westfalen 40,2), Mainz 2004, Bd. 1, S. 185-198, hier S. 195. </fn>.  Und noch etwas muss sie überrascht haben: Dieses Gebäude aus Stein war zweigeschossig. Spätestens nach der Zerstörung beim Sachsenaufstand 778 hatte die wiederaufgebaute aula regia ein repräsentatives Obergeschoss und ein kellerartiges Untergeschoss mit fünf Wirtschaftsräumen <fn> Vgl. Sveva Gai: Der Bau der Aula (776), in: Sveva Gai / Birgit Mecke: Est locus insignis…: Die Pfalz Karls des Großen in Paderborn und ihre bauliche Entwicklung bis zum Jahr 1002. Die Neuauswertung der Ausgrabungen Wilhelm Winkelmanns in den Jahren 1964-1978 (Denkmalpflege und Forschung in Westfalen 40,2), Mainz 2004, Bd. 1, S. 103-114, hier S. 107. </fn>.  Sichtbar ist heute nur noch der Grundriss im Untergeschoss. Sie müssen sich vorstellen, dass das Gebäude vermutlich fünf bis sechs Meter in die Höhe ragte. Ein für diese Zeit sicherlich beeindruckender Bau! Sehen Sie das viereckige Fundament an der Südwest-Ecke, d.h. zu Ihrer Linken? \n" +
            "Es sind vermutlich die Fundamentreste eines Balkons oder Altans <fn> Vgl. Sveva Gai: Der Bau der Aula (776), in: Sveva Gai / Birgit Mecke: Est locus insignis…: Die Pfalz Karls des Großen in Paderborn und ihre bauliche Entwicklung bis zum Jahr 1002. Die Neuauswertung der Ausgrabungen Wilhelm Winkelmanns in den Jahren 1964-1978 (Denkmalpflege und Forschung in Westfalen 40,2), Mainz 2004, Bd. 1, S. 103-114, hier S. 113. </fn>.  Ein Balkon oder Altan, d.h. ein „vom Erdboden aus gestützter balkonartiger Anbau“ <fn> Art. Altan, in: DUDEN. Fremdwörterbuch, 6., auf der Grundlage der amtlichen Neuregelung der deutschen Rechtschreibung überarbeitete und erweiterte Auflage, Mannheim u.a.,1997, S. 53. </fn>,  diente dem Herrscher zum repräsentativen Auftritt: Hier konnte er direkt aus dem Festsaal, der aula regia, im Obergeschoss vor sein Gefolge treten und aus erhöhter Position sprechen. Dies war ein deutliches Zeichen seiner Königswürde. Dabei stellte sich auch schon früh die Frage, wo Karl der Große im Festsaal Platz nahm. Doch dazu gibt es leider weder Aussagen in Geschichtswerken noch einen konkreten archäologischen Befund. Vermutlich saß Karl auf einem mobilen, klappbaren Thron, der entweder auf der nördlichen Längsseite – gegenüber dem Eingang – oder auf einer der Schmalseiten im Obergeschoss gestanden haben könnte <fn> Vgl. Sveva Gai: Der Bau der Aula (776), in: Sveva Gai / Birgit Mecke: Est locus insignis…: Die Pfalz Karls des Großen in Paderborn und ihre bauliche Entwicklung bis zum Jahr 1002. Die Neuauswertung der Ausgrabungen Wilhelm Winkelmanns in den Jahren 1964-1978 (Denkmalpflege und Forschung in Westfalen 40,2), Mainz 2004, Bd. 1, S. 103-114, hier S. 107-113. </fn>. \n" +
            "Schauen Sie nach rechts auf die gegenüberliegende, südöstliche Seite. Sehen Sie die dunkelgraue Pflastersteine in Höhe des heutigen Bodenniveaus? Sie markieren die zwei parallel verlaufenden Mauern, die den repräsentativen Zugang zum Obergeschoss bildeten <fn> Vgl. Sveva Gai: Der Wiederaufbau der Pfalz mit der Errichtung des östlichen Zugangs zur Aula. Phase Ib, in: Sveva Gai / Birgit Mecke: Est locus insignis…: Die Pfalz Karls des Großen in Paderborn und ihre bauliche Entwicklung bis zum Jahr 1002. Die Neuauswertung der Ausgrabungen Wilhelm Winkelmanns in den Jahren 1964-1978 (Denkmalpflege und Forschung in Westfalen 40,2), Mainz 2004, Bd. 1, S. 120-121. </fn>.", "")));
            kaiserpfalz.Pages.Add(CreateImagePage(CreateImage("Rekonstruktion der Aula", "Rekonstruktion der Aula , Bauphase Ib. \n[Bildnachweis: Gai / Mecke 2004, S. 120, Abb. 68]", "kaiserpfalz_image_5_1.jpg"), null, null, CreateAudio("sprechertext_5_1.mp3", "Eine Vorstellung, wie diese aula regia bei der Reichsversammlung im Jahre 785 ausgesehen haben könnte, vermittelt Ihnen die Zeichnung der Archäologinnen Sveva Gai und Birgit Mecke auf Ihrem Display.\n" +
            "Aber zurück zur dunkelgrauen Pflasterung... Sehen Sie vielleicht noch andere Stellen, die diese Pflasterung aufweisen? Ja, genau. Neben der Bartholomäuskapelle ist der Mauerverlauf eines weiteren karolingischen Gebäudes so gekennzeichnet. Im Gegensatz zur aula regia können Sie diesen Ort begehen. Hier stand vor 799 eine Kirche. Sie war dem Salvator, <fn> Annales Sangallenses Baluzii ad a. 777, ed. von Georg Heinrich Pertz (MGH SS 1), Hannover 1826, S. 63: hoc anno fuit domnus rex Karlus in Saxonia ad Patrisbrunna, et ibi aedificavit ecclesiam in honore Salvatoris. </fn>  also Jesus Christus, geweiht, ungefähr so groß wie die aula regia und ebenfalls aus Kalkbruchstein errichtet. Der Bau einer steinernen Kirche ist auch eine politische Aussage. Mitten im heidnischen Sachsen baute man schon früh einen repräsentativen Sakralbau, der vermutlich zugleich als Pfalz- und Missionskirche diente <fn> Vgl. Sveva Gai: Die Salvatorkirche mit dem sog. „Atrium“ (777), in: Sveva Gai / Birgit Mecke: Est locus insignis…: Die Pfalz Karls des Großen in Paderborn und ihre bauliche Entwicklung bis zum Jahr 1002. Die Neuauswertung der Ausgrabungen Wilhelm Winkelmanns in den Jahren 1964-1978 (Denkmalpflege und Forschung in Westfalen 40,2), Mainz 2004, Bd. 1, S. 115-117, hier S. 116. </fn>.", "")));
            kaiserpfalz.Pages.Add(CreateImagePage(CreateImage("Grundriss der Salvatorkirche", "Grundrisses der Salvatorkirche. \n[Bildnachweis: Gai / Mecke 2004, Bd. 1, S. 108]", "kaiserpfalz_image_5_2.jpg"), null, null, CreateAudio("sprechertext_5_2.mp3", "Wie Sie sicherlich schon bemerkt haben, zeichnet die Pflasterung keinen vollständigen Grundriss nach. Die Linien verschwinden unter dem heutigen Dom. Den gesamten Grundriss dieser Salvatorkirche können Sie nun auf Ihrem Display sehen. Es handelte sich vermutlich um eine einschiffige Saalkirche, deren rechteckiger Chor im Osten dreigeteilt war. Im Westen schloss die Kirche mit einem rechteckigen Westbau ab, dessen Gestalt unklar bleibt.", "")));
            kaiserpfalz.Pages.Add(CreateImagePage(CreateImage("Rekonstruktion der Salvatorkirche", "Rekonstruktion der Salvatorkirche. \n[Bildnachweis: Gai / Mecke 2004, Bd. 1, S. 122]", "kaiserpfalz_image_5_3.jpg"), null, null, CreateAudio("sprechertext_5_3.mp3", "Auf Ihrem Display sehen Sie nun den neuesten Rekonstruktionsvorschlag der Paderborner Stadtarchäologin Sveva Gai. Sie vermutet, dass es sich um einen doppelgeschossigen Baukörper handelte <fn> Vgl. Sveva Gai: Die Salvatorkirche mit dem sog. „Atrium“ (777), in: Sveva Gai / Birgit Mecke: Est locus insignis…: Die Pfalz Karls des Großen in Paderborn und ihre bauliche Entwicklung bis zum Jahr 1002. Die Neuauswertung der Ausgrabungen Wilhelm Winkelmanns in den Jahren 1964-1978 (Denkmalpflege und Forschung in Westfalen 40,2), Mainz 2004, Bd. 1, S. 115-117, hier S. 116. </fn>. Im Untergeschoss wurden 16 Gräber von Männern, Frauen und Kindern freigelegt. Die Bestattung im Westbau der Kirche war ein „besonder[s], privilegierte[r] Bestattungsplatz“ <fn> Vgl. Sveva Gai: Bauliche Veränderungen im Kirchenbereich nach der Zerstörung von 778. Phase Ib, in: Sveva Gai / Birgit Mecke: Est locus insignis…: Die Pfalz Karls des Großen in Paderborn und ihre bauliche Entwicklung bis zum Jahr 1002. Die Neuauswertung der Ausgrabungen Wilhelm Winkelmanns in den Jahren 1964-1978 (Denkmalpflege und Forschung in Westfalen 40,2), Mainz 2004, Bd. 1, S. 122-124, hier S. 122. </fn>.  Weitere hunderte von Gräbern fanden sich auch südlich der damaligen Salvatorkirche. \n" +
            "Neben diesen beiden Steinbauten gibt es in der frühen Pfalzanlage auch Hinweise für zahlreiche Holzbauten und die Anwesenheit verschiedener Handwerker, unter ihnen Maurer, Steinmetze, Schmiede, Glasmacher und Bleigießer <fn> Vgl. Sveva Gai: Werkstätten und Pfostenbebauungen, in: Sveva Gai / Birgit Mecke: Est locus insignis…: Die Pfalz Karls des Großen in Paderborn und ihre bauliche Entwicklung bis zum Jahr 1002. Die Neuauswertung der Ausgrabungen Wilhelm Winkelmanns in den Jahren 1964-1978 (Denkmalpflege und Forschung in Westfalen 40,2), Mainz 2004, Bd. 1, S. 117-119, hier S. 117. </fn>.  Besonders während der 770er Jahre waren diese Handwerker unentbehrlich. Sie halfen beim Bau der Anlage und zogen später weiter zu neuen Auftraggebern – die Mobilität der Menschen zu dieser Zeit ist nicht zu unterschätzen!\n" +
            "Viele Handwerker wurden wieder gegen Ende des 8. Jahrhunderts gebraucht. Damals beschloss man nämlich die Erweiterung der aula regia. Im Nordwesten wurde ein steinerner Wohntrakt angebaut <fn> Vgl. Sveva Gai: Der nördliche Wohntrakt, in: Sveva Gai / Birgit Mecke: Est locus insignis…: Die Pfalz Karls des Großen in Paderborn und ihre bauliche Entwicklung bis zum Jahr 1002. Die Neuauswertung der Ausgrabungen Wilhelm Winkelmanns in den Jahren 1964-1978 (Denkmalpflege und Forschung in Westfalen 40,2), Mainz 2004, Bd. 1, S. 129-130. </fn>.  Die Salvatorkirche wurde durch die „ecclesiam mira[e] magnitudinis“ <fn> Annales Laureshamenses ad a. 799, ed. von Georg Heinrich Pertz (MGH SS 1), Hannover 1826, S. 38: (...) et postea cum pace et honore magno eum remisit ad propriam sedem (…) et [rex] ibi ad Padresbrunnun aedificavit ecclesiam mira magnitudinis, et fecit eam dedicare (...). </fn>,  das heisst eine „Kirche von wunderbarer Größe“ ersetzt. Die nun der Gottesmutter und dem hl. Kilian geweihte Kirche maß tatsächlich 21 x 42,7m und gehörte somit zu den großen Kirchenbauten des Frankenreiches, vergleichbar etwa der Klosterkirche in Lorsch oder der Abteikirche in Saint-Denis. <fn> Vgl. Uwe Lobbedey: Der Paderborner Dom. Vorgeschichte, Bau und Fortleben einer westfälischen Bischofskirche, München / Berlin 1990, S. 14f. </fn>", "")));
            kaiserpfalz.Pages.Add(CreateImagePage(CreateImage("Karolingische Pfalzanlage", "Plan der karolingischen Pfalzanlage um 800. \n[Bildnachweis: Gai / Mecke 2004, Bd. I, S. 132, Abb. 77]", "kaiserpfalz_image_7_1.jpg"), null, null, CreateAudio("sprechertext_7_1.mp3", "Wie sah die karolingische Pfalz um 800 also aus? Schauen Sie dazu bitte auf Ihr Display. Sie sehen dort den Plan der Anlage zu dieser Zeit. Im unteren Bereich können Sie den Grundriss der neugebauten Kirche „von wunderbarer Größe“ erkennen. Ihre Fundamentmauern sind unter dem heutigen Dom noch erhalten. [Für weiterführende Informationen zur Besichtigung der Ausgrabungen klicken Sie bitte hier] Bei der Kirche handelte es sich um eine dreischiffige Basilika ohne Querhaus, die an die Tradition spätantiker Kirchenbauten anknüpfte <fn> Vgl. Sveva Gai: Der Bau der ecclesia mirae magnitudinis, in: Sveva Gai / Birgit Mecke: Est locus insignis…: Die Pfalz Karls des Großen in Paderborn und ihre bauliche Entwicklung bis zum Jahr 1002. Die Neuauswertung der Ausgrabungen Wilhelm Winkelmanns in den Jahren 1964-1978 (Denkmalpflege und Forschung in Westfalen 40,2), Mainz 2004, Bd. 1, S. 130-133, hier S. 131. </fn>.  Solche basilikalen Großbauten waren den zentralen Pfalzen vorbehalten <fn> Vgl. Gerhard G. Streich: Burg und Kirche während des deutschen Mittelalters. Untersuchungen zur Sakraltopographie von Pfalzen, Burgen und Herrensitzen (Vorträge und Forschungen, Sonderband 29 I, II), 2 Bde., Sigmaringen 1984, hier Bd. 1, S. 36. </fn>.  Hierin zeigt sich die Aufwertung der Paderborner Pfalz am Ende der Sachsenkriege. Sie sehen, wie sich die urbs Karoli innerhalb zweier Jahrzehnte zu einem zentralen Ort in Sachsen entwickelt hat. Im Vergleich zu Pfalzen im fränkischen Kernland, wie etwa Aachen oder Ingelheim, war sie jedoch von geringem Ausmaß.", "")));

            List<Image> slider2 = new List<Image>();
            List<long> sliderTimes2 = new List<long>();
            slider2.Add(CreateImage("", "Pfalzanlage Paderborn (Bauhhase II, 799)", "kaiserpfalz_slider_7_1.jpg"));
            sliderTimes2.Add(0);
            slider2.Add(CreateImage("", "Aachen, die karolingische Pfalz, erhaltene Fundamente. \n[Bildnachweis: Keller, Christoph : Archäologische Forschungen in Aachen, Mainz 2004, S. 52]", "kaiserpfalz_slider_7_2.jpg"));
            sliderTimes2.Add(1);
            slider2.Add(CreateImage("", "Schematischer Gesamtplan des karolingischen Zustands der Pfalz von Ingelheim am Rhein, um 800. \n" +
                    "[Bildnachweis: Untermann, Matthias : Architektur im frühen Mittelalter, Darmstadt 2006, S. 119, Abb. 105]", "kaiserpfalz_slider_7_3.jpg"));
            sliderTimes2.Add(2);
            kaiserpfalz.Pages.Add(CreateTimeSliderPage("Grundrisse", "Maßstabsgetreue Grundrisse der Pfalzanlagen in Paderborn, Aachen und Ingelheim.", sliderTimes2.ToArray(), slider2.ToArray(), true, CreateAudio("sprechertext_7_2.mp3", "Einen Eindruck der Größenverhältnisse vermittelt Ihnen der Slider.", "")));

            kaiserpfalz.Pages.Add(CreateImagePage(CreateImage("Museum in der Kaiserpfalz", "Entdecken Sie im Museum in der Kaiserpfalz Fundstücke aus der karolingischen Pfalzanlage!", "kaiserpfalz_teaser.jpg"), null, null, CreateAudio("sprechertext_8.mp3", "Nun haben Sie bereits viele Pläne und Rekonstruktionen der Gebäude gesehen. Aber weiß man denn, wie sie innen aussahen?\n" +
            "Eine Vielzahl archäologischer Funde erlaubt Rückschlüsse auf die Ausstattung der Gebäude. Allerdings lassen sich diese Funde nicht bestimmten Gebäuden zuordnen, da sie in großflächigen Schuttablagerungen gefunden wurden. Diese Schuttablagerungen fielen rund zweihundert Jahre später an, als die karolingische Pfalz abgetragen wurde, um unter Bischof Meinwerk neuen Gebäuden Raum zu geben <fn> Vgl. Matthias Preißler: Die karolingischen Malereifragmente aus Paderborn. Zu den Putzfunden aus der Pfalzanlage Karls des Großen. Archäologie der Wandmalerei (Denkmalpflege und Forschung in Westfalen 40,1), Mainz 2003, S. 130. </fn>.  Im Museum in der Kaiserpfalz können Sie eine Auswahl dieser Funde sehen. Dazu gehören etwa geritzte Ziegelplatten, die vermutlich zur Trauf- oder Eckverzierung kleinerer Bauglieder dienten. Ferner Fragmente von Fensterglas und die dazugehörigen Bleistege. Sie werden in der Fachsprache auch Bleiruten genannt. Mit ihnen wurden die einzelnen Glasteile des Fensters ehemals zusammengehalten. Darüber hinaus sind unterschiedliche Bauskulpturen und auch Wandmalereifragmente zu besichtigen. All dies zeugt von prächtig ausgestatteten Gebäuden <fn> Vgl. Birgit Mecke: Zur Ausstattung der karolingischen Pfalzanlage: Architektonische Gestaltung und Einrichtung, in: Sveva Gai / Birgit Mecke: Est locus insignis…: Die Pfalz Karls des Großen in Paderborn und ihre bauliche Entwicklung bis zum Jahr 1002. Die Neuauswertung der Ausgrabungen Wilhelm Winkelmanns in den Jahren 1964-1978 (Denkmalpflege und Forschung in Westfalen 40,2), Mainz 2004, Bd. 1, S. 173-184. </fn>.  \n" +
            "Eine 3D-Rekonstruktion der karolingischen Pfalz stellt uns freundlicherweise das Museum in der Kaiserpfalz zur Verfügung. Das dreieinhalbminütige Video können Sie nun auf Ihrem Display ansehen.\n Es visualisiert einen denkbaren Zustand der Pfalz im Jahre 799.", "")));

            var mariensaeuleImage = CreateImage("", "Die Mariensäule", "mariensaeule_teaser.jpg");
            Exhibit mariensaeule = CreateExhibit("Die Mariensäule", "", 51.716724, 8.752244000000019,
                    new [] { "Kirch" }, new [] { "Dom" }, mariensaeuleImage);
            mariensaeule.Pages.Add(CreateAppetizerPage("Startpunkt der Rundgänge: Hl. Liborius, Karl der Große, Meinwerk von Paderborn.",
                    mariensaeuleImage));
            karlsrouteSet.ActiveSet.Add(mariensaeule);

            var paderquellen1Image = CreateImage("", "Paderbrunnon, Patresbrun, Paderbrunno", "quellen1_teaser.jpg");
            Exhibit paderquellen1 = CreateExhibit("Paderbrunnon, Patresbrun, Paderbrunno", "", 51.71861412677083, 8.75122457742691,
                    new [] { "Kirche" }, new [] { "Dom" }, paderquellen1Image);
            paderquellen1.Pages.Add(CreateAppetizerPage("Die Siedlung an den 200 Quellen. Woher kommt eigentlich der Name Paderborn?",
                    paderquellen1Image));           
            karlsrouteSet.ActiveSet.Add(paderquellen1);

            var paderquellen2Image = CreateImage("", "Leben am Wasser", "quellen2_teaser.jpg");
            Exhibit paderquellen2 = CreateExhibit("Leben am Wasser", "", 51.718811867802174, 8.751070350408554,
                    new [] { "Kirche" }, new [] { "Dom" }, paderquellen2Image);
            paderquellen2.Pages.Add(CreateAppetizerPage("Paderborn – so schön wie das Land, in dem Milch und Honig fließen?",
                    paderquellen2Image));            
            karlsrouteSet.ActiveSet.Add(paderquellen2);

            var paderquellen3Image = CreateImage("", "Taufen an der Pader?", "quellen3_teaser.jpg");
            Exhibit paderquellen3 = CreateExhibit("Taufen an der Pader?", "", 51.71955795852887, 8.751071691513062,
                    new [] { "Kirche" }, new [] { "Dom" }, paderquellen3Image);
            paderquellen3.Pages.Add(CreateAppetizerPage("Donar, Wotan und Saxnot – die Abkehr von den alten Göttern.", paderquellen3Image));  
            karlsrouteSet.ActiveSet.Add(paderquellen3);

            var brueckehausImage = CreateImage("", "Sachsenkriege", "bruecke_teaser.jpg");
            Exhibit brueckeBrauhaus = CreateExhibit("Sachsenkriege", "", 51.719582883396335, 8.751005977392197,
                    new [] { "Kirche" }, new [] { "Dom" }, brueckehausImage);
            brueckeBrauhaus.Pages.Add(CreateAppetizerPage("Karl der Große und die Sachsen – dreißig Jahre Krieg!",
                    brueckehausImage)); 
            karlsrouteSet.ActiveSet.Add(brueckeBrauhaus);

            var geisselscherGartenImage = CreateImage("", "Christianisierung der Sachsen", "garten_teaser.jpg");
            Exhibit geisselscherGarten = CreateExhibit("Christianisierung der Sachsen", "", 51.72050841708062, 8.75171273946762,
                    new [] { "Kirche" }, new [] { "Dom" }, geisselscherGartenImage);
            geisselscherGarten.Pages.Add(CreateAppetizerPage("Karls neue Strategie: Tod oder Taufe?",
                    geisselscherGartenImage));  
            karlsrouteSet.ActiveSet.Add(geisselscherGarten);

            var stadtbilbiothekImage = CreateImage("", "Karls Sieg über die Sachsen", "bibliothek_teaser.jpg");
            Exhibit stadtbibliothek = CreateExhibit("Karls Sieg über die Sachsen", "", 51.718953, 8.75583,
                    new [] { "Kirche" }, new [] { "Dom" }, stadtbilbiothekImage);
            stadtbibliothek.Pages.Add(CreateAppetizerPage("Sachsen wird Teil des Frankenreiches.",
                   stadtbilbiothekImage));
            karlsrouteSet.ActiveSet.Add(stadtbibliothek);

            var domImage = CreateImage("", "Karl der Große im Wandel der Zeit", "dom_teaser.jpg");
            Exhibit dom = CreateExhibit("Karl der Große im Wandel der Zeit", "", 51.7199006, 8.754952000000003,
                    new [] { "Kirche" }, new [] { "Dom" }, domImage);
            dom.Pages.Add(CreateAppetizerPage("Der Blick auf Karl den Großen: Christ, Frankenkönig, Imperator, Heiliger.",
                    domImage));       
            karlsrouteSet.ActiveSet.Add(dom);

            var theoImage = CreateImage("", "Karl der Große macht Schule!", "theo_teaser.jpg");
            Exhibit theodoranium = CreateExhibit("Karl der Große macht Schule!", "", 51.71601, 8.754249999999956,
                    new [] { "Kirche" }, new [] { "Dom" }, theoImage);
            theodoranium.Pages.Add(CreateAppetizerPage("Was hat Karl der Große mit Schule, Schrift und Bildung zu tun?",
                    theoImage));
                 
            karlsrouteSet.ActiveSet.Add(theodoranium);

            karlsrouteSet.ActiveSet.Add(kaiserpfalz);

            var karlsroute = CreateRoute("Karlsroute", "Auf der Spur Karls des Großen!", 30*60, 4.2,
                CreateImage("", "", "theo_teaser.jpg"));
            foreach (var exhibit in karlsrouteSet.ActiveSet)
            {
                karlsroute.Waypoints.Add(CreateWayPoint(exhibit));
            }

            karlsroute.RouteTags.Add(CreateRouteTag("Bar", "bar", CreateImage("", "", "route_tag_bar.png")));
            karlsroute.RouteTags.Add(CreateRouteTag("Restaurant", "restaurant", CreateImage("", "", "route_tag_restaurant.png")));

            // do an empty write, otherwise not all changes to the db are visible in other threads
            Realm.GetInstance().Write(() => { });

            /*LinkedList<Waypoint> waypoints = new LinkedList<>();
            waypoints.add(new Waypoint(51.7189826, 8.754652599999986, 1));
            waypoints.add(new Waypoint(51.71601, 8.754249999999956, 10));
            waypoints.add(new Waypoint(51.716724, 8.752244000000019, 2));
            waypoints.add(new Waypoint(51.71861412677083, 8.75122457742691, 3));
            waypoints.add(new Waypoint(51.718811867802174, 8.751070350408554, 4));
            waypoints.add(new Waypoint(51.71955795852887, 8.751071691513062, 5));
            waypoints.add(new Waypoint(51.719582883396335, 8.751005977392197, 6));
            waypoints.add(new Waypoint(51.72050841708062, 8.75171273946762, 7));
            waypoints.add(new Waypoint(51.7199006, 8.754952000000003, 9));
            waypoints.add(new Waypoint(51.718953, 8.75583, 8));
            waypoints.add(new Waypoint(51.7189826, 8.754652599999986, -1));

            List<RouteTag> ringrouteTags = new LinkedList<>();
            ringrouteTags.add(new RouteTag("bar", "Bar", new Image(101, "", "route_tag_bar", "")));
            ringrouteTags.add(new RouteTag("restaurant", "Restaurant", new Image(101, "", "route_tag_restaurant", "")));

            Route ringroute = new Route(101, "Ringroute", "Dies ist ein einfacher Rundweg rund um den Ring.",
                    waypoints, 60 * 30, 5.2, ringrouteTags, new Image(101, "", "route_ring.jpg", ""));

            insertRoute(ringroute);*/
        }

        private Exhibit CreateExhibit (string name, string description, double latitude, double longitude,
                                       string[] tags, string[] categories, Image image)
        {
            var exhibit = BusinessEntitiyFactory.CreateBusinessObject<Exhibit> ();
            exhibit.Name = name;
            exhibit.Description = description;
            var position = BusinessEntitiyFactory.CreateBusinessObject<GeoLocation> ();
            position.Latitude = latitude;
            position.Longitude = longitude;
            exhibit.Location = position;
            exhibit.Image = image;
            var marker = BusinessEntitiyFactory.CreateBusinessObject<MapMarker> ();
            marker.Title = name;
            marker.Text = description;
            exhibit.Marker = marker;
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

        private Page CreateAppetizerPage (string text, Image img = null)
        {
            var page = BusinessEntitiyFactory.CreateBusinessObject<Page> ();
            var appetizer = BusinessEntitiyFactory.CreateBusinessObject<AppetizerPage> ();
            page.AppetizerPage = appetizer;
            appetizer.Text = text;
            if (img != null)
                appetizer.Image = img;
            return page;
        }

        private Page CreateTextPage (string text, Audio audio = null)
        {
            var page = BusinessEntitiyFactory.CreateBusinessObject<Page> ();
            var textpage = BusinessEntitiyFactory.CreateBusinessObject<TextPage> ();
            page.TextPage = textpage;
            textpage.Text = text;
            if (audio != null)
                page.Audio = audio;
            return page;
        }

        private Page CreateTimeSliderPage (string title, string text, long[] dates, Image[] images, bool hideYearNumbers, Audio audio = null)
        {
            if (dates.Length < 2 || images.Length < 2)
                throw new IllegalArgumentException ("At least two images and dates are necessary.");
            if (dates.Length != images.Length)
                throw new IllegalArgumentException ("There must be equally many dates and images.");

            var page = BusinessEntitiyFactory.CreateBusinessObject<Page> ();
            var tsPage = BusinessEntitiyFactory.CreateBusinessObject<TimeSliderPage> ();
            page.TimeSliderPage = tsPage;
            tsPage.Title = title;
            tsPage.Text = text;
            tsPage.HideYearNumbers = hideYearNumbers;
            page.Audio = audio;
            foreach (var value in dates)
            {
                var longElement = BusinessEntitiyFactory.CreateBusinessObject<LongElement> ();
                longElement.Value = value;
                tsPage.Dates.Add (longElement);
            }
            foreach (var image in images)
            {
                tsPage.Images.Add (image);
            }
            return page;
        }

        private Page CreateImagePage (Image img, Rectangle[] areas, string[] texts, Audio audio = null)
        {
            if (areas?.Length != texts?.Length)
                throw new IllegalArgumentException ("There must be equally many texts and areas.");
            var page = BusinessEntitiyFactory.CreateBusinessObject<Page> ();
            var imagePage = BusinessEntitiyFactory.CreateBusinessObject<ImagePage> ();
            page.ImagePage = imagePage;
            imagePage.Image = img;
            page.Audio = audio;
            if (areas != null)
                foreach (var rectangle in areas)
                {
                    imagePage.Areas.Add (rectangle);
                }
            if (texts != null)
                foreach (var text in texts)
                {
                    var stringElement = BusinessEntitiyFactory.CreateBusinessObject<StringElement> ();
                    stringElement.Value = text;
                    imagePage.Texts.Add (stringElement);
                }
            return page;
        }

        private Rectangle CreateRectangle (int top, int left, int bottom, int right)
        {
            var rect = BusinessEntitiyFactory.CreateBusinessObject<Rectangle> ();
            rect.Top = top;
            rect.Left = left;
            rect.Bottom = bottom;
            rect.Right = right;
            return rect;
        }

        private Image CreateImage (string title, string description, string path)
        {
            var img = BusinessEntitiyFactory.CreateBusinessObject<Image> ();
            img.Title = title;
            img.Description = description;
            img.Data = LoadByteAsset (path);
            return img;
        }

        private Audio CreateAudio (string path, string caption, string title)
        {
            var audio = BusinessEntitiyFactory.CreateBusinessObject<Audio> ();
            audio.Data = LoadByteAsset (path);
            audio.Caption = caption;
            audio.Title = title;
            return audio;
        }

        private Route CreateRoute(string title, string description, int duration, double distance, Image image)
        {
            var route = BusinessEntitiyFactory.CreateBusinessObject<Route>();
            route.Title = title;
            route.Description = description;
            route.Duration = duration;
            route.Distance = distance;
            route.Image = image;
            
            return route;
        }

        private Waypoint CreateWayPoint(Exhibit exhibit)
        {
            var waypoint = BusinessEntitiyFactory.CreateBusinessObject<Waypoint>();
            waypoint.Exhibit = exhibit;
            waypoint.Location = exhibit.Location;
            return waypoint;
        }

        private RouteTag CreateRouteTag(string name, string tag, Image image)
        {
            var routetag = BusinessEntitiyFactory.CreateBusinessObject<RouteTag>();
            routetag.Name = name;
            routetag.Tag = tag;
            routetag.Image = image;
            return routetag;
        }

        private byte[] LoadByteAsset (string name)
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