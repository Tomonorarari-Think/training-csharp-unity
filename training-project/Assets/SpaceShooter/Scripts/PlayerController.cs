/// <summary>
/// PlayerController.cs
/// 概要：プレイヤーの移動・射撃・HP管理を担当するスクリプト。
/// 対応設計ドキュメント：solution/README.md
/// </summary>

using UnityEngine;

namespace SpaceShooter
{
    /// <summary>
    /// プレイヤーの移動・射撃・HP管理を行う MonoBehaviour。
    /// Rigidbody2D と Collider2D（isTrigger = true）をアタッチした
    /// Player オブジェクトにアタッチして使用する。
    /// </summary>
    public class PlayerController : MonoBehaviour
    {
        // -----------------------------------------------------------------------
        // Inspector 公開フィールド
        // SerializeField で private のまま Inspector から設定できる
        // （02_unity/01_scene-gameobject.md §3 参照）
        // -----------------------------------------------------------------------

        /// <summary>プレイヤーの移動速度（単位/秒）。</summary>
        [SerializeField] private float speed = 5.0f;

        /// <summary>プレイヤーの最大 HP。</summary>
        [SerializeField] private int hp = 1;

        /// <summary>発射する弾の Prefab。Inspector でアサインする。</summary>
        [SerializeField] private GameObject bulletPrefab;

        /// <summary>弾の発射位置。null の場合は自身の transform を使用する。</summary>
        [SerializeField] private Transform firePoint;

        /// <summary>弾の発射間隔（秒）。</summary>
        [SerializeField] private float fireInterval = 0.3f;

        // -----------------------------------------------------------------------
        // 内部状態（Inspector には表示しない）
        // -----------------------------------------------------------------------

        private Rigidbody2D rb;
        private float       fireTimer;
        private float       horizontalInput;

        // -----------------------------------------------------------------------
        // ライフサイクル
        // -----------------------------------------------------------------------

        /// <summary>
        /// コンポーネントの初期化。Rigidbody2D を Awake でキャッシュする。
        /// GetComponent は毎フレーム呼ぶとパフォーマンスが低下するため
        /// Awake で1回だけ呼んでキャッシュする。
        /// （02_unity/02_monobehaviour.md 参照）
        /// </summary>
        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();

            if (rb == null)
            {
                Debug.LogError("Rigidbody2D が見つかりません。Player オブジェクトに Rigidbody2D をアタッチしてください。");
            }
        }

        /// <summary>
        /// 毎フレームの入力取得と発射タイマーの更新。
        /// ⚠️ 入力取得は必ず Update で行う。
        /// FixedUpdate で GetAxis を呼ぶと物理更新タイミングとずれ、入力が抜ける場合がある。
        /// Update で取得した値をフィールドに保存し、FixedUpdate で参照する。
        /// （02_unity/03_physics-input.md §4-4 参照）
        /// </summary>
        private void Update()
        {
            horizontalInput = Input.GetAxis("Horizontal");

            fireTimer += Time.deltaTime;

            // スペースキーを押している間、発射間隔ごとに弾を発射する
            if (Input.GetKey(KeyCode.Space) && fireTimer >= fireInterval)
            {
                Fire();
                fireTimer = 0.0f;
            }
        }

        /// <summary>
        /// 物理演算を使った移動処理。Rigidbody2D の操作は FixedUpdate で行う。
        /// ⚠️ Transform.position を直接変更すると物理演算と競合する。
        /// MovePosition を使うことで物理エンジンが衝突を正しく処理できる。
        /// 入力値は Update で取得済みの horizontalInput を使用する。
        /// （02_unity/03_physics-input.md §2 参照）
        /// </summary>
        private void FixedUpdate()
        {
            Vector2 movement = new Vector2(horizontalInput, 0.0f);
            rb.MovePosition(rb.position + movement * speed * Time.fixedDeltaTime);
        }

        // -----------------------------------------------------------------------
        // 射撃処理
        // -----------------------------------------------------------------------

        /// <summary>
        /// 弾を上方向に発射する。
        /// bulletPrefab を Instantiate し、生成した弾に上方向を設定する。
        /// 💡 弾の頻繁な Instantiate/Destroy は GC 負荷が高いため
        ///    Object Pool パターンを使うとより効率的になる。
        ///    （03_design/04_design-patterns.md 参照）
        /// </summary>
        private void Fire()
        {
            if (bulletPrefab == null)
            {
                Debug.LogWarning("bulletPrefab が設定されていません。Inspector でアサインしてください。");
                return;
            }

            Transform origin = firePoint != null ? firePoint : transform;
            GameObject bullet = Instantiate(bulletPrefab, origin.position, Quaternion.identity);

            // 生成した弾に上方向を設定する
            BulletController bulletController = bullet.GetComponent<BulletController>();
            if (bulletController != null)
            {
                bulletController.SetDirection(Vector2.up);
            }
        }

        // -----------------------------------------------------------------------
        // ダメージ処理
        // -----------------------------------------------------------------------

        /// <summary>
        /// ダメージを受ける。HP が 0 以下になるとゲームオーバーを通知する。
        /// GameManager への通知は Singleton 経由で行う。
        /// （03_design/04_design-patterns.md Singleton パターン参照）
        /// </summary>
        /// <param name="amount">受けるダメージ量。</param>
        public void TakeDamage(int amount)
        {
            hp -= amount;
            Debug.Log($"プレイヤーがダメージを受けました。残り HP：{hp}");

            if (hp <= 0)
            {
                GameManager.Instance.OnGameOver();
            }
        }

        // -----------------------------------------------------------------------
        // 当たり判定
        // -----------------------------------------------------------------------

        /// <summary>
        /// Trigger に入ったオブジェクトを判定する。
        /// ⚠️ CompareTag は tag == "..." より推奨される。
        ///    tag == "..." は毎回文字列を生成するためガベージが発生する。
        ///    CompareTag はガベージを生成しない。
        /// </summary>
        /// <param name="other">衝突した Collider2D。</param>
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Enemy") || other.CompareTag("EnemyBullet"))
            {
                TakeDamage(1);
            }
        }
    }
}
