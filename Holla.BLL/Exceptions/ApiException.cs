using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Holla.BLL.Exceptions
{
    public class ApiException : Exception
    {
        public List<string> Errors { get; }
        public ApiException() : base() { }

        public ApiException(string message) : base(message) { }

        public ApiException(string message, params object[] args)
            : base(String.Format(CultureInfo.CurrentCulture, message, args))
        {
        }

        public ApiException(IEnumerable<IdentityError> failures)
            : this()
        {
            foreach (var failure in failures)
            {
                Errors.Add(failure.Description);
            }
        }
    }
}
