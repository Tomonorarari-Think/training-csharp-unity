# 03. 物理演算・入力処理

---

## 1. 物理演算の概要

Unity の物理演算は **PhysX エンジン**をベースにしています（2D 物理は Box2D）。
重力・衝突・摩擦などの物理的な挙動が必要な場合に使います。

**物理演算を使う / 使わない場面の使い分け：**

| 場面 | 推奨する方法 |
|---|---|
| 重力・衝突・はね返りなど物理的な挙動が必要 | Rigidbody + Collider を使う |
| UI の移動・単純なキャラクター移動など | `Transform` を直接操作する |

物理演算は自由度が高い反面、`Transform` の直接操作と組み合わせると競合が起きます。
どちらで動かすかを設計段階で決めておくことが重要です。

---

## 2. Rigidbody

`Rigidbody`（3D）または `Rigidbody2D`（2D）を GameObject にアタッチすると、
その GameObject が Unity 物理演算の対象になります。

> スペースシューターは 2D ゲームを想定するため、以降は `Rigidbody2D` を中心に説明します。
> 3D の場合も基本的な考え方は同じです。

### 主要プロパティ

```csharp
Rigidbody2D rb = GetComponent<Rigidbody2D>();

rb.mass        = 1.0f;  // 質量（大きいほど力の影響を受けにくい）
rb.drag        = 0.5f;  // 空気抵抗（大きいほど速度が落ちやすい）
rb.gravityScale = 0.0f; // 重力の倍率（0 にすると重力なし。スペースシューターで使用）
rb.isKinematic = false; // true にすると物理演算の影響を受けなくなる（後述）
```

### 移動・力の加え方

```csharp
private Rigidbody2D rb;

void Awake()
{
    rb = GetComponent<Rigidbody2D>();
}

void FixedUpdate()
{
    // 力を加える（物理演算に従って自然に加速する）
    rb.AddForce(Vector2.up * 10.0f);

    // 物理演算を考慮した位置移動（Collider の衝突を維持したまま移動する）
    rb.MovePosition(rb.position + Vector2.right * 5.0f * Time.fixedDeltaTime);

    // 速度を直接設定する（瞬時に速度を変えたい場合に使う）
    rb.linearVelocity = new Vector2(0, 5.0f);
}
```

**velocity（linearVelocity）の多用が非推奨な理由：**
速度を直接書き換えると物理演算の連続性が失われ、
衝突の反発・摩擦といった挙動が意図どおりに動かなくなることがあります。
`AddForce` や `MovePosition` で済む場合はそちらを優先してください。

### Rigidbody2D と Rigidbody の主な違い

| | Rigidbody2D | Rigidbody |
|---|---|---|
| 対象 | 2D ゲーム | 3D ゲーム |
| 回転軸 | Z 軸のみ | X・Y・Z 軸 |
| 重力プロパティ | `gravityScale` | `useGravity`（bool） |
| 速度プロパティ | `linearVelocity` | `velocity` |

### isKinematic の使いどころ

`isKinematic = true` にすると、そのオブジェクトは物理演算の影響（重力・衝突による押し返し）を
受けなくなります。ただし Collider は有効なままなので衝突の検知はできます。

```csharp
// プレイヤーの動きをスクリプトで完全に制御したい場合
rb.isKinematic = true;
rb.MovePosition(targetPosition); // 衝突判定を維持しながらスクリプトで動かす
```

🎮 Unity での使用例：スペースシューターのプレイヤー機体は
重力の影響を受けず自分で動かすため `gravityScale = 0` または `isKinematic = true` を使います（[04_space-shooter/](../04_space-shooter/) で実装します）。

### よくあるハマりどころ

**Transform を直接操作すると物理演算と競合する**

```csharp
// 悪い例：Rigidbody がある GameObject に Transform を直接操作する
void Update()
{
    transform.position += Vector3.up * Time.deltaTime; // 物理演算と競合する
}

// 良い例：物理演算を使うなら FixedUpdate で Rigidbody の API を使う
void FixedUpdate()
{
    rb.MovePosition(rb.position + Vector2.up * Time.fixedDeltaTime);
}
```

---

## 3. Collider

**Collider** は GameObject の当たり判定の形状を定義するコンポーネントです。

### 種類と使い分け

| Collider | 形状 | 主な用途 |
|---|---|---|
| `BoxCollider2D` | 四角形（2D） | ブロック・床・壁 |
| `CircleCollider2D` | 円形（2D） | キャラクター・弾 |
| `PolygonCollider2D` | 多角形（2D） | 複雑な形状のスプライト |
| `BoxCollider` | 直方体（3D） | 箱・床・壁 |
| `CapsuleCollider` | カプセル形状（3D） | 3D キャラクター |

### Trigger と Collision の違い

