using DTC.Domain.Entities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;

namespace DTC.Tests.Fake
{
    public class FakeSignInManager : SignInManager<User>
    {
        public FakeSignInManager()
            : base(
                  new Mock<FakeUserManager>().Object,
                  new HttpContextAccessor(),
                  new Mock<IUserClaimsPrincipalFactory<User>>().Object,
                  new Mock<Microsoft.Extensions.Options.IOptions<IdentityOptions>>().Object,
                  new Mock<ILogger<SignInManager<User>>>().Object,
                  new Mock<Microsoft.AspNetCore.Authentication.IAuthenticationSchemeProvider>().Object                  )
        { }
    }
}
