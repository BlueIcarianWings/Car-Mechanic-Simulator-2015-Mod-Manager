using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using System.Drawing;   //Needed for pointf type
using System.IO;    //For streamReader,

namespace CMS2015ModManager
{
    //Class to hold and manage engine data
    //Will be a big one with lots of data
    class CarData
    {
        //Class Types
        private struct DoubleFloatPoint
        {
            //vector might work for this but it's seem to designed for 3D maths instead of storage
            //or pointf
            public float a;
            public float b;
        }

        private struct TripleFloatPoint
        {
            //vector3 might work for this but it's seem to designed for 3D maths instead of storage
            //or point3d
            public float x;
            public float y;
            public float z;
        }

        //[PartsType] //0 to 6 (index)
        //As a car may have 0 to 7 of these, I'm going to use a struct in a <List> internaly
        //and have the class deal with all the processing. I'll have to use an 'all at one' setter
        private struct PartsType //0 to 6
        {
            public string PartsName;
            public TripleFloatPoint PartsPosition;
            public TripleFloatPoint PartsRotation;
            public float PartsScale;
        }

        private string ModManVersion;       //Hold the version string of this tool, shows which version a file was made with

        //----------------------------
        //Class Data
        #region Data Definitions
        //[Main]
        private string MainName;     //My addition to tie the 'in game' name to the car
        private string MainModel;
        private string MainRustMask;
        private TripleFloatPoint MainRotation;

        //[Other]
        private int OtherPower;
        private string OtherTransmissionType;
        private int OtherGears;
        private float OtherFinalDriveRatio;
        private int OtherWeight;
        private float OtherRpmFactor;
        private float OtherRpmAngle;
        private float OtherSpeedoFactor;
        private float OtherSpeedoAngle;
        private float OtherSuspTravel;
        private float OtherLifterArmsRise;
        private float OtherLifterArmsAngle;
        private float OtherDoorAngle;           //Gull wing door
        private float OtherCX;

        //[Suspension]
        private float SuspensionFrontAxleStart;
        private float SuspensionWheelBase;
        private float SuspensionHeight;
        private float SuspensionHeightRear;
        private float SuspensionFrontTrack;
        private float SuspensionRearTrack;
        private float SuspensionFrontSpringLength;
        private float SuspensionScale;
        private int   SuspensionSidesFlip;			//Kaszlak specfic at the moment
        private string SuspensionFrontCenterSet;
        private string SuspensionFrontRightSet;
        private string SuspensionFrontLeftSet;		//Kaszlak specfic at the moment
        private string SuspensionRearCenterSet;
        private string SuspensionRearRightSet;
        private string SuspensionRearLeftSet;		//Kaszlak specfic at the moment

        //[Engine]
        private TripleFloatPoint EnginePosition;
        private TripleFloatPoint EngineRotation;
        private float EngineScale;
        private string EngineType;
        private float EnginePM;
        //Declare the list of the engine swap options
        private List<string> EngineSwapOptions;

        //[Driveshaft]
        private TripleFloatPoint DriveshaftPosition;
        private TripleFloatPoint DriveshaftRotation;
        private float DriveshaftScale;
        private string DriveshaftType;
        private float DriveshaftLength;
        private float DriveshaftSize;
        private float DriveshaftPM;

        //[Wheels]
        private int WheelsWheelWidth;
        private string WheelsTire;
        private string WheelsRim;
        private string WheelsRimcap;		//Mercedes 300SL specfic at the moment
        private int WheelsRimSize;
        private int WheelsTireSize;
        //Maserati specfic at the moment
        private int WheelsWheelWidthRear;
        private int WheelsRimSizeRear;
        private int WheelsTireSizeRear;

        //[Wheels_Rear]
        private int WheelsRearWheelWidth;
        private string WheelsRearTire;
        private string WheelsRearRim;
        private string WheelsRearRimcap;		//Mercedes 300SL specfic at the moment
        private int WheelsRearRimSize;
        private int WheelsRearTireSize;

        //[PartsType] //0 to 6 (index)
        //As a car may have 0 to 7 of these, I'm going to use a struct in a <List> internaly
        //and have the class deal with all the processing. I'll have to use an 'all at one' setter

        //Declare the list of the Parts struct
        private List<PartsType> Parts;

        //[Interior]
        private TripleFloatPoint InteriorSeatLeftPos;
        private TripleFloatPoint InteriorSeatLeftRot;
        private float InteriorSeatScale;
        private float InteriorSeatHeightMod;
        private string InteriorSeat;
        private string InteriorWheel;
        private TripleFloatPoint InteriorWheelPos;
        private TripleFloatPoint InteriorWheelRot;
        private float InteriorWheelScale;

        //[Logic]
        private DoubleFloatPoint LogicGlobalCondition;
        private DoubleFloatPoint LogicPartsConditions;
        private DoubleFloatPoint LogicPanelsConditions;
        private float LogicUniqueMod;
        #endregion

        //----------------------------
        //Class methods
        public CarData(string Version)
        {
            //Need to initialize(/create) the parts list here
            Parts = new List<PartsType>();
            //Need to initialize(/create) the parts list here
            EngineSwapOptions = new List<string>();
            //Call the clear all method to reset the lot;
            CarDataClearAll();
            //Store the passed in version number
            ModManVersion = Version;
        }

        //Loads a car data file into the object from the fullpath and filename given
        public void LoadCarDataFile(string Filename)
        {
            //Check if the Car Data File exists
            if (File.Exists(Filename))
            {
                //Load the whole file
                string[] CDFlines = System.IO.File.ReadAllLines(Filename);
                //Remove old data
                CarDataClearAll();

                //Loop through the lines to process them
                for (int i = 0; i < CDFlines.Length; i++)
                {
                    //Fill out the local until we get a new new heading (starts with[)
                    // then save the local to the list and start again

                    //If this is a new definition
                    if (CDFlines[i].StartsWith("["))
                    {
                        //We have a line with new section, so ID the section
                        string line = CDFlines[i].Substring(1, CDFlines[i].Length - 2).Trim();  //Remove the leading on trailing spaces
                        
                        switch (line)
                        {
                            case "main":  //Main data section
                                CarDataMainSectionProc(CDFlines, ref i);     //Special case ;name                              
                                break;
                            case "other":
                                CarDataOtherSectionProc(CDFlines, ref i);
                                break;
                            case "suspension":
                                CarDataSuspensionSectionProc(CDFlines, ref i);
                                break;
                            case "engine":
                                CarDataEngineSectionProc(CDFlines, ref i);
                                break;
                            case "driveshaft":
                                CarDataDriveshaftSectionProc(CDFlines, ref i);
                                break;
                            case "wheels":
                                CarDataWheelsSectionProc(CDFlines, ref i);
                                break;
                            case "wheels_rear":
                                CarDataWheelsRearSectionProc(CDFlines, ref i);
                                break;
                            case "interior":
                                CarDataInteriorSectionProc(CDFlines, ref i);
                                break;
                            case "logic":
                                CarDataLogicSectionProc(CDFlines, ref i);
                                break;
                            default:
                                //parts<num> here
                                //I could have had parts0 to parts6 in the switch, easy but long winded and not future proof.
                                //If parts ever got over that number I'd need to expand it.
                                //Instead I'll further process the line here to have as many parts as can be handeled

                                //If the line starts with "parts" call the parts section processor
                                //We will lose the number part of the label, but it doesn't matter, we will just use the list index
                                if (line.StartsWith("parts"))
                                {
                                    CarDataPartsSectionProc(CDFlines, ref i);
                                }
                                break;
                        }
                    }
                    else
                    {
                        //Blank line etc
                        //Should probably never hit this, as once a [Header] is found we process it until we find the next [Header]
                    }
                }
            }
        }

