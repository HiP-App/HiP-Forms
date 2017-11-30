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
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.Common.Contracts;

namespace PaderbornUniversity.SILab.Hip.Mobile.Ios.Contracts
{
    public class IosMediaFileManager : IMediaFileManager
    {
        private static string MediaFolderPath
        {
            get
            {
                var documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                var mediaFolderPath = Path.Combine(documentsPath, "Media");
                Directory.CreateDirectory(mediaFolderPath);
                return mediaFolderPath;
            }
        }

        private const string RestApiTimestampPathSuffix = ".timestamp";

        public void DeleteFile(string filePath)
        {
            File.Delete(filePath + RestApiTimestampPathSuffix);
            File.Delete(filePath);
        }

        public async Task<string> WriteMediaToDiskAsync(byte[] bytes, int restApiId, DateTimeOffset timestamp)
        {
            Debug.WriteLine($"Writing file with id {restApiId}");
            var filePath = Path.Combine(MediaFolderPath, restApiId.ToString());
            var restIdPath = filePath + RestApiTimestampPathSuffix;
            using (var fs = new FileStream(filePath, FileMode.Create))
            {
                await fs.WriteAsync(bytes, 0, bytes.Length);
            }
            using (var fs = new FileStream(restIdPath, FileMode.Create))
            {
                var buffer = Encoding.UTF8.GetBytes(timestamp.ToString("o", CultureInfo.InvariantCulture)); // ISO 8601
                await fs.WriteAsync(buffer, 0, buffer.Length);
            }

            return filePath;
        }

        public byte[] ReadFromDisk(string filePath) => File.ReadAllBytes(filePath);

        public async Task<byte[]> ReadFromDiskAsync(string filePath)
        {
            using (var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read,
                                           bufferSize: 4096, useAsync: true))
            {
                using (var ms = new MemoryStream())
                {
                    await fs.CopyToAsync(ms);
                    return ms.ToArray();
                }
            }
        }

        public Task PruneAsync(IList<int> restApiIdsToKeep)
        {
            var mediaFiles = Directory.EnumerateFiles(MediaFolderPath)
                                      .Where(file => !file.EndsWith(RestApiTimestampPathSuffix, StringComparison.Ordinal));
            foreach (var mediaFile in mediaFiles)
            {
                var apiId = int.Parse(Path.GetFileName(mediaFile) ?? throw new Exception("File name is null!"));
                if (!restApiIdsToKeep.Contains(apiId))
                {
                    Debug.WriteLine($"Deleting {mediaFile}");
                    File.Delete(mediaFile);
                    File.Delete(mediaFile + RestApiTimestampPathSuffix);
                }
            }
            return Task.CompletedTask;
        }

        public async Task<bool> ContainsMedia(int restApiId, DateTimeOffset timestamp)
        {
            var timestampFile = PathForRestApiId(restApiId) + RestApiTimestampPathSuffix;
            if (File.Exists(timestampFile))
            {
                var stringTimestamp = Encoding.UTF8.GetString(await ReadFromDiskAsync(timestampFile));
                if (DateTimeOffset.TryParseExact(stringTimestamp, "o", CultureInfo.InvariantCulture,
                                                 DateTimeStyles.None, out var savedFileTimestamp))
                {
                    return timestamp == savedFileTimestamp;
                }

                Debug.WriteLine($"Warning: Found invalid media file timestamp '{stringTimestamp}'");
            }
            return false;
        }

        public string PathForRestApiId(int restApiId) => Path.Combine(MediaFolderPath, restApiId.ToString());
    }
}