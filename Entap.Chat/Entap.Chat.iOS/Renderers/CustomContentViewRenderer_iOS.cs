using System;
using CoreGraphics;
using Entap.Chat;
using Entap.Chat.iOS;
using Foundation;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(CustomContentView), typeof(CustomContentViewRenderer_iOS))]
namespace Entap.Chat.iOS
{
    public class CustomContentViewRenderer_iOS : ViewRenderer
    {
        public CustomContentViewRenderer_iOS()
        {
            RegisterKeyboardObserver();
        }

        protected override void Dispose(bool disposing)
        {
            UnRegisterKeyboardObserver();
            base.Dispose(disposing);
        }

        // KeyboardObserver
        NSObject _keyboardShownObserver;
        NSObject _keyboardHiddenObserver;
        double _translationY;

        void RegisterKeyboardObserver()
        {
            if (_keyboardShownObserver == null)
            {
                _keyboardShownObserver = UIKeyboard.Notifications.ObserveWillShow(OnKeyboardShown);
            }
            if (_keyboardHiddenObserver == null)
            {
                _keyboardHiddenObserver = UIKeyboard.Notifications.ObserveWillHide(OnKeyboardHidden);
            }
        }

        void UnRegisterKeyboardObserver()
        {
            if (_keyboardShownObserver != null)
            {
                _keyboardShownObserver.Dispose();
                _keyboardShownObserver = null;
            }
            if (_keyboardHiddenObserver != null)
            {
                _keyboardHiddenObserver.Dispose();
                _keyboardHiddenObserver = null;
            }
        }

        void OnKeyboardShown(object sender, UIKeyboardEventArgs e)
        {
            // キーボード出現時に画面全体をずらす。
            NSValue result = (NSValue)e.Notification.UserInfo.ObjectForKey(new NSString(UIKeyboard.FrameEndUserInfoKey));
            CGSize keyboardSize = result.RectangleFValue.Size;
            if (Element != null)
            {
                _translationY = -keyboardSize.Height;
                Element.TranslationY = _translationY;
            }
        }

        void OnKeyboardHidden(object sender, UIKeyboardEventArgs e)
        {
            // キーボード消滅時に画面を元に戻す。
            if (Element != null)
            {
                Element.TranslationY -= _translationY;
            }
        }
    }
}
