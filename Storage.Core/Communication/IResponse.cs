using System;
using System.Net;

namespace Storage.Core
{
    public interface IResponse
    {
        ServiceStatusCode StatusCode { get; }
        HttpStatusCode HttpStatusCode { get; }
    }
}
