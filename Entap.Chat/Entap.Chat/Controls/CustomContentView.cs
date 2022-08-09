using System;
using Xamarin.Forms;
using Xamarin.Forms.Internals;

namespace Entap.Chat
{
    [Preserve(AllMembers = true)]
    public class CustomContentView : ContentView
    {
        public CustomContentView()
        {
            EnableKeyboardOverlappingEffect();
        }

        void EnableKeyboardOverlappingEffect()
        {
            SetValue(Entap.Forms.Effects.Platform.iOS.KeyboardOverlappingEffect.IsEnabledProperty, true);
            SetValue(Entap.Forms.Effects.Platform.iOS.KeyboardOverlappingEffect.BottomMarginProperty, 0d);
        }
    }
}
