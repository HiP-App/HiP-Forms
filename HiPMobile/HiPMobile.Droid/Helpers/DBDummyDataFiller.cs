using System.IO;
using Android.Content.Res;
using de.upb.hip.mobile.pcl.BusinessLayer.Models;

namespace de.upb.hip.mobile.droid.Helpers
{
    public class DbDummyDataFiller
    {
        private AssetManager manager;

        public DbDummyDataFiller(AssetManager manager)
        {
            this.manager = manager;
        }

        public void InsertData()
        {
            var dom = CreateExhibit("Paderborner Dom",
                "Der Hohe Dom Ss. Maria, Liborius und Kilian ist die Kathedralkirche des Erzbistums Paderborn und liegt im Zentrum der Paderborner Innenstadt, oberhalb der Paderquellen.",
                51.718953, 8.75583, "dom.jpg", new[] {"Kirche"}, new[] {"Dom"});
            var uni = CreateExhibit("Universität Paderborn",
                "Die Universität Paderborn in Paderborn, Deutschland, ist eine 1972 gegründete Universität in Nordrhein-Westfalen.",
                51.706768, 8.771104, "uni.jpg", new[] {"Uni"}, new[] {"Universität"});
            var exhibits = BusinessEntitiyFactory.CreateBusinessObject<ExhibitSet>();
            exhibits.InitSet.Add(dom);
            exhibits.InitSet.Add(uni);
        }

        private Exhibit CreateExhibit(string name, string description, double latitude, double longitude,
            string imagePath, string[] tags, string[] categories)
        {
            var exhibit = BusinessEntitiyFactory.CreateBusinessObject<Exhibit>();
            exhibit.Name = name;
            exhibit.Description = description;
            var position = BusinessEntitiyFactory.CreateBusinessObject<GeoLocation>();
            position.Latitude = latitude;
            position.Longitude = longitude;
            exhibit.Location = position;
            var image = BusinessEntitiyFactory.CreateBusinessObject<Image>();
            image.Data = LoadImageAsset(imagePath);
            exhibit.Image = image;
            foreach (string tag in tags)
            {
                var stringelement = BusinessEntitiyFactory.CreateBusinessObject<StringElement>();
                stringelement.Value = tag;
                exhibit.Tags.Add(stringelement);
            }
            foreach (string category in categories)
            {
                var stringelement = BusinessEntitiyFactory.CreateBusinessObject<StringElement>();
                stringelement.Value = category;
                exhibit.Categories.Add(stringelement);
            }
            return exhibit;
        }

        private byte[] LoadImageAsset(string name)
        {
            byte[] buffer = new byte[16 * 1024];
            byte[] data;
            using (Stream input = manager.Open(name))
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    int read;
                    while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        ms.Write(buffer, 0, read);
                    }
                    data = ms.ToArray();
                }
            }
            return data;
        }
    }
}