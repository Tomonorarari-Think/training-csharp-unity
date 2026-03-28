// 対応章：04章「コレクション・LINQ」
// 演習内容：List・Dictionary の基本操作と LINQ のクエリ記述の解答例

using System;
using System.Collections.Generic;
using System.Linq;

class Answer04
{
    static void Main()
    {
        // ============================================================
        // 問題1：List の基本操作
        // ============================================================

        // List<T> の宣言と初期化（空のリストを作成）
        List<string> enemies = new List<string>();

        // Add で要素を追加する
        enemies.Add("スライム");
        enemies.Add("ゴブリン");
        enemies.Add("ドラゴン");

        // foreach で全要素を出力する
        Console.WriteLine("--- 全敵リスト ---");
        foreach (string enemy in enemies)
        {
            Console.WriteLine(enemy);
        }

        // Remove で指定した値の要素を削除する
        // 同じ値が複数ある場合は最初の1つだけ削除される
        enemies.Remove("ゴブリン");

        // Count で現在の要素数を取得する（削除後は 2）
        Console.WriteLine($"削除後の要素数：{enemies.Count}"); // 2


        // ============================================================
        // 問題2：Dictionary の基本操作
        // ============================================================

        // Dictionary<TKey, TValue> の宣言と初期化
        Dictionary<string, int> inventory = new Dictionary<string, int>();

        // Add でキーと値のペアを追加する
        inventory.Add("HP回復薬", 3);
        inventory.Add("毒消し",   1);
        inventory.Add("エーテル", 2);

        // キーで値にアクセスする
        // 存在しないキーを指定すると KeyNotFoundException が発生するため注意
        Console.WriteLine($"HP回復薬の所持数：{inventory["HP回復薬"]}"); // 3

        // ContainsKey でキーの存在を確認してから出力する
        if (inventory.ContainsKey("毒消し"))
        {
            Console.WriteLine("毒消し：あり");
        }
        else
        {
            Console.WriteLine("毒消し：なし");
        }


        // ============================================================
        // 問題3：LINQ の基本操作
        // ============================================================

        List<int> scores = new List<int> { 45, 82, 60, 91, 38, 75 };

        // Where で条件に合う要素だけ絞り込む
        // ToList() を呼ぶことで IEnumerable<int> を List<int> に変換する
        List<int> passedScores = scores.Where(s => s >= 60).ToList();
        // → { 82, 60, 91, 75 }

        // OrderByDescending で降順に並べ替える
        // OrderBy を使うと昇順になる
        List<int> sortedScores = passedScores.OrderByDescending(s => s).ToList();
        // → { 91, 82, 75, 60 }

        // FirstOrDefault で最初の要素を取得する
        // リストが空の場合はデフォルト値（int なら 0）が返る
        int topScore = sortedScores.FirstOrDefault();
        Console.WriteLine($"最高スコア：{topScore}"); // 91


        // ============================================================
        // 問題4：よくあるハマりどころの修正
        // ============================================================

        List<string> fixedList = new List<string> { "スライム", "ゴブリン", "ドラゴン" };

        // --- 解答①：逆順の for ループを使う ---
        // 後ろから削除することでインデックスがずれる問題を回避する
        for (int i = fixedList.Count - 1; i >= 0; i--)
        {
            if (fixedList[i] == "ゴブリン")
            {
                fixedList.RemoveAt(i);
            }
        }

        Console.WriteLine("--- 解答①（逆順 for ループ）---");
        foreach (var enemy in fixedList)
        {
            Console.WriteLine(enemy); // スライム、ドラゴン
        }

        // --- 解答②：削除対象を別リストに集めてから削除する ---
        List<string> fixedList2 = new List<string> { "スライム", "ゴブリン", "ドラゴン" };

        // 削除したい要素を別リストに集める
        List<string> toRemove = new List<string>();
        foreach (var enemy in fixedList2)
        {
            if (enemy == "ゴブリン") toRemove.Add(enemy);
        }

        // ループが終わってから削除する（foreach の外で Remove する）
        foreach (var enemy in toRemove)
        {
            fixedList2.Remove(enemy);
        }

        Console.WriteLine("--- 解答②（別リストを使う）---");
        foreach (var enemy in fixedList2)
        {
            Console.WriteLine(enemy); // スライム、ドラゴン
        }

        // --- LINQ を使った書き方（参考）---
        // Where で "ゴブリン" 以外を残して新しいリストを作る方法もある
        List<string> fixedList3 = new List<string> { "スライム", "ゴブリン", "ドラゴン" };
        fixedList3 = fixedList3.Where(e => e != "ゴブリン").ToList();

        Console.WriteLine("--- 参考（LINQ の Where を使う）---");
        foreach (var enemy in fixedList3)
        {
            Console.WriteLine(enemy); // スライム、ドラゴン
        }
    }
}
