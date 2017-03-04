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

using System;
using de.upb.hip.mobile.pcl.BusinessLayer.Managers;
using de.upb.hip.mobile.pcl.BusinessLayer.Models;
using de.upb.hip.mobile.pcl.Common.Contracts;

namespace de.upb.hip.mobile.pcl.Common
{
    public partial class DbDummyDataFiller
    {

        private readonly IDataLoader dataLoader = IoCManager.Resolve<IDataLoader>();

        private Exhibit CreateExhibit(string name, string description, double latitude, double longitude,
                                       string[] tags, string[] categories, Image image)
        {
            var exhibit = DbManager.CreateBusinessObject<Exhibit>();
            exhibit.Name = name;
            exhibit.Description = description;
            var position = DbManager.CreateBusinessObject<GeoLocation>();
            position.Latitude = latitude;
            position.Longitude = longitude;
            exhibit.Location = position;
            exhibit.Image = image;
            var marker = DbManager.CreateBusinessObject<MapMarker>();
            marker.Title = name;
            marker.Text = description;
            exhibit.Marker = marker;
            foreach (var tag in tags)
            {
                var stringelement = DbManager.CreateBusinessObject<StringElement>();
                stringelement.Value = tag;
                exhibit.Tags.Add(stringelement);
            }
            foreach (var category in categories)
            {
                var stringelement = DbManager.CreateBusinessObject<StringElement>();
                stringelement.Value = category;
                exhibit.Categories.Add(stringelement);
            }
            return exhibit;
        }

        private Page CreateAppetizerPage(string text, Image img = null)
        {
            var page = DbManager.CreateBusinessObject<Page>();
            var appetizer = DbManager.CreateBusinessObject<AppetizerPage>();
            page.AppetizerPage = appetizer;
            appetizer.Text = text;
            if (img != null)
                appetizer.Image = img;
            return page;
        }

        // ReSharper disable once UnusedMember.Local
        private Page CreateTextPage(string text, string title, string description, string fontFamilyName = null, Audio audio = null)
        {
            var page = DbManager.CreateBusinessObject<Page>();
            var textpage = DbManager.CreateBusinessObject<TextPage>();
            page.TextPage = textpage;
            textpage.Text = text;
            textpage.FontFamily = fontFamilyName;
            textpage.Title = title;
            textpage.Description = description;

            if (audio != null)
                page.Audio = audio;
            return page;
        }

        private Page CreateTimeSliderPage(string title, string text, long[] dates, Image[] images, bool hideYearNumbers, Audio audio = null)
        {
            if (dates.Length < 2 || images.Length < 2)
                throw new ArgumentException("At least two images and dates are necessary.");
            if (dates.Length != images.Length)
                throw new ArgumentException("There must be equally many dates and images.");

            var page = DbManager.CreateBusinessObject<Page>();
            var tsPage = DbManager.CreateBusinessObject<TimeSliderPage>();
            page.TimeSliderPage = tsPage;
            tsPage.Title = title;
            tsPage.Text = text;
            tsPage.HideYearNumbers = hideYearNumbers;
            page.Audio = audio;
            foreach (var value in dates)
            {
                var longElement = DbManager.CreateBusinessObject<LongElement>();
                longElement.Value = value;
                tsPage.Dates.Add(longElement);
            }
            foreach (var image in images)
            {
                tsPage.Images.Add(image);
            }
            return page;
        }

        private Page CreateImagePage(Image img, Rectangle[] areas, string[] texts, Audio audio = null)
        {
            if (areas?.Length != texts?.Length)
                throw new ArgumentException("There must be equally many texts and areas.");
            var page = DbManager.CreateBusinessObject<Page>();
            var imagePage = DbManager.CreateBusinessObject<ImagePage>();
            page.ImagePage = imagePage;
            imagePage.Image = img;
            page.Audio = audio;
            if (areas != null)
                foreach (var rectangle in areas)
                {
                    imagePage.Areas.Add(rectangle);
                }
            if (texts != null)
                foreach (var text in texts)
                {
                    var stringElement = DbManager.CreateBusinessObject<StringElement>();
                    stringElement.Value = text;
                    imagePage.Texts.Add(stringElement);
                }
            return page;
        }

        // ReSharper disable once UnusedMember.Local
        private Rectangle CreateRectangle(int top, int left, int bottom, int right)
        {
            var rect = DbManager.CreateBusinessObject<Rectangle>();
            rect.Top = top;
            rect.Left = left;
            rect.Bottom = bottom;
            rect.Right = right;
            return rect;
        }

        private Image CreateImage(string title, string description, string path)
        {
            var img = DbManager.CreateBusinessObject<Image>();
            img.Title = title;
            img.Description = description;
            img.Data = dataLoader.LoadByteData(path);
            return img;
        }

        private Audio CreateAudio(string path, string caption, string title)
        {
            var audio = DbManager.CreateBusinessObject<Audio>();
            audio.Data = dataLoader.LoadByteData(path);
            audio.Caption = caption;
            audio.Title = title;
            return audio;
        }

        private Route CreateRoute(string title, string description, int duration, double distance, Image image)
        {
            var route = DbManager.CreateBusinessObject<Route>();
            route.Title = title;
            route.Description = description;
            route.Duration = duration;
            route.Distance = distance;
            route.Image = image;

            return route;
        }

        private Waypoint CreateWayPoint(Exhibit exhibit)
        {
            var waypoint = DbManager.CreateBusinessObject<Waypoint>();
            waypoint.Exhibit = exhibit;
            waypoint.Location = exhibit.Location;
            return waypoint;
        }

        private RouteTag CreateRouteTag(string name, string tag, Image image)
        {
            var routetag = DbManager.CreateBusinessObject<RouteTag>();
            routetag.Name = name;
            routetag.Tag = tag;
            routetag.Image = image;
            return routetag;
        }

    }
}