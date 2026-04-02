# binpatchwin

Windows用バイナリパッチ作成・適用ツール

2つのファイルの差分（パッチ）を作成し、元ファイルに適用して復元するGUIアプリケーションです。
パッチファイル単体からは復元先ファイルを再構築できないため、正規ユーザーのみがアップデートを適用できます。

## 特徴

- **差分のみを配布** - 修正版ファイル全体を配布する必要がありません
- **セキュリティ** - XOR差分方式により、パッチファイル単体では元ファイルも復元先ファイルも再構築不可能
- **ハッシュ検証** - SHA-256で元ファイルの整合性を自動検証
- **簡単操作** - ドラッグ&ドロップ対応のGUI

## ダウンロード

[Releases](https://github.com/sudosanet/binpatchwin/releases) からダウンロードしてください。

| ファイル | サイズ | 説明 |
|---------|--------|------|
| `binpatchwin.exe` | 約200KB | 軽量版（.NET 10 ランタイムが必要） |
| `binpatchwin-self-contained.exe` | 約110MB | ランタイム同梱版（インストール不要） |

## 動作環境

- Windows 10 / 11 (x64)

### .NET 10 ランタイムのインストール（軽量版を使う場合）

軽量版 (`binpatchwin.exe`) を使用するには .NET 10 デスクトップランタイムが必要です。
ランタイム同梱版 (`binpatchwin-self-contained.exe`) を使う場合はインストール不要です。

以下のリンクからインストーラーをダウンロードし、実行してください。

[.NET 10 デスクトップランタイム (x64) をダウンロード](https://dotnet.microsoft.com/ja-jp/download/dotnet/thank-you/runtime-desktop-10.0.5-windows-x64-installer)

## 使い方

### パッチ作成（開発者向け）

1. 「パッチ作成」タブを開く
2. 元ファイル (A) をドラッグ&ドロップまたは「参照」で選択
3. 変更後ファイル (B) をドラッグ&ドロップまたは「参照」で選択
4. 「パッチ作成」ボタンをクリック
5. 元ファイルと同じ場所に `.bpat` ファイルが生成されます

### パッチ適用（ユーザー向け）

1. 「パッチ適用」タブを開く
2. 元ファイル (A) をドラッグ&ドロップまたは「参照」で選択
3. パッチファイル (.bpat) をドラッグ&ドロップまたは「参照」で選択
4. 「パッチ適用」ボタンをクリック
5. 確認ダイアログで「はい」を選択すると元ファイルが上書きされます

## パッチファイル形式 (.bpat)

| オフセット | サイズ | フィールド |
|-----------|--------|-----------|
| 0x00 | 4 bytes | マジックナンバー: `BPAT` |
| 0x04 | 2 bytes | バージョン: `0x0001` |
| 0x06 | 32 bytes | 元ファイルの SHA-256 ハッシュ |
| 0x26 | 8 bytes | 元ファイルサイズ |
| 0x2E | 8 bytes | 変更後ファイルサイズ |
| 0x36 | 4 bytes | デルタレコード数 |
| 0x3A | 可変 | デルタレコード群 |

各デルタレコードはオフセット (8 bytes) + データ長 (4 bytes) + XOR差分データで構成されます。

## ビルド

### 必要環境

- .NET 10 SDK

### ビルド・テスト

```bash
dotnet build
dotnet test
```

### 単一ファイル発行

```bash
# 軽量版（ランタイム必要）
dotnet publish binpatchwin.csproj -c Release -r win-x64 --no-self-contained -p:PublishSingleFile=true -o publish

# ランタイム同梱版
dotnet publish binpatchwin.csproj -c Release -r win-x64 --self-contained -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true -o publish
```

## ライセンス

MIT