        //Writes the car data file from the object
        public void WriteCarDataToFile(string Filename)
        {
            //Remove duplicate \'s from the string
            //Filename.Replace("\\\\", "\\");
            //Create a local file writer
            StreamWriter writer = new StreamWriter(Filename);

            //[Main]
            writer.WriteLine("[main]");
            if (MainName != "") { writer.WriteLine(";name= " + MainName); }
            writer.WriteLine("model= " + MainModel);
            writer.WriteLine("rustMask= " + MainRustMask);
            writer.WriteLine("rotation= {0:f1},{1:f1},{2:f1}", MainRotation.x, MainRotation.y, MainRotation.z); //apply formatting of float to 1DP
            writer.WriteLine();     //Blank line seperator

            //[Other]
            writer.WriteLine("[other]");
            if (OtherPower != 0)            { writer.WriteLine("otherPower= " + OtherPower); }
            if (OtherTransmissionType != ""){ writer.WriteLine("transmissionType= " + OtherTransmissionType); }
            if (OtherGears != 0)            { writer.WriteLine("gears= " + OtherGears); }
            if (OtherFinalDriveRatio != 0)  { writer.WriteLine("finalDriveRatio= {0:f3}", OtherFinalDriveRatio); }
            if (OtherWeight != 0)           { writer.WriteLine("weight= " + OtherWeight); }
            if (OtherRpmFactor != 0)        { writer.WriteLine("rpmFactor= {0:f2}", OtherRpmFactor); }
            if (OtherRpmAngle != 0)         { writer.WriteLine("rpmAngle= {0:f2}", OtherRpmAngle); }
            if (OtherSpeedoFactor != 0)     { writer.WriteLine("speedoFactor= {0:f2}", OtherSpeedoFactor); }
            if (OtherSpeedoAngle != 0)      { writer.WriteLine("speedoAngle= {0:f2}", OtherSpeedoAngle); }
            if (OtherSuspTravel != 0)       { writer.WriteLine("suspTravel= {0:f2}", OtherSuspTravel); }
            if (OtherLifterArmsRise != 0)   { writer.WriteLine("lifterArmsRise= {0:f2}", OtherLifterArmsRise); }
            if (OtherLifterArmsAngle != 0)  { writer.WriteLine("lifterArmsAngle= {0:f2}", OtherLifterArmsAngle); }
            if (OtherDoorAngle != 0)        { writer.WriteLine("doorAngle= " + OtherDoorAngle); }          //Gull wing door
            if (OtherCX != 0)               { writer.WriteLine("cx= {0:f3}", OtherCX); }
            writer.WriteLine();     //Blank line seperator

            //[Suspension]
            writer.WriteLine("[suspension]");
            if (SuspensionFrontAxleStart != 0)    { writer.WriteLine("frontAxleStart= " + SuspensionFrontAxleStart); }
            if (SuspensionWheelBase != 0)         { writer.WriteLine("wheelBase= " + SuspensionWheelBase); }
            if (SuspensionHeight != 0)            { writer.WriteLine("height= " + SuspensionHeight); }
            if (SuspensionHeightRear != 0)        { writer.WriteLine("heightRear= " + SuspensionHeightRear); }
            if (SuspensionFrontTrack != 0)        { writer.WriteLine("frontTrack= " + SuspensionFrontTrack); }
            if (SuspensionRearTrack != 0)         { writer.WriteLine("rearTrack= " + SuspensionRearTrack); }
            if (SuspensionFrontSpringLength != 0) { writer.WriteLine("frontSpringLength= " + SuspensionFrontSpringLength); }
            if (MainModel == "car_Kaszlak")     //Kaszlak specfic bits
            {
                if (SuspensionScale != 0) { writer.WriteLine("forceScale= " + SuspensionScale); }
                if (SuspensionSidesFlip != 0) { writer.WriteLine("sidesFlip= " + SuspensionSidesFlip); }                //Kaszlak specfic at the moment
                if (SuspensionFrontLeftSet != "") { writer.WriteLine("frontLeftSet= " + SuspensionFrontLeftSet); }      //Kaszlak specfic at the moment
                if (SuspensionRearLeftSet != "") { writer.WriteLine("rearLeftSet= " + SuspensionRearLeftSet); }         //Kaszlak specfic at the moment
            }
            else    //This will be obsolete later (post 1.0.8.0)    (not yet done 1.1.0.2)
            {
                if (SuspensionScale != 0) { writer.WriteLine("scale= " + SuspensionScale); }
            }
            if (SuspensionFrontCenterSet != "")   { writer.WriteLine("frontCenterSet= " + SuspensionFrontCenterSet); }
            if (SuspensionFrontRightSet != "")    { writer.WriteLine("frontRightSet= " + SuspensionFrontRightSet); }
            if (SuspensionRearCenterSet != "")    { writer.WriteLine("rearCenterSet= " + SuspensionRearCenterSet); }
            if (SuspensionRearRightSet != "")     { writer.WriteLine("rearRightSet= " + SuspensionRearRightSet); }            
            writer.WriteLine();     //Blank line seperator

            //[Engine]
            writer.WriteLine("[engine]");
            writer.WriteLine("position= {0:f1},{1:f1},{2:f1}", EnginePosition.x, EnginePosition.y, EnginePosition.z);   //Always write the position
            writer.WriteLine("rotation= {0:f1},{1:f1},{2:f1}", EngineRotation.x, EngineRotation.y, EngineRotation.z);   //Always write the rotation
            if (EngineScale != 0)  { writer.WriteLine("scale= {0:f2}", EngineScale); }
            if (EngineType != "")  { writer.WriteLine("type= {0:f2}", EngineType); }
            if (EnginePM != 0)     { writer.WriteLine("pm= {0:f2}", EnginePM); }
            //Declare the list of the engine swap options
            if (EngineSwapOptions.Count > 1)
            {
                bool Commatoggle = false;   //Need to control the comma between engines
                string swapops = "";

                foreach (string line in EngineSwapOptions)
                {
                    if (Commatoggle)
                    {
                        swapops += "," + line;
                    }
                    else
                    {
                        swapops= line;     //First engine option doesn't need a toggle
                        Commatoggle = true;         //Toggle the toggle
                    }
                }
                writer.WriteLine("swapoptions= "+ swapops);
            }
            writer.WriteLine();     //Blank line seperator

            //[Driveshaft]
            if (DriveshaftType != "")     //Skip all this if there is no driveshaft
            {
                writer.WriteLine("[driveshaft]");
                if ((DriveshaftPosition.x != 0) || (DriveshaftPosition.y != 0) || (DriveshaftPosition.z != 0))
                { writer.WriteLine("position= " + DriveshaftPosition.x + ", " + DriveshaftPosition.y + ", " + DriveshaftPosition.z); }
                if ((DriveshaftRotation.x != 0) || (DriveshaftRotation.y != 0) || (DriveshaftRotation.z != 0))
                { writer.WriteLine("rotation= " + DriveshaftRotation.x + ", " + DriveshaftRotation.y + ", " + DriveshaftRotation.z); }
                if (DriveshaftScale != 0) { writer.WriteLine("scale= " + DriveshaftScale); }
                if (DriveshaftType != "") { writer.WriteLine("type= " + DriveshaftType); }
                if (DriveshaftLength != 0) { writer.WriteLine("length= " + DriveshaftLength); }
                if (DriveshaftSize != 0) { writer.WriteLine("size= " + DriveshaftSize); }
                if (DriveshaftPM != 0) { writer.WriteLine("pm= " + DriveshaftPM); }
                writer.WriteLine();     //Blank line seperator
            }

            //[Wheels]
            writer.WriteLine("[wheels]");
            if (WheelsWheelWidth != 0) { writer.WriteLine("wheelWidth= " + WheelsWheelWidth); }
            if (WheelsTire != "")      { writer.WriteLine("tire= " + WheelsTire); }
            if (WheelsRim != "")       { writer.WriteLine("rim= " + WheelsRim); }
            if (WheelsRimcap != "")    { writer.WriteLine("rimCap= " + WheelsRimcap); }        //Mercedes 300SL specfic at the moment
            if (WheelsRimSize != 0)    { writer.WriteLine("rimSize= " + WheelsRimSize); }
            if (WheelsTireSize != 0)   { writer.WriteLine("tireSize= " + WheelsTireSize); }
            //Maserati specfic at the moment
            if (WheelsWheelWidthRear != 0) { writer.WriteLine("wheelWidthRear= " + WheelsWheelWidthRear); }
            if (WheelsRimSizeRear != 0)    { writer.WriteLine("rimSizeRear= " + WheelsRimSizeRear); }
            if (WheelsTireSizeRear != 0)   { writer.WriteLine("tireSizeRear= " + WheelsTireSizeRear); }
            writer.WriteLine();     //Blank line seperator

            //[Wheels_Rear]
            if (WheelsRearRim != "")
            {
                writer.WriteLine("[wheels_rear]");
                if (WheelsRearWheelWidth != 0) { writer.WriteLine("wheelWidth= " + WheelsRearWheelWidth); }
                if (WheelsRearTire != "") { writer.WriteLine("tire= " + WheelsRearTire); }
                if (WheelsRearRim != "") { writer.WriteLine("rim= " + WheelsRearRim); }
                if (WheelsRearRimcap != "") { writer.WriteLine("rimCap= " + WheelsRearRimcap); }        //Mercedes 300SL specfic at the moment
                if (WheelsRearRimSize != 0) { writer.WriteLine("rimSize= " + WheelsRearRimSize); }
                if (WheelsRearTireSize != 0) { writer.WriteLine("tireSize= " + WheelsRearTireSize); }
                writer.WriteLine();     //Blank line seperator
            }

            //[PartsType] //0 to 6 (index)
            //As a car may have 0 to 7 of these, I'm going to use a struct in a <List> internaly
            int LocalCounter = 0;       //Index to use in output labels
            int ControlCounter = 0;     //Index to use in working through stored data
            //in 1.0.7.7 RedDot Fixed a bug with the Maluch (car_Kaszlak) by starting it's [parts<number>] at 1 instead of 0
            //This check and the use of the seperate counters allows for this special case
            if(MainModel == "car_Kaszlak")
            {
                LocalCounter = 1;   //If the Maluch/Kaszlak special case set the different start position
            }

            while (ControlCounter < Parts.Count())
            {
                writer.WriteLine("[parts" + LocalCounter + "]");
                writer.WriteLine("name= " + Parts[ControlCounter].PartsName);
                writer.WriteLine("position= " + Parts[ControlCounter].PartsPosition.x + "," + Parts[ControlCounter].PartsPosition.y + "," + Parts[ControlCounter].PartsPosition.z);
                writer.WriteLine("rotation= " + Parts[ControlCounter].PartsRotation.x + "," + Parts[ControlCounter].PartsRotation.y + "," + Parts[ControlCounter].PartsRotation.z);
                writer.WriteLine("scale= " + Parts[ControlCounter].PartsScale);
                writer.WriteLine();     //Blank line seperator
                LocalCounter++;
                ControlCounter++;
            }

            //[Interior]
            writer.WriteLine("[interior]");
            if ((InteriorSeatLeftPos.x != 0) || (InteriorSeatLeftPos.y != 0) || (InteriorSeatLeftPos.z != 0))
            { writer.WriteLine("seatLeftPos= " + InteriorSeatLeftPos.x + ", " + InteriorSeatLeftPos.y + ", " + InteriorSeatLeftPos.z); }
            if ((InteriorSeatLeftRot.x != 0) || (InteriorSeatLeftRot.y != 0) || (InteriorSeatLeftRot.z != 0))
            { writer.WriteLine("seatLeftRot= " + InteriorSeatLeftRot.x + ", " + InteriorSeatLeftRot.y + ", " + InteriorSeatLeftRot.z); }
            if (InteriorSeatScale != 0)     { writer.WriteLine("seatScale= " + InteriorSeatScale); }
            if (InteriorSeatHeightMod != 0) { writer.WriteLine("seatHeightMod= " + InteriorSeatHeightMod); }
            if (InteriorSeat != "")         { writer.WriteLine("seat= " + InteriorSeat); }
            if (InteriorWheel != "")        { writer.WriteLine("wheel= " + InteriorWheel); }
            if ((InteriorWheelPos.x != 0) || (InteriorWheelPos.y != 0) || (InteriorWheelPos.z != 0))
            { writer.WriteLine("wheelPos= " + InteriorWheelPos.x + ", " + InteriorWheelPos.y + ", " + InteriorWheelPos.z); }
            if ((InteriorWheelRot.x != 0) || (InteriorWheelRot.y != 0) || (InteriorWheelRot.z != 0))
            { writer.WriteLine("wheelRot= " + InteriorWheelRot.x + ", " + InteriorWheelRot.y + ", " + InteriorWheelRot.z); }
            if (InteriorWheelScale != 0) {  writer.WriteLine("wheelScale= " + InteriorWheelScale); }
            writer.WriteLine();     //Blank line seperator

            //[Logic]
            writer.WriteLine("[logic]");
            writer.WriteLine("globalCondition= " + LogicGlobalCondition.a + "," + LogicGlobalCondition.b);
            writer.WriteLine("partssConditions= " + LogicPartsConditions.a + "," + LogicPartsConditions.b);
            writer.WriteLine("panelsConditions= " + LogicPanelsConditions.a + "," + LogicPanelsConditions.b);
            writer.WriteLine("uniqueMod= " + LogicUniqueMod);
            writer.WriteLine();     //Blank line seperator

            writer.WriteLine();     //Blank line seperator
            writer.WriteLine("\nMade with= CMS15ModManager {0}", ModManVersion);

            //we are finished with the writer so close and bin it
            writer.Close();
            writer.Dispose();
        }

