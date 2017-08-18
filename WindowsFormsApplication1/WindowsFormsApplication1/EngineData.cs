using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMS2015ModManager
{
    //Class to hold and manage engine data
    class EngineData
    {
        //Class data
        private string name;
        private string engineSound;
        private bool BlockOBD;
        private int maxPower;
        private int maxPowerRPM;
        private int maxTorqueRPM;
        private int minRPM;
        private int maxRPM;

        //--------------------
        //Class methods

        // Constructor
        public EngineData()
        {
            //Need to init the data
            name = "";
            engineSound = "";
            BlockOBD = false;
            maxPower = 0;
            maxPowerRPM = 0;
            maxTorqueRPM = 0;
            minRPM = 0;
            maxRPM = 0;
        }

        //Reset the contents
        public void Reset()
        {
            //Need to reset the data
            name = "";
            engineSound = "";
            BlockOBD = false;
            maxPower = 0;
            maxPowerRPM = 0;
            maxTorqueRPM = 0;
            minRPM = 0;
            maxRPM = 0;
        }

        #region Getters and Setters

        public string _Name
        {
            get { return name; }
            set { name = value; }
        }

        public bool _BlockOBD
        {
            get { return BlockOBD; }
            set { BlockOBD = value; }
        }

        public string _EngineSound
        {
            get { return engineSound; }
            set { engineSound = value; }
        }

        public int _maxPower
        {
            get { return maxPower; }
            set { maxPower = value; }
        }

        public int _maxPowerRPM
        {
            get { return maxPowerRPM; }
            set { maxPowerRPM = value; }
        }

        public int _maxTorqueRPM
        {
            get { return maxTorqueRPM; }
            set { maxTorqueRPM = value; }
        }

        public int _minRPM
        {
            get { return minRPM; }
            set { minRPM = value; }
        }

        public int _maxRPM
        {
            get { return maxRPM; }
            set { maxRPM = value; }
        }

        #endregion
    }
}
