using System;

namespace PaderbornUniversity.SILab.Hip.Mobile.Shared.Helpers
{
    public class CachingObserver<T> : IObserver<T>
    {
        /// <summary>
        /// The last value received in OnNext.
        /// </summary>
        public T Last { get; private set; }

        public void OnCompleted()
        {
        }

        public void OnError(Exception error)
        {
            throw error;
        }

        public void OnNext(T value)
        {
            Last = value;
        }
    }
}