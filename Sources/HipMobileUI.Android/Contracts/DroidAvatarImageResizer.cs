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

using System.IO;
using System.Threading.Tasks;
using Android.Graphics;
using PaderbornUniversity.SILab.Hip.Mobile.Droid.Contracts;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.UserManagement;
using Xamarin.Forms;

[assembly: Dependency(typeof(DroidAvatarImageResizer))]

namespace PaderbornUniversity.SILab.Hip.Mobile.Droid.Contracts
{
    public class DroidAvatarImageResizer : IAvatarImageResizer
    {
        private const int Quality = 10;

        public byte[] ResizeAvatar(byte[] avatar, float width, float height)
        {
            Bitmap avatarOriginal = BitmapFactory.DecodeByteArray(avatar, 0, avatar.Length);
            Bitmap avatarNew = Bitmap.CreateScaledBitmap(avatarOriginal, (int)width, (int)height, false);

            var stream = new MemoryStream();
            avatarNew.Compress(Bitmap.CompressFormat.Png, Quality, stream);
            var avatarNewBytes = stream.ToArray();
            return avatarNewBytes;
        }
    }
}