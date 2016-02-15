using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpcDaClient
{
    class DefaultItemValueConverter<T> : IItemValueConverter<T>
    {
        public T FromDaItem(DaItem item)
        {
            if (item.RawValue == null)
            {
                return default(T);
            }
            return (T)item.RawValue;
        }

        public object ToObject(T value)
        {
            return value;
        }
    }

    class ItemValueConverterString : IItemValueConverter<string>
    {
        public Encoding Encoding { get; set; } = Encoding.Default;
        public int ByteLength { get; set; } = 2;

        public string FromDaItem(DaItem item)
        {
            if (item.RawValue == null)
            {
                return null;
            }
            byte[] buffer;
            try
            {
                buffer = BitConverter.GetBytes((short)item.RawValue);
            }
            catch
            {
                buffer = ((short[])item.RawValue).SelectMany(_ => BitConverter.GetBytes(_)).ToArray();
            }
            return Encoding.GetString(buffer);
        }

        public object ToObject(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return new short[ByteLength / 2];
            }
            var chars = value.ToArray();
            var bytes = new byte[ByteLength];
            Encoding.GetBytes(chars, 0, GetCharCount(chars, Encoding, ByteLength), bytes, 0);
            return Enumerable.Range(0, ByteLength / 2).Select(_ => BitConverter.ToInt16(bytes, _ * 2)).ToArray();
        }

        private int GetCharCount(char[] chars, Encoding encoding, int byteLength)
        {
            var byteCount = encoding.GetByteCount(chars);
            if (byteCount <= ByteLength)
                return chars.Length;
            var charCount = chars.Length * ByteLength / byteCount;
            byteCount = encoding.GetByteCount(chars, 0, charCount);
            while (byteCount < byteLength)
            {
                charCount++;
                byteCount = encoding.GetByteCount(chars, 0, charCount);
                if (byteCount > byteLength)
                {
                    return charCount - 1;
                }
            }
            while (byteCount > byteLength)
            {
                charCount--;
                byteCount = encoding.GetByteCount(chars, 0, charCount);
            }
            return charCount;
        }
    }
}
