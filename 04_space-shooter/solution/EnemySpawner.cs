/// <summary>
/// EnemySpawner.cs
/// 概要：敵を格子状に一括生成し、GameManager に敵の登録を通知するスクリプト。
/// 対応設計ドキュメント：solution/README.md
/// </summary>

using UnityEngine;

namespace SpaceShooter
{
    /// <summary>
    /// 敵を格子状に一括生成するクラス。
    ///
    /// 敵の生成責務を GameManager から分離して EnemySpawner に切り出した理由：
    /// GameManager に生成処理も持たせると「ゲーム進行管理」と「敵の生成」という
    /// 2つの責任を持つことになり、単一責任の原則に違反する。
    /// 分離することでウェーブ管理・敵パターンの変更も容易になる。
    /// （03_design/01_solid-principles.md 単一責任の原則参照）
    /// </summary>
    public class EnemySpawner : MonoBehaviour
    {
        // -----------------------------------------------------------------------
        // Inspector 公開フィールド
        // -----------------------------------------------------------------------

        /// <summary>生成する敵の Prefab。Inspector でアサインする。</summary>
        [SerializeField] private GameObject enemyPrefab;

        /// <summary>生成する行数（縦方向）。</summary>
        [SerializeField] private int rows = 3;

        /// <summary>生成する列数（横方向）。</summary>
        [SerializeField] private int columns = 8;

        /// <summary>横方向の敵同士の間隔。</summary>
        [SerializeField] private float xSpacing = 1.2f;

        /// <summary>縦方向の敵同士の間隔。</summary>
        [SerializeField] private float ySpacing = 0.8f;

        /// <summary>生成グループの左上の開始位置。</summary>
        [SerializeField] private Vector2 startPosition = new Vector2(-4.0f, 3.0f);

        // -----------------------------------------------------------------------
        // ライフサイクル
        // -----------------------------------------------------------------------

        /// <summary>
        /// ゲーム開始時に敵を一括生成する。
        /// </summary>
        private void Start()
        {
            if (enemyPrefab == null)
            {
                Debug.LogError("enemyPrefab が設定されていません。EnemySpawner の Inspector でアサインしてください。");
                return;
            }

            SpawnEnemies();
        }

        // -----------------------------------------------------------------------
        // 生成処理
        // -----------------------------------------------------------------------

        /// <summary>
        /// rows × columns の格子状に敵を生成する。
        /// 生成した敵ごとに GameManager.RegisterEnemy() を呼び
        /// ゲームの残り敵数を正しく設定する。
        /// </summary>
        public void SpawnEnemies()
        {
            for (int row = 0; row < rows; row++)
            {
                for (int col = 0; col < columns; col++)
                {
                    // 格子状の生成位置を計算する
                    float x = startPosition.x + col * xSpacing;
                    float y = startPosition.y - row * ySpacing;
                    Vector3 spawnPosition = new Vector3(x, y, 0.0f);

                    // 敵を生成する
                    Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);

                    // GameManager に敵が1体生成されたことを通知する
                    // RegisterEnemy() で敵の残数をインクリメントする
                    if (GameManager.Instance != null)
                    {
                        GameManager.Instance.RegisterEnemy();
                    }
                }
            }

            Debug.Log($"敵を {rows * columns} 体生成しました。");
        }
    }
}
