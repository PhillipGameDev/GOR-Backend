using System.Collections.Generic;
using System.Threading.Tasks;
using GameOfRevenge.Common.Models.Chat;
using GameOfRevenge.Common.Net;

namespace GameOfRevenge.Common.Interface
{
    public interface IChatManager
    {
        Task<Response<ChatMessageTable>> CreateMessage(int playerId, string content, int allianceId = 0);
        Task<Response<ChatMessages>> GetMessages(long chatId = 0, int alianceId = 0);
        Task<Response<ChatMessageFlagTable>> DeleteMessage(int playerId, long chatId, int allianceId = 0);
        Task<Response> ReportMessage(int playerId, long chatId, byte reportType, int allianceId = 0);
        Task<Response> BlockPlayer(int playerId, int blockPlayerId);
        Task<Response<List<int>>> GetBlockedPlayers(int playerId);
    }
}
