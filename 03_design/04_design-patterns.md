# 04 — デザインパターン

## 1. デザインパターンとは

デザインパターンとは、「よくある設計上の問題に対する、再利用可能な解決策のテンプレート」です。
1994年に Erich Gamma・Richard Helm・Ralph Johnson・John Vlissides の4人（GoF：Gang of Four）が
著書の中で23のパターンを提唱したことで広く普及しました。

このドキュメントでは、Unity 開発で特によく使われる以下の6つを扱います。

| パターン | 一言まとめ |
|---|---|
| **Singleton** | 1インスタンスを保証 |
| **Observer** | 状態変化を通知 |
| **Command** | 操作をオブジェクト化 |
| **State** | 状態ごとに振る舞いを切り替え |
| **Factory** | 生成処理をカプセル化 |
| **Object Pool** | オブジェクトを使い回す |

> **目的について：** パターンの名前を暗記することが目的ではありません。
> 「このパターンを使うと、どんな問題がどう解決されるか」を理解することが目的です。
> コードレビューや設計議論で「Observer パターンで解決できそうです」と言えるようになることを目指してください。

---

## 2. Singleton パターン

### 問題意識

GameManager や AudioManager のような「シーン全体で1つだけ存在すべきオブジェクト」が
複数生成されてしまうと、スコアが2箇所で管理されたり BGM が二重に再生されたりする問題が起きます。
**インスタンスが必ず1つだけであることを保証したい。**

### 概念

「クラスのインスタンスがシーン全体で1つだけ存在することを保証する」パターンです。
static なプロパティで唯一のインスタンスを保持し、どこからでもアクセスできるようにします。

### コード例

```csharp
public class GameManager : MonoBehaviour
{
    // シーン全体で唯一のインスタンスを保持する static プロパティ
    public static GameManager Instance { get; private set; }

    public int Score { get; private set; }

    void Awake()
    {
        // すでに Instance が存在する場合（2つ目以降）は自分自身を破棄する
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        // シーンをまたいでも破棄されないようにする（必要な場合）
        DontDestroyOnLoad(gameObject);
    }

    public void AddScore(int points)
    {
        Score += points;
    }
}
```

```csharp
// どこからでも GameManager.Instance 経由でアクセスできる
public class Enemy : MonoBehaviour
{
    void OnDeath()
    {
        GameManager.Instance.AddScore(100);
    }
}
```

### 🎮 スペースシューターでの使用例

`GameManager` を Singleton にすることで、`Enemy`・`Player`・UI クラスから
`GameManager.Instance.AddScore()` のようにスコアへアクセスできる設計になります。

### メリット・デメリット

| | 内容 |
|---|---|
| **メリット** | どこからでもアクセスできる・インスタンスが1つであることを保証できる |
| **デメリット** | グローバル状態になりテストしにくい・依存関係が見えにくくなる・乱用すると依存性逆転の原則（SOLID の D）に反する |

