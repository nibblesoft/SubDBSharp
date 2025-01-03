using System;
using System.Net.Http;

namespace SubDBSharp.Models
{
    /// <summary>
    /// Represents an HTTP request with a specified endpoint, method, and optional body content.
    /// </summary>
    public interface IRequest
    {
        /// <summary>
        /// Gets or sets the HTTP content to be sent as the body of an HTTP request.
        /// </summary>
        /// <remarks>
        /// Typically used to include serialized data or other payloads in HTTP requests such as POST or PUT.
        /// </remarks>
        HttpContent Body { get; }

        /// <summary>
        /// Gets or sets the URI that represents the target endpoint of the HTTP request.
        /// </summary>
        /// <remarks>
        /// Used to specify the destination URL for the HTTP operation being executed.
        /// </remarks>
        Uri EndPoint { get; }

        /// <summary>
        /// Gets or sets the HTTP method to be used for the request.
        /// </summary>
        /// <remarks>
        /// Specifies the action to be performed by the request, such as GET, POST, PUT, or DELETE.
        /// </remarks>
        HttpMethod Method { get; }
    }
}