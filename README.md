Windows Azure Toolkit for Windows 8 on Windows Azure WebSites
==
このプロジェクトはWindows Azure Toolkit for Windows 8に含まれるPush通知用
サーバー部分（Cloud Serviceを使用したWindows Azure部分）だけを抜き出して
Windows Azure Webサイトで動作するように変更したものです。

プロジェクトについて
----
もともとCloud Serviceを使用する形態だったので少し使い勝手が悪い部分を
Windows Azure Webサイトで動作するように手直ししたものです。

元ネタはこちら - [Windows Azure Toolkit for Windows 8](http://watwindows8.codeplex.com/)

Windows Azure Webサイトに対応させた背景
----
+ Push通知用サーバー立てるのにわざわざ面倒なパッケージ作成とデプロイしたくない
+ ローカルでのデバッグがしにくい（証明書やらWindows Azureのエミュレータやらポートの問題やら）
+ Windows Azure Webサイトならデプロイ簡単＋SSLが使える


#### 環境 ####
以下の環境で作成しています。

+ Windows 7 SP1 x64 JPN
+ Visual Studio 2010 SP1
+ ASP.NET MVC 4
+ ASP.NET WebAPI
+ Windows Azure SDK for .NET 1.7
+ Push Notification Registration Cloud Service 1.0.6
+ WnsRecipe 0.0.3.2
+ Membership Providers for Windows Azure 1.0.3

細かいコンポーネント類は参照してるNuGetパッケージを見てください。
Windows 8 Release Preview上のVisual Studio 2012 RCでもビルド可。

#### 利用方法 ####
1. Windows Azure Webサイトを作成して PushNotificationWeb.zip をUploadするか、ソースをビルドして
適当にWebサイトにデプロイします。

2. 管理ポータルのWindows Azure WebサイトにあるConfigureページを開き、appSettingsを追加します。
追加する設定は以下の通り。

+ AdministratorEmail
        + サイト管理者のEmailアドレス
+ AdministratorName
        + サイト管理者のユーザー名（ログインに必要）
+ AdministratorPassword
        + サイト管理者のパスワード（ログインに必要）
+ DataConnectionString
        + Windows Azure Storageへの接続文字列
+ TileImagesContainer
        + Push通知に使用する画像を保存するWindows Azure Blobストレージのコンテナ名
+ WNSPackageSID
        + WNSに登録されているパッケージのSID（ms-app://～で始まる文字列）
+ WNSClientSecret
        + WNSに登録されているクライアントシークレットキー

3. 設定完了後にWebサイトに接続すれば、Windows Azure StorageのTable/Blob上に必要な情報が登録されます。

4. クライアント側は https://yoursite.azurewebsites.net/endpoints へつないでチャネルを登録すればOKです。


ライセンス
----
## Microsoft Public License (Ms-PL)
***
This license governs use of the accompanying software. If you use the software, you accept this license. If you do not accept the license, do not use the software.

1. Definitions

The terms "reproduce," "reproduction," "derivative works," and "distribution" have the same meaning here as under U.S. copyright law.

A "contribution" is the original software, or any additions or changes to the software.

A "contributor" is any person that distributes its contribution under this license.

"Licensed patents" are a contributor's patent claims that read directly on its contribution.

2. Grant of Rights

(A) Copyright Grant- Subject to the terms of this license, including the license conditions and limitations in section 3, each contributor grants you a non-exclusive, worldwide, royalty-free copyright license to reproduce its contribution, prepare derivative works of its contribution, and distribute its contribution or any derivative works that you create.

(B) Patent Grant- Subject to the terms of this license, including the license conditions and limitations in section 3, each contributor grants you a non-exclusive, worldwide, royalty-free license under its licensed patents to make, have made, use, sell, offer for sale, import, and/or otherwise dispose of its contribution in the software or derivative works of the contribution in the software.

3. Conditions and Limitations

(A) No Trademark License- This license does not grant you rights to use any contributors' name, logo, or trademarks.

(B) If you bring a patent claim against any contributor over patents that you claim are infringed by the software, your patent license from such contributor to the software ends automatically.

(C) If you distribute any portion of the software, you must retain all copyright, patent, trademark, and attribution notices that are present in the software.

(D) If you distribute any portion of the software in source code form, you may do so only under this license by including a complete copy of this license with your distribution. If you distribute any portion of the software in compiled or object code form, you may only do so under a license that complies with this license.

(E) The software is licensed "as-is." You bear the risk of using it. The contributors give no express warranties, guarantees or conditions. You may have additional consumer rights under your local laws which this license cannot change. To the extent permitted under your local laws, the contributors exclude the implied warranties of merchantability, fitness for a particular purpose and non-infringement.
