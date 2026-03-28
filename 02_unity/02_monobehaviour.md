# 02. MonoBehaviour とライフサイクル

---

## 1. MonoBehaviour とは

`MonoBehaviour` は Unity スクリプトの基底クラスです。
`MonoBehaviour` を継承したクラスを GameObject にアタッチすることで、
Unity のライフサイクルメソッド（`Awake`・`Start`・`Update` など）が自動で呼ばれるようになります。

```csharp
using UnityEngine;

// MonoBehaviour を継承して GameObject にアタッチできるようにする
public class PlayerController : MonoBehaviour
{
    void Start()  { }
    void Update() { }
}
```

💡 C# 経験者向け：`MonoBehaviour` は Unity エンジンが管理するクラスです。
`new` でインスタンスを生成してはいけません。
代わりに `AddComponent<T>()` またはプレハブの `Instantiate()` を使います。

```csharp
// 悪い例：new では生成できない（警告または意図しない動作）
PlayerController pc = new PlayerController();

// 良い例：AddComponent で GameObject に追加する
PlayerController pc = gameObject.AddComponent<PlayerController>();
```

---

## 2. ライフサイクルの全体像

Unity は MonoBehaviour のメソッドを決まった順序で自動的に呼び出します。

```
ゲーム起動・シーンロード
│
├── Awake()        — 最初に1回だけ呼ばれる（自分自身の初期化）
├── OnEnable()     — オブジェクトが有効化されるたびに呼ばれる
├── Start()        — 最初のフレームの前に1回だけ呼ばれる（他オブジェクトへの依存がある初期化）
│
├──【毎フレーム繰り返し】
│   ├── FixedUpdate()  — 物理演算のタイミングで一定間隔（デフォルト 0.02秒）
│   ├── Update()       — 毎フレーム（入力・ゲームロジック）
│   └── LateUpdate()   — 全 Update 終了後（カメラ追従など）
│
├── OnDisable()    — オブジェクトが無効化されるたびに呼ばれる
└── OnDestroy()    — オブジェクトが破棄されるときに1回呼ばれる
```

