# 05. 非同期処理（コルーチン・Awaitable・UniTask）

[02_monobehaviour.md](./02_monobehaviour.md) のコルーチン基礎を前提とした発展的な内容です。

---

## 1. なぜ非同期処理が必要か

Unity はメインスレッドで動作しており、重い処理を同期的に実行すると
その間ゲームの画面更新が止まり「フリーズ」したように見えます。

**非同期処理が必要になる典型的な場面：**

- 一定時間待ってから処理を実行したい（例：3秒後に爆発）
- Web API・ファイルの読み込みを待つ（例：セーブデータのロード）
- 演出を時間をかけて行いたい（例：フェードイン・アウト）
- 複数の処理を並行して実行したい（例：BGM フェードと敵の登場アニメーションを同時に）

---

## 2. コルーチンの限界

コルーチンは Unity の全バージョンで使える非同期処理の仕組みです。
まず基本構文を確認します。

```csharp
using System.Collections;
using UnityEngine;

public class Example : MonoBehaviour
{
    void Start()
    {
        StartCoroutine(WaitAndFire());
    }

    IEnumerator WaitAndFire()
    {
        yield return new WaitForSeconds(3.0f); // 3秒待つ
        Fire();
    }

    void Fire() { Debug.Log("発射！"); }
}
```

シンプルな待機処理には十分ですが、以下の限界があります。

| 限界 | 説明 |
|---|---|
| 戻り値を返せない | `IEnumerator` のため計算結果を呼び出し元に返せない |
| 例外処理が書きにくい | `yield return` をまたいだ `try-catch` が正しく動作しない |
| 並行待ちが複雑 | 複数のコルーチンが全部終わるまで待つ処理を書くのが難しい |
| MonoBehaviour 依存 | `StartCoroutine` は MonoBehaviour のメソッドのため非 MonoBehaviour クラスから使いにくい |
| キャンセルが難しい | `StopCoroutine` のタイミング管理が煩雑になりやすい |

これらの限界を解消するため、**Unity 2023.1 以降では async/await が推奨**されています。

---

## 3. Awaitable（Unity 2023.1〜）

### 3-1. Awaitable とは

`Awaitable` は Unity 2023.1 から標準搭載された Unity 専用の async/await 対応クラスです。
外部パッケージ不要で使えます。
Unity の PlayerLoop と統合されており、コルーチンの代替として設計されています。

> ⚠️ **Unity 2023.1 未満**
> `Awaitable` クラスは使用できません。
> コルーチンまたは UniTask を使用してください。

### 3-2. 基本構文（コルーチンとの対比）

```csharp
// ===== コルーチン版 =====
IEnumerator WaitAndFireCoroutine()
{
    yield return new WaitForSeconds(3.0f);
    Fire();
}

// 呼び出し方
StartCoroutine(WaitAndFireCoroutine());


// ===== Awaitable 版 =====
async Awaitable WaitAndFireAsync()
{
    await Awaitable.WaitForSecondsAsync(3.0f);
    Fire();
}

// 呼び出し方（Start や Update から直接 await できる）
async void Start()
{
    await WaitAndFireAsync();
}
```

### 3-3. よく使う Awaitable のメソッド

| メソッド | 動作 |
|---|---|
| `Awaitable.NextFrameAsync()` | 次のフレームまで待つ |
| `Awaitable.WaitForSecondsAsync(秒)` | 指定秒数待つ |
| `Awaitable.EndOfFrameAsync()` | フレーム末まで待つ |
| `Awaitable.FixedUpdateAsync()` | 次の FixedUpdate まで待つ |

### 3-4. キャンセル（CancellationToken）

async 処理のキャンセルには `CancellationToken` を使います。
MonoBehaviour の `destroyCancellationToken` を渡すと、
**GameObject が破棄されたときに自動でキャンセル**されます。