| 設定 | 物理的な押し返し | 衝突の検知 | 主な用途 |
|---|---|---|---|
| `isTrigger = false`（Collision） | あり | あり | 壁・床など |
| `isTrigger = true`（Trigger） | なし（すり抜け） | あり | アイテム取得・ダメージ判定など |

### コールバックメソッド

```csharp
// ===== Collision（isTrigger = false）=====

// 衝突した瞬間
void OnCollisionEnter2D(Collision2D collision)
{
    Debug.Log($"{collision.gameObject.name} と衝突した");
}

// 衝突し続けている間（毎フレーム）
void OnCollisionStay2D(Collision2D collision) { }

// 衝突が終わった瞬間
void OnCollisionExit2D(Collision2D collision) { }


// ===== Trigger（isTrigger = true）=====

// Trigger に入った瞬間
void OnTriggerEnter2D(Collider2D other)
{
    Debug.Log($"{other.gameObject.name} が Trigger に入った");
}

// Trigger から出た瞬間
void OnTriggerExit2D(Collider2D other) { }
```

🎮 Unity での使用例：スペースシューターの弾と敵の当たり判定

```csharp
// 弾のスクリプト
public class Bullet : MonoBehaviour
{
    [SerializeField] private int damage = 10;

    void OnTriggerEnter2D(Collider2D other)
    {
        // タグで衝突相手を識別する（CompareTag を使う）
        if (other.CompareTag("Enemy"))
        {
            other.GetComponent<EnemyBase>()?.TakeDamage(damage);
            Destroy(gameObject); // 弾を消す
        }
    }
}
```

### よくあるハマりどころ

**Collider コールバックが呼ばれない主な原因：**

1. 両方の GameObject に Collider がない
2. Trigger のコールバック（`OnTriggerEnter` 等）を受け取るには
   **どちらか一方に Rigidbody（または Rigidbody2D）が必要**です
3. Layer の設定で衝突が無効になっている
   （`Edit → Project Settings → Physics 2D → Layer Collision Matrix` で確認）

```csharp
// 特定のレイヤー間の衝突を無効化する
Physics2D.IgnoreLayerCollision(
    LayerMask.NameToLayer("Bullet"),
    LayerMask.NameToLayer("Player")  // 自分が撃った弾が自分に当たらないようにする
);
```

---

## 4. Input System

### 4-1. Legacy Input Manager と新 Input System

> ⚠️ **Unity 2019.1 以降**
> Unity には新旧2つの入力システムが存在します。
>
> - **Legacy Input Manager（旧）**：`Input.GetKey()` / `Input.GetAxis()` など。手軽に使えますが柔軟性に欠けます
> - **Input System パッケージ（新）**：Unity 2019.1 以降で利用可能。複数デバイス対応・リマップ対応で新規プロジェクトに推奨。別途パッケージのインストールが必要です
>
> 本ドキュメントでは両方の基本的な使い方を説明します。

### 4-2. Legacy Input Manager（旧）

```csharp
void Update()
{
    // キー入力
    if (Input.GetKey(KeyCode.Space))     { /* 押している間 */ }
    if (Input.GetKeyDown(KeyCode.Space)) { /* 押した瞬間のみ */ }
    if (Input.GetKeyUp(KeyCode.Space))   { /* 離した瞬間のみ */ }

    // 軸入力（-1 〜 1 の連続値）
    float horizontal = Input.GetAxis("Horizontal"); // 左右（A/D または ←/→）
    float vertical   = Input.GetAxis("Vertical");   // 上下（W/S または ↑/↓）
}
```

🎮 Unity での使用例：スペースシューターの弾発射（Legacy 版）

```csharp
public class PlayerShooter : MonoBehaviour
{
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform  firePoint;

    void Update()
    {
        // スペースキーを押した瞬間に弾を発射する
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        }
    }
}
```

### 4-3. 新 Input System（推奨）

**パッケージのインストール：**

1. `Window → Package Manager` を開きます
2. 左上のドロップダウンで「Unity Registry」を選択します
3. 検索欄に「Input System」と入力します
4. `Input System` を選択して「Install」をクリックします
5. バックエンドの切り替えダイアログが出たら「Yes」をクリックします（エディタが再起動します）

**PlayerInput コンポーネントを使った基本構成：**

1. `Assets` フォルダ右クリック → `Create → Input Actions` で Input Action Asset を作成します
2. `Move`・`Fire` などのアクションを定義してキーをバインドします
3. プレイヤー GameObject に `PlayerInput` コンポーネントをアタッチして
   作成した Input Action Asset を設定します

```csharp
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private Vector2 moveInput;

    // PlayerInput の「Send Messages」モードで自動的に呼ばれる
    void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }

    void OnFire(InputValue value)
    {
        if (value.isPressed)
        {
            Debug.Log("発射！");
        }
    }

    void FixedUpdate()
    {
        // OnMove で受け取った値を使って移動する
        GetComponent<Rigidbody2D>().MovePosition(
            (Vector2)transform.position + moveInput * 5.0f * Time.fixedDeltaTime
        );
    }
}
```

