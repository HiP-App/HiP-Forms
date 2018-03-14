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

using PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.Models;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.ServiceAccessLayer.ContentApiDtos;
using System;
using System.Text.RegularExpressions;

namespace PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.DtoToModelConverters
{
    public class PageConverter : DtoToModelConverter<Page, PageDto>
    {
        protected override Page CreateModelInstance(PageDto dto)
        {
            switch (dto.Type)
            {
                case PageTypeDto.TextPage: return new TextPage();
                case PageTypeDto.ImagePage: return new ImagePage();
                case PageTypeDto.SliderPage: return new TimeSliderPage();
                default: throw new NotImplementedException();
            }
        }

        public override void Convert(PageDto dto, Page existingModelObject)
        {
            existingModelObject.IdForRestApi = dto.Id;
            existingModelObject.Timestamp = dto.Timestamp;

            switch (existingModelObject)
            {
                case ImagePage imagePage:
                    // Nothing to do
                    break;

                case TimeSliderPage timeSliderPage:
                    timeSliderPage.HideYearNumbers = dto.HideYearNumbers;
                    timeSliderPage.Title = dto.Title;
                    timeSliderPage.Text = (dto.Text != null) ? Regex.Unescape(dto.Text) : null; // Necessary for iOS
                    timeSliderPage.SliderImages.Clear();

                    foreach (var entry in dto.Images)
                    {
                        timeSliderPage.SliderImages.Add(new TimeSliderPageImage
                        {
                            Page = timeSliderPage,
                            Date = entry.Date
                        });
                    }
                    break;

                case TextPage textPage:
                    textPage.Title = dto.Title;
                    textPage.Text = (dto.Text != null) ? Regex.Unescape(dto.Text) : null; // Necessary for iOS
                    textPage.Description = (dto.Description != null) ? Regex.Unescape(dto.Description) : null; // Necessary for iOS
                    textPage.FontFamily = dto.FontFamily;
                    break;
            }
        }
    }
}