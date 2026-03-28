/// <summary>
/// EnemyBase.cs
/// 概要：敵キャラクターの共通処理（射撃・HP管理・消滅）を持つ抽象基底クラス。
/// 対応設計ドキュメント：solution/README.md
/// </summary>

using UnityEngine;

namespace SpaceShooter
{
    /// <summary>
    /// 敵キャラクターの共通処理を定義する抽象基底クラス。
    /// 移動処理（Move）は各派生クラスで実装する。
    /// 射撃・ダメージ処理・消滅処理はこのクラスで共通実装する。
    ///
    /// 抽象クラスにすることで「Move() を実装していない派生クラス」を
    /// コンパイルエラーで検出できる。
    /// （03_design/01_solid-principles.md 開放閉鎖の原則参照）
    /// </summary>
    public abstract class EnemyBase : MonoBehaviour
    {
        // -----------------------------------------------------------------------
        // Inspector 公開フィールド（protected で派生クラスからも参照可能）
        // -----------------------------------------------------------------------

        /// <summary>移動速度（単位/秒）。</summary>
        [SerializeField] protected float speed = 2.0f;

        /// <summary>HP。0 以下になると消滅する。</summary>
        [SerializeField] protected int hp = 1;

        /// <summary>発射する弾の Prefab。Inspector でアサインする。</summary>
        [SerializeField] protected GameObject bulletPrefab;

        /// <summary>弾の発射間隔（秒）。</summary>
        [SerializeField] protected float fireInterval = 2.0f;

        // -----------------------------------------------------------------------
        // 内部状態
        // -----------------------------------------------------------------------

        /// <summary>発射タイマー。派生クラスの Update から更新して使う。</summary>
        protected float fireTimer;

        // -----------------------------------------------------------------------
        // ライフサイクル
        // -----------------------------------------------------------------------

        /// <summary>
        /// コンポーネントの初期化。
        /// 派生クラスで Awake を override する場合は base.Awake() を呼ぶこと。
        /// </summary>
        protected virtual void Awake()
        {
            // 派生クラスが必要なコンポーネントをキャッシュするための拡張点
            // GetComponent は Awake で1回だけ呼んでキャッシュする
            // （02_unity/02_monobehaviour.md 参照）
        }

        // -----------------------------------------------------------------------
        // 抽象メソッド（派生クラスで必ず実装する）
        // -----------------------------------------------------------------------

        /// <summary>
        /// 移動処理。派生クラスごとに異なる移動パターンを実装する。
        /// abstract にすることで実装漏れをコンパイルエラーで検出できる。
        /// </summary>
        public abstract void Move();

        // -----------------------------------------------------------------------
        // 射撃処理
        // -----------------------------------------------------------------------

        /// <summary>
        /// 弾を下方向に発射する。
        /// プレイヤーの弾と共通の BulletController を使い、direction で向きを切り替える。
        /// </summary>
        public virtual void Fire()
        {
            if (bulletPrefab == null)
            {
                Debug.LogWarning($"bulletPrefab が設定されていません。{gameObject.name} の Inspector でアサインしてください。");
                return;
            }

            GameObject bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);

            // 生成した弾に下方向を設定する
            BulletController bulletController = bullet.GetComponent<BulletController>();
            if (bulletController != null)
            {
                bulletController.SetDirection(Vector2.down);
            }
        }

        // -----------------------------------------------------------------------
        // ダメージ処理
        // -----------------------------------------------------------------------

        /// <summary>
        /// ダメージを受ける。HP が 0 以下になると Die() を呼ぶ。
        /// </summary>
        /// <param name="amount">受けるダメージ量。</param>
        public virtual void TakeDamage(int amount)
        {
            hp -= amount;

            if (hp <= 0)
            {
                Die();
            }
        }

        /// <summary>
        /// 消滅処理。スコアを加算して GameManager に通知した後 Destroy する。
        /// スコア加算は GameManager に委譲する（単一責任の原則）。
        /// </summary>
        private void Die()
        {
            if (GameManager.Instance != null)
            {
                // スコア加算と残敵数の更新を GameManager に委譲する
                GameManager.Instance.AddScore(100);
                GameManager.Instance.OnEnemyDefeated();
            }

            Destroy(gameObject);
        }

        // -----------------------------------------------------------------------
        // 当たり判定
        // -----------------------------------------------------------------------

        /// <summary>
        /// プレイヤーの弾との衝突を判定する。
        /// </summary>
        /// <param name="other">衝突した Collider2D。</param>
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("PlayerBullet"))
            {
                TakeDamage(1);
            }
        }
    }
}
