using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaderbornUniversity.SILab.Hip.Mobile.Shared.Common.Contracts
{
    public interface IStorageSizeProvider
    {
        double GetDatabaseSize();
    }
}