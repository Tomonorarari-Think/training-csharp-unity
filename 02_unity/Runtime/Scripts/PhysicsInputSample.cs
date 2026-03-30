/// <summary>
/// 対応ドキュメント：03_physics-input.md
/// 概要：Rigidbody2D による移動・OnTriggerEnter2D による当たり判定・入力処理の基本を学ぶ
/// アタッチ先：Rigidbody2D と Collider2D（isTrigger = true）をアタッチした Player オブジェクト
/// </summary>

using UnityEngine;

namespace TrainingCsharpUnity
{
    public class PhysicsInputSample : MonoBehaviour
    {
        // --- Inspector で設定するフィールド ---
        [SerializeField] private float       moveForce    = 10.0f; // 移動に加える力の大きさ
        [SerializeField] private GameObject  bulletPrefab;          // 弾のプレハブ
        [SerializeField] private Transform   firePoint;             // 弾の発射位置

        // Awake でキャッシュするコンポーネント
        private Rigidbody2D rb;

        // Update → FixedUpdate にフラグで渡す発射要求
        // ⚠️ Input.GetKeyDown を FixedUpdate に書くと入力が抜けるためフラグを使う（§4-4）
        private bool    fireRequested = false;
        private Vector2 moveInput;               // Update → FixedUpdate への移動入力キャッシュ

        // -----------------------------------------------------------------------
        // Awake：Rigidbody2D を Awake でキャッシュする（§2）
        // -----------------------------------------------------------------------
        void Awake()
        {
            rb = GetComponent<Rigidbody2D>();

            if (rb == null)
            {
                Debug.LogError("Rigidbody2D が見つかりません。このオブジェクトに Rigidbody2D をアタッチしてください。");
            }
        }

        // -----------------------------------------------------------------------
        // Update：入力の取得とフラグ管理（§4-4）
        // ⚠️ 入力取得は必ず Update で行う。FixedUpdate で GetKeyDown を呼ぶと抜けが起きる
        // ⚠️ 新 Input System（PlayerInput）を使う場合は 03_physics-input.md §4-3 を参照
        // -----------------------------------------------------------------------
        void Update()
        {
            // --- 移動入力の取得（Update でキャッシュして FixedUpdate に渡す）---
            moveInput = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));

            // スペースキーを押した瞬間に発射フラグを立てる（Legacy Input Manager 使用）
            if (Input.GetKeyDown(KeyCode.Space))
            {
                fireRequested = true;
            }
        }

        // -----------------------------------------------------------------------
        // FixedUpdate：Rigidbody2D を使った移動処理（§2）
        // ⚠️ Rigidbody の操作は FixedUpdate で行う
        // ⚠️ Transform.position を直接書き換えると物理演算と競合する
        // -----------------------------------------------------------------------
        void FixedUpdate()
        {
            // AddForce で力を加えて移動する（物理演算に従って自然に加速する）
            // moveInput は Update でキャッシュ済み
            rb.AddForce(moveInput * moveForce);

            // 発射フラグが立っていたら弾を発射する
            if (fireRequested)
            {
                Fire();
                fireRequested = false;
            }
        }

        // -----------------------------------------------------------------------
        // OnTriggerEnter2D：Trigger に何かが入った瞬間に呼ばれる（§3）
        // ⚠️ Collider の isTrigger = true が必要
        // ⚠️ どちらか一方に Rigidbody2D が必要
        // -----------------------------------------------------------------------
        void OnTriggerEnter2D(Collider2D other)
        {
            // CompareTag を使う（== より推奨：アロケーションなし・タイポ検出）
            if (other.CompareTag("Enemy"))
            {
                Debug.Log($"敵「{other.gameObject.name}」と接触しました");
                // ここに被ダメージ処理を書く
                // 例：TakeDamage(10);
            }
        }

        // -----------------------------------------------------------------------
        // Fire：弾の発射処理（Instantiate の例）（§4-3）
        // -----------------------------------------------------------------------
        private void Fire()
        {
            if (bulletPrefab == null)
            {
                Debug.LogWarning("bulletPrefab が設定されていません。Inspector でアサインしてください。");
                return;
            }

            Transform origin = firePoint != null ? firePoint : transform;
            Instantiate(bulletPrefab, origin.position, origin.rotation);
            Debug.Log("弾を発射しました");
        }
    }
}
