﻿using System;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;
using GameOfRevenge.Common.Services;
using GameOfRevenge.Common;
using GameOfRevenge.Business.Manager.Base;
using GameOfRevenge.Common.Net;
using GameOfRevenge.Common.Interface;
using GameOfRevenge.Common.Models.Chat;

namespace GameOfRevenge.Business.Manager.Kingdom
{
    public class ChatManager : BaseManager, IChatManager
    {
        public async Task<Response<ChatMessageTable>> CreateMessage(int playerId, string content)
        {
            try
            {
                var spParams = new Dictionary<string, object>()
                {
                    { "PlayerId", playerId },
                    { "Content", content }
                };

                return await Db.ExecuteSPSingleRow<ChatMessageTable>("CreateChatMessage", spParams);
            }
            catch (InvalidModelExecption ex)
            {
                return new Response<ChatMessageTable>()
                {
                    Case = 200,
                    Data = null,
                    Message = ex.Message
                };
            }
            catch (Exception ex)
            {
                return new Response<ChatMessageTable>()
                {
                    Case = 0,
                    Data = null,
                    Message = ErrorManager.ShowError(ex)
                };
            }
        }

        public async Task<Response<ChatMessages>> GetMessages(long chatId = 0)//, int length)
        {
            try
            {
                var spParams = new Dictionary<string, object>();
                if (chatId > 0) spParams.Add("ChatId", chatId);
//                    { "Length", length }

                var msgResp = await Db.ExecuteSPMultipleRow<ChatMessageTable>("GetChatMessages", spParams);

                var result = new ChatMessages();
                if (msgResp.IsSuccess && msgResp.HasData && (msgResp.Data.Count > 0))
                {
                    var msgs = msgResp.Data;
                    var len = msgs.Count - 1;
                    var players = new Dictionary<int, PlayerBase>();
                    var messages = new List<ChatMessage>();
                    for (int num = len; num >= 0; num--)
                    {
                        var msg = msgs[num];
                        if (!players.ContainsKey(msg.PlayerId))
                        {
                            players.Add(msg.PlayerId, new PlayerBase()
                            {
                                PlayerId = msg.PlayerId,
                                Username = msg.Username,
                                VIPPoints = msg.VIPPoints
                            });
                        }
                        messages.Add(new ChatMessage()
                        {
                            ChatId = msg.ChatId,
                            PlayerId = msg.PlayerId,
                            Date = msg.Date,
                            Content = msg.Content
                        });
                    }

                    result.Players = players.Values.ToList();
                    result.Messages = messages;
                }

                return new Response<ChatMessages>()
                {
                    Case = msgResp.Case,
                    Data = result,
                    Message = msgResp.Message
                };
            }
            catch (Exception ex)
            {
                return new Response<ChatMessages>()
                {
                    Case = 0,
                    Data = null,
                    Message = ErrorManager.ShowError(ex)
                };
            }
        }
    }
}