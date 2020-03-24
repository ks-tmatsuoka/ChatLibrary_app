using System;
using CoreGraphics;
using Entap.Chat;
using Entap.Chat.iOS;
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
        Foundation.NSObject _keyboardShownObserver;
        Foundation.NSObject _keyboardHiddenObserver;

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
            System.Diagnostics.Debug.WriteLine("Keyboard is shown.");

            // キーボード出現時に画面全体をずらす。
            var keyboardFrame = e.FrameEnd;
            nfloat dy = 0;
            if (UIDevice.CurrentDevice.CheckSystemVersion(11, 0))
            {
                var window = UIApplication.SharedApplication.KeyWindow;
                if (window != null)
                {
                    dy = window.SafeAreaInsets.Bottom;
                }
            }
            var duration = e.AnimationDuration;

            var pageFrame = Element.Bounds;
            UIView.Animate(duration, () =>
            {
                var trans = CGAffineTransform.MakeTranslation(0, -keyboardFrame.Height + dy);
                this.Transform = trans;
            });
        }

        void OnKeyboardHidden(object sender, UIKeyboardEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Keyboard is hidden.");

            // キーボード消滅時に画面を元に戻す。
            var duration = e.AnimationDuration;

            var pageFrame = Element.Bounds;
            UIView.Animate(duration, async () =>
            {
                // 少し遅延させないと、UIButtonのタップが正常に取れない
                await System.Threading.Tasks.Task.Delay(10);
                var trans = CGAffineTransform.MakeIdentity();
                this.Transform = trans;
            });
        }
    }
}
