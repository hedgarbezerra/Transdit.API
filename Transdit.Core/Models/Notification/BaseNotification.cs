﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Transdit.Core.Models.Notification
{
    public abstract class BaseNotification
    {
        public string Message { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
    }
}
