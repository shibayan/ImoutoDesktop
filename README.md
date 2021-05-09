# いもうとデスクトップ <img align="right" src="https://user-images.githubusercontent.com/1356444/117574881-40d68a00-b11a-11eb-870f-5eb373959501.png">

[![Build](https://github.com/shibayan/ImoutoDesktop/actions/workflows/build.yml/badge.svg)](https://github.com/shibayan/ImoutoDesktop/actions/workflows/build.yml)
[![Release](https://img.shields.io/github/release/shibayan/ImoutoDesktop.svg?include_prereleases&sort=semver)](https://github.com/shibayan/ImoutoDesktop/releases/latest)
[![License](https://img.shields.io/github/license/shibayan/ImoutoDesktop.svg)](https://github.com/shibayan/ImoutoDesktop/blob/master/LICENSE)

## はじめに

「いもうとデスクトップ」はデスクトップ右下に常駐した妹キャラに文字入力でさまざまなお願いをして、他 PC を操作してもらえる対話型のリモート操作ソフトです。

プレビュー版では動かない機能がまだ多いですが、技術的負債を返済しながら対応していきます。

元ネタ : [窓の社 - 【NEWS】妹がリモートPCを操作する対話型の遠隔操作ソフト「いもうとデスクトップ」](https://forest.watch.impress.co.jp/yashiro/2006/imoutodesktop.html)

## 動作環境

- クライアント - Windows 10
- サーバ - Windows 10

## 使用条件

実妹がいない男子の個人利用に限り無償で使用可能。

## インストール

適当なフォルダに展開するだけです。

## アンインストール

レジストリなどは使用していないので、展開したファイルをすべて削除してください。

## 使い方

### サーバ側

`ImoutoDesktop.Server.exe` を実行するだけです。プレビュー版は gRPC の Insecure Channel として動作します。

### クライアント側

`ImoutoDesktop.exe` を実行すると、デフォルトのいもうとである「さくら」が立ち上がるので「さくら」を右クリックで表示されるメニューから、「機能」->「オプション」からサーバのアドレスやポート番号、パスワードを入力し、OK をクリックすると設定が保存されます。

設定保存後はコマンド入力ボックスに「接続」を入力すると接続が開始されます。

接続が完了すると吹き出しとコマンド入力ウィンドウが表示されます。このあとはコマンド入力で、リモート PC の操作が可能です。

## 実行可能なコマンド

### 「～を実行」

カレントディレクトリに存在するファイルを実行。ただし接続先の PC で実行される。

### 「～を表示/開く」

カレントディレクトリに存在するファイルを表示。ただし接続先の PC で表示される。

### 「～へ移動」

カレントディレクトリを変更する。カレントディレクトリは DOS コマンドの cd と共通。

～の部分には「ドキュメント」「マイドキュメント」「ミュージック」「マイミュージック」「ピクチャ」「マイピクチャ」「デスクトップ」「お気に入り」を指定することも出来る。

絶対パスと相対パスも指定可能。

### 「～を削除」

カレントディレクトリに存在するファイルを削除する。

### 「接続」

サーバと接続されていない場合接続する。

### 「切断」

サーバと接続されている場合切断する。

## 実行可能なコマンド

dir, ver, mkdir, rmdir, copy, del, move, ren, cd, type, ls, chdir, rm, cp, start

## 免責事項

本ソフトウェアを使用することで発生したいかなる損害についても、ソフトウェア制作者は一切責任を負いません。

## 著作権

本ソフトウェア、ならびに本ソフトウェアに関係するすべてのファイルの著作権はソフトウェア制作者に帰属します。

## 謝辞

いもうとデスクトップを開発する上で「プログラム技術＠2ch掲示板」で職人の方々に数多くの機能提案、改善点、デバッグなどの協力や「さくら」「かえで」「すみれ」の立ち絵や性格などすべて開発していただきました。この場を借りてお礼申し上げます。

[いもうとデスクトップを実際に作ってみないか？3](http://pc11.2ch.net/test/read.cgi/tech/1210054407/)
