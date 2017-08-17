using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMS2015ModManager
{
    //Class to hold and manage map data
    class MapData
    {
        //Class data
        private string name;
        private string credits;
        private string type;
        private string file;
        private string folder;
        private string preview;
        private string description;

        //--------------------
        //Class methods

        // Constructor
        public MapData()
        {
            //Need to init the data
            Reset();
        }

        //Reset the contents
        public void Reset()
        {
            //Need to reset the data
            name = "";
            credits = "";
            type = "";
            file = "";
            folder = "";
            preview = "";
            description = "";
        }

        #region Getters and Setters

        public string _Name
        {
            get { return name; }
            set { name = value; }
        }

        public string _Credits
        {
            get { return credits; }
            set { credits = value; }
        }

        public string _Type
        {
            get { return type; }
            set { type = value; }
        }

        public string _File
        {
            get { return file; }
            set { file = value; }
        }

        public string _Folder
        {
            get { return folder; }
            set { folder = value; }
        }

        public string _Preview
        {
            get { return preview; }
            set { preview = value; }
        }

        public string _Description
        {
            get { return description; }
            set { description = value; }
        }
        #endregion
    }
}
