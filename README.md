# training-csharp-unity

C#・Unity・設計思想を体系的に学ぶための学習リポジトリです。

---

## このリポジトリで学べること

- **C# 基礎〜中級**：型・クラス・継承・インターフェース・LINQ など
- **Unity 基礎**：C# スクリプトのアタッチ・デバッグ・コンポーネント設計
- **設計思想**：SOLID 原則・UML・代表的なデザインパターン
- **実践課題**：スペースシューターゲームの設計・実装・振り返り

### 対象読者

C# / Unity の経験を問わず使えるように構成しています。
他言語の経験がある方も、C# 特有の記法や Unity との組み合わせを学ぶ出発点として活用できます。

### 学習後のゴールイメージ

1. C# の基本構文とオブジェクト指向の概念を理解し、簡単なクラス設計ができる
2. Unity で C# スクリプトをアタッチして動作させ、Visual Studio でデバッグができる
3. SOLID やデザインパターンといった設計思想の概念を知り、コードレビューや設計議論で言葉として使える

---

## 必要な環境

| ツール | 推奨バージョン |
|---|---|
| Unity | ※運用者が記入してください |
| Visual Studio | 2022（UnityのC#開発用） |
| Git | 2.x 以上 |

環境構築の詳細な手順は [00_setup/environment.md](./00_setup/environment.md) を参照してください。

---

## クイックスタート

**1. リポジトリをクローンまたは fork する**

```bash
git clone https://github.com/Tomonorarari-Think/training-csharp-unity.git
cd training-csharp-unity
```

**2. 環境を整える**

[00_setup/environment.md](./00_setup/environment.md) の手順に沿って Unity・Visual Studio・Git の設定を行ってください。

**3. 学習を始める**

[01_csharp/](./01_csharp/) から順に読み進めてください。

---

## フォルダ構成

```
training-csharp-unity/
├── 00_setup/          — 環境構築・Git の基本操作
├── 01_csharp/         — C# 基礎〜中級（型・クラス・継承・LINQ）
├── 02_unity/          — Unity と C# の基礎・デバッグ方法
├── 03_design/         — 設計思想（SOLID・UML・デザインパターン）
├── 04_space-shooter/  — メイン課題（設計→実装→振り返り）
├── 05_reflection/     — 振り返り・記録の残し方の例
└── training-project/  — Unity プロジェクト本体
```

---

## Unity パッケージの導入方法

`02_unity/` のスクリプトを既存の Unity プロジェクトに取り込む場合、以下の 2 つの方法があります。

### UPM（git URL）で導入する

1. Unity エディタのメニューから **Window → Package Manager** を開く
2. 左上の **＋** ボタンをクリックし、**Add package from git URL...** を選択する
3. 以下の URL を入力して **Add** をクリックする

```
https://github.com/Tomonorarari-Think/training-csharp-unity.git?path=02_unity/Runtime
```

### .unitypackage でインポートする

1. [GitHub Releases](https://github.com/Tomonorarari-Think/training-csharp-unity/releases) ページから最新の `.unitypackage` をダウンロードする
2. Unity エディタのメニューから **Assets → Import Package → Custom Package...** を選択する
3. ダウンロードしたファイルを選択してインポートする

---

## 参考リンク

- [Unity 公式：C# 命名規則とコードスタイル](https://unity.com/ja/how-to/naming-and-code-style-tips-c-scripting-unity)
- [Unity 公式：設計思想 eBook（SOLID・デザインパターン）](https://unity.com/resources/design-patterns-solid-ebook)

詳細は各公式ドキュメントを参照してください。

---

## ライセンス

[MIT License](./LICENSE)
