# 04. コレクション・LINQ

---

## 1. コレクションとは

**コレクション**は、複数のデータをまとめて扱うための仕組みです。
C# には配列もありますが、コレクションは要素を動的に追加・削除できる点が異なります。

| | 配列 | コレクション |
|---|---|---|
| サイズ | 固定（宣言時に決まる） | 可変（実行中に増減できる） |
| 速度 | 高速 | 若干のオーバーヘッドあり |
| 操作 | 基本的なもののみ | 追加・削除・検索など豊富 |

配列はサイズが決まっている場合に向いており、
コレクションは要素数が変わる場合に向いています。

🎮 Unity での使用例：コレクションが必要になる場面の例

- 出現する敵の管理（増えたり減ったりする）
- インベントリのアイテム一覧
- 弾丸のオブジェクトプール

💡 C言語経験者向け：C言語の配列や自作の連結リストに相当しますが、
C# では標準ライブラリとして充実したコレクションが用意されています。
メモリ管理も自動なので、要素の追加・削除を安全に行えます。

---

## 2. List\<T\>

`List<T>` は Unity で最もよく使うコレクションです。
`T` には格納したい型（`int`・`string`・`GameObject` など）を指定します。

### 宣言と初期化

```csharp
using System.Collections.Generic;

List<int>    scores  = new List<int>();           // 空のリスト
List<string> names   = new List<string> { "勇者", "魔法使い" }; // 初期値あり
```

### 基本操作

```csharp
List<string> enemies = new List<string>();

// 追加
enemies.Add("スライム");
enemies.Add("ゴブリン");
enemies.Add("ドラゴン");

// 要素数の取得
int count = enemies.Count; // 3

// インデックスによるアクセス
string first = enemies[0]; // "スライム"

// 存在確認
bool hasSlime = enemies.Contains("スライム"); // true

// 削除（値で指定）
enemies.Remove("ゴブリン"); // "ゴブリン" を削除

// 削除（インデックスで指定）
enemies.RemoveAt(0); // 先頭の要素を削除

// 全要素をループ処理
foreach (string enemy in enemies)
{
    Console.WriteLine(enemy);
}
```

### 🎮 Unity での使用例：敵オブジェクトの管理

```csharp
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    private List<GameObject> enemyList = new List<GameObject>();

    // 敵をリストに追加する
    public void RegisterEnemy(GameObject enemy)
    {
        enemyList.Add(enemy);
    }

    // 倒された敵をリストから削除する
    public void RemoveEnemy(GameObject enemy)
    {
        enemyList.Remove(enemy);
        Destroy(enemy);
    }

    // 全ての敵にダメージを与える
    public void DamageAllEnemies(int damage)
    {
        foreach (GameObject enemy in enemyList)
        {
            enemy.GetComponent<EnemyBase>().TakeDamage(damage);
        }
    }
}
```

### よくあるハマりどころ：foreach 中の削除

`foreach` でループしながら要素を削除すると例外が発生します。

```csharp
// 悪い例：foreach 中に Remove すると InvalidOperationException が発生する
foreach (GameObject enemy in enemyList)
{
    if (enemy == null)
    {
        enemyList.Remove(enemy); // エラー！
    }
}
```

**回避策①：逆順の for ループを使う**

```csharp
for (int i = enemyList.Count - 1; i >= 0; i--)
{
    if (enemyList[i] == null)
    {
        enemyList.RemoveAt(i); // 後ろから削除するのでインデックスがずれない
    }
}
```

**回避策②：削除する要素を別リストに集めてから削除する**

```csharp
List<GameObject> toRemove = new List<GameObject>();

foreach (GameObject enemy in enemyList)
{
    if (enemy == null) toRemove.Add(enemy);
}

foreach (GameObject enemy in toRemove)
{
    enemyList.Remove(enemy);
}
```

---

## 3. Dictionary\<TKey, TValue\>

`Dictionary<TKey, TValue>` はキーと値のペアを管理するコレクションです。
キーを使って高速に値を取得できます。

