using System;
using System.Collections.Generic;
using Xamarin.Forms.Internals;

namespace PaderbornUniversity.SILab.Hip.Mobile.Shared.Helpers
{
    public class Observable<T> : IObservable<T>
    {
        private class DisposeAction : IDisposable
        {
            private readonly IList<IObserver<T>> observers;
            private readonly IObserver<T> observer;

            public DisposeAction(IList<IObserver<T>> observers, IObserver<T> observer)
            {
                this.observers = observers;
                this.observer = observer;
            }

            public void Dispose()
            {
                observers.Remove(observer);
            }
        }

        private T current;

        public T Current
        {
            get => current;
            set
            {
                current = value;
                UpdateObservers();
            }
        }

        private readonly IList<IObserver<T>> observers = new List<IObserver<T>>();

        public Observable(T current)
        {
            this.current = current;
        }

        public IDisposable Subscribe(IObserver<T> observer)
        {
            observer.OnNext(current);
            observers.Add(observer);
            return new DisposeAction(observers, observer);
        }

        private void UpdateObservers()
        {
            observers.ForEach(observer => observer.OnNext(Current));
        }
    }
}