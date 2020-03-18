using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using Xamarin.Forms.Internals;

namespace Entap.Chat
{
    /// <summary>
    /// ObservableCollectionの拡張メソッド
    /// </summary>
    [Preserve(AllMembers = true)]
    public static class ObservableCollectionExtensions
    {
        #region AddRange
        /// <summary>
        /// ObservableCollection型のデータに、IEnumerable型のデータを追加する
        /// </summary>
        public static void AddRange<T>(this ObservableCollection<T> source, IEnumerable<T> collection)
        {
            if (ValidateCollectionCount(source, collection))
            {
                return;
            }

            var property = typeof(ObservableCollection<T>).GetProperty("Items", BindingFlags.NonPublic | BindingFlags.Instance);
            var method = typeof(ObservableCollection<T>).GetMethod("OnCollectionReset", BindingFlags.NonPublic | BindingFlags.Instance);

            if (property.GetValue(source) is List<T> list)
            {
                list.AddRange(collection);
                method.Invoke(source, null);
            }
        }

        const int switchForeachThresold = 2;
        static bool ValidateCollectionCount<T>(ObservableCollection<T> source, IEnumerable<T> collection)
        {
            var count = collection.Count();
            if (count <= switchForeachThresold)
            {
                foreach (var item in collection)
                {
                    source.Add(item);
                }
                return true;
            }

            return false;
        }
        #endregion
    }
}
