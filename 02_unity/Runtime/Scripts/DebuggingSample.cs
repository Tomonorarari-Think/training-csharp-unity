/// <summary>
/// 対応ドキュメント：04_debugging.md
/// 概要：Debug.Log の種類・変数の出力・条件付きコンパイル・デバッガー練習用の複雑な処理を学ぶ
/// アタッチ先：シーン上の任意の GameObject（Play ボタンを押して Console を確認する）
/// </summary>

using UnityEngine;
using System.Diagnostics;

namespace TrainingCsharpUnity
{
    public class DebuggingSample : MonoBehaviour
    {
        // ウォッチウィンドウで確認しやすいよう複数の変数を用意する
        private int    playerHp     = 100;
        private int    enemyCount   = 5;
        private float  score        = 0.0f;
        private bool   isGameOver   = false;
        private string playerName   = "勇者";

        // -----------------------------------------------------------------------
        // Start：各デバッグ手法のサンプルを呼び出す
        // -----------------------------------------------------------------------
        void Start()
        {
            ShowDebugLogs();
            DebugWithFormat();
            EditorOnlyLog("エディタ上でのみ表示されるメッセージ");
            ComplexCalculation();
        }

        // -----------------------------------------------------------------------
        // ShowDebugLogs：Log / LogWarning / LogError の使い分け（§2-1）
        // Console ウィンドウで色の違いを確認する
        // -----------------------------------------------------------------------
        private void ShowDebugLogs()
        {
            UnityEngine.Debug.Log("通常ログ（白）：処理が通過したことを確認する");
            UnityEngine.Debug.LogWarning("警告ログ（黄）：問題ではないが注意が必要な状況");
            UnityEngine.Debug.LogError("エラーログ（赤）：問題が発生している状況");
        }

        // -----------------------------------------------------------------------
        // DebugWithFormat：文字列補間を使った変数の出力（§2-2）
        // -----------------------------------------------------------------------
        private void DebugWithFormat()
        {
            UnityEngine.Debug.Log($"プレイヤー名：{playerName}、HP：{playerHp}、スコア：{score}");
            UnityEngine.Debug.Log($"現在の位置：{transform.position}");
            UnityEngine.Debug.Log($"敵の数：{enemyCount}、ゲームオーバー：{isGameOver}");
        }

        // -----------------------------------------------------------------------
        // EditorOnlyLog：[Conditional] 属性でエディタ上でのみ有効なログ（§2-3）
        // UNITY_EDITOR が定義されている環境（エディタ上）でのみ呼び出しが有効になる
        // ビルド後のプレイヤーでは呼び出し自体がコンパイル時に除去される
        // -----------------------------------------------------------------------
        [Conditional("UNITY_EDITOR")]
        private void EditorOnlyLog(string message)
        {
            UnityEngine.Debug.Log($"[EditorOnly] {message}");
        }

        // -----------------------------------------------------------------------
        // ComplexCalculation：デバッガー練習用の複雑な処理（§4・§5）
        //
        // 【練習手順】
        // 1. この関数の先頭行にブレークポイントを設置する（行番号左の余白をクリック）
        // 2. Unity を Play して Visual Studio にアタッチする（「Unity にアタッチ」ボタン）
        // 3. F10 でステップオーバーしながら変数の変化を確認する
        // 4. ウォッチウィンドウに "playerHp" や "score" を登録して値を監視する
        // -----------------------------------------------------------------------
        private void ComplexCalculation()
        {
            int damage = 30;

            // --- ここにブレークポイントを設置して F10 で1行ずつ追う ---
            playerHp -= damage;                    // damage を受ける

            if (playerHp <= 0)
            {
                playerHp  = 0;
                isGameOver = true;
                UnityEngine.Debug.Log("ゲームオーバー！");
            }
            else if (playerHp <= 30)
            {
                UnityEngine.Debug.LogWarning($"HP が残り少なくなっています：{playerHp}");
            }

            // スコア計算（条件によって加算量が変わる処理）
            for (int i = 0; i < enemyCount; i++)
            {
                // ブレークポイントに条件「i == 3」を設定して3体目のときだけ止まる練習
                float bonus = (i % 2 == 0) ? 1.5f : 1.0f; // 偶数番目はボーナス
                score += 100.0f * bonus;
            }

            UnityEngine.Debug.Log($"最終スコア：{score}、HP：{playerHp}、ゲームオーバー：{isGameOver}");
        }
    }
}
