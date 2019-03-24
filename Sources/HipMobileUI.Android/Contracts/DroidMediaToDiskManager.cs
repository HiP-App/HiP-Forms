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
using Android.Content;
using Android.Content.Res;
using PaderbornUniversity.SILab.Hip.Mobile.Droid.Contracts;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.Managers;
using Xamarin.Forms;
using Environment = System.Environment;

[assembly: Dependency(typeof(DroidMediaToDiskManager))]

namespace PaderbornUniversity.SILab.Hip.Mobile.Droid.Contracts
{
    public class DroidMediaToDiskManager : IMediaToDiskManager
    {
        public Task<Stream> WriteMediaToDisk()
        {

            var documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            var mediaFolderPath = Path.Combine(documentsPath, "Media");
            if (!Directory.Exists(mediaFolderPath))
            {
                Directory.CreateDirectory(mediaFolderPath);
            }
          
            var filePath = mediaFolderPath;
            var resId = 0;
            var negI = 0;
            FileStream writeStream;

            for (int i = -2; i <= 155; i++)
            {
                filePath = Path.Combine(mediaFolderPath, i.ToString());

                if (!File.Exists(filePath))
                {
                    writeStream = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.Write);

                    resId = 0;

                    if (i < 0)
                    {
                        negI = i * -1;
                        resId = MainActivity.Instance.Resources.GetIdentifier("media_m" + negI.ToString(), "raw", MainActivity.Instance.PackageName);
                    }
                    else
                    {

                        resId = MainActivity.Instance.Resources.GetIdentifier("media_" + i.ToString(), "raw", MainActivity.Instance.PackageName);
                    }

                    if (resId != 0)
                    {
                        var mediaFile = MainActivity.Instance.Resources.OpenRawResource(resId);
                        ReadWriteStream(mediaFile, writeStream);
                    }
                }       
            }
            
            return null;
        }

        private void ReadWriteStream(Stream readStream, Stream writeStream)
        {
            int length = 256;
            byte[] buffer = new byte[length];
            int bytesRead = readStream.Read(buffer, 0, length);
            while (bytesRead > 0)
            {
                writeStream.Write(buffer, 0, bytesRead);
                bytesRead = readStream.Read(buffer, 0, length);
            }
            readStream.Close();
            writeStream.Close();
        }
    }
}