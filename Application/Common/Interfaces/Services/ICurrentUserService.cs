using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common.Interfaces.Services
{
    namespace Application.Common.Interfaces.Services
    {
        public interface ICurrentUserService
        {
            string? UserId { get; }

            string? Email { get; }

            IReadOnlyList<string> Roles { get; }

            bool IsInRole(string roleName);

            bool IsAuthenticated { get; }
        }
    }
}
