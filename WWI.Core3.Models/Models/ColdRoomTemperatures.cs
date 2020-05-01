﻿using System;
using System.Collections.Generic;

namespace WWI.Core3.Models.Models
{
    public partial class ColdRoomTemperatures
    {
        public long ColdRoomTemperatureId { get; set; }
        public int ColdRoomSensorNumber { get; set; }
        public DateTime RecordedWhen { get; set; }
        public decimal Temperature { get; set; }
        public DateTime ValidFrom { get; set; }
        public DateTime ValidTo { get; set; }
    }
}