### 宣言と初期化

```csharp
using System.Collections.Generic;

// アイテム名（string）と所持数（int）を管理する
Dictionary<string, int> inventory = new Dictionary<string, int>();

// 初期値あり
Dictionary<string, int> itemCount = new Dictionary<string, int>
{
    { "ポーション", 3 },
    { "エーテル",   1 },
};
```

### 基本操作

```csharp
Dictionary<string, int> inventory = new Dictionary<string, int>();

// キーと値のペアを追加
inventory.Add("ポーション", 5);
inventory.Add("エーテル",   2);

// キーで値にアクセス・更新
int potionCount = inventory["ポーション"]; // 5
inventory["ポーション"] = 3;               // 値を更新

// キーの存在確認
bool hasPotion = inventory.ContainsKey("ポーション"); // true

// 安全な値の取得（TryGetValue）
if (inventory.TryGetValue("エーテル", out int count))
{
    Console.WriteLine($"エーテル：{count} 個");
}
else
{
    Console.WriteLine("エーテルは持っていない");
}

// 全ペアをループ処理
foreach (KeyValuePair<string, int> item in inventory)
{
    Console.WriteLine($"{item.Key}：{item.Value} 個");
}
```

🎮 Unity での使用例：

```csharp
public class Inventory : MonoBehaviour
{
    private Dictionary<string, int> items = new Dictionary<string, int>();

    // アイテムを追加する（既にあれば数を増やす）
    public void AddItem(string itemName, int amount)
    {
        if (items.ContainsKey(itemName))
        {
            items[itemName] += amount;
        }
        else
        {
            items.Add(itemName, amount);
        }
    }

    // アイテムを使う
    public bool UseItem(string itemName)
    {
        if (!items.TryGetValue(itemName, out int count) || count <= 0)
        {
            Debug.Log($"{itemName} は持っていない");
            return false;
        }
        items[itemName]--;
        return true;
    }
}
```

### よくあるハマりどころ：存在しないキーへのアクセス

```csharp
// 悪い例：キーが存在しない場合 KeyNotFoundException が発生する
int count = inventory["存在しないアイテム"]; // エラー！

// 良い例：TryGetValue で安全に取得する
if (inventory.TryGetValue("存在しないアイテム", out int count))
{
    // 存在する場合の処理
}
```

---

## 4. その他のコレクション（読み物）

用途に合わせて使い分けられるよう、名前と概要だけ押さえておいてください。

| コレクション | 特徴 | 主な用途 |
|---|---|---|
| `Queue<T>` | 先入れ先出し（FIFO） | タスクキュー・メッセージキュー |
| `Stack<T>` | 後入れ先出し（LIFO） | アンドゥ機能・履歴管理 |
| `HashSet<T>` | 重複なし・高速な存在確認 | 既訪問チェック・ユニーク管理 |

