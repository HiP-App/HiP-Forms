using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaderbornUniversity.SILab.Hip.Mobile.Shared.ServiceAccessLayer
    {
    class TransientRetry
        {
        public static T Do<T> (
            Func<T> action,
            TimeSpan retryInterval,
            int retryCount = 3)
            {
            var exceptions = new List<Exception> ();

            for (int retry = 0; retry < retryCount; retry++)
                {
                try
                    {
                    if (retry > 0)
                        Task.Delay (retryInterval);
                    return action ();
                    }
                catch (Exception ex)
                    {
                    exceptions.Add (ex);
                    }
                }

            throw new AggregateException (exceptions);
            }
        }
    }
