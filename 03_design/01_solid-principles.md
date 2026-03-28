# 01 — SOLID 原則

## 1. SOLID 原則とは

SOLID 原則は、オブジェクト指向設計における5つの指針の総称です。
ソフトウェアエンジニアの Robert C. Martin（通称 Uncle Bob）によって提唱されました。

コードは小さいうちはシンプルに書けますが、機能が増えるにつれて
「ちょっと直すだけのつもりが別の場所まで壊れた」
「新しい機能を追加しようとしたら既存コードを大量に修正しなければならなかった」
といった問題が起きやすくなります。
SOLID 原則は、こうした「変更しにくいコード」を防ぐための設計指針です。

| 頭文字 | 原則名（英語） | 原則名（日本語） |
|---|---|---|
| **S** | Single Responsibility Principle | 単一責任の原則 |
| **O** | Open/Closed Principle | 開放閉鎖の原則 |
| **L** | Liskov Substitution Principle | リスコフの置換原則 |
| **I** | Interface Segregation Principle | インターフェース分離の原則 |
| **D** | Dependency Inversion Principle | 依存性逆転の原則 |

---

## 2. S — 単一責任の原則

**「クラスは1つの責任だけを持つべき」**

クラスを変更する理由が1つだけになるよう設計します。
責任が1つであれば、ある機能を変更したときに別の機能を壊すリスクが下がります。

```csharp
// ❌ 悪い例：Player クラスが何でも担当している
public class Player : MonoBehaviour
{
    private int hp = 100;
    private int score = 0;

    // 移動処理
    void Update()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        transform.position += new Vector3(h, v, 0) * 5.0f * Time.deltaTime;
    }

    // ダメージ処理
    public void TakeDamage(int amount)
    {
        hp -= amount;
        if (hp <= 0)
        {
            Debug.Log("ゲームオーバー");
            // ゲームオーバー処理...
        }
    }

    // スコア管理
    public void AddScore(int points)
    {
        score += points;
        Debug.Log($"スコア：{score}");
        // セーブ処理...
    }

    // ログ出力
    public void LogStatus()
    {
        Debug.Log($"HP：{hp}、スコア：{score}、位置：{transform.position}");
    }
}
```

```csharp
// ✅ 良い例：責任をクラスに分離する
// 移動だけを担当するクラス
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5.0f;

    void Update()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        transform.position += new Vector3(h, v, 0) * moveSpeed * Time.deltaTime;
    }
}

// HP管理だけを担当するクラス
public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private int maxHp = 100;
    private int currentHp;

    void Awake() => currentHp = maxHp;

    public void TakeDamage(int amount)
    {
        currentHp -= amount;
        if (currentHp <= 0)
            OnDead();
    }

    private void OnDead()
    {
        Debug.Log("ゲームオーバー");
    }
}

// スコア管理だけを担当するクラス
public class ScoreManager : MonoBehaviour
{
    private int score = 0;

    public void AddScore(int points)
    {
        score += points;
        Debug.Log($"スコア：{score}");
    }
}
```

> **🎮 スペースシューターでの活用例：**
> プレイヤーの「移動（PlayerMovement）」「弾の発射（PlayerShooter）」「HP管理（PlayerHealth）」を
> それぞれ別クラスに分けることで、弾の発射ロジックを変更しても移動やHPに影響しない設計になります。

> **よくある誤解：**
> 「クラスを細かく分けすぎると管理が大変では？」という疑問は自然です。
> 「責任」とはメソッドの数ではなく、**変更理由の数**です。
> 「移動速度の調整」と「ダメージ計算の変更」は異なる変更理由なので、別クラスに分けるのが適切です。

---

## 3. O — 開放閉鎖の原則

**「クラスは拡張に対して開いており、変更に対して閉じているべき」**

新しい機能を追加するとき、既存のコードを修正しなくて済む設計を目指します。
既存コードを触るほど、動いていた部分を壊すリスクが上がります。

```csharp
// ❌ 悪い例：敵の種類を switch で分岐している
public class EnemyManager : MonoBehaviour
{
    public void Attack(string enemyType)
    {
        // 新しい敵を追加するたびにここを修正しなければならない
        switch (enemyType)
        {
            case "Slime":
                Debug.Log("スライムが体当たりで攻撃！");
                break;
            case "Goblin":
                Debug.Log("ゴブリンが剣で攻撃！");
                break;
            case "Boss":
                Debug.Log("ボスがビームで攻撃！");
                break;
            // 新しい敵を追加 → このクラスを毎回修正する必要がある
        }
    }
}
```

