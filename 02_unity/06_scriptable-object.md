# 06 — ScriptableObject によるデータ管理

## 目標

- ScriptableObject とは何かを説明できる
- `[CreateAssetMenu]` を使って ScriptableObject アセットを作れる
- MonoBehaviour から ScriptableObject を参照して値を使える
- データとロジックを分離する意義を理解する

---

## 1. ScriptableObject とは

**ScriptableObject** とは、「MonoBehaviour に依存しないデータコンテナとして使える Unity のクラス」です。

- GameObject にアタッチ不要
- `Assets/` フォルダにアセットとして保存できる
- 複数の GameObject から同じデータを共有できる

### MonoBehaviour との違い

| 項目 | MonoBehaviour | ScriptableObject |
|---|---|---|
| アタッチ先 | GameObject 必須 | 不要（Asset として保存） |
| ライフサイクル | Scene に依存 | Project に依存 |
| 主な用途 | ゲームロジック | データ管理 |
| メモリ | インスタンスごとに確保 | 複数から共有できる |

MonoBehaviour がゲームの「動き」を担当するのに対し、ScriptableObject は「データ」を担当します。

---

## 2. ScriptableObject の作り方

### ScriptableObject クラスの定義

```csharp
using UnityEngine;

// [CreateAssetMenu] を付けると Unity エディタのメニューからアセットを作成できる
[CreateAssetMenu(
    fileName = "EnemyData",
    menuName = "SpaceShooter/EnemyData")]
public class EnemyData : ScriptableObject
{
    public float speed;
    public int   hp;
    public int   score;
}
```

| 属性パラメータ | 説明 |
|---|---|
| `fileName` | アセット作成時のデフォルトファイル名 |
| `menuName` | `Assets → Create` メニューに表示されるパス |

### Unity エディタでのアセット作成手順

1. Project ウィンドウで右クリック
2. **Create → SpaceShooter → EnemyData** を選択
3. ファイル名を設定して Enter

これで `EnemyData.asset` ファイルが作成されます。Inspector でフィールドの値を直接編集できます。

---

## 3. ScriptableObject の参照方法

### MonoBehaviour から参照する

`[SerializeField]` で Inspector に公開し、アセットをドラッグ＆ドロップで設定します。

```csharp
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [SerializeField]
    private EnemyData enemyData; // Inspector でアセットを割り当てる

    private float speed;
    private int   hp;

    void Start()
    {
        // アセットの値を読み込む
        speed = enemyData.speed;
        hp    = enemyData.hp;
    }

    void Update()
    {
        // speed を使って移動する
        transform.Translate(Vector2.down * speed * Time.deltaTime);
    }

    public void TakeDamage(int damage)
    {
        hp -= damage;
        if (hp <= 0)
        {
            Destroy(gameObject);
        }
    }
}
```

---

## 4. 🎮 スペースシューターでの使用例

`04_space-shooter/` で実装した敵の種類ごとに ScriptableObject でデータを管理するとどうなるか、リファクタリング案として紹介します。

### アセットの例

| ファイル名 | speed | hp | score |
|---|---|---|---|
| `NormalEnemyData.asset` | 2 | 1 | 100 |
| `FastEnemyData.asset` | 4 | 1 | 200 |
| `BossEnemyData.asset` | 1 | 10 | 1000 |

### リファクタリング前（パラメータをコードにハードコード）

```csharp
// NG：敵の種類が増えるたびにコードを変更する必要がある
public class EnemyController : MonoBehaviour
{
    private float speed = 2f;  // ハードコード
    private int   hp    = 1;
    private int   score = 100;
}
```

### リファクタリング後（ScriptableObject でデータを外部化）

```csharp
// OK：データはアセットで管理、スクリプトは変更不要
[SerializeField] private EnemyData enemyData;

void Start()
{
    speed = enemyData.speed;
    hp    = enemyData.hp;
}
```

敵の種類が増えたときに **スクリプトを変更せずにアセットを追加するだけ** で対応できます。

→ [04_space-shooter/](../04_space-shooter/) の実装を参考に試してみてください。

---

## 5. ScriptableObject のメリット・注意点

### メリット

- **データとロジックを分離できる** — コードを変えずにデータだけ変更できる
- **同じデータを複数の GameObject で共有できる** — メモリ効率が良い
- **Inspector でデータを編集できる** — プログラマー以外でも調整しやすい
- **Prefab ごとにデータをコピーしなくて済む** — メンテナンスコストが下がる

### 注意点

**ゲーム実行中に値を変更すると、エディタ上のアセットに保存されてしまいます。**

```csharp
// NG：実行中に ScriptableObject のフィールドを直接書き換える
void TakeDamage(int damage)
{
    enemyData.hp -= damage; // ← アセットのデータが変わってしまう（ゲーム終了後も残る）
}

// OK：ランタイムの状態は MonoBehaviour のフィールドで管理する
private int currentHp; // ScriptableObject のコピーを保持

void Start()
{
    currentHp = enemyData.hp; // 初期値だけ ScriptableObject から読む
}

void TakeDamage(int damage)
{
    currentHp -= damage; // ランタイムの状態は自前のフィールドで管理
}
```

**ルール：**

- 初期値・設定値 → ScriptableObject で持つ
- 実行中の状態変化 → MonoBehaviour のフィールドで管理する

---

## 6. よくあるハマりどころ

### ScriptableObject の値をゲーム中に変更するとアセットが書き変わる

前述の通り、`enemyData.hp -= damage;` のような書き方はアセットを上書きします。
ランタイムの状態は必ず MonoBehaviour 側のフィールドに保持してください。

### [CreateAssetMenu] を付け忘れてメニューに出ない

`[CreateAssetMenu]` 属性が付いていないと、Unity の Create メニューに表示されません。
コードから `ScriptableObject.CreateInstance<T>()` を使えば作成できますが、通常は属性を付けるのが簡単です。

```csharp
// エディタスクリプトやランタイムでインスタンスを作る場合
EnemyData data = ScriptableObject.CreateInstance<EnemyData>();
```

### new で生成しようとする

```csharp
// NG：ScriptableObject は new で生成できない
EnemyData data = new EnemyData(); // 動作するが Unity の管理外になる

// OK：CreateInstance を使う
EnemyData data = ScriptableObject.CreateInstance<EnemyData>();
```

---

## 7. バージョン別 API 変更まとめ

ScriptableObject は Unity の初期から存在する安定した機能です。`[CreateAssetMenu]` 属性も Unity 5.1 以降から使用可能で、現在の Unity バージョンでの互換性に問題はありません。

---

## 8. 参考リンク

- [Unity 公式マニュアル — ScriptableObject](https://docs.unity3d.com/ja/current/Manual/class-ScriptableObject.html)
- [本リポジトリ 02_unity/01_scene-gameobject.md](01_scene-gameobject.md)
- [本リポジトリ 04_space-shooter/](../04_space-shooter/)
