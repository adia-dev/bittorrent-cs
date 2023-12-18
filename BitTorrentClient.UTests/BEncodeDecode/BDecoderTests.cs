namespace BitTorrentTest.BEncodeDecode;
using System.Text;
using BitTorrent.BEncodeDecode;

public class BDecoderTests
{
    [Fact]
    public void DecodeShouldReturnDictionaryWhenGivenValidDictionaryEncodedBytes()
    {
        byte[] encoded = Encoding.UTF8.GetBytes("d3:key5:valuee");
        Dictionary<string, object> expected = new() { { "key", Encoding.UTF8.GetBytes("value") } };

        object result = BDecoder.Decode(encoded);

        Assert.Equal(expected, result);
    }

    [Fact]
    public void DecodeShouldReturnListWhenGivenValidListEncodedBytes()
    {
        byte[] encoded = Encoding.UTF8.GetBytes("l5:valuee");
        List<object> expected = new() { Encoding.UTF8.GetBytes("value") };

        object result = BDecoder.Decode(encoded);

        Assert.Equal(expected, result);
    }

    [Fact]
    public void DecodeShouldThrowFileNotFoundExceptionWhenFileDoesNotExist()
    {
        string invalidPath = "nonexistent.file";

        _ = Assert.Throws<FileNotFoundException>(() => BDecoder.DecodeFile(invalidPath));
    }
}

