using System;
using Xamarin.Forms;
using Android.Content;
using Android.Util;
using Mono;
using Xamarin.Forms.Platform.Android;

namespace Entap.Chat.Android
{
    public class DisplayManager
    {
        public static DisplayManager Current => instance;
        private static readonly DisplayManager instance = new DisplayManager();
        
        public DisplayManager()
        {
        }

        private Context context;
        public Context Context
        {
            get
            {
                return context;
            }
            set
            {
                if (context != value)
                {
                    context = value;
                }
            }
        }

        public Rectangle GetDisplaySize()
        {
            if (Context is null)
                return new Rectangle(0, 0, 0, 0);
            double height;
            double width;
            var resources = Context.Resources;
            var resourceId1 = resources.GetIdentifier("status_bar_height", "dimen", "android");
            if (resourceId1 > 0)
            {
                var metrics = resources.DisplayMetrics;
                // ステータスバーの高さを取得
                var statusBarHeight = resources.GetDimensionPixelSize(resourceId1) / metrics.Density;
                // 画面全体の高さを取得(ステータスバーを含む)
                var totalHeight = metrics.HeightPixels / metrics.Density;
                // コンテンツ領域の高さを取得
                height = totalHeight - statusBarHeight;
                // 幅を取得
                width = metrics.WidthPixels / metrics.Density;
            }
            else
            {
                DisplayMetrics metrics = new DisplayMetrics();
                Context.GetActivity().WindowManager.DefaultDisplay.GetMetrics(metrics);
                width = (double)metrics.WidthPixels / (double)metrics.Density;
                height = (double)metrics.HeightPixels / (double)metrics.Density;
            }
            return new Rectangle(0, 0, width, height);
        }
    }
}
