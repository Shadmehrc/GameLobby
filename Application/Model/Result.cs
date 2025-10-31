using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Model
{
    public enum ErrorCode
    {
        None,
        BadRequest,
        NotFound,
        Full,
        NotOpen,
        Locked,
        Unknown
    }

    public sealed class Result<T>
    {
        public bool IsSuccess { get; init; }
        public T? Value { get; init; }
        public string Message { get; init; } = "";
        public ErrorCode Code { get; init; } = ErrorCode.None;

        public static Result<T> Ok(T value, string? msg = null)
            => new() { IsSuccess = true, Value = value, Message = msg ?? "" };

        public static Result<T> Fail(string msg, ErrorCode code = ErrorCode.Unknown, T? value = default)
            => new() { IsSuccess = false, Value = value, Message = msg, Code = code };
    }
}
