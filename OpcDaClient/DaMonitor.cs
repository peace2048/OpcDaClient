using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpcDaClient.RcwWrapper;
using System.Reactive.Disposables;
using System.Reactive.Subjects;

namespace OpcDaClient
{
    public class DaMonitor : RcwWrapper.IOpcDataCallback, IDisposable, IObservable<List<DaItem>>
    {
        private RcwWrapper.IOpcGroup _group;
        private List<ItemClass> _items = new List<ItemClass>();
        private IDisposable _disposable = Disposable.Empty;

        public DaMonitor()
        {
        }

        public void Add(DaItem item)
        {
            if (_group != null)
            {
                var def = new RcwWrapper.OpcItemDefine { IsActive = true, ItemId = item.Node.ItemId, ClientHandle = _clientHandleSequence.GetNext() };
                _group.AddItems(new[] { def });
                _items.Add(new ItemClass { ClientHandle = def.ClientHandle, Item = item, ServerHandle = def.ServerHandle });
            }
            else
            {
                _items.Add(new ItemClass { Item = item });
            }
        }

        public void AddRange(IEnumerable<DaItem> items)
        {
            if (_group != null)
            {
                var defs = items.Select(_ => new OpcItemDefine { IsActive = true, ItemId = _.Node.ItemId, ClientHandle = _clientHandleSequence.GetNext() }).ToArray();
                _group.AddItems(defs);
                _items.AddRange(defs.Select(_ => new ItemClass { ClientHandle = _.ClientHandle, Item = items.First(__ => __.Node.ItemId == _.ItemId), ServerHandle = _.ServerHandle }));
            }
            else
            {
                _items.AddRange(items.Select(_ => new ItemClass { Item = _ }));
            }
        }

        private Sequence _clientHandleSequence;
        internal void Attach(IOpcGroup group, Sequence sequence)
        {
            if (_group != null)
            {
                throw new InvalidOperationException("既に開始されています。");
            }
            _group = group;
            _clientHandleSequence = sequence;
            var defs = _items.Select(_ => new OpcItemDefine { ClientHandle = _clientHandleSequence.GetNext(), IsActive = true, ItemId = _.Item.Node.ItemId }).ToArray();
            _group.AddItems(defs);
            _items.Zip(defs, (item, def) =>
            {
                item.ClientHandle = def.ClientHandle;
                item.ServerHandle = def.ServerHandle;
                return 0;
            })
            .Sum();
            var disposable = _group.Watch(this);
            _disposable = Disposable.Create(() =>
            {
                disposable.Dispose();
                _items.Clear();
                _group.Dispose();
                _group = null;
            });
        }

        public void Dispose()
        {
            _disposable.Dispose();
        }

        public void OnDataChanged(int transactionId, OpcDataChanged[] values)
        {
            _subject.OnNext(values.Join(_items, a => a.ClientHandle, b => b.ClientHandle, (a, b) =>
            {
                var value = b.Item.Result;
                value.ErrorCode = a.ErrorCode;
                value.Quality = a.Quality;
                value.Timestamp = a.Timestamp;
                value.Value = a.Value;
                return b.Item;
            })
            .ToList());
        }

        private Subject<List<DaItem>> _subject = new Subject<List<DaItem>>();

        public IDisposable Subscribe(IObserver<List<DaItem>> observer)
        {
            return _subject.Subscribe(observer);
        }

        private class ItemClass
        {
            public DaItem Item { get; set; }
            public int ServerHandle { get; set; }
            public int ClientHandle { get; set; }
        }
    }
}
