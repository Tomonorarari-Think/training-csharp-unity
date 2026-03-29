# 06 — デリゲート・イベント・Action/Func

## 目標

- デリゲートとは何かを説明できる
- `Action` / `Func` を使って処理を変数・引数として渡せる
- ラムダ式を使って簡潔にデリゲートを書ける
- `event` を使った Observer パターンの基礎を理解する
- Unity での C# event と UnityEvent の使い分けができる

---

## 1. デリゲートとは

**デリゲート（delegate）** とは、「メソッドを変数として扱う仕組み」です。

メソッドを別のメソッドに渡したり、後から呼び出せるようにするための仕組みです。コールバック・イベント・戦略パターンなどに幅広く使われます。

> 💡 **C 言語経験者へ**
>
> C 言語の **関数ポインタ** に相当する概念です。
> ただし C# のデリゲートは型安全であり、インスタンスメソッドも参照できます。

---

## 2. delegate の基本構文

### delegate 型の宣言とメソッドの代入

```csharp
// delegate 型を宣言する（戻り値なし・int 引数1つ）
delegate void AttackDelegate(int damage);

class Player
{
    void SwordAttack(int damage)
    {
        Console.WriteLine($"剣攻撃！ダメージ: {damage}");
    }

    void MagicAttack(int damage)
    {
        Console.WriteLine($"魔法攻撃！ダメージ: {damage}");
    }

    void Example()
    {
        // delegate 変数にメソッドを代入する
        AttackDelegate attack = SwordAttack;

        // delegate を呼び出す
        attack(30); // → 剣攻撃！ダメージ: 30

        // 別のメソッドに切り替え
        attack = MagicAttack;
        attack(50); // → 魔法攻撃！ダメージ: 50
    }
}
```

### マルチキャストデリゲート

`+=` で複数のメソッドを登録すると、呼び出し時にすべて実行されます。

```csharp
AttackDelegate attack = SwordAttack;
attack += MagicAttack; // 2つ目を追加

attack(20);
// → 剣攻撃！ダメージ: 20
// → 魔法攻撃！ダメージ: 20

attack -= SwordAttack; // 登録解除
```

### null チェック

デリゲート変数が null のまま呼び出すと `NullReferenceException` が発生します。`?.Invoke()` を使うと null の場合に安全にスキップできます。

```csharp
// NG：null の場合にクラッシュする
attack(10);

// OK：null なら何もしない
attack?.Invoke(10);
```

---

## 3. Action と Func

`delegate` をその都度宣言するのは手間がかかります。.NET には「戻り値なし」「戻り値あり」の2種類の汎用デリゲート型が用意されています。

### Action（戻り値なし）

```csharp
Action greet = () => Console.WriteLine("こんにちは");
greet(); // → こんにちは

Action<string> greetName = name => Console.WriteLine($"こんにちは、{name}");
greetName("田中"); // → こんにちは、田中

Action<string, int> showScore = (name, score) =>
    Console.WriteLine($"{name}: {score}点");
showScore("田中", 85); // → 田中: 85点
```

### Func（戻り値あり）

**最後の型引数が戻り値の型** です。

```csharp
Func<int> getScore = () => 100;          // 引数なし・int を返す
int s = getScore();                       // s = 100

Func<int, int> double_ = x => x * 2;    // int を受け取り int を返す
int d = double_(5);                       // d = 10

Func<int, int, int> add = (x, y) => x + y; // int 2つを受け取り int を返す
int sum = add(3, 4);                     // sum = 7

Func<string, bool> isLong = s => s.Length > 5; // string を受け取り bool を返す
```

### Action / Func / delegate の使い分け

| 状況 | 使う型 |
|---|---|
| 戻り値なし | `Action` / `Action<T>` |
| 戻り値あり | `Func<TResult>` / `Func<T, TResult>` |
| 特定のシグネチャを型として明示したい | `delegate` |

実際のコードでは `Action` と `Func` が最もよく使われます。`delegate` を明示的に宣言するのは、型に意味のある名前を付けたい場合などに限られます。

---

## 4. ラムダ式

**ラムダ式** とは「その場で定義する匿名メソッド」です。`=>` 演算子で引数と処理を結びます。

```csharp
// 引数なし
Action greet = () => Console.WriteLine("こんにちは");

// 引数1つ（括弧は省略可）
Action<string> greetName = name => Console.WriteLine($"こんにちは、{name}");

// 引数2つ以上（括弧が必要）
Action<int, int> printSum = (x, y) => Console.WriteLine(x + y);

// 戻り値あり
Func<int, int> double_ = x => x * 2;
Func<int, int, int> add = (x, y) => x + y;

// 複数行の処理（ブロックラムダ）
Func<int, string> classify = score =>
{
    if (score >= 80) return "優";
    if (score >= 60) return "良";
    return "可";
};
```

LINQ でもラムダ式を使います。

```csharp
List<int> scores = new List<int> { 45, 82, 60, 91 };
List<int> passed = scores.Where(s => s >= 60).ToList();
```

---

## 5. event キーワード

`event` は「デリゲートに制限を付けた特殊な形」です。

