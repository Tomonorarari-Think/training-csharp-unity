# 02_unity — Unity 基礎

## このフォルダで学べること

- Scene・GameObject・Component の基本概念と操作方法
- MonoBehaviour のライフサイクル（Awake / Start / Update / FixedUpdate など）
- Rigidbody・Collider・Input System を使った物理演算と入力処理の基礎
- Visual Studio と Unity を連携させたデバッグ方法
- 非同期処理（コルーチン・Awaitable・UniTask）の使い分け

---

## 読み進め方

以下の順番で学習することを推奨します。

1. [01_scene-gameobject.md](01_scene-gameobject.md) — Scene・GameObject・Component
2. [02_monobehaviour.md](02_monobehaviour.md) — ライフサイクル・コルーチン基礎
3. [03_physics-input.md](03_physics-input.md) — 物理演算・入力処理
4. [04_debugging.md](04_debugging.md) — Visual Studio デバッグ
5. [05_async.md](05_async.md) — 非同期処理（Awaitable・UniTask）

---

## サンプルスクリプトの動作確認

`Runtime/Scripts/` には各ドキュメントに対応するサンプルスクリプトが含まれています。
Unity プロジェクトにインポートして動作を確認しながら学習を進めてください。

インポート方法は以下の2通りです。

### 方法A：UPM（git URL）でインポート

常に最新版を取得したい場合に適した方法です。

1. Unity の **Window → Package Manager** を開きます。
2. 左上の「**+**」ボタンをクリックします。
3. 「**Add package from git URL**」を選択します。
4. 以下の URL を入力します。

   ```
   https://github.com/Tomonorarari-Think/training-csharp-unity.git?path=02_unity
   ```

5. 「**Add**」ボタンを押します。
6. インポート完了後、**Packages/C# Unity 研修パッケージ/Runtime/Scripts/** にスクリプトが追加されていることを確認します。

### 方法B：.unitypackage でインポート

git を使わずに導入したい場合や、バージョンを固定して使いたい場合に適した方法です。

1. 以下の GitHub Releases ページにアクセスします。

   ```
   https://github.com/Tomonorarari-Think/training-csharp-unity/releases
   ```

2. 最新リリースから **training-csharp-unity.unitypackage** をダウンロードします。
3. Unity エディタを開いた状態で、ダウンロードした `.unitypackage` をダブルクリックします。
4. **Import Unity Package** ダイアログで「**All**」→「**Import**」ボタンを押します。
5. **Assets/** 以下にスクリプトが追加されていることを確認します。

### 方法A・B の使い分け

| 方法 | 向いている場面 |
|---|---|
| UPM（git URL） | 常に最新版を使いたい場合 |
| .unitypackage | git を使わずに導入したい場合・バージョンを固定したい場合 |

---

## 動作確認の手順

インポート後、以下の手順でサンプルスクリプトの動作を確認できます。

1. Unity で新しい空の Scene を開きます。
2. Hierarchy ウィンドウで右クリック →「**Create Empty**」で空の GameObject を作成します。
3. 確認したいサンプルスクリプトを GameObject にアタッチします。
4. **Play** ボタンを押して、Console ウィンドウへの出力を確認します。
5. 詳細な動作確認は各 `.md` ドキュメントを参照してください。

---

## Unity バージョンについて

> **推奨 Unity バージョン：**（運用者が記入してください）

- [05_async.md](05_async.md) で紹介している `Awaitable` クラスは **Unity 2023.1 以降** でのみ使用できます。それ以前のバージョンではコルーチンまたは UniTask を使用してください。
- **Unity 6 以降**では `FindObjectOfType` など一部の API が非推奨となり、警告が表示される場合があります。代替 API については各ドキュメントの「バージョン別 API 変更まとめ」を参照してください。

---

## 参考リンク

- [Unity 公式マニュアル](https://docs.unity3d.com/ja/current/Manual/index.html)
- [Unity 公式 C# 命名規則](https://unity.com/ja/how-to/naming-and-code-style-tips-c-scripting-unity)
