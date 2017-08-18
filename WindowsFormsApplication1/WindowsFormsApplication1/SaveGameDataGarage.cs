using System;
using System.IO;    //File read / write
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMS2015ModManager
{
    class SaveGameDataGarage
    {
        //Class data
        //Values
        //Garage file
        private bool FastUnbolting;
        private bool FastAssembly;
        private bool FastExamine;
        private bool Tablet;
        private bool RepairLv1;
        private bool RepairLv2;
        private bool RepairLv3;
        private bool RepairLv4;
        private bool RepairLv5;
        private bool OBDReader;
        private bool CompressionTester;
        private bool ElectricalReader;
        private bool WallA;
        private bool WallB;
        private bool FloorA;
        private bool Floor2B;
        private bool FloorStripes;
        private bool Windows;
        private bool Framework;
        private bool CarLifters;
        private bool PaintShop;
        private bool TestRoad;
        private bool ParkingAB;
        private bool ParkingC;
        private bool ParkingD;
        private bool ParkingE;
        private bool ParkingF;
        private bool ParkingG;
        private bool ParkingH;
        /*  TBD, need to go through each to find the location, and valid range
        //GarageCustom
        private int CustomFloorA;
        private int CustomFloor2B;
        private int CustomFloorStripes;
        private int CustomWindows;
        private int CustomRack;
        private int CustomCarLifters;
        */
        //Memory location (values are set in the constructor, and is the bit that will need the most maintenance)
        //Garage file
        private int FastUnbolting_MemLoc;
        private int FastAssembly_MemLoc;
        private int FastExamine_MemLoc;
        private int Tablet_MemLoc;
        private int RepairLv1_MemLoc;
        private int RepairLv2_MemLoc;
        private int RepairLv3_MemLoc;
        private int RepairLv4_MemLoc;
        private int RepairLv5_MemLoc;
        private int OBDReader_MemLoc;
        private int CompressionTester_MemLoc;
        private int ElectricalReader_MemLoc;
        private int WallA_MemLoc;
        private int WallB_MemLoc;
        private int FloorA_MemLoc;
        private int Floor2B_MemLoc;
        private int FloorStripes_MemLoc;
        private int Windows_MemLoc;
        private int Framework_MemLoc;
        private int CarLifters_MemLoc;
        private int PaintShop_MemLoc;
        private int TestRoad_MemLoc;
        private int ParkingAB_MemLoc;
        private int ParkingC_MemLoc;
        private int ParkingD_MemLoc;
        private int ParkingE_MemLoc;
        private int ParkingF_MemLoc;
        private int ParkingG_MemLoc;
        private int ParkingH_MemLoc;

        /*
//GarageCustom
private int CustomFloorA_MemLoc;
private int CustomFloor2B_MemLoc;
private int CustomFloorStripes_MemLoc;
private int CustomWindows_MemLoc;
private int CustomRack_MemLoc;
private int CustomCarLifters_MemLoc;
*/
        // Constructor
        public SaveGameDataGarage()
        {
            //Set the initial vales to default nothings
            //Garage file
            FastUnbolting = false;
            FastAssembly = false;
            FastExamine = false;
            Tablet = false;
            RepairLv1 = false;
            RepairLv2 = false;
            RepairLv3 = false;
            RepairLv4 = false;
            RepairLv5 = false;
            OBDReader = false;
            CompressionTester = false;
            ElectricalReader = false;
            WallA = false;
            WallB = false;
            FloorA = false;
            Floor2B = false;
            FloorStripes = false;
            Windows = false;
            Framework = false;
            CarLifters = false;
            PaintShop = false;
            TestRoad = false;
            ParkingAB = false;
            ParkingC = false;
            ParkingD = false;
            ParkingE = false;
            ParkingF = false;
            ParkingG = false;
            ParkingH = false;

            //Set the memory locations of the values
            //Garage file
            FastUnbolting_MemLoc = 63;//3f
            FastAssembly_MemLoc = 64;//40
            FastExamine_MemLoc = 65;//41
            Tablet_MemLoc = 66;//42
            RepairLv1_MemLoc = 73;//49
            RepairLv2_MemLoc = 74;//4a
            RepairLv3_MemLoc = 75;//4b
            RepairLv4_MemLoc = 76;//4c
            RepairLv5_MemLoc = 77;//4d
            OBDReader_MemLoc = 83;//53
            CompressionTester_MemLoc = 84;//54
            ElectricalReader_MemLoc = 85;//55
            WallA_MemLoc = 67;//43
            WallB_MemLoc = 68;//44
            FloorA_MemLoc = 69;//45
            Floor2B_MemLoc = 70;//46
            FloorStripes_MemLoc = 71;//47
            Windows_MemLoc = 72;//48
            Framework_MemLoc = 88;//58
            CarLifters_MemLoc = 89;//59
            PaintShop_MemLoc = 123;//7b
            TestRoad_MemLoc = 125;//7d
            ParkingAB_MemLoc = 131;//83
            ParkingC_MemLoc = 133;//85
            ParkingD_MemLoc = 134;//86
            ParkingE_MemLoc = 135;//87
            ParkingF_MemLoc = 136;//88
            ParkingG_MemLoc = 137;//89
            ParkingH_MemLoc = 138;//8a
        }

        //Loads a Save game Global data file into the object from the fullpath and filename given
        public bool LoadGarageSaveFile(string path)
        {
            bool RetVal = false;    //Setup the return value, default false
            string fullpath = path + "\\garage";    //Setup the full path/file name

            if (File.Exists(fullpath))  //Check if the file exists
            {
                RetVal = true;
                using (BinaryReader b = new BinaryReader(File.Open(fullpath, FileMode.Open)))
                {
                    b.BaseStream.Seek(FastUnbolting_MemLoc, 0);    //Move to location
                    FastUnbolting = b.ReadBoolean();                 //Read the value

                    b.BaseStream.Seek(FastAssembly_MemLoc, 0);    //Move to location
                    FastAssembly = b.ReadBoolean();                 //Read the value

                    b.BaseStream.Seek(FastExamine_MemLoc, 0);    //Move to location
                    FastExamine = b.ReadBoolean();                 //Read the value

                    b.BaseStream.Seek(Tablet_MemLoc, 0);    //Move to location
                    Tablet = b.ReadBoolean();                 //Read the value

                    b.BaseStream.Seek(RepairLv1_MemLoc, 0);    //Move to location
                    RepairLv1 = b.ReadBoolean();                 //Read the value

                    b.BaseStream.Seek(RepairLv2_MemLoc, 0);    //Move to location
                    RepairLv2 = b.ReadBoolean();                 //Read the value

                    b.BaseStream.Seek(RepairLv3_MemLoc, 0);    //Move to location
                    RepairLv3 = b.ReadBoolean();                 //Read the value

                    b.BaseStream.Seek(RepairLv4_MemLoc, 0);    //Move to location
                    RepairLv4 = b.ReadBoolean();                 //Read the value

                    b.BaseStream.Seek(RepairLv5_MemLoc, 0);    //Move to location
                    RepairLv5 = b.ReadBoolean();                 //Read the value

                    b.BaseStream.Seek(OBDReader_MemLoc, 0);    //Move to location
                    OBDReader = b.ReadBoolean();                 //Read the value

                    b.BaseStream.Seek(CompressionTester_MemLoc, 0);    //Move to location
                    CompressionTester = b.ReadBoolean();                 //Read the value

                    b.BaseStream.Seek(ElectricalReader_MemLoc, 0);    //Move to location
                    ElectricalReader = b.ReadBoolean();                 //Read the value

                    b.BaseStream.Seek(WallA_MemLoc, 0);    //Move to location
                    WallA = b.ReadBoolean();                 //Read the value

                    b.BaseStream.Seek(WallB_MemLoc, 0);    //Move to location
                    WallB = b.ReadBoolean();                 //Read the value

                    b.BaseStream.Seek(FloorA_MemLoc, 0);    //Move to location
                    FloorA = b.ReadBoolean();                 //Read the value

                    b.BaseStream.Seek(Floor2B_MemLoc, 0);    //Move to location
                    Floor2B = b.ReadBoolean();                 //Read the value

                    b.BaseStream.Seek(FloorStripes_MemLoc, 0);    //Move to location
                    FloorStripes = b.ReadBoolean();                 //Read the value

                    b.BaseStream.Seek(Windows_MemLoc, 0);    //Move to location
                    Windows = b.ReadBoolean();                 //Read the value

                    b.BaseStream.Seek(Framework_MemLoc, 0);    //Move to location
                    Framework = b.ReadBoolean();                 //Read the value

                    b.BaseStream.Seek(CarLifters_MemLoc, 0);    //Move to location
                    CarLifters = b.ReadBoolean();                 //Read the value

                    b.BaseStream.Seek(PaintShop_MemLoc, 0);    //Move to location
                    PaintShop = b.ReadBoolean();                 //Read the value

                    b.BaseStream.Seek(TestRoad_MemLoc, 0);    //Move to location
                    TestRoad = b.ReadBoolean();                 //Read the value

                    b.BaseStream.Seek(ParkingAB_MemLoc, 0);    //Move to location
                    ParkingAB = b.ReadBoolean();                 //Read the value

                    b.BaseStream.Seek(ParkingC_MemLoc, 0);    //Move to location
                    ParkingC = b.ReadBoolean();                 //Read the value

                    b.BaseStream.Seek(ParkingD_MemLoc, 0);    //Move to location
                    ParkingD = b.ReadBoolean();                 //Read the value

                    b.BaseStream.Seek(ParkingE_MemLoc, 0);    //Move to location
                    ParkingE = b.ReadBoolean();                 //Read the value

                    b.BaseStream.Seek(ParkingF_MemLoc, 0);    //Move to location
                    ParkingF = b.ReadBoolean();                 //Read the value

                    b.BaseStream.Seek(ParkingG_MemLoc, 0);    //Move to location
                    ParkingG = b.ReadBoolean();                 //Read the value

                    b.BaseStream.Seek(ParkingH_MemLoc, 0);    //Move to location
                    ParkingH = b.ReadBoolean();                 //Read the value
                }
            }

            return RetVal;      //Return success or failure
        }

        //Writes the Save game Global data file from the object
        public void WriteGarageSaveFile(string path)
        {
            string fullpath = path + "\\garage";
            if (File.Exists(fullpath))  //Check if the file exists
            {
                using (BinaryWriter b = new BinaryWriter(File.Open(fullpath, FileMode.Open)))
                {
                    b.BaseStream.Seek(FastUnbolting_MemLoc, 0);   //Move to location
                    b.Write(FastUnbolting);                       //Write the value

                    b.BaseStream.Seek(FastAssembly_MemLoc, 0);   //Move to location
                    b.Write(FastAssembly);                       //Write the value

                    b.BaseStream.Seek(FastExamine_MemLoc, 0);   //Move to location
                    b.Write(FastExamine);                       //Write the value

                    b.BaseStream.Seek(Tablet_MemLoc, 0);   //Move to location
                    b.Write(Tablet);                       //Write the value

                    b.BaseStream.Seek(RepairLv1_MemLoc, 0);   //Move to location
                    b.Write(RepairLv1);                       //Write the value

                    b.BaseStream.Seek(RepairLv2_MemLoc, 0);   //Move to location
                    b.Write(RepairLv2);                       //Write the value

                    b.BaseStream.Seek(RepairLv3_MemLoc, 0);   //Move to location
                    b.Write(RepairLv3);                       //Write the value

                    b.BaseStream.Seek(RepairLv4_MemLoc, 0);   //Move to location
                    b.Write(RepairLv4);                       //Write the value

                    b.BaseStream.Seek(RepairLv5_MemLoc, 0);   //Move to location
                    b.Write(RepairLv5);                       //Write the value

                    b.BaseStream.Seek(OBDReader_MemLoc, 0);   //Move to location
                    b.Write(OBDReader);                       //Write the value

                    b.BaseStream.Seek(CompressionTester_MemLoc, 0);   //Move to location
                    b.Write(CompressionTester);                       //Write the value

                    b.BaseStream.Seek(ElectricalReader_MemLoc, 0);   //Move to location
                    b.Write(ElectricalReader);                       //Write the value

                    b.BaseStream.Seek(WallA_MemLoc, 0);   //Move to location
                    b.Write(WallA);                       //Write the value

                    b.BaseStream.Seek(WallB_MemLoc, 0);   //Move to location
                    b.Write(WallB);                       //Write the value

                    b.BaseStream.Seek(FloorA_MemLoc, 0);   //Move to location
                    b.Write(FloorA);                       //Write the value

                    b.BaseStream.Seek(Floor2B_MemLoc, 0);   //Move to location
                    b.Write(Floor2B);                       //Write the value

                    b.BaseStream.Seek(FloorStripes_MemLoc, 0);   //Move to location
                    b.Write(FloorStripes);                       //Write the value

                    b.BaseStream.Seek(Windows_MemLoc, 0);   //Move to location
                    b.Write(Windows);                       //Write the value

                    b.BaseStream.Seek(Framework_MemLoc, 0);   //Move to location
                    b.Write(Framework);                       //Write the value

                    b.BaseStream.Seek(CarLifters_MemLoc, 0);   //Move to location
                    b.Write(CarLifters);                       //Write the value

                    b.BaseStream.Seek(PaintShop_MemLoc, 0);   //Move to location
                    b.Write(PaintShop);                       //Write the value

                    b.BaseStream.Seek(TestRoad_MemLoc, 0);   //Move to location
                    b.Write(TestRoad);                       //Write the value

                    b.BaseStream.Seek(ParkingAB_MemLoc, 0);   //Move to location
                    b.Write(ParkingAB);                       //Write the value

                    b.BaseStream.Seek(ParkingC_MemLoc, 0);   //Move to location
                    b.Write(ParkingC);                       //Write the value

                    b.BaseStream.Seek(ParkingD_MemLoc, 0);   //Move to location
                    b.Write(ParkingD);                       //Write the value

                    b.BaseStream.Seek(ParkingE_MemLoc, 0);   //Move to location
                    b.Write(ParkingE);                       //Write the value

                    b.BaseStream.Seek(ParkingF_MemLoc, 0);   //Move to location
                    b.Write(ParkingF);                       //Write the value

                    b.BaseStream.Seek(ParkingG_MemLoc, 0);   //Move to location
                    b.Write(ParkingG);                       //Write the value

                    b.BaseStream.Seek(ParkingH_MemLoc, 0);   //Move to location
                    b.Write(ParkingH);                       //Write the value
                }
            }
        }

        #region Getters and Setters
        public bool _FastUnbolting  //CTRL + R + E
        {
            get { return FastUnbolting; }
            set { FastUnbolting = value; }
        }

        public bool _FastAssembly
        {
            get {  return FastAssembly; }
            set { FastAssembly = value; }
        }

        public bool _FastExamine
        {
            get { return FastExamine;  }
            set { FastExamine = value; }
        }

        public bool _Tablet
        {
            get { return Tablet; }
            set { Tablet = value; }
        }

        public bool _RepairLv1
        {
            get { return RepairLv1; }
            set { RepairLv1 = value; }
        }

        public bool _RepairLv2
        {
            get { return RepairLv2; }
            set  { RepairLv2 = value; }
        }

        public bool _RepairLv3
        {
            get { return RepairLv3; }
            set { RepairLv3 = value; }
        }

        public bool _RepairLv4
        {
            get { return RepairLv4; }
            set { RepairLv4 = value; }
        }

        public bool _RepairLv5
        {
            get { return RepairLv5; }
            set { RepairLv5 = value; }
        }

        public bool _OBDReader
        {
            get { return OBDReader; }
            set { OBDReader = value; }
        }

        public bool _CompressionTester
        {
            get { return CompressionTester; }
            set { CompressionTester = value; }
        }

        public bool _ElectricalReader
        {
            get { return ElectricalReader; }
            set { ElectricalReader = value; }
        }

        public bool _WallA
        {
            get { return WallA; }
            set { WallA = value; }
        }

        public bool _WallB
        {
            get { return WallB; }
            set { WallB = value; }
        }

        public bool _FloorA
        {
            get  { return FloorA; }
            set { FloorA = value; }
        }

        public bool _Floor2B
        {
            get { return Floor2B; }
            set { Floor2B = value; }
        }

        public bool _FloorStripes
        {
            get { return FloorStripes; }
            set { FloorStripes = value; }
        }

        public bool _Windows
        {
            get { return Windows; }
            set { Windows = value; }
        }

        public bool _Framework
        {
            get { return Framework; }
            set { Framework = value; }
        }

        public bool _CarLifters
        {
            get { return CarLifters; }
            set { CarLifters = value; }
        }

        public bool _PaintShop
        {
            get { return PaintShop; }
            set { PaintShop = value; }
        }

        public bool _TestRoad
        {
            get { return TestRoad; }
            set { TestRoad = value; }
        }

        public bool _ParkingAB
        {
            get { return ParkingAB; }
            set  { ParkingAB = value; }
        }

        public bool _ParkingC
        {
            get { return ParkingC; }
            set { ParkingC = value; }
        }

        public bool _ParkingD
        {
            get { return ParkingD; }
            set { ParkingD = value; }
        }

        public bool _ParkingE
        {
            get { return ParkingE; }
            set { ParkingE = value; }
        }

        public bool _ParkingF
        {
            get { return ParkingF; }
            set { ParkingF = value; }
        }

        public bool _ParkingG
        {
            get { return ParkingG; }
            set { ParkingG = value; }
        }

        public bool _ParkingH
        {
            get { return ParkingH; }
            set { ParkingH = value; }
        }
        #endregion
    }
}
