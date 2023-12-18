namespace BitTorrent.BEncodeDecode;
using System.Text;

public class BDecoder
{
    private static readonly byte[] BCodes = System.Text.Encoding.UTF8.GetBytes("dlie:");
    private static readonly byte DictionaryCode = BCodes[0];
    private static readonly byte ListCode = BCodes[1];
    private static readonly byte NumberCode = BCodes[2];
    private static readonly byte EndCode = BCodes[3];
    private static readonly byte DividerCode = BCodes[4];

    public static object Decode(byte[] bytes)
    {
        var enumerator = ((IEnumerable<byte>)bytes).GetEnumerator();
        _ = enumerator.MoveNext();

        return DecodeNextObject(enumerator);
    }

    public static object DecodeFile(string path)
    {
        if (!File.Exists(path))
            throw new FileNotFoundException("unable to find file: " + path);

        byte[] bytes = File.ReadAllBytes(path);

        return Decode(bytes);
    }

    private static object DecodeNextObject(IEnumerator<byte> enumerator)
    {
        if (enumerator.Current == DictionaryCode)
            return DecodeDictionary(enumerator);

        if (enumerator.Current == ListCode)
            return DecodeList(enumerator);

        if (enumerator.Current == NumberCode)
            return DecodeNumber(enumerator);

        return DecodeByteArray(enumerator);
    }

    private static byte[] DecodeByteArray(IEnumerator<byte> enumerator)
    {
        var lengthBytes = new List<byte>();

        do
        {
            if (enumerator.Current == DividerCode)
                break;

            lengthBytes.Add(enumerator.Current);
        } while (enumerator.MoveNext());

        string lengthStr = Encoding.UTF8.GetString(lengthBytes.ToArray());

        // TODO: create custom exception for parsing errors
        if (!int.TryParse(lengthStr, out int length))
            throw new Exception("unable to parse length of the byte array: " + lengthStr);

        byte[] bytes = new byte[length];

        for (int i = 0; i < length; ++i)
        {
            _ = enumerator.MoveNext();
            bytes[i] = enumerator.Current;
        }

        return bytes;
    }

    private static long DecodeNumber(IEnumerator<byte> enumerator)
    {
        List<byte> bytes = new();

        while (enumerator.MoveNext())
        {
            if (enumerator.Current == EndCode)
                break;

            bytes.Add(enumerator.Current);
        }

        string numStr = Encoding.UTF8.GetString(bytes.ToArray());
        if (!long.TryParse(numStr, out long number))
            throw new Exception("unable to parse number: " + numStr);

        return number;
    }

    private static List<object> DecodeList(IEnumerator<byte> enumerator)
    {
        List<object> list = new();

        while (enumerator.MoveNext())
        {
            if (enumerator.Current == EndCode)
                break;

            list.Add(DecodeNextObject(enumerator));
        }

        return list;
    }

    private static object DecodeDictionary(IEnumerator<byte> enumerator)
    {
        Dictionary<string, object> dict = new();
        List<string> keys = new();

        while (enumerator.MoveNext())
        {
            if (enumerator.Current == EndCode)
                break;

            string key = Encoding.UTF8.GetString(DecodeByteArray(enumerator));
            _ = enumerator.MoveNext();
            object val = DecodeNextObject(enumerator);

            keys.Add(key);
            dict.Add(key, val);
        }

        var sortedKeys = keys.OrderBy(k => BitConverter.ToString(Encoding.UTF8.GetBytes(k)));
        if (!keys.SequenceEqual(sortedKeys))
            throw new Exception("error loading dictionary: keys are not sorted.");

        return dict;
    }
}