> **Unity 公式 eBook 参照：** Singleton パターンの詳細は eBook p.66〜p.72 を参照してください。
> [Unity 公式 設計思想 eBook](https://unity.com/resources/design-patterns-solid-ebook)

---

## 3. Observer パターン

### 問題意識

プレイヤーの HP が 0 になったとき、「UI の更新」「ゲームオーバーサウンドの再生」「エフェクトの表示」など
複数のオブジェクトに通知したい場面があります。
`PlayerHealth` が UI・AudioManager・EffectManager を直接参照すると、
依存関係が複雑になりどれか一つを変更するたびに影響範囲が広がります。
**通知する側と受け取る側を疎結合に保ちたい。**

### 概念

「あるオブジェクトの状態変化を複数の別オブジェクトに通知する」パターンです。
通知する側（Subject）は受け取る側（Observer）の具体的なクラスを知りません。
イベントを通じて間接的に通知します。

### コード例

```csharp
// 通知する側（Subject）：C# の event と Action を使った実装
public class PlayerHealth : MonoBehaviour
{
    // 死亡時に発火するイベント（購読者を登録できる）
    public event System.Action OnPlayerDied;

    private int hp = 100;

    public void TakeDamage(int amount)
    {
        hp -= amount;

        if (hp <= 0)
        {
            // 購読しているすべての Observer に通知する
            OnPlayerDied?.Invoke();
        }
    }
}
```

```csharp
// 受け取る側（Observer）：UI クラスがイベントを購読する
public class GameOverUI : MonoBehaviour
{
    [SerializeField] private PlayerHealth playerHealth;

    void OnEnable()
    {
        // イベントを購読する
        playerHealth.OnPlayerDied += ShowGameOverScreen;
    }

    void OnDisable()
    {
        // ⚠️ 購読解除を忘れるとメモリリークの原因になる
        playerHealth.OnPlayerDied -= ShowGameOverScreen;
    }

    private void ShowGameOverScreen()
    {
        Debug.Log("ゲームオーバー画面を表示する");
    }
}
```

### Unity での実装方法

| 方法 | 特徴 | 向いている場面 |
|---|---|---|
| **C# の `event` / `Action`** | パフォーマンスが高い・コードで購読管理 | スクリプト同士の通知 |
| **UnityEvent** | Inspector から購読先を設定できる | デザイナーが購読先を設定する場合 |

### 🎮 スペースシューターでの使用例

敵が倒されたときに `OnEnemyDied` イベントを発火させると、
`ScoreManager`・`EffectManager`・`EnemyCounter` がそれぞれ独立して購読できます。
新しい Observer を追加しても `Enemy` クラスを変更する必要がありません。

### メリット・デメリット

| | 内容 |
|---|---|
| **メリット** | 通知する側と受け取る側が疎結合になる・新しい Observer を追加しても Subject のコードを変更不要 |
| **デメリット** | 購読解除し忘れでメモリリークが発生する可能性がある・イベントの流れが追いにくくなる場合がある |

> **Unity 公式 eBook 参照：** Observer パターンの詳細は eBook p.93〜p.100 を参照してください。
> [Unity 公式 設計思想 eBook](https://unity.com/resources/design-patterns-solid-ebook)

---

## 4. Command パターン

### 問題意識

「一つ前の操作を取り消したい（Undo）」という機能を実装したい場面があります。
また、プレイヤーの入力を順番通りに処理したい・記録して再生したいというケースもあります。
操作を if 文で直接処理してしまうと、Undo や記録が難しくなります。
**操作自体をオブジェクトとして扱えるようにしたい。**

### 概念

「操作をオブジェクトとして表現する」パターンです。
各操作が `Execute()`（実行）と `Undo()`（取り消し）を持つことで、
履歴管理・Undo/Redo・再実行が実現できます。

### コード例

```csharp
// 操作の共通インターフェース
public interface ICommand
{
    void Execute(); // 実行する
    void Undo();    // 取り消す
}
```

```csharp
// 移動操作を表すコマンドクラス
public class MoveCommand : ICommand
{
    private Transform target;     // 移動対象のオブジェクト
    private Vector3   moveOffset; // 移動量
    private Vector3   prevPosition; // Undo 用に移動前の位置を記録する

    public MoveCommand(Transform target, Vector3 moveOffset)
    {
        this.target     = target;
        this.moveOffset = moveOffset;
    }

    public void Execute()
    {
        prevPosition   = target.position;
        target.position += moveOffset;
    }

    public void Undo()
    {
        // 実行前の位置に戻す
        target.position = prevPosition;
    }
}
```

```csharp
// コマンドの履歴を管理するクラス
public class CommandHistory
{
    private Stack<ICommand> history = new Stack<ICommand>();

    public void Execute(ICommand command)
    {
        command.Execute();
        history.Push(command); // 履歴に積む
    }

    public void Undo()
    {
        if (history.Count == 0) return;
        history.Pop().Undo(); // 最後のコマンドを取り出して取り消す
    }
}
```

### 🎮 スペースシューターでの使用例

プレイヤーの入力を `MoveCommand`・`ShootCommand` としてオブジェクト化すると、
キューに積んで順番に実行するリプレイ機能や、
AI が同じコマンドを使うオートプレイ機能が実装しやすくなります。

### メリット・デメリット

| | 内容 |
|---|---|
| **メリット** | Undo/Redo が実装しやすい・操作のキューイング・ログが取りやすい |
| **デメリット** | クラス数が増える・シンプルな処理には過剰になる場合がある |

> **Unity 公式 eBook 参照：** Command パターンの詳細は eBook p.73〜p.78 を参照してください。
> [Unity 公式 設計思想 eBook](https://unity.com/resources/design-patterns-solid-ebook)

---

## 5. State パターン

### 問題意識

キャラクターの「待機・移動・ジャンプ・攻撃」のような状態を
if 文や switch 文で管理すると、状態が増えるにつれて条件分岐が膨大になります。
また、ある状態の処理を変更したとき、他の状態に影響が及ぶリスクが高まります。
**状態ごとの処理をクラスとして独立させたい。**

### 概念

「オブジェクトの内部状態に応じて振る舞いを切り替える」パターンです。
各状態をクラスとして表現し、Context オブジェクトが現在の状態を保持して切り替えます。

### コード例

```csharp
// 状態の共通インターフェース
public interface IState
{
    void Enter();          // 状態に入ったときの処理
    void Update();         // 状態中の毎フレーム処理
    void Exit();           // 状態を抜けるときの処理
}
```

```csharp
// 待機状態
public class IdleState : IState
{
    public void Enter()  { Debug.Log("待機状態に入った"); }
    public void Update() { /* 入力待ち */ }
    public void Exit()   { Debug.Log("待機状態を抜ける"); }
}

// 移動状態
public class MoveState : IState
{
    public void Enter()  { Debug.Log("移動状態に入った"); }
    public void Update() { /* 移動処理 */ }
    public void Exit()   { Debug.Log("移動状態を抜ける"); }
}

// ジャンプ状態
public class JumpState : IState
{
    public void Enter()  { Debug.Log("ジャンプ状態に入った"); }
    public void Update() { /* ジャンプ処理 */ }
    public void Exit()   { Debug.Log("ジャンプ状態を抜ける"); }
}
```

```csharp
// 現在の状態を保持して切り替えるクラス
public class StateContext : MonoBehaviour
{
    private IState currentState;

    public void ChangeState(IState newState)
    {
        currentState?.Exit();     // 現在の状態を終了する
        currentState = newState;
        currentState.Enter();     // 新しい状態を開始する
    }

    void Update()
    {
        currentState?.Update();   // 現在の状態の毎フレーム処理
    }
}
```

### 🎮 スペースシューターでの使用例

ゲーム全体の状態（`TitleState`・`PlayingState`・`GameOverState`・`ClearState`）を
State パターンで管理することで、状態遷移のロジックが一箇所にまとまり
ゲームの進行管理が見通しよくなります。

### メリット・デメリット

| | 内容 |
|---|---|
| **メリット** | 状態ごとの処理が独立したクラスになり見通しがよい・新しい状態の追加が容易 |
| **デメリット** | 状態数が少ない場合はクラス数が増えすぎる・状態遷移のルールが複雑になると管理が難しい |

> **Unity 公式 eBook 参照：** State パターンの詳細は eBook p.80〜p.92 を参照してください。
> [Unity 公式 設計思想 eBook](https://unity.com/resources/design-patterns-solid-ebook)

---

## 6. Factory パターン

### 問題意識

ステージの進行度に応じて異なる敵を生成する場合、呼び出し側が
「今はどの敵クラスを new すべきか」を知っている必要があります。
敵の種類が増えるたびに呼び出し側のコードを修正しなければならず、
開放閉鎖の原則に反します。
**生成処理を一箇所にまとめ、呼び出し側に詳細を知らせたくない。**

### 概念

「オブジェクトの生成処理をカプセル化する」パターンです。
呼び出し側は「何を欲しいか」だけを伝え、具体的なクラスの生成は Factory に任せます。

### コード例

```csharp
// 敵の種類を表す列挙型
public enum EnemyType
{
    Walking, // 歩く敵
    Flying,  // 飛ぶ敵
    Boss,    // ボス
}
```

```csharp
// 敵の生成処理をまとめた Factory クラス
public class EnemyFactory : MonoBehaviour
{
    [SerializeField] private GameObject walkingEnemyPrefab;
    [SerializeField] private GameObject flyingEnemyPrefab;
    [SerializeField] private GameObject bossPrefab;

    public GameObject Create(EnemyType type, Vector3 position)
    {
        GameObject prefab = type switch
        {
            EnemyType.Walking => walkingEnemyPrefab,
            EnemyType.Flying  => flyingEnemyPrefab,
            EnemyType.Boss    => bossPrefab,
            _                 => null,
        };

        if (prefab == null) return null;

        return Instantiate(prefab, position, Quaternion.identity);
    }
}
```

```csharp
// 呼び出し側は EnemyType を渡すだけでよい
public class StageDirector : MonoBehaviour
{
    [SerializeField] private EnemyFactory enemyFactory;

    void SpawnWave()
    {
        // 具体的なクラスを知らなくても生成できる
        enemyFactory.Create(EnemyType.Flying,  new Vector3(5, 0, 0));
        enemyFactory.Create(EnemyType.Walking, new Vector3(8, 0, 0));
    }
}
```

### 🎮 スペースシューターでの使用例

`EnemyFactory` がステージのウェーブデータを読んで対応する敵を生成する設計にすると、
新しい敵タイプを追加しても `StageDirector` を変更せずに済みます。

### メリット・デメリット

| | 内容 |
|---|---|
| **メリット** | 生成処理が一箇所にまとまる・新しい種類の追加が容易（開放閉鎖の原則） |
| **デメリット** | シンプルな生成には過剰になる場合がある |

> **Unity 公式 eBook 参照：** Factory パターンの詳細は eBook p.51〜p.56 を参照してください。
> [Unity 公式 設計思想 eBook](https://unity.com/resources/design-patterns-solid-ebook)

---

## 7. Object Pool パターン

### 問題意識

弾を発射するたびに `Instantiate` し、画面外で `Destroy` する処理を繰り返すと、
GC（ガベージコレクション）が頻繁に発生してフレームレートが落ちることがあります。
**オブジェクトを都度生成・破棄せず、使い回したい。**

### 概念

「オブジェクトをあらかじめ作成しておき、プールから貸し出し・返却して再利用する」パターンです。
`Instantiate` / `Destroy` の代わりに `SetActive(true)` / `SetActive(false)` を使うことで
GC 負荷を大幅に削減できます。

### コード例

```csharp
// シンプルな Object Pool の実装例
public class SimpleObjectPool : MonoBehaviour
{
    [SerializeField] private GameObject prefab;         // プールするオブジェクトのプレハブ
    [SerializeField] private int        initialSize = 10; // 初期生成数

    private Queue<GameObject> pool = new Queue<GameObject>();

    void Awake()
    {
        // 初期化時にまとめて生成しておく
        for (int i = 0; i < initialSize; i++)
        {
            GameObject obj = Instantiate(prefab);
            obj.SetActive(false);
            pool.Enqueue(obj);
        }
    }

    // プールからオブジェクトを取り出す
    public GameObject Get(Vector3 position, Quaternion rotation)
    {
        // プールが空の場合は新たに生成する
        GameObject obj = pool.Count > 0 ? pool.Dequeue() : Instantiate(prefab);
        obj.transform.SetPositionAndRotation(position, rotation);
        obj.SetActive(true);
        return obj;
    }

    // オブジェクトをプールに返却する
    public void Return(GameObject obj)
    {
        obj.SetActive(false);
        pool.Enqueue(obj);
    }
}
```

> **⚠️ Unity 2021 以降：**
> `UnityEngine.Pool` 名前空間に `ObjectPool<T>` が標準搭載されています。
> 新規プロジェクトでは自作より標準実装の利用を推奨します。
> 詳細は [Unity 公式 ObjectPool ドキュメント](https://docs.unity3d.com/ScriptReference/Pool.ObjectPool_1.html) を参照してください。

### 🎮 スペースシューターでの使用例

弾オブジェクトを `BulletPool` で管理することで、
大量の弾が飛び交う場面でも GC 負荷を抑えてフレームレートを安定させられます。
返却のタイミングは「画面外に出たとき」や「敵に当たったとき」が典型的です。

### メリット・デメリット

| | 内容 |
|---|---|
| **メリット** | `Instantiate` / `Destroy` の回数を減らせる・GC の負荷を軽減できる |
| **デメリット** | プールのサイズ管理が必要・返却時のオブジェクト状態リセット処理が必要 |

> **Unity 公式 eBook 参照：** Object Pool パターンの詳細は eBook p.57〜p.65 を参照してください。
> [Unity 公式 設計思想 eBook](https://unity.com/resources/design-patterns-solid-ebook)

---

## 8. デザインパターン まとめ

| パターン | 一言まとめ | 使いどころ |
|---|---|---|
| **Singleton** | 1インスタンスを保証 | GameManager・AudioManager |
| **Observer** | 状態変化を通知 | イベント・UI 更新・サウンド |
| **Command** | 操作をオブジェクト化 | Undo/Redo・リプレイ・入力管理 |
| **State** | 状態ごとに振る舞いを切り替え | キャラクター状態・ゲーム進行 |
| **Factory** | 生成処理をカプセル化 | 敵・アイテムの生成 |
| **Object Pool** | オブジェクトを使い回す | 弾・エフェクト・頻繁に生成するもの |

---

## 9. パターンを使う判断基準

- パターンは「問題があるから使う」ものです。最初からパターンありきで設計する必要はありません。
- 「コードが複雑になってきた」「同じ修正を何箇所もしている」と感じたときに、
  該当するパターンがないか検討してみてください。
- `04_space-shooter/` の設計では、上記6つのパターンのうちどれが使えるか考えながら
  クラス設計に取り組んでみましょう。

---

## 参考リンク

- [Unity 公式 設計思想 eBook（無料）](https://unity.com/resources/design-patterns-solid-ebook)
- [Unity 公式 ObjectPool ドキュメント](https://docs.unity3d.com/ScriptReference/Pool.ObjectPool_1.html)
- [03_design/01_solid-principles.md](01_solid-principles.md) — SOLID 原則（設計の前提知識）
- [04_space-shooter/](../04_space-shooter/) — パターンを実際に使う課題
