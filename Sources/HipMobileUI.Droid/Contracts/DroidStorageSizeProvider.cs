using System.IO;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.Common;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.Common.Contracts;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.DataAccessLayer;

namespace PaderbornUniversity.SILab.Hip.Mobile.Droid.Contracts
{
    class DroidStorageSizeProvider : IStorageSizeProvider
    {
        public double GetDatabaseSizeMb()
        {
            var dataAccess = IoCManager.Resolve<IDataAccess>();
            FileInfo fileInfo = new FileInfo(dataAccess.DatabasePath);

            if (!fileInfo.Exists)
                return 0.0;

            return (fileInfo.Length / 1024f) / 1024f;
        }
    }
}