using System;

namespace PaderbornUniversity.SILab.Hip.Mobile.HipMobileTests.TestHelpers
{
    internal class CachingObserver<T> : IObserver<T>
    {
        public T Last { get; private set; }

        public void OnCompleted()
        {
        }

        public void OnError(Exception error)
        {
        }

        public void OnNext(T value)
        {
            Last = value;
        }
    }
}