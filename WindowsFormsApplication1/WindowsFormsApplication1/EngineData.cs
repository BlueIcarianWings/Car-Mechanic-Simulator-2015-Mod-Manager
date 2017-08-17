using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApplication1
{
    class EngineData
    {
        //Struct to hold engine data
        public struct EngineDataType
        {
            public string name;
            public int maxPower;
            public int maxPowerRPM;
            public int maxTorqueRPM;
            public int minRPM;
            public int maxRPM;
        };
        //Array of engines
        private EngineDataType[] EnginesList;
    }
}