        #region Section processing
        //Parse the  [main] part of the car data file
        private void CarDataMainSectionProc(string[] CDFlines, ref int i)
        {
            //the i passed in is the line counter, is currently sat on the "[Main]" line
            i++;    //Move it along to the next line, so the while loop condition check doesn't end immediately

            while ((i < CDFlines.Length) && (!(CDFlines[i].StartsWith("["))))   //Keep reading lines until another section header line is found or out of lines
            {
                //Check for blank lines and null lines (end of file(might be able to remove the null check, legacy from a stream reader style))
                if ((CDFlines[i] != "") && (CDFlines[i] != null) && (!(CDFlines[i].StartsWith(";"))))    //if the line is empty or a comment skip over all this
                {
                        int j = CDFlines[i].IndexOf('=');              //Find the end of label string
                        string label = CDFlines[i].Substring(0, j);    //Grabs the bit upto the '='
                        //Grab the bit after the '=' and remove the leading and trailing spaces
                        string line = CDFlines[i].Substring(j + 1, CDFlines[i].Length - (j + 1)).Trim(' ');

                        switch (label)  //Fill out the Main data
                        {
                            case "model":
                                MainModel = line;
                                break;
                            case "rustMask":
                                MainRustMask = line;
                                break;
                            case "rotation":
                                string[] words = line.Split(',');                       //Split the data out
                                                                                        //convert the strings to numbers
                                float.TryParse(words[0], out MainRotation.x);
                                float.TryParse(words[1], out MainRotation.y);
                                float.TryParse(words[2], out MainRotation.z);
                                break;
                            default:
                                //Nothing here
                                //Blank lines and comments should be eaten outside of this if
                                //malformed lines will end up here
                                break;
                        }
                        i++;    //Move to next line
                }
                else
                {
                    //Blank line or comment(or null line, shouldn't be I'll see later if it needs a break)
                    //My //Special case ;name
                    //need to check  for the ;name for my line (if in main section)
                    if (CDFlines[i].StartsWith(";name"))
                    {
                        int j = CDFlines[i].IndexOf('=');                      //Find the end of label
                                                                               //Grab the bit after the '=' and remove the leading and trailing spaces
                        string line = CDFlines[i].Substring(j + 1, CDFlines[i].Length - (j + 1)).Trim(' ');
                        //line = line.Trim(' ');                                 //Remove the leading on trailing spaces
                        MainName = line;
                    }
                    //Normal comment or Special case ;name either way we need to,...
                    i++;    //Move to next line
                }
            }
            i--;    //Knock the counter back a line as the while loop that called us, will inc it and step over the section header we just found.
        }

        //Parse the [other] part of the car data file
        private void CarDataOtherSectionProc(string[] CDFlines, ref int i)
        {
            //the i passed in is the line counter, is currently sat on the "[Main]" line
            i++;    //Move it along to the next line, so the while loop condition check doesn't end immediately

            while ((i < CDFlines.Length) && (!(CDFlines[i].StartsWith("["))))   //Keep reading lines until another section header line is found or out of lines
            {
                //Check for blank lines and null lines (end of file(might be able to remove the null check, legacy from a stream reader style))
                if ((CDFlines[i] != "") && (CDFlines[i] != null) && (!(CDFlines[i].StartsWith(";"))))    //if the line is empty or a comment skip over all this
                {
                    int j = CDFlines[i].IndexOf('=');              //Find the end of label string
                    string line = "";
                    string label = "";
                    if (j != -1)        //If the string doesn't have a = in it
                    {
                        label = CDFlines[i].Substring(0, j);    //Grabs the bit upto the '='
                                                                //Grab the bit after the '=' and remove the leading and trailing spaces
                        line = CDFlines[i].Substring(j + 1, CDFlines[i].Length - (j + 1)).Trim(' ');
                    }

                    switch (label)  //Fill out the Main data
                    {
                        case "transmissionType":
                            OtherTransmissionType = line;
                            break;
                        case "gears":
                            int.TryParse(line, out OtherGears);      //convert the strings to numbers
                            break;
                        case "power":
                            int.TryParse(line, out OtherPower);      //convert the strings to numbers
                            break;
                        case "finalDriveRatio":
                            float.TryParse(line, out OtherFinalDriveRatio);  //convert the strings to numbers
                            break;
                        case "weight":
                            int.TryParse(line, out OtherWeight);     //convert the strings to numbers
                            break;
                        case "rpmFactor":
                            float.TryParse(line, out OtherRpmFactor);//convert the strings to numbers
                            break;
                        case "rpmAngle":
                            float.TryParse(line, out OtherRpmAngle);   //convert the strings to numbers
                            break;
                        case "speedoFactor":
                            float.TryParse(line, out OtherSpeedoFactor);   //convert the strings to numbers
                            break;
                        case "speedoAngle":
                            float.TryParse(line, out OtherSpeedoAngle);   //convert the strings to numbers
                            break;
                        case "suspTravel":
                            float.TryParse(line, out OtherSuspTravel);   //convert the strings to numbers
                            break;
                        case "lifterArmsAngle":
                            float.TryParse(line, out OtherLifterArmsAngle);//convert the strings to numbers
                            break;
                        case "lifterArmsRise":
                            float.TryParse(line, out OtherLifterArmsRise);//convert the strings to numbers
                            break;
                        case "doorAngle":
                            float.TryParse(line, out OtherDoorAngle);//convert the strings to numbers
                            break;
                        case "cx":
                            float.TryParse(line, out OtherCX);//convert the strings to numbers
                            break;
                        default:
                            //Nothing here
                            //Blank lines and comments should be eaten outside of this if
                            //malformed lines will end up here
                            break;
                    }
                    i++;    //Move to next line
                }
                else
                {
                    //Blank line or comment(or null line, shouldn't be I'll see later if it needs a break)
                    //Normal comment or Special case ;name either way we need to,...
                    i++;    //Move to next line
                }
            }
            i--;    //Knock the counter back a line as the while loop that called us, will inc it and step over the section header we just found.
        }

