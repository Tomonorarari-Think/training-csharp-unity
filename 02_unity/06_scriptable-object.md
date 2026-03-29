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

## 4. 🎮 スペースシューターへの適用例（発展）

現在の模範解答（`04_space-shooter/solution/`）では ScriptableObject を使用していません。
まずは模範解答の構成で動くものを作ることを優先してください。

動くものができた後の発展課題として、以下のリファクタリングを検討してみましょう。

### 敵のデータを ScriptableObject で管理する

現在 `EnemyController` に直接書かれている `speed`・`hp`・`score` の値を ScriptableObject に切り出すと、スクリプトを変更せずにデータだけ追加・編集できるようになります。

```csharp
[CreateAssetMenu(
    fileName = "EnemyData",
    menuName = "SpaceShooter/EnemyData")]
public class EnemyData : ScriptableObject
{
    public float speed;   // 移動速度
    public int   hp;      // 体力
    public int   score;   // 撃破スコア
}

// EnemyController 側での参照
[SerializeField]
private EnemyData enemyData;
```

### この変更で何が嬉しいか

- 敵の種類が増えても `EnemyController` を修正不要
- Inspector 上でデータを編集できる
- データとロジックが分離されてコードが読みやすくなる

### 発展課題として試してみましょう

1. `EnemyData` ScriptableObject を作成する
2. `EnemyController` に `[SerializeField] EnemyData` を追加する
3. 既存の数値を ScriptableObject 経由に変更する
4. 複数の `EnemyData` アセットを作って敵の種類を増やす

詰まった場合は [04_space-shooter/hints.md](../04_space-shooter/hints.md) を参照してください。

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
