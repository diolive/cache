﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DioLive.Cache.WebUI.Models.ManageViewModels
{
    public class RemoveLoginVM
    {
        public string LoginProvider { get; set; }
        public string ProviderKey { get; set; }
    }
}
