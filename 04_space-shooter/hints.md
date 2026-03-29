# hints.md — 詰まったときのヒント集

## このファイルの使い方

実装中に詰まったときに参照するファイルです。

ヒントは段階的に構成されています。まず「ヒント1」を読み、それで解決しない場合に「ヒント2」へ進む形式です。
いきなりすべてのヒントを読まずに、一段階ずつ試してみてください。

どうしても解決しない場合は、[solution/](solution/) フォルダの模範解答を参照しても構いません。

> **推奨：** ヒントを見る前に、「何が原因かを自分で考える」時間を5〜10分取ってみてください。
> エラーメッセージや Console の出力を読むだけで解決することも多いです。

---

## 1. 設計に関するヒント

### クラスの分け方がわからない

**ヒント1**

このゲームに登場する「もの」を、すべて紙や別ファイルに書き出してみましたか？
まずは思いつく限り列挙してみましょう。「画面に見えるもの」だけでなく「ゲームを動かす仕組み」も含めて考えてみてください。

**ヒント2**

書き出したものの中で、「データ（状態）を持つもの」と「処理（行動）をするもの」を色分けしてみましょう。
たとえば「HPを持つ」「位置を持つ」はデータ、「移動する」「弾を撃つ」は処理です。
データと処理をセットで持つものが、クラスの候補になります。

**ヒント3**

プレイヤーと敵は似た処理を持っていませんか？
共通部分を基底クラスにまとめられないか考えてみましょう。
参考：[03_design/01_solid-principles.md](../03_design/01_solid-principles.md)（単一責任の原則）

---

### 継承を使うべきか迷っている

**ヒント1**

「A は B である」という関係が成り立ちますか？

- 「プレイヤーの弾は弾である」→ 継承が候補になります
- 「プレイヤーは弾を持つ」→ 継承ではなくフィールドで持つ方が適切です

この「is-a / has-a」の判断が継承を使うかどうかの出発点です。

**ヒント2**

[01_csharp/03_inheritance-interface.md](../01_csharp/03_inheritance-interface.md) の
「is-a / has-a の使い分け」セクションを参照してみましょう。

---

### クラス図の書き方がわからない

[03_design/03_uml-basics.md](../03_design/03_uml-basics.md) の
Mermaid によるクラス図のセクションを参照してください。
[design-template.md](design-template.md) の記入例も参考にしてみましょう。

---

## 2. 実装に関するヒント

### プレイヤーが動かない

**ヒント1**

スクリプトは GameObject にアタッチされていますか？
Hierarchy でプレイヤーの GameObject を選択して、Inspector にスクリプトが表示されているか確認してみましょう。

**ヒント2**

入力を取得するコードは `Update()` の中に書いていますか？
`FixedUpdate()` に書いた場合、入力が正しく取れないことがあります。
参考：[02_unity/03_physics-input.md](../02_unity/03_physics-input.md)（「Input の取得は必ず Update() で行う」）

**ヒント3**

`Transform.position` を直接変更していますか？
`Rigidbody2D` を使っている場合は `MovePosition()` や `AddForce()` を使うことを検討してみましょう。
`Transform.position` を直接書き換えると物理演算と競合する場合があります。

---

### 弾が発射されない

**ヒント1**

弾の Prefab は Inspector の `[SerializeField]` フィールドに設定されていますか？
設定されていない場合、`bulletPrefab` は `null` のままになります。
Console に `NullReferenceException` が出ていないか確認してみましょう。

**ヒント2**

`Instantiate()` に渡しているのは Prefab ですか？シーン上の GameObject ですか？
Prefab（Project ウィンドウにあるもの）を Inspector から設定して渡すことを推奨します。

**ヒント3**

弾に速度・力を与えるコードは `Instantiate()` の後に書いていますか？
生成した直後のオブジェクトへの参照を使って、速度を設定する必要があります。
`Instantiate()` の戻り値を変数に受け取っているか確認してみましょう。

---

### 当たり判定が機能しない

**ヒント1**

弾と敵の両方に `Collider2D` がアタッチされていますか？
どちらか一方でも欠けていると、衝突イベントは発生しません。

**ヒント2**

`OnTriggerEnter2D` を使う場合、どちらか一方に `Rigidbody2D` が必要です。
また、衝突させたい `Collider2D` の `isTrigger` が有効になっているか確認してください。

**ヒント3**

Physics 2D の Layer Collision Matrix で、弾と敵のレイヤーが衝突する設定になっていますか？
**Edit → Project Settings → Physics 2D** の「Layer Collision Matrix」で確認できます。
プレイヤーの弾が敵にだけ当たる、敵の弾がプレイヤーにだけ当たるようにレイヤーで制御することも検討してみましょう。

---

### 敵が画面端で折り返さない

**ヒント1**

「画面端」をどのように判定していますか？
固定値（例：`x > 5.0f`）で判定していると、解像度や画面サイズによって動作が変わります。
`Camera.main.ViewportToWorldPoint()` を使うと、画面端の座標を動的に取得できます。

**ヒント2**

敵の移動方向をどのように管理していますか？
方向を表す変数（例：`int direction = 1`）を持ち、端に達したら `direction *= -1` で反転させる設計を考えてみましょう。
また、端に達したときに下への移動量を加算する処理はどこで行うかも考えてみてください。

---

### スコアが更新されない

**ヒント1**

スコアを管理するクラスへの参照は正しく取得できていますか？
`GameManager` を Singleton にしている場合は、`GameManager.Instance` にアクセスできるか確認してください。
`Instance` が `null` の場合は、GameManager の Awake が実行されているか（シーンに存在しているか）を確認してみましょう。