**旧 Legacy との使い分け：**

| 場面 | 推奨 |
|---|---|
| プロトタイプ・小規模・学習用 | Legacy Input Manager でも可 |
| 本格的な開発・複数デバイス対応・リマップ対応 | 新 Input System 推奨 |

### 4-4. 入力の取得は必ず Update() で行う

`FixedUpdate` は毎フレームとは異なるタイミングで実行されるため、
`GetKeyDown` などの「1フレームだけ true になる」入力は `FixedUpdate` に書くと取りこぼします。

```csharp
private bool fireRequested = false;

void Update()
{
    // 入力は Update で取得してフラグに保存する
    if (Input.GetKeyDown(KeyCode.Space))
    {
        fireRequested = true;
    }
}

void FixedUpdate()
{
    // フラグを使って物理処理を行い、使い終わったらリセットする
    if (fireRequested)
    {
        Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        fireRequested = false;
    }
}
```

---

## 5. Layer と Tag

### Layer

Layer は物理演算・カメラ描画・レイキャストの対象を絞り込むための分類です。
`Edit → Project Settings → Tags and Layers` で追加・管理します。

```csharp
// 特定のレイヤーを対象にレイキャストする
int      layerMask = LayerMask.GetMask("Enemy");
RaycastHit2D hit   = Physics2D.Raycast(transform.position, Vector2.up, 10.0f, layerMask);

if (hit.collider != null)
{
    Debug.Log($"{hit.collider.name} に当たった");
}
```

🎮 Unity での使用例：スペースシューターのレイヤー設計

```csharp
// Player・Enemy・Bullet・Background などのレイヤーを作成し
// 弾同士・弾と自機が当たらないよう Physics 2D の Layer Collision Matrix で除外する
Physics2D.IgnoreLayerCollision(
    LayerMask.NameToLayer("PlayerBullet"),
    LayerMask.NameToLayer("Player")
);
```

### Tag

Tag は GameObject に付けるラベルです。
`gameObject.tag == "Enemy"` より `gameObject.CompareTag("Enemy")` が推奨されます。

**CompareTag が推奨される理由：**
`== 演算子`はタグの文字列を毎回生成するためガベージが発生しますが、
`CompareTag` は内部で最適化されているためパフォーマンスへの影響が少ないです。

```csharp
// 非推奨（ガベージが発生する）
if (other.gameObject.tag == "Enemy") { }

// 推奨
if (other.gameObject.CompareTag("Enemy")) { }
```

🎮 Unity での使用例：OnTriggerEnter2D でタグを確認する

```csharp
void OnTriggerEnter2D(Collider2D other)
{
    if (other.CompareTag("Enemy"))
    {
        other.GetComponent<EnemyBase>()?.TakeDamage(10);
        Destroy(gameObject);
    }
}
```

---

## 6. よくあるハマりどころまとめ

### Rigidbody の操作を Update で行う

→ 物理演算の操作は `FixedUpdate` で行い、`Transform` の直接操作と混在させないようにする。

### Collider のコールバックが呼ばれない

→ 以下を確認する：
1. 両方の GameObject に Collider があるか
2. Trigger の場合、どちらか一方に Rigidbody があるか
3. Layer Collision Matrix で衝突が有効になっているか
4. 両方の GameObject がアクティブか

### Legacy Input と新 Input System を混在させる

一つのプロジェクトで両方を混在させると挙動が予測しにくくなります。
プロジェクトの方針を統一してください。

### Input.GetKeyDown を FixedUpdate に書いて入力が抜ける

→ 入力取得は必ず `Update` で行い、フラグ経由で `FixedUpdate` に渡す。

### tag の比較に == を使う

→ `CompareTag()` を使う。`==` による比較は毎回文字列オブジェクトが生成される。

---

## 7. バージョン別 API 変更まとめ

| 旧 API / 旧仕様 | 変更バージョン | 新 API / 推奨 |
|---|---|---|
| Legacy Input Manager | Unity 2019.1〜 | 新 Input System パッケージを推奨 |
| `Input.GetAxis()` 等 | 廃止ではないが非推奨 | `PlayerInput` コンポーネントを推奨 |
| `Rigidbody2D.velocity` | Unity 6〜 | `Rigidbody2D.linearVelocity` に名称変更 |

---

## 8. 参考リンク

- [Unity 公式マニュアル：Rigidbody 2D](https://docs.unity3d.com/ja/current/Manual/class-Rigidbody2D.html)
- [Unity 公式マニュアル：Collider 2D](https://docs.unity3d.com/ja/current/Manual/Collider2D.html)
- [Unity 公式マニュアル：Input System](https://docs.unity3d.com/Packages/com.unity.inputsystem@1.7/manual/index.html)
