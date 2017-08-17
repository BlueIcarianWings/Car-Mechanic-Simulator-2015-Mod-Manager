using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;    //For streamReader, etc

namespace WindowsFormsApplication1
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }

    public class CMS2015MM
    {
        string ConfigFN = "CMS2015MMConfig.txt";    //Holds the name of the config file
        private string ConfigDir = null;        //Holds dir the config file is in (same as the exe)
        private string SavedGamesDir = null;    //Holds the dir of the saved games
        private string SavedGamesDirBkUp = null;//Holds the backup dir of the saved games
        private string CarsDataDir = null;      //Holds the dir of the car data
        private string CarsDataDirBkUp = null;  //Holds the backup dir of the car 
        private string ModMapDir = null;        //Holds the dir the custom maps live in

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

        //Strut to hold car list data
        public struct CarListType
        {
            public string InternalName;
            public string ExternalName;
        }
        //Array of cars list from cars.txt
        private List<CarListType> CarsCurrentList = new List<CarListType>();

        //Array of car data files in dir
        private List<string> CarsAvailableFilesList = new List<string>();

        //-------------------------------------------
        #region Struts for Car Data
        public struct DoubleFloatPoint
        {
            public float a;
            public float b;
        }

        public struct TripleFloatPoint
        {
            public float x;
            public float y;
            public float z;
        }

        public struct CarMainType
        {
            public string name;     //My addition to tie the 'in game' name to the car
            public string model;
            public string rustMask;
            public TripleFloatPoint rotation;
        }

        public struct CarOtherType
        {
            public string engineSound;
            public string transmissionType;
            public int gears;
            public float finalDriveRatio;
            public int weight;
            public float rpmFactor;
            public int rpmAngle;
            public float speedoAngle;
            public float suspTravel;
            public float lifterArmsRise;
            public float cx;
        }

        public struct CarSuspensionType
        {
            public float frontAxleStart;
            public float wheelBase;
            public float height;
            public float frontTrack;
            public float rearTrack;
            public float frontSpringLength;
            public float scale;

            public string frontCenterSet;
            public string frontRightSet;
            public string rearCenterSet;
            public string rearRightSet;
        }

        public struct CarEngineType
        {
            public TripleFloatPoint position;
            public TripleFloatPoint rotation;
            public float scale;
            public string type;
            public string sound;
        }

        public struct CarDriveshaftType
        {
            public TripleFloatPoint position;
            public TripleFloatPoint rotation;
            public float scale;
            public string type;
            public float length;
        }

        public struct CarWheelsType
        {
            public int wheelWidth;
            public string tire;
            public string rim;
            public int rimSize;
            public int tireSize;
        }

        public struct CarPartsType //0 to 6
        {
            public string name;
            public TripleFloatPoint position;
            public TripleFloatPoint rotation;
            public float scale;
        }

        public struct CarInteriorType
        {
            public TripleFloatPoint seatLeftPos;
            public TripleFloatPoint seatLeftRot;
            public float seatScale;
            public string seat;
            public string wheel;
            public TripleFloatPoint wheelPos;
            public TripleFloatPoint wheelRot;
        }

        public struct CarLogicType
        {
            public DoubleFloatPoint globalCondition;
            public DoubleFloatPoint partsConditions;
            public DoubleFloatPoint panelsConditions;
            public float uniqueMod;
            public bool blockOBD;
        }

        //Overal struct to bring them together

        public struct CarDataCompleteType
        {
            public CarMainType Main;
            public CarOtherType Other;
            public CarSuspensionType Suspension;
            public CarEngineType Engine;
            public CarDriveshaftType Driveshaft;
            public CarWheelsType Wheels;
            public List<CarPartsType> Parts;
            public CarInteriorType Interior;
            public CarLogicType Logic;
        }

        public CarDataCompleteType LoadedCarData;   //Final object to hold all the car data
        //Cannot get the size of a list inside a strut, so will use a manual external to it count
        private int PartsCounter = 0;               //Holds the size of the Parts list
        #endregion
        //-------------------------------------------

        // Constructor
        public CMS2015MM()
        {
            //Need to initialize(/create) the list here
            LoadedCarData.Parts = new List<CarPartsType>();
        }

        // Destructor
        ~CMS2015MM()
        {
            // Some resource cleanup routines
        }

        #region Config stuff
        //Load config data from config file
        public void ReadConfigFile()
        {
            //Asumes the config flie is local to the Mod Manager exe

            //Get dir we are currently in
            ConfigDir = System.IO.Path.GetDirectoryName(Application.ExecutablePath);

            //Check if the config file exists
            if (File.Exists(ConfigFN))
            {
                //create a streamReader to accses the config file
                StreamReader reader = new StreamReader(ConfigFN);
                //string list (an array) to hold file output
                List<string> list = new List<string>();
                //string to hold a single line
                string line;

                //loop through all of file a line at a time
                while (true)
                {
                    //Read a line from the file
                    line = reader.ReadLine();
                    //check if line is null
                    if (line == null)
                    {
                        break;  //exit loop if an empty line
                    }
                    else
                    {
                        list.Add(line); //Add line to the list
                    }
                }

                //we are finished with the reader so close and bin it
                reader.Close();
                reader.Dispose();

                //Now to process the config 
                SavedGamesDir = list[0];    //Grab the first line
                SavedGamesDirBkUp = list[1];//Grab the second line
                CarsDataDir = list[2];      //grab the third line
                CarsDataDirBkUp = list[3];  //grab the forth line
                ModMapDir = list[4];        //grab the fifth line
            }
            else
            {
                //the file doesn't exist load defaults
                SavedGamesDir = "%userprofile%\\appdata\\locallow\\Red Dot Games\\Car Mechanic Simulator 2015";
                SavedGamesDirBkUp = "%userprofile%\\appdata\\locallow\\Red Dot Games\\Car Mechanic Simulator 2015\\MMBackUp";
                CarsDataDir = "C:\\Program Files (x86)\\Steam\\steamapps\\common\\Car Mechanic Simulator 2015\\cms2015_Data\\Datacars";
                CarsDataDirBkUp = "C:\\Program Files (x86)\\Steam\\steamapps\\common\\Car Mechanic Simulator 2015\\cms2015_Data\\Datacars\\MMBackUp";
                ModMapDir = "C:\\Program Files (x86)\\Steam\\steamapps\\common\\Car Mechanic Simulator 2015\\Mods\\Tracks";
            }
        }

        //Save the config data file
        public void SaveConfigFile()
        {
            //Save the config file data, called whenever a change has been made via the menus
            //the menus will set the strings and this will write both

            using (StreamWriter writer = new StreamWriter(ConfigFN))
            {
                writer.WriteLine(SavedGamesDir);
                writer.WriteLine(SavedGamesDirBkUp);
                writer.WriteLine(CarsDataDir);
                writer.WriteLine(CarsDataDirBkUp);
                writer.WriteLine(ModMapDir);
            }
        }

        #region Getters and Setters
        public string GetSavedGamesDir()
        {
            return SavedGamesDir;
        }

        public string GetSavedGamesDirBkUp()
        {
            return SavedGamesDirBkUp;
        }

        public void SetSavedGamesDir(string Input)
        {
            SavedGamesDir = Input;
        }

        public void SetSavedGamesDirBkUp(string Input)
        {
            SavedGamesDirBkUp = Input;
        }

        public string GetCarsDataDir()
        {
            return CarsDataDir;
        }

        public string GetCarsDataDirBkUp()
        {
            return CarsDataDirBkUp;
        }

        public void SetCarsDataDir(string Input)
        {
            CarsDataDir = Input;
        }

        public void SetCarsDataDirBkUp(string Input)
        {
            CarsDataDirBkUp = Input;
        }

        public string GetModMapDir()
        {
            return ModMapDir;
        }

        public void SetModMapDir(string Input)
        {
            ModMapDir = Input;
        }
        #endregion
        #endregion

        //Copy a directory, choice to copy sub dirs or not
        public void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs)
        {
            //Taken from
            //https://msdn.microsoft.com/en-us/library/bb762914%28v=vs.110%29.aspx

            // Get the subdirectories for the specified directory.
            DirectoryInfo dir = new DirectoryInfo(sourceDirName);

            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException(
                    "Source directory does not exist or could not be found: "
                    + sourceDirName);
            }

            DirectoryInfo[] dirs = dir.GetDirectories();
            // If the destination directory doesn't exist, create it.
            if (!Directory.Exists(destDirName))
            {
                Directory.CreateDirectory(destDirName);
            }

            // Get the files in the directory and copy them to the new location.
            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files)
            {
                string temppath = Path.Combine(destDirName, file.Name);
                file.CopyTo(temppath, false);
            }

            // If copying subdirectories, copy them and their contents to new location.
            if (copySubDirs)
            {
                foreach (DirectoryInfo subdir in dirs)
                {
                    string temppath = Path.Combine(destDirName, subdir.Name);
                    DirectoryCopy(subdir.FullName, temppath, copySubDirs);
                }
            }
        }

        //Copy a single file
        public void FileCopy(string InFile)
        {
            //As Infile is the path and filename, we need to seperate the file name out.
            string filename = Path.GetFileName(InFile);
            //Assemble dest path
            string Dest = CarsDataDir + "\\" + filename;
            // To copy a file to another location and 
            // overwrite the destination file if it already exists.
            System.IO.File.Copy(InFile, Dest, true);
        }

        //Overwrites or copies to new a single file
        public void OverwriteFile(string SourceFile, string DestFile)
        {
            // To copy a file to another location and 
            // overwrite the destination file if it already exists.
            System.IO.File.Copy(SourceFile, DestFile, true);
        }

        #region Engine stuff
        //Load and parse the engine data file
        public void LoadEngineDataFile()
        {
            //Loads the file that defines the engine stats

            //local to index through engine data array
            int EngineIndex = 0;

            //Check if the config file exists
            if (File.Exists(CarsDataDir + "\\engines.txt"))
            {
                //create a streamReader to accses the config file
                StreamReader reader = new StreamReader(CarsDataDir + "\\engines.txt");
                //string to hold a single line
                string line;
                //Create the array
                EnginesList = new EngineDataType[0];

                //loop through all of file a line at a time
                while (true)
                {
                    //Read a line from the file
                    line = reader.ReadLine();
                    //check if line is null
                    if (line == null)
                    {
                        break;  //exit loop if an empty line
                    }
                    else
                    {
                        //Check the line, if it starts with a [ read out the next six lines of engine data
                        if (line.StartsWith("["))
                        {
                            //Resize the array to one size bigger
                            Array.Resize(ref EnginesList, EnginesList.Length + 1);

                            //grab engine name
                            EnginesList[EngineIndex].name = line.Substring(1, line.Length - 2); //need to start with the line already read

                            //I'm using magic numbers for the start positions here, bad but saves a load of leg work counting out to the '='
                            //and if the engine file does change I'll probably need to change this anyway

                            //Read a line from the file so we can process it, allows for various number string lengths
                            line = reader.ReadLine();
                            //line = line.Substring(9, line.Length-9);   //Find the length and to trim the bit left-over minus the txet part
                            int.TryParse(line = line.Substring(9, line.Length - 9), out EnginesList[EngineIndex].maxPower);
                            line = reader.ReadLine();
                            line = line.Substring(12, line.Length - 12);   //Find the length and to trim the bit left-over minus the txet part
                            int.TryParse(line, out EnginesList[EngineIndex].maxPowerRPM);
                            line = reader.ReadLine();
                            line = line.Substring(13, line.Length - 13);   //Find the length and to trim the bit left-over minus the txet part
                            int.TryParse(line, out EnginesList[EngineIndex].maxTorqueRPM);
                            line = reader.ReadLine();
                            line = line.Substring(7, line.Length - 7);   //Find the length and to trim the bit left-over minus the txet part
                            int.TryParse(line, out EnginesList[EngineIndex].minRPM);
                            line = reader.ReadLine();
                            line = line.Substring(7, line.Length - 7);   //Find the length and to trim the bit left-over minus the txet part
                            int.TryParse(line, out EnginesList[EngineIndex].maxRPM);

                            EngineIndex++;  //Increment counter
                        }
                        else
                        {
                            string temp;
                            temp = line;
                        }
                    }
                }
                //we are finished with the reader so close and bin it
                reader.Close();
                reader.Dispose();
            }
        }

        //Write the engine data file
        public void WriteEngineDataFile()
        {
            //local to index through engine data array
            int Index = 0;

            //Create a local file writer
            StreamWriter writer = new StreamWriter(CarsDataDir + "\\engines.txt");
            
            //Write out each engine in turn
            while (Index < EnginesList.Length)
            {
                writer.WriteLine("[" + EnginesList[Index].name + "]");
                writer.WriteLine("maxPower=" + EnginesList[Index].maxPower);
                writer.WriteLine("maxPowerRPM=" + EnginesList[Index].maxPowerRPM);
                writer.WriteLine("maxTorqueRPM=" + EnginesList[Index].maxTorqueRPM);
                writer.WriteLine("minRPM=" + EnginesList[Index].minRPM);
                writer.WriteLine("maxRPM=" + EnginesList[Index].maxRPM);
                writer.WriteLine();     //Blank line seperator

                Index++;    //Increment counter
            }

            //we are finished with the writer so close and bin it
            writer.Close();
            writer.Dispose();
        }

        #region Getters and Setters

        public int GetEngineDataArraySize()
        {
            return EnginesList.Length;
        }

        public string GetEngineDataName(int index)
        {
            //check if slot is valid
            if (index <= EnginesList.Length)
            {
                return EnginesList[index].name; //return name
            }
            else
            {
                return "No such slot";          //return a warning if not valid
            }
        }

        public int GetEngineDataMaxPower(int index)
        {
            //check if slot is valid
            if (index <= EnginesList.Length)
            {
                return EnginesList[index].maxPower; //return maxPower
            }
            else
            {
                return 0;          //return a warning if not valid
            }
        }

        public int GetEngineDataMaxPowerRPM(int index)
        {
            //check if slot is valid
            if (index <= EnginesList.Length)
            {
                return EnginesList[index].maxPowerRPM; //return maxPowerRPM
            }
            else
            {
                return 0;          //return a warning if not valid
            }
        }

        public int GetEngineDataMaxTorqueRPM(int index)
        {
            //check if slot is valid
            if (index <= EnginesList.Length)
            {
                return EnginesList[index].maxTorqueRPM; //return maxTorqueRPM
            }
            else
            {
                return 0;          //return a warning if not valid
            }
        }

        public int GetEngineDataMinRPM(int index)
        {
            //check if slot is valid
            if (index <= EnginesList.Length)
            {
                return EnginesList[index].minRPM; //return minRPM
            }
            else
            {
                return 0;          //return a warning if not valid
            }
        }

        public int GetEngineDataMaxRPM(int index)
        {
            //check if slot is valid
            if (index <= EnginesList.Length)
            {
                return EnginesList[index].maxRPM; //return maxRPM
            }
            else
            {
                return 0;          //return a warning if not valid
            }
        }

        public void SetEngineDataMaxPower(int index, int value)
        {
            //check if slot is valid
            if (index <= EnginesList.Length)
            {
                EnginesList[index].maxPower = value; //set maxPower
            }
        }

        public void SetEngineDataMaxPowerRPM(int index, int value)
        {
            //check if slot is valid
            if (index <= EnginesList.Length)
            {
                EnginesList[index].maxPowerRPM = value; //set maxPowerRPM
            }
        }

        public void SetEngineDataMaxTorqueRPM(int index, int value)
        {
            //check if slot is valid
            if (index <= EnginesList.Length)
            {
                EnginesList[index].maxTorqueRPM = value; //set maxTorqueRPM
            }
        }

        public void SetEngineDataMinRPM(int index, int value)
        {
            //check if slot is valid
            if (index <= EnginesList.Length)
            {
                EnginesList[index].minRPM = value; //set minRPM
            }
        }

        public void SetEngineDataMaxRPM(int index, int value)
        {
            //check if slot is valid
            if (index <= EnginesList.Length)
            {
                EnginesList[index].maxRPM = value; //set maxRPM
            }
        }
        #endregion
        #endregion

        #region Car List stuff

        //Read and parse the contents of cars.txt
        public void ReadCarsCurrent()
        {
            //Loads the file that holds the list of cars currently in game

            //Create local single instance of type
            CarListType CLTtemp;
            //Empty out old data
            CarsCurrentList.Clear();

            //Check if the config file exists
            if (File.Exists(CarsDataDir + "//cars.txt"))
            {
                //create a streamReader to accses the config file
                StreamReader reader = new StreamReader(CarsDataDir + "//cars.txt");
                //string to hold a single line
                string line;

                //loop through all of file a line at a time
                while (true)
                {
                    //Read a line from the file
                    line = reader.ReadLine();
                    //check if line is null
                    if (line == null)
                    {
                        break;  //exit loop if an empty line
                    }
                    else
                    {
                        //Process line here
                        //Need the names sperating out
                        string[] words = line.Split(',');
                        CLTtemp.InternalName = words[0];
                        //Trim the leading space
                        string temp = words[1].Trim();
                        CLTtemp.ExternalName = temp;
                        CarsCurrentList.Add(CLTtemp);
                    }
                }

                //we are finished with the reader so close and bin it
                reader.Close();
                reader.Dispose();
            }
        }

        //Write the cars.txt file
        public void WriteCarsCurrent()
        {
            //local to index through engine data array
            int Index = 0;

            //Create a local file writer
            StreamWriter writer = new StreamWriter(CarsDataDir + "//cars.txt");

            //Write out each engine in turn
            while (Index < CarsCurrentList.Count)
            {
                writer.WriteLine(CarsCurrentList[Index].InternalName + ", " + CarsCurrentList[Index].ExternalName);

                Index++;    //Increment counter
            }

            //we are finished with the writer so close and bin it
            writer.Close();
            writer.Dispose();
        }

        //Assemble a  list of all car data files in the Datacars dir
        public void FindCarsFiles()
        {
            //Get a list of all the txt files
            string[] temp = Directory.GetFiles(CarsDataDir, "*.txt");
            //Empty out old data
            CarsAvailableFilesList.Clear();

            //We have the full path, so trim down to just the filenames
            for (int index = 0; index < temp.Length; index++)
            {
                //Grab the filename from the full path
                temp[index] = new FileInfo(temp[index]).Name;

                //We also have the cars.txt and engines.txt in the list, so we need to remove them
                if ((!(temp[index] == "cars.txt")) && (!(temp[index] == "engines.txt")))
                {
                    CarsAvailableFilesList.Add(temp[index]);
                }
            }
        }

        //Add a car to the current list
        public void AddToCurrent(string Internal, string External)
        {
            //Add to internal current list

            //Create local single instance of type
            CarListType CLTtemp;
            //Fill it out
            CLTtemp.InternalName = Internal;
            CLTtemp.ExternalName = External;

            //Add the local to the list
            CarsCurrentList.Add(CLTtemp);
        }

        //Remove a car from the current list
        public void RemoveFromCurrent(int Index)
        {
            //check if slot is valid
            if (Index <= CarsCurrentList.Count)
            {
                //Remove unwanted line from list
                CarsCurrentList.RemoveAt(Index);
            }
        }

        //Empty both lists
        public void EmptyCurrentAvailableCarLists()
        {
            CarsCurrentList.Clear();
            CarsAvailableFilesList.Clear();
        }

        #region Getters and Setters

        public int GetCarsCurrentArraySize()
        {
            return CarsCurrentList.Count;
        }

        public void SetCarsCurrentInternal(int index, string value)
        {
            //check if slot is valid
            if (index <= CarsCurrentList.Count)
            {
                //Create local single instance of type
                CarListType CLTtemp;
                //Fill it out
                CLTtemp.InternalName = value;
                CLTtemp.ExternalName = CarsCurrentList[index].ExternalName;
                //Replace the element
                CarsCurrentList[index] = CLTtemp;
            }
        }

        public void SetCarsCurrentExternal(int index, string value)
        {
            //check if slot is valid
            if (index <= CarsCurrentList.Count)
            {
                //Create local single instance of type
                CarListType CLTtemp;
                //Fill it out
                CLTtemp.InternalName = CarsCurrentList[index].InternalName;
                CLTtemp.ExternalName = value;
                //Replace the element
                CarsCurrentList[index] = CLTtemp;
            }
        }

        public string GetCarsCurrentInternal(int index)
        {
            //check if slot is valid
            if (index <= CarsCurrentList.Count)
            {
                return CarsCurrentList[index].InternalName; //Get internal name
            }
            else
            {
                return "Invalid";
            }
        }

        public string GetCarsCurrentExternal(int index)
        {
            //check if slot is valid
            if (index <= CarsCurrentList.Count)
            {
                return CarsCurrentList[index].ExternalName; //Get external name
            }
            else
            {
                return "Invalid";
            }
        }

        public int GetCarsAvailableArraySize()
        {
            return CarsAvailableFilesList.Count;
        }

        public string GetCarsAvailableFilesList(int index)
        {
            //check if slot is valid
            if (index <= CarsAvailableFilesList.Count)
            {
                return CarsAvailableFilesList[index]; //Get internal name
            }
            else
            {
                return "Invalid";
            }
        }

        public void SetCarsAvailableFilesList(int index, string value)
        {
            //check if slot is valid
            if (index <= CarsAvailableFilesList.Count)
            {
                CarsAvailableFilesList[index] = value; //Set internal name
            }
        }

        #endregion
        #endregion

        #region Car Data stuff

        //Load and parse the car data file
        public void LoadCarDataFromFile(string filename)
        {
            //Loads the contents of a car data file and stores it in LoadedCarData
            //Labels in the strut are the same as the file
            //All labels may not be present in the file
            //Labels may not be in the same order
            //Lines that start ; (a semi-colon) a comments that can be ignored (bar the name line I'm adding)

            //Will need to inspect each line to see if it's the start of a section and what it is if not.

            //Check if the config file exists
            if (File.Exists(CarsDataDir + "//" + filename))
            {
                //create a streamReader to accses the config file
                StreamReader reader = new StreamReader(CarsDataDir + "//" + filename);
                //string to hold a single line
                string line;

                //Clear out the old data
                CarDataSectionClearAll();

                //loop through all of file a line at a time
                while (true)
                {
                    //Read a line from the file
                    line = reader.ReadLine();
                    //check if line is null
                    if (line == null)
                    {
                        break;  //exit loop if an empty line
                    }
                    else
                    {
                        //Process line here
                        //Need the names sperating out
                        //string[] words = line.Split(',');
                        //Trim the leading space
                        //string temp = words[1].Trim();

                        //Check what the string starts with
                        //[ for a section
                        if (line.StartsWith("["))
                        {
                            //We have a line with new section, so ID the section
                            line = line.Substring(1, line.Length - 2);
                            line = line.Trim(' ');                          //Remove the leading on trailing spaces

                            switch (line)
                            {
                                case "main":  //Main data section
                                    CarDataMainSectionProc(reader);     //Special case ;name                              
                                    break;
                                case "other":
                                    CarDataOtherSectionProc(reader);
                                    break;
                                case "suspension":
                                    CarDataSuspensionSectionProc(reader);
                                    break;
                                case "engine":
                                    CarDataEngineSectionProc(reader);
                                    break;
                                case "driveshaft":
                                    CarDataDriveshaftSectionProc(reader);
                                    break;
                                case "wheels":
                                    CarDataWheelsSectionProc(reader);
                                    break;
                                case "interior":
                                    CarDataInteriorSectionProc(reader);
                                    break;
                                case "logic":
                                    CarDataLogicSectionProc(reader);
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
                                        CarDataPartsSectionProc(reader);
                                    }
                                    break;
                            }
                        }
                        else
                        {
                            //Blank line or comment probably
                        }

                    }
                }

                //we are finished with the reader so close and bin it
                reader.Close();
                reader.Dispose();
            }
        }

        //Write the car data to file
        public void WriteCarDataToFile(string filename)
        {
            //local to index through engine data array
            int Index = 0;

            //Create a local file writer
            StreamWriter writer = new StreamWriter(CarsDataDir + "\\" + filename);

                writer.WriteLine("[main]");
                writer.WriteLine(";name= " + LoadedCarData.Main.name);
                writer.WriteLine("model= " + LoadedCarData.Main.model);
                writer.WriteLine("rustMask= " + LoadedCarData.Main.rustMask);
                writer.WriteLine("rotation= " + LoadedCarData.Main.rotation.x + "," + LoadedCarData.Main.rotation.y + "," + LoadedCarData.Main.rotation.z);
                writer.WriteLine();     //Blank line seperator

                writer.WriteLine("[other]");
                writer.WriteLine("engineSound= " + LoadedCarData.Other.engineSound);
                writer.WriteLine("transmissionType= " + LoadedCarData.Other.transmissionType);
                writer.WriteLine("gears= " + LoadedCarData.Other.gears);
                writer.WriteLine("finalDriveRatio= " + LoadedCarData.Other.finalDriveRatio);
                writer.WriteLine("weight= " + LoadedCarData.Other.weight);
                writer.WriteLine("rpmFactor= " + LoadedCarData.Other.rpmFactor);
                writer.WriteLine("rpmAngle= " + LoadedCarData.Other.rpmAngle);
                writer.WriteLine("speedoAngle= " + LoadedCarData.Other.speedoAngle);
                writer.WriteLine("suspTravel= " + LoadedCarData.Other.suspTravel);
                writer.WriteLine("lifterArmsRise= " + LoadedCarData.Other.lifterArmsRise);
                writer.WriteLine("cx= " + LoadedCarData.Other.cx);
                writer.WriteLine();     //Blank line seperator

                writer.WriteLine("[suspension]");
                writer.WriteLine("frontAxleStart= " + LoadedCarData.Suspension.frontAxleStart);
                writer.WriteLine("wheelBase= " + LoadedCarData.Suspension.wheelBase);
                writer.WriteLine("height= " + LoadedCarData.Suspension.height);
                writer.WriteLine("frontTrack= " + LoadedCarData.Suspension.frontTrack);
                writer.WriteLine("rearTrack= " + LoadedCarData.Suspension.rearTrack);
                writer.WriteLine("frontSpringLength= " + LoadedCarData.Suspension.frontSpringLength);
                writer.WriteLine("scale= " + LoadedCarData.Suspension.scale);
                writer.WriteLine();     //Blank line seperator
                writer.WriteLine("frontCenterSet= " + LoadedCarData.Suspension.frontCenterSet);
                writer.WriteLine("frontRightSet= " + LoadedCarData.Suspension.frontRightSet);
                writer.WriteLine("rearCenterSet= " + LoadedCarData.Suspension.rearCenterSet);
                writer.WriteLine("rearRightSet= " + LoadedCarData.Suspension.rearRightSet);
                writer.WriteLine();     //Blank line seperator

                writer.WriteLine("[engine]");
                writer.WriteLine("position= " + LoadedCarData.Engine.position.x + "," + LoadedCarData.Engine.position.y + "," + LoadedCarData.Engine.position.z);
                writer.WriteLine("rotation= " + LoadedCarData.Engine.rotation.x + "," + LoadedCarData.Engine.rotation.y + "," + LoadedCarData.Engine.rotation.z);
                writer.WriteLine("scale= " + LoadedCarData.Engine.scale);
                writer.WriteLine("type= " + LoadedCarData.Engine.type);
                writer.WriteLine("sound= " + LoadedCarData.Engine.sound);
                writer.WriteLine();     //Blank line seperator

                writer.WriteLine("[driveshaft]");
                writer.WriteLine("position= " + LoadedCarData.Driveshaft.position.x + "," + LoadedCarData.Driveshaft.position.y + "," + LoadedCarData.Driveshaft.position.z);
                writer.WriteLine("rotation= " + LoadedCarData.Driveshaft.rotation.x + "," + LoadedCarData.Driveshaft.rotation.y + "," + LoadedCarData.Driveshaft.rotation.z);
                writer.WriteLine("scale= " + LoadedCarData.Driveshaft.scale);
                writer.WriteLine("type= " + LoadedCarData.Driveshaft.type);
                writer.WriteLine("length= " + LoadedCarData.Driveshaft.length);
                writer.WriteLine();     //Blank line seperator

                writer.WriteLine("[wheels]");
                writer.WriteLine("wheelWidth= " + LoadedCarData.Wheels.wheelWidth);
                writer.WriteLine("tire= " + LoadedCarData.Wheels.tire);
                writer.WriteLine("rim= " + LoadedCarData.Wheels.rim);
                writer.WriteLine("rimSize= " + LoadedCarData.Wheels.rimSize);
                writer.WriteLine("tireSize= " + LoadedCarData.Wheels.tireSize);
                writer.WriteLine();     //Blank line seperator

                //
                //PARTS HERE
                int LocalCounter = 0;
                while (LocalCounter < PartsCounter)
                {
                    writer.WriteLine("[parts" + LocalCounter + "]");
                    writer.WriteLine("name= " + LoadedCarData.Parts[LocalCounter].name);
                    writer.WriteLine("position= " + LoadedCarData.Parts[LocalCounter].position.x + "," + LoadedCarData.Parts[LocalCounter].position.y + "," + LoadedCarData.Parts[LocalCounter].position.z);
                    writer.WriteLine("rotation= " + LoadedCarData.Parts[LocalCounter].rotation.x + "," + LoadedCarData.Parts[LocalCounter].rotation.y + "," + LoadedCarData.Parts[LocalCounter].rotation.z);
                    writer.WriteLine("scale= " + LoadedCarData.Parts[LocalCounter].scale);
                    writer.WriteLine();     //Blank line seperator
                    LocalCounter ++;
                }

                writer.WriteLine("[interior]");
                writer.WriteLine("seatLeftPos= " + LoadedCarData.Interior.seatLeftPos.x + "," + LoadedCarData.Interior.seatLeftPos.y + "," + LoadedCarData.Interior.seatLeftPos.z);
                writer.WriteLine("seatLeftRot= " + LoadedCarData.Interior.seatLeftRot.x + "," + LoadedCarData.Interior.seatLeftRot.y + "," + LoadedCarData.Interior.seatLeftRot.z);
                writer.WriteLine("seatScale= " + LoadedCarData.Interior.seatScale);
                writer.WriteLine("seat= " + LoadedCarData.Interior.seat);
                writer.WriteLine("wheel= " + LoadedCarData.Interior.wheel);
                writer.WriteLine("wheelPos= " + LoadedCarData.Interior.wheelPos.x + "," + LoadedCarData.Interior.wheelPos.y + "," + LoadedCarData.Interior.wheelPos.z);
                writer.WriteLine("wheelRot= " + LoadedCarData.Interior.wheelRot.x + "," + LoadedCarData.Interior.wheelRot.y + "," + LoadedCarData.Interior.wheelRot.z);
                writer.WriteLine();     //Blank line seperator

                writer.WriteLine("[logic]");
                writer.WriteLine("globalCondition= " + LoadedCarData.Logic.globalCondition.a + "," + LoadedCarData.Logic.globalCondition.b);
                writer.WriteLine("partsConditions= " + LoadedCarData.Logic.partsConditions.a + "," + LoadedCarData.Logic.partsConditions.b);
                writer.WriteLine("panelsConditions= " + LoadedCarData.Logic.panelsConditions.a + "," + LoadedCarData.Logic.panelsConditions.b);
                writer.WriteLine("uniqueMod= " + LoadedCarData.Logic.uniqueMod);
                writer.WriteLine("blockOBD= " + LoadedCarData.Logic.blockOBD);

                Index++;    //Increment counter

            //we are finished with the writer so close and bin it
            writer.Close();
            writer.Dispose();
        }

        //Parse the  [main] part of the car data file
        public void CarDataMainSectionProc(StreamReader reader)
        {
            bool mainsection = true;    //Control the loop below

            while (mainsection)         //Loop through section
            {
                string line = reader.ReadLine();   //Grab the next line
                if (!(line.StartsWith(";")))
                {
                    //not a comment, so check the line
                    if (line != "")     //if the line is empty skip over all this
                    {
                        int i = line.IndexOf('=');              //Find the end of label string
                        string label = line.Substring(0, i);    //Grabs the bit upto the '='
                        line = line.Substring(i + 1, line.Length - (i + 1));    //Grab the bit after the '='
                        line = line.Trim(' ');                                         //Remove the leading on trailing spaces

                        switch (label)  //Fill out the Main data
                        {
                            case "model":
                                LoadedCarData.Main.model = line;
                                break;
                            case "rustMask":
                                LoadedCarData.Main.rustMask = line;
                                break;
                            case "rotation":
                                string[] words = line.Split(',');                       //Split the data out
                                                                                        //convert the strings to numbers
                                float.TryParse(words[0], out LoadedCarData.Main.rotation.x);
                                float.TryParse(words[1], out LoadedCarData.Main.rotation.y);
                                float.TryParse(words[2], out LoadedCarData.Main.rotation.z);
                                break;
                            default:
                                //Assume a blank line, and exit
                                mainsection = false;
                                break;
                        }
                    }
                    else
                    {
                        //Empty line force exit
                        mainsection = false;
                    }
                }
                else
                {
                    //skip comment lines
                    //need to check  for the ;name for my line (if in main section)
                    if (line.StartsWith(";name"))
                    {
                        int i = line.IndexOf('=');                      //Find the end of label
                        line = line.Substring(i + 1, line.Length - (i + 1));   //Grab the bit after the '='
                        line = line.Trim(' ');                                 //Remove the leading on trailing spaces
                        LoadedCarData.Main.name = line;
                    }
                }
            }
        }

        //Empty the  [main] part of the car data file
        public void CarDataMainSectionProcClear()
        {
            LoadedCarData.Main.name = "";
            LoadedCarData.Main.model = "";
            LoadedCarData.Main.rustMask = "";
            LoadedCarData.Main.rotation.x = 0;
            LoadedCarData.Main.rotation.y = 0;
            LoadedCarData.Main.rotation.z = 0;
        }

        //Parse the [other] part of the car data file
        public void CarDataOtherSectionProc(StreamReader reader)
        {
            bool othersection = true;    //Control the loop below

            while (othersection)         //Loop through section
            {
                string line = reader.ReadLine();   //Grab the next line
                if (!(line.StartsWith(";")))
                {
                    //not a comment, so check the line
                    if ((line != "") && (line.Length > 3))     //if the line is empty skip over all this (extra check on length to skip that line with a space on)
                    {
                        int i = line.IndexOf('=');              //Find the end of label string
                        string label = line.Substring(0, i);    //Grabs the bit upto the '='
                        line = line.Substring(i + 1, line.Length - (i + 1));    //Grab the bit after the '='
                        line = line.Trim(' ');                                  //Remove the leading on trailing spaces'='

                        switch (label)  //Fill out the Main data
                        {
                            case "engineSound":
                                LoadedCarData.Other.engineSound = line;
                                break;
                            case "transmissionType":
                                LoadedCarData.Other.transmissionType = line;
                                break;
                            case "gears":
                                int.TryParse(line, out LoadedCarData.Other.gears);      //convert the strings to numbers
                                break;
                            case "finalDriveRatio":
                                float.TryParse(line, out LoadedCarData.Other.finalDriveRatio);  //convert the strings to numbers
                                break;
                            case "weight":
                                int.TryParse(line, out LoadedCarData.Other.weight);     //convert the strings to numbers
                                break;
                            case "rpmFactor":
                                float.TryParse(line, out LoadedCarData.Other.rpmFactor);//convert the strings to numbers
                                break;
                            case "rpmAngle":
                                int.TryParse(line, out LoadedCarData.Other.rpmAngle);   //convert the strings to numbers
                                break;
                            case "speedoAngle":
                                float.TryParse(line, out LoadedCarData.Other.speedoAngle);   //convert the strings to numbers
                                break;
                            case "suspTravel":
                                float.TryParse(line, out LoadedCarData.Other.suspTravel);   //convert the strings to numbers
                                break;
                            case "lifterArmsRise":
                                float.TryParse(line, out LoadedCarData.Other.lifterArmsRise);//convert the strings to numbers
                                break;
                            case "cx":
                                float.TryParse(line, out LoadedCarData.Other.cx);//convert the strings to numbers
                                break;
                            default:
                                //Assume a blank line, and exit
                                othersection = false;
                                break;
                        }
                    }
                    else
                    {
                        //Empty line force exit
                        othersection = false;
                    }
                }
                else
                {
                    //skip comment lines
                }
            }
        }

        //Empty the [other] part of the car data file
        public void CarDataOtherSectionProcClear()
        {
            LoadedCarData.Other.engineSound = "";
            LoadedCarData.Other.transmissionType = "";
            LoadedCarData.Other.gears = 0;
            LoadedCarData.Other.finalDriveRatio = 0;
            LoadedCarData.Other.weight = 0;
            LoadedCarData.Other.rpmFactor = 0;
            LoadedCarData.Other.rpmAngle = 0;
            LoadedCarData.Other.speedoAngle = 0;
            LoadedCarData.Other.suspTravel = 0;
            LoadedCarData.Other.lifterArmsRise = 0;
            LoadedCarData.Other.cx = 0;
        }

        //Parse the [suspension] part of the car data file
        public void CarDataSuspensionSectionProc(StreamReader reader)
        {
            bool suspensionsection = true;  //Control the loop below
            bool skipread = false;          //skip line read? special case controller
            bool exittoggle = false;        //Controls if we eat blank lines or exit upon them
            string line = null;             //Local string to hold line being processed

            while (suspensionsection)         //Loop through section
            {
                if (skipread == false)  //Skip line if we want to reuse the line after getting through the empty lines
                {
                    line = reader.ReadLine();   //Grab the next line
                }
                else
                {
                    skipread = false;   //Reset skip line bool
                }
                if (!(line.StartsWith(";")))
                {
                    //not a comment, so check the line
                    if (line != "")     //if the line is empty skip over all this
                    {
                        int i = line.IndexOf('=');              //Find the end of label string
                        string label = line.Substring(0, i);    //Grabs the bit upto the '='
                        line = line.Substring(i + 1, line.Length - (i + 1));    //Grab the bit after the '='
                        line = line.Trim(' ');                                  //Remove the leading on trailing spaces'='

                        switch (label)  //Fill out the Main data
                        {
                            case "frontAxleStart":
                                float.TryParse(line, out LoadedCarData.Suspension.frontAxleStart);  //convert the strings to numbers
                                break;
                            case "wheelBase":
                                float.TryParse(line, out LoadedCarData.Suspension.wheelBase);   //convert the strings to numbers
                                break;
                            case "height":
                                float.TryParse(line, out LoadedCarData.Suspension.height);      //convert the strings to numbers
                                break;
                            case "frontTrack":
                                float.TryParse(line, out LoadedCarData.Suspension.frontTrack);  //convert the strings to numbers
                                break;
                            case "rearTrack":
                                float.TryParse(line, out LoadedCarData.Suspension.rearTrack);   //convert the strings to numbers
                                break;
                            case "frontSpringLength":
                                float.TryParse(line, out LoadedCarData.Suspension.frontSpringLength);   //convert the strings to numbers
                                break;
                            case "scale":
                                float.TryParse(line, out LoadedCarData.Suspension.scale);   //convert the strings to numbers
                                break;
                            case "frontCenterSet":
                                LoadedCarData.Suspension.frontCenterSet = line;
                                break;
                            case "frontRightSet":
                                LoadedCarData.Suspension.frontRightSet = line;
                                break;
                            case "rearCenterSet":
                                LoadedCarData.Suspension.rearCenterSet = line;
                                break;
                            case "rearRightSet":
                                LoadedCarData.Suspension.rearRightSet = line;
                                break;
                            default:
                                //Assume a blank line, and exit
                                suspensionsection = false;
                                break;
                        }
                    }
                    else
                    {
                        //Empty line, would normally force an exit, but in the suspension section we need to allow for blank lines
                        //Problem is I need to know when to stop and return for the next section.
                        //I could read the next line, but that 'uses up' that line, meaning when I return to the top level I'm missing the section title I need

                        //It's hacky, but I'm going to assume only a single group of empty lines, read through them, and exit as normal on the second empty line group
                        //I'll use a bool to signal if it's the first or second group, and if to reuse a read line (instead or getting the next line)

                        //Here with a null line
                        if (!exittoggle)    //If first bunch of null/empty lines
                        {
                            while (line == "")   //Keep eating lines until no longer null
                            {
                                line = reader.ReadLine();   //Grab the next line
                            }
                            //Line is longer null
                            skipread = true;    //Set skipread true
                                                //Next readline, with not get a line but will use the value read here
                            exittoggle = true;  //Set exit toggle
                                                //Next blank line cause this to exit
                        }
                        else
                        {   //Second bunch of null/empty lines so exit
                            suspensionsection = false;
                        }                        
                    }
                }
                else
                {
                    //skip comment lines
                }
            }
        }

        //Empty the [suspension] part of the car data file
        public void CarDataSuspensionSectionProcClear()
        {
            LoadedCarData.Suspension.frontAxleStart = 0;
            LoadedCarData.Suspension.wheelBase = 0;
            LoadedCarData.Suspension.height = 0;
            LoadedCarData.Suspension.frontTrack = 0;
            LoadedCarData.Suspension.rearTrack = 0;
            LoadedCarData.Suspension.frontSpringLength = 0;
            LoadedCarData.Suspension.scale = 0;
            LoadedCarData.Suspension.frontCenterSet = "";
            LoadedCarData.Suspension.frontRightSet = "";
            LoadedCarData.Suspension.rearCenterSet = "";
            LoadedCarData.Suspension.rearRightSet = "";
        }

        //Parse the  [engine] part of the car data file
        public void CarDataEngineSectionProc(StreamReader reader)
        {
            bool enginesection = true;  //Control the loop below
            string[] words;             //Local array used to split out values

            while (enginesection)         //Loop through section
            {
                string line = reader.ReadLine();   //Grab the next line
                if (!(line.StartsWith(";")))
                {
                    //not a comment, so check the line
                    if (line != "")     //if the line is empty skip over all this
                    {
                        int i = line.IndexOf('=');              //Find the end of label string
                        string label = line.Substring(0, i);    //Grabs the bit upto the '='
                        line = line.Substring(i + 1, line.Length - (i + 1));    //Grab the bit after the '='
                        line = line.Trim(' ');                                  //Remove the leading on trailing spaces

                        switch (label)  //Fill out the Main data
                        {
                            case "position":
                                words = line.Split(',');                                //Split the data out
                                                                                        //convert the strings to numbers
                                float.TryParse(words[0], out LoadedCarData.Engine.position.x);
                                float.TryParse(words[1], out LoadedCarData.Engine.position.y);
                                float.TryParse(words[2], out LoadedCarData.Engine.position.z);
                                break;
                            case "rotation":
                                words = line.Split(',');                                //Split the data out
                                                                                        //convert the strings to numbers
                                float.TryParse(words[0], out LoadedCarData.Engine.rotation.x);
                                float.TryParse(words[1], out LoadedCarData.Engine.rotation.y);
                                float.TryParse(words[2], out LoadedCarData.Engine.rotation.z);
                                break;
                            case "scale":
                                float.TryParse(line, out LoadedCarData.Engine.scale);   //convert the strings to numbers
                                break;
                            case "type":
                                LoadedCarData.Engine.type = line;
                                break;
                            case "sound":
                                LoadedCarData.Engine.type = line;
                                break;
                            default:
                                //Assume a blank line, and exit
                                enginesection = false;
                                break;
                        }
                    }
                    else
                    {
                        //Empty line force exit
                        enginesection = false;
                    }
                }
                else
                {
                    //skip comment lines
                }
            }
        }

        //Empty the  [engine] part of the car data file
        public void CarDataEngineSectionProcClear()
        {
            LoadedCarData.Engine.position.x = 0;
            LoadedCarData.Engine.position.y = 0;
            LoadedCarData.Engine.position.z = 0;
            LoadedCarData.Engine.rotation.x = 0;
            LoadedCarData.Engine.rotation.y = 0;
            LoadedCarData.Engine.rotation.z = 0;
            LoadedCarData.Engine.scale = 0;
            LoadedCarData.Engine.type = "";
            LoadedCarData.Engine.type = "";
        }

        //Parse the  [driveshaft] part of the car data file
        public void CarDataDriveshaftSectionProc(StreamReader reader)
        {
            bool driveshaftsection = true;  //Control the loop below
            string[] words;             //Local array used to split out values

            while (driveshaftsection)         //Loop through section
            {
                string line = reader.ReadLine();   //Grab the next line
                if (!(line.StartsWith(";")))
                {
                    //not a comment, so check the line
                    if (line != "")     //if the line is empty skip over all this
                    {
                        int i = line.IndexOf('=');              //Find the end of label string
                        string label = line.Substring(0, i);    //Grabs the bit upto the '='
                        line = line.Substring(i + 1, line.Length - (i + 1));    //Grab the bit after the '='
                        line = line.Trim(' ');                                  //Remove the leading on trailing spaces

                        switch (label)  //Fill out the Main data
                        {
                            case "position":
                                words = line.Split(',');                                //Split the data out
                                                                                        //convert the strings to numbers
                                float.TryParse(words[0], out LoadedCarData.Driveshaft.position.x);
                                float.TryParse(words[1], out LoadedCarData.Driveshaft.position.y);
                                float.TryParse(words[2], out LoadedCarData.Driveshaft.position.z);
                                break;
                            case "rotation":
                                words = line.Split(',');                                //Split the data out
                                                                                        //convert the strings to numbers
                                float.TryParse(words[0], out LoadedCarData.Driveshaft.rotation.x);
                                float.TryParse(words[1], out LoadedCarData.Driveshaft.rotation.y);
                                float.TryParse(words[2], out LoadedCarData.Driveshaft.rotation.z);
                                break;
                            case "scale":
                                float.TryParse(line, out LoadedCarData.Driveshaft.scale);   //convert the strings to numbers
                                break;
                            case "type":
                                LoadedCarData.Driveshaft.type = line;
                                break;
                            case "length":
                                float.TryParse(line, out LoadedCarData.Driveshaft.length);   //convert the strings to numbers
                                break;
                            default:
                                //Assume a blank line, and exit
                                driveshaftsection = false;
                                break;
                        }
                    }
                    else
                    {
                        //Empty line force exit
                        driveshaftsection = false;
                    }
                }
                else
                {
                    //skip comment lines
                }
            }
        }

        //Empty the  [driveshaft] part of the car data file
        public void CarDataDriveshaftSectionProcClear()
        {
            LoadedCarData.Driveshaft.position.x = 0;
            LoadedCarData.Driveshaft.position.y = 0;
            LoadedCarData.Driveshaft.position.z = 0;
            LoadedCarData.Driveshaft.rotation.x = 0;
            LoadedCarData.Driveshaft.rotation.y = 0;
            LoadedCarData.Driveshaft.rotation.z = 0;
            LoadedCarData.Driveshaft.scale = 0;
            LoadedCarData.Driveshaft.type = "";
            LoadedCarData.Driveshaft.length = 0;
        }

        //Parse the  [wheels] part of the car data file
        public void CarDataWheelsSectionProc(StreamReader reader)
        {
            bool wheelssection = true;  //Control the loop below

            while (wheelssection)         //Loop through section
            {
                string line = reader.ReadLine();   //Grab the next line
                if (!(line.StartsWith(";")))
                {
                    //not a comment, so check the line
                    if (line != "")     //if the line is empty skip over all this
                    {
                        int i = line.IndexOf('=');              //Find the end of label string
                        string label = line.Substring(0, i);    //Grabs the bit upto the '='
                        line = line.Substring(i + 1, line.Length - (i + 1));    //Grab the bit after the '='
                        line = line.Trim(' ');                                  //Remove the leading on trailing spaces

                        switch (label)  //Fill out the Main data
                        {
                            case "wheelWidth":
                                int.TryParse(line, out LoadedCarData.Wheels.wheelWidth);   //convert the strings to numbers
                                break;
                            case "tire":
                                LoadedCarData.Wheels.tire = line;
                                break;
                            case "rim":
                                LoadedCarData.Wheels.rim = line;
                                break;
                            case "rimSize":
                                int.TryParse(line, out LoadedCarData.Wheels.rimSize);   //convert the strings to numbers
                                break;
                            case "tireSize":
                                int.TryParse(line, out LoadedCarData.Wheels.tireSize);   //convert the strings to numbers
                                break;
                            default:
                                //Assume a blank line, and exit
                                wheelssection = false;
                                break;
                        }
                    }
                    else
                    {
                        //Empty line force exit
                        wheelssection = false;
                    }
                }
                else
                {
                    //skip comment lines
                }
            }
        }

        //Empty the  [wheels] part of the car data file
        public void CarDataWheelsSectionProcClear()
        {
            LoadedCarData.Wheels.wheelWidth = 0;
            LoadedCarData.Wheels.tire = "";
            LoadedCarData.Wheels.rim = "";
            LoadedCarData.Wheels.rimSize = 0;
            LoadedCarData.Wheels.tireSize = 0;
        }

        //Parse the [Parts<num>] part of the car data file
        public void CarDataPartsSectionProc(StreamReader reader)
        {
            bool partssection = true;   //Control the loop below
            string[] words;             //Local array used to split out values
            CarPartsType LocalParts;    //Local data struct to fill out and add to overall data collection

            //I have to init the parts, as the complier things it be will uninit later
            //This clears out the old data, but only for the in-use part of the list
            LocalParts.name = "N";
            LocalParts.position.x = 0;
            LocalParts.position.y = 0;
            LocalParts.position.z = 0;
            LocalParts.rotation.x = 0;
            LocalParts.rotation.y = 0;
            LocalParts.rotation.z = 0;
            LocalParts.scale = 0;

            while (partssection)        //Loop through section
            {
                string line = reader.ReadLine();   //Grab the next line
                if (!(line.StartsWith(";")))
                {
                    //not a comment, so check the line
                    if (line != "")     //if the line is empty skip over all this
                    {
                        int i = line.IndexOf('=');              //Find the end of label string
                        string label = line.Substring(0, i);    //Grabs the bit upto the '='
                        line = line.Substring(i + 1, line.Length - (i + 1));    //Grab the bit after the '='
                        line = line.Trim(' ');                                  //Remove the leading on trailing spaces

                        switch (label)  //Fill out the Main data
                        {
                            case "name":
                                LocalParts.name = line;
                                break;
                            case "position":
                                words = line.Split(',');                                //Split the data out
                                                                                        //convert the strings to numbers
                                float.TryParse(words[0], out LocalParts.position.x);
                                float.TryParse(words[1], out LocalParts.position.y);
                                float.TryParse(words[2], out LocalParts.position.z);
                                break;
                            case "rotation":
                                words = line.Split(',');                                //Split the data out
                                                                                        //convert the strings to numbers
                                float.TryParse(words[0], out LocalParts.rotation.x);
                                float.TryParse(words[1], out LocalParts.rotation.y);
                                float.TryParse(words[2], out LocalParts.rotation.z);
                                break;
                            case "scale":
                                float.TryParse(line, out LocalParts.scale);   //convert the strings to numbers
                                break;
                            default:
                                //Assume a blank line, and exit
                                partssection = false;
                                break;
                        }
                    }
                    else
                    {
                        //Empty line force exit
                        partssection = false;
                    }
                }
                else
                {
                    //skip comment lines
                }
            }

            //Assume we have all the data, so add the local to the collection list
            LoadedCarData.Parts.Add(LocalParts);
            //Update the counter
            PartsCounter++;
        }

        //Empty the [Parts<num>] part of the car data file
        public void CarDataPartsSectionProcClear()
        {
            LoadedCarData.Parts.Clear();
            PartsCounter = 0;
        }

        //Parse the  [interior] part of the car data file
        public void CarDataInteriorSectionProc(StreamReader reader)
        {
            bool interiorsection = true;    //Control the loop below
            string[] words;                 //Local array used to split out values

            while (interiorsection)         //Loop through section
            {
                string line = reader.ReadLine();   //Grab the next line
                if (!(line.StartsWith(";")))
                {
                    //not a comment, so check the line
                    if (line != "")     //if the line is empty skip over all this
                    {
                        int i = line.IndexOf('=');              //Find the end of label string
                        string label = line.Substring(0, i);    //Grabs the bit upto the '='
                        line = line.Substring(i + 1, line.Length - (i + 1));    //Grab the bit after the '='
                        line = line.Trim(' ');                                  //Remove the leading on trailing spaces

                        switch (label)  //Fill out the Interior data
                        {
                            case "seatLeftPos":
                                words = line.Split(',');                                //Split the data out
                                                                                        //convert the strings to numbers
                                float.TryParse(words[0], out LoadedCarData.Interior.seatLeftPos.x);
                                float.TryParse(words[1], out LoadedCarData.Interior.seatLeftPos.y);
                                float.TryParse(words[2], out LoadedCarData.Interior.seatLeftPos.z);
                                break;
                            case "seatLeftRot":
                                words = line.Split(',');                                //Split the data out
                                                                                        //convert the strings to numbers
                                float.TryParse(words[0], out LoadedCarData.Interior.seatLeftRot.x);
                                float.TryParse(words[1], out LoadedCarData.Interior.seatLeftRot.y);
                                float.TryParse(words[2], out LoadedCarData.Interior.seatLeftRot.z);
                                break;
                            case "seatScale":
                                float.TryParse(line, out LoadedCarData.Interior.seatScale);   //convert the strings to numbers
                                break;
                            case "seat":
                                LoadedCarData.Interior.seat = line;
                                break;
                            case "wheel":
                                LoadedCarData.Interior.wheel = line;
                                break;
                            case "wheelPos":
                                words = line.Split(',');                                //Split the data out
                                                                                        //convert the strings to numbers
                                float.TryParse(words[0], out LoadedCarData.Interior.wheelPos.x);
                                float.TryParse(words[1], out LoadedCarData.Interior.wheelPos.y);
                                float.TryParse(words[2], out LoadedCarData.Interior.wheelPos.z);
                                break;
                            case "wheelRot":
                                words = line.Split(',');                                //Split the data out
                                                                                        //convert the strings to numbers
                                float.TryParse(words[0], out LoadedCarData.Interior.wheelRot.x);
                                float.TryParse(words[1], out LoadedCarData.Interior.wheelRot.y);
                                float.TryParse(words[2], out LoadedCarData.Interior.wheelRot.z);
                                break;
                            default:
                                //Assume a blank line, and exit
                                interiorsection = false;
                                break;
                        }
                    }
                    else
                    {
                        //Empty line force exit
                        interiorsection = false;
                    }
                }
                else
                {
                    //skip comment lines
                }
            }
        }

        //Empty the  [interior] part of the car data file
        public void CarDataInteriorSectionProcClear()
        {
            LoadedCarData.Interior.seatLeftPos.x = 0;
            LoadedCarData.Interior.seatLeftPos.y = 0;
            LoadedCarData.Interior.seatLeftPos.z = 0;
            LoadedCarData.Interior.seatLeftRot.x = 0;
            LoadedCarData.Interior.seatLeftRot.y = 0;
            LoadedCarData.Interior.seatLeftRot.z = 0;
            LoadedCarData.Interior.seatScale = 0;
            LoadedCarData.Interior.seat = "";
            LoadedCarData.Interior.wheel = "";
            LoadedCarData.Interior.wheelPos.x = 0;
            LoadedCarData.Interior.wheelPos.y = 0;
            LoadedCarData.Interior.wheelPos.z = 0;
            LoadedCarData.Interior.wheelRot.x = 0;
            LoadedCarData.Interior.wheelRot.y = 0;
            LoadedCarData.Interior.wheelRot.z = 0;
        }

        //Parse the  [logic] part of the car data file
        public void CarDataLogicSectionProc(StreamReader reader)
        {
            bool logicsection = true;  //Control the loop below
            string[] words;             //Local array used to split out values

            while (logicsection)         //Loop through section
            {
                string line = reader.ReadLine();   //Grab the next line

                if ((line != "") && (line != null))    //if the line is empty skip over all this
                {
                    if (!(line.StartsWith(";")))    //not a comment, so check the line
                    {
                        int i = line.IndexOf('=');              //Find the end of label string
                        string label = line.Substring(0, i);    //Grabs the bit upto the '='
                        line = line.Substring(i + 1, line.Length - (i + 1));    //Grab the bit after the '='
                        line = line.Trim(' ');                                  //Remove the leading or trailing spaces

                        switch (label)  //Fill out the Main data
                        {
                            case "globalCondition":
                                words = line.Split(',');                                //Split the data out
                                                                                        //convert the strings to numbers
                                float.TryParse(words[0], out LoadedCarData.Logic.globalCondition.a);
                                float.TryParse(words[1], out LoadedCarData.Logic.globalCondition.b);
                                break;
                            case "partsConditions":
                                words = line.Split(',');                                //Split the data out
                                                                                        //convert the strings to numbers
                                float.TryParse(words[0], out LoadedCarData.Logic.partsConditions.a);
                                float.TryParse(words[1], out LoadedCarData.Logic.partsConditions.b);
                                break;
                            case "panelsConditions":
                                words = line.Split(',');                                //Split the data out
                                                                                        //convert the strings to numbers
                                float.TryParse(words[0], out LoadedCarData.Logic.panelsConditions.a);
                                float.TryParse(words[1], out LoadedCarData.Logic.panelsConditions.b);
                                break;
                            case "uniqueMod":
                                float.TryParse(line, out LoadedCarData.Logic.uniqueMod);   //convert the strings to numbers
                                break;
                            case "blockOBD":
                                bool.TryParse(line, out LoadedCarData.Logic.blockOBD);   //convert the strings to numbers
                                break;
                            default:
                                //Assume a blank line, and exit
                                logicsection = false;
                                break;
                        }
                    }
                    else
                    {
                        //skip comment lines
                    }
                }
                else
                {
                    //Empty line force exit
                    logicsection = false;
                }
            }
        }

        //Empty the  [logic] part of the car data file
        public void CarDataLogicSectionProcClear()
        {

            LoadedCarData.Logic.globalCondition.a = 0;
            LoadedCarData.Logic.globalCondition.b = 0;
            LoadedCarData.Logic.partsConditions.a = 0;
            LoadedCarData.Logic.partsConditions.b = 0;
            LoadedCarData.Logic.panelsConditions.a = 0;
            LoadedCarData.Logic.panelsConditions.b = 0;
            LoadedCarData.Logic.uniqueMod = 0;
            LoadedCarData.Logic.blockOBD = false;
        }

        //Empty out all data
        public void CarDataSectionClearAll()
        {
            CarDataMainSectionProcClear();
            CarDataOtherSectionProcClear();
            CarDataSuspensionSectionProcClear();
            CarDataEngineSectionProcClear();
            CarDataDriveshaftSectionProcClear();
            CarDataWheelsSectionProcClear();
            CarDataPartsSectionProcClear();
            CarDataInteriorSectionProcClear();
            CarDataLogicSectionProcClear();
        }

        #region Getters and Setters

        public int GetCarDataPartsArraySize()
        {
            //Return the parts counter
            return PartsCounter;
        }

        //Add a element yo the [Parts<num>] part of the car data file
        public void CarDataPartsSectionAdd(string name, float px, float py, float pz, float rx, float ry, float rz, float scale)
        {
            CarPartsType LocalParts;    //Local data struct to fill out and add to overall data collection

            //I have to init the parts, as the complier things it be will uninit later
            //This clears out the old data, but only for the in-use part of the list
            LocalParts.name = name;
            LocalParts.position.x = px;
            LocalParts.position.y = py;
            LocalParts.position.z = pz;
            LocalParts.rotation.x = rx;
            LocalParts.rotation.y = ry;
            LocalParts.rotation.z = rz;
            LocalParts.scale = scale;

            //We have all the data, so add the local to the collection list
            LoadedCarData.Parts.Add(LocalParts);
            //Update the counter
            PartsCounter++;
        }
        #endregion
        #endregion
    }
}