**ヒント2**

敵が倒されたことを `GameManager` にどうやって伝えていますか？
以下のいずれかのアプローチで伝える方法を検討してみましょう。
参考：[03_design/04_design-patterns.md](../03_design/04_design-patterns.md)

- 直接参照（敵が GameManager への参照を持つ）
- イベント通知（Observer パターン）
- Singleton 経由（`GameManager.Instance.AddScore()`）

---

### ゲームオーバーの処理がうまくいかない

**ヒント1**

ゲームオーバーになる条件は何ですか？
プレイヤーが敵に当たる場合と、敵が画面下部に到達する場合で、それぞれを別のメソッドで処理していますか？
2つの条件が混在しているとデバッグが難しくなります。

**ヒント2**

ゲームオーバー後に何をしたいですか？
シーンを再ロードする場合は `SceneManager.LoadScene()` を使います。
ファイルの先頭に `using UnityEngine.SceneManagement;` が必要です。
UI を表示するだけなら `SetActive(true)` でパネルを表示する方法もあります。

---

## 3. パフォーマンスに関するヒント

### 弾を大量に Instantiate すると重くなる

Object Pool パターンを検討してみましょう。
弾を都度生成・破棄する代わりに、あらかじめ生成した弾をプールから取り出して使い回す設計です。

参考：[03_design/04_design-patterns.md](../03_design/04_design-patterns.md)（Object Pool パターン）

---

### Update() 内で GetComponent() を呼んでいる

`GetComponent()` は処理コストが高いため、毎フレーム呼ぶのは避けることを推奨します。
`Awake()` または `Start()` で1回だけ呼んで、`private` フィールドにキャッシュしてみましょう。

参考：[02_unity/02_monobehaviour.md](../02_unity/02_monobehaviour.md)（Awake() のセクション）

---

## 4. 発展課題（ScriptableObject）に関するヒント

`02_unity/06_scriptable-object.md` で紹介した ScriptableObject リファクタリングに取り組む際のヒントです。

---

### EnemyData クラスをどこに作ればよいかわからない

**ヒント1**

`Assets/SpaceShooter/Scripts/` フォルダに `EnemyData.cs` を新規作成してください。
`MonoBehaviour` ではなく `ScriptableObject` を継承する点に注意してください。

**ヒント2**

最小構成はこの形です。

```csharp
using UnityEngine;

[CreateAssetMenu(
    fileName = "EnemyData",
    menuName = "SpaceShooter/EnemyData")]
public class EnemyData : ScriptableObject
{
    public float speed;
    public int   hp;
    public int   score;
    public float fireInterval;
}
```

`[CreateAssetMenu]` を付けると、Project ウィンドウの右クリックメニュー
**Create → SpaceShooter → EnemyData** からアセットを作成できます。

---

### EnemyBase に EnemyData を組み込む方法がわからない

**ヒント1**

`EnemyBase` に `[SerializeField] private EnemyData enemyData;` を追加して
Inspector でアセットをアサインする設計が基本です。

**ヒント2**

`EnemyData` がアサインされているときだけ値を上書きするようにすると
既存の Inspector 設定を壊さずに移行できます。

```csharp
protected virtual void Awake()
{
    if (enemyData != null)
    {
        speed        = enemyData.speed;
        hp           = enemyData.hp;
        fireInterval = enemyData.fireInterval;
    }
}
```

**ヒント3**

スコアは現在 `EnemyBase.Die()` に `100` とハードコードされています。
`EnemyData` に `score` フィールドを追加し、`Die()` の中で
`GameManager.Instance.AddScore(scoreValue)` のように渡すには
`Awake()` で `scoreValue` にコピーしておくと使いやすいです。

---

### ゲーム実行中に EnemyData の値を変えたらアセットが書き変わった

`ScriptableObject` のフィールドを実行中に直接書き換えると
エディタのアセットファイルに保存されてしまいます。

**対処法：** ランタイムで変化する値（hp の減少など）は
`MonoBehaviour` 側のフィールドに `Awake()` でコピーして管理してください。
ScriptableObject は初期値の入れ物として使い、直接書き換えないようにしましょう。

```csharp
// NG：ScriptableObject のフィールドを直接変更する
enemyData.hp -= damage;

// OK：MonoBehaviour 側のフィールドを変更する
hp -= damage; // hp は Awake() で enemyData.hp からコピーした値
```

---

### 複数の EnemyData アセットをどう切り替えればよいかわからない

**ヒント1**

Prefab を敵の種類ごとに分けて、それぞれの Prefab の Inspector で
異なる `EnemyData` アセットをアサインします。
`EnemySpawner` では Prefab をリストで管理し、生成時に選択する設計が一般的です。

**ヒント2**

`EnemySpawner` に以下のように Prefab リストを持たせる形が参考になります。

```csharp
[SerializeField] private List<GameObject> enemyPrefabs;

void SpawnEnemy()
{
    int index = Random.Range(0, enemyPrefabs.Count);
    Instantiate(enemyPrefabs[index], spawnPosition, Quaternion.identity);
}
```

---

## 5. それでも解決しない場合

上記のヒントを試しても解決しない場合は、[solution/](solution/) フォルダの模範解答を参照してください。

模範解答はあくまで一例であり、唯一の正解ではありません。
自分のコードと模範解答を比較して「どこが違うか」「なぜ違うのか」を言語化することも、大切な学びになります。

詰まった内容と解決方法は、[05_reflection/retrospective-template.md](../05_reflection/retrospective-template.md) に記録しておくことを推奨します。
「何で詰まって、どう解決したか」は次の開発でも役立つ経験です。
