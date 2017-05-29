using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.Common;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.Common.Contracts;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.DataAccessLayer;

namespace PaderbornUniversity.SILab.Hip.Mobile.Droid.Contracts
    {
    class DroidStorageSizeProvider : IStorageSizeProvider
        {
        public double GetDatabaseSize ()
        {
        var dataAccess = IoCManager.Resolve<IDataAccess> ();
        FileInfo fileInfo = new FileInfo (dataAccess.DatabasePath);

        if (!fileInfo.Exists)
            return 0.0;

        return (fileInfo.Length / 1024f) / 1024f;
        }
        }
    }