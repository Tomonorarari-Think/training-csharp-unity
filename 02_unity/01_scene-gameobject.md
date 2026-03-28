# 01. Scene・GameObject・Component

---

## 1. Unity のゲーム構造の全体像

Unity のゲームは以下の階層で構成されています。

```
Project
└── Scene（ゲームの1画面）
    └── GameObject（シーン上のあらゆるもの）
        └── Component（GameObject に機能を追加するパーツ）
```

- **Project**：Unity プロジェクト全体。スクリプト・画像・音声などすべてのアセットを管理します
- **Scene**：ゲームの1画面に相当します。タイトル・ステージ・ゲームオーバーなど、画面ごとに Scene を用意するのが基本です
- **複数 Scene の切り替え**：`SceneManager.LoadScene()` で別の Scene に遷移できます

```csharp
using UnityEngine.SceneManagement;

SceneManager.LoadScene("GameScene");      // シーン名で遷移
SceneManager.LoadScene(1);               // ビルドインデックスで遷移
```

---

## 2. GameObject

**GameObject** は Scene 上のあらゆるものの基本単位です。
カメラ・ライト・キャラクター・UI・見えないトリガー領域まで、シーンに存在するものはすべて GameObject です。

### Transform は唯一の必須 Component

すべての GameObject は **Transform** を必ず1つ持ちます。
Transform は位置・回転・スケールを管理するコンポーネントで、削除できません。

```csharp
// Transform の基本操作
transform.position  = new Vector3(0, 0, 0); // 位置
transform.rotation  = Quaternion.identity;  // 回転
transform.localScale = Vector3.one;          // スケール
```

### 親子関係（Hierarchy）

GameObject は親子関係を持てます。
子の GameObject は親の Transform に追従して移動・回転します。

```csharp
// スクリプトから親子関係を設定する
childObject.transform.SetParent(parentObject.transform);

// 親を取得する
Transform parent = transform.parent;

// 子の数を取得する
int childCount = transform.childCount;
```

### Unity エディタとの対応

| ウィンドウ | 役割 |
|---|---|
| **Hierarchy** | Scene 内の全 GameObject を階層で表示 |
| **Inspector** | 選択した GameObject の Component 一覧と設定値を表示 |

---

## 3. Component

**Component** は「GameObject に機能を追加するパーツ」です。
Unity は「1つの GameObject に必要な機能を持つ Component を組み合わせる」という
**コンポーネント指向**の設計思想を採用しています。

💡 C# 経験者向け：Component はクラスのインスタンスに相当します。
`AddComponent<T>()` でインスタンスを生成して GameObject にアタッチするイメージです。

### 代表的な Component

| Component | 役割 |
|---|---|
| `Transform` | 位置・回転・スケールの管理（必須） |
| `MeshRenderer` | 3D メッシュの描画 |
| `Collider` | 当たり判定の形状（Box / Sphere / Capsule など） |
| `Rigidbody` | 物理演算（重力・力・速度） |
| `AudioSource` | 音の再生 |
| `Camera` | ゲームの映像を撮影してスクリーンに映す |
| 自作スクリプト | `MonoBehaviour` を継承したクラス |

---

## 4. スクリプトから GameObject・Component を操作する

### 4-1. 自分自身の Component を取得する

`GetComponent<T>()` で同じ GameObject にアタッチされた Component を取得します。
毎フレーム呼ぶとパフォーマンスに影響するため、**Awake または Start でキャッシュ**するのが基本です。

```csharp
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Rigidbody rb;

    void Awake()
    {
        // Awake でキャッシュする（毎フレーム呼ばない）
        rb = GetComponent<Rigidbody>();

        // 取得できなかった場合の null チェック
        if (rb == null)
        {
            Debug.LogError("Rigidbody が見つかりません");
        }
    }

    void Update()
    {
        rb.AddForce(Vector3.forward); // キャッシュ済みの rb を使う
    }
}
```

### 4-2. 他の GameObject を参照する方法

