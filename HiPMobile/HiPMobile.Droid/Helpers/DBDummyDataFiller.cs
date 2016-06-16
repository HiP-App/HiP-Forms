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
using Java.Lang;

namespace de.upb.hip.mobile.droid.Helpers {
    public class DbDummyDataFiller {

        private readonly AssetManager manager;

        private readonly string lorem =
            "Lorem ipsum dolor sit amet, consetetur sadipscing elitr, sed diam nonumy eirmod tempor invidunt ut labore et dolore magna aliquyam erat, sed diam voluptua. At vero eos et accusam et justo duo dolores et ea rebum. Stet clita kasd gubergren, no sea takimata sanctus est Lorem ipsum dolor sit amet. Lorem ipsum dolor sit amet, consetetur sadipscing elitr, sed diam nonumy eirmod tempor invidunt ut labore et dolore magna aliquyam erat, sed diam voluptua. At vero eos et accusam et justo duo dolores et ea rebum. Stet clita kasd gubergren, no sea takimata sanctus est Lorem ipsum dolor sit amet. Lorem ipsum dolor sit amet, consetetur sadipscing elitr, sed diam nonumy eirmod tempor invidunt ut labore et dolore magna aliquyam erat, sed diam voluptua. At vero eos et accusam et justo duo dolores et ea rebum. Stet clita kasd gubergren, no sea takimata sanctus est Lorem ipsum dolor sit amet." +
            "Duis autem vel eum iriure dolor in hendrerit in vulputate velit esse molestie consequat, vel illum dolore eu feugiat nulla facilisis at vero eros et accumsan et iusto odio dignissim qui blandit praesent luptatum zzril delenit augue duis dolore te feugait nulla facilisi.Lorem ipsum dolor sit amet, consectetuer adipiscing elit, sed diam nonummy nibh euismod tincidunt ut laoreet dolore magna aliquam erat volutpat." +
            "Ut wisi enim ad minim veniam, quis nostrud exerci tation ullamcorper suscipit lobortis nisl ut aliquip ex ea commodo consequat.Duis autem vel eum iriure dolor in hendrerit in vulputate velit esse molestie consequat, vel illum dolore eu feugiat nulla facilisis at vero eros et accumsan et iusto odio dignissim qui blandit praesent luptatum zzril delenit augue duis dolore te feugait nulla facilisi." +
            "Nam liber tempor cum soluta nobis eleifend option congue nihil imperdiet doming id quod mazim placerat facer possim assum.Lorem ipsum dolor sit amet, consectetuer adipiscing elit, sed diam nonummy nibh euismod tincidunt ut laoreet dolore magna aliquam erat volutpat.Ut wisi enim ad minim veniam, quis nostrud exerci tation ullamcorper suscipit lobortis nisl ut aliquip ex ea commodo consequat." +
            "Duis autem vel eum iriure dolor in hendrerit in vulputate velit esse molestie consequat, vel illum dolore eu feugiat nulla facilisis." +
            "At vero eos et accusam et justo duo dolores et ea rebum.Stet clita kasd gubergren, no sea takimata sanctus est Lorem ipsum dolor sit amet. Lorem ipsum dolor sit amet, consetetur sadipscing elitr, sed diam nonumy eirmod tempor invidunt ut labore et dolore magna aliquyam erat, sed diam voluptua.At vero eos et accusam et justo duo dolores et ea rebum. Stet clita kasd gubergren, no sea takimata sanctus est Lorem ipsum dolor sit amet. Lorem ipsum dolor sit amet, consetetur sadipscing elitr, At accusam aliquyam diam diam dolore dolores duo eirmod eos erat, et nonumy sed tempor et et invidunt justo labore Stet clita ea et gubergren, kasd magna no rebum. sanctus sea sed takimata ut vero voluptua.est Lorem ipsum dolor sit amet. Lorem ipsum dolor sit amet, consetetur sadipscing elitr, sed diam nonumy eirmod tempor invidunt ut labore et dolore magna aliquyam erat." +
            "Consetetur sadipscing elitr, sed diam nonumy eirmod tempor invidunt ut labore et dolore magna aliquyam erat, sed diam voluptua.At vero eos et accusam et justo duo dolores et ea rebum. Stet clita kasd gubergren, no sea takimata sanctus est Lorem ipsum dolor sit amet. Lorem ipsum dolor sit amet, consetetur sadipscing elitr, sed diam nonumy eirmod tempor invidunt ut labore et dolore magna aliquyam erat, sed diam voluptua.At vero eos et accusam et justo duo dolores et ea rebum. Stet clita";

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
            exhibits.ActiveSet.Add(dom);
            exhibits.ActiveSet.Add(uni);

