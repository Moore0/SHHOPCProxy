using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SHH.OPCProxy.Comm.Model
{
    /// <summary>
    /// OPC质量状态
    /// </summary>
    public enum SHHOPCQualityStatus
    {
        OPCQualityBad = 0,
        OPCStatusConfigError = 4,
        OPCStatusNotConnected = 8,
        OPCStatusDeviceFailure = 12,
        OPCStatusSensorFailure = 16,
        OPCStatusLastKnown = 20,
        OPCStatusCommFailure = 24,
        OPCStatusOutOfService = 28,
        OPCQualityUncertain = 64,
        OPCStatusLastUsable = 68,
        OPCStatusSensorCal = 80,
        OPCStatusEGUExceeded = 84,
        OPCStatusSubNormal = 88,
        OPCQualityGood = 192,
        OPCQualityMask = 192,
        OPCStatusLocalOverride = 216,
        OPCStatusMask = 252
    }
}
