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
        private readonly MmoActor actor;

        public IReadOnlyResourceTable ResourceInfo { get; private set; }
        public float Value { get; set; }

        public PlayerResources(IReadOnlyResourceTable info, float value, MmoActor actor)
        {
            this.actor = actor;
            Value = value;
            ResourceInfo = info;
            log.Info($"INIT player resource info {JsonConvert.SerializeObject(ResourceInfo)} value {Value}");
        }

        public bool HasAvailableRequirment(IReadOnlyDataRequirement requirment)
        {
            log.Info($"Check Resource is Avaliable resourceId {ResourceInfo.Code} value {Value} requirment {requirment.Value}");
            return Value >= requirment.Value;
        }

        public void UpdateResourceValue(float val)
        {
            Value += val;
            log.Info($"Update Resources Type {ResourceInfo.Code} UpdateVal {val} totalVal {Value}");
            var obj = new UpdateResourceResponse(ResourceInfo.Id, Value);
            actor.SendEvent(EventCode.UpdateResource, obj);
        }
    }
}
