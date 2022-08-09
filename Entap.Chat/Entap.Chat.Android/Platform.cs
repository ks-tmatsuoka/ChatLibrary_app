using System;
using Android.App;
using Android.Runtime;

namespace Entap.Chat.Android
{
    [Preserve(AllMembers = true)]
    public static class Platform
    {
        static Activity _activity;
        public static Activity Activity => _activity;
        public static void Init(Activity activity)
        {
            _activity = activity;
            Entap.Froms.Effects.Android.Platform.Init();
        }
    }
}
