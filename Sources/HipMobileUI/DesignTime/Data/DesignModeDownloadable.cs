// Copyright (C) 2017 History in Paderborn App - Universität Paderborn
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//       http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.Models;

namespace PaderbornUniversity.SILab.Hip.Mobile.UI.DesignTime.Data
{
    public class DesignModeDownloadable : IDownloadable
    {
        public static readonly DesignModeDownloadable SampleInstance = new DesignModeDownloadable
        {
            Id = "SampleDownloadable",
            IdForRestApi = 17,
            Name = "Sample Downloadable",
            Description = "A sample downloadable thingy that can be downloaded with a downloader",
            Type = DownloadableType.Exhibit,
            Image = null /*new Shared.BusinessLayer.Models.Image
            {
                Id = "SampleImage",
                IdForRestApi = 42,
                Title = "Sample Image",
                Description = "A sample image/picture/photo/drawing",
                DataPath = "/some/path/to/the/image.png",
                Timestamp = DateTimeOffset.Now
            }*/
        };

        public string Id { get; set; }
        public int IdForRestApi { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public Shared.BusinessLayer.Models.Image Image { get; set; }
        public DownloadableType Type { get; set; }
    }
}
