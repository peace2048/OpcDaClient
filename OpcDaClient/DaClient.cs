using OpcDaClient.RcwWrapper;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OpcDaClient
{
    public sealed class DaClient : IDisposable
    {
        private IServerFactory _factory;
        private IOpcServer _server;
        private IOpcGroup _group;
        private ConcurrentDictionary<string, int> _serverHandles;

        public DaClient(IServerFactory factory)
        {
            _factory = factory;
        }

        public void Connect(string progId)
        {
            if (_server != null)
            {
                throw new InvalidOperationException("既にOPC Serverに接続されています。");
            }
            _server = _factory.CreateFromProgId(progId);
            _group = _server.AddGroup("default", false, 1000, 0, 0);
            _serverHandles = new ConcurrentDictionary<string, int>();
        }

        public void Disconnect()
        {
            if (_server == null)
            {
                throw new InvalidOperationException("OPC Serverに接続されていません。");
            }
            _group.Dispose();
            _group = null;
            _server.Dispose();
            _server = null;
            _serverHandles = null;
        }

        internal IOpcGroup CreateGroup(int updateRate, float deadband, int localeId)
        {
            return _server.AddGroup(string.Empty, true, updateRate, deadband, localeId);
        }

        public void Read(IEnumerable<DaItem> items)
        {
            var handles = items.Select(_ => _serverHandles.GetOrAdd(
                _.Node.ItemId,
                itemId =>
                {
                    var def = new OpcItemDefine { IsActive = true, ItemId = itemId, ClientHandle = _clientHandleSequence.GetNext() };
                    _group.AddItems(new[] { def });
                    return def.ServerHandle;
                }))
                .ToArray();
            items.Zip(
                _group.Read(handles),
                (item, result) =>
                {
                    item.Result.ErrorCode = result.ErrorCode;
                    item.Result.Quality = result.Quality;
                    item.Result.Timestamp = result.Timestamp;
                    item.Result.Value = result.Value;
                    return item;
                })
                .Count();
        }

        private readonly Sequence _clientHandleSequence = new Sequence(0, int.MaxValue);

        public DaMonitor Watch(IEnumerable<DaItem> items, TimeSpan updateRate, float deadband, int localeId)
        {
            var monitor = new DaMonitor();
            monitor.AddRange(items);
            Watch(monitor, updateRate, deadband, localeId);
            return monitor;
        }

        public void Watch(DaMonitor monitor, TimeSpan updateRate, float deadband, int localeId)
        {
            var group = _server.AddGroup(string.Empty, true, (int)updateRate.TotalMilliseconds, deadband, localeId);
            monitor.Attach(group, _clientHandleSequence);
        }

        public void Dispose()
        {
            _serverHandles = null;
            Interlocked.Exchange(ref _group, null)?.Dispose();
            Interlocked.Exchange(ref _server, null)?.Dispose();
        }
    }
}
