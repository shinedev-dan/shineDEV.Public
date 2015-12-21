using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AccountPlugin.Models.Owin
{
    public class ConfirmationRecord
    {
        public ConfirmationRecord()
            : this(DateTimeOffset.UtcNow)
        {
        }

        public ConfirmationRecord(DateTimeOffset confirmedOn)
        {
            ConfirmedOn = confirmedOn;
        }

        public DateTimeOffset ConfirmedOn { get; private set; }
    }
}