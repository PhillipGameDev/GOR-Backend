using GameOfRevenge.Common.Net;
using System.Threading.Tasks;

namespace GameOfRevenge.Common.Interface.UserData
{
    public interface IUserClanDataManager
    {
        Task<Response> CreateClan(int playerId, string clanName);
        Task<Response> DeleteClan(int playerId, string clanName);
        Task<Response> InviteMember(int playerId, int inviteId);
        Task<Response> AcceptMember(int playerId, int inviteId);
        Task<Response> RejectMember(int playerId, int inviteId);
        Task<Response> RequestMember(int playerId, int clanId);
    }
}