        //Parse the [suspension] part of the car data file
        private void CarDataSuspensionSectionProc(string[] CDFlines, ref int i)
        {
            //the i passed in is the line counter, is currently sat on the "[Main]" line
            i++;    //Move it along to the next line, so the while loop condition check doesn't end immediately

            while ((i < CDFlines.Length) && (!(CDFlines[i].StartsWith("["))))   //Keep reading lines until another section header line is found or out of lines
            {
                //Check for blank lines and null lines (end of file(might be able to remove the null check, legacy from a stream reader style))
                if ((CDFlines[i] != "") && (CDFlines[i] != null) && (!(CDFlines[i].StartsWith(";"))))    //if the line is empty or a comment skip over all this
                {
                    int j = CDFlines[i].IndexOf('=');              //Find the end of label string
                    string label = CDFlines[i].Substring(0, j);    //Grabs the bit upto the '='
                                                                   //Grab the bit after the '=' and remove the leading and trailing spaces
                    string line = CDFlines[i].Substring(j + 1, CDFlines[i].Length - (j + 1)).Trim(' ');

                    switch (label)  //Fill out the Main data
                    {
                        case "frontAxleStart":
                            float.TryParse(line, out SuspensionFrontAxleStart);  //convert the strings to numbers
                            break;
                        case "wheelBase":
                            float.TryParse(line, out SuspensionWheelBase);   //convert the strings to numbers
                            break;
                        case "height":
                            float.TryParse(line, out SuspensionHeight);      //convert the strings to numbers
                            break;
                        case "heightRear":
                            float.TryParse(line, out SuspensionHeightRear);  //convert the strings to numbers
                            break;
                        case "frontTrack":
                            float.TryParse(line, out SuspensionFrontTrack);  //convert the strings to numbers
                            break;
                        case "rearTrack":
                            float.TryParse(line, out SuspensionRearTrack);   //convert the strings to numbers
                            break;
                        case "frontSpringLength":
                            float.TryParse(line, out SuspensionFrontSpringLength);   //convert the strings to numbers
                            break;
                        case "scale":   //Will be removed after patch 1.0.8.0 (should be)
                            float.TryParse(line, out SuspensionScale);   //convert the strings to numbers
                            break;
                        case "forceScale":   //Kaszlak / Maluch
                            float.TryParse(line, out SuspensionScale);   //convert the strings to numbers
                            break;
                        case "sidesFlip":   //Kaszlak / Maluch
                            int.TryParse(line, out SuspensionSidesFlip);   //convert the strings to numbers
                            break;
                        case "frontCenterSet":
                            SuspensionFrontCenterSet = line;
                            break;
                        case "frontRightSet":
                            SuspensionFrontRightSet = line;
                            break;
                        case "frontLeftSet":
                            SuspensionFrontLeftSet = line;
                            break;
                        case "rearCenterSet":
                            SuspensionRearCenterSet = line;
                            break;
                        case "rearRightSet":
                            SuspensionRearRightSet = line;
                            break;
                        case "rearLeftSet":
                            SuspensionRearLeftSet = line;
                            break;
                        default:
                            //Nothing here
                            //Blank lines and comments should be eaten outside of this if
                            //malformed lines will end up here
                            break;
                    }
                    i++;    //Move to next line
                }
                else
                {
                    //Blank line or comment(or null line, shouldn't be I'll see later if it needs a break)
                    //Normal comment or Special case ;name either way we need to,...
                    i++;    //Move to next line
                }
            }
            i--;    //Knock the counter back a line as the while loop that called us, will inc it and step over the section header we just found.
        }

        //Parse the  [engine] part of the car data file
        private void CarDataEngineSectionProc(string[] CDFlines, ref int i)
        {
            //the i passed in is the line counter, is currently sat on the "[Main]" line
            i++;    //Move it along to the next line, so the while loop condition check doesn't end immediately
            string[] words;             //Local array used to split out values

            while ((i < CDFlines.Length) && (!(CDFlines[i].StartsWith("["))))   //Keep reading lines until another section header line is found or out of lines
            {
                //Check for blank lines and null lines (end of file(might be able to remove the null check, legacy from a stream reader style))
                if ((CDFlines[i] != "") && (CDFlines[i] != null) && (!(CDFlines[i].StartsWith(";"))))    //if the line is empty or a comment skip over all this
                {
                    int j = CDFlines[i].IndexOf('=');              //Find the end of label string
                    string label = CDFlines[i].Substring(0, j);    //Grabs the bit upto the '='
                                                                   //Grab the bit after the '=' and remove the leading and trailing spaces
                    string line = CDFlines[i].Substring(j + 1, CDFlines[i].Length - (j + 1)).Trim(' ');

                    switch (label)  //Fill out the Main data
                    {
                        case "position":
                            words = line.Split(',');                                //Split the data out
                                                                                    //convert the strings to numbers
                            float.TryParse(words[0], out EnginePosition.x);
                            float.TryParse(words[1], out EnginePosition.y);
                            float.TryParse(words[2], out EnginePosition.z);
                            break;
                        case "rotation":
                            words = line.Split(',');                                //Split the data out
                                                                                    //convert the strings to numbers
                            float.TryParse(words[0], out EngineRotation.x);
                            float.TryParse(words[1], out EngineRotation.y);
                            float.TryParse(words[2], out EngineRotation.z);
                            break;
                        case "scale":
                            float.TryParse(line, out EngineScale);   //convert the strings to numbers
                            break;
                        case "type":
                            EngineType = line;
                            break;
                        case "pm":
                            float.TryParse(line, out EnginePM);   //convert the strings to numbers
                            break;
                        case "swapoptions":
                            words = line.Split(',');            //Split the data out
                            EngineSwapOptions.AddRange(words);        //Add the split array to the list
                            //words[0];
                            break;
                        default:
                            //Nothing here
                            //Blank lines and comments should be eaten outside of this if
                            //malformed lines will end up here
                            break;
                    }
                    i++;    //Move to next line
                }
                else
                {
                    //Blank line or comment(or null line, shouldn't be I'll see later if it needs a break)
                    //Normal comment or Special case ;name either way we need to,...
                    i++;    //Move to next line
                }
            }
            i--;    //Knock the counter back a line as the while loop that called us, will inc it and step over the section header we just found.
        }

        //Parse the  [driveshaft] part of the car data file
        private void CarDataDriveshaftSectionProc(string[] CDFlines, ref int i)
        {
            //the i passed in is the line counter, is currently sat on the "[Main]" line
            i++;    //Move it along to the next line, so the while loop condition check doesn't end immediately
            string[] words;             //Local array used to split out values

            while ((i < CDFlines.Length) && (!(CDFlines[i].StartsWith("["))))   //Keep reading lines until another section header line is found or out of lines
            {
                //Check for blank lines and null lines (end of file(might be able to remove the null check, legacy from a stream reader style))
                if ((CDFlines[i] != "") && (CDFlines[i] != null) && (!(CDFlines[i].StartsWith(";"))))    //if the line is empty or a comment skip over all this
                {
                    int j = CDFlines[i].IndexOf('=');              //Find the end of label string
                    string label = CDFlines[i].Substring(0, j);    //Grabs the bit upto the '='
                                                                   //Grab the bit after the '=' and remove the leading and trailing spaces
                    string line = CDFlines[i].Substring(j + 1, CDFlines[i].Length - (j + 1)).Trim(' ');

                    switch (label)  //Fill out the Main data
                    {
                        case "position":
                            words = line.Split(',');                                //Split the data out
                                                                                    //convert the strings to numbers
                            float.TryParse(words[0], out DriveshaftPosition.x);
                            float.TryParse(words[1], out DriveshaftPosition.y);
                            float.TryParse(words[2], out DriveshaftPosition.z);
                            break;
                        case "rotation":
                            words = line.Split(',');                                //Split the data out
                                                                                    //convert the strings to numbers
                            float.TryParse(words[0], out DriveshaftRotation.x);
                            float.TryParse(words[1], out DriveshaftRotation.y);
                            float.TryParse(words[2], out DriveshaftRotation.z);
                            break;
                        case "scale":
                            float.TryParse(line, out DriveshaftScale);   //convert the strings to numbers
                            break;
                        case "type":
                            DriveshaftType = line;
                            break;
                        case "length":
                            float.TryParse(line, out DriveshaftLength);   //convert the strings to numbers
                            break;
                        case "size":
                            float.TryParse(line, out DriveshaftSize);   //convert the strings to numbers
                            break;
                        case "pm":
                            float.TryParse(line, out DriveshaftPM);   //convert the strings to numbers
                            break;
                        default:
                            //Nothing here
                            //Blank lines and comments should be eaten outside of this if
                            //malformed lines will end up here
                            break;
                    }
                    i++;    //Move to next line
                }
                else
                {
                    //Blank line or comment(or null line, shouldn't be I'll see later if it needs a break)
                    //Normal comment or Special case ;name either way we need to,...
                    i++;    //Move to next line
                }
            }
            i--;    //Knock the counter back a line as the while loop that called us, will inc it and step over the section header we just found.
        }

