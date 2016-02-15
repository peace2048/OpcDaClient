using System;
using System.Linq;
using System.Reactive.Linq;

namespace OpcDaClient
{
    public class DaItem
    {
        public DaNode Node { get; set; }
        public object RawValue { get { return Result.Value; } set { Result.Value = value; } }
        public DaValue Result { get; private set; } = new DaValue();
    }

    public class DaItem<T> : DaItem, IObservable<T>
    {
        private IItemValueConverter<T> _converter;

        public IItemValueConverter<T> Converter
        {
            get
            {
                if (_converter == null)
                {
                    _converter = new DefaultItemValueConverter<T>();
                }
                return _converter;
            }
            set
            {
                _converter = value;
            }
        }

        public T Value
        {
            get { return Converter.FromDaItem(this); }
            set { RawValue = Converter.ToObject(value); }
        }

        public IDisposable Subscribe(IObserver<T> observer)
        {
            return Result.Select(_ => Value).Subscribe(observer);
        }
    }

    public class DaItemString : DaItem<string>
    {
        public DaItemString(System.Text.Encoding encoding, int byteLength)
        {
            Converter = new ItemValueConverterString { Encoding = encoding, ByteLength = byteLength };
        }
    }
}