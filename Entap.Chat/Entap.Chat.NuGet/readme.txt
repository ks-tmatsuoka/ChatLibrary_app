
This readme.txt file will be displayed immediately after the NuGet package is installed.

[利用する際の手順]
１.初期化処理を追加する
・AppDelegate
Entap.Chat.iOS.Platform.Init();

・MainActivity
Entap.Chat.Android.Platform.Init(this);

・App.xaml.cs
Entap.Chat.Settings.Current.Init(new ChatService(), new ChatControlService());
※ChatService、ChatControlServiceはインターフェースの項目で説明する各インターフェースを継承したクラスを指定する

2.ffimageを使用しているためffimageのnugetを追加+ffimageの初期化処理を書く



[コントロールについて]
3つのコントロールがある

・ChatListView
チャットのリスト部分のコントロール。各リスト内のViewCellは利用者が定義する
RoomIdとIsGroupChatとLastReadMessageIdはバインド必須
ChatListViewを単体で使用する際はAddMessageCommandをMode=OneWayToSource、AddMessageCommandParameterをMode=TwoWayでバインドする必要があるので注意


・BottomController
画面下に配置するメッセージの入力送信を行うコントローラ
デフォルトではカメラから動画、画像の送信とライブラリから動画、画像の送信がサポートされている
テキスト入力欄の左にあるライブラリから画像を取得等のメニューをカスタムして配置したい場合、BottomContorollerMenuViewBaseを継承したViewを作成し、それをBottomControllerのMenuViewプロパティにセットして使うことができる

・ChatControl
ChatListViewとBottomControllerを合わせたコントロールで各メッセージのViewCellも定義済みのもの
BottomControllerのMenuView部分はカスタムできるようになっている
RoomIdとIsGroupChatとLastReadMessageIdはバインド必須



[インターフェースについて]
メッセージの受信、送信、更新の処理やBottomController内のMenuViewのボタン押した際の各処理などについては下記のインターフェースを継承し、そこに処理を書く必要がある

・IChatService
このライブラリを使う上で必ず処理を書く必要のあるもの

・IChatControlService
ChatControlを使う際、必ず処理を書く必要のあるもの



[Entap.Chat.Settingsについて]
ここでチャット内で使う文言のフォーマットの設定などを変更できる



[外部ライブラリについて]
・Entap.Chatで使用しているライブラリ一覧

Entap.Chat
    Xamarin.FFImageLoading.Forms

Entap.Chat.Android
    Xamarin.FFImageLoading.Forms

Entap.Chat.iOS
    Xamarin.FFImageLoading.Forms


・このライブラリを組み込む際にプロジェクトに追加が必要なライブラリ一覧
sqlite-net-pcl 
Xamarin.FFImageLoading.Forms


・WebSocket関連のライブラリについて
サーバ側がRatchetを使ってWebSocket実装を行っている場合、アプリ側は以下２つの外部ライブラリを使用
    WampSharp
    WampSharp.WAMP1.Default
※実装方法ははサンプルプロジェクトのコードを参照





その他処理については、サンプルプロジェクトを参考にしてください