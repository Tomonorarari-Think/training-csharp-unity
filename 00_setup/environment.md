# 開発環境構築手順書

Unity・C#・Git が初めての方でも、この手順書に沿って進めれば環境を揃えられます。
わからない箇所はメンターに気軽に質問してください。

---

## 1. インストールが必要なもの一覧

| ソフトウェア | 推奨バージョン | 用途 |
|---|---|---|
| Unity Hub | 最新版 | Unity Editor の管理・起動ツール |
| Unity Editor | 推奨：20XX.X.X LTS | ゲームエンジン本体 |
| Visual Studio | Community 2022（無料） | スクリプト編集・デバッグ |
| Git | 最新版 | バージョン管理 |

> **LTS（Long Term Support）とは？**
> 長期サポート版のことで、安定性が高く研修・業務に適しています。
> 最新の LTS バージョンは Unity Hub のインストール画面で確認できます。

---

## 2. Unity Hub のインストールと設定

### 2-1. Unity Hub のインストール

1. 公式ダウンロードページ（ https://unity.com/ja/download ）を開きます。
2. 「Unity Hub をダウンロード」ボタンをクリックしてインストーラーをダウンロードします。
3. ダウンロードしたインストーラー（`UnityHubSetup.exe`）を実行します。
4. ライセンス規約を確認し「同意する」を選択して進めます。
5. インストール先フォルダはデフォルトのまま「インストール」をクリックします。
6. インストールが完了したら「Unity Hub を起動」にチェックを入れて「完了」をクリックします。

### 2-2. Unity Editor のインストール

1. Unity Hub が起動したら、左メニューの「インストール」をクリックします。
2. 右上の「エディターをインストール」ボタンをクリックします。
3. 「推奨リリース」タブに表示されている LTS バージョンを選択し「インストール」をクリックします。
4. **モジュール選択画面で以下にチェックが入っていることを確認してください。**
   - `Microsoft Visual Studio Community`（チェックを入れると VS と同時インストールされます）
   - `Documentation`（オフラインでドキュメントを参照できます）
5. 「続行」→「インストール」をクリックします。ダウンロードに数分〜十数分かかります。

> **ハマりポイント：Visual Studio を別途インストール済みの場合**
> 既に Visual Studio をインストールしている場合は、`Microsoft Visual Studio Community` のチェックを外しても構いません。
> ただし、Unity 開発ワークロードが追加されているかを「3. Visual Studio の設定」で必ず確認してください。

### 2-3. ライセンス認証（Personal ライセンスの取得）

Unity を使用するにはライセンス認証が必要です（無料の Personal ライセンスで問題ありません）。

1. Unity Hub の左上にあるアカウントアイコンをクリックし「サインイン」を選択します。
2. Unity ID をお持ちでない場合は「Unity ID を作成」からアカウントを作成してください（無料）。
3. サインイン後、左メニューの「設定」→「ライセンス」をクリックします。
4. 「新しいライセンスを追加」をクリックします。
5. 「Unity Personal を使用する」を選択し「実行」をクリックします。
6. 「Unity Personal を無料でお使いください」画面が表示されたら認証完了です。

> **ハマりポイント：ライセンス認証を忘れた場合**
> Unity Editor を起動しようとしたときに「ライセンスが見つかりません」というエラーが出ます。
> その場合は上記の手順 3〜6 を改めて実施してください。
> 「設定」→「ライセンス」から追加・更新が可能です。

---

## 3. Visual Studio のインストールと設定

### 3-1. Visual Studio のインストール

> **Unity Hub と同時にインストールした場合は、この手順はスキップできます。**
> ただし、以降の「3-2. ワークロードの確認」は必ず実施してください。

1. Visual Studio 公式サイト（ https://visualstudio.microsoft.com/ja/ ）を開きます。
2. 「Visual Studio Community 2022」の「無料ダウンロード」をクリックします。
3. ダウンロードしたインストーラー（`VisualStudioSetup.exe`）を実行します。
4. 「続行」をクリックするとワークロード選択画面が表示されます。
5. **「Unityによるゲーム開発」にチェックを入れます**（後述の通り、これを忘れるとデバッグができません）。
6. 「インストール」をクリックしてインストールを完了させます。

### 3-2. ワークロードの確認・追加

「Unityによるゲーム開発」ワークロードを選択し忘れた場合は、以下の手順で後から追加できます。

1. Windows の「スタートメニュー」→「Visual Studio Installer」を起動します。
2. 「Visual Studio Community 2022」の「変更」をクリックします。
3. 「ワークロード」タブの「Unityによるゲーム開発」にチェックを入れます。
4. 右下の「変更」をクリックしてインストールを完了させます。

> **このワークロードを忘れると何が起きるか**
> Unity でスクリプトをデバッグするとき、Visual Studio を Unity にアタッチする機能が使えなくなります。
> ブレークポイントを設定しても止まらない、という症状が出たらワークロードを確認してください。

### 3-3. Unity と Visual Studio の連携設定

Unity のスクリプトをダブルクリックしたとき、Visual Studio で開くように設定します。

