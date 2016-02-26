using System.Collections.Generic;
using System.Linq;

namespace OpcDaClient.DeviceXPlorer
{
    public class MelDevice
    {
        public static readonly DxpBitDevice B                   = new DxpBitDevice(nameof(B),   AddressNotation.Hexadecimal,    "リンクリレー" );
        public static readonly DxpWordDevice BM                 = new DxpWordDevice(nameof(BM), AddressNotation.Hexadecimal,    "ランダムアクセスバッファ");
        public static readonly DxpWordDevice CC                 = new DxpWordDevice(nameof(CC), AddressNotation.Decimal,        "カウンタコイル");
        public static readonly DxpWordDevice CN                 = new DxpWordDevice(nameof(CN), AddressNotation.Decimal,        "カウンタ現在値");
        public static readonly DxpWordDevice CS                 = new DxpWordDevice(nameof(CS), AddressNotation.Decimal,        "カウンタ接点");
        public static readonly DxpWordDevice D                  = new DxpWordDevice(nameof(D),  AddressNotation.Decimal,        "データレジスタ");
        public static readonly DxpBitDevice DX                  = new DxpBitDevice(nameof(DX),  AddressNotation.Hexadecimal,    "ダイレクト入力");
        public static readonly DxpBitDevice DY                  = new DxpBitDevice(nameof(DY),  AddressNotation.Hexadecimal,    "ダイレクト出力");
        public static readonly DxpWordDeviceWithBlockNumber ER  = new DxpWordDeviceWithBlockNumber(nameof(ER),  AddressNotation.Decimal, "拡張ファイルレジスタ");
        public static readonly DxpBitDevice F                   = new DxpBitDevice(nameof(F),   AddressNotation.Decimal,        "アナンシェータ");
        public static readonly DxpWordDevice G                  = new DxpWordDevice(nameof(G),  AddressNotation.Decimal,        "バッファメモリ");
        public static readonly DxpBitDevice L                   = new DxpBitDevice(nameof(L),   AddressNotation.Decimal,        "ラッチリレー");
        public static readonly DxpBitDevice M                   = new DxpBitDevice (nameof(M),  AddressNotation.Decimal,        "内部リレー");
        public static readonly DxpWordDevice R                  = new DxpWordDevice(nameof(R),  AddressNotation.Decimal,        "ファイルレジスタ");
        public static readonly DxpBitDevice S                   = new DxpBitDevice(nameof(S),   AddressNotation.Decimal,        "ステップリレー");
        public static readonly DxpBitDevice SB                  = new DxpBitDevice(nameof(SB),  AddressNotation.Hexadecimal,    "リンク特殊リレー");
        public static readonly DxpBitDevice SC                  = new DxpBitDevice(nameof(SC),  AddressNotation.Decimal,        "積算タイマコイル");
        public static readonly DxpWordDevice SD                 = new DxpWordDevice(nameof(SD), AddressNotation.Decimal,        "特殊レジスタ");
        public static readonly DxpBitDevice SM                  = new DxpBitDevice(nameof(SM),  AddressNotation.Decimal,        "特殊リレー");
        public static readonly DxpWordDevice SN                 = new DxpWordDevice(nameof(SN), AddressNotation.Decimal,        "積算タイマ現在値");
        public static readonly DxpBitDevice SS                  = new DxpBitDevice(nameof(SS),  AddressNotation.Decimal,        "積算タイマ接点");
        public static readonly DxpBitDevice ST                  = new DxpBitDevice(nameof(ST),  AddressNotation.Decimal,        "ステート");
        public static readonly DxpWordDevice SW                 = new DxpWordDevice(nameof(SW), AddressNotation.Hexadecimal,    "リンク特殊レジスタ");
        public static readonly DxpBitDevice TC                  = new DxpBitDevice(nameof(TC),  AddressNotation.Decimal,        "タイマコイル");
        public static readonly DxpWordDevice TN                 = new DxpWordDevice(nameof(TN), AddressNotation.Decimal,        "タイマ現在値");
        public static readonly DxpBitDevice TS                  = new DxpBitDevice(nameof(TS),  AddressNotation.Decimal,        "タイマ接点");
        public static readonly DxpBitDevice V                   = new DxpBitDevice(nameof(V),   AddressNotation.Decimal,        "エッジリレー");
        public static readonly DxpWordDevice W                  = new DxpWordDevice(nameof(W),  AddressNotation.Hexadecimal,    "リンクレジスタ");
        public static readonly DxpBitDevice X                   = new DxpBitDevice(nameof(X),   AddressNotation.Hexadecimal,    "入力リレー");
        public static readonly DxpBitDevice Y                   = new DxpBitDevice(nameof(Y),   AddressNotation.Hexadecimal,    "出力リレー");
        public static readonly DxpWordDevice Z                  = new DxpWordDevice(nameof(Z),  AddressNotation.Decimal,        "インデックスレジスタ");
        public static readonly DxpWordDevice ZR                 = new DxpWordDevice(nameof(ZR), AddressNotation.Decimal,        "拡張ファイルレジスタ");

        public static IEnumerable<DxpDevice> Defined()
        {
            var devType = typeof(DxpDevice);

            return typeof(MelDevice)
                .GetFields(System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public)
                .Where(_ => devType.IsAssignableFrom(_.FieldType))
                .Select(_ => (DxpDevice)_.GetValue(null));
        }
    }
}