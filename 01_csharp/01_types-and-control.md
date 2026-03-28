# 01. 型・変数・制御構文

---

## 1. C# とは

C# は Microsoft が開発した静的型付けのオブジェクト指向言語です。
.NET プラットフォーム上で動作し、Unity の公式スクリプト言語として採用されています。

💡 C言語経験者向け：C言語と文法が似ており、読み替えやすい部分が多いです。
大きく異なる点は **メモリ管理が不要** であることで、
ガベージコレクション（GC）が不要になったオブジェクトのメモリを自動で解放してくれます。
`malloc` / `free` を意識する必要はありません。

---

## 2. 変数と型

### 値型

```csharp
int    score    = 100;        // 整数
float  speed    = 3.5f;       // 小数（f を付けることが必須）
double distance = 1.23456789; // 高精度小数
bool   isAlive  = true;       // true / false
char   grade    = 'A';        // 1文字（シングルクォート）
```

🎮 Unity での使用例：
- `float` は座標・速度・時間など、Unity で最もよく使う型です
- `bool` はフラグ管理（`isDead`、`isGrounded` など）に使います

```csharp
float  playerX    = transform.position.x; // プレイヤーのX座標
float  jumpForce  = 5.0f;                 // ジャンプ力
bool   isGrounded = false;                // 接地判定
```

### 文字列

```csharp
string name    = "Unity";
string message = "スコア：" + score; // 文字列の連結
string info    = $"名前：{name}、スコア：{score}"; // 文字列補間（推奨）
```

💡 C言語経験者向け：`string` は `char[]` ではなく専用の型として扱います。
終端文字（`'\0'`）を意識する必要はなく、`+` 演算子で連結できます。

### 型推論（var）

```csharp
var count  = 10;      // int として推論される
var name   = "Unity"; // string として推論される
var speed  = 3.5f;    // float として推論される
```

💡 C言語経験者向け：`var` は型を省略できますが、**型安全**です。
コンパイル時に型が確定するため、後から別の型の値を代入することはできません。
JavaScript の `var`（動的型付け）とは異なります。

---

## 3. 定数と readonly

### const

コンパイル時に値が確定する定数です。

```csharp
const int MaxHp    = 100;
const float Gravity = 9.8f;
```

### readonly

実行時に初期化される定数です。コンストラクタ内で値を設定できます。

```csharp
readonly int startLevel;

public Player(int level)
{
    startLevel = level; // コンストラクタでのみ代入可能
}
```

| | const | readonly |
|---|---|---|
| 初期化タイミング | コンパイル時 | 実行時（コンストラクタ内） |
| static | 暗黙的に static | 明示的に指定可能 |

🎮 Unity での使用例：マジックナンバーを避けるために使います。

```csharp
// 悪い例
if (hp > 100) { ... }

// 良い例
const int MaxHp = 100;
if (hp > MaxHp) { ... }
```

---

## 4. 制御構文

### 条件分岐

#### if / else if / else

```csharp
int hp = 50;

if (hp <= 0)
{
    // 死亡処理
}
else if (hp <= 30)
{
    // 瀕死演出
}
else
{
    // 通常状態
}
```

#### switch

```csharp
string state = "running";

switch (state)
{
    case "idle":
        // 待機アニメーション
        break;
    case "running":
        // 走りアニメーション
        break;
    default:
        break;
}
```

C# 7 以降ではパターンマッチングを使った書き方もできます。

```csharp
object value = 42;

switch (value)
{
    case int n when n > 0:
        // 正の整数の場合
        break;
    case string s:
        // 文字列の場合
        break;
}
```

### 繰り返し

#### for

```csharp
for (int i = 0; i < 10; i++)
{
    // 10回繰り返す
}
```

#### foreach

コレクション（配列・リストなど）の全要素を順に処理するときに使います。

```csharp
string[] enemies = { "スライム", "ゴブリン", "ドラゴン" };

foreach (string enemy in enemies)
{
    Debug.Log(enemy + "を倒した！");
}
```

🎮 Unity での使用例：シーン内のオブジェクト一覧を処理するときによく使います。

```csharp
// リスト内の全ての敵にダメージを与える
foreach (Enemy enemy in enemyList)
{
    enemy.TakeDamage(10);
}
```

💡 C言語経験者向け：`foreach` は C言語にはありませんが、C++ の範囲 for 文（`for (auto x : v)`）に相当します。

