# 02. クラス・オブジェクト・メソッド・プロパティ

---

## 1. クラスとオブジェクトの概念

**クラス**は「設計図」で、**オブジェクト**はその設計図から作られた「実体」です。

たとえば `Player` というクラスを設計図として定義しておくと、
その設計図から `player1`・`player2` という別々の実体（インスタンス）を何個でも生成できます。
それぞれの実体は独立した名前・HP などの値を持ちます。

```
Player クラス（設計図）
  ├── player1（HP: 100、名前: "勇者"）
  └── player2（HP:  80、名前: "魔法使い"）
```

💡 C言語経験者向け：C言語の `struct` に近い概念ですが、
データ（フィールド）だけでなく、処理（メソッド）も一緒に持てる点が大きく異なります。

---

## 2. クラスの基本構造

```csharp
// クラスの宣言
class Player
{
    // フィールド（メンバー変数）
    string name;
    int    hp;

    // コンストラクタ（インスタンス生成時に呼ばれる初期化処理）
    public Player(string name, int hp)
    {
        this.name = name;
        this.hp   = hp;
    }

    // メソッド（処理）
    public void TakeDamage(int damage)
    {
        hp -= damage;
        if (hp < 0) hp = 0;
    }

    public void ShowStatus()
    {
        Console.WriteLine($"{name} の HP：{hp}");
    }
}
```

### インスタンスの生成（new キーワード）

```csharp
// new を使ってインスタンスを生成する
Player player1 = new Player("勇者", 100);
Player player2 = new Player("魔法使い", 80);

player1.TakeDamage(30);
player1.ShowStatus(); // 勇者 の HP：70

player2.ShowStatus(); // 魔法使い の HP：80（別インスタンスなので影響を受けない）
```

---

## 3. アクセス修飾子

フィールドやメソッドへのアクセス範囲をコントロールします。

| 修飾子 | アクセスできる範囲 |
|---|---|
| `public` | どこからでもアクセス可能 |
| `private` | クラス内からのみアクセス可能 |
| `protected` | クラス内と派生クラスからアクセス可能 |
| `internal` | 同じアセンブリ（プロジェクト）内からアクセス可能 |

```csharp
class Player
{
    public  string name;   // どこからでも読み書き可能
    private int    hp;     // クラス内からのみアクセス可能
    protected int  level;  // 派生クラスからもアクセス可能
}
```

原則として、フィールドは `private` にして外部から直接変更できないようにします。
必要な場合だけ `public` メソッドやプロパティ経由でアクセスさせる設計が基本です。

### 🎮 Unity での使用例：SerializeField

Unity では `public` フィールドは Inspector に表示されますが、
`public` を多用すると外部から意図せず変更されるリスクがあります。
`[SerializeField]` 属性を使うと、`private` のまま Inspector に表示できます。

```csharp
using UnityEngine;

class PlayerController : MonoBehaviour
{
    [SerializeField] private float moveSpeed  = 5.0f; // Inspector に表示される
    [SerializeField] private int   maxHp      = 100;  // Inspector に表示される
    private int currentHp; // Inspector には表示されない（内部管理用）
}
```

---

## 4. プロパティ

フィールドを直接公開する代わりに、プロパティ経由でアクセスすることで
「値の検証」や「読み取り専用化」などを実現できます。

💡 C言語経験者向け：getter 関数と setter 関数をひとつの構文にまとめたような仕組みです。

### get / set の書き方

```csharp
class Player
{
    private int hp;

    public int Hp
    {
        get { return hp; }          // 値を返す
        set
        {
            if (value < 0) value = 0; // 0 未満にならないよう検証
            hp = value;
        }
    }
}
```

```csharp
Player player = new Player();
player.Hp = 100;  // set が呼ばれる
int h = player.Hp; // get が呼ばれる
player.Hp = -10;   // 自動で 0 に補正される
```

### 自動実装プロパティ

検証が不要な場合は短く書けます。

```csharp
class Player
{
    public string Name { get; set; }    // 読み書き可能
    public int    Score { get; set; } = 0; // 初期値付き
}
```

### 読み取り専用プロパティ

```csharp
class Player
{
    private int hp = 100;
    public int Hp { get { return hp; } } // get のみ（外部から変更不可）

    // 式形式でも書ける（C# 6 以降）
    public bool IsDead => hp <= 0;
}
```

---

## 5. static メンバー

`static` を付けると、インスタンスを生成しなくても使えるフィールドやメソッドを定義できます。
また、すべてのインスタンスで値が共有されます。

```csharp
class GameManager
{
    public static int TotalScore = 0; // 全インスタンスで共有される

    public static void AddScore(int amount)
    {
        TotalScore += amount;
    }
}
```

```csharp
GameManager.AddScore(100); // インスタンス不要で呼び出せる
Console.WriteLine(GameManager.TotalScore); // 100
```

🎮 Unity での使用例：Unity 組み込みの `Mathf` クラスのメソッドも static です。

