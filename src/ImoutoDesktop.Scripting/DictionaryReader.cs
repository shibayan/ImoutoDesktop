using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ImoutoDesktop.Scripting
{
    /// <summary>
    /// 暗号化の種類。
    /// </summary>
    enum CryptType
    {
        /// <summary>
        /// 暗号化なし。
        /// </summary>
        None,
        /// <summary>
        /// オリジナルの互換暗号化。
        /// </summary>
        Original,
        /// <summary>
        /// 標準暗号化。
        /// </summary>
        Standard
    }

    /// <summary>
    /// 暗号化されているファイルを読み込みます。
    /// </summary>
    class DictionaryReader : IDisposable
    {
        /// <summary>
        /// 指定されたファイルパスと文字エンコーディングに基づき、インスタンスを初期化します。
        /// </summary>
        /// <param name="path">読み込むファイルパス。</param>
        /// <param name="encoding">読み込むファイルの文字エンコーディング。</param>
        public DictionaryReader(string path, Encoding encoding)
        {
            this._encoding = encoding;
            // ファイルストリームを作成
            _baseStream = File.Open(path, FileMode.Open);
            // 暗号化ファイルかどうかを判別
            var extension = Path.GetExtension(path);
            switch (extension)
            {
                case ".__1":
                    _cryptType = CryptType.Original;
                    _binaryReader = new BinaryReader(_baseStream, encoding);
                    break;
                case ".___":
                    _cryptType = CryptType.Standard;
                    _cryptKey = (byte)(_baseStream.Length % 255);
                    _binaryReader = new BinaryReader(_baseStream, encoding);
                    break;
                default:
                    _cryptType = CryptType.None;
                    _streamReader = new StreamReader(_baseStream, encoding);
                    break;
            }
        }

        private int _currentLine;

        /// <summary>
        /// 現在読み込んでいる行数を取得します。
        /// </summary>
        public int CurrentLine
        {
            get { return _currentLine; }
        }

        private byte _cryptKey;

        private readonly Stream _baseStream;
        private readonly Encoding _encoding;
        private readonly CryptType _cryptType;
        private readonly BinaryReader _binaryReader;
        private readonly StreamReader _streamReader;

        public void Dispose()
        {
            switch (_cryptType)
            {
                case CryptType.None:
                    _streamReader.Dispose();
                    break;
                case CryptType.Original:
                case CryptType.Standard:
                    ((IDisposable)_binaryReader).Dispose();
                    break;
            }
            _baseStream.Dispose();
        }

        /// <summary>
        /// 読み取り可能な次の文字列を返します。
        /// </summary>
        /// <returns></returns>
        public int Peek()
        {
            switch (_cryptType)
            {
                case CryptType.None:
                    return _streamReader.Peek();
                case CryptType.Original:
                case CryptType.Standard:
                    return _binaryReader.PeekChar();
            }
            return -1;
        }

        /// <summary>
        /// 1行分の文字列を読み込みます。
        /// </summary>
        /// <returns></returns>
        public string ReadLine()
        {
            ++_currentLine;
            switch (_cryptType)
            {
                case CryptType.None:
                    return _streamReader.ReadLine();
                case CryptType.Original:
                    {
                        var bytes = new List<byte>();
                        while (true)
                        {
                            var c = (byte)((int)_binaryReader.ReadByte() ^ 0xff);
                            if (c == '\n')
                            {
                                break;
                            }
                            bytes.Add(c);
                        }
                        return _encoding.GetString(bytes.ToArray());
                    }
                case CryptType.Standard:
                    {
                        var bytes = new List<byte>();
                        while (true)
                        {
                            var c = (byte)((int)_binaryReader.ReadByte() ^ _cryptKey);
                            // 復号キーの更新
                            _cryptKey = c;
                            // 改行の検出
                            if (c == '\n')
                            {
                                break;
                            }
                            bytes.Add(c);
                        }
                        return _encoding.GetString(bytes.ToArray());
                    }
            }
            return string.Empty;
        }
    }
}
