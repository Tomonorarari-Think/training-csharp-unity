# GitHub の基本操作

このドキュメントでは、リポジトリをクローンして手元で使えるようになることをゴールに、
Git と GitHub の最低限の操作を説明します。

---

## 1. Git と GitHub の違い

- **Git**：ファイルの変更履歴をローカル（手元のPC）で管理するツールです
- **GitHub**：Git リポジトリをクラウド上に保存・共有できるサービスです

「Git で変更を記録して、GitHub でチームや他の人と共有する」という関係性です。
Git は GitHub がなくても使えますが、このリポジトリでは GitHub と合わせて使います。

---

## 2. GitHub アカウントの作成

> すでにアカウントをお持ちの場合はこのセクションをスキップしてください。

1. [https://github.com/](https://github.com/) にアクセスします
2. **Sign up** をクリックします
3. メールアドレス・パスワード・ユーザー名を入力します
4. 画面の指示に従って認証を完了させます
5. ログインできれば完了です

---

## 3. リポジトリをクローンする

リポジトリをクローンすると、GitHub 上のファイル一式が手元のフォルダにコピーされます。

1. [このリポジトリのページ](https://github.com/Tomonorarari-Think/training-csharp-unity) を開きます
2. 緑色の **Code** ボタンをクリックします
3. **HTTPS** タブが選択されていることを確認し、URL をコピーします
4. ターミナル（またはコマンドプロンプト）で以下を実行します

```bash
git clone https://github.com/Tomonorarari-Think/training-csharp-unity.git
```

実行後、`training-csharp-unity` フォルダが作成され、その中にすべてのファイルが入っています。

---

## 4. リポジトリを fork する（独学者向け）

演習コードを自分のリポジトリに保存したい場合は、fork を使います。

### fork と clone の使い分け

| 方法 | 説明 |
|---|---|
| **clone** | 元のリポジトリをそのまま手元に取得する |
| **fork** | 自分のアカウントにコピーを作ってから取得する |

演習コードを自分のリポジトリに記録・管理したい場合は fork が適しています。

### fork の手順

1. [このリポジトリのページ](https://github.com/Tomonorarari-Think/training-csharp-unity) を開きます
2. 右上の **Fork** ボタンをクリックします
3. 自分のアカウントにコピーが作成されます
4. fork 先のリポジトリページを開き、**Code** ボタンから URL をコピーします
5. 以下のコマンドでクローンします（`[あなたのユーザー名]` は実際のものに置き換えてください）

```bash
git clone https://github.com/[あなたのユーザー名]/training-csharp-unity.git
```

---

## 5. 基本的な Git 操作

演習コードを自分の fork に保存する際に使う最低限のコマンドです。

### 変更を確認する

```bash
# 変更されたファイルの一覧を確認する
git status

# 変更内容の差分を確認する
git diff
```

### 変更を記録する

```bash
# すべての変更をステージングする
git add .

# コミットメッセージを付けて記録する
git commit -m "コミットメッセージ"
```

### リモートに送る

```bash
git push origin main
```

### 最新の状態に更新する

```bash
git pull
```

---

## 6. よくあるハマりどころ

### push できない場合

GitHub へのプッシュには認証が必要です。
パスワードではなく **Personal Access Token（PAT）** が必要な場合があります。
設定方法は [GitHub 認証ドキュメント](https://docs.github.com/ja/authentication) を参照してください。

### コミットメッセージを間違えた場合

直前のコミットのメッセージは以下のコマンドで修正できます。

```bash
git commit --amend
```

> まだ push していないコミットに対してのみ使用してください。

### pull せずに push してエラーになった場合

リモートのブランチが手元より進んでいる場合、push は拒否されます。
先に pull して最新の状態を取り込んでから、再度 push してください。

```bash
git pull
git push origin main
```

---

## 7. 参考リンク

- [GitHub 公式サイト](https://github.com/)
- [GitHub 認証ドキュメント](https://docs.github.com/ja/authentication)
