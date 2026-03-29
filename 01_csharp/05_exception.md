# 05 — 例外処理（try-catch-finally）

## 目標

- 例外とは何かを説明できる
- try-catch-finally を使ったエラーハンドリングを書ける
- throw で例外を送出できる
- Unity での適切な例外処理の使い方を理解する

---

## 1. 例外とは

**例外（Exception）** とは、プログラムの実行中に発生した「想定外の問題」のことです。

例として：

- 配列の範囲外にアクセスした
- null のオブジェクトのメソッドを呼んだ
- ファイルが見つからなかった
- 文字列を数値に変換しようとして失敗した

例外処理を書かないと、例外が発生した時点でプログラムがクラッシュして止まります。`try-catch-finally` を使うことで、問題が起きても適切に対処してプログラムを継続させることができます。

> 💡 **C 言語経験者へ**
>
> C 言語にはネイティブな例外処理の仕組みがありません（`setjmp` / `longjmp` を使う方法はありますが一般的ではありません）。
> C# の `try-catch` は Java の `try-catch` とほぼ同じ概念です。

---

## 2. try-catch-finally の基本構文

### 基本形

```csharp
try
{
    // 例外が発生する可能性のある処理
    int[] numbers = { 1, 2, 3 };
    Console.WriteLine(numbers[10]); // 範囲外アクセス → 例外が発生する
}
catch (Exception e)
{
    // 例外が発生したときの処理
    Console.WriteLine($"エラーが発生しました: {e.Message}");
    Console.WriteLine(e.StackTrace); // どこで発生したかのスタックトレース
}
finally
{
    // 例外の有無に関わらず必ず実行される処理
    // ファイルのクローズ・リソースの解放などに使う
    Console.WriteLine("finally は必ず実行される");
}
```

### 例外の種類を指定して catch する

複数の `catch` ブロックを並べることで、例外の種類に応じた処理を書けます。

```csharp
try
{
    string input = "abc";
    int value = int.Parse(input);
    int result = 10 / value;
}
catch (FormatException e)
{
    // 数値への変換失敗
    Console.WriteLine($"フォーマットエラー: {e.Message}");
}
catch (DivideByZeroException e)
{
    // 0 除算
    Console.WriteLine($"ゼロ除算エラー: {e.Message}");
}
catch (Exception e)
{
    // 上記以外のすべての例外（最後に書く）
    Console.WriteLine($"予期しないエラー: {e.Message}");
}
```

> **重要：** より具体的な例外クラスを先に書いてください。
> `Exception` を先に書くとすべての例外がそこで捕捉されてしまい、後の `catch` ブロックが到達不能になります。

### finally の省略

`finally` はリソースの解放が不要な場合は省略できます。

```csharp
try
{
    int value = int.Parse("42");
    Console.WriteLine(value);
}
catch (FormatException e)
{
    Console.WriteLine($"変換失敗: {e.Message}");
}
// finally なしでも問題ない
```

---

## 3. よく使う例外クラス

| 例外クラス | 発生する場面 |
|---|---|
| `NullReferenceException` | null のオブジェクトにアクセスした |
| `IndexOutOfRangeException` | 配列の範囲外にアクセスした |
| `ArgumentNullException` | null を渡してはいけない引数に null を渡した |
| `ArgumentException` | 引数が不正な値だった |
| `InvalidOperationException` | 操作が無効な状態で呼ばれた |
| `DivideByZeroException` | 0 で除算した |
| `IOException` | ファイルの読み書きで問題が発生した |
| `FormatException` | 文字列の形式が不正だった（`Parse` 失敗など） |

これらはすべて `System.Exception` を継承しているため、`catch (Exception e)` で一括補足できます。ただし、一括補足は基本的に推奨しません（後述）。

---

## 4. throw による例外の送出

### 新しい例外を送出する

```csharp
void SetHp(int value)
{
    if (value < 0)
    {
        throw new ArgumentException($"HP は 0 以上である必要があります。value={value}");
    }
    hp = value;
}
```

呼び出し元では `catch` で受け取れます。

```csharp
try
{
    SetHp(-10);
}
catch (ArgumentException e)
{
    Console.WriteLine($"引数エラー: {e.Message}");
}
```

### catch した例外を再送出する

```csharp
try
{
    // なんらかの処理
}
catch (Exception e)
{
    Debug.LogError("ログを記録してから再送出");
    throw; // ← 元の例外をそのまま上位に伝える（推奨）
    // throw e; は書かない（後述）
}
```

> **`throw` と `throw e` の違い**
>
> - `throw;` — スタックトレースを保持したまま例外を再送出する（推奨）
> - `throw e;` — スタックトレースがリセットされ、この行が例外の発生元に見える（非推奨）

---

## 5. 独自例外クラスの定義

`Exception` を継承することで、ゲームロジック上の「あってはならない状態」を型として表現できます。

