using System;
using System.Collections.Generic;
using GameOfRevenge.Common.Models;

namespace GameOfRevenge.WebAdmin.Models
{
    public class InputResourceModel
    {
        public int PlayerId { get; set; }
        public string ResourceType { get; set; }
        public string ResourceValue { get; set; }

        public long Value { get; set; }

        public InputResourceModel()
        {
        }
    }
}
