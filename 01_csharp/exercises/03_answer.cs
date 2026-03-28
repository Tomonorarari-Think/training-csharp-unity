// 対応章：03章「継承・抽象クラス・インターフェース」
// 演習内容：継承・抽象クラス・インターフェースの定義と実装の解答例
// 実行方法：01_csharp/how-to-run.md を参照してください

using System;

// ============================================================
// 問題3：インターフェースの定義
// ============================================================

// インターフェース名は先頭に I を付ける（命名規則）
// メソッドの実装は持たず、定義のみ記述する
interface IDamageable
{
    void TakeDamage(int damage);
}

// ============================================================
// 問題1：基底クラスの定義
// ============================================================

class Character
{
    // protected：派生クラスからアクセスできるが外部からは直接アクセスできない
    protected string name;
    protected int    hp;

    public Character(string name, int hp)
    {
        this.name = name;
        this.hp   = hp;
    }

    // virtual を付けることで派生クラスで override できる
    // virtual がない場合、派生クラスで override しようとするとコンパイルエラー
    public virtual void Attack()
    {
        Console.WriteLine($"{name} が攻撃した");
    }
}

// ============================================================
// 問題1 + 問題3：Player クラス（継承 + インターフェース実装）
// ============================================================

// コロン（:）の後に 継承するクラス、続いてインターフェースを列挙する
// クラスの継承は1つのみ、インターフェースは複数実装できる
class Player : Character, IDamageable
{
    // base(name, hp) で親クラスのコンストラクタを呼ぶ
    // base() を省くと親クラスの初期化が行われずコンパイルエラーになる
    public Player(string name, int hp) : base(name, hp) { }

    // override で親クラスの virtual メソッドを上書きする
    public override void Attack()
    {
        Console.WriteLine($"{name} がプレイヤー攻撃した");
    }

    // IDamageable の実装：インターフェースで要求されたメソッドを必ず実装する
    public void TakeDamage(int damage)
    {
        hp -= damage;
        Console.WriteLine($"{name} が {damage} ダメージを受けた");
    }
}

// ============================================================
// 問題2：抽象クラスと派生クラスの定義
// ============================================================

// abstract クラスは直接 new できない
// 派生クラスに Attack() の実装を強制する契約として機能する
abstract class Enemy
{
    protected string name;

    public Enemy(string name)
    {
        this.name = name;
    }

    // abstract メソッド：派生クラスで必ず override しなければコンパイルエラー
    public abstract void Attack();
}

// abstract クラスを継承し、abstract メソッドを実装する
class Slime : Enemy
{
    public Slime(string name) : base(name) { }

    // abstract メソッドの実装（override が必須）
    public override void Attack()
    {
        Console.WriteLine("スライムが体当たりした！");
    }
}

// ============================================================
// メインの実行コード
// ============================================================

class Answer03
{
    static void Main()
    {
        // 問題1の確認
        Player player = new Player("勇者", 100);
        player.Attack(); // 勇者 がプレイヤー攻撃した

        // 問題2の確認
        // abstract クラス Enemy は直接 new できないため派生クラスで生成する
        Enemy slime = new Slime("スライム");
        slime.Attack(); // スライムが体当たりした！

        // 問題3の確認
        // インターフェース型の変数に代入できる（Player が IDamageable を実装しているため）
        IDamageable target = new Player("勇者", 100);
        target.TakeDamage(30); // 勇者 が 30 ダメージを受けた

        // ============================================================
        // 問題4：is-a / has-a の判断（解答）
        // ============================================================

        // Player は Character である
        // → 継承（is-a）を使う
        //   「Player is-a Character」が成り立つので継承が適切

        // Player は Weapon を持つ
        // → has-a（フィールドとして持つ）を使う
        //   「Player is-a Weapon」とは言えないのでフィールドとして保持する

        // Slime は Enemy である
        // → 継承（is-a）を使う
        //   「Slime is-a Enemy」が成り立つので継承が適切
    }
}