1. Unity Editor を起動してプロジェクトを開きます。
2. メニューバーの「Edit」→「Preferences」をクリックします。
3. 左メニューの「External Tools」をクリックします。
4. 「External Script Editor」のドロップダウンを「Visual Studio 2022」に変更します。
5. 設定は自動保存されます。スクリプトファイルをダブルクリックして Visual Studio が開けば成功です。

---

## 4. Git のインストール

### 4-1. Windows へのインストール

1. Git 公式サイト（ https://git-scm.com/ ）を開きます。
2. 「Download for Windows」をクリックしてインストーラーをダウンロードします。
3. インストーラーを実行し、基本的にすべての設定をデフォルトのまま「Next」で進めます。
4. 「Choosing the default editor used by Git」の画面では、使い慣れたエディターを選択してください（Vim が不慣れな場合は「Use Visual Studio Code as Git's default editor」を推奨します）。
5. 「Adjusting your PATH environment」では「Git from the command line and also from 3rd-party software」を選択します（デフォルト）。
6. 最後まで進んで「Install」をクリックし、インストールを完了させます。

### 4-2. macOS へのインストール

1. ターミナルを開きます（Spotlight 検索で「ターミナル」と入力すると起動できます）。
2. 以下のコマンドを実行します。Homebrew がインストールされていない場合は先に Homebrew を導入してください。

```bash
brew install git
```

3. インストール後、以下のコマンドでバージョンが表示されれば成功です。

```bash
git --version
```

### 4-3. インストール後の初期設定

Git に自分の名前とメールアドレスを登録します。コミット履歴に表示されるため、正確な情報を設定してください。

```bash
git config --global user.name "山田 太郎"
git config --global user.email "yamada@example.com"
```

### 4-4. Windows での改行コード設定（Windows ユーザーのみ）

Windows と macOS・Linux では改行コードの仕様が異なります（Windows: CRLF、macOS/Linux: LF）。
そのまま作業するとファイルの差分が大量に出てしまうため、以下の設定を行います。

```bash
git config --global core.autocrlf true
```

> **この設定の意味**
> `true` に設定すると、チェックアウト時に LF → CRLF に変換し、コミット時に CRLF → LF に変換してくれます。
> これにより、チーム内で OS が混在していても改行コードの違いによるトラブルを防げます。

---

## 5. 動作確認

インストールが完了したら、以下の手順でそれぞれ正常に動作するかを確認してください。

### 5-1. Unity Hub の起動確認

1. デスクトップまたはスタートメニューから「Unity Hub」を起動します。
2. Unity Hub のホーム画面が表示され、インストールした Unity Editor のバージョンが「インストール」タブに表示されていることを確認します。

### 5-2. Unity Editor でのプロジェクト起動確認

1. Unity Hub の「プロジェクト」タブを開きます。
2. 「新しいプロジェクト」をクリックします。
3. テンプレートに「3D（URP）」または「3D」を選択し、プロジェクト名を入力して「プロジェクトを作成」をクリックします。
4. Unity Editor が起動し、空のシーンが表示されれば成功です。

### 5-3. Visual Studio の連携確認

1. Unity Editor の「Project」ウィンドウで「Assets」フォルダを右クリックします。
2. 「Create」→「C# Script」を選択し、スクリプトを作成します。
3. 作成したスクリプトファイルをダブルクリックします。
4. Visual Studio が起動し、スクリプトの内容が表示されれば成功です。

### 5-4. Git の動作確認

ターミナル（Windows: コマンドプロンプトまたは Git Bash）を開き、以下を実行します。

```bash
git --version
```

`git version 2.XX.X` のようにバージョン番号が表示されれば成功です。

---

## 6. よくあるハマりどころまとめ

### Git のパスが通っていない場合

Git をインストールしたのに `git --version` がエラーになる場合は、パスの設定を確認します。

```bash
# Windows（コマンドプロンプト）
where git

# macOS / Linux（ターミナル）
which git
```

コマンドが見つからない場合は、Git インストーラーを再実行して「Adjusting your PATH environment」の設定を確認してください。

### Visual Studio と Visual Studio Code の混同

「Visual Studio」と「Visual Studio Code」は**まったく別のソフトウェア**です。

| ソフト | 特徴 |
|---|---|
| Visual Studio | Microsoft の統合開発環境（IDE）。Unity との連携が充実している |
| Visual Studio Code | Microsoft の軽量テキストエディター。拡張機能が豊富だが Unity デバッグには追加設定が必要 |

Unity 研修では**Visual Studio**（IDE）を使用します。インストール時に混同しないよう注意してください。

### Unity のバージョンが複数インストールされている場合

Unity Hub では複数バージョンの Editor を管理できますが、プロジェクトを開くバージョンを間違えると正常に動作しません。

- プロジェクトの `ProjectSettings/ProjectVersion.txt` を開くと、そのプロジェクトで作成されたバージョンが記載されています。
- Unity Hub の「プロジェクト」タブでプロジェクト名の右側に表示されているバージョン番号を確認し、正しいバージョンで開くようにしてください。
- 異なるバージョンで開くとアップグレードの確認ダイアログが表示されます。研修では指定バージョン以外でのアップグレードは原則行わないでください。

---

## 7. 参考リンク

- Unity Hub 公式：https://unity.com/ja/download
- Visual Studio 公式：https://visualstudio.microsoft.com/ja/
- Git 公式：https://git-scm.com/
