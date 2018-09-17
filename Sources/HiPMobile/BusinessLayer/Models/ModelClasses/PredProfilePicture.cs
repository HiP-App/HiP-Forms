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

using System.IO;
using Xamarin.Forms;

namespace PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.Models
{
    public class PredProfilePicture : IPredProfilePicture
    {
        
        public PredProfilePicture(string id, byte[] data)
        {
            Id = id;
            ImageFullBytes = data;
        }

        public string Id { get; }

        //public string ImageFullString { get; }

       // public string ImageSmallString { get; }

        public ImageSource ImageFull { get; set; }

        private byte[] imageFullBytes;
        public byte[] ImageFullBytes
        {
            get
            {
                return imageFullBytes;
            }
            set
            {
                imageFullBytes = value;
                ImageFull = ImageSource.FromStream(() => new MemoryStream(imageFullBytes));
            }
        }

        public ImageSource ImageSmall { get; set; }

        private byte[] imageSmallBytes;
        public byte[] ImageSmallBytes
        {
            get
            {
                return imageSmallBytes;
            }
            set
            {
                imageSmallBytes = value;
                ImageSmall = ImageSource.FromStream(() => new MemoryStream(imageSmallBytes));
            }
        }
    }
}