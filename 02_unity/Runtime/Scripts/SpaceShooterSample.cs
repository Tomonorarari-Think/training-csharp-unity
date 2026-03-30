/// <summary>
/// 対応ドキュメント：02_unity/ の総まとめ（04_space-shooter/ の予習）
/// 概要：02_unity/ で学んだ内容をスペースシューターの文脈でまとめたサンプル。
///        実際の課題（04_space-shooter/）のヒントとなる最小構成のプレイヤースクリプト。
/// アタッチ先：Rigidbody2D と Collider2D（isTrigger = true）をアタッチした Player オブジェクト
/// </summary>

using UnityEngine;

namespace TrainingCsharpUnity
{
    public class SpaceShooterSample : MonoBehaviour
    {
        // -----------------------------------------------------------------------
        // SerializeField でフィールドを Inspector に公開する（01_scene-gameobject.md §3）
        // private のまま Inspector から設定でき、外部から直接変更されない
        // -----------------------------------------------------------------------
        [SerializeField] private float      moveSpeed     = 5.0f;  // 移動速度
        [SerializeField] private float      fireInterval  = 0.3f;  // 発射間隔（秒）
        [SerializeField] private GameObject bulletPrefab;           // 弾のプレハブ
        [SerializeField] private Transform  firePoint;              // 弾の発射位置
        [SerializeField] private int        maxHp         = 3;      // 最大 HP

        // 内部状態（Inspector には表示しない）
        private Rigidbody2D rb;
        private int         currentHp;
        private float       lastFireTime  = -999.0f; // 最後に発射した時刻（初回すぐ撃てるよう大きなマイナス値）
        private bool        fireRequested = false;   // Update → FixedUpdate への発射フラグ
        private Vector2     moveInput;               // Update → FixedUpdate への移動入力キャッシュ

        // -----------------------------------------------------------------------
        // Awake：コンポーネントのキャッシュ（02_monobehaviour.md §3-1）
        // GetComponent は Awake で1回だけ呼んでキャッシュする（毎フレーム呼ばない）
        // -----------------------------------------------------------------------
        void Awake()
        {
            rb        = GetComponent<Rigidbody2D>();
            currentHp = maxHp;

            if (rb == null)
            {
                Debug.LogError("Rigidbody2D が見つかりません。Player オブジェクトに Rigidbody2D をアタッチしてください。");
            }
        }

        // -----------------------------------------------------------------------
        // Update：入力取得とクールダウン管理（03_physics-input.md §4-4）
        // ⚠️ 入力は必ず Update で取得する（FixedUpdate では入力が抜ける）
        // -----------------------------------------------------------------------
        void Update()
        {
            // --- 移動入力の取得（Update でキャッシュして FixedUpdate に渡す）---
            float horizontal = Input.GetAxis("Horizontal");
            float vertical   = Input.GetAxis("Vertical");
            moveInput = new Vector2(horizontal, vertical).normalized;

            // スペースキーを押している間、発射間隔を超えていたら発射フラグを立てる
            // Input.GetKey（押し続けている間）を使うことで連射が可能
            if (Input.GetKey(KeyCode.Space))
            {
                // クールダウンチェック（fireInterval 秒ごとに発射できる）
                if (Time.time - lastFireTime >= fireInterval)
                {
                    fireRequested = true;
                    lastFireTime  = Time.time;
                }
            }
        }

        // -----------------------------------------------------------------------
        // FixedUpdate：物理演算を使った移動処理（03_physics-input.md §2）
        // Rigidbody2D の操作は FixedUpdate で行う
        // ⚠️ Transform.position を直接操作すると物理演算と競合する
        // -----------------------------------------------------------------------
        void FixedUpdate()
        {
            // --- 移動処理（Update でキャッシュした moveInput を使う）---
            // MovePosition で物理演算を考慮しながら移動する
            // Time.fixedDeltaTime を掛けることでフレームレートに依存しない移動量になる
            rb.MovePosition(rb.position + moveInput * moveSpeed * Time.fixedDeltaTime);

            // --- 発射処理（Update からフラグで受け取る）---
            if (fireRequested)
            {
                Fire();
                fireRequested = false;
            }
        }

        // -----------------------------------------------------------------------
        // Fire：弾の発射処理（01_scene-gameobject.md §4-3）
        // Instantiate でプレハブからインスタンスを生成する
        // -----------------------------------------------------------------------
        private void Fire()
        {
            if (bulletPrefab == null)
            {
                Debug.LogWarning("bulletPrefab が設定されていません。Inspector でアサインしてください。");
                return;
            }

            Transform origin = firePoint != null ? firePoint : transform;

            // プレハブから弾のインスタンスを生成する
            GameObject bullet = Instantiate(bulletPrefab, origin.position, origin.rotation);
            Debug.Log($"弾を発射しました（位置：{origin.position}）");
        }

        // -----------------------------------------------------------------------
        // OnTriggerEnter2D：敵との当たり判定（03_physics-input.md §3）
        // ⚠️ この Collider2D の isTrigger = true が必要
        // ⚠️ どちらか一方に Rigidbody2D が必要
        // -----------------------------------------------------------------------
        void OnTriggerEnter2D(Collider2D other)
        {
            // CompareTag を使う（== より推奨：アロケーションなし・タイポ検出）（§5）
            if (other.CompareTag("Enemy"))
            {
                currentHp--;
                Debug.Log($"敵に当たりました！残り HP：{currentHp}");

                if (currentHp <= 0)
                {
                    Debug.Log("HP が 0 になりました。ゲームオーバー処理をここに書く");
                    // 例：GameManager.Instance.GameOver();
                    gameObject.SetActive(false); // サンプルとして非アクティブにする
                }
            }
        }
    }
}