        //Parse the  [wheels] part of the car data file
        private void CarDataWheelsSectionProc(string[] CDFlines, ref int i)
        {
            //the i passed in is the line counter, is currently sat on the "[Main]" line
            i++;    //Move it along to the next line, so the while loop condition check doesn't end immediately

            while ((i < CDFlines.Length) && (!(CDFlines[i].StartsWith("["))))   //Keep reading lines until another section header line is found or out of lines
            {
                //Check for blank lines and null lines (end of file(might be able to remove the null check, legacy from a stream reader style))
                if ((CDFlines[i] != "") && (CDFlines[i] != null) && (!(CDFlines[i].StartsWith(";"))))    //if the line is empty or a comment skip over all this
                {
                    int j = CDFlines[i].IndexOf('=');              //Find the end of label string
                    string label = CDFlines[i].Substring(0, j);    //Grabs the bit upto the '='
                                                                   //Grab the bit after the '=' and remove the leading and trailing spaces
                    string line = CDFlines[i].Substring(j + 1, CDFlines[i].Length - (j + 1)).Trim(' ');

                    switch (label)  //Fill out the Main data
                    {
                        case "wheelWidth":
                            int.TryParse(line, out WheelsWheelWidth);   //convert the strings to numbers
                            break;
                        case "tire":
                            WheelsTire = line;
                            break;
                        case "rim":
                            WheelsRim = line;
                            break;
                        case "rimSize":
                            int.TryParse(line, out WheelsRimSize);   //convert the strings to numbers
                            break;
                        case "rimcap":
                            WheelsRimcap = line;
                            break;
                        case "tireSize":
                            int.TryParse(line, out WheelsTireSize);   //convert the strings to numbers
                            break;
                        //Maserati specfic at the moment
                        case "wheelWidthRear":
                            int.TryParse(line, out WheelsWheelWidthRear);   //convert the strings to numbers
                            break;
                        case "rimSizeRear":
                            int.TryParse(line, out WheelsRimSizeRear);   //convert the strings to numbers
                            break;
                        case "tireSizeRear":
                            int.TryParse(line, out WheelsTireSizeRear);   //convert the strings to numbers
                            break;
                        default:
                            //Nothing here
                            //Blank lines and comments should be eaten outside of this if
                            //malformed lines will end up here
                            break;
                    }
                    i++;    //Move to next line
                }
                else
                {
                    //Blank line or comment(or null line, shouldn't be I'll see later if it needs a break)
                    //Normal comment or Special case ;name either way we need to,...
                    i++;    //Move to next line
                }
            }
            i--;    //Knock the counter back a line as the while loop that called us, will inc it and step over the section header we just found.
        }

        //Parse the  [wheels] part of the car data file
        private void CarDataWheelsRearSectionProc(string[] CDFlines, ref int i)
        {
            //the i passed in is the line counter, is currently sat on the "[Main]" line
            i++;    //Move it along to the next line, so the while loop condition check doesn't end immediately

            while ((i < CDFlines.Length) && (!(CDFlines[i].StartsWith("["))))   //Keep reading lines until another section header line is found or out of lines
            {
                //Check for blank lines and null lines (end of file(might be able to remove the null check, legacy from a stream reader style))
                if ((CDFlines[i] != "") && (CDFlines[i] != null) && (!(CDFlines[i].StartsWith(";"))))    //if the line is empty or a comment skip over all this
                {
                    int j = CDFlines[i].IndexOf('=');              //Find the end of label string
                    string label = CDFlines[i].Substring(0, j);    //Grabs the bit upto the '='
                                                                   //Grab the bit after the '=' and remove the leading and trailing spaces
                    string line = CDFlines[i].Substring(j + 1, CDFlines[i].Length - (j + 1)).Trim(' ');

                    switch (label)  //Fill out the Main data
                    {
                        case "wheelWidth":
                            int.TryParse(line, out WheelsRearWheelWidth);   //convert the strings to numbers
                            break;
                        case "tire":
                            WheelsRearTire = line;
                            break;
                        case "rim":
                            WheelsRearRim = line;
                            break;
                        case "rimSize":
                            int.TryParse(line, out WheelsRearRimSize);   //convert the strings to numbers
                            break;
                        case "rimcap":
                            WheelsRearRimcap = line;
                            break;
                        case "tireSize":
                            int.TryParse(line, out WheelsRearTireSize);   //convert the strings to numbers
                            break;
                        default:
                            //Nothing here
                            //Blank lines and comments should be eaten outside of this if
                            //malformed lines will end up here
                            break;
                    }
                    i++;    //Move to next line
                }
                else
                {
                    //Blank line or comment(or null line, shouldn't be I'll see later if it needs a break)
                    //Normal comment or Special case ;name either way we need to,...
                    i++;    //Move to next line
                }
            }
            i--;    //Knock the counter back a line as the while loop that called us, will inc it and step over the section header we just found.
        }

        //Parse the [Parts<num>] part of the car data file
        public void CarDataPartsSectionProc(string[] CDFlines, ref int i)
        {
            PartsType LocalPD;          //Local to fill out before adding to the Parts List
            string[] words;             //Local array used to split out values

            //I have to init the parts, as the complier things it be will uninit later
            //This clears out the old data, but only for the in-use part of the list
            LocalPD.PartsName = "N";
            LocalPD.PartsPosition.x = 0;
            LocalPD.PartsPosition.y = 0;
            LocalPD.PartsPosition.z = 0;
            LocalPD.PartsRotation.x = 0;
            LocalPD.PartsRotation.y = 0;
            LocalPD.PartsRotation.z = 0;
            LocalPD.PartsScale = 0;

            //the i passed in is the line counter, is currently sat on the "[Main]" line
            i++;    //Move it along to the next line, so the while loop condition check doesn't end immediately

            while ((i < CDFlines.Length) && (!(CDFlines[i].StartsWith("["))))   //Keep reading lines until another section header line is found or out of lines
            {
                //Check for blank lines and null lines (end of file(might be able to remove the null check, legacy from a stream reader style))
                if ((CDFlines[i] != "") && (CDFlines[i] != null) && (!(CDFlines[i].StartsWith(";"))))    //if the line is empty or a comment skip over all this
                {
                    int j = CDFlines[i].IndexOf('=');              //Find the end of label string
                    string label = CDFlines[i].Substring(0, j);    //Grabs the bit upto the '='
                                                                   //Grab the bit after the '=' and remove the leading and trailing spaces
                    string line = CDFlines[i].Substring(j + 1, CDFlines[i].Length - (j + 1)).Trim(' ');

                    switch (label)  //Fill out the Main data
                    {
                        case "name":
                            LocalPD.PartsName = line;
                            break;
                        case "position":
                            words = line.Split(',');                                //Split the data out
                                                                                    //convert the strings to numbers
                            float.TryParse(words[0], out LocalPD.PartsPosition.x);
                            float.TryParse(words[1], out LocalPD.PartsPosition.y);
                            float.TryParse(words[2], out LocalPD.PartsPosition.z);
                            break;
                        case "rotation":
                            words = line.Split(',');                                //Split the data out
                                                                                    //convert the strings to numbers
                            float.TryParse(words[0], out LocalPD.PartsRotation.x);
                            float.TryParse(words[1], out LocalPD.PartsRotation.y);
                            float.TryParse(words[2], out LocalPD.PartsRotation.z);
                            break;
                        case "scale":
                            float.TryParse(line, out LocalPD.PartsScale);   //convert the strings to numbers
                            break;
                        default:
                            //Nothing here
                            //Blank lines and comments should be eaten outside of this if
                            //malformed lines will end up here
                            break;
                    }
                    i++;    //Move to next line
                }
                else
                {
                    //Blank line or comment(or null line, shouldn't be I'll see later if it needs a break)
                    //Normal comment or Special case ;name either way we need to,...
                    i++;    //Move to next line
                }
            }
            //We have all the data, so add the local to the collection list
            Parts.Add(LocalPD);
            i--;    //Knock the counter back a line as the while loop that called us, will inc it and step over the section header we just found.
        }