| 方法 | パフォーマンス | 推奨度 | 備考 |
|---|---|---|---|
| **SerializeField でアサイン** | 高速 | ✅ 推奨 | Inspector から直接設定 |
| `GameObject.FindWithTag()` | 中程度 | ⚠️ 限定使用 | Awake/Start の1回のみ |
| `GameObject.Find()` | 低速 | ❌ 非推奨 | Awake/Start の1回のみ |
| `FindFirstObjectByType<T>()` | 中程度 | ⚠️ 限定使用 | Unity 2023.1 以降の新 API |

#### Inspector から直接アサインする（推奨）

```csharp
public class PlayerController : MonoBehaviour
{
    // Inspector でドラッグ&ドロップしてアサインする
    [SerializeField] private GameObject enemyObject;
    [SerializeField] private AudioSource bgmSource;
}
```

#### GameObject.FindWithTag()

```csharp
void Awake()
{
    // タグで検索（Awake/Start の1回のみにする）
    GameObject player = GameObject.FindWithTag("Player");
}
```

#### GameObject.Find()

```csharp
void Awake()
{
    // 名前で検索（コストが高いため Awake/Start の1回のみ）
    GameObject obj = GameObject.Find("PlayerObject");
}
```

#### Unity 6 以降の Find 系 API

> ⚠️ **Unity 6 以降**
> `FindObjectOfType<T>()` / `FindObjectsOfType<T>()` は Unity 2023.1 で deprecated になりました。
> Unity 6 以降は以下の新 API を使用してください。

```csharp
// 条件に合う最初のオブジェクトを取得（旧 FindObjectOfType の代替）
EnemyBase enemy = FindFirstObjectByType<EnemyBase>();

// 条件に合う任意のオブジェクトを取得（存在確認など高速な用途に）
EnemyBase anyEnemy = FindAnyObjectByType<EnemyBase>();

// 条件に合う全オブジェクトを取得（旧 FindObjectsOfType の代替）
EnemyBase[] allEnemies = FindObjectsByType<EnemyBase>(FindObjectsSortMode.None);
```

**バージョン別の対応：**

| Unity バージョン | 使用する API |
|---|---|
| Unity 2022 以前 | `FindObjectOfType<T>()` / `FindObjectsOfType<T>()` |
| Unity 2023.1 〜 | deprecated 警告が出る（動作はするが移行推奨） |
| Unity 6 以降 | `FindFirstObjectByType<T>()` / `FindAnyObjectByType<T>()` / `FindObjectsByType<T>()` |

### 4-3. GameObject の生成と削除

```csharp
[SerializeField] private GameObject bulletPrefab; // Inspector でプレハブをアサイン

// プレハブからインスタンスを生成する
GameObject bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);

// 位置・親を指定して生成する
GameObject bullet2 = Instantiate(bulletPrefab, spawnPoint.position, spawnPoint.rotation, parentTransform);

// GameObject を削除する
Destroy(gameObject);          // 即時削除
Destroy(gameObject, 3.0f);   // 3秒後に削除
```

---

## 5. Prefab（プレハブ）

**Prefab** は「GameObject のテンプレート」です。
同じ構成の GameObject を何度でも生成できます。

### Prefab の作り方

1. Hierarchy ウィンドウで GameObject を作成・設定します
2. その GameObject を Project ウィンドウにドラッグ&ドロップします
3. Project ウィンドウに青いアイコンのファイルが作成されます

### Override と Apply

- **Override**：Prefab から生成したインスタンスに加えた変更。Hierarchy 上では太字で表示されます
- **Apply**：Override した変更を Prefab 本体に反映すること。全インスタンスに変更が適用されます

> ⚠️ Apply は全インスタンスに影響します。意図せず他のシーンの設定が変わることがあるため注意してください。

🎮 Unity での使用例：スペースシューターでは弾・敵キャラクターを Prefab として作成し、
`Instantiate()` で動的に生成する設計を使います（[04_space-shooter/](../04_space-shooter/) で実装します）。

