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

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Practices.Unity;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.ContentApiFetchers.Contracts;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.DtoToModelConverters;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.Managers;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.Models;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.Common;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.ServiceAccessLayer.ContentApiAccesses.Contracts;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.ServiceAccessLayer.ContentApiDtos;

namespace PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.ContentApiFetchers {
    public class BaseDataFetcher : IBaseDataFetcher {

        private readonly IExhibitsApiAccess exhibitsApiAccess;
        private readonly IMediasApiAccess mediasApiAccess;
        private readonly IFileApiAccess fileApiAccess;

        [Dependency]
        public ExhibitConverter ExhibitConverter { private get; set; }

        [Dependency]
        public MediaToImageConverter ImageConverter { private get; set; }


        private IList<ExhibitDto> fetchedChangedExhibits;

        public BaseDataFetcher (IExhibitsApiAccess exhibitsApiAccess, IMediasApiAccess mediasApiAccess, IFileApiAccess fileApiAccess)
        {
            this.exhibitsApiAccess = exhibitsApiAccess;
            this.mediasApiAccess = mediasApiAccess;
            this.fileApiAccess = fileApiAccess;
        }

        public async Task<bool> IsDatabaseUpToDate ()
        {
            var exhibitSet = ExhibitManager.GetExhibitSets ().SingleOrDefault ();

            fetchedChangedExhibits = await FetchExhibits (exhibitSet);

            return fetchedChangedExhibits.Any ();
        }

        public async Task FetchBaseDataIntoDatabase ()
        {
            var exhibitSet = ExhibitManager.GetExhibitSets ().SingleOrDefault ();

            if (fetchedChangedExhibits == null)
            {
                fetchedChangedExhibits = await FetchExhibits (exhibitSet);
            }

            if (exhibitSet == null)
            {
                using (DbManager.StartTransaction ())
                {
                    exhibitSet = DbManager.CreateBusinessObject<ExhibitSet> ();
                }
            }

            var newExhibits = new List<ExhibitDto> ();
            var updatedExhibits = new Dictionary<ExhibitDto, Exhibit> ();
            var requiredAppetizerPages = new List<int> ();
            var requiredImages = new List<int> ();

            foreach (var exhibit in fetchedChangedExhibits)
            {
                var dbExhibit = exhibitSet.SingleOrDefault (x => x.IdForRestApi == exhibit.Id);

                if (dbExhibit != null && exhibit.Timestamp != dbExhibit.UnixTimestamp)
                {
                    updatedExhibits.Add (exhibit, dbExhibit);
                    if (exhibit.Pages.Any ())
                    {
                        requiredAppetizerPages.Add (exhibit.Pages.First ());
                    }
                    requiredImages.Add (exhibit.Image);
                }
                else
                {
                    newExhibits.Add (exhibit);
                    if (exhibit.Pages.Any ())
                    {
                        requiredAppetizerPages.Add (exhibit.Pages.First ());
                    }
                    requiredImages.Add (exhibit.Image);
                }
            }

            //TODO: Fetch all appetizerpages for updated and new exhibits
            //TODO: Add appetizerpages images to required images

            var images = await FetchImages (requiredImages);
            var files = await FetchFiles (requiredImages);

            using (DbManager.StartTransaction ())
            {
                ProcessUpdatedExhibits (updatedExhibits, images, files);
                ProcessNewExhibits (newExhibits, images, files);
            }
        }

        private void ProcessUpdatedExhibits (Dictionary<ExhibitDto, Exhibit> updatedExhibits, IList<MediaDto> images, IList<FileDto> files)
        {
            foreach (var exhibitPair in updatedExhibits)
            {
                var exhibitDto = exhibitPair.Key;
                var dbExhibit = exhibitPair.Value;

                ExhibitConverter.Convert (exhibitDto, dbExhibit);

                AddImageToExhibit (dbExhibit, exhibitDto.Image, images, files);

                //TODO: If exhibits content was already downloaded 
                //-> Show dialog whether to download new data or do it directly depending on setting
            }
        }

        private void ProcessNewExhibits (IEnumerable<ExhibitDto> newExhibits, IList<MediaDto> images, IList<FileDto> files)
        {
            foreach (var exhibitDto in newExhibits)
            {
                var dbExhibit = ExhibitConverter.Convert (exhibitDto);

                AddImageToExhibit (dbExhibit, exhibitDto.Image, images, files);
            }
        }

        private void AddImageToExhibit (Exhibit exhibit, int mediaId, IList<MediaDto> images, IList<FileDto> files)
        {
            var exhibitImage = images.SingleOrDefault (x => x.Id == mediaId);
            if (exhibitImage != null)
            {
                exhibit.Image = ImageConverter.Convert (exhibitImage);

                var imageFile = files.SingleOrDefault (x => x.MediaId == mediaId);
                if (imageFile != null)
                {
                    exhibit.Image.Data = imageFile.Data;
                }
            }
        }

        private async Task<IList<ExhibitDto>> FetchExhibits (ExhibitSet exhibitSet)
        {
            ExhibitsDto exhibits;
            if (exhibitSet != null)
            {
                exhibits = await exhibitsApiAccess.GetExhibits (exhibitSet.Timestamp);
            }
            else
            {
                exhibits = await exhibitsApiAccess.GetExhibits ();
            }

            return exhibits.Items;
        }

        private async Task<IList<MediaDto>> FetchImages (IList<int> requiredImages)
        {
            var medias = await mediasApiAccess.GetMedias (requiredImages);
            return medias.Items;
        }

        private async Task<IList<FileDto>> FetchFiles (IList<int> requiredImages)
        {
            var files = new List<FileDto> ();
            foreach (int mediaId in requiredImages)
            {
                var file = await fileApiAccess.GetFile (mediaId);
                files.Add (file);
            }

            return files;
        }

    }
}