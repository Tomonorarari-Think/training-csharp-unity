# 04_space-shooter — スペースシューター

## 1. この課題について

### ゲーム概要

敵キャラクターが横移動しながら降りてくる、縦スクロール型のシューティングゲームです。
プレイヤーは弾を撃って敵を倒し、全滅させるとステージクリアです。

### この課題の目的

この課題のゴールは「自分でクラスを設計し、Unity で実装すること」です。
設計通りにスムーズに進められればそれがベストですが、
実装を進めるうちに「設計を変えた方がよい」「こう設計すればよかった」と気づくことも
実務では日常的に起こり得ることです。
うまくいかない体験を恐れずに取り組んでください。
設計・実装・振り返りのサイクルを回すことがこの課題の本質です。

### 学習のゴール

1. ゲームの登場要素をクラスとして設計できる
2. 設計に基づいて Unity で実装できる
3. 設計と実装を振り返り、気づいたことを言語化できる（うまくいった点・改善したい点どちらも対象）

---

## 2. この課題で使う知識

以下のドキュメントを参考にしながら進めてください。

- [01_csharp/02_classes.md](../01_csharp/02_classes.md) — クラス設計の基礎
- [01_csharp/03_inheritance-interface.md](../01_csharp/03_inheritance-interface.md) — 継承・インターフェース
- [02_unity/01_scene-gameobject.md](../02_unity/01_scene-gameobject.md) — GameObject・Prefab
- [02_unity/02_monobehaviour.md](../02_unity/02_monobehaviour.md) — ライフサイクル
- [02_unity/03_physics-input.md](../02_unity/03_physics-input.md) — Rigidbody2D・Collider・Input
- [03_design/01_solid-principles.md](../03_design/01_solid-principles.md) — SOLID 原則
- [03_design/02_action-game-class-design.md](../03_design/02_action-game-class-design.md) — クラス設計の考え方
- [03_design/03_uml-basics.md](../03_design/03_uml-basics.md) — クラス図・シーケンス図

---

## 3. 進め方

### Step 1：ゲームの仕様を把握する

このファイルの「4. ゲームの仕様」セクションを読みます。
登場要素（プレイヤー・敵・弾・ゲーム進行）をひと通り把握し、
「どんなクラスが必要になるか」を自分なりに整理してみてください。

### Step 2：設計書を作成する

[design-template.md](design-template.md) をコピーして自分の設計書を作成します。
クラス一覧・各クラスの責務・クラス図・シーケンス図を記述してください。

設計書を書く際のポイントです。

- 「完璧な設計」を目指す必要はありません。まず自分の考えで書いてみることが大切です。
- 実装中に設計を変更しても構いません。変更した場合はその理由もあわせて記録しておくと、振り返りのときに役立ちます。

### Step 3：Unity で実装する

`training-project/` に新しい Scene を作成して実装を始めます。
設計書に基づいてスクリプトを作成・アタッチしてください。

詰まった場合は [hints.md](hints.md) を参照してください。
実装中に気づいたこと・変更したことはメモしておくと、Step 4 の振り返りに活かせます。

### Step 4：振り返りを行う

[05_reflection/retrospective-template.md](../05_reflection/retrospective-template.md) を使って、
設計と実装を振り返りまとめます。
うまくいった点・改善したい点どちらも記録してください。
「想定通りだったこと」と「想定と違ったこと」の両方が、次の設計への学びになります。

---

## 4. ゲームの仕様

### プレイヤー

- 画面下部を左右に移動できます。
- スペースキー（または任意のキー）で弾を発射できます。
- 敵の弾または敵本体に当たるとゲームオーバーです。

### 敵

- 複数体が横一列に並んで登場します。
- 左右に移動し、端に達すると下に降りてきます。
- 一定間隔でランダムに弾を発射します。
- プレイヤーの弾に当たると消滅します。

### 弾

- プレイヤーの弾：上方向に移動します。
- 敵の弾：下方向に移動します。
- 画面外に出たら消滅します。

### ゲームの進行

- 敵を全滅させるとステージクリアです。
- 敵が画面下部まで到達するとゲームオーバーです。
- スコアを画面に表示します。

---

## 5. フォルダ構成

| ファイル / フォルダ | 内容 |
|---|---|
| [README.md](README.md) | 課題の説明・仕様（本ファイル） |
| [design-template.md](design-template.md) | 設計書テンプレート |
| [hints.md](hints.md) | 詰まったときのヒント集 |
| [solution/](solution/) | 模範解答スクリプト（自分で実装した後に参照） |

---

## 6. 模範解答について

`solution/` フォルダに模範解答スクリプトを用意しています。

ただし、以下の点に注意してください。

- 模範解答は「唯一の正解」ではありません。設計にはさまざまなアプローチがあります。
- 自分で実装してから参照することを強く推奨します。先に見てしまうと設計を考える経験が失われます。
- 自分の設計・実装と模範解答を比較することで、「こういう考え方もあるのか」という新たな気づきが得られます。
- 模範解答と異なるアプローチでも、動作していれば問題ありません。

---

## 参考リンク

- [02_unity/Runtime/Scripts/SpaceShooterSample.cs](../02_unity/Runtime/Scripts/SpaceShooterSample.cs) — 02_unity の総まとめサンプル（ヒントとして参照可）
- [03_design/04_design-patterns.md](../03_design/04_design-patterns.md) — Object Pool・State・Factory が活用できるパターン集
- [05_reflection/retrospective-template.md](../05_reflection/retrospective-template.md) — 振り返りテンプレート