        //Parse the  [interior] part of the car data file
        private void CarDataInteriorSectionProc(string[] CDFlines, ref int i)
        {
            //the i passed in is the line counter, is currently sat on the "[Main]" line
            i++;    //Move it along to the next line, so the while loop condition check doesn't end immediately
            string[] words;             //Local array used to split out values

            while ((i < CDFlines.Length) && (!(CDFlines[i].StartsWith("["))))   //Keep reading lines until another section header line is found or out of lines
            {
                //Check for blank lines and null lines (end of file(might be able to remove the null check, legacy from a stream reader style))
                if ((CDFlines[i] != "") && (CDFlines[i] != null) && (!(CDFlines[i].StartsWith(";"))))    //if the line is empty or a comment skip over all this
                {
                    int j = CDFlines[i].IndexOf('=');              //Find the end of label string
                    string label = CDFlines[i].Substring(0, j);    //Grabs the bit upto the '='
                                                                   //Grab the bit after the '=' and remove the leading and trailing spaces
                    string line = CDFlines[i].Substring(j + 1, CDFlines[i].Length - (j + 1)).Trim(' ');

                    switch (label)  //Fill out the Main data
                    {
                        case "seatLeftPos":
                            words = line.Split(',');                                //Split the data out
                                                                                   //convert the strings to numbers
                            float.TryParse(words[0], out InteriorSeatLeftPos.x);
                            float.TryParse(words[1], out InteriorSeatLeftPos.y);
                            float.TryParse(words[2], out InteriorSeatLeftPos.z);
                            break;
                        case "seatLeftRot":
                            words = line.Split(',');                                //Split the data out
                                                                                   //convert the strings to numbers
                            float.TryParse(words[0], out InteriorSeatLeftRot.x);
                            float.TryParse(words[1], out InteriorSeatLeftRot.y);
                            float.TryParse(words[2], out InteriorSeatLeftRot.z);
                            break;
                        case "seatScale":
                            float.TryParse(line, out InteriorSeatScale);   //convert the strings to numbers
                            break;
                        case "seatHeightMod":
                            float.TryParse(line, out InteriorSeatHeightMod);   //convert the strings to numbers
                            break;
                        case "seat":
                            InteriorSeat = line;
                            break;
                        case "wheel":
                            InteriorWheel = line;
                            break;
                        case "wheelPos":
                            words = line.Split(',');                                //Split the data out
                                                                                   //convert the strings to numbers
                            float.TryParse(words[0], out InteriorWheelPos.x);
                            float.TryParse(words[1], out InteriorWheelPos.y);
                            float.TryParse(words[2], out InteriorWheelPos.z);
                            break;
                        case "wheelRot":
                            words = line.Split(',');                                //Split the data out
                                                                                   //convert the strings to numbers
                            float.TryParse(words[0], out InteriorWheelRot.x);
                            float.TryParse(words[1], out InteriorWheelRot.y);
                            float.TryParse(words[2], out InteriorWheelRot.z);
                            break;
                        case "wheelScale":
                            float.TryParse(line, out InteriorWheelScale);   //convert the strings to numbers
                            break;
                        default:
                            //Nothing here
                            //Blank lines and comments should be eaten outside of this if
                            //malformed lines will end up here
                            break;
                    }
                    i++;    //Move to next line
                }
                else
                {
                    //Blank line or comment(or null line, shouldn't be I'll see later if it needs a break)
                    //Normal comment or Special case ;name either way we need to,...
                    i++;    //Move to next line
                }
            }
            i--;    //Knock the counter back a line as the while loop that called us, will inc it and step over the section header we just found.
        }

        //Parse the  [logic] part of the car data file
        private void CarDataLogicSectionProc(string[] CDFlines, ref int i)
        {
            //the i passed in is the line counter, is currently sat on the "[Main]" line
            i++;    //Move it along to the next line, so the while loop condition check doesn't end immediately
            string[] words;             //Local array used to split out values

            while ((i < CDFlines.Length) && (!(CDFlines[i].StartsWith("["))))   //Keep reading lines until another section header line is found or out of lines
            {
                //Check for blank lines and null lines (end of file(might be able to remove the null check, legacy from a stream reader style))
                if ((CDFlines[i] != "") && (CDFlines[i] != null) && (!(CDFlines[i].StartsWith(";"))))    //if the line is empty or a comment skip over all this
                {
                    int j = CDFlines[i].IndexOf('=');              //Find the end of label string
                    string label = CDFlines[i].Substring(0, j);    //Grabs the bit upto the '='
                                                                   //Grab the bit after the '=' and remove the leading and trailing spaces
                    string line = CDFlines[i].Substring(j + 1, CDFlines[i].Length - (j + 1)).Trim(' ');

                    switch (label)  //Fill out the Main data
                    {
                        case "globalCondition":
                            words = line.Split(',');                                //Split the data out
                                                                                    //convert the strings to numbers
                            float.TryParse(words[0], out LogicGlobalCondition.a);
                            float.TryParse(words[1], out LogicGlobalCondition.b);
                            break;
                        case "partsConditions":
                            words = line.Split(',');                                //Split the data out
                                                                                    //convert the strings to numbers
                            float.TryParse(words[0], out LogicPartsConditions.a);
                            float.TryParse(words[1], out LogicPartsConditions.b);
                            break;
                        case "panelsConditions":
                            words = line.Split(',');                                //Split the data out
                                                                                    //convert the strings to numbers
                            float.TryParse(words[0], out LogicPanelsConditions.a);
                            float.TryParse(words[1], out LogicPanelsConditions.b);
                            break;
                        case "uniqueMod":
                            float.TryParse(line, out LogicUniqueMod);   //convert the strings to numbers
                            break;
                        default:
                            //Nothing here
                            //Blank lines and comments should be eaten outside of this if
                            //malformed lines will end up here
                            break;
                    }
                    i++;    //Move to next line
                }
                else
                {
                    //Blank line or comment(or null line, shouldn't be I'll see later if it needs a break)
                    //Normal comment or Special case ;name either way we need to,...
                    i++;    //Move to next line
                }
            }
            i--;    //Knock the counter back a line as the while loop that called us, will inc it and step over the section header we just found.
        }

        //Empty out all data
        public void CarDataClearAll()
        {
            //[Main]
            MainName = "";     //My addition to tie the 'in game' name to the car
            MainModel = "";
            MainRustMask = "";
            MainRotation.x = 0;
            MainRotation.y = 0;
            MainRotation.z = 0;

            //[Other]
            OtherPower = 0;
            OtherTransmissionType = "";
            OtherGears = 0;
            OtherFinalDriveRatio = 0.0f;
            OtherWeight = 0;
            OtherRpmFactor = 0.0f;
            OtherRpmAngle = 0;
            OtherSpeedoFactor = 0.0f;
            OtherSpeedoAngle = 0.0f;
            OtherSuspTravel = 0.0f;
            OtherLifterArmsRise = 0.0f;
            OtherLifterArmsAngle = 0.0f;
            OtherDoorAngle = 0.0f;           //Gull wing door
            OtherCX = 0.0f;

            //[Suspension]
            SuspensionFrontAxleStart = 0.0f;
            SuspensionWheelBase = 0.0f;
            SuspensionHeight = 0.0f;
            SuspensionHeightRear = 0.0f;
            SuspensionFrontTrack = 0.0f;
            SuspensionRearTrack = 0.0f;
            SuspensionFrontSpringLength = 0.0f;
            SuspensionScale = 0.0f;             //Kaszlak specfic post 1.0.8.0 (should be)
            SuspensionSidesFlip = 0;			//Kaszlak specfic at the moment
            SuspensionFrontCenterSet = "";
            SuspensionFrontRightSet = "";
            SuspensionFrontLeftSet = "";		//Kaszlak specfic at the moment
            SuspensionRearCenterSet = "";
            SuspensionRearRightSet = "";
            SuspensionRearLeftSet = "";		//Kaszlak specfic at the moment

            //[Engine]
            EnginePosition.x = 0.0f;
            EnginePosition.y = 0.0f;
            EnginePosition.z = 0.0f;
            EngineRotation.x = 0.0f;
            EngineRotation.y = 0.0f;
            EngineRotation.z = 0.0f;
            EngineScale = 0.0f;
            EngineType = "";
            EnginePM = 0.0f;
            //Empty out the list of engine swap options
            RemoveAllSwapOptions();

            //[Driveshaft]
            DriveshaftPosition.x = 0.0f;
            DriveshaftPosition.y = 0.0f;
            DriveshaftPosition.z = 0.0f;
            DriveshaftRotation.x = 0.0f;
            DriveshaftRotation.y = 0.0f;
            DriveshaftRotation.z = 0.0f;
            DriveshaftScale = 0.0f;
            DriveshaftType = "";
            DriveshaftLength = 0.0f;
            DriveshaftSize = 0;
            DriveshaftPM = 0.0f;

            //[Wheels]
            WheelsWheelWidth = 0;
            WheelsTire = "";
            WheelsRim = "";
            WheelsRimcap = "";		//Mercedes 300SL specfic at the moment
            WheelsRimSize = 0;
            WheelsTireSize = 0;
            //Maserati specfic at the moment
            WheelsWheelWidthRear = 0;
            WheelsRimSizeRear = 0;
            WheelsTireSizeRear = 0;

            //[Wheels_Rear]
            WheelsRearWheelWidth = 0;
            WheelsRearTire = "";
            WheelsRearRim = "";
            WheelsRearRimcap = "";		//Mercedes 300SL specfic at the moment
            WheelsRearRimSize = 0;
            WheelsRearTireSize = 0;

            //[PartsType] //0 to 6 (index)
            RemoveAllParts();

            //[Interior]
            InteriorSeatLeftPos.x = 0.0f;
            InteriorSeatLeftPos.y = 0.0f;
            InteriorSeatLeftPos.z = 0.0f;
            InteriorSeatLeftRot.x = 0.0f;
            InteriorSeatLeftRot.y = 0.0f;
            InteriorSeatLeftRot.z = 0.0f;
            InteriorSeatScale = 0.0f;
            InteriorSeatHeightMod = 0.0f;
            InteriorSeat = "";
            InteriorWheel = "";
            InteriorWheelPos.x = 0.0f;
            InteriorWheelPos.y = 0.0f;
            InteriorWheelPos.z = 0.0f;
            InteriorWheelRot.x = 0.0f;
            InteriorWheelRot.y = 0.0f;
            InteriorWheelRot.z = 0.0f;
            InteriorWheelScale = 0.0f;

            //[Logic]
            LogicGlobalCondition.a = 0.0f;
            LogicGlobalCondition.b = 0.0f;
            LogicPartsConditions.a = 0.0f;
            LogicPartsConditions.b = 0.0f;
            LogicPanelsConditions.a = 0.0f;
            LogicPanelsConditions.b = 0.0f;
            LogicUniqueMod = 0.0f;            
    }
        #endregion

