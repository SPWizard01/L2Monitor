/*
 * Copyright 2019 Chris Morgan <chmorgan@gmail.com>
 *
 * GPLv3 licensed, see LICENSE for full text
 * Commercial licensing available
 */
using Serilog;
using System;
using System.Collections.Generic;

namespace PacketDotNetConnections.Http
{
    /// <summary>
    /// HTTP request, the first phase of an http exchange
    /// </summary>
    public class HttpRequest : HttpMessage
    {
        private static readonly ILogger Log = Serilog.Log.ForContext<HttpRequest>();

        /// <summary>
        /// Types of requests
        /// </summary>
        public enum Methods
        {
            /// <summary>
            /// Unknown method
            /// </summary>
            Unknown,

            /// <summary>
            /// Head
            /// </summary>
            Head,

            /// <summary>
            /// Get
            /// </summary>
            Get,

            /// <summary>
            /// Post
            /// </summary>
            Post,

            /// <summary>
            /// Put
            /// </summary>
            Put,

            /// <summary>
            /// Delete
            /// </summary>
            Delete,

            /// <summary>
            /// Trace
            /// </summary>
            Trace,

            /// <summary>
            /// Options
            /// </summary>
            Options,

            /// <summary>
            /// Connect
            /// </summary>
            Connect
        }

        /// <summary>
        /// The method of this request
        /// </summary>
        public Methods Method;

        /// <summary>
        /// Url in the request
        /// </summary>
        public string Url;

        /// <summary>
        /// Constructor
        /// </summary>
        public HttpRequest()
        {
            Log.Debug("");
        }

        /// <summary>
        /// Map of cookie names and values
        /// </summary>
        public Dictionary<string, string> Cookies
        {
            get
            {
                Dictionary<string, string> cookieDictionary = new Dictionary<string, string>();

                // do we have any cookies in the header?
                if (!Headers.ContainsKey("Cookie"))
                {
                    return cookieDictionary;
                }

                string cookieText = Headers["Cookie"];

                // split the cookie into combined key/value pairs
                string[] pairSplitDelmiters = new string[1];
                pairSplitDelmiters[0] = "; ";
                string[] cookies = cookieText.Split(pairSplitDelmiters, StringSplitOptions.None);

                // split each combined key/value pair into a key and value
                string[] keyValueSplitDelmiters = new string[1];
                keyValueSplitDelmiters[0] = "=";
                foreach (string keyvalue in cookies)
                {
                    string[] key_and_value = keyvalue.Split(keyValueSplitDelmiters,
                                                            StringSplitOptions.None);

                    // store the key and value into the collection
                    cookieDictionary[key_and_value[0]] = key_and_value[1];
                }

                return cookieDictionary;
            }
        }

        /// <summary>
        /// returns ProcessStatus.Complete to indicate that the request method
        ///    was successfully found and that the caller should proceed to the next phase
        /// returns ProcessStatus.Error if the request method either couldn't be extracted
        ///    or wasn't valid
        /// </summary>
        /// <param name="line">
        /// A <see cref="string"/>
        /// </param>
        /// <returns>
        /// A <see cref="ProcessStatus"/>
        /// </returns>
        protected override ProcessStatus ProcessRequestResponseFirstLineHandler(string line)
        {
            Log.Debug("line '{0}'", line);

            // format like: GET /wiki/Basic_access_authentication HTTP/1.1
            string[] tokens = line.Split(' ');

            // do we have the correct number of tokens? if not
            // report the error to the higher level code
            int expectedTokensLength = 3;
            if (tokens.Length != expectedTokensLength)
            {
                Log.Debug("tokens.Length {0} != expected number of {1}",
                          tokens.Length, expectedTokensLength);
                return ProcessStatus.Error;
            }

            // is the first token a valid method?
            if (StringToMethod(tokens[0], out Method))
            {
                // store the url and version as well
                Url = tokens[1];
                if (!StringToHttpVersion(tokens[2], out HttpVersion))
                {
                    string errorString = string.Format("unable to convert {0} to an http version", tokens[2]);
                    Log.Error(errorString);
                    return ProcessStatus.Error;
                }

                Log.Debug("returning ProcessStatus.Complete");
                return ProcessStatus.Complete;
            }
            else
            {
                // not a warn because its ok to find unrecognized methods if we
                // are incorrectly parsing a non-http stream as if it were http
                Log.Information("Unrecognized method of '{0}' parsed out of line '{1}'",
                               tokens[0], line);
            }

            return ProcessStatus.Error;
        }

        /// <summary>
        /// Covert the text of a method into its enum value
        /// </summary>
        /// <param name="token">
        /// A <see cref="string"/>
        /// </param>
        /// <param name="method">
        /// A <see cref="Methods"/>
        /// </param>
        /// <returns>
        /// A <see cref="bool"/>
        /// </returns>
        public static bool StringToMethod(string token,
                                          out Methods method)
        {
            method = Methods.Unknown;

            if (token == "HEAD")
                method = Methods.Head;
            else if (token == "GET")
                method = Methods.Get;
            else if (token == "POST")
                method = Methods.Post;
            else if (token == "PUT")
                method = Methods.Put;
            else if (token == "DELETE")
                method = Methods.Delete;
            else if (token == "TRACE")
                method = Methods.Trace;
            else if (token == "OPTIONS")
                method = Methods.Options;
            else if (token == "CONNECT")
                method = Methods.Connect;
            else
                return false; // no match, return false

            return true;
        }

        /// <summary>
        /// ToString override
        /// </summary>
        /// <returns>
        /// A <see cref="string"/>
        /// </returns>
        public override string ToString()
        {
            return string.Format("Method: {0}, Url: {1}, {2}",
                                 Method, Url, base.ToString());
        }
    }
}