```csharp
float distance = Mathf.Abs(-5.0f);    // 絶対値：5.0
float clamped  = Mathf.Clamp(hp, 0, 100); // hp を 0〜100 に収める
```

### よくあるハマりどころ

`static` フィールドはすべてのインスタンスで共有されます。
片方のインスタンスが値を変えると、もう片方にも影響します。

```csharp
class Counter
{
    public static int count = 0;
}

Counter a = new Counter();
Counter b = new Counter();
Counter.count = 10;

// a も b も同じ count を参照している
// Counter.count は 10（意図しない共有に注意）
```

---

## 6. this キーワード

`this` は「自分自身のインスタンス」を指します。
フィールド名とパラメータ名が同じ場合に区別するために使います。

```csharp
class Player
{
    private string name;
    private int    hp;

    public Player(string name, int hp)
    {
        this.name = name; // this.name はフィールド、name はパラメータ
        this.hp   = hp;
    }
}
```

`this` を省略できる場合は省略で構いませんが、
名前が被る場面では明示的に使うと可読性が上がります。

---

## 7. メソッドの詳細

### 戻り値のないメソッド（void）

```csharp
public void TakeDamage(int damage)
{
    hp -= damage;
}
```

### 戻り値のあるメソッド

```csharp
public bool IsAlive()
{
    return hp > 0;
}
```

### 引数のデフォルト値

引数を省略した場合に使われる初期値を設定できます。

```csharp
public void TakeDamage(int damage, bool showLog = true)
{
    hp -= damage;
    if (showLog)
    {
        Console.WriteLine($"{damage} ダメージを受けた");
    }
}
```

```csharp
player.TakeDamage(20);        // showLog = true（デフォルト）
player.TakeDamage(20, false); // ログを出さない
```

### メソッドのオーバーロード

同じ名前で引数の異なるメソッドを複数定義できます。

```csharp
class Player
{
    public void Attack()
    {
        Console.WriteLine("通常攻撃");
    }

    public void Attack(int damage)
    {
        Console.WriteLine($"{damage} ダメージの攻撃");
    }

    public void Attack(string skillName, int damage)
    {
        Console.WriteLine($"{skillName}：{damage} ダメージ");
    }
}
```

```csharp
player.Attack();               // 通常攻撃
player.Attack(50);             // 50 ダメージの攻撃
player.Attack("ファイア", 80); // ファイア：80 ダメージ
```

---

## 8. Unity とクラスの関係

### MonoBehaviour

Unity のスクリプトは `MonoBehaviour` を継承したクラスとして作成します。
`MonoBehaviour` は Unity が提供する基底クラスで、
これを継承することで GameObject にアタッチして動作させられます。

```csharp
using UnityEngine;

// MonoBehaviour を継承することで GameObject にアタッチできる
public class PlayerController : MonoBehaviour
{
    // Start はシーン開始時に1回呼ばれる
    void Start()
    {
        Debug.Log("ゲームスタート");
    }

    // Update は毎フレーム呼ばれる
    void Update()
    {
        // 毎フレームの処理を書く
    }
}
```

### Inspector でフィールドが表示される仕組み

Unity Inspector には以下の条件を満たすフィールドが表示されます：

- `public` フィールド
- `[SerializeField]` 属性を付けた `private` フィールド

Inspector から値を変更することで、コードを書き換えずにパラメータ調整ができます。

```csharp
public class EnemyController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 3.0f;  // Inspector で調整可能
    [SerializeField] private int   hp        = 50;    // Inspector で調整可能
    private bool isDead = false;                       // Inspector には出ない
}
```

---

## 9. よくあるハマりどころまとめ

### new を忘れてインスタンスを生成しない

```csharp
Player player; // 宣言だけで null のまま
player.TakeDamage(10); // NullReferenceException が発生する

// 正しくは new でインスタンスを生成する
Player player = new Player("勇者", 100);
```

### static と non-static の混在エラー

```csharp
class Counter
{
    private int count = 0;

    public static void Increment()
    {
        count++; // エラー：static メソッドから非 static フィールドにアクセスできない
    }
}
```

static メソッドからは static メンバーにしかアクセスできません。

### Unity で MonoBehaviour を new で生成しようとする

`MonoBehaviour` を継承したクラスは `new` で生成できません。

```csharp
PlayerController pc = new PlayerController(); // エラーになる
```

Unity のコンポーネントは `AddComponent` で追加します。

```csharp
PlayerController pc = gameObject.AddComponent<PlayerController>();
```

### コンストラクタの引数の順番を間違える

```csharp
// コンストラクタ：Player(string name, int hp)
Player player = new Player(100, "勇者"); // 順番が逆でコンパイルエラーになる
Player player = new Player("勇者", 100); // 正しい順番
```

---

## 10. 参考リンク

- [Microsoft C# ドキュメント：クラス](https://learn.microsoft.com/ja-jp/dotnet/csharp/fundamentals/types/classes)
- [Microsoft C# ドキュメント：プロパティ](https://learn.microsoft.com/ja-jp/dotnet/csharp/programming-guide/classes-and-structs/properties)
