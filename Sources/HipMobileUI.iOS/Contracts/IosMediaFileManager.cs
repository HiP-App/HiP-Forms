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
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.Common.Contracts;

namespace PaderbornUniversity.SILab.Hip.Mobile.Ios.Contracts
{
    public class IosMediaFileManager: IMediaFileManager
    {
        private const string MediaFolder = "Media";

        public void DeleteFile(string filePath)
        {
            File.Delete(filePath);
        }

        public async Task<(string md5, string filePath)> WriteMediaToDiskAsync(byte[] bytes)
        {
            string md5Hash;
            using (var md5 = MD5.Create())
            {
                md5Hash = md5
                    .ComputeHash(bytes)
                    .Select(b => b.ToString("x2"))
                    .Aggregate(new StringBuilder(), (cur, next) => cur.Append(next))
                    .ToString();
            }

            var documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            var filePath = Path.Combine(Path.Combine(documentsPath, MediaFolder), md5Hash);
            using (var fs = new FileStream(filePath, FileMode.Create))
            {
                await fs.WriteAsync(bytes, 0, bytes.Length);
            }

            return (md5Hash, filePath);
        }
        
        public async Task<byte[]> ReadFromDiskAsync(string filePath)
        {
            using (var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read, 
                                           bufferSize: 4096, useAsync: true))
            {
                var bytes = new byte[fs.Length];
                var totalRead = 0;
                int read;
                while ((read = await fs.ReadAsync(bytes, totalRead, 4096)) != 0)
                {
                    totalRead += read;
                }
                return bytes;
            }
        }
    }
}