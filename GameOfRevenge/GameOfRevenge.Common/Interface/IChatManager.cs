using System.Collections.Generic;
using System.Threading.Tasks;
using GameOfRevenge.Common.Models.Chat;
using GameOfRevenge.Common.Net;

namespace GameOfRevenge.Common.Interface
{
    public interface IChatManager
    {
        Task<Response<ChatMessageTable>> CreateMessage(int playerId, string content);
        Task<Response<ChatMessages>> GetMessages(long chatId = 0);//, int length);
        Task<Response<ChatMessageFlagTable>> DeleteMessage(int playerId, long chatId);
        Task<Response> ReportMessage(int playerId, long chatId, byte reportType);
        Task<Response> BlockPlayer(int playerId, int blockPlayerId);
        Task<Response<List<int>>> GetBlockedPlayers(int playerId);
    }
}
