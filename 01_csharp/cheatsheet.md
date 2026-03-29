# C# チートシート

研修中・研修後の手元リファレンス。各セクションへのリンク：

[1. 型と変数](#1-型と変数) ／
[2. 型変換](#2-型変換クイックリファレンス) ／
[3. 制御構文](#3-制御構文) ／
[4. クラス・オブジェクト](#4-クラスオブジェクト) ／
[5. 継承・インターフェース](#5-継承インターフェース) ／
[6. コレクション](#6-コレクション) ／
[7. LINQ](#7-linq-クイックリファレンス) ／
[8. Unity 関連記法](#8-よく使う-unity-関連の-c-記法) ／
[9. Visual Studio ショートカット](#9-よく使うショートカットvisual-studio) ／
[10. 例外処理](#10-例外処理)

---

## 1. 型と変数

### 主要な型一覧

| 型 | 用途 | 備考 |
|---|---|---|
| `int` | 整数 | -2,147,483,648 〜 2,147,483,647 |
| `float` | 小数（Unity 標準） | リテラルに `f` が必要（例：`1.5f`） |
| `double` | 高精度小数 | `float` より精度が高い。Unity では `float` を使うことが多い |
| `bool` | 真偽値 | `true` / `false` のみ |
| `char` | 1文字 | シングルクォート（例：`'A'`） |
| `string` | 文字列 | 参照型だが値型のように扱える |
| `var` | 型推論 | コンパイル時に型が確定する。型安全 |

### 宣言・初期化

```csharp
int    score      = 100;
float  speed      = 3.5f;       // f を忘れずに
double precision  = 1.23456789;
bool   isAlive    = true;
char   grade      = 'A';
string name       = "勇者";
var    count      = 10;         // int として推論される
string formatted  = $"名前：{name}、スコア：{score}"; // 文字列補間
```

### const と readonly

```csharp
const int MaxHp = 100;              // コンパイル時定数。暗黙的に static
readonly int startLevel;            // 実行時定数。コンストラクタでのみ代入可
```

### null 許容型と null 合体演算子

```csharp
int?   nullableInt  = null;         // 値型への null 代入
string name2        = null;

string display = name2 ?? "名無し"; // null なら右辺を使う
int    value   = nullableInt ?? 0;  // null なら 0

enemy?.TakeDamage(10);              // null 条件演算子：null なら何もしない
int? hp = enemy?.Hp;                // null なら hp = null
```

---

## 2. 型変換クイックリファレンス

```csharp
// int → float（暗黙変換）
int   i = 10;
float f = i;                        // 自動変換

// float → int（小数点以下切り捨て）
float f2 = 3.7f;
int   i2 = (int)f2;                 // i2 = 3（切り捨て。四捨五入ではない）

// string → int
int parsed  = int.Parse("42");      // 失敗すると例外が発生する
bool ok     = int.TryParse("abc", out int result); // 失敗しても例外が出ない

// int → string
string s = 42.ToString();
string s2 = $"{42}";               // 文字列補間でも変換できる

// object → 任意の型（キャスト）
object obj   = "テキスト";
string str   = (string)obj;        // 失敗すると InvalidCastException
string str2  = obj as string;      // 失敗すると null（例外は出ない）
```

---

## 3. 制御構文

### if / else if / else

```csharp
if (hp <= 0)       { /* 死亡 */ }
else if (hp <= 30) { /* 瀕死 */ }
else               { /* 通常 */ }
```

### switch

```csharp
// 通常
switch (state)
{
    case "idle":    /* 処理 */ break;
    case "running": /* 処理 */ break;
    default:                   break;
}

// パターンマッチング（C# 7 以降）
switch (value)
{
    case int n when n > 0: /* 正の整数 */ break;
    case string s:         /* 文字列   */ break;
}
```

### for / foreach / while / do-while

```csharp
for (int i = 0; i < 10; i++) { }

foreach (string item in list) { }

while (count < 5) { count++; }

do { input = Read(); } while (input < 0);
```

### break / continue / return

```csharp
for (int i = 0; i < 10; i++)
{
    if (i == 3) continue; // 残りをスキップして次のループへ
    if (i == 7) break;    // ループを抜ける
}

int Add(int a, int b) { return a + b; }
```

### 三項演算子

```csharp
string result = score >= 60 ? "合格" : "不合格";
```

---

## 4. クラス・オブジェクト

### クラスの基本構造

```csharp
class Player
{
    // フィールド
    private string name;
    private int    hp;

    // コンストラクタ
    public Player(string name, int hp)
    {
        this.name = name;
        this.hp   = hp;
    }

    // メソッド
    public void TakeDamage(int damage)
    {
        hp -= damage;
        if (hp < 0) hp = 0;
    }
}

// インスタンス生成
Player player = new Player("勇者", 100);
player.TakeDamage(30);
```

### アクセス修飾子一覧

| 修飾子 | アクセスできる範囲 |
|---|---|
| `public` | どこからでも |
| `private` | クラス内のみ |
| `protected` | クラス内 + 派生クラス |
| `internal` | 同じアセンブリ内 |

### プロパティ

```csharp
// get / set
public int Hp
{
    get { return hp; }
    set { hp = value < 0 ? 0 : value; } // 値の検証
}

// 自動実装
public string Name { get; set; } = "名無し";

// 読み取り専用
public int MaxHp { get; } = 100;

// 式形式（C# 6 以降）
public bool IsDead => hp <= 0;
```

### static メンバー

```csharp
class GameManager
{
    public static int Score = 0;               // 全インスタンス共有

    public static void AddScore(int amount)
    {
        Score += amount;
    }
}

GameManager.AddScore(100); // インスタンス不要
```

### this キーワード

```csharp
public Player(string name, int hp)
{
    this.name = name; // this.name はフィールド、name はパラメータ
    this.hp   = hp;
}
```

### メソッドのオーバーロード

```csharp
public void Attack() { }                          // 引数なし
public void Attack(int damage) { }                // int
public void Attack(string skill, int damage) { }  // string + int
```

---

## 5. 継承・インターフェース

### 継承

```csharp
class Character
{
    protected string name;
    protected int    hp;

    public Character(string name, int hp) { this.name = name; this.hp = hp; }

    public virtual void Attack() { Console.WriteLine($"{name} が攻撃した"); }
}

class Player : Character
{
    public Player(string name, int hp) : base(name, hp) { } // base() で親を初期化

    public override void Attack()           // virtual を上書き
    {
        base.Attack();                      // 親の処理も呼べる
        Console.WriteLine("追加攻撃！");
    }
}

sealed class FinalBoss : Character { }     // sealed：これ以上継承不可
```

### abstract クラス

```csharp
abstract class Enemy
{
    public abstract void Attack();          // 実装なし・派生クラスで必須

    public void ShowName() { }             // 通常メソッドも持てる
}

class Slime : Enemy
{
    public override void Attack() { Console.WriteLine("体当たり！"); }
}

// new Enemy() はコンパイルエラー（abstract クラスはインスタンス化不可）
```

### インターフェース

```csharp
interface IDamageable                       // 先頭に I を付ける命名規則
{
    void TakeDamage(int damage);
}

interface IMovable
{
    void Move(float x, float y);
}

// 複数インターフェースの実装
class Player : Character, IDamageable, IMovable
{
    public void TakeDamage(int damage) { hp -= damage; }
    public void Move(float x, float y) { }
}

IDamageable target = new Player("勇者", 100); // インターフェース型で受け取れる
target.TakeDamage(30);
```

---

## 6. コレクション

### List\<T\>

```csharp
using System.Collections.Generic;

List<string> enemies = new List<string>();

enemies.Add("スライム");                    // 追加
enemies.Remove("スライム");                 // 値で削除
enemies.RemoveAt(0);                        // インデックスで削除
int count    = enemies.Count;               // 要素数
bool hasGob  = enemies.Contains("ゴブリン"); // 存在確認

foreach (string e in enemies) { }           // 全要素ループ

// foreach 中の削除は InvalidOperationException が発生するため逆順 for で行う
for (int i = enemies.Count - 1; i >= 0; i--)
{
    if (enemies[i] == "ゴブリン") enemies.RemoveAt(i);
}
```

### Dictionary\<TKey, TValue\>

```csharp
Dictionary<string, int> inventory = new Dictionary<string, int>();

inventory.Add("ポーション", 3);             // 追加
int n = inventory["ポーション"];            // キーで取得（存在しないと例外）
inventory["ポーション"] = 5;               // 更新

bool exists = inventory.ContainsKey("エーテル"); // キーの存在確認

// 安全な取得（推奨）
if (inventory.TryGetValue("ポーション", out int val)) { }

// 全ペアのループ
foreach (KeyValuePair<string, int> pair in inventory)
{
    Console.WriteLine($"{pair.Key}：{pair.Value}");
}
```

### その他のコレクション（宣言例）

```csharp
Queue<string> queue  = new Queue<string>(); // 先入れ先出し（FIFO）
Stack<string> stack  = new Stack<string>(); // 後入れ先出し（LIFO）
HashSet<int>  set    = new HashSet<int>();  // 重複なし・高速な存在確認
```

---

## 7. LINQ クイックリファレンス

```csharp
using System.Linq; // 必須
```

```csharp
List<int> scores = new List<int> { 45, 82, 60, 91, 38, 75 };

// Where：条件で絞り込む
List<int> passed = scores.Where(s => s >= 60).ToList();

// Select：要素を変換する
List<string> labels = scores.Select(s => $"{s}点").ToList();

// FirstOrDefault：最初の要素（なければデフォルト値）
int top = scores.FirstOrDefault(s => s >= 90); // 91
int missing = scores.FirstOrDefault(s => s > 100); // 0（見つからない場合）

// Count：条件に合う要素数
int passCount = scores.Count(s => s >= 60);

// OrderBy / OrderByDescending：並べ替え
List<int> asc  = scores.OrderBy(s => s).ToList();
List<int> desc = scores.OrderByDescending(s => s).ToList();

// ToList：IEnumerable<T> を List<T> に変換
List<int> result = scores.Where(s => s >= 60).ToList(); // ここで処理が確定する

// Any：条件を満たす要素が1つでも存在するか
bool anyHigh = scores.Any(s => s >= 90); // true
```

---

## 8. よく使う Unity 関連の C# 記法

### SerializeField / RequireComponent

```csharp
using UnityEngine;

[RequireComponent(typeof(Rigidbody))] // このコンポーネントを必須にする
public class PlayerController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5.0f; // private のまま Inspector に表示
    [SerializeField] private int   maxHp     = 100;
}
```

### コルーチン

```csharp
using System.Collections;
using UnityEngine;

public class Example : MonoBehaviour
{
    void Start()
    {
        StartCoroutine(FadeOut()); // コルーチンの開始
    }

    IEnumerator FadeOut()
    {
        yield return new WaitForSeconds(1.0f); // 1秒待つ
        // 1秒後の処理
        yield return null;                      // 1フレーム待つ
    }
}
```

### イベント（Action / UnityEvent）

```csharp
using System;
using UnityEngine;
using UnityEngine.Events;

public class EventExample : MonoBehaviour
{
    public event Action       OnDead;        // C# 標準イベント
    public UnityEvent         OnJump;        // Unity イベント（Inspector から設定可能）
    public UnityEvent<int>    OnDamaged;     // 引数付き Unity イベント

    void Die()
    {
        OnDead?.Invoke();      // 購読者がいる場合のみ呼び出す
        OnJump?.Invoke();
        OnDamaged?.Invoke(30);
    }
}
```

### null 条件演算子と Unity オブジェクトの注意点

```csharp
// 通常の C# オブジェクトには ?. が使える
component?.DoSomething();

// Unity オブジェクト（MonoBehaviour など）では注意が必要
// Destroy() 後、== null は true だが ?. は null とみなさないことがある
// Unity オブジェクトの null チェックは == null を使うのが安全
if (enemy != null)
{
    enemy.TakeDamage(10);
}
```

---

## 9. よく使うショートカット（Visual Studio）

| ショートカット | 動作 |
|---|---|
| `F5` | デバッグ実行 |
| `Ctrl+F5` | 通常実行（デバッガーなし） |
| `F9` | ブレークポイントの設置・解除 |
| `F10` | ステップオーバー（次の行へ進む） |
| `F11` | ステップイン（メソッドの中へ入る） |
| `Shift+F11` | ステップアウト（メソッドから出る） |
| `Ctrl+Space` | コード補完 |
| `Ctrl+K, Ctrl+C` | 選択行をコメントアウト |
| `Ctrl+K, Ctrl+U` | 選択行のコメント解除 |
| `Ctrl+Shift+B` | ビルド |

---

## 10. 例外処理

### try-catch-finally の基本形

```csharp
try
{
    // 例外が発生する可能性のある処理
}
catch (FormatException e)     // 具体的な例外を先に書く
{
    Debug.LogError(e.Message);
}
catch (Exception e)           // その他すべての例外（最後）
{
    Debug.LogException(e);
}
finally
{
    // 例外の有無に関わらず必ず実行（リソース解放など）
}
```

### throw の書き方

```csharp
// 新しい例外を送出する
throw new ArgumentException("引数が不正です");

// catch した例外を再送出する（スタックトレースを保持）
throw;   // OK
// throw e; は NG（スタックトレースがリセットされる）
```

### よく使う例外クラス

| 例外クラス | 発生する場面 |
|---|---|
| `NullReferenceException` | null のオブジェクトにアクセスした |
| `IndexOutOfRangeException` | 配列の範囲外にアクセスした |
| `ArgumentNullException` | null を渡してはいけない引数に null を渡した |
| `ArgumentException` | 引数が不正な値だった |
| `InvalidOperationException` | 無効な状態で操作が呼ばれた |
| `DivideByZeroException` | 0 で除算した |
| `FormatException` | `Parse` などで文字列形式が不正だった |
| `IOException` | ファイルの読み書きで問題が発生した |

---

## 参考リンク

- [Microsoft C# 公式ドキュメント](https://learn.microsoft.com/ja-jp/dotnet/csharp/)
