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
using System.Linq;
using System.Text.RegularExpressions;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.Managers;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.Models;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.ServiceAccessLayer.ContentApiDtos;

namespace PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.DtoToModelConverters
{
    public class PageConverter : DtoToModelConverter<Page, PageDto>
    {
        public override void Convert(PageDto dto, Page existingModelObject)
        {
            existingModelObject.IdForRestApi = dto.Id;
            existingModelObject.Timestamp = dto.Timestamp;
            switch (dto.Type)
            {
                case PageTypeDto.AppetizerPage:
                    if (existingModelObject.AppetizerPage == null)
                    {
                        existingModelObject.AppetizerPage = DbManager.CreateBusinessObject<AppetizerPage>();
                    }
                    // Necessary for iOS
                    if (dto.Text != null)
                    {
                        existingModelObject.AppetizerPage.Text = Regex.Unescape(dto.Text);
                    }
                    else
                    {
                        existingModelObject.AppetizerPage.Text = dto.Text;
                    }

                    break;
                case PageTypeDto.ImagePage:
                    if (existingModelObject.ImagePage == null)
                    {
                        existingModelObject.ImagePage = DbManager.CreateBusinessObject<ImagePage>();
                    }
                    break;
                case PageTypeDto.SliderPage:
                    if (existingModelObject.TimeSliderPage == null)
                    {
                        existingModelObject.TimeSliderPage = DbManager.CreateBusinessObject<TimeSliderPage>();
                    }
                    existingModelObject.TimeSliderPage.HideYearNumbers = dto.HideYearNumbers;
                    existingModelObject.TimeSliderPage.Title = dto.Title;
                    // Necessary for iOS
                    if (dto.Text != null)
                    {
                        existingModelObject.TimeSliderPage.Text = Regex.Unescape(dto.Text);
                    }
                    else
                    {
                        existingModelObject.TimeSliderPage.Text = dto.Text;
                    }

                    foreach (var image in dto.Images)
                    {
                        var longElement = DbManager.CreateBusinessObject<LongElement>();
                        longElement.Value = image.Date;
                        existingModelObject.TimeSliderPage.Dates.Add(longElement);
                    }
                    break;
                case PageTypeDto.TextPage:
                    if (existingModelObject.TextPage == null)
                    {
                        existingModelObject.TextPage = DbManager.CreateBusinessObject<TextPage>();
                    }
                    existingModelObject.TextPage.Title = dto.Title;
                    // Necessary for iOS
                    if (dto.Text != null)
                    {
                        existingModelObject.TextPage.Text = Regex.Unescape(dto.Text);
                    }
                    else
                    {
                        existingModelObject.TextPage.Text = dto.Text;
                    }
                    // Necessary for iOS
                    if (dto.Description != null)
                    {
                        existingModelObject.TextPage.Description = Regex.Unescape(dto.Description);
                    }
                    else
                    {
                        existingModelObject.TextPage.Description = dto.Description;
                    }
                    existingModelObject.TextPage.FontFamily = dto.FontFamily;
                    break;
            }
        }
    }
}