/// <summary>
/// EnemyController.cs
/// 概要：通常の敵の移動処理（左右移動・端での折り返し・降下）を実装する派生クラス。
/// 対応設計ドキュメント：solution/README.md
/// </summary>

using UnityEngine;

namespace SpaceShooter
{
    /// <summary>
    /// 通常の敵キャラクターの移動処理を担当するクラス。
    /// EnemyBase を継承し、Move() を override して
    /// 左右に移動・端で折り返し・降下する動作を実装する。
    /// </summary>
    public class EnemyController : EnemyBase
    {
        // -----------------------------------------------------------------------
        // Inspector 公開フィールド
        // -----------------------------------------------------------------------

        /// <summary>画面端に達したときの降下量。</summary>
        [SerializeField] private float descendAmount = 0.5f;

        // -----------------------------------------------------------------------
        // 内部状態
        // -----------------------------------------------------------------------

        /// <summary>現在の移動方向。1 = 右方向、-1 = 左方向。</summary>
        private int   moveDirection = 1;

        /// <summary>折り返しを判定するX座標。カメラのビューポートから動的に取得する。</summary>
        private float screenEdgeX;

        // -----------------------------------------------------------------------
        // ライフサイクル
        // -----------------------------------------------------------------------

        /// <summary>
        /// 画面端の座標を動的に取得して screenEdgeX に設定する。
        /// 固定値にすると解像度やカメラの設定によって動作が変わるため、
        /// Camera.main.ViewportToWorldPoint() を使って動的に取得する。
        /// （hints.md「敵が画面端で折り返さない」参照）
        /// </summary>
        private void Start()
        {
            // Viewport 座標の X = 1.0（右端）をワールド座標に変換する
            // Z は 0 を渡す（2D では使用しない）
            Vector3 rightEdge = Camera.main.ViewportToWorldPoint(new Vector3(1.0f, 0.5f, 0.0f));
            screenEdgeX = rightEdge.x - 0.5f; // 少し内側を折り返し位置にする
        }

        /// <summary>
        /// 毎フレームの移動処理と発射タイマーの更新。
        /// </summary>
        private void Update()
        {
            Move();

            // 発射タイマーを更新して一定間隔で弾を発射する
            fireTimer += Time.deltaTime;
            if (fireTimer >= fireInterval)
            {
                Fire();
                fireTimer = 0.0f;
            }
        }

        // -----------------------------------------------------------------------
        // 移動処理（EnemyBase の abstract メソッドを実装）
        // -----------------------------------------------------------------------

        /// <summary>
        /// 左右に移動し、画面端に達すると方向を反転して下に降りる。
        /// EnemyBase.Move() を override して具体的な移動パターンを実装する。
        /// </summary>
        public override void Move()
        {
            // 横方向に移動する
            transform.position += new Vector3(moveDirection * speed * Time.deltaTime, 0.0f, 0.0f);

            // 画面端に達したら折り返して降下する
            if (Mathf.Abs(transform.position.x) >= screenEdgeX)
            {
                // 移動方向を反転する
                moveDirection *= -1;

                // Y座標を下げて降下する
                transform.position += new Vector3(0.0f, -descendAmount, 0.0f);

                // 敵が画面下部まで到達したらゲームオーバー
                Vector3 bottomEdge = Camera.main.ViewportToWorldPoint(new Vector3(0.5f, 0.0f, 0.0f));
                if (transform.position.y <= bottomEdge.y)
                {
                    if (GameManager.Instance != null)
                    {
                        GameManager.Instance.OnGameOver();
                    }
                }
            }
        }
    }
}
