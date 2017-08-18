using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;    //For streamReader, 

namespace CMS2015ModManager
{
    public partial class Form1 : Form
    {
        private string ModManVersion = "0.9.4.1";       //Version constant for ModManager
        private string GameVersion = "1.1.1.2";         //Version constant for the game

        //Class object for class that does the acutal mod managing stuff    //here so it's scope is within the form object  //should move the config stuff out at somepoint
        CMS2015MM ModMan;

        //Car Data, single object
        CarData CarDataObject;

        //Engine Data objects
        List<EngineData> EngineDataList = new List<EngineData>();

        //Engine Data objects
        List<TireData> TireDataList = new List<TireData>();

        //Map Data objects
        List<MapData> MapDataList = new List<MapData>();

        int PartsSelectedIndex = -1;        //Need this to track what the previous selected [Parts] index was when updating the [Parts] data list
        bool NewPartToStore = false;        //Need this to track if a new item needs saving
        int AddOnLoadSelectedIndex = -1;    //Need this to track what the previous selected [AddOnLoad] index was when updating the [AddOnLoad] data list
        bool NewAddOnLoadToStore = false;   //Need this to track if a new item needs saving

        public Form1()
        {
            //Setup the form controls
            InitializeComponent();

            //Setup the class that does the acutal mod managing stuff
            ModMan = new CMS2015MM();

            //Setup the list of class objects that'll handle engine data
            EngineDataList = new List<EngineData>();

            //Setup the list of class objects that'll handle engine data
            TireDataList = new List<TireData>();

            //Setup the list of class objects that'll handle engine data
            MapDataList = new List<MapData>();

            //Setup the the class objects that'll handle car data
            CarDataObject = new CarData(ModManVersion);     //Pass in the version number of this tool
        }

        //stuff to do when the form is loaded
        private void Form1_Load(object sender, EventArgs e)
        {
            //Called from within InitializeComponent()
            //A good place to load data after the form is loaded

            //load the config file, contains saved game dir locations
            ModMan.ReadConfigFile();

            //Car List Tab
            //load the list of cars file
            ModMan.ReadCarsCurrent();
            //Populate the cars currently in list, list box
            PopulateCarsCurrentList();
            //Find car data files
            ModMan.FindCarsFiles();
            //Populate the cars files available, list box
            PopulateCarsAvailableList();

            //Engine Data Tab
            //load the engine data file
            LoadEngineDataFile();
            //Populate the engine data drop down list
            PopulateAvailableEnginesComboBox();

            //Tire Data Tab
            //load the tire data files
            LoadTireDataFile();
            //Populate the tire data drop down list
            PopulateAvailableTiresComboBox();

            //Car Data Tab
            //List of available car text file already loaded
            //Populate the available cars drop down list
            PopulateAvailableCarsComboBox();

            //Map Data Tab
            //Get the info from the map data files
            LoadMapData();
            //Populate the map data list box
            PopulateAvailableMapslistBox();

            //Save Games Tab
            //Get the list of profiles
            PopulateProfileComboBox();
        }

        #region Menu bar
        //Save Game Items

        //Handle set saves source click from menu strip
        private void setSourceDirToolStripMenuItemSaves_Click(object sender, EventArgs e)
        {
            //Open up a folder browser
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            //Get the result and check the dialog was ok'd
            DialogResult result = fbd.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                //Retrieve and set the selected dir
                ModMan.SetSavedGamesDir(fbd.SelectedPath);
                //Update the config file
                ModMan.SaveConfigFile();
            }            
        }