`event` を使うと外部クラスから直接呼び出すことができなくなり、**購読（`+=`）と解除（`-=`）のみが許可**されます。これにより「イベントの発火は発行者だけが行う」という設計を型レベルで保証できます。

Observer パターンの実装に使われます。
→ [03_design/04_design-patterns.md](../03_design/04_design-patterns.md)（Observer パターンの設計例）

### event の定義・購読・解除・発火

```csharp
using System;

// イベント発行者（Publisher）
class Player
{
    // event を定義する（Action は引数なし版）
    public event Action OnPlayerDied;

    int hp = 100;

    public void TakeDamage(int damage)
    {
        hp -= damage;
        if (hp <= 0)
        {
            // イベントを発火する（購読者がいる場合のみ）
            OnPlayerDied?.Invoke();
        }
    }
}

// イベント購読者（Subscriber）
class GameManager
{
    Player player;

    void Start()
    {
        player = new Player();

        // イベントを購読する
        player.OnPlayerDied += HandlePlayerDied;
    }

    void HandlePlayerDied()
    {
        Console.WriteLine("ゲームオーバー！");
    }

    void Stop()
    {
        // イベントを解除する（メモリリーク防止）
        player.OnPlayerDied -= HandlePlayerDied;
    }
}
```

> **外部からの直接呼び出しは禁止されます**
>
> ```csharp
> player.OnPlayerDied();         // コンパイルエラー（event の外部発火は不可）
> player.OnPlayerDied?.Invoke(); // コンパイルエラー（同上）
> ```

### Unity での OnEnable / OnDisable パターン

```csharp
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] private Player player;

    void OnEnable()
    {
        // シーン有効化時に購読する
        player.OnPlayerDied += ShowGameOverUI;
    }

    void OnDisable()
    {
        // シーン無効化時に解除する（必ずセットで書く）
        player.OnPlayerDied -= ShowGameOverUI;
    }

    void ShowGameOverUI()
    {
        Debug.Log("ゲームオーバー UI を表示");
    }
}
```

`OnEnable` で購読・`OnDisable` で解除をセットで書くのが Unity での定石です。

---

## 6. 🎮 Unity での使用例

### C# event と UnityEvent の比較

Unity には `UnityEvent` という独自のイベント機構も用意されています。

| 項目 | C# event | UnityEvent |
|---|---|---|
| 設定方法 | コードで `+=` 購読 | Inspector からドラッグ＆ドロップで設定可 |
| パフォーマンス | 高速 | やや低速（リフレクションを使う） |
| デバッグ | やや難（コードを読む必要あり） | Inspector で購読状況を確認可 |
| 推奨場面 | コード完結・高頻度なイベント | 設定を柔軟に変えたい・デザイナーが触る場合 |

→ [02_unity/03_physics-input.md](../02_unity/03_physics-input.md)（UnityEvent の具体的な使い方）

### UnityEvent の定義例

```csharp
using UnityEngine;
using UnityEngine.Events;

public class EventExample : MonoBehaviour
{
    // C# event
    public event Action OnDead;

    // UnityEvent（Inspector から設定可能）
    public UnityEvent OnJump;

    // 引数付き UnityEvent
    public UnityEvent<int> OnDamaged;

    void Die()
    {
        OnDead?.Invoke();
        OnJump?.Invoke();
        OnDamaged?.Invoke(30);
    }
}
```

---

## 7. よくあるハマりどころ

### OnDisable/OnDestroy でイベントの購読を解除し忘れる

購読したまま GameObject が破棄されると、存在しないオブジェクトへの参照が残り続けてメモリリークや予期しないエラーの原因になります。

```csharp
// OnEnable で += したら OnDisable で -= する（必ずセット）
void OnEnable()  { player.OnPlayerDied += HandlePlayerDied; }
void OnDisable() { player.OnPlayerDied -= HandlePlayerDied; }
```

### マルチキャストデリゲートで null チェックを忘れる

```csharp
// NG：null の場合にクラッシュする
onDead();

// OK：?.Invoke() を使う
onDead?.Invoke();
```

### event と通常の delegate のアクセス制限の違い

```csharp
public Action onDead;    // 外部から onDead() と直接呼び出せてしまう
public event Action onDead; // 外部からの直接呼び出しを禁止できる
```

外部から発火させたくないイベントは `event` を使うことで設計を型レベルで守れます。

### Func の型引数の順番を間違える

`Func` の **最後の型引数が戻り値の型** です。

```csharp
Func<int, string>     // int を受け取って string を返す
Func<int, int, bool>  // int 2つを受け取って bool を返す（最後が戻り値）
```

---

## 8. 参考リンク

- [Microsoft C# ドキュメント — デリゲート](https://learn.microsoft.com/ja-jp/dotnet/csharp/programming-guide/delegates/)
- [Microsoft C# ドキュメント — イベント](https://learn.microsoft.com/ja-jp/dotnet/csharp/programming-guide/events/)
- [本リポジトリ 03_design/04_design-patterns.md](../03_design/04_design-patterns.md) — Observer パターンの実装例
- [本リポジトリ 02_unity/03_physics-input.md](../02_unity/03_physics-input.md) — UnityEvent の使い方