```csharp
// 最小構成の独自例外クラス
class PlayerDeadException : Exception
{
    public PlayerDeadException(string message)
        : base(message) { }
}

// 送出側
void TakeDamage(int damage)
{
    hp -= damage;
    if (hp <= 0)
    {
        throw new PlayerDeadException($"プレイヤーが死亡しました。最終ダメージ={damage}");
    }
}

// 受け取る側
try
{
    player.TakeDamage(999);
}
catch (PlayerDeadException e)
{
    Console.WriteLine($"ゲームオーバー: {e.Message}");
    // ゲームオーバー処理…
}
```

独自例外を定義する利点は、汎用的な `Exception` ではなく意味のある型名で catch できることです。コードの可読性とデバッグ効率が向上します。

---

## 6. 🎮 Unity での使用例

### int.Parse() の失敗を try-catch で処理する

ユーザー入力など、変換が保証できない場合に使います。

```csharp
using UnityEngine;

public class ScoreParser : MonoBehaviour
{
    void ParseScore(string input)
    {
        try
        {
            int value = int.Parse(input);
            Debug.Log($"スコア: {value}");
        }
        catch (FormatException e)
        {
            Debug.LogError($"変換失敗: {e.Message}");
        }
    }
}
```

> **ヒント：** 変換が失敗してもクラッシュさせたくない場合は `int.TryParse()` の方がシンプルに書けます。
>
> ```csharp
> if (int.TryParse(input, out int value))
> {
>     Debug.Log($"スコア: {value}");
> }
> else
> {
>     Debug.LogWarning("変換できませんでした");
> }
> ```

### null チェックと例外処理の使い分け

Unity では `NullReferenceException` を `try-catch` で握りつぶすより、**事前の null チェック**を推奨します。

```csharp
// NG：try-catch で null を握りつぶす
try
{
    enemy.TakeDamage(10);
}
catch (NullReferenceException)
{
    // なぜ null なのかわからないまま無視してしまう
}

// OK：null チェックで事前に対処する
if (enemy != null)
{
    enemy.TakeDamage(10);
}
else
{
    Debug.LogWarning("enemy が null です");
}
```

**理由：**

- `try-catch` はパフォーマンスコストが高い（特に例外が頻発する場合）
- `NullReferenceException` を握りつぶすとバグの発見が遅れる
- null になった原因を特定しにくくなる

### Debug.LogException() の使い方

`catch` ブロックで例外を Unity Console にスタックトレース付きで出力できます。

```csharp
try
{
    // 例外が発生する可能性のある処理
    LoadData();
}
catch (Exception e)
{
    // スタックトレース付きで Unity Console に出力する
    Debug.LogException(e);
    // Debug.LogError(e.Message) よりもスタックトレースが見やすい
}
```

`Debug.LogException(e)` を使うと、Console ウィンドウでクリックしてスタックトレースを確認できるためデバッグがしやすくなります。

---

## 7. よくあるハマりどころ

### catch (Exception e) だけ書いてすべてを握りつぶす

```csharp
// NG：原因がわからなくなる
try
{
    // 何かの処理
}
catch (Exception)
{
    // 何もしない（サイレント失敗）
}
```

例外をすべて握りつぶすと、バグがあっても気づけなくなります。最低でもログを出力してください。

### finally を使わずにリソースを解放し忘れる

```csharp
// NG：例外が発生するとファイルが閉じられない
StreamReader reader = new StreamReader("data.txt");
string content = reader.ReadToEnd(); // 例外が発生すると reader.Close() が呼ばれない
reader.Close();

// OK：finally で確実に解放する
StreamReader reader = null;
try
{
    reader = new StreamReader("data.txt");
    string content = reader.ReadToEnd();
}
finally
{
    reader?.Close(); // 例外の有無に関わらず Close される
}

// さらに良い：using 文（Dispose を自動で呼ぶ）
using (StreamReader reader = new StreamReader("data.txt"))
{
    string content = reader.ReadToEnd();
} // ブロックを抜けると自動で Dispose（Close）される
```

### throw e; と throw; の違い

```csharp
catch (Exception e)
{
    throw e; // NG：スタックトレースがこの行にリセットされる
    throw;   // OK：元のスタックトレースを保持したまま再送出
}
```

### Unity で NullReferenceException が頻発する場合

`try-catch` ではなく、原因を調査して null チェックで対処してください。
Unity の `GetComponent<T>()` は対象が見つからなければ null を返します。

```csharp
var rb = GetComponent<Rigidbody2D>();
if (rb == null)
{
    Debug.LogError("Rigidbody2D が見つかりません。GameObject に追加してください。");
    return;
}
rb.velocity = Vector2.up;
```

---

## 8. 参考リンク

- [Microsoft C# ドキュメント — 例外処理](https://learn.microsoft.com/ja-jp/dotnet/csharp/fundamentals/exceptions/)
- [本リポジトリ 01_csharp/02_classes.md](02_classes.md)
- [本リポジトリ 02_unity/04_debugging.md](../02_unity/04_debugging.md) — デバッガーでの例外確認方法