        //Handle set saves backup click from menu strip
        private void setBackupDirToolStripMenuItemSaves_Click(object sender, EventArgs e)
        {
            //Open up a folder browser
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            //Get the result and check the dialog was ok'd
            DialogResult result = fbd.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                //Retrieve and set the selected dir
                ModMan.SetSavedGamesDirBkUp(fbd.SelectedPath);
                //Update the config file
                ModMan.SaveConfigFile();
            }
        }

        //Handle the menu request to backup the save dir
        private void backupSavesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Prompt the user to see if they are sure
            DialogResult PromptResult = MessageBox.Show("This will backup all the save profiles overwriting the ones in the save backup folder\nAre you sure?", "Backup Game Saves", MessageBoxButtons.YesNo);

            if (PromptResult == DialogResult.Yes)
            {
                ModMan.DirectoryCopy(ModMan.GetSavedGamesDir(), ModMan.GetSavedGamesDirBkUp(), true);
            }
        }

        //Handle the menu request to restore the saves dir
        private void restoreSavesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Prompt the user to see if they are sure
            DialogResult PromptResult = MessageBox.Show("This will overwrite all the save profiles with the ones in the save backup folder\nAre you sure?", "Restore Game Saves", MessageBoxButtons.YesNo);

            if (PromptResult == DialogResult.Yes)
            {
                ModMan.DirectoryCopy(ModMan.GetSavedGamesDirBkUp(), ModMan.GetSavedGamesDir(), true);
            }
        }

        //Car Data Items

        //Handle set car data source click from menu strip
        private void setCarDataSourceDirToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Open up a folder browser
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            //Get the result and check the dialog was ok'd
            DialogResult result = fbd.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                //Retrieve and set the selected dir
                ModMan.SetCarsDataDir(fbd.SelectedPath);
                //Update the config file
                ModMan.SaveConfigFile();
            }
        }

        //Handle set mod car data backup click from menu strip
        private void setModBackupDirToolStripMenuItemCar_Click(object sender, EventArgs e)
        {
            //Open up a folder browser
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            //Get the result and check the dialog was ok'd
            DialogResult result = fbd.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                //Retrieve and set the selected dir
                ModMan.SetCarsDataDirBkUpMod(fbd.SelectedPath);
                //Update the config file
                ModMan.SaveConfigFile();
            }
        }

        //Handle the menu request to backup the mod car data dir
        private void backupModCarDataToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Call the restore all function for the mod car data files
            CDCopyAllModifiedCarDataFiles(true);
        }

        //Handle the menu request to restore the mod car data dir
        private void restoreModCarDataToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Call the restore all function for the mod car data files
            CDCopyAllModifiedCarDataFiles(false);
        }

        //Handle the menu request to backup the default car data dir
        private void setDefaultBackupDirToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Open up a folder browser
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            //Get the result and check the dialog was ok'd
            DialogResult result = fbd.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                //Retrieve and set the selected dir
                ModMan.SetCarsDataDirBkUpDefault(fbd.SelectedPath);
                //Update the config file
                ModMan.SaveConfigFile();
            }
        }

        //Handle set default car data backup click from menu strip
        private void backupDefaultCarDataToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Call the restore all function for the default car data files
            CDCopyAllDefaultCarDataFiles(true);
        }

        //Handle the menu request to restore the default car data dir
        private void restoreDefaultCarDataToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Call the restore all function for the default car data files
            CDCopyAllDefaultCarDataFiles(false);
        }

        //Misc Menu Items

        //Handle a click on the about menu item
        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Car Mechanic Simulator 2015 Mod Manager Version " + ModManVersion + "\nVery much a work in progress\nDesigned for " + GameVersion + "\n\nThanks to all that have helped\n\nBy Blue Icarian Wings");
        }

        //Handle a call to exit
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Close this all down
            Application.Exit();
        }
        #endregion

        #region Map stuff
        //Handle the request to the games mod maps dir
        private void SetGameMapButton_Click(object sender, EventArgs e)
        {
            //Open up a folder browser
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            //Get the result and check the dialog was ok'd
            DialogResult result = fbd.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                //Retrieve and set the selected dir
                ModMan.SetModMapDir(fbd.SelectedPath);
                //Update the config file
                ModMan.SaveConfigFile();
            }
        }

        //Handles a call to copy a custom map to the map dir
        private void LoadModMapButton_Click(object sender, EventArgs e)
        {
            //Open up a folder browser
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            //Get the result and check the dialog was ok'd
            DialogResult result = fbd.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                //Retrieve and set the selected dir
                string folder = "\\";
                folder += Path.GetFileName(fbd.SelectedPath);

                //If the directory exists get rid of it
                if (Directory.Exists(ModMan.GetModMapDir() + folder))
                {
                    //Delete directory with the map in
                    Directory.Delete(ModMan.GetModMapDir() + folder, true);
                }

                //Copy over the directory
                ModMan.DirectoryCopy(fbd.SelectedPath, ModMan.GetModMapDir() + folder, true);
                //Empty out and rebuild the map list
                LoadMapData();
                //Rebuild the list
                PopulateAvailableMapslistBox();
            }
        }

        //Assembles a list of info on the maps in the maps mod directory
        private void LoadMapData()
        {
            //Get a list of all the sub folders in the map mod directory
            string[] DirList = Directory.GetDirectories(ModMan.GetModMapDir());

            //Empty out old data
            MapDataList.Clear();

            //For each entry we need to check if the subfolder contains a mod file and get it's info
            //(this may need filtering adding if they use the same dir for all mods)
            foreach(string DirLine in DirList)
            {
                string Filename = DirLine + "\\cms15mod.txt";

                if (File.Exists(Filename))      //If there is mod file in the directory
                {
                    //Local temp object to fill out
                    MapData LocalMap;
                    //Local temp object to fill out
                    LocalMap = new MapData();   //Call it's constructor to init it

                    //Get folder the map is stored in
                    LocalMap._Folder = Path.GetFileName(DirLine);

                    //Load the whole file
                    string[] MDlines = System.IO.File.ReadAllLines(Filename);

                    foreach (string FileLine in MDlines)
                    {
                        //Check for blank lines and null lines (end of file(might be able to remove the null check, legacy from a stream reader style))
                        if ((FileLine != "") && (FileLine != null))         //if the line is empty skip over all this
                        {
                            int j = FileLine.IndexOf('=');              //Find the end of label string
                            string line = "";
                            string label = "";
                            if (j != -1)        //If the string doesn't have a = in it
                            {
                                label = FileLine.Substring(0, j);       //Grabs the bit upto the '='
                                                                        //Grab the bit after the '=' and remove the leading and trailing spaces
                                line = FileLine.Substring(j + 1, FileLine.Length - (j + 1)).Trim(' ');
                            }

                            switch (label)  //Fill out the Main data
                            {
                                case "name":
                                    LocalMap._Name = line;
                                    break;
                                case "credits":
                                    LocalMap._Credits = line;
                                    break;
                                case "type":
                                    LocalMap._Type = line;
                                    break;
                                case "file":
                                    LocalMap._File = line;
                                    break;
                                case "image":
                                    LocalMap._Preview = line;
                                    break;
                                case "description":
                                    LocalMap._Description = line;
                                    break;
                                default:
                                    //Nothing here
                                    //Blank lines and malformed lines will end up here
                                    break;
                            }
                        }
                        else
                        {
                            //Blank line or null line
                        }
                    }
                    //Add the filled out local to the list
                    MapDataList.Add(LocalMap);
                }
            }
        }

        //Populate the maps currently available list box
        private void PopulateAvailableMapslistBox()
        {
            //Empty out current contents
            AvailableMapslistBox.Items.Clear();

            //Loop through all of the maps in the list
            foreach(MapData Line in MapDataList)
            {
                AvailableMapslistBox.Items.Add(Line._Name);
            }
            
        }

        //Handle a call to select a map
        private void AvailableMapslistBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            //Get the index of the selected map
            int Index = AvailableMapslistBox.SelectedIndex;

            //Empty out old data
            MDTSelectedMaplistBox.Items.Clear();

            //Local temp to assemble info
            string line;

            line = "[mod]";
            MDTSelectedMaplistBox.Items.Add(line);
            line = "name = " + MapDataList[Index]._Name;
            MDTSelectedMaplistBox.Items.Add(line);
            line = "credits = " + MapDataList[Index]._Credits;
            MDTSelectedMaplistBox.Items.Add(line);
            line = "type = " + MapDataList[Index]._Type;
            MDTSelectedMaplistBox.Items.Add(line);
            line = "file = " + MapDataList[Index]._File;
            MDTSelectedMaplistBox.Items.Add(line);
            line = "image = " + MapDataList[Index]._Preview;
            MDTSelectedMaplistBox.Items.Add(line);
            line = "description = " + MapDataList[Index]._Description;
            MDTSelectedMaplistBox.Items.Add(line);
        }

        //Handle a call to chose a track bundle filename
        private void MDTMDFBundleFilebutton_Click(object sender, EventArgs e)
        {
            // Show the dialog and get result.
            OpenFileDialog Dialog = new OpenFileDialog();
            if (Dialog.ShowDialog() == DialogResult.OK)
            {
                //Set the GUI element to the got filename
                MDTMDFBundleFiletextBox.Text = Dialog.FileName;
            }
        }

        //Handle a call to open an existing map info file
        private void MDTMDFOpenbutton_Click(object sender, EventArgs e)
        {
            //Local to hold filname
            string Filename = "";

            // Show the dialog and get result.
            OpenFileDialog Dialog = new OpenFileDialog();
            if (Dialog.ShowDialog() == DialogResult.OK)
            {
                //Get the selected filename
                Filename = Dialog.FileName;
            }

            if (File.Exists(Filename))      //If there is mod file in the directory
            {
                //Load the whole file
                string[] MDlines = System.IO.File.ReadAllLines(Filename);

                foreach (string FileLine in MDlines)
                {
                    //Check for blank lines and null lines (end of file(might be able to remove the null check, legacy from a stream reader style))
                    if ((FileLine != "") && (FileLine != null))         //if the line is empty skip over all this
                    {
                        int j = FileLine.IndexOf('=');              //Find the end of label string
                        string line = "";
                        string label = "";
                        if (j != -1)        //If the string doesn't have a = in it
                        {
                            label = FileLine.Substring(0, j);       //Grabs the bit upto the '='
                                                                    //Grab the bit after the '=' and remove the leading and trailing spaces
                            line = FileLine.Substring(j + 1, FileLine.Length - (j + 1)).Trim(' ');
                        }

                        switch (label)  //Fill out the Main data
                        {
                            case "name":
                                MDTMDFNametextBox.Text = line;
                                break;
                            case "credits":
                                MDTMDFCreditstextBox.Text = line;
                                break;
                            case "type":
                                MDTMDFTypetextBox.Text = line;
                                break;
                            case "file":
                                MDTMDFBundleFiletextBox.Text = line;
                                break;
                            case "image":
                                MDTMDFPicturetextBox.Text = line;
                                break;
                            case "description":
                                MDTMDFDescriptiontextBox.Text = line;
                                break;
                            default:
                                //Nothing here
                                //Blank lines and malformed lines will end up here
                                break;
                        }
                    }
                    else
                    {
                        //Blank line or null line
                    }
                }
            }
        }

        //Handle a call to clear out the map info file GUI
        private void MDTMDFResetbutton_Click(object sender, EventArgs e)
        {
            MDTMDFNametextBox.Text = "";
            MDTMDFCreditstextBox.Text = "";
            MDTMDFTypetextBox.Text = "";
            MDTMDFBundleFiletextBox.Text = "";
            MDTMDFPicturetextBox.Text = "";
            MDTMDFDescriptiontextBox.Text = "";
        }

        //Handle a call to save the map info file
        private void MDTMDFSavebutton_Click(object sender, EventArgs e)
        {
            //Need a save file dialog
            SaveFileDialog savefile = new SaveFileDialog();
            // set a default file name
            savefile.FileName = "cms15mod.txt";
            // set filters - this can be done in properties as well
            savefile.Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";

            if (savefile.ShowDialog() == DialogResult.OK)
            {
                //Create a local file writer
                StreamWriter writer = new StreamWriter(savefile.FileName);

                //Write out the file contents
                writer.WriteLine("[mod]");
                writer.WriteLine("name=" + MDTMDFNametextBox.Text);
                writer.WriteLine("credits=" + MDTMDFCreditstextBox.Text);
                writer.WriteLine("type=" + MDTMDFTypetextBox.Text);
                writer.WriteLine("file=" + MDTMDFBundleFiletextBox.Text);
                writer.WriteLine("image=" + MDTMDFPicturetextBox.Text);
                writer.WriteLine("description=" + MDTMDFDescriptiontextBox.Text);
                writer.WriteLine("\nMade with= CMS15ModManager {0]", ModManVersion);

                //we are finished with the writer so close and bin it
                writer.Close();
                writer.Dispose();
            }
        }

        //Handle a call to reload the map list
        private void MDTRefreshbutton_Click(object sender, EventArgs e)
        {
            //Empty out and rebuild the map list
            LoadMapData();
            //Rebuild the list
            PopulateAvailableMapslistBox();
        }

        //Handle a call to delete a map
        private void DeleteMapbutton_Click(object sender, EventArgs e)
        {
            //Get the index of the selected car data file
            int Index = AvailableMapslistBox.SelectedIndex;
            //Check if a line has been selected
            if (Index > -1)
            {
                //Assemble directory path
                //Retrieve and set the selected dir
                string folder = "\\" + MapDataList[Index]._Folder;
                folder = ModMan.GetModMapDir() + folder;
                //Delete directory with the map in
                Directory.Delete(folder, true);
                //Empty out and rebuild the map list
                LoadMapData();
                //Rebuild the list
                PopulateAvailableMapslistBox();
                //Clear out the Selected Map box
                MDTSelectedMaplistBox.Items.Clear();
            }
        }

        //Handle a call to select a map preview picture
        private void MDTMDFPicturebutton_Click(object sender, EventArgs e)
        {
            //Assume the image is named correctly

            //Open up a file browser
            OpenFileDialog ofd = new OpenFileDialog();
            // Show the dialog and get result.
            DialogResult result = ofd.ShowDialog();
            if (result == DialogResult.OK) // Test result.
            {
                string PicturePath = ofd.FileName;      //Local to hold path/filename
                //Check if there is a thumbnail picture present to use
                if (File.Exists(PicturePath))
                {
                    Image image = Image.FromFile(PicturePath);  //Use the picture if it exists
                    MDTMDFPreviewpictureBox.Image = image;
                }
                else
                {
                    PicturePath = ModMan.GetCarsDataDir() + "\\car_placeholder.jpg";
                    Image image = Image.FromFile(PicturePath);  //Use the place holder image
                    MDTMDFPreviewpictureBox.Image = image;
                }
                //Save the path/filename
                MDTMDFPicturetextBox.Text = Path.GetFileName(PicturePath);
            }
        }

        #endregion

        #region Engine stuff
        //Load and parse the engine data file
        public void LoadEngineDataFile()
        {
            //Loads the file that contains the engine definitions

            //Setup the filename
            string FileName = ModMan.GetCarsDataDir() + "\\engines.txt";
            //Check if the config file exists
            if (File.Exists(FileName))
            {
                //Load the whole file
                string[] EDFlines = System.IO.File.ReadAllLines(FileName);
                //Local to fill out before adding to the EngineDataList
                EngineData LocalED;
                //Locals to fill out before putting into LocalED (A property or indexer may not be passed as an out or ref parameter)
                bool LocalBool = false;
                int LocalInt = 0;
                
                //Loop through the lines to process them
                for (int i = 0; i < EDFlines.Length; i++)
                {

                    //Fill out the local until we get a new new heading (starts with[)
                    // then save the local to the list and start again
                    
                    //If this is a new definition
                    if (EDFlines[i].StartsWith("["))
                    {
                        //New definition so start to fill out the local
                        LocalED = new EngineData(); //Call it's constructor to init it

                        //We are currently sat on the [<name>] line so lets get the name
                        int j = EDFlines[i].IndexOf('=');               //Find the end of label string
                        string line = EDFlines[i].Substring(j + 1, EDFlines[i].Length - (j + 1));    //Grab the bit after the '='
                        LocalED._Name = line.Trim(' ');                 //Remove the leading on trailing spaces and store the name of the new definition
                        i++;    //Move it along to the next line, so the if condition check doesn't end it immediately

                        //TO-DO wrap the if here in a loop
                        while ((i < EDFlines.Length) && (!(EDFlines[i].StartsWith("["))))   //Keep reading lines until another section header line is found or out of lines
                        {

                            //Check for blank lines and null lines (end of file(might be able to remove the null check, legacy from a stream reader style))
                            if ((EDFlines[i] != "") && (EDFlines[i] != null) && (!(EDFlines[i].StartsWith(";"))))    //if the line is empty or a comment skip over all this
                            {
                                j = EDFlines[i].IndexOf('=');              //Find the end of label string
                                string label = EDFlines[i].Substring(0, j);    //Grabs the bit upto the '='
                                                                               //Grab the bit after the '=' and remove the leading and trailing spaces
                                line = EDFlines[i].Substring(j + 1, EDFlines[i].Length - (j + 1)).Trim(' ');

                                switch (label)  //Fill out the Main data
                                {
                                    case "blockOBD":
                                        bool.TryParse(line, out LocalBool);     //convert the strings to a bool
                                        LocalED._BlockOBD = LocalBool;          //copy into the local object (we cannot use it directly with 'out')
                                        break;
                                    case "engineSound":
                                        LocalED._EngineSound = line;
                                        break;
                                    case "maxPower":
                                        int.TryParse(line, out LocalInt);       //convert the strings to a number
                                        LocalED._maxPower = LocalInt;           //copy into the local object (we cannot use it directly with 'out')
                                        break;
                                    case "maxPowerRPM":
                                        int.TryParse(line, out LocalInt);       //convert the strings to a number
                                        LocalED._maxPowerRPM = LocalInt;        //copy into the local object (we cannot use it directly with 'out')
                                        break;
                                    case "maxTorqueRPM":
                                        int.TryParse(line, out LocalInt);       //convert the strings to a number
                                        LocalED._maxTorqueRPM = LocalInt;       //copy into the local object (we cannot use it directly with 'out')
                                        break;
                                    case "minRPM":
                                        int.TryParse(line, out LocalInt);       //convert the strings to a number
                                        LocalED._minRPM = LocalInt;             //copy into the local object (we cannot use it directly with 'out')
                                        break;
                                    case "maxRPM":
                                        int.TryParse(line, out LocalInt);       //convert the strings to a number
                                        LocalED._maxRPM = LocalInt;             //copy into the local object (we cannot use it directly with 'out')
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
                                i++;    //Move to next line
                            }
                        }
                        i--;    //Knock the counter back a line as the while loop that called us, will inc it and step over the section header we just found.
                        EngineDataList.Add(LocalED);    //Add full definition to the list
                    }
                }
            }
        }

        //Populate the engine data drop down combo box
        private void PopulateAvailableEnginesComboBox()
        {
            int index = 0;  //local loop counter
            //Loop through all of the loaded engines
            while (index < EngineDataList.Count)
            {
                //Add an item to the engines data combo box
                AvailableEnginesComboBox.Items.Add(EngineDataList[index]._Name);
                index++;    //inc counter
            }
        }
        
        //Handle a selection in the engines combo box
        private void AvailableEnginesComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            //Get the index of the selected engine
            int Index = AvailableEnginesComboBox.SelectedIndex;

            //Show the picture file
            //Assemble the path and filename
            string PicturePath = null;
            string SelectedEngine = AvailableEnginesComboBox.Text;
            SelectedEngine = SelectedEngine.Substring(1, SelectedEngine.Length-2);
            PicturePath = ModMan.GetConfigDir() + "\\Images\\" + SelectedEngine + ".png";
            //PicturePath = PicturePath + ".png";                                  //Add jpg

            //Check if there is a thumbnail picture present to use
            if (File.Exists(PicturePath))
            {
                Image image = Image.FromFile(PicturePath);  //Use the picture if it exists
                EDTImagepictureBox.Image = image;
            }
            else
            {
                PicturePath = ModMan.GetCarsDataDir() + "\\car_placeholder.jpg";
                Image image = Image.FromFile(PicturePath);  //Use the place holder image
                EDTImagepictureBox.Image = image;
            }

            //Set the data fields with the values from the selected engine
            EDTBlockOBDcheckBox.Checked = EngineDataList[Index]._BlockOBD;
            EDTEngineSoundtextBox.Text = EngineDataList[Index]._EngineSound;
            EDTmaxPowerNumericUpDown.Value = EngineDataList[Index]._maxPower;
            EDTmaxPowerRPMNumericUpDown.Value = EngineDataList[Index]._maxPowerRPM;
            EDTmaxTorqueRPMNumericUpDown.Value = EngineDataList[Index]._maxTorqueRPM;
            EDTminRPMLabelNumericUpDown.Value = EngineDataList[Index]._minRPM;
            EDTmaxRPMLabelNumericUpDown.Value = EngineDataList[Index]._maxRPM;
        }

        //Send the engine data fields to the engine data file (changes internal values which the reset reads from)
        private void EDTCommitButton_Click(object sender, EventArgs e)
        {
            //Check if ok to save file
            if (MessageBox.Show("Update Engine Data file?\nReset will use these values from now on\n\nAre you sure?", "Confirm commit", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                //Get the index of the selected engine
                int Index = AvailableEnginesComboBox.SelectedIndex;

                //send the new engine data values
                EngineDataList[Index]._maxPower = (int)EDTmaxPowerNumericUpDown.Value;      //Need to cast the decimal to an int
                EngineDataList[Index]._maxPowerRPM = (int)EDTmaxPowerRPMNumericUpDown.Value;
                EngineDataList[Index]._maxTorqueRPM = (int)EDTmaxTorqueRPMNumericUpDown.Value;
                EngineDataList[Index]._minRPM = (int)EDTminRPMLabelNumericUpDown.Value;
                EngineDataList[Index]._maxRPM = (int)EDTmaxRPMLabelNumericUpDown.Value;

                //write to file
                Index = 0;  //reset counter used to index through engine data array

                //Setup the filename
                string FileName = ModMan.GetCarsDataDir() + "\\engines.txt";
                //Create a local file writer
                StreamWriter writer = new StreamWriter(FileName);

                //Write out each engine in turn
                while (Index < EngineDataList.Count)
                {
                    writer.WriteLine(EngineDataList[Index]._Name);
                    if (EngineDataList[Index]._BlockOBD)
                    {
                        writer.WriteLine("blockOBD=" + EngineDataList[Index]._BlockOBD);
                    }
                    writer.WriteLine("engineSound=" + EngineDataList[Index]._EngineSound);
                    writer.WriteLine("maxPower=" + EngineDataList[Index]._maxPower);
                    writer.WriteLine("maxPowerRPM=" + EngineDataList[Index]._maxPowerRPM);
                    writer.WriteLine("maxTorqueRPM=" + EngineDataList[Index]._maxTorqueRPM);
                    writer.WriteLine("minRPM=" + EngineDataList[Index]._minRPM);
                    writer.WriteLine("maxRPM=" + EngineDataList[Index]._maxRPM);
                    writer.WriteLine();     //Blank line seperator

                    Index++;    //Increment counter
                }

                writer.WriteLine();     //Blank line seperator
                writer.WriteLine("\nMade with= CMS15ModManager {0}", ModManVersion);
                //we are finished with the writer so close and bin it
                writer.Close();
                writer.Dispose();
            }
        }

        //Handles a call to reset the current engine data fields, from stored values (file in the active folder)
        private void EDTResetButton_Click(object sender, EventArgs e)
        {
            //Prompt the user to see if they are sure
            DialogResult PromptResult = MessageBox.Show("This will reset all values to those from the existing engine data file\n\nAre you sure?", "Reset Engine Data", MessageBoxButtons.YesNo);

            if (PromptResult == DialogResult.Yes)
            {
                //Reset the engine data values, just call the other function that sets them initialy
                AvailableEnginesComboBox_SelectedIndexChanged(sender, e);
            }
        }

        //Handles a call to restore a single engine definition from the default folder
        private void EDTRestoreDefaultbutton_Click(object sender, EventArgs e)
        {
            //Messing and dulicating doing this here, but I don't feel like redesigning the EngineData class to have a read function right now

            //Prompt the user to see if they are sure
            DialogResult PromptResult = MessageBox.Show("This will restore this engines values to those from the default engine data file\n\nAre you sure?", "Restore Engine Data", MessageBoxButtons.YesNo);

            if (PromptResult == DialogResult.Yes)
            {
                //Create a local Engine Data object
                List<EngineData> DefaultEngineDataList = new List<EngineData>();

                //Loads the file that contains the default engine definitions

                //Setup the filename
                string FileName = ModMan.GetCarsDataDirBkUpDefault() + "\\engines.txt";
                //Check if the config file exists
                if (File.Exists(FileName))
                {
                    //Load the whole file
                    string[] EDFlines = System.IO.File.ReadAllLines(FileName);
                    //Local to fill out before adding to the EngineDataList
                    EngineData LocalED;
                    //Locals to fill out before putting into LocalED (A property or indexer may not be passed as an out or ref parameter)
                    bool LocalBool = false;
                    int LocalInt = 0;

                    //Loop through the lines to process them
                    for (int i = 0; i < EDFlines.Length; i++)
                    {

                        //Fill out the local until we get a new new heading (starts with[)
                        // then save the local to the list and start again

                        //If this is a new definition
                        if (EDFlines[i].StartsWith("["))
                        {
                            //New definition so start to fill out the local
                            LocalED = new EngineData(); //Call it's constructor to init it

                            //We are currently sat on the [<name>] line so lets get the name
                            int j = EDFlines[i].IndexOf('=');               //Find the end of label string
                            string line = EDFlines[i].Substring(j + 1, EDFlines[i].Length - (j + 1));    //Grab the bit after the '='
                            LocalED._Name = line.Trim(' ');                 //Remove the leading on trailing spaces and store the name of the new definition
                            i++;    //Move it along to the next line, so the if condition check doesn't end it immediately

                            //TO-DO wrap the if here in a loop
                            while ((i < EDFlines.Length) && (!(EDFlines[i].StartsWith("["))))   //Keep reading lines until another section header line is found or out of lines
                            {

                                //Check for blank lines and null lines (end of file(might be able to remove the null check, legacy from a stream reader style))
                                if ((EDFlines[i] != "") && (EDFlines[i] != null) && (!(EDFlines[i].StartsWith(";"))))    //if the line is empty or a comment skip over all this
                                {
                                    j = EDFlines[i].IndexOf('=');              //Find the end of label string
                                    string label = EDFlines[i].Substring(0, j);    //Grabs the bit upto the '='
                                                                                   //Grab the bit after the '=' and remove the leading and trailing spaces
                                    line = EDFlines[i].Substring(j + 1, EDFlines[i].Length - (j + 1)).Trim(' ');

                                    switch (label)  //Fill out the Main data
                                    {
                                        case "blockOBD":
                                            bool.TryParse(line, out LocalBool);     //convert the strings to a bool
                                            LocalED._BlockOBD = LocalBool;          //copy into the local object (we cannot use it directly with 'out')
                                            break;
                                        case "engineSound":
                                            LocalED._EngineSound = line;
                                            break;
                                        case "maxPower":
                                            int.TryParse(line, out LocalInt);       //convert the strings to a number
                                            LocalED._maxPower = LocalInt;           //copy into the local object (we cannot use it directly with 'out')
                                            break;
                                        case "maxPowerRPM":
                                            int.TryParse(line, out LocalInt);       //convert the strings to a number
                                            LocalED._maxPowerRPM = LocalInt;        //copy into the local object (we cannot use it directly with 'out')
                                            break;
                                        case "maxTorqueRPM":
                                            int.TryParse(line, out LocalInt);       //convert the strings to a number
                                            LocalED._maxTorqueRPM = LocalInt;       //copy into the local object (we cannot use it directly with 'out')
                                            break;
                                        case "minRPM":
                                            int.TryParse(line, out LocalInt);       //convert the strings to a number
                                            LocalED._minRPM = LocalInt;             //copy into the local object (we cannot use it directly with 'out')
                                            break;
                                        case "maxRPM":
                                            int.TryParse(line, out LocalInt);       //convert the strings to a number
                                            LocalED._maxRPM = LocalInt;             //copy into the local object (we cannot use it directly with 'out')
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
                                    i++;    //Move to next line
                                }
                            }
                            i--;    //Knock the counter back a line as the while loop that called us, will inc it and step over the section header we just found.
                            DefaultEngineDataList.Add(LocalED);    //Add full definition to the list
                        }
                    }
                    //Now we have a full list, copy over the wanted one

                    //Check the file isn't empty
                    if (DefaultEngineDataList.Count > 0)
                    {
                        string SelectedEngine = AvailableEnginesComboBox.Text;  //Get the currently selected engine

                        //Loop through each entry
                        foreach (EngineData Eng in DefaultEngineDataList)
                        {
                            if (Eng._Name == SelectedEngine) //If the name matches
                            {
                                //Set the data fields with the values from the selected engine
                                EDTBlockOBDcheckBox.Checked = Eng._BlockOBD;
                                EDTEngineSoundtextBox.Text = Eng._EngineSound;
                                EDTmaxPowerNumericUpDown.Value = Eng._maxPower;
                                EDTmaxPowerRPMNumericUpDown.Value = Eng._maxPowerRPM;
                                EDTmaxTorqueRPMNumericUpDown.Value = Eng._maxTorqueRPM;
                                EDTminRPMLabelNumericUpDown.Value = Eng._minRPM;
                                EDTmaxRPMLabelNumericUpDown.Value = Eng._maxRPM;

                                //Write the file by calling the save button
                                EDTCommitButton_Click(sender, e);

                                break;  //Exit the foreach loop
                            }
                        }
                    }
                }   //File.Exists check
            }
        }

        //Handles a call to restore all engines to default
        private void EDTRestoreAllDefaultbutton_Click(object sender, EventArgs e)
        {
            //Prompt the user to see if they are sure
            DialogResult PromptResult = MessageBox.Show("This will restore all engines to those from the default engine data file\n\nAre you sure?", "Restore All Engine Data", MessageBoxButtons.YesNo);

            if (PromptResult == DialogResult.Yes)
            {
                // overwrite the destination file if it already exists.
                System.IO.File.Copy(ModMan.GetCarsDataDirBkUpDefault() + "\\engines.txt", ModMan.GetCarsDataDir() + "\\engines.txt", true);
            }
        }

        //Handles a call to restore a single engine to a mod
        private void EDTRestoreModbutton_Click(object sender, EventArgs e)
        {
            //Prompt the user to see if they are sure
            DialogResult PromptResult = MessageBox.Show("This will restore this engines values to those from the mod engine data file\n\nAre you sure?", "Restore Mod Data", MessageBoxButtons.YesNo);

            if (PromptResult == DialogResult.Yes)
            {
                //Messing and dulicating doing this here, but I don't feel like redesigning the EngineData class to have a read function right now

                //Create a local Engine Data object
                List<EngineData> ModEngineDataList = new List<EngineData>();

                //Loads the file that contains the mod engine definitions

                //Setup the filename
                string FileName = ModMan.GetCarsDataDirBkUpMod() + "\\engines.txt";
                //Check if the config file exists
                if (File.Exists(FileName))
                {
                    //Load the whole file
                    string[] EDFlines = System.IO.File.ReadAllLines(FileName);
                    //Local to fill out before adding to the EngineDataList
                    EngineData LocalED;
                    //Locals to fill out before putting into LocalED (A property or indexer may not be passed as an out or ref parameter)
                    bool LocalBool = false;
                    int LocalInt = 0;

                    //Loop through the lines to process them
                    for (int i = 0; i < EDFlines.Length; i++)
                    {

                        //Fill out the local until we get a new new heading (starts with[)
                        // then save the local to the list and start again

                        //If this is a new definition
                        if (EDFlines[i].StartsWith("["))
                        {
                            //New definition so start to fill out the local
                            LocalED = new EngineData(); //Call it's constructor to init it

                            //We are currently sat on the [<name>] line so lets get the name
                            int j = EDFlines[i].IndexOf('=');               //Find the end of label string
                            string line = EDFlines[i].Substring(j + 1, EDFlines[i].Length - (j + 1));    //Grab the bit after the '='
                            LocalED._Name = line.Trim(' ');                 //Remove the leading on trailing spaces and store the name of the new definition
                            i++;    //Move it along to the next line, so the if condition check doesn't end it immediately

                            //TO-DO wrap the if here in a loop
                            while ((i < EDFlines.Length) && (!(EDFlines[i].StartsWith("["))))   //Keep reading lines until another section header line is found or out of lines
                            {

                                //Check for blank lines and null lines (end of file(might be able to remove the null check, legacy from a stream reader style))
                                if ((EDFlines[i] != "") && (EDFlines[i] != null) && (!(EDFlines[i].StartsWith(";"))))    //if the line is empty or a comment skip over all this
                                {
                                    j = EDFlines[i].IndexOf('=');              //Find the end of label string
                                    string label = EDFlines[i].Substring(0, j);    //Grabs the bit upto the '='
                                                                                   //Grab the bit after the '=' and remove the leading and trailing spaces
                                    line = EDFlines[i].Substring(j + 1, EDFlines[i].Length - (j + 1)).Trim(' ');

                                    switch (label)  //Fill out the Main data
                                    {
                                        case "blockOBD":
                                            bool.TryParse(line, out LocalBool);     //convert the strings to a bool
                                            LocalED._BlockOBD = LocalBool;          //copy into the local object (we cannot use it directly with 'out')
                                            break;
                                        case "engineSound":
                                            LocalED._EngineSound = line;
                                            break;
                                        case "maxPower":
                                            int.TryParse(line, out LocalInt);       //convert the strings to a number
                                            LocalED._maxPower = LocalInt;           //copy into the local object (we cannot use it directly with 'out')
                                            break;
                                        case "maxPowerRPM":
                                            int.TryParse(line, out LocalInt);       //convert the strings to a number
                                            LocalED._maxPowerRPM = LocalInt;        //copy into the local object (we cannot use it directly with 'out')
                                            break;
                                        case "maxTorqueRPM":
                                            int.TryParse(line, out LocalInt);       //convert the strings to a number
                                            LocalED._maxTorqueRPM = LocalInt;       //copy into the local object (we cannot use it directly with 'out')
                                            break;
                                        case "minRPM":
                                            int.TryParse(line, out LocalInt);       //convert the strings to a number
                                            LocalED._minRPM = LocalInt;             //copy into the local object (we cannot use it directly with 'out')
                                            break;
                                        case "maxRPM":
                                            int.TryParse(line, out LocalInt);       //convert the strings to a number
                                            LocalED._maxRPM = LocalInt;             //copy into the local object (we cannot use it directly with 'out')
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
                                    i++;    //Move to next line
                                }
                            }
                            i--;    //Knock the counter back a line as the while loop that called us, will inc it and step over the section header we just found.
                            ModEngineDataList.Add(LocalED);    //Add full definition to the list
                        }
                    }
                    //Now we have a full list, copy over the wanted one

                    //Check the file isn't empty
                    if (ModEngineDataList.Count > 0)
                    {
                        string SelectedEngine = AvailableEnginesComboBox.Text;  //Get the currently selected engine

                        //Loop through each entry
                        foreach (EngineData Eng in ModEngineDataList)
                        {
                            if (Eng._Name == SelectedEngine) //If the name matches
                            {
                                //Set the data fields with the values from the selected engine
                                EDTBlockOBDcheckBox.Checked = Eng._BlockOBD;
                                EDTEngineSoundtextBox.Text = Eng._EngineSound;
                                EDTmaxPowerNumericUpDown.Value = Eng._maxPower;
                                EDTmaxPowerRPMNumericUpDown.Value = Eng._maxPowerRPM;
                                EDTmaxTorqueRPMNumericUpDown.Value = Eng._maxTorqueRPM;
                                EDTminRPMLabelNumericUpDown.Value = Eng._minRPM;
                                EDTmaxRPMLabelNumericUpDown.Value = Eng._maxRPM;

                                //Write the file by calling the save button
                                EDTCommitButton_Click(sender, e);

                                break;  //Exit the foreach loop
                            }
                        }
                    }
                }   //File.Exists check
            }
        }

        //Handles a call to restore all engines to mods
        private void EDTRestoreAllModbutton_Click(object sender, EventArgs e)
        {
            //Prompt the user to see if they are sure
            DialogResult PromptResult = MessageBox.Show("This will restore all engines to those from the mod engine data file\n\nAre you sure?", "Restore All Mods To Engine Data", MessageBoxButtons.YesNo);

            if (PromptResult == DialogResult.Yes)
            {
                // overwrite the destination file if it already exists.
                System.IO.File.Copy(ModMan.GetCarsDataDirBkUpDefault() + "\\engines.txt", ModMan.GetCarsDataDir() + "\\engines.txt", true);
            }
        }

        //Backup the all engine mods
        private void EDTBackupAllModbutton_Click(object sender, EventArgs e)
        {
            //Prompt the user to see if they are sure
            DialogResult PromptResult = MessageBox.Show("This will overwrite all the exisiting backed up modified engines\nwith the ones currently in the active folder\n\nAre you sure?", "Backup Mod Engine Data", MessageBoxButtons.YesNo);

            if (PromptResult == DialogResult.Yes)
            {
                // overwrite the destination file if it already exists.
                System.IO.File.Copy(ModMan.GetCarsDataDir() + "\\engines.txt", ModMan.GetCarsDataDirBkUpDefault() + "\\engines.txt", true);
            }
        }

        #endregion

        #region Tire stuff
        //Load and parse the engine data file
        public void LoadTireDataFile()
        {
            //Loads the file that contains the engine definitions

            //Setup the filename
            string FileName = ModMan.GetCarsDataDir() + "\\tires.txt";

            //Check if the config file exists
            if (File.Exists(FileName))
            {
                //Load the whole file
                string[] TDFlines = System.IO.File.ReadAllLines(FileName);
                //Local to fill out before adding to the EngineDataList
                TireData LocalTD;
                //Locals to fill out before putting into LocalED (A property or indexer may not be passed as an out or ref parameter)
                int LocalInt = 0;
                float LocalFloat = 0;

                //Loop through the lines to process them
                for (int i = 0; i < TDFlines.Length; i++)
                {

                    //Fill out the local until we get a new new heading (starts with[)
                    // then save the local to the list and start again

                    //If this is a new definition
                    if (TDFlines[i].StartsWith("["))
                    {
                        //New definition so start to fill out the local
                        LocalTD = new TireData(); //Call it's constructor to init it

                        //We are currently sat on the [<name>] line so lets get the name
                        int j = TDFlines[i].IndexOf('=');               //Find the end of label string
                        string line = TDFlines[i].Substring(j + 1, TDFlines[i].Length - (j + 1));    //Grab the bit after the '='
                        LocalTD._Name = line.Trim(' ');                 //Remove the leading on trailing spaces and store the name of the new definition
                        i++;    //Move it along to the next line, so the if condition check doesn't end it immediately

                        //TO-DO wrap the if here in a loop
                        while ((i < TDFlines.Length) && (!(TDFlines[i].StartsWith("["))))   //Keep reading lines until another section header line is found or out of lines
                        {

                            //Check for blank lines and null lines (end of file(might be able to remove the null check, legacy from a stream reader style))
                            if ((TDFlines[i] != "") && (TDFlines[i] != null) && (!(TDFlines[i].StartsWith(";"))))    //if the line is empty or a comment skip over all this
                            {
                                j = TDFlines[i].IndexOf('=');              //Find the end of label string
                                string label = TDFlines[i].Substring(0, j);    //Grabs the bit upto the '='
                                                                               //Grab the bit after the '=' and remove the leading and trailing spaces
                                line = TDFlines[i].Substring(j + 1, TDFlines[i].Length - (j + 1)).Trim(' ');

                                switch (label)  //Fill out the Main data
                                {
                                    case "gripMod":
                                        float.TryParse(line, out LocalFloat);   //convert the strings to a number
                                        LocalTD._gripMod = LocalFloat;
                                        break;
                                    case "price":
                                        int.TryParse(line, out LocalInt);   //convert the strings to a number
                                        LocalTD._price = LocalInt;          //copy into the local object (we cannot use it directly with 'out')
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
                                i++;    //Move to next line
                            }
                        }
                        i--;    //Knock the counter back a line as the while loop that called us, will inc it and step over the section header we just found.
                        TireDataList.Add(LocalTD);    //Add full definition to the list
                    }
                }
            }
        }

        //Populate the engine data drop down combo box
        private void PopulateAvailableTiresComboBox()
        {
            int index = 0;  //local loop counter
            //Loop through all of the loaded engines
            while (index < TireDataList.Count)
            {
                //Add an item to the engines data combo box
                AvailableTirescomboBox.Items.Add(TireDataList[index]._Name);
                index++;    //inc counter
            }
        }
        
        //Handle a selection in the engines combo box
        private void AvailableTirescomboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            //Get the index of the selected engine
            int Index = AvailableTirescomboBox.SelectedIndex;

            //Show the picture file
            //Assemble the path and filename
            string PicturePath = null;
            string SelectedCar = AvailableTirescomboBox.Text;
            SelectedCar = SelectedCar.Substring(1, SelectedCar.Length - 2);
            PicturePath = ModMan.GetConfigDir() + "\\Images\\" + SelectedCar + ".png";
            //PicturePath = PicturePath + ".png";                                  //Add jpg

            //Check if there is a thumbnail picture present to use
            if (File.Exists(PicturePath))
            {
                Image image = Image.FromFile(PicturePath);  //Use the picture if it exists
                TDTImagepictureBox.Image = image;
            }
            else
            {
                PicturePath = ModMan.GetCarsDataDir() + "\\car_placeholder.jpg";
                Image image = Image.FromFile(PicturePath);  //Use the place holder image
                TDTImagepictureBox.Image = image;
            }

            //Set the data fields with the values from the selected engine
            TDTGripModnumericUpDown.Value = (decimal)TireDataList[Index]._gripMod;
            TDTPricenumericUpDown.Value = TireDataList[Index]._price;
        }

        //Send the engine data fields to the engine data file (changes internal values which the reset reads from)
        private void TDTCommitbutton_Click(object sender, EventArgs e)
        {
            //Check if ok to save file
            if (MessageBox.Show("Update Tire Data file?\nReset will use these values from now on", "Confirm commit", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                //Get the index of the selected engine
                int Index = AvailableTirescomboBox.SelectedIndex;

                //send the new engine data values
                TireDataList[Index]._gripMod = (float)TDTGripModnumericUpDown.Value;      //Need to cast the decimal to an int
                TireDataList[Index]._price = (int)TDTPricenumericUpDown.Value;

                //write to file
                Index = 0;  //reset counter used to index through engine data array

                //Setup the filename
                string FileName = ModMan.GetCarsDataDir() + "\\tires.txt";
                //Create a local file writer
                StreamWriter writer = new StreamWriter(FileName);

                //Write out each tire in turn
                while (Index < TireDataList.Count)
                {
                    writer.WriteLine(TireDataList[Index]._Name);
                    writer.WriteLine("gripMod=" + TireDataList[Index]._gripMod);
                    writer.WriteLine("price=" + TireDataList[Index]._price);
                    writer.WriteLine();     //Blank line seperator

                    Index++;    //Increment counter
                }

                writer.WriteLine();     //Blank line seperator
                writer.WriteLine("\nMade with= CMS15ModManager {0}", ModManVersion);
                //we are finished with the writer so close and bin it
                writer.Close();
                writer.Dispose();
            }
        }
        
        //Reset all the tire data fields, from stored values
        private void TDTResetbutton_Click(object sender, EventArgs e)
        {
            //Prompt the user to see if they are sure
            DialogResult PromptResult = MessageBox.Show("This will reset all values to those from the existing tire data file", "Reset Tire Data", MessageBoxButtons.YesNo);

            if (PromptResult == DialogResult.Yes)
            {
                //Reset the engine data values, just call the other function that sets them initialy
                AvailableTirescomboBox_SelectedIndexChanged(sender, e);
            }
        }

        //Handles a call to restore a single tire definition from the default folder
        private void TDTRestoreDefaultbutton_Click(object sender, EventArgs e)
        {
            //Messing and dulicating doing this here, but I don't feel like redesigning the TireData class to have a read function right now

            //Prompt the user to see if they are sure
            DialogResult PromptResult = MessageBox.Show("This will restore this tires values to those from the default tire data file\n\nAre you sure?", "Restore Tire Data", MessageBoxButtons.YesNo);

            if (PromptResult == DialogResult.Yes)
            {
                //Create a local Engine Data object
                List<TireData> DefaultTireDataList = new List<TireData>();

                //Loads the file that contains the default engine definitions

                //Setup the filename
                string FileName = ModMan.GetCarsDataDirBkUpDefault() + "\\tires.txt";
                //Check if the config file exists
                if (File.Exists(FileName))
                {
                    //Load the whole file
                    string[] TDFlines = System.IO.File.ReadAllLines(FileName);
                    //Local to fill out before adding to the EngineDataList
                    TireData LocalTD;
                    //Locals to fill out before putting into LocalED (A property or indexer may not be passed as an out or ref parameter)
                    float LocalFloat = 0;
                    int LocalInt = 0;

                    //Loop through the lines to process them
                    for (int i = 0; i < TDFlines.Length; i++)
                    {

                        //Fill out the local until we get a new new heading (starts with[)
                        // then save the local to the list and start again

                        //If this is a new definition
                        if (TDFlines[i].StartsWith("["))
                        {
                            //New definition so start to fill out the local
                            LocalTD = new TireData(); //Call it's constructor to init it

                            //We are currently sat on the [<name>] line so lets get the name
                            int j = TDFlines[i].IndexOf('=');               //Find the end of label string
                            string line = TDFlines[i].Substring(j + 1, TDFlines[i].Length - (j + 1));    //Grab the bit after the '='
                            LocalTD._Name = line.Trim(' ');                 //Remove the leading on trailing spaces and store the name of the new definition
                            i++;    //Move it along to the next line, so the if condition check doesn't end it immediately

                            //TO-DO wrap the if here in a loop
                            while ((i < TDFlines.Length) && (!(TDFlines[i].StartsWith("["))))   //Keep reading lines until another section header line is found or out of lines
                            {

                                //Check for blank lines and null lines (end of file(might be able to remove the null check, legacy from a stream reader style))
                                if ((TDFlines[i] != "") && (TDFlines[i] != null) && (!(TDFlines[i].StartsWith(";"))))    //if the line is empty or a comment skip over all this
                                {
                                    j = TDFlines[i].IndexOf('=');              //Find the end of label string
                                    string label = TDFlines[i].Substring(0, j);    //Grabs the bit upto the '='
                                                                                   //Grab the bit after the '=' and remove the leading and trailing spaces
                                    line = TDFlines[i].Substring(j + 1, TDFlines[i].Length - (j + 1)).Trim(' ');

                                    switch (label)  //Fill out the Main data
                                    {
                                        case "gripMod":
                                            float.TryParse(line, out LocalFloat);   //convert the strings to a number
                                            LocalTD._gripMod = LocalFloat;
                                            break;
                                        case "price":
                                            int.TryParse(line, out LocalInt);   //convert the strings to a number
                                            LocalTD._price = LocalInt;          //copy into the local object (we cannot use it directly with 'out')
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
                                    i++;    //Move to next line
                                }
                            }
                            i--;    //Knock the counter back a line as the while loop that called us, will inc it and step over the section header we just found.
                            DefaultTireDataList.Add(LocalTD);    //Add full definition to the list
                        }
                    }
                    //Now we have a full list, copy over the wanted one

                    //Check the file isn't empty
                    if (DefaultTireDataList.Count > 0)
                    {
                        string SelectedEngine = AvailableEnginesComboBox.Text;  //Get the currently selected engine

                        //Loop through each entry
                        foreach (TireData Tire in DefaultTireDataList)
                        {
                            if (Tire._Name == SelectedEngine) //If the name matches
                            {
                                //Set the data fields with the values from the selected engine
                                TDTGripModnumericUpDown.Value = (decimal)Tire._gripMod;
                                TDTPricenumericUpDown.Value = Tire._price;

                                //Write the file by calling the save button
                                EDTCommitButton_Click(sender, e);

                                break;  //Exit the foreach loop
                            }
                        }
                    }
                }   //File.Exists check
            }
        }

        //Handles a call to restore all tires to default
        private void TDTRestoreAllDefaultbutton_Click(object sender, EventArgs e)
        {
            //Prompt the user to see if they are sure
            DialogResult PromptResult = MessageBox.Show("This will restore all tires to those from the default engine data file\n\nAre you sure?", "Restore All Tire Data", MessageBoxButtons.YesNo);

            if (PromptResult == DialogResult.Yes)
            {
                // overwrite the destination file if it already exists.
                System.IO.File.Copy(ModMan.GetCarsDataDirBkUpDefault() + "\\tires.txt", ModMan.GetCarsDataDir() + "\\tires.txt", true);
            }
        }

        //Handles a call to restore a single tire to a mod
        private void TDTRestoreModbutton_Click(object sender, EventArgs e)
        {

            //Prompt the user to see if they are sure
            DialogResult PromptResult = MessageBox.Show("This will restore this tires values to those from the default tire data file\n\nAre you sure?", "Restore Tire Data", MessageBoxButtons.YesNo);

            if (PromptResult == DialogResult.Yes)
            {
                //Create a local Engine Data object
                List<TireData> DefaultTireDataList = new List<TireData>();

                //Loads the file that contains the default engine definitions

                //Setup the filename
                string FileName = ModMan.GetCarsDataDirBkUpMod() + "\\tires.txt";
                //Check if the config file exists
                if (File.Exists(FileName))
                {
                    //Load the whole file
                    string[] TDFlines = System.IO.File.ReadAllLines(FileName);
                    //Local to fill out before adding to the EngineDataList
                    TireData LocalTD;
                    //Locals to fill out before putting into LocalED (A property or indexer may not be passed as an out or ref parameter)
                    float LocalFloat = 0;
                    int LocalInt = 0;

                    //Loop through the lines to process them
                    for (int i = 0; i < TDFlines.Length; i++)
                    {

                        //Fill out the local until we get a new new heading (starts with[)
                        // then save the local to the list and start again

                        //If this is a new definition
                        if (TDFlines[i].StartsWith("["))
                        {
                            //New definition so start to fill out the local
                            LocalTD = new TireData(); //Call it's constructor to init it

                            //We are currently sat on the [<name>] line so lets get the name
                            int j = TDFlines[i].IndexOf('=');               //Find the end of label string
                            string line = TDFlines[i].Substring(j + 1, TDFlines[i].Length - (j + 1));    //Grab the bit after the '='
                            LocalTD._Name = line.Trim(' ');                 //Remove the leading on trailing spaces and store the name of the new definition
                            i++;    //Move it along to the next line, so the if condition check doesn't end it immediately

                            //TO-DO wrap the if here in a loop
                            while ((i < TDFlines.Length) && (!(TDFlines[i].StartsWith("["))))   //Keep reading lines until another section header line is found or out of lines
                            {

                                //Check for blank lines and null lines (end of file(might be able to remove the null check, legacy from a stream reader style))
                                if ((TDFlines[i] != "") && (TDFlines[i] != null) && (!(TDFlines[i].StartsWith(";"))))    //if the line is empty or a comment skip over all this
                                {
                                    j = TDFlines[i].IndexOf('=');              //Find the end of label string
                                    string label = TDFlines[i].Substring(0, j);    //Grabs the bit upto the '='
                                                                                   //Grab the bit after the '=' and remove the leading and trailing spaces
                                    line = TDFlines[i].Substring(j + 1, TDFlines[i].Length - (j + 1)).Trim(' ');

                                    switch (label)  //Fill out the Main data
                                    {
                                        case "gripMod":
                                            float.TryParse(line, out LocalFloat);   //convert the strings to a number
                                            LocalTD._gripMod = LocalFloat;
                                            break;
                                        case "price":
                                            int.TryParse(line, out LocalInt);   //convert the strings to a number
                                            LocalTD._price = LocalInt;          //copy into the local object (we cannot use it directly with 'out')
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
                                    i++;    //Move to next line
                                }
                            }
                            i--;    //Knock the counter back a line as the while loop that called us, will inc it and step over the section header we just found.
                            DefaultTireDataList.Add(LocalTD);    //Add full definition to the list
                        }
                    }
                    //Now we have a full list, copy over the wanted one

                    //Check the file isn't empty
                    if (DefaultTireDataList.Count > 0)
                    {
                        string SelectedEngine = AvailableEnginesComboBox.Text;  //Get the currently selected engine

                        //Loop through each entry
                        foreach (TireData Tire in DefaultTireDataList)
                        {
                            if (Tire._Name == SelectedEngine) //If the name matches
                            {
                                //Set the data fields with the values from the selected engine
                                TDTGripModnumericUpDown.Value = (decimal)Tire._gripMod;
                                TDTPricenumericUpDown.Value = Tire._price;

                                //Write the file by calling the save button
                                EDTCommitButton_Click(sender, e);

                                break;  //Exit the foreach loop
                            }
                        }
                    }
                }   //File.Exists check
            }
        }

        //Handles a call to restore all tires to mods
        private void TDTRestoreAllModbutton_Click(object sender, EventArgs e)
        {
            //Prompt the user to see if they are sure
            DialogResult PromptResult = MessageBox.Show("This will restore all tires to those from the mod engine data file\n\nAre you sure?", "Restore All Mods To Tire Data", MessageBoxButtons.YesNo);

            if (PromptResult == DialogResult.Yes)
            {
                // overwrite the destination file if it already exists.
                System.IO.File.Copy(ModMan.GetCarsDataDirBkUpDefault() + "\\tires.txt", ModMan.GetCarsDataDir() + "\\tires.txt", true);
            }
        }

        //Backup the all engine mods
        private void TDTBackupAllModbutton_Click(object sender, EventArgs e)
        {
            //Prompt the user to see if they are sure
            DialogResult PromptResult = MessageBox.Show("This will overwrite all the exisiting backed up modified tires\nwith the ones currently in the active folder\n\nAre you sure?", "Backup Mod Tire Data", MessageBoxButtons.YesNo);

            if (PromptResult == DialogResult.Yes)
            {
                // overwrite the destination file if it already exists.
                System.IO.File.Copy(ModMan.GetCarsDataDir() + "\\tires.txt", ModMan.GetCarsDataDirBkUpDefault() + "\\tires.txt", true);
            }
        }
        #endregion

        #region Car List stuff
        //Populate the cars currently in the list, list box
        private void PopulateCarsCurrentList()
        {
            int index = 0;      //local loop counter
            string name = null; //local to assemble string for list
            //Empty out current contents
            CarsCurrentlyListBox.Items.Clear();

            //Loop through all of the car in the list
            while (index < ModMan.GetCarsCurrentArraySize())
            {
                //Assemble the name
                name = ModMan.GetCarsCurrentInternal(index) + " (" + ModMan.GetCarsCurrentExternal(index) + ")";

                //Add an item to the cars in list, list box
                CarsCurrentlyListBox.Items.Add(name);
                index++;    //inc counter
            }
        }

        //Populate the cars Available list box
        private void PopulateCarsAvailableList()
        {
            int index = 0;      //local loop counter
            //Empty out current list
            AvailableCarsListBox.Items.Clear();

            //Loop through all of the Available car file
            while (index < ModMan.GetCarsAvailableArraySize())
            {
                //Add an item to the cars Available list box
                AvailableCarsListBox.Items.Add(ModMan.GetCarsAvailableFilesList(index));

                index++;    //inc counter
            }
        }

        //Removes a car from the current list
        private void CarListRemoveButton_Click(object sender, EventArgs e)
        {
            //Get the index of the selected car data file
            int Index = CarsCurrentlyListBox.SelectedIndex;
            //Check if a line has been selected
            if (Index > -1)
            {
                //Remove from current list
                ModMan.RemoveFromCurrent(Index);
                //Need to update the available list
                //Clear out the list
                CarsCurrentlyListBox.Items.Clear();
                //Repopulate it
                PopulateCarsCurrentList();
            }
        }

        //Adds a car to the Current list from the Available list
        private void CarListAddButton_Click(object sender, EventArgs e)
        {
            //Get the index of the selected engine
            int Index = AvailableCarsListBox.SelectedIndex;
            //Check if a line has been selected
            if (Index > -1)
            {
                //local to hold return from dialog box
                string InGameName = "Name Here";

                //This is a inefficient, loading the whole file for one value, but as I have the function already
                //Load the selected data file
                CarDataObject.LoadCarDataFile(ModMan.GetCarsDataDir() + "\\" + AvailableCarsListBox.Text);

                //ModMan.LoadCarDataFromFile(AvailableCarsListBox.Text);
                //if (ModMan.LoadedCarData.Main.name != "")
                if (CarDataObject._MainName != "")
                {
                    //If name isn't null fill it in
                    InGameName = CarDataObject._MainName;
                }

                //Use a dialog box to get an in game name
                if (InputBox("Enter car name", "Enter in game visible name here:", ref InGameName) == DialogResult.OK)
                {
                    //Remove ".txt" from end
                    string PassName = AvailableCarsListBox.Text.Substring(0, AvailableCarsListBox.Text.Length - 4);

                    //Add to list
                    ModMan.AddToCurrent(PassName, InGameName);

                    //Need to update the current list
                    //Clear out the list
                    CarsCurrentlyListBox.Items.Clear();
                    //Repopulate it
                    PopulateCarsCurrentList();
                }
            }
        }

        //Handles a call to save the cars currently in the list
        private void CarsListCommitButton_Click(object sender, EventArgs e)
        {
            //Check if ok to save file
            if (MessageBox.Show("Update In game car list file?\nRefresh will use these values from now on", "Confirm commit", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                //write to file
                ModMan.WriteCarsCurrent();
                //Update the list the Car Data Tab uses
                PopulateAvailableCarsComboBox();
            }
        }

        //Handles a call to update both list on the cars list tab
        private void CarsListRefreshButton_Click(object sender, EventArgs e)
        {
            //Need to update the current list
            //Clear out the list
            CarsCurrentlyListBox.Items.Clear();
            //Repopulate it
            PopulateCarsCurrentList();

            //Need to update the available list
            //Clear out the list
            AvailableCarsListBox.Items.Clear();
            //Repopulate it
            PopulateCarsAvailableList();
        }

        //Handles a call to reload the list from the dir and file
        private void CarsListReloadButton_Click(object sender, EventArgs e)
        {
            //Empty the internal lists
            ModMan.EmptyCurrentAvailableCarLists();

            //load the list of cars file
            ModMan.ReadCarsCurrent();
            //Find car data files
            ModMan.FindCarsFiles();

            //Need to update the current list
            //Clear out the list
            CarsCurrentlyListBox.Items.Clear();
            //Repopulate it
            PopulateCarsCurrentList();

            //Need to update the available list
            //Clear out the list
            AvailableCarsListBox.Items.Clear();
            //Repopulate it
            PopulateCarsAvailableList();
        }

        //Handle a call to import a new car data file
        private void CarsListImportbutton_Click(object sender, EventArgs e)
        {
            //Open up a file browser
            OpenFileDialog ofd = new OpenFileDialog();
            // Show the dialog and get result.
            DialogResult result = ofd.ShowDialog();
            if (result == DialogResult.OK) // Test result.
            {
                //Copy the file
                ModMan.FileCopy(ofd.FileName);

                //Need to update the available list
                //Empty the internal lists
                ModMan.EmptyCurrentAvailableCarLists();
                //Clear out the list
                AvailableCarsListBox.Items.Clear();
                //Find car data files
                ModMan.FindCarsFiles();
                //Repopulate it
                PopulateCarsAvailableList();
            }
        }

        //Handle a call to copy (clone) a car data file
        private void CarListCopybutton_Click(object sender, EventArgs e)
        {
            //Get the index of the selected car data file
            int Index = AvailableCarsListBox.SelectedIndex;
            //Check if a line has been selected
            if (Index > -1)
            {
                //local to hold return from dialog box
                string NewName = "Name Here";
                //Use a dialog box to get an in game name
                if (InputBox("Enter filename", "Enter new car filename here:", ref NewName) == DialogResult.OK)
                {
                    //Need to assemble the full path
                    string DestFullFileName = ModMan.GetCarsDataDir() + "\\" + NewName + ".txt";
                    //Need to assemble the full path
                    string SourceFullFileName = ModMan.GetCarsDataDir() + "\\" + AvailableCarsListBox.Text;

                    //Copy to new file
                    ModMan.OverwriteFile(SourceFullFileName, DestFullFileName);

                    //Need to update the available list
                    //Empty the internal lists
                    ModMan.EmptyCurrentAvailableCarLists();
                    //Clear out the list
                    AvailableCarsListBox.Items.Clear();
                    //Find car data files
                    ModMan.FindCarsFiles();
                    //Repopulate it
                    PopulateCarsAvailableList();
                }
            }
        }

        //Handle a call to overwrite a car data file
        private void CarListOverwritebutton_Click(object sender, EventArgs e)
        {
            //Get the index of the selected car data file
            int Index = AvailableCarsListBox.SelectedIndex;
            //Check if a line has been selected
            if (Index > -1)
            {
                //Open up a file browser
                OpenFileDialog ofd = new OpenFileDialog();
                // Show the dialog and get result.
                DialogResult result = ofd.ShowDialog();
                if (result == DialogResult.OK) // Test result.
                {
                    //Need to assemble the full path
                    string DestFullFileName = ModMan.GetCarsDataDir() + "\\" + AvailableCarsListBox.Text;
                    //Need to assemble the full path
                    //string SourceFullFileName = ModMan.GetCarsDataDir() + "\\" + AvailableCarsListBox.Text;
                    string SourceFullFileName = ofd.FileName;

                    //Overwrite the file
                    ModMan.OverwriteFile(SourceFullFileName, DestFullFileName);

                    //Need to update the available list
                    //Empty the internal lists
                    ModMan.EmptyCurrentAvailableCarLists();
                    //Clear out the list
                    AvailableCarsListBox.Items.Clear();
                    //Find car data files
                    ModMan.FindCarsFiles();
                    //Repopulate it
                    PopulateCarsAvailableList();
                }
            }
        }

        //Handles a call to remove a car data file
        private void CarsListDeletebutton_Click(object sender, EventArgs e)
        {
            //Get the index of the selected car data file
            int Index = AvailableCarsListBox.SelectedIndex;
            //Check if a line has been selected
            if (Index > -1)
            {
                //Need to assemble the full path
                string FullFileName = ModMan.GetCarsDataDir() + "\\" + AvailableCarsListBox.Text;
                //Delete the file
                File.Delete(FullFileName);

                //Need to update the available list
                //Empty the internal lists
                ModMan.EmptyCurrentAvailableCarLists();
                //Clear out the list
                AvailableCarsListBox.Items.Clear();
                //Find car data files
                ModMan.FindCarsFiles();
                //Repopulate it
                PopulateCarsAvailableList();

                //Car Data Tab
                //List of available car text file already loaded
                //Populate the available cars drop down list
                PopulateAvailableCarsComboBox();
            }
        }
        #endregion

        #region Car Data stuff
        //Populate the available car files drop down combo box
        private void PopulateAvailableCarsComboBox()
        {
            //Clear down the existing list
            AvailableCarsDataComboBox.Items.Clear();

            int index = 0;  //local loop counter
            //Loop through all of the loaded engines
            while (index < ModMan.GetCarsAvailableArraySize())
            {
                //Add an item to the engines data combo box
                AvailableCarsDataComboBox.Items.Add(ModMan.GetCarsAvailableFilesList(index));
                index++;    //inc counter
            }

            //Populate engine options
            foreach (EngineData type in EngineDataList)
            {
                string temp = type._Name.Substring(1, type._Name.Length - 2).Trim();  //Remove the leading on trailing spaces
                CDETypecomboBox.Items.Add(temp);
            }

            //Populate tire options
            foreach (TireData type in TireDataList)
            {
                string temp = type._Name.Substring(1, type._Name.Length - 2).Trim();  //Remove the leading on trailing spaces
                CDWTirecomboBox.Items.Add(temp);
            }
        }

        //Handle a selection in the available cars combo box
        private void AvailableCarsDataComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            //Get the index of the selected engine
            //int Index = AvailableCarsDataComboBox.SelectedIndex;
            //It sorts the alphabetically, which screws up the index number to internal data relationship

            //Show the picture file
            //Assemble the path and filename
            string PicturePath = null;
            string SelectedCar = AvailableCarsDataComboBox.Text;
            PicturePath = ModMan.GetCarsDataDir() + "\\" + SelectedCar;
            PicturePath = PicturePath.Substring(0, PicturePath.Length - 3);     //Remove txt
            PicturePath = PicturePath + "jpg";                                  //Add jpg

            //Update the GUI to display it
            //Clear down all the GUI to remove old values
            ClearOutCarDataTabGUI();

            //Check if there is a thumbnail picture present to use
            if (File.Exists(PicturePath))
            {
                Image image = Image.FromFile(PicturePath);  //Use the picture if it exists
                CarDataThumbNailPictureBox.Image = image;
            }
            else
            {
                PicturePath = ModMan.GetCarsDataDir() + "\\car_placeholder.jpg";
                Image image = Image.FromFile(PicturePath);  //Use the place holder image
                CarDataThumbNailPictureBox.Image = image;
            }

            //Load the selected data file
            CarDataObject.LoadCarDataFile(ModMan.GetCarsDataDir() + "\\" + SelectedCar);

            //Fill out the GUI from the CarDataObject
            FillOutCarDataTabGUI();
        }

        //Resets the Car Data tab GUI elements
        public void ClearOutCarDataTabGUI()
        {
            //Drop Down box
            //AvailableCarsDataComboBox.Text = "";

            //Picture box
            string PicturePath = ModMan.GetCarsDataDir() + "\\car_placeholder.jpg";
            Image image = Image.FromFile(PicturePath);  //Use the place holder image
            CarDataThumbNailPictureBox.Image = image;

            //[Main] section
            CDMNameTextBox.Text = "";
            CDMModelTextBox.Text = "";
            CDMRustMaskTextBox.Text = "";
            CDMRotXnumericUpDown.Value = 0;
            CDMRotYnumericUpDown.Value = 0;
            CDMRotZnumericUpDown.Value = 0;

            //[Other] section
            CDOTranstextBox.Text = "";
            CDOGearsnumericUpDown.Value = 0;
            CDOFinalDrivenumericUpDown.Value = 0;
            CDOWeightnumericUpDown.Value = 0;
            CDORPMFactornumericUpDown.Value = 0;
            CDORPMAnglenumericUpDown.Value = 0;
            CDOSpeedoAnglenumericUpDown.Value = 0;
            CDOSuspTRavelnumericUpDown.Value = 0;
            CDOLifterArmsRisenumericUpDown.Value = 0;
            CDOCXNumericUpDown.Value = 0;
            CDORightHandDrivecheckBox.Checked = false;
            CDOTransVolnumericUpDown.Value = 0;
            CDODeleteOnLoadtextBox.Text = "";
            CDOTuneOnLoadtextBox.Text = "";
            CDOColorMattecheckBox.Checked = false;

            //[Suspension] section
            CDSFrontAxleStartnumericUpDown.Value = 0;
            CDSWheelBasenumericUpDown.Value = 0;
            CDSHeightnumericUpDown.Value = 0;
            CDSHeightRearnumericUpDown.Value = 0;
            CDSFrontTracknumericUpDown.Value = 0;
            CDSRearTracknumericUpDown.Value = 0;
            CDSFrontSpringLengthnumericUpDown.Value = 0;
            //Kaszlak stuff
            CDSForceScalenumericUpDown.Value = 0;
            CDSForceScaleRearnumericUpDown.Value = 0;
            CDSFrontCenterSettextBox.Text = "";
            CDSFrontRightSettextBox.Text = "";
            CDSRearCenterSettextBox.Text = "";
            CDSRearRightSettextBox.Text = "";

            //[Engine] section
            CDEPosXnumericUpDown.Value = 0;
            CDEPosYnumericUpDown.Value = 0;
            CDEPosZnumericUpDown.Value = 0;
            CDERotXnumericUpDown.Value = 0;
            CDERotYnumericUpDown.Value = 0;
            CDERotZnumericUpDown.Value = 0;
            CDEScalenumericUpDown.Value = 0;
            CDETypecomboBox.SelectedText = "";
            CDETypecomboBox.Text = "";
            CDEPMnumericUpDown.Value = 0;
            //Engine swaps handled by the swap options dialog box

            //[Driveshaft] section
            CDDPosXnumericUpDown.Value = 0;
            CDDPosYnumericUpDown.Value = 0;
            CDDPosZnumericUpDown.Value = 0;
            CDDRotXnumericUpDown.Value = 0;
            CDDRotYnumericUpDown.Value = 0;
            CDDRotZnumericUpDown.Value = 0;
            CDDScalenumericUpDown.Value = 0;
            CDDLengthnumericUpDown.Value = 0;
            CDDTypetextBox.Text = "";

            //[Wheel] section
            CDWWheelWidthnumericUpDown.Value = 0;
            CDWRimSizenumericUpDown.Value = 0;
            CDWTireSizenumericUpDown.Value = 0;
            CDWTirecomboBox.SelectedText = "";
            CDWTirecomboBox.Text = "";
            CDWRimtextBox.Text = "";
            CDWCambernumericUpDown.Value = 0;
            //Rear stuff in front wheel area
            //Maserati only, seems to crash game otherwise
            CDWWheelWidthRearnumericUpDown.Value = 0;
            CDWTireSizeRearnumericUpDown.Value = 0;
            CDWRimSizeRearnumericUpDown.Value = 0;
            //Disable the Maserati stuff, it'll be re-enabled if the selected vehicle is a Maserati
            CDWWheelWidthRearnumericUpDown.Enabled = false;
            CDWTireSizeRearnumericUpDown.Enabled = false;
            CDWRimSizeRearnumericUpDown.Enabled = false;

            //[Wheel_Rear] section
            CDWRWheelWidthnumericUpDown.Value = 0;
            CDWRRimSizenumericUpDown.Value = 0;
            CDWRTireSizenumericUpDown.Value = 0;
            CDWRTirecomboBox.SelectedText = "";
            CDWRTirecomboBox.Text = "";
            CDWRRimtextBox.Text = "";
            CDWRimCaptextBox.Text = "";
            CDWRCambernumericUpDown.Value = 0;

            //[Interior] section
            CDISLPosXnumericUpDown.Value = 0;
            CDISLPosYnumericUpDown.Value = 0;
            CDISLPosZnumericUpDown.Value = 0;
            CDISLRotXnumericUpDown.Value = 0;
            CDISLRotYnumericUpDown.Value = 0;
            CDISLRotZnumericUpDown.Value = 0;
            CDISeatScalenumericUpDown.Value = 0;
            CDISeattextBox.Text = "";
            CDIWheeltextBox.Text = "";
            CDIWheelPosXnumericUpDown.Value = 0;
            CDIWheelPosYnumericUpDown.Value = 0;
            CDIWheelPosZnumericUpDown.Value = 0;
            CDIWheelRotXnumericUpDown.Value = 0;
            CDIWheelRotYnumericUpDown.Value = 0;
            CDIWheelRotZnumericUpDown.Value = 0;

            //[Logic] section
            CDLGloConAnumericUpDown.Value = 0;
            CDLGloConBnumericUpDown.Value = 0;
            CDLPartConAnumericUpDown.Value = 0;
            CDLPartConBnumericUpDown.Value = 0;
            CDLPanConAnumericUpDown.Value = 0;
            CDLPanConBnumericUpDown.Value = 0;

            //[Parts] section
            CDPcomboBox.Items.Clear();          //Clear out the combo box list
            NewPartToStore = false;             //Reset the tracking flag
            PartsSelectedIndex = -1;            //Reset the tracking number
            CDPNametextBox.Text = "";
            CDPPosXnumericUpDown.Value = 0;
            CDPPosYnumericUpDown.Value = 0;
            CDPPosZnumericUpDown.Value = 0;
            CDPRotXnumericUpDown.Value = 0;
            CDPRotYnumericUpDown.Value = 0;
            CDPRotZnumericUpDown.Value = 0;
            CDPScalenumericUpDown.Value = 0;
            CDPProXnumericUpDown.Value = 0;
            CDPProYnumericUpDown.Value = 0;
            CDPProZnumericUpDown.Value = 0;

            //[AddOnLoad] section
            CDAcomboBox.Items.Clear();          //Clear out the combo box list
            NewAddOnLoadToStore = false;        //Reset the tracking flag
            AddOnLoadSelectedIndex = -1;        //Reset the tracking number
            CDANametextBox.Text = "";
            CDAPosXnumericUpDown.Value = 0;
            CDAPosYnumericUpDown.Value = 0;
            CDAPosZnumericUpDown.Value = 0;
            CDARotXnumericUpDown.Value = 0;
            CDARotYnumericUpDown.Value = 0;
            CDARotZnumericUpDown.Value = 0;
            CDAProXnumericUpDown.Value = 0;
            CDAProYnumericUpDown.Value = 0;
            CDAProZnumericUpDown.Value = 0;
        }

        //Fill out the GUI from the CarDataObject
        public void FillOutCarDataTabGUI()
        {
            //[Main] section
            CDMNameTextBox.Text = CarDataObject._MainName;
            CDMModelTextBox.Text = CarDataObject._MainModel;
            CDMRustMaskTextBox.Text = CarDataObject._MainRustMask;
            CDMRotXnumericUpDown.Value = (Decimal)CarDataObject._MainRotationX;     //Cast to decimal as it's what the NumUpDown uses
            CDMRotYnumericUpDown.Value = (Decimal)CarDataObject._MainRotationY;
            CDMRotZnumericUpDown.Value = (Decimal)CarDataObject._MainRotationZ;

            //[Other] section
            CDOPowernumericUpDown.Value = (Decimal)CarDataObject._OtherPower;
            CDOTranstextBox.Text = CarDataObject._OtherTransmissionType;
            CDOGearsnumericUpDown.Value = (Decimal)CarDataObject._OtherGears;
            CDOFinalDrivenumericUpDown.Value = (Decimal)CarDataObject._OtherFinalDriveRatio;
            CDOWeightnumericUpDown.Value = (Decimal)CarDataObject._OtherWeight;
            CDORPMFactornumericUpDown.Value = (Decimal)CarDataObject._OtherRpmFactor;
            CDORPMAnglenumericUpDown.Value = (Decimal)CarDataObject._OtherRpmAngle;
            CDOSpeedoFactornumericUpDown.Value = (Decimal)CarDataObject._OtherSpeedoFactor;
            CDOSpeedoAnglenumericUpDown.Value = (Decimal)CarDataObject._OtherSpeedoAngle;
            CDOSuspTRavelnumericUpDown.Value = (Decimal)CarDataObject._OtherSuspTravel;
            CDOLifterArmsRisenumericUpDown.Value = (Decimal)CarDataObject._OtherLifterArmsRise;
            CDOLifterArmsAnglenumericUpDown.Value = (Decimal)CarDataObject._OtherLifterArmsAngle;
            CDOCXNumericUpDown.Value = (Decimal)CarDataObject._OtherCX;
            CDORightHandDrivecheckBox.Checked = CarDataObject._OtherRightHandDrive;
            CDOTransVolnumericUpDown.Value = (Decimal)CarDataObject._OtherTransVol;
            CDODeleteOnLoadtextBox.Text = CarDataObject._OtherDeleteOnLoad;
            CDOTuneOnLoadtextBox.Text = CarDataObject._OtherTuneOnLoad;
            CDOColorMattecheckBox.Checked = CarDataObject._OtherColorMatte;

            //[Suspension] section
            CDSFrontAxleStartnumericUpDown.Value = (Decimal)CarDataObject._SuspensionFrontAxleStart;
            CDSWheelBasenumericUpDown.Value = (Decimal)CarDataObject._SuspensionWheelBase;
            CDSHeightnumericUpDown.Value = (Decimal)CarDataObject._SuspensionHeight;
            CDSHeightRearnumericUpDown.Value = (Decimal)CarDataObject._SuspensionHeightRear;
            CDSFrontTracknumericUpDown.Value = (Decimal)CarDataObject._SuspensionFrontTrack;
            CDSRearTracknumericUpDown.Value = (Decimal)CarDataObject._SuspensionRearTrack;
            CDSFrontSpringLengthnumericUpDown.Value = (Decimal)CarDataObject._SuspensionFrontSpringLength;
            CDSForceScalenumericUpDown.Value = (Decimal)CarDataObject._SuspensionForceScale;
            CDSForceScaleRearnumericUpDown.Value = (Decimal)CarDataObject._SuspensionForceScaleRear;
            CDSSidesFlipnumericUpDown.Value = (Decimal)CarDataObject._SuspensionsidesFlip;

            CDSFrontCenterSettextBox.Text = CarDataObject._SuspensionFrontCenterSet;
            CDSFrontLeftSettextBox.Text = CarDataObject._SuspensionFrontLeftSet;
            CDSFrontRightSettextBox.Text = CarDataObject._SuspensionFrontRightSet;
            CDSRearCenterSettextBox.Text = CarDataObject._SuspensionRearCenterSet;
            CDSrearLeftSettextBox.Text = CarDataObject._SuspensionRearLeftSet;
            CDSRearRightSettextBox.Text = CarDataObject._SuspensionRearRightSet;

            //[Engine] section
            CDEPosXnumericUpDown.Value = (Decimal)CarDataObject._EnginePositionX;
            CDEPosYnumericUpDown.Value = (Decimal)CarDataObject._EnginePositionY;
            CDEPosZnumericUpDown.Value = (Decimal)CarDataObject._EnginePositionZ;
            CDERotXnumericUpDown.Value = (Decimal)CarDataObject._EngineRotationX;
            CDERotYnumericUpDown.Value = (Decimal)CarDataObject._EngineRotationY;
            CDERotZnumericUpDown.Value = (Decimal)CarDataObject._EngineRotationZ;
            CDEScalenumericUpDown.Value = (Decimal)CarDataObject._EngineScale;
            CDETypecomboBox.Text = CarDataObject._EngineType;
            CDEPMnumericUpDown.Value = (Decimal)CarDataObject._EnginePM;
            //Engine swaps handled by the swap options dialog box

            //[Driveshaft] section
            CDDPosXnumericUpDown.Value = (Decimal)CarDataObject._DriveshaftPositionX;
            CDDPosYnumericUpDown.Value = (Decimal)CarDataObject._DriveshaftPositionY;
            CDDPosZnumericUpDown.Value = (Decimal)CarDataObject._DriveshaftPositionZ;
            CDDRotXnumericUpDown.Value = (Decimal)CarDataObject._DriveshaftRotationX;
            CDDRotYnumericUpDown.Value = (Decimal)CarDataObject._DriveshaftRotationY;
            CDDRotZnumericUpDown.Value = (Decimal)CarDataObject._DriveshaftRotationZ;
            CDDScalenumericUpDown.Value = (Decimal)CarDataObject._DriveshaftScale;
            CDDLengthnumericUpDown.Value = (Decimal)CarDataObject._DriveshaftLength;
            CDDSizenumericUpDown.Value = (Decimal)CarDataObject._DriveshaftSize;
            CDDTypetextBox.Text = CarDataObject._DriveshaftType;
            CDDPMnumericUpDown.Value = (Decimal)CarDataObject._DriveshaftPM;

            //[Wheels] section
            CDWWheelWidthnumericUpDown.Value = (Decimal)CarDataObject._WheelsWheelWidth;
            CDWRimSizenumericUpDown.Value = (Decimal)CarDataObject._WheelsRimSize;
            CDWTireSizenumericUpDown.Value = (Decimal)CarDataObject._WheelsTireSize;
            CDWTirecomboBox.SelectedText = CarDataObject._WheelsTire;
            CDWRimtextBox.Text = CarDataObject._WheelsRim;
            CDWRimCaptextBox.Text = CarDataObject._WheelsRimcap;
            CDWCambernumericUpDown.Value = (Decimal)CarDataObject._WheelsCamber;

            //Re-enable the Maserati stuff if needed
            if ((AvailableCarsDataComboBox.Text).Contains("Maserati"))
            {
                CDWWheelWidthRearnumericUpDown.Enabled = true;
                CDWTireSizeRearnumericUpDown.Enabled = true;
                CDWRimSizeRearnumericUpDown.Enabled = true;
                CDWWheelWidthRearnumericUpDown.Value = (Decimal)CarDataObject._WheelsWheelWidthRear;
                CDWTireSizeRearnumericUpDown.Value = (Decimal)CarDataObject._WheelsTireSizeRear;
                CDWRimSizeRearnumericUpDown.Value = (Decimal)CarDataObject._WheelsRimSizeRear;
            }

            //[Wheels_Rear] section
            CDWRWheelWidthnumericUpDown.Value = (Decimal)CarDataObject._WheelsRearWheelWidth;
            CDWRRimSizenumericUpDown.Value = (Decimal)CarDataObject._WheelsRearRimSize;
            CDWRTireSizenumericUpDown.Value = (Decimal)CarDataObject._WheelsRearTireSize;
            CDWRTirecomboBox.SelectedText = CarDataObject._WheelsRearTire;
            CDWRRimtextBox.Text = CarDataObject._WheelsRearRim;
            CDWRimCaptextBox.Text = CarDataObject._WheelsRearRimcap;
            CDWRCambernumericUpDown.Value = (Decimal)CarDataObject._WheelsRearCamber;

            //[Interior] section
            CDISLPosXnumericUpDown.Value = (Decimal)CarDataObject._InteriorSeatLeftPosX;
            CDISLPosYnumericUpDown.Value = (Decimal)CarDataObject._InteriorSeatLeftPosY;
            CDISLPosZnumericUpDown.Value = (Decimal)CarDataObject._InteriorSeatLeftPosZ;
            CDISLRotXnumericUpDown.Value = (Decimal)CarDataObject._InteriorSeatLeftRotX;
            CDISLRotYnumericUpDown.Value = (Decimal)CarDataObject._InteriorSeatLeftRotY;
            CDISLRotZnumericUpDown.Value = (Decimal)CarDataObject._InteriorSeatLeftRotZ;
            CDISeatScalenumericUpDown.Value = (Decimal)CarDataObject._InteriorSeatScale;
            CDISeattextBox.Text = CarDataObject._InteriorSeat;
            CDIWheeltextBox.Text = CarDataObject._InteriorWheel;
            CDISeatHeightModnumericUpDown.Value = (Decimal)CarDataObject._InteriorSeatHeightMod;
            CDIWheelPosXnumericUpDown.Value = (Decimal)CarDataObject._InteriorWheelPosX;
            CDIWheelPosYnumericUpDown.Value = (Decimal)CarDataObject._InteriorWheelPosY;
            CDIWheelPosZnumericUpDown.Value = (Decimal)CarDataObject._InteriorWheelPosZ;
            CDIWheelRotXnumericUpDown.Value = (Decimal)CarDataObject._InteriorWheelRotX;
            CDIWheelRotYnumericUpDown.Value = (Decimal)CarDataObject._InteriorWheelRotY;
            CDIWheelRotZnumericUpDown.Value = (Decimal)CarDataObject._InteriorWheelRotZ;
            CDIWheelScalenumericUpDown.Value = (Decimal)CarDataObject._InteriorWheelScale;

            //[Logic] section
            CDLGloConAnumericUpDown.Value = (Decimal)CarDataObject._LogicGlobalConditionA;
            CDLGloConBnumericUpDown.Value = (Decimal)CarDataObject._LogicGlobalConditionB;
            CDLPartConAnumericUpDown.Value = (Decimal)CarDataObject._LogicPartsConditionsA;
            CDLPartConBnumericUpDown.Value = (Decimal)CarDataObject._LogicPartsConditionsB;
            CDLPanConAnumericUpDown.Value = (Decimal)CarDataObject._LogicPanelsConditionsA;
            CDLPanConBnumericUpDown.Value = (Decimal)CarDataObject._LogicPanelsConditionsB;
            CDLUniqueModnumericUpDown.Value = (Decimal)CarDataObject._LogicUniqueMod;

            //[Parts] section
            //Populate parts panel combo
            int CDPListSize = 0;

            if (CarDataObject.ReturnPartsSize() > 0)                    //If theres is more than one part
            {
                while (CDPListSize < CarDataObject.ReturnPartsSize())   //While counter is less than the total
                {
                    CDPcomboBox.Items.Add(CDPListSize);                 //Add to the list
                    CDPListSize++;                                      //Increase the counter
                }

                CDPcomboBox.SelectedIndex = 0;  //Set the index
                CDFillOutCarDataPartsGUI(CDPcomboBox.SelectedIndex);    //Fill out this GUI section to the selected index
            }

            //[AddOnLoad] section
            //Populate parts panel combo
            int CDAListSize = 0;

            if (CarDataObject.ReturnAddOnLoadSize() > 0)                    //If theres is more than one part
            {
                while (CDAListSize < CarDataObject.ReturnAddOnLoadSize())   //While counter is less than the total
                {
                    CDAcomboBox.Items.Add(CDAListSize);                 //Add to the list
                    CDAListSize++;                                      //Increase the counter
                }

                CDAcomboBox.SelectedIndex = 0;  //Set the index
                CDFillOutCarDataAddOnLoadGUI(CDPcomboBox.SelectedIndex);    //Fill out this GUI section to the selected index
            }

        }

        //Fill out the [Parts] GUI section to the selected index
        public void CDFillOutCarDataPartsGUI(int index)
        {
            CDPNametextBox.Text = CarDataObject.GetPartsName(index);
            CDPPosXnumericUpDown.Value = (Decimal)CarDataObject.GetPartsPosX(index);
            CDPPosYnumericUpDown.Value = (Decimal)CarDataObject.GetPartsPosY(index);
            CDPPosZnumericUpDown.Value = (Decimal)CarDataObject.GetPartsPosZ(index);
            CDPRotXnumericUpDown.Value = (Decimal)CarDataObject.GetPartsRotX(index);
            CDPRotYnumericUpDown.Value = (Decimal)CarDataObject.GetPartsRotY(index);
            CDPRotZnumericUpDown.Value = (Decimal)CarDataObject.GetPartsRotZ(index);
            CDPScalenumericUpDown.Value = (Decimal)CarDataObject.GetPartScale(index);
            CDPProXnumericUpDown.Value = (Decimal)CarDataObject.GetPartProScaleX(index);
            CDPProYnumericUpDown.Value = (Decimal)CarDataObject.GetPartProScaleY(index);
            CDPProZnumericUpDown.Value = (Decimal)CarDataObject.GetPartProScaleZ(index);
        }

        //Update the [Parts] GUI for the selected index
        private void CDPcomboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            //Do we need to store a new item in the list?
            if (NewPartToStore == true)
            {
                //Store the new item into a new entry in the list
                CarDataObject.PartsAdder(CDPNametextBox.Text, (float)CDPPosXnumericUpDown.Value, (float)CDPPosYnumericUpDown.Value, (float)CDPPosZnumericUpDown.Value, (float)CDPRotXnumericUpDown.Value, (float)CDPRotYnumericUpDown.Value, (float)CDPRotZnumericUpDown.Value, (float)CDPScalenumericUpDown.Value, (float)CDPProXnumericUpDown.Value, (float)CDPProYnumericUpDown.Value, (float)CDPProZnumericUpDown.Value);
            }
            else if(PartsSelectedIndex != -1)   //If this is the first time here (we have loaded a new car data file) do not do anything
            {
                //Else update the selected one
                CarDataObject.PartsSetter(PartsSelectedIndex, CDPNametextBox.Text, (float)CDPPosXnumericUpDown.Value, (float)CDPPosYnumericUpDown.Value, (float)CDPPosZnumericUpDown.Value, (float)CDPRotXnumericUpDown.Value, (float)CDPRotYnumericUpDown.Value, (float)CDPRotZnumericUpDown.Value, (float)CDPScalenumericUpDown.Value, (float)CDPProXnumericUpDown.Value, (float)CDPProYnumericUpDown.Value, (float)CDPProZnumericUpDown.Value);
            }

            NewPartToStore = false;                                 //Reset the tracking flag
            CDFillOutCarDataPartsGUI(CDPcomboBox.SelectedIndex);    //Fill out this GUI section to the selected index
            PartsSelectedIndex = CDPcomboBox.SelectedIndex;         //Update the index
        }

        //Add a new [Parts] item
        private void CDPNewbutton_Click(object sender, EventArgs e)
        {
            CDPcomboBox.Items.Add(CarDataObject.ReturnPartsSize());     //Add a new entry to the parts combo box, will not add a new item to the list
            NewPartToStore = true;                                      //Toggle the flag to let us know there is new item to store
        }

        //Remove the current [Parts] item
        private void CDPDeletebutton_Click(object sender, EventArgs e)
        {
            CarDataObject.RemovePartsItem(CDPcomboBox.SelectedIndex);   //Call the functionto remove the selected part
        }

        //Fill out the [AddOnLoad] GUI section to the selected index
        public void CDFillOutCarDataAddOnLoadGUI(int index)
        {
            CDANametextBox.Text = CarDataObject.GetAddOnLoadName(index);
            CDAPosXnumericUpDown.Value = (Decimal)CarDataObject.GetAddOnLoadPosX(index);
            CDAPosYnumericUpDown.Value = (Decimal)CarDataObject.GetAddOnLoadPosY(index);
            CDAPosZnumericUpDown.Value = (Decimal)CarDataObject.GetAddOnLoadPosZ(index);
            CDARotXnumericUpDown.Value = (Decimal)CarDataObject.GetAddOnLoadRotX(index);
            CDARotYnumericUpDown.Value = (Decimal)CarDataObject.GetAddOnLoadRotY(index);
            CDARotZnumericUpDown.Value = (Decimal)CarDataObject.GetAddOnLoadRotZ(index);
            CDAProXnumericUpDown.Value = (Decimal)CarDataObject.GetAddOnLoadProScaleX(index);
            CDAProYnumericUpDown.Value = (Decimal)CarDataObject.GetAddOnLoadProScaleY(index);
            CDAProZnumericUpDown.Value = (Decimal)CarDataObject.GetAddOnLoadProScaleZ(index);
        }

        //Update the [AddOnLoad] GUI for the selected index
        private void CDAcomboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (NewAddOnLoadToStore == true)
            {
                //Store the new item into a new entry in the list
                CarDataObject.AddOnLoadAdder(CDANametextBox.Text, (float)CDAPosXnumericUpDown.Value, (float)CDAPosYnumericUpDown.Value, (float)CDAPosZnumericUpDown.Value, (float)CDARotXnumericUpDown.Value, (float)CDARotYnumericUpDown.Value, (float)CDARotZnumericUpDown.Value, (float)CDAProXnumericUpDown.Value, (float)CDAProYnumericUpDown.Value, (float)CDAProZnumericUpDown.Value);
            }
            else if (AddOnLoadSelectedIndex != -1)   //If this is the first time here (we have loaded a new car data file) do not do anything
            {
                //Else update the selected one
                CarDataObject.AddOnLoadSetter(AddOnLoadSelectedIndex, CDANametextBox.Text, (float)CDAPosXnumericUpDown.Value, (float)CDAPosYnumericUpDown.Value, (float)CDAPosZnumericUpDown.Value, (float)CDARotXnumericUpDown.Value, (float)CDARotYnumericUpDown.Value, (float)CDARotZnumericUpDown.Value, (float)CDAProXnumericUpDown.Value, (float)CDAProYnumericUpDown.Value, (float)CDAProZnumericUpDown.Value);
            }

            NewAddOnLoadToStore = false;                                //Reset the tracking flag
            CDFillOutCarDataAddOnLoadGUI(CDAcomboBox.SelectedIndex);    //Fill out this GUI section to the selected index
            AddOnLoadSelectedIndex = CDAcomboBox.SelectedIndex;         //Update the index
        }

        //Add a new [AddOnLoad] item
        private void CDANewbutton_Click(object sender, EventArgs e)
        {
            CDAcomboBox.Items.Add(CarDataObject.ReturnAddOnLoadSize());     //Add a new entry to the parts combo box, will not add a new item to the list
            NewAddOnLoadToStore = true;                                     //Toggle the flag to let us know there is new item to store
        }

        //Remove the current [AddOnLoad] item
        private void CDADeletebutton_Click(object sender, EventArgs e)
        {
            CarDataObject.RemoveAddOnLoadItem(CDAcomboBox.SelectedIndex);   //Call the functionto remove the selected part
        }

        //Restore all the default car data files to the active area
        public void CDCopyAllDefaultCarDataFiles(bool Backup)
        {
            //True = backup
            //False = restore

            //Create locals to make life easier
            string sourceDirName;
            string destDirName;
            DialogResult PromptResult;

            if (Backup)
            {
                //Prompt the user to see if they are sure
                PromptResult = MessageBox.Show("This will overwrite all the exisiting backed up default car data files\nwith the ones currently in the active folder\n\nAre you sure?", "Backup Car Data Files", MessageBoxButtons.YesNo);
                sourceDirName = ModMan.GetCarsDataDir();
                destDirName = ModMan.GetCarsDataDirBkUpDefault();
            }
            else
            {
                //Prompt the user to see if they are sure
                PromptResult = MessageBox.Show("This will overwrite all the car data files in the active folder\nwith the ones in the default backup folder\n\nAre you sure?", "Restore Car Data Files", MessageBoxButtons.YesNo);
                sourceDirName = ModMan.GetCarsDataDirBkUpDefault();
                destDirName = ModMan.GetCarsDataDir();
            }

            if (PromptResult == DialogResult.Yes)
            {
                // Get the subdirectories for the specified directory.
                DirectoryInfo sourceDir = new DirectoryInfo(sourceDirName);

                //Check if source dir exists
                if (!sourceDir.Exists)
                {
                    throw new DirectoryNotFoundException("Source directory does not exist or could not be found: " + sourceDirName);
                }

                // If the destination directory doesn't exist, create it.
                if (!Directory.Exists(destDirName))
                {
                    Directory.CreateDirectory(destDirName);
                }

                // Get the files in the directory and copy them to the new location.
                FileInfo[] files = sourceDir.GetFiles();
                foreach (FileInfo file in files)
                {
                    //Check if the file is NOT on the ignore list
                    if ((file.Name != "cars.txt") && (file.Name != "engines.txt") && (file.Name != "tires.txt"))
                    {
                        file.CopyTo((Path.Combine(destDirName, file.Name)), true);
                    }
                }
                //Repopulate the combo box
                PopulateAvailableCarsComboBox();
            }
        }

        //Restore all the Mod car data files to the active area
        public void CDCopyAllModifiedCarDataFiles(bool Backup)
        {
            //True = backup
            //False = restore

            //Create locals to make life easier
            string sourceDirName;
            string destDirName;
            DialogResult PromptResult;

            if (Backup)
            {
                //Prompt the user to see if they are sure
                PromptResult = MessageBox.Show("This will overwrite all the exisiting backed up modified car data files\nwith the ones currently in the active folder\n\nAre you sure?", "Backup Mod Car Data Files", MessageBoxButtons.YesNo);
                sourceDirName = ModMan.GetCarsDataDir();
                destDirName = ModMan.GetCarsDataDirBkUpMod();
            }
            else
            {
                //Prompt the user to see if they are sure
                PromptResult = MessageBox.Show("This will overwrite all the car data files in the active folder\nwith the ones in the modified backup folder\n\nAre you sure?", "Restore Car Data Files", MessageBoxButtons.YesNo);
                sourceDirName = ModMan.GetCarsDataDirBkUpMod();
                destDirName = ModMan.GetCarsDataDir();
            }

            if (PromptResult == DialogResult.Yes)
            {
                // Get the subdirectories for the specified directory.
                DirectoryInfo sourceDir = new DirectoryInfo(sourceDirName);

                //Check if source dir exists
                if (!sourceDir.Exists)
                {
                    throw new DirectoryNotFoundException("Source directory does not exist or could not be found: " + sourceDirName);
                }

                // If the destination directory doesn't exist, create it.
                if (!Directory.Exists(destDirName))
                {
                    Directory.CreateDirectory(destDirName);
                }

                // Get the files in the directory and copy them to the new location.
                FileInfo[] files = sourceDir.GetFiles();
                foreach (FileInfo file in files)
                {
                    //Check if the file is NOT on the ignore list
                    if ((file.Name != "cars.txt") && (file.Name != "engines.txt") && (file.Name != "tires.txt"))
                    {
                        file.CopyTo((Path.Combine(destDirName, file.Name)), true);
                    }
                }
                //Repopulate the combo box
                PopulateAvailableCarsComboBox();
            }
        }

        //Dialog box with a text input field
        public static DialogResult InputBox(string title, string promptText, ref string value)
        {
            //Taken from
            //http://www.csharp-examples.net/inputbox/

            Form form = new Form();
            Label label = new Label();
            TextBox textBox = new TextBox();
            Button buttonOk = new Button();
            Button buttonCancel = new Button();

            form.Text = title;
            label.Text = promptText;
            textBox.Text = value;

            buttonOk.Text = "OK";
            buttonCancel.Text = "Cancel";
            buttonOk.DialogResult = DialogResult.OK;
            buttonCancel.DialogResult = DialogResult.Cancel;

            label.SetBounds(9, 20, 372, 13);
            textBox.SetBounds(12, 36, 372, 20);
            buttonOk.SetBounds(228, 72, 75, 23);
            buttonCancel.SetBounds(309, 72, 75, 23);

            label.AutoSize = true;
            textBox.Anchor = textBox.Anchor | AnchorStyles.Right;
            buttonOk.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            buttonCancel.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;

            form.ClientSize = new Size(396, 107);
            form.Controls.AddRange(new Control[] { label, textBox, buttonOk, buttonCancel });
            form.ClientSize = new Size(Math.Max(300, label.Right + 10), form.ClientSize.Height);
            form.FormBorderStyle = FormBorderStyle.FixedDialog;
            form.StartPosition = FormStartPosition.CenterScreen;
            form.MinimizeBox = false;
            form.MaximizeBox = false;
            form.AcceptButton = buttonOk;
            form.CancelButton = buttonCancel;

            DialogResult dialogResult = form.ShowDialog();
            value = textBox.Text;
            return dialogResult;
        }

        //Dialog box with a large text input field for pasting in a car data file as text
        public static DialogResult CarDataTextInputBox(ref string OutputValue)
        {
            //Setup object
            Form form = new Form();
            Label label = new Label();
            TextBox textBox = new TextBox();
            Button buttonOk = new Button();
            Button buttonCancel = new Button();

            form.Text = "Car Data File as text";
            label.Text = "Please paste and copy the Car Data text into the box below";
            textBox.Text = OutputValue;

            buttonOk.Text = "OK";
            buttonCancel.Text = "Cancel";
            buttonOk.DialogResult = DialogResult.OK;
            buttonCancel.DialogResult = DialogResult.Cancel;

            label.SetBounds(9, 10, 372, 13);
            textBox.Multiline = true;
            textBox.ScrollBars = ScrollBars.Vertical;
            textBox.SetBounds(12, 36, 672, 820);
            buttonOk.SetBounds(105, 872, 75, 23);
            buttonCancel.SetBounds(186, 872, 75, 23);

            label.AutoSize = true;
            textBox.Anchor = textBox.Anchor | AnchorStyles.Right;
            buttonOk.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            buttonCancel.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;

            form.ClientSize = new Size(696, 907);
            form.Controls.AddRange(new Control[] { label, textBox, buttonOk, buttonCancel });
            form.ClientSize = new Size(Math.Max(600, label.Right + 10), form.ClientSize.Height);
            form.FormBorderStyle = FormBorderStyle.FixedDialog;
            form.StartPosition = FormStartPosition.CenterScreen;
            form.MinimizeBox = false;
            form.MaximizeBox = false;
            form.AcceptButton = buttonOk;
            form.CancelButton = buttonCancel;

            DialogResult dialogResult = form.ShowDialog();
            
            //Get text
            OutputValue = textBox.Text;
            return dialogResult;
        }
       
        //Handles a call to reset the Car Data tab GUI elements (reset by reloading from the existing file)
        private void CDResetbutton_Click(object sender, EventArgs e)
        {
            //Prompt the user to see if they are sure
            DialogResult PromptResult = MessageBox.Show("This will reset all values to those from the existing car data file\n\nAre you sure?", "Reset Car Data File", MessageBoxButtons.YesNo);

            if (PromptResult == DialogResult.Yes)
            {
                //Save the current car selected (remember the car we're on, not saving the car data file)
                string CurrentCar = AvailableCarsDataComboBox.Text;
                //Clean up the GUI
                ClearOutCarDataTabGUI();
                //Restore the selected car
                AvailableCarsDataComboBox.Text = CurrentCar;
                //Reload the data
                AvailableCarsDataComboBox_SelectedIndexChanged(sender, e);
            }
        }

        //Handles a call to restore a single Car Data File to default (restores by copying over the 'default' file from the default backup then doing a reset to sort the GUI)
        private void CDRestorebutton_Click(object sender, EventArgs e)
        {
            //Prompt the user to see if they are sure
            DialogResult PromptResult = MessageBox.Show("This will overwrite this car data files with the one in the default backup folder\nAre you sure?", "Save Car Data File", MessageBoxButtons.YesNo);

            if (PromptResult == DialogResult.Yes)
            {
                //Save the current car selected (remember the car we're on, not saving the car data file)
                string CurrentCar = AvailableCarsDataComboBox.Text;
                //Setup the full path/filenames
                string SourceFile = ModMan.GetCarsDataDirBkUpDefault() + "\\" + CurrentCar;
                string DestFile = ModMan.GetCarsDataDir() + "\\" + CurrentCar;

                //Restore the car data file from the 'default' backup one
                System.IO.File.Copy(SourceFile, DestFile, true);

                //Reset the GUI to use the restored data                
                //Clean up the GUI
                ClearOutCarDataTabGUI();
                //Restore the selected car
                AvailableCarsDataComboBox.Text = CurrentCar;
                //Reload the data
                AvailableCarsDataComboBox_SelectedIndexChanged(sender, e);
            }
        }

        //Handles a call to restore all the Car Data Files to default(restores by copying over the 'default' file from the default backup then doing a reset to sort the GUI)
        private void CDRestoreAllbutton_Click(object sender, EventArgs e)
        {
            //Call the restore all function for the default car data files
            CDCopyAllDefaultCarDataFiles(false);
        }

        //Handles a call to restore all the Car Data Files to a Mod(restores by copying over the 'default' file from the default backup then doing a reset to sort the GUI)
        private void CDRestoreAllModbutton_Click(object sender, EventArgs e)
        {
            //Call the restore all function for the modified car data files
            CDCopyAllModifiedCarDataFiles(false);
        }

        //Handles a call to restore a single Car Data File to a Mod(restores by copying over the 'default' file from the default backup then doing a reset to sort the GUI)
        private void CDRestoreModbutton_Click(object sender, EventArgs e)
        {
            //Prompt the user to see if they are sure
            DialogResult PromptResult = MessageBox.Show("This will overwrite this car data files with the one in the mod backup folder\nAre you sure?", "Save Car Data File", MessageBoxButtons.YesNo);

            if (PromptResult == DialogResult.Yes)
            {
                //Save the current car selected (remember the car we're on, not saving the car data file)
                string CurrentCar = AvailableCarsDataComboBox.Text;
                //Setup the full path/filenames
                string SourceFile = ModMan.GetCarsDataDirBkUpMod() + "\\" + CurrentCar;
                string DestFile = ModMan.GetCarsDataDir() + "\\" + CurrentCar;

                //Restore the car data file from the 'default' backup one
                System.IO.File.Copy(SourceFile, DestFile, true);

                //Reset the GUI to use the restored data                
                //Clean up the GUI
                ClearOutCarDataTabGUI();
                //Restore the selected car
                AvailableCarsDataComboBox.Text = CurrentCar;
                //Reload the data
                AvailableCarsDataComboBox_SelectedIndexChanged(sender, e);
            }
        }

        //Handles a call to backup all mods
        private void CDBackupAllModsbutton_Click(object sender, EventArgs e)
        {
            //Call the restore all function for the modified car data files
            CDCopyAllModifiedCarDataFiles(true);
        }

        //Handles a call to clear out all data and start again
        private void CDNewbutton_Click(object sender, EventArgs e)
        {
            //Reset the GUI elements
            ClearOutCarDataTabGUI();
            //Empty out all existing data
            CarDataObject.CarDataClearAll();
        }

        //Handles a call to save the updated data as a new file
        private void CDSaveAsNewbutton_Click(object sender, EventArgs e)
        {
            //First prompt for a file name
            //Make sure it has .txt on the end
            //AvailableCarsDataComboBox.Text

            //Local to hold new filename
            string FileName = "";

            //Use a dialog box to get an in game name
            if (InputBox("Enter new car data filename", "Enter filename here:", ref FileName) == DialogResult.OK)
            {
                //Check for and add ".txt" to end if needed
                if(!(FileName.EndsWith(".txt")))
                {
                    FileName = FileName + ".txt";
                }

                //Set the Current selected item to the new filename as that is what we use to get the filename
                AvailableCarsDataComboBox.Text = FileName;

                //Then commit the data changes and save
                CDSavebutton_Click(sender, e);  //Reuse the function that does both

                //Reload files list, both tabs

                //Car List Tab
                //load the list of cars file
                ModMan.ReadCarsCurrent();
                //Populate the cars currently in list, list box
                PopulateCarsCurrentList();

                //Find car data files
                ModMan.FindCarsFiles();
                //Populate the cars files available, list box
                PopulateCarsAvailableList();

                //Car Data Tab
                //List of available car text file already loaded
                //Populate the available cars drop down list
                PopulateAvailableCarsComboBox();

                //Need to update selected index, so save works if it's pressed
                AvailableCarsDataComboBox.SelectedIndex = AvailableCarsDataComboBox.Items.Count-1;
            }
        }

        //Handles a call to save the updated data
        private void CDSavebutton_Click(object sender, EventArgs e)
        {
            //Need export the data from the gui to the LoadedCarData
            //Get the index of the selected engine
            int Index = AvailableCarsDataComboBox.SelectedIndex;
            //Check if a line has been selected
            if (Index > -1)
            {
                //Prompt the user to see if they are sure
                DialogResult PromptResult = MessageBox.Show("This will overwrite the existing car data file?","Save Car Data File",MessageBoxButtons.YesNo);
                
                if (PromptResult == DialogResult.Yes)
                {
                    //[Main] section
                    CarDataObject._MainName = CDMNameTextBox.Text;
                    CarDataObject._MainModel = CDMModelTextBox.Text;
                    CarDataObject._MainRustMask = CDMRustMaskTextBox.Text;
                    CarDataObject._MainRotationX = (float)CDMRotXnumericUpDown.Value;
                    CarDataObject._MainRotationY = (float)CDMRotYnumericUpDown.Value;
                    CarDataObject._MainRotationZ = (float)CDMRotZnumericUpDown.Value;

                    //[Other] section
                    CarDataObject._OtherPower = (int)CDOPowernumericUpDown.Value;
                    CarDataObject._OtherTransmissionType = CDOTranstextBox.Text;
                    CarDataObject._OtherGears = (int)CDOGearsnumericUpDown.Value;
                    CarDataObject._OtherFinalDriveRatio = (float)CDOFinalDrivenumericUpDown.Value;
                    CarDataObject._OtherWeight = (int)CDOWeightnumericUpDown.Value;
                    CarDataObject._OtherRpmFactor = (float)CDORPMFactornumericUpDown.Value;
                    CarDataObject._OtherRpmAngle = (float)CDORPMAnglenumericUpDown.Value;
                    CarDataObject._OtherSpeedoFactor = (float)CDOSpeedoFactornumericUpDown.Value;
                    CarDataObject._OtherSpeedoAngle = (float)CDOSpeedoAnglenumericUpDown.Value;
                    CarDataObject._OtherSuspTravel = (float)CDOSuspTRavelnumericUpDown.Value;
                    CarDataObject._OtherLifterArmsRise = (float)CDOLifterArmsRisenumericUpDown.Value;
                    CarDataObject._OtherLifterArmsAngle = (float)CDOLifterArmsAnglenumericUpDown.Value;
                    CarDataObject._OtherCX = (float)CDOCXNumericUpDown.Value;
                    CarDataObject._OtherRightHandDrive = CDORightHandDrivecheckBox.Checked;
                    CarDataObject._OtherTransVol = (float)CDOTransVolnumericUpDown.Value;
                    CarDataObject._OtherDeleteOnLoad = CDODeleteOnLoadtextBox.Text;
                    CarDataObject._OtherTuneOnLoad = CDOTuneOnLoadtextBox.Text;
                    CarDataObject._OtherColorMatte = CDOColorMattecheckBox.Checked;

                    //[Suspension] section
                    CarDataObject._SuspensionFrontAxleStart = (float)CDSFrontAxleStartnumericUpDown.Value;
                    CarDataObject._SuspensionWheelBase = (float)CDSWheelBasenumericUpDown.Value;
                    CarDataObject._SuspensionHeight = (float)CDSHeightnumericUpDown.Value;
                    CarDataObject._SuspensionHeightRear = (float)CDSHeightRearnumericUpDown.Value;
                    CarDataObject._SuspensionFrontTrack = (float)CDSFrontTracknumericUpDown.Value;
                    CarDataObject._SuspensionRearTrack = (float)CDSRearTracknumericUpDown.Value;
                    CarDataObject._SuspensionFrontSpringLength = (float)CDSFrontSpringLengthnumericUpDown.Value;
                    CarDataObject._SuspensionForceScale = (int)CDSForceScalenumericUpDown.Value;
                    CarDataObject._SuspensionForceScaleRear = (int)CDSForceScaleRearnumericUpDown.Value;
                    CarDataObject._SuspensionsidesFlip = (int)CDSSidesFlipnumericUpDown.Value;

                    CarDataObject._SuspensionFrontCenterSet = CDSFrontCenterSettextBox.Text;
                    CarDataObject._SuspensionFrontLeftSet = CDSFrontLeftSettextBox.Text;
                    CarDataObject._SuspensionFrontRightSet = CDSFrontRightSettextBox.Text;
                    CarDataObject._SuspensionRearCenterSet = CDSRearCenterSettextBox.Text;
                    CarDataObject._SuspensionRearLeftSet = CDSrearLeftSettextBox.Text;
                    CarDataObject._SuspensionRearRightSet = CDSRearRightSettextBox.Text;

                    //[Engine] section
                    CarDataObject._EnginePositionX = (float)CDEPosXnumericUpDown.Value;
                    CarDataObject._EnginePositionY = (float)CDEPosYnumericUpDown.Value;
                    CarDataObject._EnginePositionZ = (float)CDEPosZnumericUpDown.Value;
                    CarDataObject._EngineRotationX = (float)CDERotXnumericUpDown.Value;
                    CarDataObject._EngineRotationY = (float)CDERotYnumericUpDown.Value;
                    CarDataObject._EngineRotationZ = (float)CDERotZnumericUpDown.Value;
                    CarDataObject._EngineScale = (float)CDEScalenumericUpDown.Value;
                    CarDataObject._EngineType = CDETypecomboBox.Text;
                    CarDataObject._EnginePM = (float)CDEPMnumericUpDown.Value;
                    //Engine swaps handled by it's own dialog box

                    //[Driveshaft] section
                    CarDataObject._DriveshaftPositionX = (float)CDDPosXnumericUpDown.Value;
                    CarDataObject._DriveshaftPositionY = (float)CDDPosYnumericUpDown.Value;
                    CarDataObject._DriveshaftPositionZ = (float)CDDPosZnumericUpDown.Value;
                    CarDataObject._DriveshaftRotationX = (float)CDDRotXnumericUpDown.Value;
                    CarDataObject._DriveshaftRotationY = (float)CDDRotYnumericUpDown.Value;
                    CarDataObject._DriveshaftRotationZ = (float)CDDRotZnumericUpDown.Value;
                    CarDataObject._DriveshaftScale = (float)CDDScalenumericUpDown.Value;
                    CarDataObject._DriveshaftLength = (float)CDDLengthnumericUpDown.Value;
                    CarDataObject._DriveshaftSize = (float)CDDSizenumericUpDown.Value;
                    CarDataObject._DriveshaftType = CDDTypetextBox.Text;
                    CarDataObject._DriveshaftPM = (float)CDDPMnumericUpDown.Value;

                    //[Wheel] section
                    CarDataObject._WheelsWheelWidth = (int)CDWWheelWidthnumericUpDown.Value;
                    CarDataObject._WheelsRimSize = (int)CDWRimSizenumericUpDown.Value;
                    CarDataObject._WheelsTireSize = (int)CDWTireSizenumericUpDown.Value;
                    CarDataObject._WheelsTire = CDWTirecomboBox.Text;
                    CarDataObject._WheelsRim = CDWRimtextBox.Text;
                    CarDataObject._WheelsRimcap = CDWRimCaptextBox.Text;
                    CarDataObject._WheelsCamber = (int)CDWCambernumericUpDown.Value;
                    CarDataObject._WheelsWheelWidthRear = (int)CDWWheelWidthRearnumericUpDown.Value;
                    CarDataObject._WheelsTireSizeRear = (int)CDWTireSizeRearnumericUpDown.Value;
                    CarDataObject._WheelsRimSizeRear = (int)CDWRimSizeRearnumericUpDown.Value;

                    //[Wheel_Rear] section
                    CarDataObject._WheelsRearWheelWidth = (int)CDWRWheelWidthnumericUpDown.Value;
                    CarDataObject._WheelsRearRimSize = (int)CDWRRimSizenumericUpDown.Value;
                    CarDataObject._WheelsRearTireSize = (int)CDWRTireSizenumericUpDown.Value;
                    CarDataObject._WheelsRearTire = CDWRTirecomboBox.Text;
                    CarDataObject._WheelsRearRim = CDWRRimtextBox.Text;
                    CarDataObject._WheelsRearRimcap = CDWRRimCaptextBox.Text;
                    CarDataObject._WheelsRearCamber = (int)CDWRCambernumericUpDown.Value;

                    //[Interior] section
                    CarDataObject._InteriorSeatLeftPosX = (float)CDISLPosXnumericUpDown.Value;
                    CarDataObject._InteriorSeatLeftPosY = (float)CDISLPosYnumericUpDown.Value;
                    CarDataObject._InteriorSeatLeftPosZ = (float)CDISLPosZnumericUpDown.Value;
                    CarDataObject._InteriorSeatLeftRotX = (float)CDISLRotXnumericUpDown.Value;
                    CarDataObject._InteriorSeatLeftRotY = (float)CDISLRotYnumericUpDown.Value;
                    CarDataObject._InteriorSeatLeftRotZ = (float)CDISLRotZnumericUpDown.Value;
                    CarDataObject._InteriorSeatScale = (float)CDISeatScalenumericUpDown.Value;
                    CarDataObject._InteriorSeatScale = (float)CDISeatScalenumericUpDown.Value;
                    CarDataObject._InteriorSeat = CDISeattextBox.Text;
                    CarDataObject._InteriorWheel = CDIWheeltextBox.Text;
                    CarDataObject._InteriorSeatHeightMod = (float)CDISeatHeightModnumericUpDown.Value;
                    CarDataObject._InteriorWheelPosX = (float)CDIWheelPosXnumericUpDown.Value;
                    CarDataObject._InteriorWheelPosY = (float)CDIWheelPosYnumericUpDown.Value;
                    CarDataObject._InteriorWheelPosZ = (float)CDIWheelPosZnumericUpDown.Value;
                    CarDataObject._InteriorWheelRotX = (float)CDIWheelRotXnumericUpDown.Value;
                    CarDataObject._InteriorWheelRotY = (float)CDIWheelRotYnumericUpDown.Value;
                    CarDataObject._InteriorWheelRotZ = (float)CDIWheelRotZnumericUpDown.Value;
                    CarDataObject._InteriorWheelScale = (float)CDIWheelScalenumericUpDown.Value;

                    //[Logic] section
                    CarDataObject._LogicGlobalConditionA = (float)CDLGloConAnumericUpDown.Value;
                    CarDataObject._LogicGlobalConditionB = (float)CDLGloConBnumericUpDown.Value;
                    CarDataObject._LogicPartsConditionsA = (float)CDLPartConAnumericUpDown.Value;
                    CarDataObject._LogicPartsConditionsB = (float)CDLPartConBnumericUpDown.Value;
                    CarDataObject._LogicPanelsConditionsA = (float)CDLPanConAnumericUpDown.Value;
                    CarDataObject._LogicPanelsConditionsB = (float)CDLPanConBnumericUpDown.Value;

                    //[Parts]
                    //This ends up being handled by the logic that handles the CDPcomboBox logic

                    //Finally commit to file, using the name from the combo box
                    CarDataObject.WriteCarDataToFile(ModMan.GetCarsDataDir() + "\\" + AvailableCarsDataComboBox.Text);
                }
            }
        }

        //Handles a call to select the car data image
        private void CDSetPicturebutton_Click(object sender, EventArgs e)
        {
            //Assume the image is named correctly

            //Open up a file browser
            OpenFileDialog ofd = new OpenFileDialog();
            // Show the dialog and get result.
            DialogResult result = ofd.ShowDialog();
            if (result == DialogResult.OK) // Test result.
            {
                //Copy the file
                ModMan.FileCopy(ofd.FileName);
            }
        }

        //Load a Car Data file from a text file
        private void CDLoadFromFilebutton_Click(object sender, EventArgs e)
        {
            //Prompt the user to see if they are sure
            DialogResult PromptResult = MessageBox.Show("This will overwrite the shown values with those in the selected file (you can still view it before saving)\nAre you sure?", "Load Car Data File", MessageBoxButtons.YesNo);

            if (PromptResult == DialogResult.Yes)
            {
                //Open up a file browser
                OpenFileDialog ofd = new OpenFileDialog();
                // Show the dialog and get result.
                DialogResult result = ofd.ShowDialog();
                if (result == DialogResult.OK) // Test result.
                {
                    //Load the selected data file
                    CarDataObject.LoadCarDataFile(ofd.FileName);

                    //Update the GUI to display it
                    //Clear down all the GUI to remove old values
                    ClearOutCarDataTabGUI();

                    //Fill out the GUI from the CarDataObject
                    FillOutCarDataTabGUI();

                    //Assemble the picture path/filename
                    string PicturePath = ModMan.GetCarsDataDir() + "\\" + CarDataObject._MainModel + ".jpg";

                    //Check if there is a thumbnail picture present to use
                    if (File.Exists(PicturePath))
                    {
                        Image image = Image.FromFile(PicturePath);  //Use the picture if it exists
                        CarDataThumbNailPictureBox.Image = image;
                    }
                    else
                    {
                        PicturePath = ModMan.GetCarsDataDir() + "\\car_placeholder.jpg";
                        Image image = Image.FromFile(PicturePath);  //Use the place holder image
                        CarDataThumbNailPictureBox.Image = image;
                    }

                }
            }
        }

        //Load a Car Data file from a text box
        private void CDLoadFromTextbutton_Click(object sender, EventArgs e)
        {
            //Prompt the user to see if they are sure
            DialogResult PromptResult = MessageBox.Show("This will overwrite the shown values with those in the supplied text (you can still view it before saving)\nAre you sure?", "Load Car Data Text", MessageBoxButtons.YesNo);

            if (PromptResult == DialogResult.Yes)
            {
                //Local to Hold the return
                string CarDataText = "";
                //Call the dialog to get the result
                if (CarDataTextInputBox(ref CarDataText) == DialogResult.OK)
                {
                    //If we returned with an OK
                    //Save text to a temp file
                    //Create a local file writer
                    StreamWriter writer = new StreamWriter("TempCarDataFile.txt");
                    //writer.WriteLine("[main]");
                    writer.WriteLine(CarDataText);
                    //we are finished with the writer so close and bin it
                    writer.Close();
                    writer.Dispose();

                    //Load the temp file
                    CarDataObject.LoadCarDataFile("TempCarDataFile.txt");
                    //Delete the temp file
                    File.Delete("TempCarDataFile.txt");

                    //Update the GUI to display it
                    //Clear down all the GUI to remove old values
                    ClearOutCarDataTabGUI();

                    //Fill out the GUI from the CarDataObject
                    FillOutCarDataTabGUI();

                    //Assemble the picture path/filename
                    string PicturePath = ModMan.GetCarsDataDir() + "\\" + CarDataObject._MainModel + ".jpg";

                    //Check if there is a thumbnail picture present to use
                    if (File.Exists(PicturePath))
                    {
                        Image image = Image.FromFile(PicturePath);  //Use the picture if it exists
                        CarDataThumbNailPictureBox.Image = image;
                    }
                    else
                    {
                        PicturePath = ModMan.GetCarsDataDir() + "\\car_placeholder.jpg";
                        Image image = Image.FromFile(PicturePath);  //Use the place holder image
                        CarDataThumbNailPictureBox.Image = image;
                    }
                }
            }
        }

        //Give the user the car data text to copy
        private void CDGetTextbutton_Click(object sender, EventArgs e)
        {
            //Local to hold the Car Data Text
            string CarDataText = "";
            //Local to the hold the path/filename
            string Filename = ModMan.GetCarsDataDir() + "\\" + AvailableCarsDataComboBox.Text;

            //Fill out the string
            //Check if the Car Data File exists
            if (File.Exists(Filename))
            {
                //Get the Car Data Text
                CarDataText = System.IO.File.ReadAllText(Filename);

                //Find and replace to make it work with a multiline GUI textbox
                CarDataText = CarDataText.Replace("\n", Environment.NewLine);

                //Show the user the string
                CarDataTextInputBox(ref CarDataText);
            }
        }

        //Handles a call to the engine swap options button
        private void CDEEngineSwapOptionsbutton_Click(object sender, EventArgs e)
        {
            Form form = new Form();
            Label label = new Label();
            CheckedListBox clbox = new CheckedListBox();
            Button buttonOk = new Button();

            form.Text = "Engines";
            label.Text = "Available engines";
            buttonOk.Text = "OK";
            buttonOk.DialogResult = DialogResult.OK;

            label.SetBounds(3, 3, 300, 13);
            clbox.SetBounds(3,20,392,400);
            clbox.CheckOnClick = true;
            buttonOk.SetBounds(3, 419, 75, 23);

            form.ClientSize = new Size(400, 446);
            form.Controls.AddRange(new Control[] { label, clbox, buttonOk });
            form.FormBorderStyle = FormBorderStyle.FixedDialog;
            form.StartPosition = FormStartPosition.CenterScreen;
            form.MinimizeBox = false;
            form.MaximizeBox = false;
            form.AcceptButton = buttonOk;

            //Add engines list
            int index = 0;  //local loop counter
            //Loop through all of the loaded engines
            while (index < EngineDataList.Count)
            {
                string EngineName = EngineDataList[index]._Name;    //Get name
                EngineName = EngineName.Substring(1, EngineName.Length-2);       //Trim the [] off
                //Add an item to the engines checkbox list
                clbox.Items.Add(EngineName);
                if ((CarDataObject.SwapOptionsContains(EngineName))  || (CarDataObject._EngineType == EngineName))  //If existing engine is in the list
                {
                    clbox.SetItemChecked(index, true);              //Check the engines check box
                }
                index++;    //inc counter
            }

            DialogResult dialogResult = form.ShowDialog();

            //Will only get here after the form above is closed
            index = 0;          //reset counter
            if (clbox.CheckedItems.Count > 1)       //If there is only a single item in the list, skip saving the swap options
            {
                //Empty existing list, to avoid duplicates
                CarDataObject.RemoveAllSwapOptions();
                while (index < clbox.CheckedItems.Count)
                {
                    //Add an engine to the swap options list
                    CarDataObject.SwapOptionsSetter(clbox.CheckedItems[index].ToString());
                    index++;    //inc counter
                }
            }
        }
        #endregion

        #region Save File - General
        //Populate the profile list combo box
        private void PopulateProfileComboBox()
        {
            //Get directories
            DirectoryInfo di = new DirectoryInfo(ModMan.GetSavedGamesDir());
            //Counter for profiles
            int ProCount = 0;

            foreach (System.IO.DirectoryInfo Folder in di.GetDirectories())
            {
                //Add folder to the combo box lists
                SGETProfilecomboBox.Items.Add(Folder);
                //Increment the profiles counter
                ProCount++;
            }

            //Set the found profiles counter
            SGTProfilesFoundlabel.Text = ProCount + " Profiles Found";
        }
        #endregion

        #region Save File - Global file
        //Resets the Save Data Global tab GUI elements
        private void ClearOutSaveDataTabsGlobal()
        {
            //Fill out the GUI

            //Global
            SGETGPartsRepairednumericUpDown.Value = 0;
            SGETGMoneyIncomePartsnumericUpDown.Value = 0;
            SGETGMoneyIncomeCarsnumericUpDown.Value = 0;
            SGETGCarsSoldnumericUpDown.Value = 0;
            SGETGJobsCompletednumericUpDown.Value = 0;
            SGETGCarsOwnednumericUpDown.Value = 0;
            SGETGMoneyIncomenumericUpDown.Value = 0;
            SGETGPartsUnmountednumericUpDown.Value = 0;
            SGETGBoltsUndonenumericUpDown.Value = 0;
            SGETBankLoannumericUpDown.Value = 0;
            SGETGXPnumericUpDown.Value = 0;
            SGETGMoneynumericUpDown.Value = 0;
        }

        //Load the global save file
        private void SGEGobalFileLoad()
        {
            //Check the combo box text isn't blank
            if (SGETProfilecomboBox.Text != "")
            {
                SaveGameDataGlobal LocalGrab = new SaveGameDataGlobal();        //Create a local to get save data
                //Check if the file exists
                if (LocalGrab.LoadGlobalSaveFile(ModMan.GetSavedGamesDir() + "\\" + SGETProfilecomboBox.Text))     //Load the save file
                {
                    //Fill out the GUI
                    SGETGPartsRepairednumericUpDown.Value = LocalGrab._Stats_PartsRepaired;
                    SGETGMoneyIncomePartsnumericUpDown.Value = LocalGrab._Stats_MoneyIncomeParts;
                    SGETGMoneyIncomeCarsnumericUpDown.Value = LocalGrab._Stats_MoneyIncomeCars;
                    SGETGCarsSoldnumericUpDown.Value = LocalGrab._Stats_CarsSold;
                    SGETGJobsCompletednumericUpDown.Value = LocalGrab._Stats_JobsCompletted;
                    SGETGCarsOwnednumericUpDown.Value = LocalGrab._Stats_CarsOwned;
                    SGETGMoneyIncomenumericUpDown.Value = LocalGrab._Stats_MoneyIncome;
                    SGETGPartsUnmountednumericUpDown.Value = LocalGrab._Stats_PartsUnmounted;
                    SGETGBoltsUndonenumericUpDown.Value = LocalGrab._Stats_Bolts;
                    SGETBankLoannumericUpDown.Value = LocalGrab._bankLoan;
                    SGETGXPnumericUpDown.Value = LocalGrab._xp;
                    SGETGMoneynumericUpDown.Value = LocalGrab._money;
                }
            }
        }

        //Save the global save file
        private void SGETGSavebutton_Click(object sender, EventArgs e)
        {
            //Create a local to hold save data
            SaveGameDataGlobal LocalSave = new SaveGameDataGlobal();

            //Fill out the data object
            LocalSave._Stats_PartsRepaired = (int)SGETGPartsRepairednumericUpDown.Value;
            LocalSave._Stats_MoneyIncomeParts = (int)SGETGMoneyIncomePartsnumericUpDown.Value;
            LocalSave._Stats_MoneyIncomeCars = (int)SGETGMoneyIncomeCarsnumericUpDown.Value;
            LocalSave._Stats_CarsSold = (int)SGETGCarsSoldnumericUpDown.Value;
            LocalSave._Stats_JobsCompletted = (int)SGETGJobsCompletednumericUpDown.Value;
            LocalSave._Stats_CarsOwned = (int)SGETGCarsOwnednumericUpDown.Value;
            LocalSave._Stats_MoneyIncome = (int)SGETGMoneyIncomenumericUpDown.Value;
            LocalSave._Stats_PartsUnmounted = (int)SGETGPartsUnmountednumericUpDown.Value;
            LocalSave._Stats_Bolts = (int)SGETGBoltsUndonenumericUpDown.Value;
            LocalSave._bankLoan = (int)SGETBankLoannumericUpDown.Value;
            LocalSave._xp = (int)SGETGXPnumericUpDown.Value;
            LocalSave._money = (int)SGETGMoneynumericUpDown.Value;

            //Save the file
            LocalSave.WriteGlobalSaveFile(ModMan.GetSavedGamesDir() + "\\" + SGETProfilecomboBox.Text);
        }

        //Handles a change in the selected save profile combo box
        private void SGETGProfilecomboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            //Global file
            ClearOutSaveDataTabsGlobal(); //Clear out the GUI
            SGEGobalFileLoad();     //Load the Global tab data
            //Garage Customisation file
            ClearOutSaveDataTabsGarageCustomisation();
            SGEGarageFileLoad();
        }

        #endregion

        #region Save File - Garage Customisation file
        //Resets the Save Data Garage Custmisation tab GUI elements
        private void ClearOutSaveDataTabsGarageCustomisation()
        {
            //Clear out the GUI

            //Garage skills unlocks
            SGETGCFastUnboltingcheckBox.Checked = false;
            SGETGCFastAssemblycheckBox.Checked = false;
            SGETGCFastExaminecheckBox.Checked = false;
            SGETGCTabletcheckBox.Checked = false;
            SGETGCRepairLv1checkBox.Checked = false;
            SGETGCRepairLv2checkBox.Checked = false;
            SGETGCRepairLv3checkBox.Checked = false;
            SGETGCRepairLv4checkBox.Checked = false;
            SGETGCRepairLv5checkBox.Checked = false;
            SGETGCOBDReadercheckBox.Checked = false;
            SGETGCCompressionTestercheckBox.Checked = false;
            SGETGCElectricalReadercheckBox.Checked = false;

            //Garage customisation unlocks
            SGETGCWallAcheckBox.Checked = false;
            SGETGCWallBcheckBox.Checked = false;
            SGETGCFloorAcheckBox.Checked = false;
            SGETGCFloor2BcheckBox.Checked = false;
            SGETGCFloorStripescheckBox.Checked = false;
            SGETGCWindowscheckBox.Checked = false;
            SGETGCFrameworkcheckBox.Checked = false;
            SGETGCCarLifterscheckBox.Checked = false;
            SGETGCPaintShopcheckBox.Checked = false;
            SGETGCTestRoadcheckBox.Checked = false;
            SGETGCParkingABcheckBox.Checked = false;
            SGETGCParkingCcheckBox.Checked = false;
            SGETGCParkingDcheckBox.Checked = false;
            SGETGCParkingEcheckBox.Checked = false;
            SGETGCParkingFcheckBox.Checked = false;
            SGETGCParkingGcheckBox.Checked = false;
            SGETGCParkingHcheckBox.Checked = false;
        }

        //Load the global save file
        private void SGEGarageFileLoad()
        {
            //Check the combo box text isn't blank
            if (SGETProfilecomboBox.Text != "")
            {
                //Create a local to get save data
                SaveGameDataGarage LocalGrab = new SaveGameDataGarage();
                //Check if the file exists
                if (LocalGrab.LoadGarageSaveFile(ModMan.GetSavedGamesDir() + "\\" + SGETProfilecomboBox.Text))     //Load the save file
                {
                    //Fill out the GUI

                    //Garage skills unlocks
                    SGETGCFastUnboltingcheckBox.Checked = LocalGrab._FastUnbolting;
                    SGETGCFastAssemblycheckBox.Checked = LocalGrab._FastAssembly;
                    SGETGCFastExaminecheckBox.Checked = LocalGrab._FastExamine;
                    SGETGCTabletcheckBox.Checked = LocalGrab._Tablet;
                    SGETGCRepairLv1checkBox.Checked = LocalGrab._RepairLv1;
                    SGETGCRepairLv2checkBox.Checked = LocalGrab._RepairLv2;
                    SGETGCRepairLv3checkBox.Checked = LocalGrab._RepairLv3;
                    SGETGCRepairLv4checkBox.Checked = LocalGrab._RepairLv4;
                    SGETGCRepairLv5checkBox.Checked = LocalGrab._RepairLv5;
                    SGETGCOBDReadercheckBox.Checked = LocalGrab._OBDReader;
                    SGETGCCompressionTestercheckBox.Checked = LocalGrab._CompressionTester;
                    SGETGCElectricalReadercheckBox.Checked = LocalGrab._ElectricalReader;

                    //Garage customisation unlocks
                    SGETGCWallAcheckBox.Checked = LocalGrab._WallA;
                    SGETGCWallBcheckBox.Checked = LocalGrab._WallB;
                    SGETGCFloorAcheckBox.Checked = LocalGrab._FloorA;
                    SGETGCFloor2BcheckBox.Checked = LocalGrab._Floor2B;
                    SGETGCFloorStripescheckBox.Checked = LocalGrab._FloorStripes;
                    SGETGCWindowscheckBox.Checked = LocalGrab._Windows;
                    SGETGCFrameworkcheckBox.Checked = LocalGrab._Framework;
                    SGETGCCarLifterscheckBox.Checked = LocalGrab._CarLifters;
                    SGETGCPaintShopcheckBox.Checked = LocalGrab._PaintShop;
                    SGETGCTestRoadcheckBox.Checked = LocalGrab._TestRoad;
                    SGETGCParkingABcheckBox.Checked = LocalGrab._ParkingAB;
                    SGETGCParkingCcheckBox.Checked = LocalGrab._ParkingC;
                    SGETGCParkingDcheckBox.Checked = LocalGrab._ParkingE;
                    SGETGCParkingEcheckBox.Checked = LocalGrab._ParkingD;
                    SGETGCParkingFcheckBox.Checked = LocalGrab._ParkingF;
                    SGETGCParkingGcheckBox.Checked = LocalGrab._ParkingG;
                    SGETGCParkingHcheckBox.Checked = LocalGrab._ParkingH;
                }
            }
        }

        #region Garage Skill unlock checkboxes
        //Comments apply to all functions
        //Set all levels below on set
        //Unset all levels above on unset
        private void SGETGCRepairLv1checkBox_CheckedChanged(object sender, EventArgs e)
        {
            if (SGETGCRepairLv1checkBox.Checked == true)
            {
                //No lower to set
            }
            else
            {
                //Unset all above
                SGETGCRepairLv2checkBox.Checked = false;
                SGETGCRepairLv3checkBox.Checked = false;
                SGETGCRepairLv4checkBox.Checked = false;
                SGETGCRepairLv5checkBox.Checked = false;
            }
        }

        private void SGETGCRepairLv2checkBox_CheckedChanged(object sender, EventArgs e)
        {
            if (SGETGCRepairLv2checkBox.Checked == true)
            {
                //Set all lower
                SGETGCRepairLv1checkBox.Checked = true;
            }
            else
            {
                //Unset all above
                SGETGCRepairLv3checkBox.Checked = false;
                SGETGCRepairLv4checkBox.Checked = false;
                SGETGCRepairLv5checkBox.Checked = false;
            }
        }

        private void SGETGCRepairLv3checkBox_CheckedChanged(object sender, EventArgs e)
        {
            if (SGETGCRepairLv3checkBox.Checked == true)
            {
                //Set all lower
                SGETGCRepairLv1checkBox.Checked = true;
                SGETGCRepairLv2checkBox.Checked = true;
            }
            else
            {
                //Unset all above
                SGETGCRepairLv4checkBox.Checked = false;
                SGETGCRepairLv5checkBox.Checked = false;
            }
        }

        private void SGETGCRepairLv4checkBox_CheckedChanged(object sender, EventArgs e)
        {
            if (SGETGCRepairLv4checkBox.Checked == true)
            {
                //Set all lower
                SGETGCRepairLv1checkBox.Checked = true;
                SGETGCRepairLv2checkBox.Checked = true;
                SGETGCRepairLv3checkBox.Checked = true;
            }
            else
            {
                //Unset all above
                SGETGCRepairLv5checkBox.Checked = false;
            }
        }

        private void SGETGCRepairLv5checkBox_CheckedChanged(object sender, EventArgs e)
        {
            if (SGETGCRepairLv5checkBox.Checked == true)
            {
                //Set all lower
                SGETGCRepairLv1checkBox.Checked = true;
                SGETGCRepairLv2checkBox.Checked = true;
                SGETGCRepairLv3checkBox.Checked = true;
                SGETGCRepairLv4checkBox.Checked = true;
            }
            else
            {
                //No above to set                
            }
        }
        #endregion

        #region Garage Visual Customisation unlock checkboxes        
        //Comments apply to all functions
        //Set all levels below on set
        //Unset all levels above on unset
        private void SGETGCParkingABcheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (SGETGCParkingABcheckBox.Checked == true)
            {
                //No lower to set
            }
            else
            {
                //Unset all above
                SGETGCParkingCcheckBox.Checked = false;
                SGETGCParkingDcheckBox.Checked = false;
                SGETGCParkingEcheckBox.Checked = false;
                SGETGCParkingFcheckBox.Checked = false;
                SGETGCParkingGcheckBox.Checked = false;
                SGETGCParkingHcheckBox.Checked = false;
            }
        }

        private void SGETGCParkingCcheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (SGETGCParkingCcheckBox.Checked == true)
            {
                //Set all lower
                SGETGCParkingABcheckBox.Checked = true;
            }
            else
            {
                //Unset all above
                SGETGCParkingDcheckBox.Checked = false;
                SGETGCParkingEcheckBox.Checked = false;
                SGETGCParkingFcheckBox.Checked = false;
                SGETGCParkingGcheckBox.Checked = false;
                SGETGCParkingHcheckBox.Checked = false;
            }
        }

        private void SGETGCParkingDcheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (SGETGCParkingDcheckBox.Checked == true)
            {
                //Set all lower
                SGETGCParkingABcheckBox.Checked = true;
                SGETGCParkingCcheckBox.Checked = true;
            }
            else
            {
                //Unset all above
                SGETGCParkingEcheckBox.Checked = false;
                SGETGCParkingFcheckBox.Checked = false;
                SGETGCParkingGcheckBox.Checked = false;
                SGETGCParkingHcheckBox.Checked = false;
            }
        }

        private void SGETGCParkingEcheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (SGETGCParkingEcheckBox.Checked == true)
            {
                //Set all lower
                SGETGCParkingABcheckBox.Checked = true;
                SGETGCParkingCcheckBox.Checked = true;
                SGETGCParkingDcheckBox.Checked = true;
            }
            else
            {
                //Unset all above
                SGETGCParkingFcheckBox.Checked = false;
                SGETGCParkingGcheckBox.Checked = false;
                SGETGCParkingHcheckBox.Checked = false;
            }
        }

        private void SGETGCParkingFcheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (SGETGCParkingFcheckBox.Checked == true)
            {
                //Set all lower
                SGETGCParkingABcheckBox.Checked = true;
                SGETGCParkingCcheckBox.Checked = true;
                SGETGCParkingDcheckBox.Checked = true;
                SGETGCParkingEcheckBox.Checked = true;
            }
            else
            {
                SGETGCParkingGcheckBox.Checked = false;
                SGETGCParkingHcheckBox.Checked = false;
            }
        }

        private void SGETGCParkingGcheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (SGETGCParkingGcheckBox.Checked == true)
            {
                //Set all lower
                SGETGCParkingABcheckBox.Checked = true;
                SGETGCParkingCcheckBox.Checked = true;
                SGETGCParkingDcheckBox.Checked = true;
                SGETGCParkingEcheckBox.Checked = true;
                SGETGCParkingFcheckBox.Checked = true;
            }
            else
            {
                SGETGCParkingHcheckBox.Checked = false;
            }
        }

        private void SGETGCParkingHcheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (SGETGCParkingHcheckBox.Checked == true)
            {
                //Set all lower
                SGETGCParkingABcheckBox.Checked = true;
                SGETGCParkingCcheckBox.Checked = true;
                SGETGCParkingDcheckBox.Checked = true;
                SGETGCParkingEcheckBox.Checked = true;
                SGETGCParkingFcheckBox.Checked = true;
                SGETGCParkingGcheckBox.Checked = true;
            }
            else
            {
                //None
            }
        }

        #endregion

        //Save the garage customisation save file
        private void SGETGCSavebutton_Click(object sender, EventArgs e)
        {
            //Create a local to hold save data
            SaveGameDataGarage LocalSave = new SaveGameDataGarage();

            //Fill out the data object

            //Garage skills unlocks   
            LocalSave._FastUnbolting = SGETGCFastUnboltingcheckBox.Checked;
            LocalSave._FastAssembly = SGETGCFastAssemblycheckBox.Checked;
            LocalSave._FastExamine = SGETGCFastExaminecheckBox.Checked;
            LocalSave._Tablet = SGETGCTabletcheckBox.Checked;
            LocalSave._RepairLv1 = SGETGCRepairLv1checkBox.Checked;
            LocalSave._RepairLv2 = SGETGCRepairLv2checkBox.Checked;
            LocalSave._RepairLv3 = SGETGCRepairLv3checkBox.Checked;
            LocalSave._RepairLv4 = SGETGCRepairLv4checkBox.Checked;
            LocalSave._RepairLv5 = SGETGCRepairLv5checkBox.Checked;
            LocalSave._OBDReader = SGETGCOBDReadercheckBox.Checked;
            LocalSave._CompressionTester = SGETGCCompressionTestercheckBox.Checked;
            LocalSave._ElectricalReader = SGETGCElectricalReadercheckBox.Checked;

            //Garage customisation unlocks
            LocalSave._WallA = SGETGCWallAcheckBox.Checked;
            LocalSave._WallB = SGETGCWallBcheckBox.Checked;
            LocalSave._FloorA = SGETGCFloorAcheckBox.Checked;
            LocalSave._Floor2B = SGETGCFloor2BcheckBox.Checked;
            LocalSave._FloorStripes = SGETGCFloorStripescheckBox.Checked;
            LocalSave._Windows = SGETGCWindowscheckBox.Checked;
            LocalSave._Framework = SGETGCFrameworkcheckBox.Checked;
            LocalSave._CarLifters = SGETGCCarLifterscheckBox.Checked;
            LocalSave._PaintShop = SGETGCPaintShopcheckBox.Checked;
            LocalSave._TestRoad = SGETGCTestRoadcheckBox.Checked;
            LocalSave._ParkingAB = SGETGCParkingABcheckBox.Checked;
            LocalSave._ParkingC = SGETGCParkingCcheckBox.Checked;
            LocalSave._ParkingE = SGETGCParkingDcheckBox.Checked;
            LocalSave._ParkingD = SGETGCParkingEcheckBox.Checked;
            LocalSave._ParkingF = SGETGCParkingFcheckBox.Checked;
            LocalSave._ParkingG = SGETGCParkingGcheckBox.Checked;
            LocalSave._ParkingH = SGETGCParkingHcheckBox.Checked;

            //Save the file
            LocalSave.WriteGarageSaveFile(ModMan.GetSavedGamesDir() + "\\" + SGETProfilecomboBox.Text);
        }

        #endregion

    }
}