```csharp
using UnityEngine;

public class PlayerShooter : MonoBehaviour
{
    async void Start()
    {
        // destroyCancellationToken を渡すと、この GameObject が破棄されたとき自動キャンセル
        await ChargeAndFireAsync(destroyCancellationToken);
    }

    async Awaitable ChargeAndFireAsync(CancellationToken ct)
    {
        Debug.Log("チャージ開始");

        // キャンセルトークンを渡す
        await Awaitable.WaitForSecondsAsync(3.0f, ct);

        // ここは GameObject が生きている場合のみ実行される
        Debug.Log("発射！");
    }
}
```

### 3-5. Awaitable の制限

`Awaitable` には複数タスクを並行して待つ `WhenAll` に相当する機能がありません。
「2つのアニメーションが両方終わったら次へ進む」のような処理が必要な場合は
後述の **UniTask** を使ってください。

---

## 4. UniTask

### 4-1. UniTask とは

**UniTask** は [Cysharp](https://github.com/Cysharp/UniTask) が開発した OSS の非同期処理ライブラリです。
「Unity 向けにゼロアロケーションで動作する async/await 統合ライブラリ」として設計されており、
`Awaitable` の設計にも影響を与えた上位互換的な存在です。

実務プロジェクトでは事実上の標準として使われているライブラリのひとつです。

> ⚠️ UniTask は **外部ライブラリ**です。プロジェクトへの追加が必要です。

### 4-2. インストール手順

**方法①：UPM（git URL）でインストール**

1. `Window → Package Manager` を開きます
2. 左上の「+」→「Add package from git URL」を選択します
3. 以下の URL を入力します

```
https://github.com/Cysharp/UniTask.git?path=src/UniTask/Assets/Plugins/UniTask
```

4. 「Add」ボタンをクリックします

**方法②：.unitypackage でインストール**

1. [UniTask リリースページ](https://github.com/Cysharp/UniTask/releases) から最新の `.unitypackage` をダウンロードします
2. Unity エディタを開いた状態でファイルをダブルクリックします
3. Import Unity Package ダイアログで「All」→「Import」をクリックします

### 4-3. 基本的な使い方

```csharp
using Cysharp.Threading.Tasks; // 必須
using UnityEngine;

public class PlayerShooter : MonoBehaviour
{
    async UniTaskVoid Start()
    {
        await WaitAndFireAsync();
    }

    async UniTask WaitAndFireAsync()
    {
        Debug.Log("チャージ開始");
        await UniTask.Delay(3000); // ミリ秒単位（3000ms = 3秒）
        Debug.Log("発射！");
    }
}
```

**よく使う UniTask のメソッド：**

| メソッド | 動作 |
|---|---|
| `UniTask.Delay(ミリ秒)` | 指定ミリ秒待つ |
| `UniTask.DelayFrame(フレーム数)` | 指定フレーム数待つ |
| `UniTask.Yield()` | 次のフレームまで待つ |
| `UniTask.WhenAll(タスク...)` | 複数タスクがすべて完了するまで待つ |
| `UniTask.WhenAny(タスク...)` | いずれかのタスクが完了したら進む |

### 4-4. WhenAll の使い方

`WhenAll` は Awaitable にはない UniTask の強力な機能です。
複数の非同期処理を**並行して実行し、すべて完了するまで待つ**ことができます。

```csharp
using Cysharp.Threading.Tasks;
using UnityEngine;

public class StageDirector : MonoBehaviour
{
    async UniTaskVoid Start()
    {
        Debug.Log("ステージ演出を開始");

        // 敵の登場アニメーションと BGM フェードインを並行して実行する
        await UniTask.WhenAll(
            PlayEnemyAppearAnimationAsync(),
            FadeInBGMAsync()
        );

        // 両方の処理が完了してからゲームを開始する
        Debug.Log("ゲームスタート！");
        StartGame();
    }

    async UniTask PlayEnemyAppearAnimationAsync()
    {
        Debug.Log("敵が登場中...");
        await UniTask.Delay(2000); // 2秒の登場アニメーション
        Debug.Log("敵の登場完了");
    }

    async UniTask FadeInBGMAsync()
    {
        Debug.Log("BGM フェードイン中...");
        await UniTask.Delay(3000); // 3秒かけてフェードイン
        Debug.Log("BGM フェードイン完了");
    }

    void StartGame() { }
}
```

### 4-5. .Forget() について

非同期処理を「起動して結果を待たない」場合は `.Forget()` を明示的に呼びます。

```csharp
// 発射処理を起動して、完了を待たずに次の処理に進む
FireAsync().Forget();

// async UniTaskVoid でも同様のことができる（内部で Forget 相当の処理をする）
async UniTaskVoid FireAndForget()
{
    await UniTask.Delay(100);
    Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
}
```

### 4-6. UniTask でのキャンセル

`destroyCancellationToken` との組み合わせは Awaitable と同様に使えます。

```csharp
async UniTask ChargeAndFireAsync(CancellationToken ct)
{
    await UniTask.Delay(3000, cancellationToken: ct);
    Fire();
}

async UniTaskVoid Start()
{
    // GameObject が破棄されたら自動キャンセル
    await ChargeAndFireAsync(destroyCancellationToken);
}
```

---

## 5. 3つの手法の使い分けまとめ

| 手法 | 対応バージョン | 特徴 | 推奨する場面 |
|---|---|---|---|
| **コルーチン** | 全バージョン | シンプル・追加不要・MonoBehaviour 依存 | 既存コードの保守・簡単な待機処理 |
| **Awaitable** | Unity 2023.1〜 | 標準搭載・外部依存なし・WhenAll 非対応 | 新規プロジェクトで外部ライブラリを避けたい場合 |
| **UniTask** | Unity 2018.4〜（外部パッケージ） | 高機能・ゼロアロケーション・WhenAll 対応 | 本格的な非同期処理・並行処理が必要な場合 |

**判断フローチャート：**

```
新規プロジェクト？
├── Yes → 外部ライブラリを使える？
│         ├── Yes → UniTask 推奨
│         └── No  → Awaitable（Unity 2023.1〜）
│                   ※ Unity 2022 以前はコルーチンか UniTask
└── No（既存プロジェクト）
          → そのプロジェクトの方針に従う
            （コルーチンが使われていれば無理に変えない）
```

---

## 6. よくあるハマりどころ

### async void は使わない

`async void` のメソッドは例外を捕捉できないため、Unity 開発では使いません。

```csharp
// 悪い例：例外が捕捉できない
async void StartAsync() { await UniTask.Delay(1000); }

// 良い例（UniTask）
async UniTaskVoid StartAsync() { await UniTask.Delay(1000); }

// 良い例（Awaitable）
async Awaitable StartAsync() { await Awaitable.WaitForSecondsAsync(1.0f); }
```

### UniTask を await せずに放置するとメモリリークの可能性

```csharp
// 悪い例：UniTask を捨てている（警告が出る）
FireAsync();

// 良い例：.Forget() を明示的に呼ぶ
FireAsync().Forget();
```

### CancellationToken を渡し忘れる

GameObject が破棄された後も非同期処理が続いていると
`MissingReferenceException` が発生します。
`destroyCancellationToken` を必ず渡す習慣をつけてください。

### コルーチンと UniTask を混在させる

同じプロジェクト内でコルーチンと UniTask を混在させると
コードの一貫性が失われ、読みにくくなります。
プロジェクトの方針として使う手法を統一することを推奨します。

---

## 7. 参考リンク

- [Unity 公式マニュアル：Awaitable クラス](https://docs.unity3d.com/6000.0/Documentation/Manual/AwaitableClass.html)
- [UniTask GitHub](https://github.com/Cysharp/UniTask)
- [UniTask リリースページ（.unitypackage ダウンロード）](https://github.com/Cysharp/UniTask/releases)
