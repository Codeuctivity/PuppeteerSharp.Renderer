using System;

namespace Codeuctivity.HtmlRenderer
{
    /// <summary>
    /// Throwing on missing CreateAsync
    /// </summary>
    [Serializable]
    public class RendererException : Exception
    {
        /// <summary>
        /// Throwing on missing CreateAsync
        /// </summary>
        public RendererException()
        {
        }

        /// <summary>
        /// Throwing on missing CreateAsync
        /// </summary>
        public RendererException(string? message) : base(message)
        {
        }

        /// <summary>
        /// Throwing on missing CreateAsync
        /// </summary>
        public RendererException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}