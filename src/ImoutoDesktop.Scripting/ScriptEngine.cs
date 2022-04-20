﻿using System;
using System.IO;

namespace ImoutoDesktop.Scripting;

public class ScriptEngine : IDisposable
{
    public ScriptEngine(string path)
    {
        // 現在のディレクトリを保存
        _vm.RootDirectory = path;
        // 基礎設定ファイルの読み込み
        _vm.Settings.LoadSettings(Path.Combine(_vm.RootDirectory, "imouto.ini"));
        // エンコーディングを設定する
        _vm.Parser.Encoding = _vm.Settings.Encoding;
        _vm.Variables.Encoding = _vm.Settings.Encoding;
        // トレーサを開始
        _vm.StartTrace();
        // 辞書のパース
        foreach (var dictionary in _vm.Settings.Dictionaries)
        {
            _vm.Parser.ParseDictionary(Path.Combine(_vm.RootDirectory, dictionary), _vm.Functions);
        }
        // $OnVariableの呼び出し
        _vm.ExecFunction("OnVariable");
        // 変数のデシリアライズ
        _vm.Variables.Deserialize(Path.Combine(_vm.RootDirectory, "imouto_vars.txt"));
        // $OnConstantの呼び出し
        _vm.ExecFunction("OnConstant");
    }

    public string Invoke(string id, params string[] param)
    {
        _vm.Variables.Reject = false;
        _vm.Parameter = param;
        return _vm.ExecFunction(id);
    }

    public int Age
    {
        get => _vm.Variables.Age;
        set => _vm.Variables.Age = value;
    }

    public int TsundereLevel
    {
        get => _vm.Variables.TsundereLevel;
        set => _vm.Variables.TsundereLevel = value;
    }

    public bool Reject
    {
        get => _vm.Variables.Reject;
        set => _vm.Variables.Reject = value;
    }

    public bool AllowOperate
    {
        get => _vm.Variables.AllowRemoteOperate;
        set => _vm.Variables.AllowRemoteOperate = value;
    }

    public bool Connecting
    {
        get => _vm.Variables.Connecting;
        set => _vm.Variables.Connecting = value;
    }

    private readonly ExecutionContext _vm = new();

    #region IDisposable メンバ

    public void Dispose()
    {
        // 変数のシリアライズ
        _vm.Variables.Serialize(Path.Combine(_vm.RootDirectory, "imouto_vars.txt"));
        // トレーサを終了
        _vm.EndTrace();
    }

    #endregion
}