        #region Getters and Setters
        #region [Parts]
        //This is a bit different, as it takes a whole part item and adds it at once
        //as we use an internal <List> of structs, due to the having a possible 0 to 7 of them
        public void PartsSetter(string name, float posX, float posY, float posZ, float rotX, float rotY, float rotZ, float scale)
        {
            //Local data struct to fill out and add to overall data collection
            PartsType LocalParts;

            //Fill out the local
            LocalParts.PartsName = name;
            LocalParts.PartsPosition.x = posX;
            LocalParts.PartsPosition.y = posY;
            LocalParts.PartsPosition.z = posZ;
            LocalParts.PartsRotation.x = rotX;
            LocalParts.PartsRotation.y = rotY;
            LocalParts.PartsRotation.z = rotZ;
            LocalParts.PartsScale = scale;

            //We have all the data, so add the local to the collection list
            Parts.Add(LocalParts);
        }

        //Remove a part from the Parts List
        public void RemovePartsItem(int index)
        {
            Parts.RemoveAt(index);
        }

        //Emptys out the whole list
        public void RemoveAllParts()
        {
            Parts.Clear();
        }

        //Get the size of the parts array
        public int ReturnPartsSize()
        {
            return Parts.Count;
        }

        public string GetPartsName(int index)
        {
            return Parts[index].PartsName;
        }

        public float GetPartsPosX(int index)
        {
            return Parts[index].PartsPosition.x;
        }

        public float GetPartsPosY(int index)
        {
            return Parts[index].PartsPosition.y;
        }

        public float GetPartsPosZ(int index)
        {
            return Parts[index].PartsPosition.z;
        }

        public float GetPartsRotX(int index)
        {
            return Parts[index].PartsRotation.x;
        }

        public float GetPartsRotY(int index)
        {
            return Parts[index].PartsRotation.y;
        }

        public float GetPartsRotZ(int index)
        {
            return Parts[index].PartsRotation.z;
        }

        public float GetPartScale(int index)
        {
            return Parts[index].PartsScale;
        }
        #endregion

        #region [Main]
        public string _MainName
        {
            get { return MainName; }
            set { MainName = value; }
        }

        public string _MainModel
        {
            get { return MainModel; }
            set { MainModel = value; }
        }

        public string _MainRustMask
        {
            get { return MainRustMask; }
            set { MainRustMask = value; }
        }

        public float _MainRotationX
        {
            get { return MainRotation.x; }
            set { MainRotation.x = value; }
        }

        public float _MainRotationY
        {
            get { return MainRotation.y; }
            set { MainRotation.y = value; }
        }

        public float _MainRotationZ
        {
            get { return MainRotation.z; }
            set { MainRotation.z = value; }
        }
        #endregion

        #region [Other]
        public int _OtherPower
        {
            get { return OtherPower; }
            set { OtherPower = value; }
        }

        public string _OtherTransmissionType
        {
            get { return OtherTransmissionType; }
            set { OtherTransmissionType = value; }
        }

        public int _OtherGears
        {
            get { return OtherGears; }
            set { OtherGears = value; }
        }

        public float _OtherFinalDriveRatio
        {
            get { return OtherFinalDriveRatio; }
            set { OtherFinalDriveRatio = value; }
        }

        public int _OtherWeight
        {
            get { return OtherWeight; }
            set { OtherWeight = value; }
        }

        public float _OtherRpmFactor
        {
            get { return OtherRpmFactor; }
            set { OtherRpmFactor = value; }
        }

        public float _OtherRpmAngle
        {
            get { return OtherRpmAngle; }
            set { OtherRpmAngle = value; }
        }

        public float _OtherSpeedoFactor
        {
            get { return OtherSpeedoFactor; }
            set { OtherSpeedoFactor = value; }
        }

        public float _OtherSpeedoAngle
        {
            get { return OtherSpeedoAngle; }
            set { OtherSpeedoAngle = value; }
        }

        public float _OtherSuspTravel
        {
            get { return OtherSuspTravel; }
            set { OtherSuspTravel = value; }
        }

        public float _OtherLifterArmsRise
        {
            get { return OtherLifterArmsRise; }
            set { OtherLifterArmsRise = value; }
        }

        public float _OtherLifterArmsAngle
        {
            get { return OtherLifterArmsAngle; }
            set { OtherLifterArmsAngle = value; }
        }

        public float _OtherDoorAngle
        {
            get { return OtherDoorAngle; }
            set { OtherDoorAngle = value; }
        }

        public float _OtherCX
        {
            get { return OtherCX; }
            set { OtherCX = value; }
        }
        #endregion

        #region [Suspension]
        public float _SuspensionFrontAxleStart
        {
            get { return SuspensionFrontAxleStart; }
            set { SuspensionFrontAxleStart = value; }
        }

        public float _SuspensionWheelBase
        {
            get { return SuspensionWheelBase; }
            set { SuspensionWheelBase = value; }
        }

        public float _SuspensionHeight
        {
            get { return SuspensionHeight; }
            set { SuspensionHeight = value; }
        }

        public float _SuspensionHeightRear
        {
            get { return SuspensionHeightRear; }
            set { SuspensionHeightRear = value; }
        }

        public float _SuspensionFrontTrack
        {
            get { return SuspensionFrontTrack; }
            set { SuspensionFrontTrack = value; }
        }

        public float _SuspensionRearTrack
        {
            get { return SuspensionRearTrack; }
            set { SuspensionRearTrack = value; }
        }

        public float _SuspensionFrontSpringLength
        {
            get { return SuspensionFrontSpringLength; }
            set { SuspensionFrontSpringLength = value; }
        }

        public float _SuspensionScale
        {
            get { return SuspensionScale; }
            set { SuspensionScale = value; }
        }

        public int _SuspensionsidesFlip
        {
            get { return SuspensionSidesFlip; }
            set { SuspensionSidesFlip = value; }
        }

        public string _SuspensionFrontCenterSet
        {
            get { return SuspensionFrontCenterSet; }
            set { SuspensionFrontCenterSet = value; }
        }

        public string _SuspensionFrontRightSet
        {
            get { return SuspensionFrontRightSet; }
            set { SuspensionFrontRightSet = value; }
        }

        public string _SuspensionFrontLeftSet
        {
            get { return SuspensionFrontLeftSet; }
            set { SuspensionFrontLeftSet = value; }
        }

        public string _SuspensionRearCenterSet
        {
            get { return SuspensionRearCenterSet; }
            set { SuspensionRearCenterSet = value; }
        }

        public string _SuspensionRearRightSet
        {
            get { return SuspensionRearRightSet; }
            set { SuspensionRearRightSet = value; }
        }

        public string _SuspensionRearLeftSet
        {
            get { return SuspensionRearLeftSet; }
            set { SuspensionRearLeftSet = value; }
        }
        #endregion

        #region [Engine]
        public float _EnginePositionX
        {
            get { return EnginePosition.x; }
            set { EnginePosition.x = value; }
        }

        public float _EnginePositionY
        {
            get { return EnginePosition.y; }
            set { EnginePosition.y = value; }
        }

        public float _EnginePositionZ
        {
            get { return EnginePosition.z; }
            set { EnginePosition.z = value; }
        }

