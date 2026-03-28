/// <summary>
/// 対応ドキュメント：05_async.md
/// 概要：コルーチン・Awaitable・UniTask の3種類の非同期処理を対比形式で学ぶ
/// アタッチ先：シーン上の任意の GameObject（Play ボタンを押して Console を確認する）
/// </summary>

using System.Collections;
using System.Threading;
using UnityEngine;

// UniTask を使う場合は以下の using を追加する
// #if UNITASK_SUPPORT
// using Cysharp.Threading.Tasks;
// #endif

namespace TrainingCsharpUnity
{
    public class AsyncSample : MonoBehaviour
    {
        // -----------------------------------------------------------------------
        // Start：各非同期処理の例を起動する
        // -----------------------------------------------------------------------
        void Start()
        {
            // コルーチン版を起動する
            StartCoroutine(CoroutineExample());

            // Awaitable 版を起動する（Unity 2023.1 以降）
            _ = AwaitableExample();

            // CancellationToken の例を起動する
            _ = AwaitableWithCancel(destroyCancellationToken);

            // UniTask 版（UniTask がインストールされている場合のみ有効）
#if UNITASK_SUPPORT
            UniTaskExample().Forget();
            UniTaskWhenAllExample().Forget();
#endif
        }

        // -----------------------------------------------------------------------
        // CoroutineExample：コルーチン版（05_async.md §2）
        // IEnumerator と yield return を使った非同期処理の基本形
        // -----------------------------------------------------------------------
        private IEnumerator CoroutineExample()
        {
            Debug.Log("[コルーチン] 開始：2秒待ちます");
            yield return new WaitForSeconds(2.0f); // 2秒待つ
            Debug.Log("[コルーチン] 2秒経過しました");

            yield return null; // 次のフレームまで待つ
            Debug.Log("[コルーチン] 1フレーム経過しました");
        }

        // -----------------------------------------------------------------------
        // AwaitableExample：Awaitable 版（05_async.md §3）
        // Unity 2023.1 以降で使える標準搭載の async/await 対応クラス
        // コルーチンと同等の処理を async/await で書ける
        // -----------------------------------------------------------------------
        private async Awaitable AwaitableExample()
        {
            Debug.Log("[Awaitable] 開始：2秒待ちます");
            await Awaitable.WaitForSecondsAsync(2.0f); // コルーチンの WaitForSeconds に相当
            Debug.Log("[Awaitable] 2秒経過しました");

            await Awaitable.NextFrameAsync(); // コルーチンの yield return null に相当
            Debug.Log("[Awaitable] 1フレーム経過しました");
        }

        // -----------------------------------------------------------------------
        // AwaitableWithCancel：CancellationToken の例（05_async.md §3-4）
        // destroyCancellationToken を渡すと GameObject 破棄時に自動キャンセルされる
        // -----------------------------------------------------------------------
        private async Awaitable AwaitableWithCancel(CancellationToken ct)
        {
            Debug.Log("[Awaitable+Cancel] 開始：5秒待ちます（途中で GameObject を削除するとキャンセルされます）");

            try
            {
                await Awaitable.WaitForSecondsAsync(5.0f, ct);
                Debug.Log("[Awaitable+Cancel] 5秒経過しました（キャンセルなし）");
            }
            catch (System.OperationCanceledException)
            {
                // GameObject が破棄されたときにここに来る
                Debug.Log("[Awaitable+Cancel] キャンセルされました（GameObject が破棄された）");
            }
        }

        // -----------------------------------------------------------------------
        // UniTask 版（UniTask がインストールされている場合のみコンパイルされる）
        // UniTask をインストールしたら #if UNITASK_SUPPORT をプロジェクト設定で定義する、
        // または以下のコードの #if ブロックを直接コメント解除して使用する
        // UniTask のインストール手順：05_async.md §4-2 を参照
        // -----------------------------------------------------------------------
#if UNITASK_SUPPORT
        private async Cysharp.Threading.Tasks.UniTask UniTaskExample()
        {
            Debug.Log("[UniTask] 開始：2秒待ちます");
            await Cysharp.Threading.Tasks.UniTask.Delay(2000); // ミリ秒単位
            Debug.Log("[UniTask] 2秒経過しました");
        }

        private async Cysharp.Threading.Tasks.UniTask UniTaskWhenAllExample()
        {
            Debug.Log("[UniTask.WhenAll] 2つの処理を並行して開始します");

            // 2つの処理を並行して実行し、両方完了するまで待つ（Awaitable にはない機能）
            await Cysharp.Threading.Tasks.UniTask.WhenAll(
                TaskA(),
                TaskB()
            );

            Debug.Log("[UniTask.WhenAll] 両方の処理が完了しました");
        }

        private async Cysharp.Threading.Tasks.UniTask TaskA()
        {
            await Cysharp.Threading.Tasks.UniTask.Delay(1000);
            Debug.Log("[UniTask.WhenAll] TaskA 完了（1秒）");
        }

        private async Cysharp.Threading.Tasks.UniTask TaskB()
        {
            await Cysharp.Threading.Tasks.UniTask.Delay(3000);
            Debug.Log("[UniTask.WhenAll] TaskB 完了（3秒）");
        }
#endif
    }
}
