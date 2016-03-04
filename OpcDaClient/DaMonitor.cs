using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reactive.Disposables;
using System.Reactive.Subjects;
using OpcDaClient.Rcw;

namespace OpcDaClient
{
    public class DaMonitor : IDisposable, IObservable<List<IDaItem>>
    {
        private IOpcGroupAsync _group;
        private List<ItemClass> _items = new List<ItemClass>();
        private IDisposable _disposable = Disposable.Empty;

        public DaMonitor()
        {
        }

        public void Add(IDaItem item)
        {
            if (_group != null)
            {
                var def = new OpcItemDefine { IsActive = true };
                var result = _group.AddItems(new[] { def });
                _items.Add(new ItemClass { ClientHandle = def.ClientHandle, Item = item, ServerHandle = result[0].ServerHandle });
            }
            else
            {
                _items.Add(new ItemClass { Item = item });
            }
        }

        public void AddRange(IEnumerable<IDaItem> items)
        {
            if (_group != null)
            {
                var defs = items.Select(_ => new OpcItemDefine { IsActive = true, ItemId = _.ItemId, ClientHandle = _clientHandleSequence.GetNext() }).ToArray();
                var results = _group.AddItems(defs);
                _items.AddRange(
                    items.Zip(defs, (item, def) => new ItemClass { Item = item, ClientHandle = def.ClientHandle })
                    .Zip(results, (item, result) =>
                    {
                        item.ServerHandle = result.ServerHandle;
                        return item;
                    }));
            }
            else
            {
                _items.AddRange(items.Select(_ => new ItemClass { Item = _ }));
            }
        }

        private Sequence _clientHandleSequence;
        internal void Attach(IOpcGroupAsync group, Sequence sequence)
        {
            if (_group != null)
            {
                throw new InvalidOperationException("既に開始されています。");
            }
            _group = group;
            _clientHandleSequence = sequence;
            var defs = _items.Select(_ => new OpcItemDefine { ClientHandle = _clientHandleSequence.GetNext(), IsActive = true, ItemId = _.Item.ItemId }).ToArray();
            var results = _group.AddItems(defs);
            _items.Zip(defs, (item, def) =>
            {
                item.ClientHandle = def.ClientHandle;
                item.ServerHandle = results[0].ServerHandle;
                return 0;
            })
            .Sum();
            _group.DataChange += group_DataChange;
            _disposable = Disposable.Create(() =>
            {
                _group.DataChange -= group_DataChange;
            });
        }

        private void group_DataChange(object sender, DataChangeEventArgs e)
        {
            _subject.OnNext(e.Items.Select(_ => {
                var item = _items.FirstOrDefault(__ => _.ClientHandle == __.ClientHandle)?.Item;
                if (item != null)
                {
                    var value = item.Result;
                    value.ErrorCode = _.ErrorCode;
                    value.Quality = _.Quality;
                    value.Timestamp = _.Timestamp;
                    value.Value = _.DataValue;
                }
                return item;
            }).ToList());
        }

        public void Dispose()
        {
            _disposable.Dispose();
        }

        private Subject<List<IDaItem>> _subject = new Subject<List<IDaItem>>();

        public IDisposable Subscribe(IObserver<List<IDaItem>> observer)
        {
            return _subject.Subscribe(observer);
        }

        private class ItemClass
        {
            public IDaItem Item { get; set; }
            public int ServerHandle { get; set; }
            public int ClientHandle { get; set; }
        }
    }
}
