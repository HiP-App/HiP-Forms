using System;
using System.IO;
using System.Threading.Tasks;

namespace PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.Managers
{
    public interface IMediaToDiskManager
    {
        Task<Stream> WriteMediaToDisk();
    }
}