詳細は必要になったときに [Microsoft 公式ドキュメント](https://learn.microsoft.com/ja-jp/dotnet/csharp/programming-guide/concepts/collections) を参照してください。

---

## 5. LINQ の基礎

**LINQ**（Language Integrated Query）は、コレクションに対してクエリ（問い合わせ）を
簡潔に書けるようにする機能です。
使用するには `using System.Linq;` が必要です。

```csharp
using System.Linq;
```

### Where（条件で絞り込む）

```csharp
List<int> scores = new List<int> { 85, 42, 90, 55, 78 };

// 60点以上のスコアだけ取り出す
List<int> passed = scores.Where(s => s >= 60).ToList();
// → { 85, 90, 78 }
```

### Select（要素を変換する）

```csharp
List<string> names = new List<string> { "スライム", "ゴブリン", "ドラゴン" };

// 全ての名前に "（討伐済み）" を付ける
List<string> defeated = names.Select(n => n + "（討伐済み）").ToList();
// → { "スライム（討伐済み）", "ゴブリン（討伐済み）", "ドラゴン（討伐済み）" }
```

### FirstOrDefault（最初の要素を取得）

```csharp
List<int> scores = new List<int> { 85, 42, 90 };

// 90点以上の最初のスコアを取得（なければ 0）
int topScore = scores.FirstOrDefault(s => s >= 90); // 90

// 存在しない場合はデフォルト値（参照型は null、値型は 0 など）が返る
int missing = scores.FirstOrDefault(s => s >= 100); // 0
```

### Count（条件に合う要素数）

```csharp
int passedCount = scores.Count(s => s >= 60); // 条件を満たす要素の数
```

### OrderBy / OrderByDescending（並び替え）

```csharp
List<int> sorted     = scores.OrderBy(s => s).ToList();            // 昇順
List<int> sortedDesc = scores.OrderByDescending(s => s).ToList();  // 降順
```

### ToList（リストに変換）

LINQ の結果は `IEnumerable<T>` 型です。
`List<T>` として使いたい場合は `ToList()` で変換します。

```csharp
IEnumerable<int> query = scores.Where(s => s >= 60); // まだ処理されていない
List<int> result = query.ToList();                     // ここで初めて処理が走る
```

### 🎮 Unity での使用例：HP が 0 以下の敵を除去する

**通常の foreach を使った場合：**

```csharp
List<GameObject> toRemove = new List<GameObject>();

foreach (GameObject enemy in enemyList)
{
    if (enemy.GetComponent<EnemyBase>().Hp <= 0)
    {
        toRemove.Add(enemy);
    }
}

foreach (GameObject enemy in toRemove)
{
    enemyList.Remove(enemy);
    Destroy(enemy);
}
```

**LINQ を使った場合：**

```csharp
// HP が 0 以下の敵リストを取得
List<GameObject> deadEnemies = enemyList
    .Where(e => e.GetComponent<EnemyBase>().Hp <= 0)
    .ToList();

// 削除処理
foreach (GameObject enemy in deadEnemies)
{
    enemyList.Remove(enemy);
    Destroy(enemy);
}
```

LINQ を使うと「何を取り出したいか」が1行で明確に表現できます。

### よくあるハマりどころ：遅延評価

LINQ は `ToList()` や `foreach` が呼ばれるまで実際には処理が実行されません。

```csharp
// クエリを定義した時点ではまだ処理されていない
var query = scores.Where(s => s >= 60);

// この後に scores が変更されると、query の結果も変わる
scores.Add(95);

// ToList() が呼ばれた時点で処理が実行される（95 も含まれる）
List<int> result = query.ToList();
```

コレクションが変化しうる場合は、必要なタイミングで `ToList()` を呼ぶように意識してください。

---

## 6. var とコレクション

コレクションの型が明確なときは `var` を使うと記述が短くなります。

```csharp
// 型が長くなる場合は var を使うと読みやすい
var enemies  = new List<GameObject>();
var inventory = new Dictionary<string, int>();

// LINQ の結果も var でよく受け取る
var highScorers = scores.Where(s => s >= 80).ToList();
```

型が一目でわかる状況では `var` を積極的に使って構いません。
ただし、何の型かが不明瞭になる場合は明示的な型名を使う方が読みやすくなります。

---

## 7. よくあるハマりどころまとめ

### foreach 中に List の要素を削除する

→ 逆順の `for` ループか、削除対象を別リストに集めてから削除する。

### Dictionary の存在しないキーにアクセスする

→ `TryGetValue` か `ContainsKey` で事前に確認する。

### LINQ の遅延評価による意図しない挙動

→ 結果をすぐに確定させたい場合は `ToList()` を呼ぶ。

### using System.Linq を忘れる

```csharp
// このファイルの先頭に必要
using System.Linq;
```

LINQ のメソッド（`Where`・`Select` など）が使えない場合はまずここを確認してください。

---

## 8. 参考リンク

- [Microsoft C# ドキュメント：コレクション](https://learn.microsoft.com/ja-jp/dotnet/csharp/programming-guide/concepts/collections)
- [Microsoft C# ドキュメント：LINQ](https://learn.microsoft.com/ja-jp/dotnet/csharp/linq/)
