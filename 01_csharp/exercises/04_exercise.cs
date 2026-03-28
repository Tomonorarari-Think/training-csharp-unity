// 対応章：04章「コレクション・LINQ」
// 演習内容：List・Dictionary の基本操作と LINQ のクエリ記述を練習します

using System;
using System.Collections.Generic;
using System.Linq;

class Exercise04
{
    static void Main()
    {
        // ============================================================
        // 問題1：List の基本操作
        // ============================================================

        // TODO: string 型の List を作成し
        //       "スライム"・"ゴブリン"・"ドラゴン" を Add で追加してください
        // List<string> enemies = ???;


        // TODO: foreach で全要素を Console.WriteLine で出力してください


        // TODO: "ゴブリン" を List から Remove してください


        // TODO: 削除後の List の要素数を Console.WriteLine で出力してください
        //       期待値：2


        // ============================================================
        // 問題2：Dictionary の基本操作
        // ============================================================

        // TODO: string をキー、int を値とする Dictionary を作成し
        //       以下のデータを Add で追加してください
        //       "HP回復薬":3、"毒消し":1、"エーテル":2
        // Dictionary<string, int> inventory = ???;


        // TODO: "HP回復薬" の所持数を Console.WriteLine で出力してください
        //       期待値：3


        // TODO: "毒消し" が Dictionary に存在するか確認し
        //       結果を Console.WriteLine で出力してください
        //       ヒント：ContainsKey を使う
        //       期待値："毒消し：あり" または "毒消し：なし"


        // ============================================================
        // 問題3：LINQ の基本操作
        // ============================================================

        List<int> scores = new List<int> { 45, 82, 60, 91, 38, 75 };

        // TODO: Where を使って 60 以上のスコアだけ抽出し
        //       passedScores という List<int> に格納してください
        //       ヒント：.Where(s => s >= 60).ToList()
        // List<int> passedScores = ???;


        // TODO: passedScores を OrderByDescending で降順に並べ替え
        //       sortedScores という List<int> に格納してください
        // List<int> sortedScores = ???;


        // TODO: FirstOrDefault を使って sortedScores の最初の要素（最高スコア）を
        //       topScore という変数に取得し、Console.WriteLine で出力してください
        //       期待値：91


        // ============================================================
        // 問題4：よくあるハマりどころの確認
        // ============================================================

        // 以下のコードにはバグがあります。修正してください。
        // バグの内容：foreach でループ中に Remove を呼ぶと例外が発生する

        List<string> buggyList = new List<string> { "スライム", "ゴブリン", "ドラゴン" };

        // ↓ このコードを修正してください（foreach の中で Remove しないようにする）
        foreach (var enemy in buggyList)
        {
            if (enemy == "ゴブリン")
            {
                buggyList.Remove(enemy); // ← ここにバグがある
            }
        }

        Console.WriteLine("問題4の結果：");
        foreach (var enemy in buggyList)
        {
            Console.WriteLine(enemy);
        }
    }
}
