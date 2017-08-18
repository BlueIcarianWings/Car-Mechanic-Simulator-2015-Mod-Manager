using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;    //For streamReader, etc
using System.Drawing;   //For size method

namespace CMS2015ModManager
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
        string ConfigFN = "CMS2015MMConfig.txt";        //Holds the name of the config file
        private string ConfigDir = null;                //Holds dir the config file is in (same as the exe)
        private string SavedGamesDir = null;            //Holds the dir of the saved games
        private string SavedGamesDirBkUp = null;        //Holds the backup dir of the saved games
        private string CarsDataDir = null;              //Holds the dir of the car data
        private string CarsDataDirBkUpDefault = null;   //Holds the backup dir of the car 
        private string CarsDataDirBkUpMod = null;       //Holds the backup dir of the car 
        private string ModMapDir = null;                //Holds the dir the custom maps live in

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

        public struct DoubleFloatPoint
        {
            //vector might work for this but it's seem to designed for 3D maths instead of storage
            //or pointf
            public float a;
            public float b;
        }

        public struct TripleFloatPoint
        {
            //vector3 might work for this but it's seem to designed for 3D maths instead of storage
            //or point3d
            public float x;
            public float y;
            public float z;
        }
        //-------------------------------------------

        // Constructor
        public CMS2015MM()
        {
        }

        // Destructor
        ~CMS2015MM()
        {
            // Some resource cleanup routines
        }

        #region Config stuff
        //The config stuff should be moved into it's own class
        //Load config data from config file
        public void ReadConfigFile()
        {
            //Asumes the config flie is local to the Mod Manager exe

            //Get dir we are currently in
            ConfigDir = System.IO.Path.GetDirectoryName(Application.ExecutablePath);

            //Check for config file and create if needed, this runs at start up so also functions as a 'first run' action

            if (File.Exists(ConfigFN))  //Check if the config file exists
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
                SavedGamesDir = list[0];            //Grab the first line
                SavedGamesDirBkUp = list[1];        //Grab the second line
                CarsDataDir = list[2];              //grab the third line
                CarsDataDirBkUpDefault = list[3];   //grab the forth line
                CarsDataDirBkUpMod = list[4];       //grab the fifth line
                ModMapDir = list[5];                //grab the sixth line
            }
            else
            {
                //If we get here then there was no config file and this may be a first run

                //If the file doesn't exist create one
                CreateConfigFile();
                //Backup the default cars
                BackupDefaultCarDataFilesDialog();
            }
        }

        //Create config file
        public void CreateConfigFile()
        {
            //After creating this, I've realised theres no easy way to update the text boxes
            //So I've disabled them for now, when I redo this in it's own class, I'll figure out a better way todo it

            //If the file doesn't exist load defaults
            String User = Environment.UserName;    //Determine the name of the user
            //Save game / profiles
            SavedGamesDir = "C:\\Users\\" + User + "\\appdata\\locallow\\Red Dot Games\\Car Mechanic Simulator 2015";   //Assemble the save dir path
            SavedGamesDirBkUp = SavedGamesDir  + "\\MMBackUp";      //Assemble the save backup dir
            //Car data files
            CarsDataDir = "C:\\Program Files (x86)\\Steam\\steamapps\\common\\Car Mechanic Simulator 2015\\cms2015_Data\\Datacars";
            CarsDataDirBkUpDefault = "C:\\Program Files (x86)\\Steam\\steamapps\\common\\Car Mechanic Simulator 2015\\cms2015_Data\\Datacars\\MMBackUpDefault";
            CarsDataDirBkUpMod = "C:\\Program Files (x86)\\Steam\\steamapps\\common\\Car Mechanic Simulator 2015\\cms2015_Data\\Datacars\\MMBackUpMod";
            //Mod maps
            ModMapDir = "C:\\Program Files (x86)\\Steam\\steamapps\\common\\Car Mechanic Simulator 2015\\Mods\\Tracks";

            //These may not match if the user has installed the game to a custom location, so we need to ask
            Form form = new Form();
            Label Titlelabel = new Label();
            Label DataCarslabel = new Label();
            Button DataCarsbutton = new Button();
            Label ModMaplabel = new Label();
            Button ModMapbutton = new Button();
            Label SaveGameslabel = new Label();
            Button SaveGamebutton = new Button();
            Button buttonOk = new Button();

            form.Text = "Set game directories";
            Titlelabel.Text = "Please set game directories\nDefault values currently set, Click ok to use these values";
            Titlelabel.SetBounds(3, 3, 280, 30);

            //Data Cars
            DataCarslabel.Text = "Data Cars Directory";
            DataCarslabel.SetBounds(3, 42, 100, 13);
            DataCarsbutton.Text = "...";
            DataCarsbutton.SetBounds(3, 60, 30, 20);
            DataCarsbutton.Click += new EventHandler(DataCarsbutton_Click);

            //Mod Maps
            ModMaplabel.Text = "Mod Maps Directory";
            ModMaplabel.SetBounds(3, 82, 100, 13);
            ModMapbutton.Text = "...";
            ModMapbutton.SetBounds(3, 100, 30, 20);
            ModMapbutton.Click += new EventHandler(ModMapbutton_Click);

            //Save Game / Profiles
            SaveGameslabel.Text = "Save Game Directory";
            SaveGameslabel.SetBounds(3, 122, 100, 13);
            SaveGamebutton.Text = "...";
            SaveGamebutton.SetBounds(3, 140, 30, 20);
            SaveGamebutton.Click += new EventHandler(SaveGamebutton_Click);

            //Exit button
            buttonOk.Text = "OK";
            buttonOk.DialogResult = DialogResult.OK;            
            buttonOk.SetBounds(3, 173, 75, 23);

            //Show the dialog prompt
            form.ClientSize = new Size(300, 200);
            form.Controls.AddRange(new Control[] { Titlelabel, DataCarslabel, DataCarsbutton,
                                                               ModMaplabel, ModMapbutton,
                                                               SaveGameslabel, SaveGamebutton,
                                                               buttonOk });
            form.FormBorderStyle = FormBorderStyle.FixedDialog;
            form.StartPosition = FormStartPosition.CenterScreen;
            form.MinimizeBox = false;
            form.MaximizeBox = false;
            form.AcceptButton = buttonOk;


            DialogResult dialogResult = form.ShowDialog();

            //Will only get here after the form above is closed

            //Save the config data file
            SaveConfigFile();
        }

        //Handles a click on the save game button in the sub form above
        private void SaveGamebutton_Click(object sender, EventArgs e)
        {
            //Open up a folder browser
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            //Get the result and check the dialog was ok'd
            DialogResult result = fbd.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                //Retrieve and set the selected dir
                SavedGamesDir = fbd.SelectedPath;
            }
        }

        //Handles a click on the mod map button in the sub form above
        private void ModMapbutton_Click(object sender, EventArgs e)
        {
            //Open up a folder browser
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            //Get the result and check the dialog was ok'd
            DialogResult result = fbd.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                //Retrieve and set the selected dir
                ModMapDir = fbd.SelectedPath;
            }
        }

        //Handles a click on the data car button in the sub form above
        private void DataCarsbutton_Click(object sender, EventArgs e)
        {
            //Open up a folder browser
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            //Get the result and check the dialog was ok'd
            DialogResult result = fbd.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                //Retrieve and set the selected dir
                CarsDataDir = fbd.SelectedPath;
                //Need to update the backup dir values
                CarsDataDirBkUpDefault = CarsDataDir + "\\MMBackUpDefault";
                CarsDataDirBkUpMod = CarsDataDir + "\\MMBackUpMod";
            }
        }

        //Save the config data file
        public void SaveConfigFile()
        {
            //Save the config file data, called whenever a change has been made via the menus
            //the menus will set the strings and this will write both

            try         //I think some people have problems with permissions, this will help 'skip-over' the config file creation step
            {
                using (StreamWriter writer = new StreamWriter(ConfigFN))
                {
                    writer.WriteLine(SavedGamesDir);
                    writer.WriteLine(SavedGamesDirBkUp);
                    writer.WriteLine(CarsDataDir);
                    writer.WriteLine(CarsDataDirBkUpDefault);
                    writer.WriteLine(CarsDataDirBkUpMod);
                    writer.WriteLine(ModMapDir);
                }
            }
            catch (Exception)
            {
                //Explain to the user
                MessageBox.Show("There was a problem writing the config file.\nThis is probably accses permissons related?\nThis may affect other file writes.\n\nChanged config paths will be used for not but will not remain when this application exits", "Config file creation problem");
            }
            
        }

        //Show the Car Data Files backup of default files initial dialog
        public void BackupDefaultCarDataFilesDialog()
        {
            //Auto backup notice
            Form form = new Form();
            Label Titlelabel = new Label();
            Label Bodylabel = new Label();
            Button buttonOk = new Button();
            Button buttonCancel = new Button();

            form.Text = "Backup Default Car Data Files";
            Titlelabel.Text = "Mod Manager will now backup the Default Car Data Files";
            Titlelabel.SetBounds(3, 3, 480, 30);
            Bodylabel.Text = "If the current Car Data Files are not the Default versions you can skip this step.\nYou'll need to perform a 'verify gamecache' through the list of installed games in Steam to get the default files, then perfom a backup from the menu bar within Mod Manager\n\nRemember to update the default files after each patch updates them (and your modded car files before the patch wipes them!)";
            Bodylabel.SetBounds(3, 42, 480, 90);

            //Exit buttons
            buttonOk.Text = "OK";
            buttonOk.DialogResult = DialogResult.OK;
            buttonOk.SetBounds(3, 153, 75, 23);
            buttonCancel.Text = "Cancel";
            buttonCancel.DialogResult = DialogResult.Cancel;
            buttonCancel.SetBounds(80, 153, 75, 23);

            //Assemble and show
            form.ClientSize = new Size(500, 180);
            form.Controls.AddRange(new Control[] { Titlelabel, Bodylabel, buttonOk, buttonCancel});
            form.FormBorderStyle = FormBorderStyle.FixedDialog;
            form.StartPosition = FormStartPosition.CenterScreen;
            form.MinimizeBox = false;
            form.MaximizeBox = false;
            form.AcceptButton = buttonOk;


            DialogResult dialogResult = form.ShowDialog();

            //Perform the backup, if it's ok
            if (dialogResult.Equals(DialogResult.OK))
            {
                DirectoryCopy(CarsDataDir, CarsDataDirBkUpDefault, false);     //Copy the files but don't copy any dirs
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

        public string GetCarsDataDirBkUpDefault()
        {
            return CarsDataDirBkUpDefault;
        }

        public string GetCarsDataDirBkUpMod()
        {
            return CarsDataDirBkUpMod;
        }

        public void SetCarsDataDir(string Input)
        {
            CarsDataDir = Input;
        }

        public void SetCarsDataDirBkUpDefault(string Input)
        {
            CarsDataDirBkUpDefault = Input;
        }

        public void SetCarsDataDirBkUpMod(string Input)
        {
            CarsDataDirBkUpMod = Input;
        }

        public string GetModMapDir()
        {
            return ModMapDir;
        }

        public void SetModMapDir(string Input)
        {
            ModMapDir = Input;
        }

        public string GetConfigDir()
        {
            return ConfigDir;
        }

        public void SetConfigDir(string Input)
        {
            ConfigDir = Input;
        }
        #endregion
        #endregion

        //Copy the contents of a directory, choice to copy sub dirs or not
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
                file.CopyTo(temppath, true);
            }

            // If copying subdirectories, copy them and their contents to new location.
            if (copySubDirs)
            {
                foreach (DirectoryInfo subdir in dirs)
                {
                    string temppath = Path.Combine(destDirName, subdir.Name);
                    DirectoryCopy(subdir.FullName, temppath, copySubDirs);
                    //Does this copy the contnets of the subdir too? need to check
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

                //We also have the cars.txt, engines.txt and tires.txt in the list, so we need to remove them
                if ((!(temp[index] == "cars.txt")) && (!(temp[index] == "engines.txt")) && (!(temp[index] == "tires.txt")))
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
        #endregion

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
        

    }
}
