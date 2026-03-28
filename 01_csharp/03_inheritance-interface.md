# 03. 継承・抽象クラス・インターフェース

---

## 1. 継承の概念

**継承**とは、既存のクラスの機能を引き継いで新しいクラスを作る仕組みです。
共通の処理を親クラスにまとめることで、重複した記述を減らせます。

たとえば `Player` と `Enemy` はどちらも「名前・HP・攻撃」という共通の特徴を持ちます。
これらを `Character` クラスにまとめ、`Player` と `Enemy` がそれを継承する設計にすると
共通部分を一箇所で管理できます。

```
Character（基底クラス）
  ├── Player（派生クラス）
  └── Enemy（派生クラス）
```

### is-a 関係

継承は「A は B である」（is-a）関係が成り立つ場合に使います。

- `Player` は `Character` である → 継承を使う ✅
- `Player` は `Weapon` を持つ → 継承を使わない（後述の has-a で扱う）❌

💡 C言語経験者向け：C言語には継承の仕組みがありません。
C言語で疑似的に実現しようとすると、関数ポインタを使った構造体の入れ子などが必要になりますが、
C# では言語の機能として継承が用意されています。

---

## 2. 継承の書き方

```csharp
// 基底クラス（親クラス）
class Character
{
    public string Name { get; private set; }
    public int    Hp   { get; protected set; }

    public Character(string name, int hp)
    {
        Name = name;
        Hp   = hp;
    }

    // virtual を付けると派生クラスで override できる
    public virtual void Attack()
    {
        Console.WriteLine($"{Name} が攻撃した！");
    }

    public void ShowStatus()
    {
        Console.WriteLine($"{Name}：HP {Hp}");
    }
}
```

```csharp
// 派生クラス（子クラス）：Character を継承
class Player : Character
{
    private int magicPoint;

    // base(...) で親クラスのコンストラクタを呼ぶ
    public Player(string name, int hp, int mp) : base(name, hp)
    {
        magicPoint = mp;
    }

    // override で Attack() を上書きする
    public override void Attack()
    {
        Console.WriteLine($"{Name} が剣で攻撃した！");
    }
}
```

```csharp
class Enemy : Character
{
    public Enemy(string name, int hp) : base(name, hp) { }

    public override void Attack()
    {
        base.Attack(); // 親クラスの Attack() も呼びつつ追加処理を書ける
        Console.WriteLine($"{Name} が牙をむいた！");
    }
}
```

```csharp
Player player = new Player("勇者", 100, 50);
Enemy  enemy  = new Enemy("スライム", 30);

player.Attack(); // 勇者 が剣で攻撃した！
enemy.Attack();  // スライム が攻撃した！ → スライム が牙をむいた！

player.ShowStatus(); // 親クラスのメソッドもそのまま使える
```

### sealed

`sealed` を付けると継承や override を禁止できます。

```csharp
// これ以上継承させたくないクラス
sealed class FinalBoss : Enemy
{
    public FinalBoss() : base("ラスボス", 9999) { }
}

// sealed メソッド：このクラスでの override を最後にする
public sealed override void Attack()
{
    Console.WriteLine("最終攻撃！");
}
```

---

## 3. 抽象クラス

**抽象クラス**は「インスタンスを直接作れないクラス」です。
派生クラスに対して「このメソッドは必ず実装すること」という契約として使います。

```csharp
// abstract クラス：インスタンスを直接 new できない
abstract class Character
{
    public string Name { get; protected set; }
    public int    Hp   { get; protected set; }

    public Character(string name, int hp)
    {
        Name = name;
        Hp   = hp;
    }

    // abstract メソッド：派生クラスでの override が必須
    public abstract void Attack();

    // abstract でないメソッドはそのまま使える
    public void ShowStatus()
    {
        Console.WriteLine($"{Name}：HP {Hp}");
    }
}
```

```csharp
class Player : Character
{
    public Player(string name, int hp) : base(name, hp) { }

    // abstract なので必ず実装しなければコンパイルエラーになる
    public override void Attack()
    {
        Console.WriteLine($"{Name} が攻撃した！");
    }
}
```

