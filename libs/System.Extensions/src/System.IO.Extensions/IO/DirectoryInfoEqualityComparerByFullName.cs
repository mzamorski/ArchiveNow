using System.Collections.Generic;
using System.IO;

namespace System.Extensions.IO
{

    /// <summary>
    /// Simple equality comparer for <see cref="DirectoryInfo"/> objects
    /// based on the full name.
    /// </summary>
    public class DirectoryInfoEqualityComparerByFullName
        : IEqualityComparer<DirectoryInfo>
    {

        #region Public Methods

        /// <summary>
        /// Returns wether both <see cref="DirectoryInfo"/> objects have the same full name.
        /// </summary>
        /// <param name="x">The first <see cref="DirectoryInfo"/> object to compare.</param>
        /// <param name="y">The second <see cref="DirectoryInfo"/> object to compare.</param>
        /// <returns></returns>
        public Boolean Equals(DirectoryInfo x, DirectoryInfo y)
        {
            return (x != null && y != null &&
                String.Equals(x.FullName, y.FullName, StringComparison.CurrentCultureIgnoreCase));
        }

        /// <summary>
        /// Returns the hash code for the given <see cref="DirectoryInfo"/> object.
        /// </summary>
        /// <param name="obj">The <see cref="DirectoryInfo"/> object for which the hash code should be returned.</param>
        /// <returns></returns>
        public int GetHashCode(DirectoryInfo obj)
        {
            // The .Net framework uses the address on the managed heap to generate the hash code.
            // While it would be a bit more tricky for us, we can delegate this task and indirectly make
            // use of the .Net Framework implementation by calling GetHashCode on a string object
            // that is supposed to uniquely identify the instance.
            if (obj != null)
                return obj.FullName.GetHashCode();

            return String.Empty.GetHashCode();
        }

        #endregion

    }

}

