using System.Threading;
using System.Threading.Tasks;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.DataAccessLayer;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.Helpers;

namespace PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.ContentApiFetchers.Contracts
{
    public interface IFullDownloadableDataFetcher
    {
        /// <summary>
        /// Load all data for a downloadable (exhibit/route)
        /// </summary>
        /// <param name="downloadableId">The id of the exhibit to be fetched</param>
        /// <param name="idForRestApi">The id for the REST API of the exhibit to be fetched</param>
        /// <param name="token">Used for cancelling the download</param>
        /// <param name="listener">Used for reporting progress</param>
        /// <param name="dataAccess">Provides read/write access to the database</param>
        /// <returns></returns>
        Task FetchFullDownloadableDataIntoDatabase(
            string downloadableId, int idForRestApi, CancellationToken token, IProgressListener listener);
    }
}