using System;
using System.Collections.Generic;
using System.Text;

namespace Vestris.ResourceLib
{
    /// <summary>
    /// A resource load exception.
    /// </summary>
    public class LoadException : Exception
    {
        private Exception _outerException = null;

        /// <summary>
        /// The Win32 exception from a resource enumeration function.
        /// </summary>
        public Exception OuterException
        {
            get
            {
                return _outerException;
            }
        }

        /// <summary>
        /// A new resource load exception.
        /// </summary>
        /// <param name="message">Error message.</param>
        /// <param name="innerException">The inner exception thrown within a single resource.</param>
        /// <param name="outerException">The outer exception from the Win32 API.</param>
        public LoadException(string message, Exception innerException, Exception outerException)
            : base(message, innerException)
        {
            _outerException = outerException;
        }

        /// <summary>
        /// A combined message of the inner and outer exception.
        /// </summary>
        public override string Message
        {
            get
            {
                return _outerException != null ? string.Format("{0} {1}.",
                    base.Message, _outerException.Message) : base.Message;
            }
        }
    }
}