---

## 6. Unity エディタの主要ウィンドウ

| ウィンドウ | 役割 |
|---|---|
| **Scene ビュー** | シーンを編集する3D/2D ビュー |
| **Game ビュー** | 実際のゲーム画面のプレビュー（再生中はここで確認する） |
| **Hierarchy** | シーン内の全 GameObject を階層表示 |
| **Project** | プロジェクト内の全アセットをフォルダ形式で表示 |
| **Inspector** | 選択した GameObject の Component 一覧と設定値を編集 |
| **Console** | ログ・警告・エラーを表示（`Debug.Log()` の出力もここに出る） |

---

## 7. よくあるハマりどころ

### MissingReferenceException

削除した GameObject への参照が残っている場合に発生します。

```csharp
// 悪い例：Destroy 後もアクセスしている
Destroy(enemy);
enemy.TakeDamage(10); // MissingReferenceException

// 良い例：削除前に参照を null にする
Destroy(enemy);
enemy = null;

// または null チェックをしてからアクセスする
if (enemy != null) enemy.TakeDamage(10);
```

### GetComponent() を毎フレーム呼ぶ

`Update()` 内で `GetComponent()` を呼ぶとパフォーマンスが低下します。

```csharp
// 悪い例：毎フレーム GetComponent を呼んでいる
void Update()
{
    GetComponent<Rigidbody>().AddForce(Vector3.forward); // 毎フレームコストがかかる
}

// 良い例：Awake でキャッシュしておく
private Rigidbody rb;
void Awake() { rb = GetComponent<Rigidbody>(); }
void Update() { rb.AddForce(Vector3.forward); }
```

### FindObjectOfType を Unity 6 で使って deprecated 警告が出る

`FindObjectOfType<T>()` を使っている箇所を以下のように置き換えてください。

```csharp
// 旧 API（Unity 2023.1 以降で警告）
var manager = FindObjectOfType<GameManager>();

// 新 API（Unity 6 以降）
var manager = FindFirstObjectByType<GameManager>();
```

### 非アクティブな GameObject が Find 系で見つからない

`GameObject.Find()` / `FindWithTag()` / `FindFirstObjectByType()` は
デフォルトでは非アクティブな GameObject を検索しません。

```csharp
// FindObjectsByType はオプションで非アクティブも含めて検索できる
EnemyBase[] all = FindObjectsByType<EnemyBase>(
    FindObjectsInactive.Include,   // 非アクティブも含む
    FindObjectsSortMode.None
);
```

### Prefab の変更が全インスタンスに適用される

Inspector で変更した後に「Apply」ボタンを押すと Prefab 本体が更新され、
すべてのシーン・すべてのインスタンスに変更が反映されます。
特定のインスタンスだけ変えたい場合は Apply せずに Override のままにしてください。

---

## 8. バージョン別 API 変更まとめ（⚠️ 注意が必要な API）

| 旧 API | 廃止バージョン | 新 API |
|---|---|---|
| `FindObjectOfType<T>()` | Unity 2023.1〜 | `FindFirstObjectByType<T>()` |
| `FindObjectsOfType<T>()` | Unity 2023.1〜 | `FindObjectsByType<T>(FindObjectsSortMode)` |
| `GameObject.Find()` | 廃止ではないが非推奨 | `[SerializeField]` でアサインを推奨 |

> 💡 このような API 変更まとめが複数ファイルにまたがる場合は、
> `99_references/` などに一元化することも検討してください。

---

## 9. 参考リンク

- [Unity 公式マニュアル：GameObject](https://docs.unity3d.com/ja/current/Manual/class-GameObject.html)
- [Unity 公式 API：FindFirstObjectByType](https://docs.unity3d.com/6000.0/Documentation/ScriptReference/Object.FindFirstObjectByType.html)
- [Unity 公式マニュアル：Prefab](https://docs.unity3d.com/ja/current/Manual/Prefabs.html)
