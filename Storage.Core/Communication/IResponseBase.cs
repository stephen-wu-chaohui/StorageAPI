using System;
using System.Net;

namespace Storage.Core
{
    public interface IResponseBase
    {
        HttpStatusCode HttpStatusCode { get; }
    }
}
