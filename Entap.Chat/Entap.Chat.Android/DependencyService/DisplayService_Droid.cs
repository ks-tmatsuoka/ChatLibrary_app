using System;
using Android.Content;
using Android.Util;
using Entap.Chat.Android;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: Dependency(typeof(DisplayService_Droid))]
namespace Entap.Chat.Android
{
    public class DisplayService_Droid : IDisplayService
    {
        public Rectangle GetDisplaySize()
        {
            var context = Platform.Activity.ApplicationContext;
            if (context is null)
                return new Rectangle(0, 0, 0, 0);
            double height;
            double width;
            var resources = context.Resources;
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
                context.GetActivity().WindowManager.DefaultDisplay.GetMetrics(metrics);
                width = (double)metrics.WidthPixels / (double)metrics.Density;
                height = (double)metrics.HeightPixels / (double)metrics.Density;
            }
            var rect = new Rectangle(0, 0, width, height);
            if (rect.Width > 0)
                return new Rectangle(0, 0, rect.Width, rect.Height);
            return new Rectangle(0, 0, 100, 100);
        }

        public Thickness GetSafeArea()
        {
            return new Thickness(0, 0, 0, 0);
        }
    }
}
