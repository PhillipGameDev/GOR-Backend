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
    }
}
