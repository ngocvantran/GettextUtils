using System;

namespace Resx2Po
{
    public class StringInfo
    {
        /// <summary>
        /// Gets or sets the key.
        /// </summary>
        /// <value>
        /// The key.
        /// </value>
        public string Key { get; set; }

        /// <summary>
        /// Gets or sets the resource path.
        /// </summary>
        /// <value>
        /// The resource path.
        /// </value>
        public string Path { get; set; }

        /// <summary>
        /// Gets or sets the source language text.
        /// </summary>
        /// <value>
        /// The source language text.
        /// </value>
        public string Source { get; set; }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>
        /// The value.
        /// </value>
        public string Value { get; set; }
    }
}