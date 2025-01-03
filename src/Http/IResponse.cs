using System.Collections.Generic;
using System.Net;

namespace SubDBSharp.Http
{
    /// <summary>
    /// Represents a response received from an HTTP request.
    /// </summary>
    public interface IResponse
    {
        /// <summary>
        /// Gets the content of the response body.
        /// </summary>
        /// <remarks>
        /// The body contains the data returned by the HTTP request, which may be in various formats such as plain text,
        /// JSON object, or binary data, depending on the nature of the response.
        /// This property is represented as an object and may require casting to the expected type.
        /// </remarks>
        object Body { get; }

        /// <summary>
        /// Gets the collection of HTTP headers included in the response as key-value pairs.
        /// </summary>
        /// <remarks>
        /// The headers provide metadata about the response, such as content type, server, and other relevant information.
        /// This property is represented as a read-only dictionary where the keys are the header names
        /// and the values are the corresponding header values.
        /// </remarks>
        IReadOnlyDictionary<string, string> Headers { get; }

        /// <summary>
        /// Gets the HTTP status code associated with the response.
        /// </summary>
        /// <remarks>
        /// The status code indicates the outcome of the HTTP request, such as success, client error, or server error.
        /// It conforms to the standardized status codes as defined in the <see cref="System.Net.HttpStatusCode"/> enumeration.
        /// </remarks>
        HttpStatusCode StatusCode { get; }
    }
}