            // create dummy pages
            Image domImg = CreateImage("Paderborner Dom",
                "Der Hohe Dom Ss. Maria, Liborius und Kilian ist die Kathedralkirche des Erzbistums Paderborn und liegt im Zentrum der Paderborner Innenstadt, oberhalb der Paderquellen.",
                "dom.jpg");
            Image uniImg =
            CreateImage("Universität Paderborn",
                "Die Universität Paderborn in Paderborn, Deutschland, ist eine 1972 gegründete Universität in Nordrhein-Westfalen.",
                "uni.jpg");
            dom.Pages.Add(CreateAppetizerPage("Der Dom von außen", domImg));
            dom.Pages.Add(CreateImagePage(domImg, new []{CreateRectangle(100, 80, 250, 180)}, new []{"Der 93 Meter hohe Kirchturm."}));
            dom.Pages.Add(CreateTextPage(lorem));
            dom.Pages.Add(CreateTimeSliderPage("Timeslider", "Ein Test für die Timesliderpage mit zwei Bildern", new []{1000L, 1973}, new []{domImg, uniImg}));
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

        private Page CreateAppetizerPage(string text, Image img=null)
        {
            var page = BusinessEntitiyFactory.CreateBusinessObject<Page>();
            var appetizer = BusinessEntitiyFactory.CreateBusinessObject<AppetizerPage>();
            page.AppetizerPage = appetizer;
            appetizer.Text = text;
            if (img != null) appetizer.Image = img;
            return page;
        }

        private Page CreateTextPage(string text, Audio audio=null)
        {
            var page = BusinessEntitiyFactory.CreateBusinessObject<Page>();
            var textpage = BusinessEntitiyFactory.CreateBusinessObject<TextPage>();
            page.TextPage = textpage;
            textpage.Text = text;
            if (audio != null) textpage.Audio = audio;
            return page;
        }

        private Page CreateTimeSliderPage(string title, string text, long[] dates, Image[] images)
        {
            if(dates.Length<2 || images.Length<2) throw new IllegalArgumentException("At least two images and dates are necessary.");
            if(dates.Length != images.Length) throw new IllegalArgumentException("There must be equally many dates and images.");

            var page = BusinessEntitiyFactory.CreateBusinessObject<Page>();
            var tsPage = BusinessEntitiyFactory.CreateBusinessObject<TimeSliderPage>();
            page.TimeSliderPage = tsPage;
            tsPage.Title = title;
            tsPage.Text = text;
            foreach (var value in dates)
            {
                var longElement = BusinessEntitiyFactory.CreateBusinessObject<LongElement>();
                longElement.Value = value;
                tsPage.Dates.Add(longElement);
            }
            foreach (var image in images)
            {
                tsPage.Images.Add(image);
            }
            return page;
        }

        private Page CreateImagePage(Image img, Rectangle[] areas, string[] texts)
        {
            if(areas.Length != texts.Length) throw new IllegalArgumentException("There must be equally many texts and areas.");
            var page = BusinessEntitiyFactory.CreateBusinessObject<Page>();
            var imagePage = BusinessEntitiyFactory.CreateBusinessObject<ImagePage>();
            page.ImagePage = imagePage;
            imagePage.Image = img;
            foreach (var rectangle in areas)
            {
                imagePage.Areas.Add(rectangle);
            }
            foreach (string text in texts)
            {
                var stringElement = BusinessEntitiyFactory.CreateBusinessObject<StringElement>();
                stringElement.Value = text;
                imagePage.Texts.Add(stringElement);
            }
            return page;
        }

        private Rectangle CreateRectangle(int top, int left, int bottom, int right)
        {
            var rect = BusinessEntitiyFactory.CreateBusinessObject<Rectangle>();
            rect.Top = top;
            rect.Left = left;
            rect.Bottom = bottom;
            rect.Right = right;
            return rect;
        }

        private Image CreateImage(string title, string description, string path)
        {
            var img = BusinessEntitiyFactory.CreateBusinessObject<Image>();
            img.Title = title;
            img.Description = description;
            img.Data = LoadImageAsset(path);
            return img;
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