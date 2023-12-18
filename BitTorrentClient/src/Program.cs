namespace BitTorrent;

using BitTorrent.BEncodeDecode;

internal class Program
{
    public static void Main(string[] args)
    {
        object ehh = BDecoder.DecodeFile("/Users/abdoulayedia/Projects/Dev/C#/Bittorrent-client/resources/test.txt.torrent");

        Console.WriteLine(ehh);
    }
}