```csharp
Character c = new Character("...", 0); // コンパイルエラー：abstract クラスは new できない
Character p = new Player("勇者", 100); // OK：派生クラスとして生成できる
```

### virtual と abstract の違い

| | virtual | abstract |
|---|---|---|
| override | 任意 | 必須 |
| 実装 | 持てる | 持てない |
| インスタンス化 | 可能 | 不可能 |

---

## 4. インターフェース

**インターフェース**は「クラスが持つべき機能の契約」を定義する仕組みです。
クラスはインターフェースを実装することで「この機能を持っている」と宣言します。

### 抽象クラスとの違い

| | 抽象クラス | インターフェース |
|---|---|---|
| 多重実装 | 不可（1つだけ継承可能） | 可能（複数実装できる） |
| フィールド | 持てる | 持てない |
| 実装 | 持てる | 基本は持たない※ |

※ C# 8.0 以降はデフォルト実装が可能ですが、基本は「定義のみ」として使います。

### 命名規則

インターフェースの名前は先頭に `I` を付けます（例：`IDamageable`・`IMovable`）。
これは Unity 公式の C# 命名規則に準拠しています。
詳細は [Unity 公式 C# 命名規則](https://unity.com/ja/how-to/naming-and-code-style-tips-c-scripting-unity) を参照してください。

### インターフェースの定義と実装

```csharp
// インターフェース：TakeDamage を持つことを要求する
interface IDamageable
{
    void TakeDamage(int damage);
}
```

```csharp
// Player が IDamageable を実装する
class Player : Character, IDamageable
{
    public Player(string name, int hp) : base(name, hp) { }

    public override void Attack()
    {
        Console.WriteLine($"{Name} が攻撃した！");
    }

    // インターフェースで要求されたメソッドを実装する
    public void TakeDamage(int damage)
    {
        Hp -= damage;
        Console.WriteLine($"{Name} が {damage} ダメージを受けた（残り HP：{Hp}）");
    }
}
```

```csharp
// インターフェース型の変数に代入できる（型を気にせず操作できる）
IDamageable target = new Player("勇者", 100);
target.TakeDamage(30); // 勇者 が 30 ダメージを受けた（残り HP：70）
```

🎮 Unity での使用例：Unity には組み込みのインターフェースが多数あります。

```csharp
using UnityEngine;
using UnityEngine.EventSystems;

// IPointerClickHandler を実装すると UI クリックイベントを受け取れる
public class ClickableUI : MonoBehaviour, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("クリックされた！");
    }
}
```

---

## 5. is-a / has-a の使い分け

継承を使うかどうかは「A は B である」が自然かどうかで判断します。

### is-a（継承）

「A は B である」関係が成り立つ場合に継承を使います。

```csharp
// Player は Character である → 継承 ✅
class Player : Character { ... }
```

### has-a（コンポジション）

「A は B を持つ」関係の場合は、フィールドとして保持します。

```csharp
// Player は Weapon を持つ → フィールドとして持つ ✅
class Player : Character
{
    private Weapon weapon; // Weapon を継承するのではなく持つ

    public Player(string name, int hp, Weapon weapon) : base(name, hp)
    {
        this.weapon = weapon;
    }
}
```

### よくある間違い

「コードを再利用したい」という理由だけで is-a 関係がないのに継承を使うと
設計が壊れやすくなります。たとえば「Vehicle（乗り物）」を継承した「Engine（エンジン）」は
「Engine は Vehicle である」とは言えないため継承は不適切です。

> 設計思想については [03_design/](../03_design/) で詳しく扱います。
> 特に SOLID 原則の「リスコフの置換原則」が継承の正しい使い方に関わります。

---

## 6. 多重継承とインターフェースの多重実装

C# は**クラスの多重継承を禁止**しています。継承できる親クラスは1つだけです。

```csharp
class Player : Character, Enemy { ... } // コンパイルエラー
```

一方、**インターフェースは複数実装できます**。

```csharp
interface IMovable
{
    void Move(float x, float y);
}

interface IDamageable
{
    void TakeDamage(int damage);
}

// 複数のインターフェースを同時に実装できる
class Player : Character, IMovable, IDamageable
{
    public override void Attack() { ... }

    public void Move(float x, float y)
    {
        Console.WriteLine($"({x}, {y}) に移動した");
    }

    public void TakeDamage(int damage)
    {
        Hp -= damage;
    }
}
```

---

## 7. Unity における継承の実例

Unity の `MonoBehaviour` 自体が継承の仕組みで成り立っています。
`MonoBehaviour` を継承したクラスが GameObject にアタッチされることで、
`Start`・`Update` などのライフサイクルメソッドが自動で呼ばれます。

### 自作の基底クラスで複数の敵を管理する

```csharp
using UnityEngine;

// 全ての敵に共通する処理を基底クラスにまとめる
public abstract class EnemyBase : MonoBehaviour
{
    [SerializeField] protected int hp = 50;
    [SerializeField] protected float moveSpeed = 2.0f;

    // 全ての敵が必ず実装する処理（abstract）
    public abstract void Attack();

    // 共通処理はここに書く
    public void TakeDamage(int damage)
    {
        hp -= damage;
        if (hp <= 0) Die();
    }

    protected virtual void Die()
    {
        Debug.Log($"{gameObject.name} が倒れた");
        Destroy(gameObject);
    }
}
```

```csharp
// スライム：EnemyBase を継承して Attack を実装
public class SlimeEnemy : EnemyBase
{
    public override void Attack()
    {
        Debug.Log("スライムが体当たりした！");
    }
}
```

```csharp
// ボス：EnemyBase を継承して Attack と Die を独自実装
public class BossEnemy : EnemyBase
{
    public override void Attack()
    {
        Debug.Log("ボスが強力な攻撃をした！");
    }

    protected override void Die()
    {
        Debug.Log("ボスが倒れた！エンディングへ");
        // ボス固有の演出処理
        base.Die(); // 親クラスの Die() も呼ぶ
    }
}
```

---

## 8. よくあるハマりどころまとめ

### base() の呼び忘れ

親クラスのコンストラクタに引数がある場合、`base(...)` を呼ばないとコンパイルエラーになります。

```csharp
class Player : Character
{
    public Player(string name, int hp) : base(name, hp) { } // base() が必要
}
```

### virtual を付け忘れて override できない

```csharp
class Character
{
    public void Attack() { ... } // virtual がない
}

class Player : Character
{
    public override void Attack() { ... } // コンパイルエラー
}
```

### インターフェースのメソッドを実装し忘れる

```csharp
class Player : Character, IDamageable
{
    // TakeDamage を実装していない → コンパイルエラー
}
```

### MonoBehaviour を継承したクラスをさらに継承する場合

`MonoBehaviour` を継承したクラスはコンストラクタではなく `Awake` / `Start` で初期化します。
コンストラクタは Unity のライフサイクル外で呼ばれるため、意図しない動作の原因になります。

```csharp
// 悪い例
public class EnemyBase : MonoBehaviour
{
    public EnemyBase()
    {
        // ここで Unity の機能（GetComponent など）を使うと問題が起きる
    }
}

// 良い例
public class EnemyBase : MonoBehaviour
{
    protected virtual void Awake()
    {
        // Unity のライフサイクル内で初期化する
    }
}
```

---

## 9. 参考リンク

- [Microsoft C# ドキュメント：継承](https://learn.microsoft.com/ja-jp/dotnet/csharp/fundamentals/object-oriented/inheritance)
- [Microsoft C# ドキュメント：インターフェース](https://learn.microsoft.com/ja-jp/dotnet/csharp/fundamentals/types/interfaces)
- [Unity 公式 C# 命名規則（インターフェースの命名）](https://unity.com/ja/how-to/naming-and-code-style-tips-c-scripting-unity)
