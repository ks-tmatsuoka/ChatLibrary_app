using System;
using System.ComponentModel;
using System.Threading.Tasks;
using Entap.Chat;
using Entap.Chat.iOS;
using Foundation;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(DynamicResizedEditor), typeof(DynamicResizedEditorRenderer_iOS))]
namespace Entap.Chat.iOS
{
    public class DynamicResizedEditorRenderer_iOS : EditorRenderer
    {
        DynamicResizedEditor _dynamicResizedEditor;
        int _lineCount = 1;

        public DynamicResizedEditorRenderer_iOS()
        {
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Editor> e)
        {
            base.OnElementChanged(e);

            if (Control != null && e.NewElement != null)
            {
                _dynamicResizedEditor = e.NewElement as DynamicResizedEditor;

                this.Control.Layer.CornerRadius = _dynamicResizedEditor.CornerRadius;
                this.Control.Layer.MasksToBounds = true;
                this.Control.BackgroundColor = _dynamicResizedEditor.BackgroundColor.ToUIColor();                

                // 初期はスクロールを封じる。
                Control.ScrollEnabled = false;
                Control.Changed += OnChanged;
                _dynamicResizedEditor.Focused += OnFocused;
                _dynamicResizedEditor.Unfocused += OnUnFocused;
            }
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            if (e.PropertyName == DynamicResizedEditor.TextProperty.PropertyName)
            {
                ResizeIfNeeded();
            }
            //if (e.PropertyName == DynamicResizedEditor.IsFocusedProperty.PropertyName)
            //{
            //    var dynamicResizedEditor = Element as DynamicResizedEditor;
            //    if (dynamicResizedEditor.IsFocused)
            //    {
            //        dynamicResizedEditor.HeightRequest = -1;
            //        Control.ScrollEnabled = true;
            //    }
            //    //Resize();
            //    ResizeIfNeeded();
            //    if (dynamicResizedEditor.IsFocused)
            //    {
            //        EditorCursorMoveEnd();
            //    }
            //}
            //if (e.PropertyName == DynamicResizedEditor.ForcedMinimumHeightProperty.PropertyName)
            //{
            //    var dynamicResizedEditor = Element as DynamicResizedEditor;
            //    if (dynamicResizedEditor.ForcedMinimumHeight)
            //    {
            //        dynamicResizedEditor.HeightRequest = dynamicResizedEditor.MinimumHeightRequest;
            //        Control.ScrollEnabled = false;
            //    }
            //}
            if (e.PropertyName == DynamicResizedEditor.HeightRequestProperty.PropertyName)
            {
                var dynamicResizedEditor = Element as DynamicResizedEditor;
                if (dynamicResizedEditor.HeightRequest <= dynamicResizedEditor.MinimumHeightRequest)
                {
                    // エディターが入力状態でない時
                    Control.ScrollsToTop = true;
                    Control.ScrollEnabled = false;
                }
                else
                {
                    EditorCursorMoveEnd();
                }
            }
        }

        //void UpdateInputAccessoryView()
        //{
        //    if (_dynamicResizedEditor.IsInputAccessoryViewHiddeniOS)
        //    {
        //        Control.InputAccessoryView = null;
        //    }
        //}

        void EditorCursorMoveEnd()
        {
            Task.Run(async () =>
            {
                await Task.Delay(250);
                // エディターのカーソルを末尾に
                Device.BeginInvokeOnMainThread(() =>
                {
                    if (Control.Text.Length > 0)
                    {
                        var loc = Control.Text.Length - 1;
                        NSRange range = new NSRange(0, Control.Text.Length);
                        Control.ScrollRangeToVisible(range);
                    }
                    Control.SelectedTextRange = Control.GetTextRange(Control.EndOfDocument, Control.EndOfDocument);
                });
            });
        }

        void OnChanged(object sender, EventArgs e)
        {
            ResizeIfNeeded();
        }

        void OnFocused(object sender, FocusEventArgs e)
        {
            _dynamicResizedEditor.HeightRequest = -1;
            Control.ScrollEnabled = true;
            ResizeIfNeeded();
            EditorCursorMoveEnd();
        }

        void OnUnFocused(object sender, FocusEventArgs e)
        {
            _dynamicResizedEditor.HeightRequest = _dynamicResizedEditor.MinimumHeightRequest;
            Control.ScrollEnabled = false;
        }

        void ResizeIfNeeded()
        {
            var dynamicResizedEditor = Element as DynamicResizedEditor;
            var fitHeight = Control.SizeThatFits(new CoreGraphics.CGSize(Control.Bounds.Width, nfloat.MaxValue)).Height;
            var lineCount = CalculateLineCount(fitHeight);
            if (lineCount != _lineCount)
            {
                dynamicResizedEditor.Resize(lineCount);
                Control.ScrollEnabled = (lineCount > dynamicResizedEditor.MaxDisplayLineCount);
                _lineCount = lineCount;
            }
        }

        int CalculateLineCount(nfloat fitHeight)
        {
            var fontHeight = Control.Font.LineHeight;
            return (int)Math.Round(fitHeight / fontHeight);
        }
    }
}
