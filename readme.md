# poteto-tweet

**poteto-tweet** は、C# (.NET 8) と Oracle19c を利用して開発された、ツイート投稿やコメント、フォロー・ブロック、いいね機能を持つ Web API プロジェクトです。  
本プロジェクトは、クリーンアーキテクチャを採用し、各レイヤー（WebAPI、Application、Domain、Infrastructure）で責務を明確に分離しています。

---

## 特徴

- **ユーザ管理:**  
  ユーザ登録、認証機能を提供し、メールアドレスおよびパスワードのバリデーションを実施。

- **ツイート関連機能:**  
  ツイート投稿、ツイート一覧表示、ツイートの更新、ツイートに対するコメント機能を実装。

- **フォロー・ブロック機能:**  
  ユーザ同士のフォロー・ブロック操作、一覧表示機能を提供。

- **いいね機能:**  
  ツイートに対するいいね操作（登録・解除）およびいいね一覧の取得が可能。

- **ログ出力:**  
  Serilog を利用して、開発環境ではコンソールとファイル、本番環境ではファイルへのログ出力を実現。  
  SQL の実行時間や遅延SQLの場合、詳細なログ出力も行います。

- **将来的なDB変更対応:**  
  現在は Oracle19c を利用していますが、DB アクセスの抽象化により、将来的に PostgreSQL などへの移行を容易にする設計です。

---

## 技術スタック

- **プラットフォーム:**  
  Windows / VSCode / .NET 8

- **データベース:**  
  Oracle19c（将来的に PostgreSQL への移行を検討）

- **アーキテクチャ:**  
  クリーンアーキテクチャ  
  - プレゼンテーション層: Poteto.WebAPI  
  - アプリケーション層: Poteto.Application  
  - ドメイン層: Poteto.Domain  
  - インフラ層: Poteto.Infrastructure

- **ライブラリ:**  
  - Dapper (軽量なデータアクセス)  
  - Serilog (柔軟なログ出力)  

- **その他:**  
  Minimal API（Program.cs での設定）  
  Swagger（API ドキュメント生成）

---

## ディレクトリ構成

```
poteto-tweet/                  // Git リポジトリのルート
├── README.md                  // このファイル
├── .gitignore                 // Git 管理から除外するファイル指定
├── Poteto.sln                 // ソリューションファイル
├── docs/                      // 設計ドキュメント、仕様書など
├── tests/                     // ユニットテスト、統合テスト
└── src/                       // ソースコード
    ├── Poteto.WebAPI/        // プレゼンテーション層 (WebAPI)
    │   ├── Controllers/      // 各 API エンドポイントの実装
    │   ├── Middleware/       // ログ出力、エラーハンドリングなどのミドルウェア
    │   ├── Program.cs        // エントリーポイントおよび DI 設定
    │   └── appsettings*.json // 環境ごとの設定ファイル
    ├── Poteto.Application/   // アプリケーション層 (ユースケース、DTO、サービス)
    ├── Poteto.Domain/        // ドメイン層 (エンティティ、値オブジェクト、ドメインサービス)
    └── Poteto.Infrastructure/ // インフラ層 (DB アクセス、Unit of Work、ログ設定、構成)
```

---

## セットアップ手順

1. **リポジトリのクローン**

   ```bash
   git clone https://github.com/your_username/poteto-tweet.git
   cd poteto-tweet
   ```

2. **依存パッケージのインストール**

   各プロジェクトフォルダ内で以下のコマンドを実行してください。

   ```bash
   dotnet restore
   ```

3. **データベースのセットアップ**

   - `Poteto.Infrastructure/Scripts` フォルダ内にある SQL スクリプト（CreateTables.sql 等）を利用して、Oracle19c に必要なテーブルを作成します。

4. **環境設定**

   - `src/Poteto.WebAPI/appsettings.json` で、接続文字列やログレベルなどを設定してください。

5. **ビルドと実行**

   ```bash
   dotnet run --project src/Poteto.WebAPI/Poteto.WebAPI.csproj
   ```

   ※ Minimal API を利用しているため、Swagger UI も有効です（開発環境の場合）。

---

## コミットルール

- **コミットメッセージ:**  
  基本的に日本語で記述してください。  
  例: `初回コミット: プロジェクトの初期セットアップ`

---

## その他

- **ドキュメント:**  
  詳細な仕様書や設計資料は `docs/` フォルダにまとめています。

- **テスト:**  
  ユニットテストや統合テストの実行については、`tests/` フォルダ内の各プロジェクトをご参照ください。