#### while

```csharp
int count = 0;

while (count < 5)
{
    count++;
}
```

#### do-while

条件に関わらず最低1回は実行されます。

```csharp
int input;

do
{
    input = GetPlayerInput(); // 入力を取得
} while (input < 0); // 負の値なら再入力
```

### ジャンプ構文

```csharp
for (int i = 0; i < 10; i++)
{
    if (i == 3) continue; // このループの残りをスキップして次へ
    if (i == 7) break;    // ループを抜ける
}
```

```csharp
int Add(int a, int b)
{
    return a + b; // 値を返して関数を終了する
}
```

---

## 5. null と null 許容型

### null とは

オブジェクトが「存在しない」状態を表す値です。
参照型（`string`、クラスのインスタンスなど）に代入できます。

```csharp
string name = null; // 値がない状態
```

### null 許容型（Nullable）

値型は通常 `null` を代入できませんが、`?` を付けると null 許容になります。

```csharp
int  score  = null; // コンパイルエラー
int? score2 = null; // OK
```

### null 合体演算子（??）

値が `null` のときに代わりの値を返します。

```csharp
string playerName = null;
string displayName = playerName ?? "名無し"; // null なら "名無し" を使う
```

### よくあるハマりどころ：NullReferenceException

Unity で最もよく遭遇するエラーのひとつです。
`null` のオブジェクトに対してメソッドやプロパティにアクセスしようとすると発生します。

```csharp
// 悪い例：enemy が null かもしれない状態でアクセスしている
enemy.TakeDamage(10); // NullReferenceException の原因になる

// 良い例：null チェックをしてからアクセスする
if (enemy != null)
{
    enemy.TakeDamage(10);
}

// 短く書く場合（null 条件演算子）
enemy?.TakeDamage(10); // enemy が null なら何もしない
```

---

## 6. 型変換

### 暗黙の型変換

小さい型から大きい型への変換は自動で行われます。

```csharp
int   i = 10;
float f = i; // int → float は自動変換（情報が失われないため）
```

### 明示的な型変換（キャスト）

精度が下がる変換は明示的に指定が必要です。

```csharp
float f = 3.7f;
int   i = (int)f; // 小数点以下は切り捨て（i = 3）
```

### Convert クラスと Parse メソッド

文字列から数値に変換する場合に使います。

```csharp
string text  = "42";
int    value = int.Parse(text);    // 失敗すると例外が発生する
int    value2 = Convert.ToInt32(text); // 同様だが null を渡すと 0 になる
```

変換に失敗する可能性がある場合は `TryParse` を使います。

```csharp
string input = "abc";
bool success = int.TryParse(input, out int result);

if (success)
{
    // result に変換後の値が入る
}
else
{
    // 変換に失敗した場合の処理
}
```

---

## 7. よくあるハマりどころまとめ

### int ÷ int が小数にならない

```csharp
int   a      = 7;
int   b      = 2;
float result = a / b; // 3.5f ではなく 3.0f になる（整数除算）

// 解決策：どちらかを float にキャストする
float result2 = (float)a / b; // 3.5f
```

### float リテラルに f を付け忘れる

```csharp
float speed = 1.5;  // コンパイルエラー（1.5 は double 型）
float speed2 = 1.5f; // OK
```

### string の比較

C# では `==` で文字列の値を比較できます。ただし `null` との比較には注意が必要です。

```csharp
string a = "hello";
string b = "hello";
bool equal = (a == b); // true（値で比較）

// null との比較
string name = null;
bool isNull = (name == null);         // true
bool isEmpty = string.IsNullOrEmpty(name); // null と空文字の両方をチェックできる
```

💡 C言語経験者向け：C# に `printf` はありません。
コンソールへの出力には `Console.WriteLine()` を、
Unity 上でのデバッグには `Debug.Log()` を使います。

```csharp
Console.WriteLine("スコア：" + score); // コンソールアプリ向け
Debug.Log("スコア：" + score);         // Unity エディタのコンソール向け
```

---

## 8. 参考リンク

- [Microsoft C# ドキュメント：組み込み型一覧](https://learn.microsoft.com/ja-jp/dotnet/csharp/language-reference/builtin-types/built-in-types)
- [Microsoft C# ドキュメント：選択ステートメント（if / switch）](https://learn.microsoft.com/ja-jp/dotnet/csharp/language-reference/statements/selection-statements)
