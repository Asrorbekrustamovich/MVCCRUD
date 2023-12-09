using MVCCRUD.Models.Authorizationmodels;
using MVCCRUD.Models.Domain;

namespace MVCCRUD.Models.Service
{
    public interface IUserAuthorizationService
    {
        Task<Status>LoginAsync(LoginModel Model);
        Task<Status> RegistrationAsync(Registration model);
        Task LogoutAsync();
    }
}
