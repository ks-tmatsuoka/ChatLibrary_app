using System;
using Xamarin.Forms;
using Android.Graphics;
using System.Threading.Tasks;
using Xamarin.Forms.Platform.Android;
using Java.IO;
using Android.Media;
using System.IO;
using Xamarin.Forms.Internals;
using Entap.Chat.Droid;

[assembly: Dependency(typeof(ImageService_Droid))]
namespace Entap.Chat.Droid
{
    [Preserve(AllMembers = true)]
    public class ImageService_Droid : IImageService
    {
        public ImageSource DownSizeImage(string filePath, double limitSize)
        {
            var taskCmp = new TaskCompletionSource<ImageSource>();
            Task.Run(() =>
            {
                try 
                {
                    BitmapFactory.Options bmfOptions = new BitmapFactory.Options();
                    bmfOptions.InJustDecodeBounds = true;
                    int sampleSize;

                    if (filePath.Contains("http"))
                    {
                        //サーバの画像
                        var url = new Java.Net.URL(filePath);
                        var istream = url.OpenStream();
                        BitmapFactory.DecodeStream(istream, null, bmfOptions);
                        istream.Close();
                    }
                    else
                    {
                        // ローカルの画像
                        BitmapFactory.DecodeFile(filePath, bmfOptions);
                    }

                    // サイズ取得し縮小するscaleを求める
                    if ((bmfOptions.OutWidth * bmfOptions.OutHeight) > limitSize)
                    {
                        //１Mピクセル超えてる
                        double outArea = (double)(bmfOptions.OutWidth * bmfOptions.OutHeight) / limitSize;
                        sampleSize = (int)(Math.Sqrt(outArea) + 1);
                    }
                    else
                    {
                        //小さいのでそのまま
                        sampleSize = 1;
                    }

                    // 画像を実際に読み込む指定
                    bmfOptions.InJustDecodeBounds = false;
                    // 画像を1/InSampleSize サイズに縮小（メモリ対策）
                    bmfOptions.InSampleSize = sampleSize;
                    // システムメモリ上に再利用性の無いオブジェクトがある場合に勝手に解放（メモリ対策）
                    bmfOptions.InPurgeable = true;

                    Bitmap orgBitmap = null;
                    if (filePath.Contains("http"))
                    {
                        //サーバの画像
                        var url = new Java.Net.URL(filePath);
                        var istream = url.OpenStream();
                        orgBitmap = BitmapFactory.DecodeStream(istream, null, bmfOptions);
                        istream.Close();
                    }
                    else
                    {
                        orgBitmap = BitmapFactory.DecodeFile(filePath, bmfOptions);
                    }

                    var width = orgBitmap.Width;
                    var height = orgBitmap.Height;
                    var mtx = GetImageOrientation(filePath);
                    if (mtx == null)
                    {
                        taskCmp.SetResult(null);
                    }

                    // マトリックスを反映させたBitmapを生成する。 メモリオーバー対策でusing使用
                    using (Bitmap bitmap = Bitmap.CreateBitmap(orgBitmap, 0, 0, width, height, mtx, true))
                    {
                        // Bitmap -> byte
                        byte[] bytes = null;
                        using (var stream = new MemoryStream())
                        {
                            bitmap.Compress(Bitmap.CompressFormat.Png, 100, stream);
                            bytes = stream.ToArray();
                            var imagsource = ImageSource.FromStream(() => new MemoryStream(bytes));
                            taskCmp.SetResult(imagsource);
                        }
                    }
                }
                catch(Exception e)
                {
                    System.Diagnostics.Debug.WriteLine(filePath + " : " + e);
                    taskCmp.SetResult(null);
                }

            });
            return taskCmp.Task.Result;
        }

        Matrix GetImageOrientation(string filePath)
        {
            // Bitmap -> ImageSource
            // マトリックスを回転情報から決定する。
            var matrix = new Matrix();
            if (filePath.Contains("http"))
                return matrix;

            try
            {
                var exifInterface = new ExifInterface(filePath);
                var o = (Orientation)exifInterface.GetAttributeInt(ExifInterface.TagOrientation, (int)Orientation.Undefined);

                if (o == Orientation.Normal)
                {
                    // なにもしない
                }
                else if (o == Orientation.FlipHorizontal)
                {
                    // 水平方向にリフレクト
                    matrix.PostScale(-1f, 1f);
                }
                else if (o == Orientation.Rotate180)
                {
                    // 180度回転
                    matrix.PostRotate(180f);
                }
                else if (o == Orientation.FlipVertical)
                {
                    // 垂直方向にリフレクト
                    matrix.PostScale(1f, -1f);
                }
                else if (o == Orientation.Rotate90)
                {
                    // 反時計周り90度回転
                    matrix.PostRotate(90f);
                }
                else if (o == Orientation.Transverse)
                {
                    // 時計回り90度回転し、垂直方向にリフレクト
                    matrix.PostRotate(-90f);
                    matrix.PostScale(1f, -1f);
                }
                else if (o == Orientation.Transpose)
                {
                    // 反時計回り90度回転し、垂直方向にリフレクト
                    matrix.PostRotate(90f);
                    matrix.PostScale(1f, -1f);
                }
                else if (o == Orientation.Rotate270)
                {
                    // 反時計回りに270度回転＝時計回りに90度回転
                    matrix.PostRotate(-90f);
                }
                return matrix;
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.ToString());
                return matrix;
            }
        }
    }
}
