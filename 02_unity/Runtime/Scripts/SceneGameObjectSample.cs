/// <summary>
/// 対応ドキュメント：01_scene-gameobject.md
/// 概要：GetComponent・SerializeField・Instantiate・Destroy・FindWithTag の基本的な使い方を学ぶ
/// アタッチ先：シーン上の任意の空の GameObject（例：SampleManager）
/// </summary>

using UnityEngine;

namespace TrainingCsharpUnity
{
    public class SceneGameObjectSample : MonoBehaviour
    {
        // --- SerializeField：private フィールドを Inspector に公開する（01_scene-gameobject.md §3）---
        [SerializeField] private GameObject enemyPrefab;   // 生成する敵プレハブ
        [SerializeField] private Transform  spawnPoint;    // 生成位置

        // Awake でキャッシュするコンポーネント
        private Renderer cachedRenderer;

        // 生成した敵を保持する変数
        private GameObject spawnedEnemy;

        // -----------------------------------------------------------------------
        // Awake：自分自身のコンポーネントをキャッシュする（01_scene-gameobject.md §4-1）
        // -----------------------------------------------------------------------
        void Awake()
        {
            // GetComponent は Awake でキャッシュしておく
            // Update など毎フレーム呼ばれるメソッドで呼ぶとパフォーマンスが低下する
            cachedRenderer = GetComponent<Renderer>();

            if (cachedRenderer == null)
            {
                Debug.LogWarning($"{gameObject.name} に Renderer が見つかりません（なくても動作します）");
            }
        }

        // -----------------------------------------------------------------------
        // Start：他オブジェクトへの参照が必要な初期化処理（01_scene-gameobject.md §4-2）
        // -----------------------------------------------------------------------
        void Start()
        {
            // --- GameObject.FindWithTag の例 ---
            // ⚠️ FindWithTag はコストがかかるため Awake/Start の1回のみに限定する
            // SerializeField でアサインできる場合はそちらを優先すること
            GameObject player = GameObject.FindWithTag("Player");
            if (player != null)
            {
                Debug.Log($"Player オブジェクトを見つけました：{player.name}");
            }
            else
            {
                Debug.Log("Player タグのオブジェクトが見つかりませんでした（シーンに存在しない場合）");
            }

            // --- Unity 6 以降：FindFirstObjectByType の例 ---
            // ⚠️ FindObjectOfType は Unity 2023.1 で deprecated（廃止予定）
            // Unity 6 以降では FindFirstObjectByType を使うこと
            // SceneGameObjectSample first = FindFirstObjectByType<SceneGameObjectSample>();
            // Debug.Log($"FindFirstObjectByType の結果：{first?.gameObject.name}");
        }

        // -----------------------------------------------------------------------
        // SpawnEnemy：Instantiate でプレハブから GameObject を生成する（§4-3）
        // Inspector の「SpawnEnemy」ボタンまたは他スクリプトから呼び出せる
        // -----------------------------------------------------------------------
        public void SpawnEnemy()
        {
            if (enemyPrefab == null)
            {
                Debug.LogError("enemyPrefab が設定されていません。Inspector でアサインしてください。");
                return;
            }

            // 生成位置が設定されている場合はそこに生成、なければ自分の位置に生成
            Vector3 position = spawnPoint != null ? spawnPoint.position : transform.position;

            // Instantiate でプレハブからインスタンスを生成する
            spawnedEnemy = Instantiate(enemyPrefab, position, Quaternion.identity);
            Debug.Log($"敵を生成しました：{spawnedEnemy.name}（位置：{position}）");
        }

        // -----------------------------------------------------------------------
        // DestroyEnemy：Destroy で GameObject を削除する（§4-3）
        // -----------------------------------------------------------------------
        public void DestroyEnemy()
        {
            if (spawnedEnemy == null)
            {
                Debug.LogWarning("削除する敵が存在しません。先に SpawnEnemy を呼び出してください。");
                return;
            }

            Debug.Log($"敵を削除します：{spawnedEnemy.name}");
            Destroy(spawnedEnemy);           // 即時削除
            // Destroy(spawnedEnemy, 3.0f); // 3秒後に削除する場合はこちら
            spawnedEnemy = null;
        }
    }
}
