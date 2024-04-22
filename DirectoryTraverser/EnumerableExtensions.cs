namespace DelegatesTraverser.Extensions
{
    public static class EnumerableExtensions
    {
        public static T GetMax<T>(this IEnumerable<T> collection, Func<T, float> convertToNumber) where T : class
        {
            ArgumentNullException.ThrowIfNull(collection);
            ArgumentNullException.ThrowIfNull(convertToNumber);

            T maxItem = null;
            float maxNumber = float.MinValue;

            foreach (T item in collection)
            {
                float itemNumber = convertToNumber(item);
                if (itemNumber > maxNumber)
                {
                    maxNumber = itemNumber;
                    maxItem = item;
                }
            }

            return maxItem;
        }
    }
}
