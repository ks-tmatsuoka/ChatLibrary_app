using System;
using Foundation;

namespace Entap.Chat.iOS
{
    [Preserve(AllMembers = true)]
    public static class Platform
    {
        public static void Init()
        {
            Entap.Froms.Effects.iOS.Platform.Init();
        }
    }
}
