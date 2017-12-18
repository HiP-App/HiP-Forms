using System.IO;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.Common;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.Common.Contracts;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.DataAccessLayer;

namespace PaderbornUniversity.SILab.Hip.Mobile.Ios.Contracts
{
    class IosStorageSizeProvider : IStorageSizeProvider
    {
        public double GetDatabaseSizeMb()
        {
            string path = IoCManager.Resolve<IDataAccess>().DatabasePath;
            FileInfo fileInfo = new FileInfo(path);

            if (!fileInfo.Exists)
                return 0.0;

            return (fileInfo.Length / 1024f) / 1024f;
        }
    }
}