詳細な実行順序は [Unity 公式マニュアル（イベント関数の実行順序）](https://docs.unity3d.com/ja/current/Manual/ExecutionOrder.html) を参照してください。

---

## 3. 主要なライフサイクルメソッドの詳細

### Awake()

シーンのロード直後、他のオブジェクトの初期化より先に実行されます。
**自分自身の初期化処理**（コンポーネントのキャッシュなど）に使います。

💡 C# 経験者向け：コンストラクタの代わりに `Awake` を使います。
`MonoBehaviour` のコンストラクタで Unity の機能（`GetComponent` など）を呼ぶと
意図しない動作の原因になります。

```csharp
public class PlayerController : MonoBehaviour
{
    private Rigidbody   rb;
    private AudioSource audioSource;

    void Awake()
    {
        // コンポーネントのキャッシュは Awake で行う
        rb          = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
    }
}
```

### Start()

`Awake` の後、**最初のフレームが始まる前**に1回だけ呼ばれます。
他のオブジェクトへの参照が必要な初期化処理に使います。

**Awake と Start の使い分け：**

| 使う場面 | メソッド |
|---|---|
| 自分自身のコンポーネントの取得・初期化 | `Awake` |
| 他のオブジェクトへの参照・依存がある初期化 | `Start` |

```csharp
public class EnemyController : MonoBehaviour
{
    private PlayerController player;

    void Start()
    {
        // 他のオブジェクトへの参照は Start で行う
        // （全オブジェクトの Awake が完了した後であることが保証される）
        player = FindFirstObjectByType<PlayerController>();
    }
}
```

### Update()

**毎フレーム**呼ばれる最もよく使うメソッドです。
ユーザー入力の取得・キャラクターの移動・ゲームロジックの更新に使います。

フレームレートは実行環境によって変わるため、移動量などには **`Time.deltaTime`** を掛けて
フレームレートに依存しない処理にします。

```csharp
[SerializeField] private float moveSpeed = 5.0f;

void Update()
{
    float horizontal = Input.GetAxis("Horizontal");
    float vertical   = Input.GetAxis("Vertical");

    // Time.deltaTime を掛けることでフレームレートに関係なく
    // 「1秒あたり moveSpeed ユニット」移動するようになる
    Vector3 move = new Vector3(horizontal, 0, vertical) * moveSpeed * Time.deltaTime;
    transform.Translate(move);
}
```

### FixedUpdate()

物理演算のタイミングで**一定間隔**（デフォルト 0.02秒ごと）に呼ばれます。
`Rigidbody` を使った物理処理はここで行います。

**Update と FixedUpdate の使い分け：**

| 処理の種類 | 使うメソッド |
|---|---|
| 入力取得・ゲームロジック・アニメーション | `Update` |
| `Rigidbody` の力・速度の操作 | `FixedUpdate` |

```csharp
private Rigidbody rb;
[SerializeField] private float jumpForce = 5.0f;

void Awake()
{
    rb = GetComponent<Rigidbody>();
}

void FixedUpdate()
{
    // 物理演算は FixedUpdate で行う
    float horizontal = Input.GetAxis("Horizontal");
    rb.AddForce(new Vector3(horizontal, 0, 0) * 10.0f);
}
```

> ⚠️ `Input.GetKeyDown()` などの入力取得は `FixedUpdate` に書かないでください。
> `FixedUpdate` は毎フレームとは異なるタイミングで呼ばれるため、
> 入力が抜けることがあります。入力は `Update` で取得してください。

### LateUpdate()

その**フレームの全オブジェクトの `Update` が終わった後**に呼ばれます。
カメラの追従処理のように「プレイヤーが移動した後に位置を決めたい」処理に使います。

🎮 Unity での使用例：プレイヤーを追いかけるカメラ

```csharp
public class CameraController : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private Vector3   offset = new Vector3(0, 5, -10);

    void LateUpdate()
    {
        // プレイヤーの Update が終わった後にカメラ位置を更新する
        transform.position = player.position + offset;
    }
}
```

### OnEnable() / OnDisable()

GameObject または Component の**有効・無効が切り替わるたびに**呼ばれます。
イベントの購読・解除のペアを `OnEnable` / `OnDisable` に書くのが定番のパターンです。

```csharp
void OnEnable()
{
    // 有効化されたときにイベントを購読する
    GameEvents.OnEnemyDefeated += HandleEnemyDefeated;
}

void OnDisable()
{
    // 無効化されたときに必ず購読を解除する（メモリリーク防止）
    GameEvents.OnEnemyDefeated -= HandleEnemyDefeated;
}

void HandleEnemyDefeated()
{
    Debug.Log("敵を倒した！");
}
```

### OnDestroy()

GameObject が**破棄される直前**に1回だけ呼ばれます。
`OnDisable` でカバーできない後処理（外部リソースの解放など）に使います。

```csharp
void OnDestroy()
{
    // 外部リソースの解放・後処理
    Debug.Log($"{gameObject.name} が破棄されました");
}
```

---

## 4. 実行順序の制御

複数のスクリプトの実行順序は、デフォルトでは**不定**です。
依存関係がある場合は以下の方法で対処します。

**方法①：Awake と Start の使い分けで解決する（推奨）**

自分の初期化は `Awake`、他オブジェクトへの依存がある処理は `Start` に書くことで、
すべての `Awake` が終わった後に `Start` が呼ばれることを利用できます。

**方法②：Script Execution Order で明示的に設定する**

`Edit → Project Settings → Script Execution Order` で
スクリプトごとの実行順序を数値で指定できます（小さい数値が先に実行される）。
依存関係が複雑になった場合の最終手段として使います。

---

## 5. コルーチンの基礎（概要のみ）

**コルーチン**とは「処理を一時停止して、次のフレームや一定時間後に再開できる仕組み」です。

```csharp
using System.Collections;

public class Example : MonoBehaviour
{
    void Start()
    {
        StartCoroutine(DelayedMessage()); // コルーチンを開始する
    }

    // IEnumerator を戻り値にする
    IEnumerator DelayedMessage()
    {
        Debug.Log("処理を開始します");

        yield return new WaitForSeconds(3.0f); // 3秒待つ

        Debug.Log("3秒後に実行されました");

        yield return null; // 次のフレームまで待つ
    }
}
```

**よく使う yield return の種類：**

| yield return | 動作 |
|---|---|
| `yield return null` | 次のフレームまで待つ |
| `yield return new WaitForSeconds(n)` | n 秒待つ |
| `yield return new WaitForFixedUpdate()` | 次の FixedUpdate まで待つ |
| `yield return new WaitUntil(() => 条件)` | 条件が true になるまで待つ |

> コルーチンの詳細・`Awaitable`・`UniTask` など
> Unity の非同期処理全般については [05_async.md](./05_async.md) で詳しく扱います。
> Unity 6 以降では `async`/`await` による非同期処理が推奨されるケースが増えています。

---

## 6. enabled プロパティと gameObject.SetActive() の違い

| 操作 | 対象 | Update は呼ばれるか |
|---|---|---|
| `this.enabled = false` | コンポーネント単体を無効化 | 他のコンポーネントは動き続ける |
| `gameObject.SetActive(false)` | GameObject 全体を無効化 | GameObject 上の全コンポーネントが止まる |

```csharp
// このスクリプトのみ無効化（他のコンポーネントは動き続ける）
this.enabled = false;

// GameObject 全体を非アクティブにする（全コンポーネントが止まる）
gameObject.SetActive(false);

// 再び有効化する
gameObject.SetActive(true);
```

> ⚠️ `gameObject.SetActive(false)` にすると、
> その GameObject 上のすべての `Update`・`FixedUpdate` が呼ばれなくなります。
> 意図せず入力処理や当たり判定が止まることがあるため注意してください。

---

## 7. よくあるハマりどころ

### MonoBehaviour を new で生成しようとする

```csharp
// エラーになる（または警告が出て意図しない動作をする）
PlayerController pc = new PlayerController();

// 正しい方法
PlayerController pc = gameObject.AddComponent<PlayerController>();
```

### Awake と Start の実行タイミングを混同する

他のオブジェクトの参照を `Awake` で取得しようとすると、
相手の `Awake` がまだ実行されていない場合に `null` になることがあります。
他オブジェクトへの依存がある初期化は `Start` に書いてください。

### Update で Time.deltaTime を使い忘れる

```csharp
// 悪い例：フレームレートが高いほど速く動く
transform.Translate(Vector3.forward * 5.0f);

// 良い例：フレームレートに関係なく「1秒あたり 5 ユニット」移動する
transform.Translate(Vector3.forward * 5.0f * Time.deltaTime);
```

### FixedUpdate で入力処理を書いて入力が抜ける

`FixedUpdate` はフレームとは異なるタイミングで実行されるため、
`Input.GetKeyDown()` などの入力を `FixedUpdate` に書くと検出漏れが起きます。

```csharp
// 悪い例：FixedUpdate で GetKeyDown を呼ぶと入力が抜けることがある
void FixedUpdate()
{
    if (Input.GetKeyDown(KeyCode.Space)) Jump();
}

// 良い例：入力は Update で取得し、フラグで FixedUpdate に渡す
private bool jumpRequested = false;

void Update()
{
    if (Input.GetKeyDown(KeyCode.Space)) jumpRequested = true;
}

void FixedUpdate()
{
    if (jumpRequested) { Jump(); jumpRequested = false; }
}
```

### OnDisable でイベント購読を解除し忘れる

イベントの購読を解除しないままオブジェクトが破棄されると、
存在しないオブジェクトへの参照が残り `MissingReferenceException` の原因になります。

```csharp
// OnEnable で購読したら OnDisable で必ず解除する
void OnEnable()  { GameEvents.OnEnemyDefeated += HandleEnemyDefeated; }
void OnDisable() { GameEvents.OnEnemyDefeated -= HandleEnemyDefeated; }
```

---

## 8. 参考リンク

- [Unity 公式マニュアル：イベント関数の実行順序](https://docs.unity3d.com/ja/current/Manual/ExecutionOrder.html)
- [Unity 公式マニュアル：コルーチン](https://docs.unity3d.com/ja/current/Manual/Coroutines.html)
- 非同期処理の詳細：[02_unity/05_async.md](./05_async.md)
