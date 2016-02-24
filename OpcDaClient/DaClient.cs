using OpcDaClient.Rcw;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace OpcDaClient
{
    public sealed class DaClient : IDisposable
    {
        private static readonly ILog _logger = LogManager.GetLogger(typeof(DaClient));

        private readonly Sequence _clientHandleSequence = new Sequence(0, int.MaxValue);
        private IServerFactory _factory;
        private OpcGroup _group;
        private OpcServer _server;
        private ConcurrentDictionary<string, int> _serverHandles;

        public DaClient(IServerFactory factory)
        {
            _logger.Trace($".ctor( {factory} )");
            if (factory == null)
            {
                throw new ArgumentNullException(nameof(factory));
            }
            _factory = factory;
        }

        public void Connect(string progId)
        {
            _logger.Trace($"Connect( {progId ?? "<null>"} )");
            if (_server != null)
            {
                throw new InvalidOperationException("既にOPC Serverに接続されています。");
            }
            if (progId == null)
            {
                throw new ArgumentNullException(nameof(progId));
            }
            _server = _factory.CreateFromProgId(progId);
            if (_server == null)
            {
                throw new NullReferenceException(nameof(_server));
            }
            _group = _server.AddGroup("default", true, 1000, 0, 0, 0);
            if (_group == null)
            {
                throw new NullReferenceException(nameof(_group));
            }
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

        public void Dispose()
        {
            _serverHandles = null;
            Interlocked.Exchange(ref _group, null)?.Dispose();
            Interlocked.Exchange(ref _server, null)?.Dispose();
        }

        public void Read(IEnumerable<IDaItem> items)
        {
            _logger.Trace($"Read( {items} )");
            if (items == null)
            {
                throw new ArgumentNullException(nameof(items));
            }
            var handles = GetServerHandles(items);
            var results = _group.Read(OpcDataSource.Device, handles);
            items.Zip(results,
                (item, result) =>
                {
                    item.Result.ErrorCode = result.ErrorCode;
                    item.Result.Quality = result.Quality;
                    item.Result.Timestamp = result.Timestamp;
                    item.Result.Value = result.DataValue;
                    return 0;
                })
                .Sum();
        }

        private int[] GetServerHandles(IEnumerable<IDaItem> items)
        {
            return items.Select(_ => _serverHandles.GetOrAdd(
                _.ItemId,
                itemId =>
                {
                    var def = new OpcItemDefine { IsActive = true, ItemId = itemId, ClientHandle = _clientHandleSequence.GetNext() };
                    var results = _group.AddItems(new[] { def });
                    return results[0].ServerHandle;
                }))
                .ToArray();
        }

        public DaMonitor Watch(IEnumerable<IDaItem> items, TimeSpan updateRate, float deadband, int localeId)
        {
            var monitor = new DaMonitor();
            monitor.AddRange(items);
            Watch(monitor, updateRate, deadband, localeId);
            return monitor;
        }

        public void Watch(DaMonitor monitor, TimeSpan updateRate, float deadband, int localeId)
        {
            var clientHandle = _clientHandleSequence.GetNext();
            var group = _server.AddGroup(string.Empty, true, (int)updateRate.TotalMilliseconds, clientHandle, deadband, localeId);
            monitor.Attach(group, _clientHandleSequence);
        }

        public void Write(IEnumerable<IDaItem> items)
        {
            var handles = GetServerHandles(items);
            var values = items.Select(_ => _.Result.Value).ToArray();
            var errors = _group.Write(handles, values);
            items.Zip(errors,
                (item, err) =>
                {
                    item.Result.ErrorCode = err;
                    return 0;
                })
                .Sum();
        }

        internal OpcGroup CreateGroup(int updateRate, float deadband, int localeId)
        {
            var clientHandle = _clientHandleSequence.GetNext();
            return _server.AddGroup(string.Empty, true, updateRate, clientHandle, deadband, localeId);
        }
    }
}