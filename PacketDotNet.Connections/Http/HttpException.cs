/*
 * Copyright 2019 Chris Morgan <chmorgan@gmail.com>
 *
 * GPLv3 licensed, see LICENSE for full text
 * Commercial licensing available
 */
using System;

namespace PacketDotNetConnections.Http
{
    /// <summary>
    /// Base class for http exceptions
    /// </summary>
    public class HttpException : SystemException
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="exception">
        /// A <see cref="string"/>
        /// </param>
        public HttpException(string exception) : base(exception)
        {
        }

        /// <summary>
        /// Constructor with message
        /// </summary>
        /// <param name="exception">
        /// A <see cref="string"/>
        /// </param>
        /// <param name="e">
        /// A <see cref="Exception"/>
        /// </param>
        public HttpException(string exception, Exception e) : base(exception, e)
        {
        }
    }

    /// <summary>
    /// Thrown when http version parsing fails
    /// </summary>
    public class HttpVersionParsingException : HttpException
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="exception">
        /// A <see cref="string"/>
        /// </param>
        public HttpVersionParsingException(string exception) : base(exception)
        {
        }
    }

    /// <summary>
    /// Thrown if status code parsing fails
    /// </summary>
    public class HttpStatusCodeParsingException : HttpException
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="exception">
        /// A <see cref="string"/>
        /// </param>
        public HttpStatusCodeParsingException(string exception) : base(exception)
        {
        }
    }

    /// <summary>
    /// Chunk length was not valid
    /// </summary>
    public class HttpChunkLengthParsingException : HttpException
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="exception">
        /// A <see cref="string"/>
        /// </param>
        /// <param name="e">
        /// A <see cref="Exception"/>
        /// </param>
        public HttpChunkLengthParsingException(string exception, Exception e) :
            base(exception, e)
        {
        }
    }

    /// <summary>
    /// Content length invalid
    /// </summary>
    public class HttpContentLengthParsingException : HttpException
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="exception">
        /// A <see cref="string"/>
        /// </param>
        public HttpContentLengthParsingException(string exception) : base(exception)
        {
        }
    }
}
