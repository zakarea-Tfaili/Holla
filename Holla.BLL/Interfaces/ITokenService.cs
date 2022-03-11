using System.Threading.Tasks;
using Holla.DAL.Entities;

namespace Holla.BLL.Interfaces
{
    public interface ITokenService
    {
        Task<string> CreateToken(AppUser user);
    }
}