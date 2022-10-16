using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TravisService.Models
{
    public sealed class HandheldEvent : EventBase
    {
        public string HandheldId { get; internal set; }
    }
}
