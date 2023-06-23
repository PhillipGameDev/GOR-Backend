using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GameOfRevenge.Business.DataLink;
using GameOfRevenge.Common;
using GameOfRevenge.Common.Models;
using GameOfRevenge.Common.Net;
using GameOfRevenge.Common.Services;

namespace GameOfRevenge.Business.Manager.Base
{
    public class BaseManager
    {
        private readonly BaseDbManager baseDbManager = new BaseDbManager();
        internal string ConnectionString
        {
            get => baseDbManager.ConnectionString;
            set => baseDbManager.ConnectionString = value;
        }
        internal IBaseDbManager Db => baseDbManager;
    }
}
