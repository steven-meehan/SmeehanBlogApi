using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmeehanBlogApi.Progress
{
    /// <summary>
    /// This configures the code to work with the Progress Stack.
    /// </summary>
    public class ProgressOptions
    {
        /// <summary>
        /// This flag enables switching between the <see cref="ProgressStore"/> and the <see cref="MockProgressStore"/>
        /// during local development.
        /// </summary>
        public bool MockStore { get; set; } = false;

        /// <summary>
        /// Specifies the Quote Table name.
        /// </summary>
        public string PropertyName { get; set; } = "Active";
    }
}
