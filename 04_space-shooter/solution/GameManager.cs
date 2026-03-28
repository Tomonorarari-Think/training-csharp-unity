/// <summary>
/// GameManager.cs
/// 概要：スコア管理・敵の残数管理・ゲームオーバー・ステージクリアを一元管理するクラス。
/// 対応設計ドキュメント：solution/README.md
/// </summary>

using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SpaceShooter
{
    /// <summary>
    /// ゲーム全体の進行を管理する Singleton クラス。
    /// スコア・敵の残数・ゲームオーバー状態を一元管理し
    /// PlayerController・EnemyBase・EnemySpawner からアクセスされる。
    ///
    /// Singleton パターンを採用した理由：
    /// スコアやゲームオーバー状態は複数のクラスから参照される共通状態のため
    /// シーン全体で1インスタンスを保証する Singleton が適切。
    /// （03_design/04_design-patterns.md Singleton パターン参照）
    ///
    /// ⚠️ Singleton の乱用は依存性逆転の原則に反するため注意する。
    ///    GameManager のような「ゲーム全体の状態管理」に限定して使用すること。
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        // -----------------------------------------------------------------------
        // Singleton インスタンス
        // -----------------------------------------------------------------------

        /// <summary>
        /// シーン全体で唯一の GameManager インスタンス。
        /// 他クラスからは GameManager.Instance.AddScore() のようにアクセスする。
        /// </summary>
        public static GameManager Instance { get; private set; }

        // -----------------------------------------------------------------------
        // Inspector 公開フィールド
        // -----------------------------------------------------------------------

        /// <summary>スコアを表示する TextMeshProUGUI。Inspector でアサインする。</summary>
        [SerializeField] private TextMeshProUGUI scoreText;

        /// <summary>ゲームオーバー時に表示するパネル。Inspector でアサインする。</summary>
        [SerializeField] private GameObject gameOverPanel;

        /// <summary>ステージクリア時に表示するパネル。Inspector でアサインする。</summary>
        [SerializeField] private GameObject stageClearPanel;

        /// <summary>ゲームオーバー後にシーンをリロードするまでの待機時間（秒）。</summary>
        [SerializeField] private float reloadDelay = 3.0f;

        // -----------------------------------------------------------------------
        // 内部状態
        // -----------------------------------------------------------------------

        /// <summary>現在のスコア。</summary>
        private int score;

        /// <summary>残り敵数。EnemySpawner が敵を生成するたびに RegisterEnemy() でインクリメントされる。</summary>
        private int enemyCount;

        /// <summary>ゲームオーバーフラグ。true になると以降の処理を無効化する。</summary>
        private bool isGameOver;

        // -----------------------------------------------------------------------
        // ライフサイクル
        // -----------------------------------------------------------------------

        /// <summary>
        /// Singleton の初期化。シーンに2つ以上存在する場合は自身を破棄する。
        /// </summary>
        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                // すでに Instance が存在する場合（2つ目以降）は自身を破棄する
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }

        /// <summary>
        /// 初期状態のセットアップ。
        /// </summary>
        private void Start()
        {
            // UI の初期表示
            if (gameOverPanel != null)   gameOverPanel.SetActive(false);
            if (stageClearPanel != null) stageClearPanel.SetActive(false);
            UpdateScoreUI();
        }

        // -----------------------------------------------------------------------
        // スコア管理
        // -----------------------------------------------------------------------

        /// <summary>
        /// スコアを加算して UI を更新する。
        /// スコアの加算と UI 更新を AddScore() に集約することで
        /// 「スコアが変わるときは必ず UI も更新される」ことを保証する。
        /// </summary>
        /// <param name="points">加算するスコア。</param>
        public void AddScore(int points)
        {
            score += points;
            UpdateScoreUI();
        }

        /// <summary>スコア表示 UI を最新の score に更新する。</summary>
        private void UpdateScoreUI()
        {
            if (scoreText != null)
            {
                scoreText.text = $"SCORE: {score}";
            }
        }

        // -----------------------------------------------------------------------
        // 敵の残数管理
        // -----------------------------------------------------------------------

        /// <summary>
        /// 敵が生成されるたびに呼ばれる。残り敵数をインクリメントする。
        /// EnemySpawner から敵を Instantiate するたびに呼ぶこと。
        /// </summary>
        public void RegisterEnemy()
        {
            enemyCount++;
        }

        /// <summary>
        /// 敵が倒されるたびに呼ばれる。残り敵数をデクリメントし
        /// 0 以下になるとステージクリアを通知する。
        /// EnemyBase.Die() から呼ばれる。
        /// </summary>
        public void OnEnemyDefeated()
        {
            enemyCount--;
            Debug.Log($"残り敵数：{enemyCount}");

            if (enemyCount <= 0)
            {
                OnStageClear();
            }
        }

        // -----------------------------------------------------------------------
        // ゲーム進行
        // -----------------------------------------------------------------------

        /// <summary>
        /// ゲームオーバー処理。ゲームオーバー UI を表示し
        /// reloadDelay 秒後にシーンをリロードする。
        /// isGameOver フラグで二重呼び出しを防ぐ。
        /// </summary>
        public void OnGameOver()
        {
            if (isGameOver) return; // すでにゲームオーバー処理済みなら無視する

            isGameOver = true;
            Debug.Log("ゲームオーバー");

            if (gameOverPanel != null)
            {
                gameOverPanel.SetActive(true);
            }

            // reloadDelay 秒後にシーンをリロードする
            // Invoke は MonoBehaviour の遅延実行メソッド
            Invoke(nameof(ReloadScene), reloadDelay);
        }

        /// <summary>
        /// ステージクリア処理。ステージクリア UI を表示する。
        /// </summary>
        public void OnStageClear()
        {
            Debug.Log("ステージクリア！");

            if (stageClearPanel != null)
            {
                stageClearPanel.SetActive(true);
            }
        }

        /// <summary>
        /// 現在のシーンをリロードする。
        /// SceneManager.LoadScene を使用するには
        /// using UnityEngine.SceneManagement が必要。
        /// </summary>
        private void ReloadScene()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}
