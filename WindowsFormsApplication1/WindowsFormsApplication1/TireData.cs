using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMS2015ModManager
{
    //Class to hold and manage tire data
    class TireData
    {
        //Class data
        private string name;
        private float gripMod;
        private int price;

        //--------------------
        //Class methods

        // Constructor
        public TireData()
        {
            //Need to init the data
            name = "";
            gripMod = 0;
            price = 0;
        }

        //Reset the contents
        public void Reset()
        {
            //Need to reset the data
            name = "";
            gripMod = 0;
            price = 0;
        }

        #region Getters and Setters

        public string _Name
        {
            get { return name; }
            set { name = value; }
        }

        public float _gripMod
        {
            get { return gripMod; }
            set { gripMod = value; }
        }

        public int _price
        {
            get { return price; }
            set { price = value; }
        }

        #endregion
    }
}
