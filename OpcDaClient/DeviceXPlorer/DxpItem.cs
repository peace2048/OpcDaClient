using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpcDaClient.DeviceXPlorer
{
    public abstract class DxpItem<T> : IDxpItem
    {
        public string ItemId { get { return Node.ItemId; } }

        public DxpNode Node { get; set; }

        public DaValue Result { get; private set; } = new DaValue();

        public virtual T Value
        {
            get { return (T)Result.Value; }
            set { Result.Value = value; }
        }
    }

    public class DpxItem : DxpItem<object>
    {
        public override object Value
        {
            get { return Result.Value; }
            set { Result.Value = value; }
        }
    }

    public interface IDxpItem : IDaItem
    {
        DxpNode Node { get; set; }
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
        public string ItemId { get { return Node.ItemId; } }

        public DxpNode Node { get; set; }

        public DaValue Result { get; private set; }

        public byte[] Buffer { get; private set; }
    }

    public class DxpItemInt16 : DxpItem<Int16>
    {
        public DxpItemInt16(DxpNode node)
        {
            Node = node;
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
            switch (node.Device.DeviceType)
            {
                case DxpDeviceType.Bit:
                    Result.Value = new bool[node.Size];
                    break;
                default:
                    break;
            }
        }
    }
    public class DxpItemInt16Array : DxpItem<Int16[]>
    {
        public DxpItemInt16Array(DxpNode node)
        {
            Node = node;
            switch (node.Device.DeviceType)
            {
                case DxpDeviceType.Unknown:
                    break;
                default:
                    Result.Value = new short[node.Size];
                    break;
            }
        }
    }
}
