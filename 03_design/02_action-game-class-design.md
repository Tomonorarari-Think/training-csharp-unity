# 02 — クラス設計ワーク（横スクロールアクションゲーム）

## このワークの進め方

このドキュメントでは、横スクロールアクションゲームを題材に
**「ゲームの登場要素をクラスに分解する」** という設計体験をします。

以下の3ステップで進みます。

- **Step 1**：ゲームの登場要素を洗い出す
- **Step 2**：登場要素をクラスに分解する
- **Step 3**：クラス間の関係を整理する

> **各ステップの解答例を見る前に、まず自分で考えてみてください。**
> 設計に正解は1つではありません。自分なりの考えを持ってから解答例と比べることで、
> 「なぜこう設計するのか」という理由が身につきます。

---

## 題材：横スクロールアクションゲーム

以下のルールを持つシンプルな横スクロールアクションゲームを題材にします。

- 左から右にステージを進んでいくゲームです。
- プレイヤーキャラクターは走る・ジャンプができます。
- 敵キャラクターが歩き回っています。飛んでいる敵もいます。
- コインやアイテムを取得できます。
- ブロックを叩くとアイテムが出てきます。
- ゴールに到達するとステージクリアです。

---

## Step 1：登場要素を洗い出す

まずはゲームに登場する「もの」をすべて書き出してみましょう。

> **問い：** このゲームに登場するものを思いつく限り書き出してみましょう。
> 画面上に見えるものだけでなく、ゲームを動かすために必要な「見えない仕組み」も考えてみてください。

---

<details>
<summary>解答例を見る</summary>

| カテゴリ | 登場要素 |
|---|---|
| キャラクター | プレイヤーキャラクター、歩く敵、飛ぶ敵 |
| アイテム | コイン、パワーアップアイテム |
| ステージ部品 | ブロック、ゴールポスト |
| 管理・システム | ステージ（ゲーム全体の進行）、カメラ、BGM |

</details>

---

## Step 2：登場要素をクラスに分解する

### 2-1. クラス分けの考え方

登場要素を洗い出せたら、次は「クラス」として整理します。

> **問い：** それぞれの要素が持つ **データ（状態）** と **処理（行動）** は何でしょうか？

クラス設計の基本的な考え方は以下のとおりです。

- 「もの」や「概念」をクラスにします。
- データ（フィールド）と処理（メソッド）を一緒に持たせます。
- 似た要素は共通の基底クラスにまとめられないか考えます。

設計に正解は1つではありません。チームや規模によって適切な設計は変わります。
まず自分でフィールドとメソッドを考えてから、以下の解答例と比べてみてください。

---

### 2-2. クラス設計の解答例

#### PlayerCharacter クラス

```csharp
// プレイヤーキャラクターを表すクラス
public class PlayerCharacter : MonoBehaviour
{
    // --- データ（状態）---
    private int   hp;                  // 現在の HP
    private int   remainingLives;      // 残機数
    private bool  isPoweredUp;         // パワーアップ中かどうか
    // position は Transform が持つため省略

    // --- 処理（行動）---
    public void Move(float direction)  { } // 横方向に移動する
    public void Jump()                 { } // ジャンプする
    public void TakeDamage(int amount) { } // ダメージを受ける
    public void GetItem(ItemBase item) { } // アイテムを取得する
}
```

#### EnemyBase 抽象クラス・派生クラス

```csharp
// 敵キャラクターの共通部分をまとめた抽象クラス
public abstract class EnemyBase : MonoBehaviour
{
    // --- データ（状態）---
    protected int   hp;           // 現在の HP
    protected float moveSpeed;    // 移動速度

    // --- 処理（行動）---
    public abstract void Move();               // 移動処理（派生クラスで実装）
    public virtual  void TakeDamage(int amount){ } // ダメージを受ける（共通処理）
}

// 地上を歩く敵
public class WalkingEnemy : EnemyBase
{
    public override void Move()
    {
        // 左右に歩く処理
    }
}

// 空を飛ぶ敵
public class FlyingEnemy : EnemyBase
{
    public override void Move()
    {
        // 上下しながら飛ぶ処理
    }
}
```

#### Coin クラス

```csharp
// コインを表すクラス
public class Coin : MonoBehaviour
{
    // --- データ（状態）---
    private bool isCollected;  // すでに取得済みかどうか
    private int  scoreValue;   // 取得したときに加算されるスコア値

    // --- 処理（行動）---
    public void OnCollect() { } // プレイヤーが取得したときの処理
}
```

#### Block クラス

```csharp
// 叩けるブロックを表すクラス
public class Block : MonoBehaviour
{
    // --- データ（状態）---
    private int      hitCount;      // 叩かれた回数
    private ItemBase containedItem; // 内部に入っているアイテム

    // --- 処理（行動）---
    public void OnHit()       { } // 叩かれたときの処理
    public void SpawnItem()   { } // アイテムを出現させる
}
```

#### ItemBase 抽象クラス・派生クラス

