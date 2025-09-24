using Domain.Models;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Users.Commands.LockUser
{
    public class UserLockCommandHandler :
        IRequestHandler<LockUserCommand, bool>,
        IRequestHandler<UnlockUserCommand, bool>
    {
        private readonly UserManager<User> _userManager;

        public UserLockCommandHandler(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        public async Task<bool> Handle(LockUserCommand request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(request.UserId);
            if (user == null) return false;

            user.LockoutEnd = DateTimeOffset.UtcNow.AddYears(100);
            var result = await _userManager.UpdateAsync(user);
            return result.Succeeded;
        }

        public async Task<bool> Handle(UnlockUserCommand request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(request.UserId);
            if (user == null) return false;

            user.LockoutEnd = DateTimeOffset.UtcNow;
            var result = await _userManager.UpdateAsync(user);
            return result.Succeeded;
        }
    }
}
