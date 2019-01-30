using System.Net;

namespace Roulette.Services.Responses
{
    public class Response
    {
        public int StatusCode { get; private set; }
        public bool Success { get; private set; }
        public string[] ErrorMessages { get; private set; }

        public void SetSuccess() => Success = true;
        public void SetErrorMessages(params string[] errorMessage) => ErrorMessages = errorMessage;

        public void SetStatusCode(HttpStatusCode statusCode) => StatusCode = (int)statusCode;

        public void SetInternalServerErrorStatusCode() => StatusCode = (int)HttpStatusCode.InternalServerError;
        public void SetUnauthorizedStatusCode() => StatusCode = (int)HttpStatusCode.Unauthorized;
        public void SetBadRequestStatusCode() => StatusCode = (int)HttpStatusCode.BadRequest;
        public void SetNotFountStatusCode() => StatusCode = (int)HttpStatusCode.NotFound;
        public void SetConflictStatusCode() => StatusCode = (int)HttpStatusCode.Conflict;
        public void SetAcceptedStatusCode() => StatusCode = (int)HttpStatusCode.Accepted;
        public void SetOkStatusCode() => StatusCode = (int)HttpStatusCode.OK;
    }

    public class Response<TDto> : Response
    {
        public TDto Model { get; private set; }

        public void SetModel(TDto model) => Model = model;
    }
}
