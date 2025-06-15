using System.Collections.Generic;

namespace Kamgam.UGUINavigationWizard
{
    public static class Extensions
    {
        public static bool IsNullOrEmpty<T>(this IList<T> list)
        {
            return (list == null || list.Count == 0);
        }

        public static bool IsNullOrEmptyDeep<T>(this IList<T> list) where T : class
        {
            if (list == null || list.Count == 0)
                return true;

            foreach (var item in list)
            {
                if (item != null)
                    return false;
            }

            return true;
        }
    }
}
