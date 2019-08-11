using System;

namespace Yolol.Analysis
{
    public static class ObjectExtensions
    {
        public static T Fixpoint<T>(this T item, Func<T, T> transform)
            where T : IEquatable<T>
        {
            return item.Fixpoint(int.MaxValue, transform);
        }

        public static T Fixpoint<T>(this T item, int maxIters, Func<T, T> transform)
            where T : IEquatable<T>
        {
            for (var i = 0; i < maxIters; i++)
            {
                var prev = item;
                item = transform(prev);

                if (prev.Equals(item))
                    break;
            }

            return item;
        }
    }
}
