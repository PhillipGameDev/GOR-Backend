﻿using System;
using System.Collections.Generic;
using GameOfRevenge.Common.Models;
using GameOfRevenge.Model;
using GameOfRevenge.Buildings.Interface;
using GameOfRevenge.GameHandlers;
using GameOfRevenge.Common;
using GameOfRevenge.GameApplication;
using GameOfRevenge.Business;

namespace GameOfRevenge.Buildings.Handlers
{
    public class Academy : PlayerBuildingManager, IPlayerBuildingManager
    {
        public Academy(PlayerInstance player, StructureDetails structureData, IGameBuildingManager baseBuildingManager) : base(player, structureData, baseBuildingManager)
        {
        }

        public override void HandleUpgradeTechnology(UpgradeTechnologyRequest operation)
        {
            try
            {
                log.Debug("****************************************HandleUpgradeTechnology START**********************************************");
                var task = GameService.BUserTechnologyManager.UpgradeTechnology(Player.PlayerId, (TechnologyType)operation.TechType);
                task.Wait();
                var result = task.Result;

                if (result.IsSuccess)
                {
                    var dict = new Dictionary<byte, object>();
                    var newValues = new UpgradeTechnologyResponse(ref dict)
                    {
                        TechType = operation.TechType,
                        Level = operation.TechType + 1
                    };
                    Player.SendOperation(OperationCode.UpgradeTechnology, ReturnCode.OK, dict, result.Message);
                }
                
                else Player.SendOperation(OperationCode.UpgradeTechnology, ReturnCode.Failed, null, result.Message);
                log.Debug(Newtonsoft.Json.JsonConvert.SerializeObject(result));
                log.Debug("****************************************HandleUpgradeTechnology END**********************************************");
            }
            catch (Exception ex)
            {
                log.Error("****************************************HandleUpgradeTechnology ERROR**********************************************", ex);
            }
        }
    }
}
