using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Max.Api.Helpers
{
    public class ApiOkResponse : ApiResponse
    {
        public object Result { get; }

        public ApiOkResponse(object result)
            : base(200)
        {
            Result = result;
        }
    }
}