```csharp
// ✅ 良い例：抽象クラスを継承して拡張する
// 敵の共通インターフェースを定義する（変更しない）
public abstract class EnemyBase : MonoBehaviour
{
    public abstract void Attack();
}

// 各敵クラスを追加するだけでよい（既存コードを変更しない）
public class SlimeEnemy : EnemyBase
{
    public override void Attack()
    {
        Debug.Log("スライムが体当たりで攻撃！");
    }
}

public class GoblinEnemy : EnemyBase
{
    public override void Attack()
    {
        Debug.Log("ゴブリンが剣で攻撃！");
    }
}

public class BossEnemy : EnemyBase
{
    public override void Attack()
    {
        Debug.Log("ボスがビームで攻撃！");
    }
}

// 新しい DragonEnemy を追加しても EnemyBase も他の敵クラスも変更不要
public class DragonEnemy : EnemyBase
{
    public override void Attack()
    {
        Debug.Log("ドラゴンが炎を吐いて攻撃！");
    }
}
```

> **🎮 スペースシューターでの活用例：**
> 新しい敵機を追加するとき、`EnemyBase` を継承した新しいクラスを作るだけで済みます。
> 既存の敵クラスや EnemyManager を修正する必要がなくなります。

---

## 4. L — リスコフの置換原則

**「派生クラスは基底クラスと置き換えても正しく動作するべき」**

継承したクラスは、親クラスの「約束（契約）」を守らなければなりません。
親クラスを使っているコードが、派生クラスに差し替えても同じように動くことが条件です。

```csharp
// ❌ 悪い例：親クラスの約束を破っている
public class Bird
{
    public virtual void Fly()
    {
        Debug.Log("飛んでいます");
    }
}

// ペンギンは Bird を継承しているが、飛べない
public class Penguin : Bird
{
    public override void Fly()
    {
        // 飛べないので例外を投げる → 親の約束を破っている
        throw new System.NotSupportedException("ペンギンは飛べません");
    }
}

// Bird として渡したつもりが Penguin だと例外が起きる
public void MakeBirdFly(Bird bird)
{
    bird.Fly(); // Penguin が渡されると例外が発生してしまう
}
```

```csharp
// ✅ 良い例：飛べる能力をインターフェースで分離する
public interface IFlyable
{
    void Fly();
}

// 飛べる鳥だけ IFlyable を実装する
public class Sparrow : MonoBehaviour, IFlyable
{
    public void Fly()
    {
        Debug.Log("スズメが飛んでいます");
    }
}

// ペンギンは IFlyable を実装しない（飛ぶ約束をしない）
public class Penguin : MonoBehaviour
{
    public void Swim()
    {
        Debug.Log("ペンギンが泳いでいます");
    }
}

// 飛べるものだけを受け取るメソッドになる
public void MakeFlyableObjectFly(IFlyable flyable)
{
    flyable.Fly(); // IFlyable を実装したクラスなら必ず飛べる
}
```

> **💡 C# 経験者向け：**
> `override` したメソッドの中で `throw new NotSupportedException()` や
> `throw new NotImplementedException()` を書いているとき、
> または override して何もしない空実装にしているときは LSP 違反のサインです。
> 継承ツリーの設計を見直すか、インターフェースで分離することを検討してください。

---

## 5. I — インターフェース分離の原則

**「クライアントは使わないメソッドに依存させるべきではない」**

大きなインターフェースを小さく分割し、
各クラスが必要なインターフェースだけを実装できるようにします。

```csharp
// ❌ 悪い例：何でも入った大きなインターフェース
public interface ICharacter
{
    void Move();
    void Attack();
    void Fly();    // 飛べないキャラクターにも実装を強制してしまう
    void Swim();   // 泳げないキャラクターにも実装を強制してしまう
}

// 飛べないキャラクターでも Fly を実装しなければならない
public class Soldier : MonoBehaviour, ICharacter
{
    public void Move()   { Debug.Log("歩いて移動"); }
    public void Attack() { Debug.Log("剣で攻撃"); }
    public void Fly()    { /* 飛べないので空実装 — LSP 違反でもある */ }
    public void Swim()   { /* 泳げないので空実装 */ }
}
```

```csharp
// ✅ 良い例：インターフェースを小さく分割する
public interface IMovable
{
    void Move();
}

public interface IAttackable
{
    void Attack();
}

public interface IFlyable
{
    void Fly();
}

public interface ISwimmable
{
    void Swim();
}

// 各クラスが必要なインターフェースだけを実装する
public class Soldier : MonoBehaviour, IMovable, IAttackable
{
    public void Move()   { Debug.Log("歩いて移動"); }
    public void Attack() { Debug.Log("剣で攻撃"); }
    // Fly や Swim は実装しなくてよい
}

public class Dragon : MonoBehaviour, IMovable, IAttackable, IFlyable
{
    public void Move()   { Debug.Log("地上を移動"); }
    public void Attack() { Debug.Log("炎を吐く"); }
    public void Fly()    { Debug.Log("翼で飛ぶ"); }
}
```