```csharp
// アイテムの共通部分をまとめた抽象クラス
public abstract class ItemBase : MonoBehaviour
{
    // --- データ（状態）---
    protected string effectType; // 効果の種類（表示名など）

    // --- 処理（行動）---
    public abstract void OnPickup(PlayerCharacter player); // 取得時の効果（派生クラスで実装）
}

// パワーアップアイテム
public class PowerUpItem : ItemBase
{
    public override void OnPickup(PlayerCharacter player)
    {
        // プレイヤーをパワーアップさせる処理
    }
}
```

#### StageManager クラス

```csharp
// ステージ全体の進行を管理するクラス
public class StageManager : MonoBehaviour
{
    // --- データ（状態）---
    private int   currentScore;    // 現在のスコア
    private float remainingTime;   // 残り時間
    private bool  isStageClear;    // クリアしたかどうか

    // --- 処理（行動）---
    public void OnStageClear() { } // クリア時の処理
    public void OnGameOver()   { } // ゲームオーバー時の処理
}
```

---

### 2-3. 設計の解説

**なぜ `EnemyBase` を抽象クラスにしたのか？**

歩く敵と飛ぶ敵には「HPを持つ」「ダメージを受ける」という共通の性質があります。
この共通部分を `EnemyBase` にまとめることで、敵の種類が増えても
重複したコードを書かずに済みます（開放閉鎖の原則）。

**なぜ `ItemBase` を抽象クラスにしたのか？**

「取得したときに何かが起きる」という共通の振る舞いをインターフェースとして定義しつつ、
具体的な効果は各派生クラスに任せています。
新しいアイテムを追加するときに `ItemBase` を継承するだけで済むため、
既存コードを変更する必要がありません。

**なぜ `StageManager` を別クラスにしたのか？**

「スコアの管理」「ゲームオーバーの判定」「クリアの判定」は
プレイヤーキャラクターやステージオブジェクトとは別の責任です。
`PlayerCharacter` にこれらを持たせると、1つのクラスが複数の責任を持つことになり
変更のたびにプレイヤーの移動処理にも影響が及ぶリスクが生まれます（単一責任の原則）。

---

## Step 3：クラス間の関係を整理する

### 3-1. 関係の種類

クラス間の関係には主に以下の3種類があります。

| 関係 | 意味 | 例 |
|---|---|---|
| **継承（is-a）** | B は A の一種である | `WalkingEnemy` は `EnemyBase` である |
| **集約（has-a）** | A は B を持っている | `StageManager` はコインのリストを持つ |
| **依存** | A は処理の中で B を使う | `PlayerCharacter` はアイテム取得時に `ItemBase` を使う |

---

### 3-2. 関係の整理表

> **問い：** 以下の空欄を自分で埋めてみましょう。正解は1つではありません。

| クラスA | 関係 | クラスB | 説明 |
|---|---|---|---|
| `WalkingEnemy` | is-a | `EnemyBase` | 継承 |
| `FlyingEnemy` | is-a | `EnemyBase` | 継承 |
| `PowerUpItem` | is-a | `ItemBase` | 継承 |
| `StageManager` | has-a | `Coin`（リスト） | ステージ上のコインを管理する |
| `StageManager` | has-a | `EnemyBase`（リスト） | ステージ上の敵を管理する |
| `PlayerCharacter` | 依存 | `ItemBase` | アイテム取得時に `OnPickup` を呼ぶ |

---

### 3-3. 次のステップへ

ここで整理したクラスと関係は、**図として表現**するとより伝わりやすくなります。

[03_uml-basics.md](03_uml-basics.md) では、このクラス設計をクラス図として描く方法を学びます。
「継承は三角矢印で表す」「集約は菱形で表す」といったルールを使うと、
設計の全体像を一目で伝えられるようになります。

---

## 設計を振り返る問い

最後に、以下の問いについて考えてみてください。答えは1つではありません。

1. **プレイヤーキャラクターが「ステージ情報を持つ」のは、単一責任の原則に反しませんか？**
   たとえば `PlayerCharacter` に `currentScore` フィールドを持たせた場合、
   どんな問題が起きるでしょうか？

2. **敵の種類が10種類になった場合、この設計で対応できますか？**
   `EnemyBase` を継承した新しいクラスを追加するだけで済むでしょうか？
   それとも `StageManager` にも修正が必要でしょうか？

3. **コインと敵に「画面外に出たら消える」という共通処理が必要になった場合、どう設計を変えますか？**
   `Coin` と `EnemyBase` 両方に同じメソッドを書きますか？
   それとも共通の基底クラスやインターフェースを作りますか？

---

## 参考リンク

- [01_csharp/03_inheritance-interface.md](../01_csharp/03_inheritance-interface.md) — 継承・インターフェースの基礎
- [03_design/01_solid-principles.md](01_solid-principles.md) — SOLID 原則
- [03_design/03_uml-basics.md](03_uml-basics.md) — 次のステップ：クラス図で表現する
