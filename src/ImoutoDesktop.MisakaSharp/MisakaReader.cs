using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace ImoutoDesktop.MisakaSharp
{
    /// <summary>
    /// 美坂暗号化の種類。
    /// </summary>
    enum CryptType
    {
        /// <summary>
        /// 暗号化なし。
        /// </summary>
        None,
        /// <summary>
        /// オリジナルの美坂互換暗号化。
        /// </summary>
        Original,
        /// <summary>
        /// 美坂標準暗号化。
        /// </summary>
        Standard,
    }

    /// <summary>
    /// 美坂暗号化されているファイルを読み込みます。
    /// </summary>
    class MisakaReader : IDisposable
    {
        /// <summary>
        /// 指定されたファイルパスと文字エンコーディングに基づき、インスタンスを初期化します。
        /// </summary>
        /// <param name="path">読み込むファイルパス。</param>
        /// <param name="encoding">読み込むファイルの文字エンコーディング。</param>
        public MisakaReader(string path, Encoding encoding)
        {
            this.encoding = encoding;
            // ファイルストリームを作成
            baseStream = File.Open(path, FileMode.Open);
            // 暗号化ファイルかどうかを判別
            string extension = Path.GetExtension(path);
            switch (extension)
            {
                case ".__1":
                    cryptType = CryptType.Original;
                    binaryReader = new BinaryReader(baseStream, encoding);
                    break;
                case ".___":
                    cryptType = CryptType.Standard;
                    cryptKey = (byte)(baseStream.Length % 255);
                    binaryReader = new BinaryReader(baseStream, encoding);
                    break;
                default:
                    cryptType = CryptType.None;
                    streamReader = new StreamReader(baseStream, encoding);
                    break;
            }
        }

        private int currentLine;

        /// <summary>
        /// 現在読み込んでいる行数を取得します。
        /// </summary>
        public int CurrentLine
        {
            get { return currentLine; }
        }

        private Stream baseStream;
        private Encoding encoding;
        private byte cryptKey;
        private CryptType cryptType;
        private BinaryReader binaryReader;
        private StreamReader streamReader;

        public void Dispose()
        {
            switch (cryptType)
            {
                case CryptType.None:
                    streamReader.Dispose();
                    break;
                case CryptType.Original:
                case CryptType.Standard:
                    ((IDisposable)binaryReader).Dispose();
                    break;
            }
            baseStream.Dispose();
        }

        /// <summary>
        /// 読み取り可能な次の文字列を返します。
        /// </summary>
        /// <returns></returns>
        public int Peek()
        {
            switch (cryptType)
            {
                case CryptType.None:
                    return streamReader.Peek();
                case CryptType.Original:
                case CryptType.Standard:
                    return binaryReader.PeekChar();
            }
            return -1;
        }

        /// <summary>
        /// 1行分の文字列を読み込みます。
        /// </summary>
        /// <returns></returns>
        public string ReadLine()
        {
            ++currentLine;
            switch (cryptType)
            {
                case CryptType.None:
                    return streamReader.ReadLine();
                case CryptType.Original:
                    {
                        List<byte> bytes = new List<byte>();
                        while (true)
                        {
                            byte c = (byte)((int)binaryReader.ReadByte() ^ 0xff);
                            if (c == '\n')
                            {
                                break;
                            }
                            bytes.Add(c);
                        }
                        return encoding.GetString(bytes.ToArray());
                    }
                case CryptType.Standard:
                    {
                        List<byte> bytes = new List<byte>();
                        while (true)
                        {
                            byte c = (byte)((int)binaryReader.ReadByte() ^ cryptKey);
                            // 復号キーの更新
                            cryptKey = c;
                            // 改行の検出
                            if (c == '\n')
                            {
                                break;
                            }
                            bytes.Add(c);
                        }
                        return encoding.GetString(bytes.ToArray());
                    }
            }
            return string.Empty;
        }
    }
}
