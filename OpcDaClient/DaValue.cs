using System;
using System.Reactive.Subjects;

namespace OpcDaClient
{
    public class DaValue : IObservable<object>
    {
        private Subject<object> _subject = new Subject<object>();
        private object _value;

        public int ErrorCode { get; internal set; }
        public int Quality { get; internal set; }
        public DateTime Timestamp { get; internal set; }

        public object Value
        {
            get { return _value; }
            set
            {
                if (_value != value)
                {
                    _value = value;
                    _subject.OnNext(value);
                }
            }
        }

        public IDisposable Subscribe(IObserver<object> observer)
        {
            if (Value != null)
            {
                observer.OnNext(Value);
            }
            return _subject.Subscribe(observer);
        }
    }
}