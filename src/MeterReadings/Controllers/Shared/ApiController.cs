using MeterReadings.Shared;
using MeterReadings.Shared.Exceptions;
using Microsoft.AspNetCore.Mvc;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace MeterReadings.Controllers
{
    /// <summary>
    /// Extend this class to indicate that the extending class is an API controller.
    /// </summary>
    [ApiExceptionFilter]
    public abstract class ApiController : Controller
    {
        /// <summary>
        /// Gets the page request from the HTTP request.
        /// </summary>
        /// <returns>The page request as an instance of <see cref="PageRequest"/>.</returns>
        protected PageRequest GetPageRequest()
        {
            if (!int.TryParse(Request.Query["page"].ToString(), out int page)) throw new MissingPagingInformationException();
            if (!int.TryParse(Request.Query["page_size"].ToString(), out int pageSize)) throw new MissingPagingInformationException();

            return new PageRequest(page, pageSize);
        }

        /// <summary>
        /// Returns a success response with the specified data.
        /// </summary>
        /// <typeparam name="TPayloadType"></typeparam>
        /// <param name="payload"></param>
        /// <returns></returns>
        public ActionResult<ApiResponse> Success<TPayloadType>(TPayloadType payload)
            where TPayloadType : class => Json(new SuccessfulApiResponse<TPayloadType>(payload));
    }

    /// <summary>
    /// A standardised schema for responding to API requests.
    /// </summary>
    public abstract class ApiResponse
    {
        /// <summary>
        /// Initialise a new instance of <see cref="ApiResponse"/>.
        /// </summary>
        /// <param name="success">A <see cref="bool"/> value indicating whether or not the request was successful.</param>
        protected ApiResponse(bool success)
        {
            Success = success;
        }

        /// <summary>
        /// Gets or sets a <see cref="bool"/> value indicating whether or not the request was successful.
        /// </summary>
        [JsonPropertyName("success")]
        public bool Success { get; }
    }

    /// <summary>
    /// A standardised schema for responding to successful API requests.
    /// </summary>
    /// <param name="success">A <see cref="bool"/> value indicating whether or not the request was successful.</param>
    public class SuccessfulApiResponse : ApiResponse
    {
        /// <summary>
        /// Initialise a new instance of <see cref="SuccessfulApiResponse"/>.
        /// </summary>
        public SuccessfulApiResponse(bool success)
            : base(true) { }
    }

    /// <summary>
    /// A standardised schema for responding to successful API requests.
    /// </summary>
    /// <typeparam name="TPayloadType">The type of payload.</typeparam>
    public class SuccessfulApiResponse<TPayloadType> : SuccessfulApiResponse
        where TPayloadType : class
    {
        /// <summary>
        /// Initialise a new instance of <see cref="SuccessfulApiResponse{TPayloadType}"/>.
        /// </summary>
        /// <param name="payload">The response payload data as an instance of <typeparamref name="TPayloadType"/>.</param>
        public SuccessfulApiResponse(TPayloadType payload)
            : base(true)
        {
            Payload = payload ?? throw new ArgumentNullException(nameof(payload));
        }

        /// <summary>
        /// Gets the response payload data as an instance of <typeparamref name="TPayloadType"/>.
        /// </summary>
        [JsonPropertyName("payload")]
        public TPayloadType Payload { get; }
    }

    /// <summary>
    /// A standardised schema for responding to unsuccessful API requests.
    /// </summary>
    public class UnsuccessfulApiResponse : ApiResponse
    {
        /// <summary>
        /// Initialise a new instance of <see cref="UnsuccessfulApiResponse"/>.
        /// </summary>
        public UnsuccessfulApiResponse()
            : base(false)
        {
            ErrorMessage = GetErrorResponse();
        }

        /// <summary>
        /// Initialise a new instance of <see cref="UnsuccessfulApiResponse"/>.
        /// </summary>
        /// <param name="exception">The exception that was thrown.</param>
        public UnsuccessfulApiResponse(Exception exception)
            : base(false)
        {
            Exception = exception;

            ErrorMessage = GetErrorResponse();
        }

        /// <summary>
        /// Gets the exception that was thrown.
        /// </summary>
        [IgnoreDataMember]
        protected Exception Exception { get; }

        /// <summary>
        /// Gets the error message.
        /// </summary>
        [JsonPropertyName("error")]
        public string ErrorMessage { get; }

        /// <summary>
        /// Gets the error response to be included in the API response.
        /// </summary>
        /// <returns>The error response to be included in the API response.</returns>
        protected virtual string GetErrorResponse()
        {
            switch (Exception)
            {
                case MeterReadingsException:
                    return Exception.Message;
                default:
                    return "Unknown error";
            }
        }
    }
}