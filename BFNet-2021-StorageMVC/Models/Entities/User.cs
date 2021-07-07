using Microsoft.Azure.Cosmos.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BFNet_2021_StorageMVC.Models.Entities
{
    public class User: TableEntity
    {
        public string Username { get; set; }
        public string Password { get; set; }

        public string IconPath { get; set; }
    }
}
