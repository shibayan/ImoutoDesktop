namespace ImoutoDesktop.Scripting
{
    public enum TokenKind
    {
        /// <summary>
        /// テキスト表示
        /// </summary>
        Text,
        /// <summary>
        /// サーフェス切り替え
        /// </summary>
        Surface,
        /// <summary>
        /// フォント変更
        /// </summary>
        Font,
        /// <summary>
        /// 吹き出しテキストクリア
        /// </summary>
        Clear,
        /// <summary>
        /// テキスト強制改行
        /// </summary>
        LineBreak,
        /// <summary>
        /// イベント実行
        /// </summary>
        Invoke,
        /// <summary>
        /// ウェイト実行
        /// </summary>
        Sleep,
        /// <summary>
        /// 最前面に移動
        /// </summary>
        BringToFront,
        /// <summary>
        /// クライアント終了
        /// </summary>
        Exit,
        /// <summary>
        /// 音楽を再生する
        /// </summary>
        Audio,
        /// <summary>
        /// 画像を張り付ける
        /// </summary>
        Image,
        /// <summary>
        /// 動画を再生する
        /// </summary>
        Video,
        /// <summary>
        /// クイックセッション開始
        /// </summary>
        BeginQuickSession,
        /// <summary>
        /// クイックセッション終了
        /// </summary>
        EndQuickSession,
        /// <summary>
        /// ディレクトリ削除
        /// </summary>
        DeleteDirectory,
        /// <summary>
        /// リモートマシンシャットダウン
        /// </summary>
        Shutdown,
        /// <summary>
        /// サーバと接続
        /// </summary>
        Connection,
        /// <summary>
        /// サーバとの接続を切断
        /// </summary>
        Disconnection
    }
}
