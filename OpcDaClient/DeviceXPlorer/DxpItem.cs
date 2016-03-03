using System;
using System.Linq;
using System.Text;

namespace OpcDaClient.DeviceXPlorer
{
    public interface IDxpItem : IDaItem
    {
        DxpNode Node { get; set; }
    }

    public class DpxItem : DxpItem<object>
    {
        public override object Value
        {
            get { return Result.Value; }
            set { Result.Value = value; }
        }
    }

    public abstract class DxpItem<T> : IDxpItem
    {
        public string ItemId { get { return Node.ItemId; } }

        public DxpNode Node { get; set; }

        public DaValue Result { get; private set; } = new DaValue();

        public virtual T Value
        {
            get { return Result.Value == null ? default(T) : (T)Result.Value; }
            set { Result.Value = value; }
        }
    }

    public class DxpItemBoolean : DxpItem<bool>
    {
        public DxpItemBoolean(DxpNode node)
        {
            Node = node;
        }
    }

    public class DxpItemBooleanArray : DxpItem<bool[]>
    {
        public DxpItemBooleanArray(DxpNode node)
        {
            Node = node;
            if (node.Device.DeviceType != DxpDeviceType.Bit)
            {
                throw new NotSupportedException("Supported only bit device.");
            }
        }
    }

    public class DxpItemByteArray : IDxpItem
    {
        public DxpItemByteArray()
        {
            Result = new DaValue();
            Result.Subscribe(value =>
            {
                if (value != null)
                {
                    var t = value.GetType();
                    if (t.IsArray)
                    {
                        var arr = (Array)Result.Value;
                        if (t.GetElementType() == typeof(bool))
                        {
                        }
                    }
                }
                if (Node != null)
                {
                    switch (Node.Device.DeviceType)
                    {
                        case DxpDeviceType.Unknown:
                            break;

                        case DxpDeviceType.Bit:

                            break;

                        case DxpDeviceType.Byte:
                            if (Node.Size > 1)
                            {
                                Buffer = (byte[])Result.Value;
                            }
                            else
                            {
                                Buffer = new[] { (byte)Result.Value };
                            }
                            break;

                        case DxpDeviceType.Word:
                            if (Node.Size > 1)
                            {
                                Buffer = ((Int16[])Result.Value).SelectMany(_ => BitConverter.GetBytes(_)).ToArray();
                            }
                            else
                            {
                                Buffer = BitConverter.GetBytes((Int16)Result.Value);
                            }
                            break;

                        default:
                            break;
                    }
                }
            });
        }

        public byte[] Buffer { get; private set; }
        public string ItemId { get { return Node.ItemId; } }

        public DxpNode Node { get; set; }

        public DaValue Result { get; private set; }
    }

    public class DxpItemInt16 : DxpItem<Int16>
    {
        public DxpItemInt16(DxpNode node)
        {
            Node = node;
        }
    }

    public class DxpItemInt16Array : DxpItem<Int16[]>
    {
        public DxpItemInt16Array(DxpNode node)
        {
            Node = node;
        }
    }

    public class DxpItemString : DxpItem<string>
    {
        private Func<object, string> _toString = null;
        private Func<string, object> _toValue = null;

        public DxpItemString(DxpNode node) : this(node, DefaultEncoding)
        {
        }

        public DxpItemString(DxpNode node, Encoding encoding)
        {
            Node = node;
            Encoding = encoding;
            switch (node.Device.DeviceType)
            {
                case DxpDeviceType.Bit:
                    throw new NotSupportedException("Not suppoert bit device.");

                case DxpDeviceType.Byte:
                    _toString = FromBytes;
                    _toValue = ToBytes;
                    break;

                case DxpDeviceType.Word:
                    _toString = FromWords;
                    _toValue = ToWords;
                    break;

                default:
                    throw new NotSupportedException("Not suppoert unknown device.");
            }
        }

        public static Encoding DefaultEncoding { get; set; } = Encoding.Default;

        public Encoding Encoding { get; set; }

        public override string Value
        {
            get { return _toString(Result.Value); }
            set { Result.Value = _toValue(value); }
        }

        private string FromBytes(object value)
        {
            if (value == null) return null;
            var bytes = (byte[])value;
            return Encoding.GetString(bytes).TrimEnd((char)0);
        }

        private string FromWords(object value)
        {
            if (value == null) return null;
            var bytes = ((short[])value).SelectMany(_ => BitConverter.GetBytes(_)).ToArray();
            return Encoding.GetString(bytes).TrimEnd((char)0);
        }

        private int GetCharCount(char[] chars, int maxBytes)
        {
            var bytes = Encoding.GetByteCount(chars);
            if (bytes <= maxBytes)
            {
                return chars.Length;
            }
            var charCount = chars.Length * maxBytes / bytes;
            bytes = Encoding.GetByteCount(chars, 0, charCount);
            while (bytes < maxBytes)
            {
                charCount++;
                bytes = Encoding.GetByteCount(chars, 0, charCount);
                if (bytes > maxBytes)
                {
                    return charCount - 1;
                }
            }
            while (bytes > maxBytes)
            {
                charCount--;
                bytes = Encoding.GetByteCount(chars, 0, charCount);
            }
            return charCount;
        }

        private object ToBytes(string value)
        {
            var bytes = new byte[Node.Size];
            if (string.IsNullOrEmpty(value))
            {
                return bytes;
            }
            var chars = value.ToCharArray();
            var charCount = GetCharCount(chars, Node.Size);
            Encoding.GetBytes(chars, 0, charCount, bytes, 0);
            return bytes;
        }

        private object ToWords(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return new short[Node.Size];
            }
            var maxBytes = Node.Size * 2;
            var chars = value.ToCharArray();
            var charCount = GetCharCount(chars, maxBytes);
            var bytes = new byte[maxBytes];
            Encoding.GetBytes(chars, 0, charCount, bytes, 0);
            var r = new short[Node.Size];
            for (int i = 0; i < Node.Size; i++)
            {
                r[i] = BitConverter.ToInt16(bytes, i + i);
            }
            return r;
        }
    }
}