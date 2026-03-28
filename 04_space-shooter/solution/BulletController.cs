/// <summary>
/// BulletController.cs
/// 概要：弾の移動・画面外での自動消滅・衝突時の処理を担当するスクリプト。
/// 対応設計ドキュメント：solution/README.md
/// </summary>

using UnityEngine;

namespace SpaceShooter
{
    /// <summary>
    /// プレイヤーの弾・敵の弾の両方に使える汎用弾クラス。
    /// direction フィールドで上下の向きを切り替えることで
    /// 1つのクラスでプレイヤーの弾（上方向）と敵の弾（下方向）を共用する。
    ///
    /// 弾の種類が増えた場合（貫通弾・追尾弾など）は
    /// このクラスを基底クラスとして継承することを検討する。
    ///
    /// 💡 弾の頻繁な Instantiate/Destroy は GC 負荷が高いため
    ///    Object Pool パターンを使うとより効率的になる。
    ///    Unity 2021 以降では UnityEngine.Pool.ObjectPool&lt;T&gt; が使用可能。
    ///    （03_design/04_design-patterns.md 参照）
    /// </summary>
    public class BulletController : MonoBehaviour
    {
        // -----------------------------------------------------------------------
        // Inspector 公開フィールド
        // -----------------------------------------------------------------------

        /// <summary>弾の移動速度（単位/秒）。</summary>
        [SerializeField] private float speed = 8.0f;

        // -----------------------------------------------------------------------
        // 内部状態
        // -----------------------------------------------------------------------

        /// <summary>
        /// 移動方向。PlayerController から生成された弾は Vector2.up、
        /// EnemyBase から生成された弾は Vector2.down が設定される。
        /// </summary>
        private Vector2 direction = Vector2.up;

        // -----------------------------------------------------------------------
        // 初期化
        // -----------------------------------------------------------------------

        /// <summary>
        /// 弾の移動方向を設定する。
        /// Instantiate 直後に呼び出して向きを設定する。
        /// </summary>
        /// <param name="dir">移動方向。Vector2.up（上）または Vector2.down（下）。</param>
        public void SetDirection(Vector2 dir)
        {
            direction = dir.normalized;
        }

        // -----------------------------------------------------------------------
        // ライフサイクル
        // -----------------------------------------------------------------------

        /// <summary>
        /// 毎フレームの移動と画面外チェック。
        /// Transform.Translate で direction 方向に移動し、
        /// 画面外に出たら Destroy する。
        /// </summary>
        private void Update()
        {
            // direction 方向に speed * deltaTime 分移動する
            transform.Translate(direction * speed * Time.deltaTime);

            // 画面外チェック：Viewport 座標が 0〜1 の範囲外なら画面外
            // Camera.main.WorldToViewportPoint() でワールド座標を Viewport 座標に変換する
            Vector3 viewportPos = Camera.main.WorldToViewportPoint(transform.position);
            bool isOutOfScreen = viewportPos.y > 1.1f || viewportPos.y < -0.1f
                              || viewportPos.x > 1.1f || viewportPos.x < -0.1f;

            if (isOutOfScreen)
            {
                Destroy(gameObject);
            }
        }

        // -----------------------------------------------------------------------
        // 当たり判定
        // -----------------------------------------------------------------------

        /// <summary>
        /// 衝突相手に応じて自身を破棄する。
        /// 弾はダメージ処理を相手クラスに委譲し、自身は消滅するだけにとどめる。
        /// （単一責任の原則：弾の責務は「移動と消滅」のみ）
        /// </summary>
        /// <param name="other">衝突した Collider2D。</param>
        private void OnTriggerEnter2D(Collider2D other)
        {
            // プレイヤーの弾が敵に当たった場合
            if (direction.y > 0 && other.CompareTag("Enemy"))
            {
                // ダメージ処理は EnemyBase.OnTriggerEnter2D に委譲する
                // （弾側では EnemyBase への参照を持たないことで疎結合を保つ）
                Destroy(gameObject);
            }

            // 敵の弾がプレイヤーに当たった場合
            if (direction.y < 0 && other.CompareTag("Player"))
            {
                // ダメージ処理は PlayerController.OnTriggerEnter2D に委譲する
                Destroy(gameObject);
            }
        }
    }
}
