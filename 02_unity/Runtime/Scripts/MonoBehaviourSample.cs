/// <summary>
/// 対応ドキュメント：02_monobehaviour.md
/// 概要：MonoBehaviour の全ライフサイクルメソッドの実行順序を Debug.Log で確認する
/// アタッチ先：シーン上の任意の GameObject（Play ボタンを押して Console を確認する）
/// </summary>

using System.Collections;
using UnityEngine;

namespace TrainingCsharpUnity
{
    public class MonoBehaviourSample : MonoBehaviour
    {
        [SerializeField] private float moveSpeed = 3.0f; // 移動速度（Inspector で調整可能）

        // -----------------------------------------------------------------------
        // Awake：シーンロード直後に最初に呼ばれる。自分自身の初期化に使う
        // 💡 コンストラクタの代わりに Awake を使うこと（§1）
        // -----------------------------------------------------------------------
        void Awake()
        {
            Debug.Log($"[{gameObject.name}] Awake が呼ばれました");
            // ここで GetComponent によるキャッシュを行う
            // 例：rb = GetComponent<Rigidbody2D>();
        }

        // -----------------------------------------------------------------------
        // OnEnable：GameObject または Component が有効化されるたびに呼ばれる
        // イベントの購読はここで行い、OnDisable で必ず解除する（§3-5）
        // -----------------------------------------------------------------------
        void OnEnable()
        {
            Debug.Log($"[{gameObject.name}] OnEnable が呼ばれました");
            // イベント購読の例：
            // GameEvents.OnEnemyDefeated += HandleEnemyDefeated;
        }

        // -----------------------------------------------------------------------
        // Start：最初のフレームの前に1回だけ呼ばれる。他オブジェクトへの依存がある初期化に使う
        // Awake と Start の使い分け → §3-1・§3-2 を参照
        // -----------------------------------------------------------------------
        void Start()
        {
            Debug.Log($"[{gameObject.name}] Start が呼ばれました");

            // コルーチンの最小例（詳細は AsyncSample.cs を参照）
            StartCoroutine(WaitAndLog());
        }

        // -----------------------------------------------------------------------
        // Update：毎フレーム呼ばれる。入力・ゲームロジックに使う（§3-3）
        // ⚠️ Time.deltaTime を掛けないとフレームレートで速度が変わるので必ず使うこと
        // -----------------------------------------------------------------------
        void Update()
        {
            // Time.deltaTime を掛けることで「1秒あたり moveSpeed ユニット」移動する
            float horizontal = Input.GetAxis("Horizontal");
            float vertical   = Input.GetAxis("Vertical");
            Vector3 move = new Vector3(horizontal, vertical, 0) * moveSpeed * Time.deltaTime;
            transform.Translate(move);

            // ログは毎フレーム出力するとコンソールが溢れるためコメントアウト
            // Debug.Log($"[{gameObject.name}] Update（位置：{transform.position}）");
        }

        // -----------------------------------------------------------------------
        // FixedUpdate：物理演算のタイミングで一定間隔で呼ばれる（§3-4）
        // Rigidbody の操作はここで行う。入力取得は Update で行うこと
        // -----------------------------------------------------------------------
        void FixedUpdate()
        {
            // ログは毎フレーム出力するとコンソールが溢れるためコメントアウト
            // Debug.Log($"[{gameObject.name}] FixedUpdate");
            // Rigidbody を使う場合はここで AddForce / MovePosition などを呼ぶ
        }

        // -----------------------------------------------------------------------
        // LateUpdate：全オブジェクトの Update が終わった後に呼ばれる（§3-5）
        // カメラの追従処理など「Update の後に実行したい処理」に使う
        // -----------------------------------------------------------------------
        void LateUpdate()
        {
            // ログは毎フレーム出力するとコンソールが溢れるためコメントアウト
            // Debug.Log($"[{gameObject.name}] LateUpdate");
        }

        // -----------------------------------------------------------------------
        // OnDisable：GameObject または Component が無効化されるたびに呼ばれる（§3-6）
        // OnEnable で購読したイベントは必ずここで解除する
        // -----------------------------------------------------------------------
        void OnDisable()
        {
            Debug.Log($"[{gameObject.name}] OnDisable が呼ばれました");
            // イベント解除の例：
            // GameEvents.OnEnemyDefeated -= HandleEnemyDefeated;
        }

        // -----------------------------------------------------------------------
        // OnDestroy：GameObject が破棄される直前に1回呼ばれる（§3-7）
        // -----------------------------------------------------------------------
        void OnDestroy()
        {
            Debug.Log($"[{gameObject.name}] OnDestroy が呼ばれました");
        }

        // -----------------------------------------------------------------------
        // WaitAndLog：コルーチンの最小例（詳細は AsyncSample.cs を参照）
        // yield return で処理を一時停止して再開できる（§5）
        // -----------------------------------------------------------------------
        private IEnumerator WaitAndLog()
        {
            Debug.Log($"[{gameObject.name}] コルーチン開始：3秒後にログを出します");
            yield return new WaitForSeconds(3.0f); // 3秒待つ
            Debug.Log($"[{gameObject.name}] コルーチン：3秒経過しました");
        }

        // -----------------------------------------------------------------------
        // enabled と gameObject.SetActive の違い（§6）
        // -----------------------------------------------------------------------
        // this.enabled = false      → このコンポーネントのみ無効化（他は動き続ける）
        // gameObject.SetActive(false) → GameObject 全体を無効化（全コンポーネントが止まる）
        //
        // SetActive(false) にすると Update・FixedUpdate も呼ばれなくなる点に注意すること
    }
}
