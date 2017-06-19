﻿// Copyright (C) 2017 History in Paderborn App - Universität Paderborn
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

using PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.Managers;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.Models;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.ServiceAccessLayer.ContentApiDtos;

namespace PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.DtoToModelConverters {
    public class ExhibitConverter : DtoToModelConverter<Exhibit, ExhibitDto> {

        public override void Convert (ExhibitDto dto, Exhibit existingModelObject)
        {
            existingModelObject.IdForRestApi = dto.Id;
            existingModelObject.Timestamp = dto.Timestamp;

            existingModelObject.Name = dto.Name;
            existingModelObject.Description = dto.Description;
            if (existingModelObject.Location == null)
            {
                existingModelObject.Location = DbManager.CreateBusinessObject<GeoLocation>();
            }

            existingModelObject.Location.Latitude = dto.Latitude;
            existingModelObject.Location.Longitude = dto.Longitude;
        }

    }
}