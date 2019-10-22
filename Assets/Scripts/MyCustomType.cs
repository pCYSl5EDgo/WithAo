using AoAndSugi.Game.Models;
using ExitGames.Client.Photon;
using UnityEngine;
using Unity.Mathematics;

public static class MyCustomType
{
    public static readonly byte[] bufferMaxTeamCount = new byte[4];
    public static readonly byte[] bufferMatchTime = new byte[4];
    public static readonly byte[] bufferBoardSize = new byte[8];

    // カスタムタイプを登録するメソッド（起動時に一度だけ呼び出す）
    public static void Register()
    {
        PhotonPeer.RegisterType(typeof(MaxTeamCount), 1, SerializeMaxTeamCount, DeserializeMaxTeamCount);
        PhotonPeer.RegisterType(typeof(MatchTime), 1, SerializeMatchTime, DeserializeMatchTime);
        PhotonPeer.RegisterType(typeof(BoardSize), 2, SerializeBoardSize, DeserializeBoardSize);
    }

    // バイト列に変換して送信データに書き込むメソッド
    private static short SerializeMaxTeamCount(StreamBuffer outStream, object customObject)
    {
        MaxTeamCount maxTeamCount = (MaxTeamCount)customObject;
        int index = 0;
        lock (bufferMaxTeamCount)
        {
            Protocol.Serialize(maxTeamCount.Value, bufferMaxTeamCount, ref index);
            outStream.Write(bufferMaxTeamCount, 0, index);
        }
        return (short)index; // 書き込んだバイト数を返す
    }

    // バイト列に変換して送信データに書き込むメソッド
    private static short SerializeMatchTime(StreamBuffer outStream, object customObject)
    {
        MatchTime matchTime = (MatchTime)customObject;
        int index = 0;
        lock (bufferMatchTime)
        {
            Protocol.Serialize(matchTime.Value, bufferMatchTime, ref index);
            outStream.Write(bufferMatchTime, 0, index);
        }
        return (short)index; // 書き込んだバイト数を返す
    }

    // バイト列に変換して送信データに書き込むメソッド
    private static short SerializeBoardSize(StreamBuffer outStream, object customObject)
    {
        BoardSize boardSize = (BoardSize)customObject;
        int index = 0;
        lock (bufferBoardSize)
        {
            Protocol.Serialize(boardSize.Value.x, bufferBoardSize, ref index);
            Protocol.Serialize(boardSize.Value.y, bufferBoardSize, ref index);
            outStream.Write(bufferBoardSize, 0, index);
        }
        return (short)index; // 書き込んだバイト数を返す
    }

    // 受信データからバイト列を読み込んで変換するメソッド
    private static object DeserializeMaxTeamCount(StreamBuffer inStream, short length)
    {
        int value;
        int index = 0;
        lock (bufferMaxTeamCount)
        {
            inStream.Read(bufferMaxTeamCount, 0, length);
            Protocol.Deserialize(out value, bufferMaxTeamCount, ref index);
        }
        return new MaxTeamCount { Value = value };
    }

    // 受信データからバイト列を読み込んで変換するメソッド
    private static object DeserializeMatchTime(StreamBuffer inStream, short length)
    {
        int value;
        int index = 0;
        lock (bufferMatchTime)
        {
            inStream.Read(bufferMatchTime, 0, length);
            Protocol.Deserialize(out value, bufferMatchTime, ref index);
        }
        return new MatchTime { Value = value };
    }

    // 受信データからバイト列を読み込んで変換するメソッド
    private static object DeserializeBoardSize(StreamBuffer inStream, short length)
    {
        int valueX;
        int valueY;
        int index = 0;
        lock (bufferBoardSize)
        {
            inStream.Read(bufferBoardSize, 0, length);
            Protocol.Deserialize(out valueX, bufferBoardSize, ref index);
            Protocol.Deserialize(out valueY, bufferBoardSize, ref index);
        }
        return new BoardSize { Value = new int2 { x = valueX, y = valueY } };
    }
}