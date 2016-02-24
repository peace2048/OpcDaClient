using System.Collections.Generic;
using System.Linq;

namespace OpcDaClient.DeviceXPlorer.Melsec
{
    public class MelDevice
    {
        public static readonly DxpDevice B  = new DxpDevice("B", "リンクリレー", DxpDeviceType.Bit, AddressNotation.Unknown);
        public static readonly DxpDevice BM = new DxpDevice("BM", "ランダムアクセスバッファ", DxpDeviceType.Word, AddressNotation.Unknown);
        public static readonly DxpDevice CC = new DxpDevice("CC", "カウンタコイル", DxpDeviceType.Bit, AddressNotation.Unknown);
        public static readonly DxpDevice CN = new DxpDevice("CN", "カウンタ現在値", DxpDeviceType.Word, AddressNotation.Unknown);
        public static readonly DxpDevice CS = new DxpDevice("CS", "カウンタ接点", DxpDeviceType.Bit, AddressNotation.Unknown);
        public static readonly DxpDevice D  = new DxpDevice("D", "データレジスタ", DxpDeviceType.Word, AddressNotation.Unknown);
        public static readonly DxpDevice DX = new DxpDevice("DX", "ダイレクト入力", DxpDeviceType.Bit, AddressNotation.Unknown);
        public static readonly DxpDevice DY = new DxpDevice("DY", "ダイレクト出力", DxpDeviceType.Bit, AddressNotation.Unknown);
        public static readonly DxpDevice ER = new DxpDevice("ER", "拡張ファイルレジスタ", DxpDeviceType.Word, AddressNotation.Unknown);
        public static readonly DxpDevice F  = new DxpDevice("F", "アナンシェータ", DxpDeviceType.Bit, AddressNotation.Unknown);
        public static readonly DxpDevice G  = new DxpDevice("G", "バッファメモリ", DxpDeviceType.Word, AddressNotation.Unknown);
        public static readonly DxpDevice L  = new DxpDevice("L", "ラッチリレー", DxpDeviceType.Bit, AddressNotation.Unknown);
        public static readonly DxpDevice M  = new DxpDevice("M", "内部リレー", DxpDeviceType.Bit, AddressNotation.Unknown);
        public static readonly DxpDevice R  = new DxpDevice("R", "ファイルレジスタ", DxpDeviceType.Word, AddressNotation.Unknown);
        public static readonly DxpDevice S  = new DxpDevice("S", "ステップリレー", DxpDeviceType.Bit, AddressNotation.Unknown);
        public static readonly DxpDevice SB = new DxpDevice("SB", "リンク特殊リレー", DxpDeviceType.Bit, AddressNotation.Unknown);
        public static readonly DxpDevice SC = new DxpDevice("SC", "積算タイマコイル", DxpDeviceType.Bit, AddressNotation.Unknown);
        public static readonly DxpDevice SD = new DxpDevice("SD", "特殊レジスタ", DxpDeviceType.Word, AddressNotation.Unknown);
        public static readonly DxpDevice SM = new DxpDevice("SM", "特殊リレー", DxpDeviceType.Bit, AddressNotation.Unknown);
        public static readonly DxpDevice SN = new DxpDevice("SN", "積算タイマ現在値", DxpDeviceType.Word, AddressNotation.Unknown);
        public static readonly DxpDevice SS = new DxpDevice("SS", "積算タイマ接点", DxpDeviceType.Bit, AddressNotation.Unknown);
        public static readonly DxpDevice ST = new DxpDevice("ST", "ステート", DxpDeviceType.Bit, AddressNotation.Unknown);
        public static readonly DxpDevice SW = new DxpDevice("SW", "リンク特殊レジスタ", DxpDeviceType.Word, AddressNotation.Unknown);
        public static readonly DxpDevice TC = new DxpDevice("TC", "タイマコイル", DxpDeviceType.Bit, AddressNotation.Unknown);
        public static readonly DxpDevice TN = new DxpDevice("TN", "タイマ現在値", DxpDeviceType.Word, AddressNotation.Unknown);
        public static readonly DxpDevice TS = new DxpDevice("TS", "タイマ接点", DxpDeviceType.Bit, AddressNotation.Unknown);
        public static readonly DxpDevice V  = new DxpDevice("V", "エッジリレー", DxpDeviceType.Bit, AddressNotation.Unknown);
        public static readonly DxpDevice W  = new DxpDevice("W", "リンクレジスタ", DxpDeviceType.Word, AddressNotation.Unknown);
        public static readonly DxpDevice X  = new DxpDevice("X", "入力リレー", DxpDeviceType.Bit, AddressNotation.Unknown);
        public static readonly DxpDevice Y  = new DxpDevice("Y", "出力リレー", DxpDeviceType.Bit, AddressNotation.Unknown);
        public static readonly DxpDevice Z  = new DxpDevice("Z", "インデックスレジスタ", DxpDeviceType.Word, AddressNotation.Unknown);
        public static readonly DxpDevice ZR = new DxpDevice("ZR", "拡張ファイルレジスタ", DxpDeviceType.Word, AddressNotation.Unknown);

        public static IEnumerable<DxpDevice> Defined()
        {
            return typeof(DxpDevice)
                .GetFields(System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public)
                .Where(_ => _.FieldType == typeof(DxpDevice))
                .Select(_ => (DxpDevice)_.GetValue(null));
        }
    }
}