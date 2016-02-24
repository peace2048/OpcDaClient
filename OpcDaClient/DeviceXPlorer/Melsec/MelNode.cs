using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpcDaClient.DeviceXPlorer.Melsec
{
    public class MelNode : DxpNode
    {
        public MelNode()
        {
        }

        public MelNode(string itemId)
        {
            var branch = string.Empty;
            var id = itemId;
            var dots = itemId.Split('.');
            if (dots.Length > 1)
            {
                branch = string.Join(".", dots.Take(dots.Length - 1));
                id = dots[dots.Length - 1];
            }
            var parts = id.Split(':');
            Device = MelDevice.Defined().FirstOrDefault(_ => _.Name == parts[0]);
        }

        public MelDevice Device { get; set; }
        public override string ItemId
        {
            get
            {
                return "Device1." + Device.Name + Address.ToString();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public MelNode Next(int length)
        {
            return new MelNode { Address = Address + Size, Device = Device, Size = length };
        }
    }

    public class MelDevice
    {
        public static readonly MelDevice X = new MelDevice { Name = "X", Description = "入力リレー", DeviceType = DxpDeviceType.Bit };
        public static readonly MelDevice Y = new MelDevice { Name = "Y", Description = "出力リレー", DeviceType = DxpDeviceType.Bit };
        public static readonly MelDevice DX = new MelDevice { Name = "DX", Description = "ダイレクト入力", DeviceType = DxpDeviceType.Bit };
        public static readonly MelDevice DY = new MelDevice { Name = "DY", Description = "ダイレクト出力", DeviceType = DxpDeviceType.Bit };
        public static readonly MelDevice M = new MelDevice { Name = "M", Description = "内部リレー", DeviceType = DxpDeviceType.Bit };
        public static readonly MelDevice L = new MelDevice { Name = "L", Description = "ラッチリレー", DeviceType = DxpDeviceType.Bit };
        public static readonly MelDevice S = new MelDevice { Name = "S", Description = "ステップリレー", DeviceType = DxpDeviceType.Bit };
        public static readonly MelDevice SM = new MelDevice { Name = "SM", Description = "特殊リレー", DeviceType = DxpDeviceType.Bit };
        public static readonly MelDevice B = new MelDevice { Name = "B", Description = "リンクリレー", DeviceType = DxpDeviceType.Bit };
        public static readonly MelDevice F = new MelDevice { Name = "F", Description = "アナンシェータ", DeviceType = DxpDeviceType.Bit };
        public static readonly MelDevice V = new MelDevice { Name = "V", Description = "エッジリレー", DeviceType = DxpDeviceType.Bit };
        public static readonly MelDevice TC = new MelDevice { Name = "TC", Description = "タイマコイル", DeviceType = DxpDeviceType.Bit };
        public static readonly MelDevice TS = new MelDevice { Name = "TS", Description = "タイマ接点", DeviceType = DxpDeviceType.Bit };
        public static readonly MelDevice SC = new MelDevice { Name = "SC", Description = "積算タイマコイル", DeviceType = DxpDeviceType.Bit };
        public static readonly MelDevice SS = new MelDevice { Name = "SS", Description = "積算タイマ接点", DeviceType = DxpDeviceType.Bit };
        public static readonly MelDevice CC = new MelDevice { Name = "CC", Description = "カウンタコイル", DeviceType = DxpDeviceType.Bit };
        public static readonly MelDevice CS = new MelDevice { Name = "CS", Description = "カウンタ接点", DeviceType = DxpDeviceType.Bit };
        public static readonly MelDevice SB = new MelDevice { Name = "SB", Description = "リンク特殊リレー", DeviceType = DxpDeviceType.Bit };
        public static readonly MelDevice ST = new MelDevice { Name = "ST", Description = "ステート", DeviceType = DxpDeviceType.Bit };
        public static readonly MelDevice D = new MelDevice { Name = "D", Description = "データレジスタ", DeviceType = DxpDeviceType.Word };
        public static readonly MelDevice SD = new MelDevice { Name = "SD", Description = "特殊レジスタ", DeviceType = DxpDeviceType.Word };
        public static readonly MelDevice W = new MelDevice { Name = "W", Description = "リンクレジスタ", DeviceType = DxpDeviceType.Word };
        public static readonly MelDevice TN = new MelDevice { Name = "TN", Description = "タイマ現在値", DeviceType = DxpDeviceType.Word };
        public static readonly MelDevice SN = new MelDevice { Name = "SN", Description = "積算タイマ現在値", DeviceType = DxpDeviceType.Word };
        public static readonly MelDevice CN = new MelDevice { Name = "CN", Description = "カウンタ現在値", DeviceType = DxpDeviceType.Word };
        public static readonly MelDevice R = new MelDevice { Name = "R", Description = "ファイルレジスタ", DeviceType = DxpDeviceType.Word };
        public static readonly MelDevice ER = new MelDevice { Name = "ER", Description = "拡張ファイルレジスタ", DeviceType = DxpDeviceType.Word };
        public static readonly MelDevice BM = new MelDevice { Name = "BM", Description = "ランダムアクセスバッファ", DeviceType = DxpDeviceType.Word };
        public static readonly MelDevice SW = new MelDevice { Name = "SW", Description = "リンク特殊レジスタ", DeviceType = DxpDeviceType.Word };
        public static readonly MelDevice Z = new MelDevice { Name = "Z", Description = "インデックスレジスタ", DeviceType = DxpDeviceType.Word };
        public static readonly MelDevice ZR = new MelDevice { Name = "ZR", Description = "拡張ファイルレジスタ", DeviceType = DxpDeviceType.Word };
        public static readonly MelDevice G = new MelDevice { Name = "G", Description = "バッファメモリ", DeviceType = DxpDeviceType.Word };

        public static IEnumerable<MelDevice> Defined()
        {
            return typeof(MelDevice)
                .GetFields(System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public)
                .Where(_ => _.FieldType == typeof(MelDevice))
                .Select(_ => (MelDevice)_.GetValue(null));
        }
        private MelDevice() { }
        public string Name { get; private set; }
        public string Description { get; private set; }
        public DxpDeviceType DeviceType { get; private set; }
    }
}
