# 03_design — オブジェクト指向設計

## このフォルダで学べること

- SOLID 原則（オブジェクト指向設計の5つの原則）
- クラス設計の実践（横スクロールアクションゲームを題材にしたワーク）
- UML（クラス図・シーケンス図）の読み書き
- デザインパターン（Singleton・Observer・Command 等）

---

## このフォルダのゴール

このフォルダでは「コードを自分で書ける」状態を目標としていません。

**「名前と概念を知っており、コードレビューや設計議論で言葉として使える」** 状態を目標としています。

たとえば「この実装は Single Responsibility Principle に違反していますね」「Observer パターンで解決できそうです」といった会話ができるようになることが最終目標です。

より深く学びたい場合は、下記参考リンクの Unity 公式 eBook（無料）を通読することをお勧めします。

---

## 読み進め方

以下の順番で学習することを推奨します。

1. [01_solid-principles.md](01_solid-principles.md) — SOLID 原則
2. [02_action-game-class-design.md](02_action-game-class-design.md) — クラス設計ワーク（横スクロールアクションゲーム）
3. [03_uml-basics.md](03_uml-basics.md) — UML 基礎
4. [04_design-patterns.md](04_design-patterns.md) — デザインパターン

---

## 前提知識

`01_csharp/` の内容、特に継承・インターフェース（[03_inheritance-interface.md](../01_csharp/03_inheritance-interface.md)）を先に読んでおくことを推奨します。SOLID 原則やデザインパターンの説明で頻繁に登場する概念です。

---

## 参考リンク

- [Unity 公式 設計思想 eBook（無料）](https://unity.com/resources/design-patterns-solid-ebook) — SOLID 原則・デザインパターンを Unity の文脈で解説した公式資料
- [Unity 公式 C# 命名規則](https://unity.com/ja/how-to/naming-and-code-style-tips-c-scripting-unity)