> **🎮 スペースシューターでの活用例：**
> `IDamageable`（ダメージを受けられる）、`IMovable`（移動できる）、`IShootable`（弾を撃てる）を
> 個別のインターフェースとして定義することで、
> たとえば「弾は撃てないが壊せる障害物」のようなオブジェクトも
> 必要な分だけ実装できる柔軟な設計になります。

---

## 6. D — 依存性逆転の原則

**「上位モジュールは下位モジュールに依存してはならない。両者とも抽象に依存するべき」**

具体的なクラス（実装の詳細）ではなく、インターフェースや抽象クラスに依存する設計にします。
これにより、具体的な実装を差し替えても上位のクラスを変更しなくて済みます。

```csharp
// ❌ 悪い例：Player が具体的なログクラスに直接依存している
public class UnityConsoleLogger
{
    public void Log(string message)
    {
        Debug.Log($"[UnityConsole] {message}");
    }
}

public class Player : MonoBehaviour
{
    // 具体的なクラスに直接依存している
    // ログの種類を変えたいとき Player クラスを修正しなければならない
    private UnityConsoleLogger logger = new UnityConsoleLogger();

    public void TakeDamage(int amount)
    {
        logger.Log($"ダメージを受けました：{amount}");
    }
}
```

```csharp
// ✅ 良い例：インターフェースに依存させる
// 抽象（インターフェース）を定義する
public interface ILogger
{
    void Log(string message);
}

// 具体的な実装はインターフェースを満たすだけ
public class UnityConsoleLogger : ILogger
{
    public void Log(string message)
    {
        Debug.Log($"[UnityConsole] {message}");
    }
}

public class FileLogger : ILogger
{
    public void Log(string message)
    {
        // ファイルに書き出す処理（例）
        System.IO.File.AppendAllText("log.txt", message + "\n");
    }
}

// Player は ILogger インターフェースだけを知っている
public class Player : MonoBehaviour
{
    // コンストラクタ注入（依存性注入の最もシンプルな形）
    private ILogger logger;

    public void Initialize(ILogger logger)
    {
        this.logger = logger;
    }

    public void TakeDamage(int amount)
    {
        // UnityConsoleLogger でも FileLogger でも同じコードで動く
        logger.Log($"ダメージを受けました：{amount}");
    }
}
```

> **💡 C# 経験者向け：**
> 上記の `Initialize(ILogger logger)` のように、外部から依存オブジェクトを渡す手法を
> **依存性注入（Dependency Injection / DI）** と呼びます。
> Unity では MonoBehaviour のコンストラクタが使いにくいため、
> `Initialize` メソッドや `[SerializeField]` で渡すパターンがよく使われます。
> 大規模プロジェクトでは VContainer などの DI フレームワークが利用されます。

---

## 7. SOLID 原則まとめ

| 原則 | 一言まとめ | 違反のサイン |
|---|---|---|
| **単一責任** | 1クラス1責任 | クラスが肥大化・変更理由が複数ある |
| **開放閉鎖** | 追加OK・変更NG | 機能追加のたびに既存コードを修正している |
| **リスコフ** | 親と置き換え可能 | override で例外を投げる・空実装になっている |
| **インターフェース分離** | 小さく分割 | 使わないメソッドを実装させられている |
| **依存性逆転** | 抽象に依存 | 具体的なクラス名が上位モジュールに現れる |

---

## 8. SOLID を意識した設計への第一歩

SOLID 原則は「全部を完璧に守ること」が目的ではありません。
コードを**変更しやすく、壊れにくくする**ための指針です。

最初は **単一責任の原則（S）** だけを意識するところから始めるだけでも、
クラスの肥大化が防がれ、コードの見通しが大きく改善します。

`04_space-shooter/` の設計では、今回学んだ SOLID 原則を意識しながら
プレイヤー・敵・弾・スコアのクラス設計に挑戦します。
「このクラスの責任は1つか？」「新しい敵を追加したとき既存コードを修正しなくていいか？」
を問いながら設計する練習をしてみてください。

---

## 参考リンク

- [Unity 公式 設計思想 eBook（無料）](https://unity.com/resources/design-patterns-solid-ebook) — SOLID 原則の章：p.12〜p.47
- [Microsoft C# ドキュメント（インターフェース）](https://learn.microsoft.com/ja-jp/dotnet/csharp/fundamentals/types/interfaces)
