# ツールの概要
ローカルリポジトリのコミット・プッシュ漏れをチェックするツール

# 使い方
* ルートディレクトリを入力、もしくはドラッグ＆ドロップ。
* `check`ボタンをクリック

# アプリの処理
* .gitのフォルダーを検索
* `git status`のコマンドを実行 … nothing to commit, working tree clean が返却されたらコミット漏れなしと判断
* `git log origin/master..master`のコマンドを実行 … ブランクであればPUSH漏れなしと判断

# 制限事項
ドライブをルートディレクトリとして指定できません。
