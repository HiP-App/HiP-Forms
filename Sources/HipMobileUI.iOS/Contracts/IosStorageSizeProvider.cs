using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using Foundation;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.Common;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.Common.Contracts;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.DataAccessLayer;
using UIKit;

namespace PaderbornUniversity.SILab.Hip.Mobile.Ios.Contracts
    {
    class IosStorageSizeProvider:IStorageSizeProvider
        {
            public double GetDatabaseSize ()
            {
            string path = IoCManager.Resolve<IDataAccess> ().DatabasePath;
            FileInfo fileInfo = new FileInfo (path);

            if (!fileInfo.Exists)
                return 0.0;

            return (fileInfo.Length / 1024f) / 1024f;
            }

        }
    }