        public float _EngineRotationX
        {
            get { return EngineRotation.x; }
            set { EngineRotation.x = value; }
        }

        public float _EngineRotationY
        {
            get { return EngineRotation.y; }
            set { EngineRotation.y = value; }
        }

        public float _EngineRotationZ
        {
            get { return EngineRotation.z; }
            set { EngineRotation.z = value; }
        }

        public float _EngineScale
        {
            get { return EngineScale; }
            set { EngineScale = value; }
        }

        public string _EngineType
        {
            get { return EngineType; }
            set { EngineType = value; }
        }

        public float _EnginePM
        {
            get { return EnginePM; }
            set { EnginePM = value; }
        }

        //as we use an internal <List> of strings to hold the engine SwapOptions
        public void SwapOptionsSetter(string engine)
        {
            //Add engine to the collection list
            EngineSwapOptions.Add(engine);
        }

        //Remove a part from the Parts List
        public void RemoveSwapOptionsItem(int index)
        {
            EngineSwapOptions.RemoveAt(index);
        }

        //Empty out the list of engine swap options
        public void RemoveAllSwapOptions()
        {
            EngineSwapOptions.Clear();
        }

        //Get the size of the SwapOptions array
        public int ReturnSwapOptionsSize()
        {
            return EngineSwapOptions.Count;
        }

        //Return an engine option from the SwapOptions list
        public string GetSwapOptions(int index)
        {
            return EngineSwapOptions[index];
        }

        //Checks if the EngineSwapOptions contains a give engine
        public bool SwapOptionsContains(string compare)
        {
            //Setup the local return value
            bool retval = false;

            foreach(string line in EngineSwapOptions)
            {
                if(compare == line)
                {
                    retval = true;
                }
            }


            return retval;
        }

        #endregion

        #region [Driveshaft]
        public float _DriveshaftPositionX
        {
            get { return DriveshaftPosition.x; }
            set { DriveshaftPosition.x = value; }
        }

        public float _DriveshaftPositionY
        {
            get { return DriveshaftPosition.y; }
            set { DriveshaftPosition.y = value; }
        }

        public float _DriveshaftPositionZ
        {
            get { return DriveshaftPosition.z; }
            set { DriveshaftPosition.z = value; }
        }

        public float _DriveshaftRotationX
        {
            get { return DriveshaftRotation.x; }
            set { DriveshaftRotation.x = value; }
        }

        public float _DriveshaftRotationY
        {
            get { return DriveshaftRotation.y; }
            set { DriveshaftRotation.y = value; }
        }

        public float _DriveshaftRotationZ
        {
            get { return DriveshaftRotation.z; }
            set { DriveshaftRotation.z = value; }
        }

        public float _DriveshaftScale
        {
            get { return DriveshaftScale; }
            set { DriveshaftScale = value; }
        }

        public string _DriveshaftType
        {
            get { return DriveshaftType; }
            set { DriveshaftType = value; }
        }

        public float _DriveshaftLength
        {
            get { return DriveshaftLength; }
            set { DriveshaftLength = value; }
        }

        public float _DriveshaftSize
        {
            get { return DriveshaftSize; }
            set { DriveshaftSize = value; }
        }

        public float _DriveshaftPM
        {
            get { return DriveshaftPM; }
            set { DriveshaftPM = value; }
        }
        #endregion

        #region [Wheels]
        public int _WheelsWheelWidth
        {
            get { return WheelsWheelWidth; }
            set { WheelsWheelWidth = value; }
        }

        public string _WheelsTire
        {
            get { return WheelsTire; }
            set { WheelsTire = value; }
        }

        public string _WheelsRim
        {
            get { return WheelsRim; }
            set { WheelsRim = value; }
        }

        public string _WheelsRimcap
        {
            get { return WheelsRimcap; }
            set { WheelsRimcap = value; }
        }

        public int _WheelsRimSize
        {
            get { return WheelsRimSize; }
            set { WheelsRimSize = value; }
        }

        public int _WheelsTireSize
        {
            get { return WheelsTireSize; }
            set { WheelsTireSize = value; }
        }

        public int _WheelsWheelWidthRear
        {
            get { return WheelsWheelWidthRear; }
            set { WheelsWheelWidthRear = value; }
        }

        public int _WheelsTireSizeRear
        {
            get { return WheelsTireSizeRear; }
            set { WheelsTireSizeRear = value; }
        }

        public int _WheelsRimSizeRear
        {
            get { return WheelsRimSizeRear; }
            set { WheelsRimSizeRear = value; }
        }
        #endregion

        #region [Wheels_Rear]
        public int _WheelsRearWheelWidth
        {
            get { return WheelsRearWheelWidth; }
            set { WheelsRearWheelWidth = value; }
        }

        public string _WheelsRearTire
        {
            get { return WheelsRearTire; }
            set { WheelsRearTire = value; }
        }

        public string _WheelsRearRim
        {
            get { return WheelsRearRim; }
            set { WheelsRearRim = value; }
        }

        public string _WheelsRearRimcap
        {
            get { return WheelsRearRimcap; }
            set { WheelsRearRimcap = value; }
        }

        public int _WheelsRearRimSize
        {
            get { return WheelsRearRimSize; }
            set { WheelsRearRimSize = value; }
        }

        public int _WheelsRearTireSize
        {
            get { return WheelsRearTireSize; }
            set { WheelsRearTireSize = value; }
        }
        #endregion

        #region [Interior]
        public float _InteriorSeatLeftPosX
        {
            get { return InteriorSeatLeftPos.x; }
            set { InteriorSeatLeftPos.x = value; }
        }

        public float _InteriorSeatLeftPosY
        {
            get { return InteriorSeatLeftPos.y; }
            set { InteriorSeatLeftPos.y = value; }
        }

        public float _InteriorSeatLeftPosZ
        {
            get { return InteriorSeatLeftPos.z; }
            set { InteriorSeatLeftPos.z = value; }
        }

        public float _InteriorSeatLeftRotX
        {
            get { return InteriorSeatLeftRot.x; }
            set { InteriorSeatLeftRot.x = value; }
        }

        public float _InteriorSeatLeftRotY
        {
            get { return InteriorSeatLeftRot.y; }
            set { InteriorSeatLeftRot.y = value; }
        }

        public float _InteriorSeatLeftRotZ
        {
            get { return InteriorSeatLeftRot.z; }
            set { InteriorSeatLeftRot.z = value; }
        }

        public float _InteriorSeatScale
        {
            get { return InteriorSeatScale; }
            set { InteriorSeatScale = value; }
        }

        public float _InteriorSeatHeightMod
        {
            get { return InteriorSeatHeightMod; }
            set { InteriorSeatHeightMod = value; }
        }

        public string _InteriorSeat
        {
            get { return InteriorSeat; }
            set { InteriorSeat = value; }
        }

        public string _InteriorWheel
        {
            get { return InteriorWheel; }
            set { InteriorWheel = value; }
        }

        public float _InteriorWheelPosX
        {
            get { return InteriorWheelPos.x; }
            set { InteriorWheelPos.x = value; }
        }

        public float _InteriorWheelPosY
        {
            get { return InteriorWheelPos.y; }
            set { InteriorWheelPos.y = value; }
        }

        public float _InteriorWheelPosZ
        {
            get { return InteriorWheelPos.z; }
            set { InteriorWheelPos.z = value; }
        }

        public float _InteriorWheelRotX
        {
            get { return InteriorWheelRot.x; }
            set { InteriorWheelRot.x = value; }
        }

        public float _InteriorWheelRotY
        {
            get { return InteriorWheelRot.y; }
            set { InteriorWheelRot.y = value; }
        }

        public float _InteriorWheelRotZ
        {
            get { return InteriorWheelRot.z; }
            set { InteriorWheelRot.z = value; }
        }

        public float _InteriorWheelScale
        {
            get { return InteriorWheelScale; }
            set { InteriorWheelScale = value; }
        }
        #endregion

        #region [Logic]
        public float _LogicGlobalConditionA
        {
            get { return LogicGlobalCondition.a; }
            set { LogicGlobalCondition.a = value; }
        }

        public float _LogicGlobalConditionB
        {
            get { return LogicGlobalCondition.b; }
            set { LogicGlobalCondition.b = value; }
        }

        public float _LogicPartsConditionsA
        {
            get { return LogicPartsConditions.a; }
            set { LogicPartsConditions.a = value; }
        }

        public float _LogicPartsConditionsB
        {
            get { return LogicPartsConditions.b; }
            set { LogicPartsConditions.b = value; }
        }

        public float _LogicPanelsConditionsA
        {
            get { return LogicPanelsConditions.a; }
            set { LogicPanelsConditions.a = value; }
        }

        public float _LogicPanelsConditionsB
        {
            get { return LogicPanelsConditions.b; }
            set { LogicPanelsConditions.b = value; }
        }

        public float _LogicUniqueMod
        {
            get { return LogicUniqueMod; }
            set { LogicUniqueMod = value; }
        }

        #endregion
        #endregion
    }
}
