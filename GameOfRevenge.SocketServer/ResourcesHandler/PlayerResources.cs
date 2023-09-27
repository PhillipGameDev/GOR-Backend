using ExitGames.Logging;
using Newtonsoft.Json;
using GameOfRevenge.Common.Models;
using GameOfRevenge.Common.Interface.Model;
using GameOfRevenge.GameHandlers;
using GameOfRevenge.Model;

namespace GameOfRevenge.ResourcesHandler
{
    public class PlayerResources : IPlayerResources
    {
        private static readonly ILogger log = LogManager.GetCurrentClassLogger();
        private readonly PlayerInstance actor;

        public IReadOnlyResourceTable ResourceInfo { get; private set; }
        public long Value { get; set; }

        public PlayerResources(IReadOnlyResourceTable info, long value, PlayerInstance actor)
        {
            this.actor = actor;
            Value = value;
            ResourceInfo = info;
//            log.Info($"INIT player resource info {JsonConvert.SerializeObject(ResourceInfo)} value {Value}");
        }

        public bool HasAvailableRequirement(IReadOnlyDataRequirement requirment)
        {
            log.Info($"Check Resource is Avaliable resourceId {ResourceInfo.Code} value {Value} requirment {requirment.Value}");
            return Value >= requirment.Value;
        }

        public void IncrementResourceValue(int val)
        {
            Value += (long)val;
            log.Info($"Update Resources Type {ResourceInfo.Code} UpdateVal {val} totalVal {Value}");
//            var obj = new UpdateResourceResponse(ResourceInfo.Id, Value);
//            actor.SendEvent(EventCode.UpdateResource, obj);
        }
    }
}
