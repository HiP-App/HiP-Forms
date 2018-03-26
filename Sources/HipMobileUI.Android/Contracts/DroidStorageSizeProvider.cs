using PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.Managers;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.Common.Contracts;
using System.IO;

namespace PaderbornUniversity.SILab.Hip.Mobile.Droid.Contracts
{
    class DroidStorageSizeProvider : IStorageSizeProvider
    {
        public double GetDatabaseSizeMb()
        {
            var fileInfo = new FileInfo(DbManager.DataAccess.DatabasePath);

            if (!fileInfo.Exists)
                return 0;

            return (fileInfo.Length / 1024f) / 1024f;
        }
    }
}