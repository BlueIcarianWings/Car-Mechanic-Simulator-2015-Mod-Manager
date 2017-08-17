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

namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        //Class object for class that does the acutal mod managing stuff    //here so it's scope is within the form object
        CMS2015MM ModMan;

        public Form1()
        {
            //Setup the form controls
            InitializeComponent();
            //setup the class that does the acutal mod managing stuff
            ModMan = new CMS2015MM();
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

            //Car Data Tab
            //List of available car text file already loaded
            //Populate the available cars drop down list
            PopulateAvailableCarsComboBox();

            //Engine Data Tab
            //load the engine data file
            ModMan.LoadEngineDataFile();
            //Populate the engine data drop down list
            PopulateAvailableEnginesComboBox();
        }

        //Populate the engine data drop down combo box
        private void PopulateAvailableEnginesComboBox()
        {
            int index = 0;  //local loop counter
            //Loop through all of the loaded engines
            while (index < ModMan.GetEngineDataArraySize())
            {
                //Add an item to the engines data combo box
                AvailableEnginesComboBox.Items.Add(ModMan.GetEngineDataName(index));
                index++;    //inc counter
            }
        }

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

        //Handle set car data source click from menu strip
        private void setSourceDirToolStripMenuItemCar_Click(object sender, EventArgs e)
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

        //Handle set car data backup click from menu strip
        private void setBackupDirToolStripMenuItemCar_Click(object sender, EventArgs e)
        {
            //Open up a folder browser
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            //Get the result and check the dialog was ok'd
            DialogResult result = fbd.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                //Retrieve and set the selected dir
                ModMan.SetCarsDataDirBkUp(fbd.SelectedPath);
                //Update the config file
                ModMan.SaveConfigFile();
            }
        }

        //Handle a click on the about menu item
        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Car Mechanic Simulator 2015 Mod Manager\nVery much a work in progress\nDesigned for V1.0.6.3");
        }

        //Handle the menu request to backup the save dir
        private void backupSavesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ModMan.DirectoryCopy(ModMan.GetSavedGamesDir(), ModMan.GetSavedGamesDirBkUp(), true);
        }

        //Handle the menu request to restore the saves dir
        private void restoreSavesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ModMan.DirectoryCopy(ModMan.GetSavedGamesDirBkUp(), ModMan.GetSavedGamesDir(), true);
        }

        //Handle the menu request to backup the car data dir
        private void backupCarDataToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ModMan.DirectoryCopy(ModMan.GetCarsDataDir(), ModMan.GetCarsDataDirBkUp(), true);
        }

        //Handle the menu request to restore the car data dir
        private void restoreCarDataToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ModMan.DirectoryCopy(ModMan.GetCarsDataDirBkUp(), ModMan.GetCarsDataDir(), true);
        }

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

        //Handle a selection in the engines combo box
        private void AvailableEnginesComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            //Get the index of the selected engine
            int Index = AvailableEnginesComboBox.SelectedIndex;

            //Set the data fields with the values from the selected engine
            EDTmaxPowerNumericUpDown.Value = ModMan.GetEngineDataMaxPower(Index);
            EDTmaxPowerRPMNumericUpDown.Value = ModMan.GetEngineDataMaxPowerRPM(Index);
            EDTmaxTorqueRPMNumericUpDown.Value = ModMan.GetEngineDataMaxTorqueRPM(Index);
            EDTminRPMLabelNumericUpDown.Value = ModMan.GetEngineDataMinRPM(Index);
            EDTmaxRPMLabelNumericUpDown.Value = ModMan.GetEngineDataMaxRPM(Index);
        }

        //Send the engine data fields to the engine data file (changes internal values which the reset reads from)
        private void EDTCommitButton_Click(object sender, EventArgs e)
        {
            //Check if ok to save file
            if (MessageBox.Show("Update Engine Data file?\nReset will use these values from now on", "Confirm commit", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                //Get the index of the selected engine
                int Index = AvailableEnginesComboBox.SelectedIndex;

                //send the new engine data values
                ModMan.SetEngineDataMaxPower(Index, (int)EDTmaxPowerNumericUpDown.Value);   //Need to cast the decimal to an int
                ModMan.SetEngineDataMaxPowerRPM(Index, (int)EDTmaxPowerRPMNumericUpDown.Value);
                ModMan.SetEngineDataMaxTorqueRPM(Index, (int)EDTmaxTorqueRPMNumericUpDown.Value);
                ModMan.SetEngineDataMinRPM(Index, (int)EDTminRPMLabelNumericUpDown.Value);
                ModMan.SetEngineDataMaxRPM(Index, (int)EDTmaxRPMLabelNumericUpDown.Value);

                //write to file
                ModMan.WriteEngineDataFile();
            }
        }

        //Reset all the engine data fields, from stored values
        private void EDTResetButton_Click(object sender, EventArgs e)
        {
            //Reset the engine data values, just call the other function that sets them initialy
            AvailableEnginesComboBox_SelectedIndexChanged(sender, e);
        }

        //Populate the cars currently in list, list box
        private void PopulateCarsCurrentList()
        {
            int index = 0;      //local loop counter
            string name = null; //local to assemble string for list

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
                ModMan.LoadCarDataFromFile(AvailableCarsListBox.Text);
                if (ModMan.LoadedCarData.Main.name != "")
                {
                    //If name isn't null fill it in
                    InGameName = ModMan.LoadedCarData.Main.name;
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
            PicturePath = ModMan.GetCarsDataDir() + "\\" + AvailableCarsDataComboBox.Text;
            PicturePath = PicturePath.Substring(0, PicturePath.Length - 3);     //Remove txt
            PicturePath = PicturePath + "jpg";                                  //Add jpg

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
            ModMan.LoadCarDataFromFile(AvailableCarsDataComboBox.Text);

            //Update the GUI to display it
            //Will need a function to clear down all the GUI

            //[Main] section
            CDMNameTextBox.Text = ModMan.LoadedCarData.Main.name;
            CDMModelTextBox.Text = ModMan.LoadedCarData.Main.model;
            CDMRustMaskTextBox.Text = ModMan.LoadedCarData.Main.rustMask;
            CDMRotXnumericUpDown.Value = (Decimal)ModMan.LoadedCarData.Main.rotation.x;     //Cast to decimal as it's what the NumUpDown uses
            CDMRotYnumericUpDown.Value = (Decimal)ModMan.LoadedCarData.Main.rotation.y;
            CDMRotZnumericUpDown.Value = (Decimal)ModMan.LoadedCarData.Main.rotation.z;

            //[Other] section
            CDOEngineSoundtextBox.Text = ModMan.LoadedCarData.Other.engineSound;
            CDOTranstextBox.Text = ModMan.LoadedCarData.Other.transmissionType;
            CDOGearsnumericUpDown.Value = (Decimal)ModMan.LoadedCarData.Other.gears;
            CDOFinalDrivenumericUpDown.Value = (Decimal)ModMan.LoadedCarData.Other.finalDriveRatio;
            CDOWeightnumericUpDown.Value = (Decimal)ModMan.LoadedCarData.Other.weight;
            CDORPMFactornumericUpDown.Value = (Decimal)ModMan.LoadedCarData.Other.rpmFactor;
            CDORPMAnglenumericUpDown.Value = (Decimal)ModMan.LoadedCarData.Other.rpmAngle;
            CDOSpeedoAnglenumericUpDown.Value = (Decimal)ModMan.LoadedCarData.Other.speedoAngle;
            CDOSuspTRavelnumericUpDown.Value = (Decimal)ModMan.LoadedCarData.Other.suspTravel;
            CDOLifterArmsRisenumericUpDown.Value = (Decimal)ModMan.LoadedCarData.Other.lifterArmsRise;
            CDOCXNumericUpDown.Value = (Decimal)ModMan.LoadedCarData.Other.cx;

            //[Suspension] section
            CDSFrontAxleStartnumericUpDown.Value = (Decimal)ModMan.LoadedCarData.Suspension.frontAxleStart;
            CDSWheelBasenumericUpDown.Value = (Decimal)ModMan.LoadedCarData.Suspension.wheelBase;
            CDSHeightnumericUpDown.Value = (Decimal)ModMan.LoadedCarData.Suspension.height;
            CDSFrontTracknumericUpDown.Value = (Decimal)ModMan.LoadedCarData.Suspension.frontTrack;
            CDSRearTracknumericUpDown.Value = (Decimal)ModMan.LoadedCarData.Suspension.rearTrack;
            CDSFrontSpringLengthnumericUpDown.Value = (Decimal)ModMan.LoadedCarData.Suspension.frontSpringLength;
            CDSScalenumericUpDown.Value = (Decimal)ModMan.LoadedCarData.Suspension.scale;

            CDSFrontCenterSettextBox.Text = ModMan.LoadedCarData.Suspension.frontCenterSet;
            CDSFrontRightSettextBox.Text = ModMan.LoadedCarData.Suspension.frontRightSet;
            CDSRearCenterSettextBox.Text = ModMan.LoadedCarData.Suspension.rearCenterSet;
            CDSRearRightSettextBox.Text = ModMan.LoadedCarData.Suspension.rearRightSet;

            //[Engine] section
            CDEPosXnumericUpDown.Value = (Decimal)ModMan.LoadedCarData.Engine.position.x;
            CDEPosYnumericUpDown.Value = (Decimal)ModMan.LoadedCarData.Engine.position.y;
            CDEPosZnumericUpDown.Value = (Decimal)ModMan.LoadedCarData.Engine.position.z;
            CDERotXnumericUpDown.Value = (Decimal)ModMan.LoadedCarData.Engine.rotation.x;
            CDERotYnumericUpDown.Value = (Decimal)ModMan.LoadedCarData.Engine.rotation.y;
            CDERotZnumericUpDown.Value = (Decimal)ModMan.LoadedCarData.Engine.rotation.z;
            CDEScalenumericUpDown.Value = (Decimal)ModMan.LoadedCarData.Engine.scale;
            CDETypetextBox.Text = ModMan.LoadedCarData.Engine.type;
            CDESoundtextBox.Text = ModMan.LoadedCarData.Engine.sound;

            //[Driveshaft] section
            CDDPosXnumericUpDown.Value = (Decimal)ModMan.LoadedCarData.Driveshaft.position.x;
            CDDPosYnumericUpDown.Value = (Decimal)ModMan.LoadedCarData.Driveshaft.position.y;
            CDDPosZnumericUpDown.Value = (Decimal)ModMan.LoadedCarData.Driveshaft.position.z;
            CDDRotXnumericUpDown.Value = (Decimal)ModMan.LoadedCarData.Driveshaft.rotation.x;
            CDDRotYnumericUpDown.Value = (Decimal)ModMan.LoadedCarData.Driveshaft.rotation.y;
            CDDRotZnumericUpDown.Value = (Decimal)ModMan.LoadedCarData.Driveshaft.rotation.z;
            CDDScalenumericUpDown.Value = (Decimal)ModMan.LoadedCarData.Driveshaft.scale;
            CDDLengthnumericUpDown.Value = (Decimal)ModMan.LoadedCarData.Driveshaft.length;
            CDDTypetextBox.Text = ModMan.LoadedCarData.Driveshaft.type;

            //[Wheel] section
            CDWWheelWidthnumericUpDown.Value = (Decimal)ModMan.LoadedCarData.Wheels.wheelWidth;
            CDWRimSizenumericUpDown.Value = (Decimal)ModMan.LoadedCarData.Wheels.rimSize;
            CDWTireSizenumericUpDown.Value = (Decimal)ModMan.LoadedCarData.Wheels.tireSize;
            CDWTiretextBox.Text = ModMan.LoadedCarData.Wheels.tire;
            CDWRimtextBox.Text = ModMan.LoadedCarData.Wheels.rim;

            //[Interior] section
            CDISLPosXnumericUpDown.Value = (Decimal)ModMan.LoadedCarData.Interior.seatLeftPos.x;
            CDISLPosYnumericUpDown.Value = (Decimal)ModMan.LoadedCarData.Interior.seatLeftPos.y;
            CDISLPosZnumericUpDown.Value = (Decimal)ModMan.LoadedCarData.Interior.seatLeftPos.z;
            CDISLRotXnumericUpDown.Value = (Decimal)ModMan.LoadedCarData.Interior.seatLeftRot.x;
            CDISLRotYnumericUpDown.Value = (Decimal)ModMan.LoadedCarData.Interior.seatLeftRot.y;
            CDISLRotZnumericUpDown.Value = (Decimal)ModMan.LoadedCarData.Interior.seatLeftRot.z;
            CDISeatScalenumericUpDown.Value = (Decimal)ModMan.LoadedCarData.Interior.seatScale;
            CDISeattextBox.Text = ModMan.LoadedCarData.Interior.seat;
            CDIWheeltextBox.Text = ModMan.LoadedCarData.Interior.wheel;
            CDIWheelPosXnumericUpDown.Value = (Decimal)ModMan.LoadedCarData.Interior.wheelPos.x;
            CDIWheelPosYnumericUpDown.Value = (Decimal)ModMan.LoadedCarData.Interior.wheelPos.y;
            CDIWheelPosZnumericUpDown.Value = (Decimal)ModMan.LoadedCarData.Interior.wheelPos.z;
            CDIWheelRotXnumericUpDown.Value = (Decimal)ModMan.LoadedCarData.Interior.wheelRot.x;
            CDIWheelRotYnumericUpDown.Value = (Decimal)ModMan.LoadedCarData.Interior.wheelRot.y;
            CDIWheelRotZnumericUpDown.Value = (Decimal)ModMan.LoadedCarData.Interior.wheelRot.z;

            //[Logic] section
            CDLGloConAnumericUpDown.Value = (Decimal)ModMan.LoadedCarData.Logic.globalCondition.a;
            CDLGloConBnumericUpDown.Value = (Decimal)ModMan.LoadedCarData.Logic.globalCondition.b;
            CDLPartConAnumericUpDown.Value = (Decimal)ModMan.LoadedCarData.Logic.partsConditions.a;
            CDLPartConBnumericUpDown.Value = (Decimal)ModMan.LoadedCarData.Logic.partsConditions.b;
            CDLPanConAnumericUpDown.Value = (Decimal)ModMan.LoadedCarData.Logic.panelsConditions.a;
            CDLPanConBnumericUpDown.Value = (Decimal)ModMan.LoadedCarData.Logic.panelsConditions.b;
            CDLBlockOBDcheckBox.Checked = ModMan.LoadedCarData.Logic.blockOBD;

            //Need to deal with the Parts info
            int CDPListSize;
            CDPListSize = ModMan.GetCarDataPartsArraySize();

            //Currently we can only handle 6 entries, so trim to that if it's greater
            if(CDPListSize>7)
            {
                CDPListSize = 7;
            }

            //Fill out the parts
            switch (CDPListSize)    //This setup will grab the highest numbered one, then flow down
            {
                case 7:
                    CDP6NametextBox.Text = ModMan.LoadedCarData.Parts[6].name;
                    CDP6PosXnumericUpDown.Value = (Decimal)ModMan.LoadedCarData.Parts[6].position.x;
                    CDP6PosYnumericUpDown.Value = (Decimal)ModMan.LoadedCarData.Parts[6].position.y;
                    CDP6PosZnumericUpDown.Value = (Decimal)ModMan.LoadedCarData.Parts[6].position.z;
                    CDP6RotXnumericUpDown.Value = (Decimal)ModMan.LoadedCarData.Parts[6].rotation.x;
                    CDP6RotYnumericUpDown.Value = (Decimal)ModMan.LoadedCarData.Parts[6].rotation.y;
                    CDP6RotZnumericUpDown.Value = (Decimal)ModMan.LoadedCarData.Parts[6].rotation.z;
                    CDP6ScalenumericUpDown.Value = (Decimal)ModMan.LoadedCarData.Parts[6].scale;
                    goto case 6;    //Switch statment fall through is not a thing in C#, so we have to use a 'goto' to force it
                case 6:
                    CDP5NametextBox.Text = ModMan.LoadedCarData.Parts[5].name;
                    CDP5PosXnumericUpDown.Value = (Decimal)ModMan.LoadedCarData.Parts[5].position.x;
                    CDP5PosYnumericUpDown.Value = (Decimal)ModMan.LoadedCarData.Parts[5].position.y;
                    CDP5PosZnumericUpDown.Value = (Decimal)ModMan.LoadedCarData.Parts[5].position.z;
                    CDP5RotXnumericUpDown.Value = (Decimal)ModMan.LoadedCarData.Parts[5].rotation.x;
                    CDP5RotYnumericUpDown.Value = (Decimal)ModMan.LoadedCarData.Parts[5].rotation.y;
                    CDP5RotZnumericUpDown.Value = (Decimal)ModMan.LoadedCarData.Parts[5].rotation.z;
                    CDP5ScalenumericUpDown.Value = (Decimal)ModMan.LoadedCarData.Parts[5].scale;
                    goto case 5;
                case 5:
                    CDP4NametextBox.Text = ModMan.LoadedCarData.Parts[4].name;
                    CDP4PosXnumericUpDown.Value = (Decimal)ModMan.LoadedCarData.Parts[4].position.x;
                    CDP4PosYnumericUpDown.Value = (Decimal)ModMan.LoadedCarData.Parts[4].position.y;
                    CDP4PosZnumericUpDown.Value = (Decimal)ModMan.LoadedCarData.Parts[4].position.z;
                    CDP4RotXnumericUpDown.Value = (Decimal)ModMan.LoadedCarData.Parts[4].rotation.x;
                    CDP4RotYnumericUpDown.Value = (Decimal)ModMan.LoadedCarData.Parts[4].rotation.y;
                    CDP4RotZnumericUpDown.Value = (Decimal)ModMan.LoadedCarData.Parts[4].rotation.z;
                    CDP4ScalenumericUpDown.Value = (Decimal)ModMan.LoadedCarData.Parts[4].scale;
                    goto case 4;
                case 4:
                    CDP3NametextBox.Text = ModMan.LoadedCarData.Parts[3].name;
                    CDP3PosXnumericUpDown.Value = (Decimal)ModMan.LoadedCarData.Parts[3].position.x;
                    CDP3PosYnumericUpDown.Value = (Decimal)ModMan.LoadedCarData.Parts[3].position.y;
                    CDP3PosZnumericUpDown.Value = (Decimal)ModMan.LoadedCarData.Parts[3].position.z;
                    CDP3RotXnumericUpDown.Value = (Decimal)ModMan.LoadedCarData.Parts[3].rotation.x;
                    CDP3RotYnumericUpDown.Value = (Decimal)ModMan.LoadedCarData.Parts[3].rotation.y;
                    CDP3RotZnumericUpDown.Value = (Decimal)ModMan.LoadedCarData.Parts[3].rotation.z;
                    CDP3ScalenumericUpDown.Value = (Decimal)ModMan.LoadedCarData.Parts[3].scale;
                    goto case 3;
                case 3:
                    CDP2NametextBox.Text = ModMan.LoadedCarData.Parts[2].name;
                    CDP2PosXnumericUpDown.Value = (Decimal)ModMan.LoadedCarData.Parts[2].position.x;
                    CDP2PosYnumericUpDown.Value = (Decimal)ModMan.LoadedCarData.Parts[2].position.y;
                    CDP2PosZnumericUpDown.Value = (Decimal)ModMan.LoadedCarData.Parts[2].position.z;
                    CDP2RotXnumericUpDown.Value = (Decimal)ModMan.LoadedCarData.Parts[2].rotation.x;
                    CDP2RotYnumericUpDown.Value = (Decimal)ModMan.LoadedCarData.Parts[2].rotation.y;
                    CDP2RotZnumericUpDown.Value = (Decimal)ModMan.LoadedCarData.Parts[2].rotation.z;
                    CDP2ScalenumericUpDown.Value = (Decimal)ModMan.LoadedCarData.Parts[2].scale;
                    goto case 2;
                case 2:
                    CDP1NametextBox.Text = ModMan.LoadedCarData.Parts[1].name;
                    CDP1PosXnumericUpDown.Value = (Decimal)ModMan.LoadedCarData.Parts[1].position.x;
                    CDP1PosYnumericUpDown.Value = (Decimal)ModMan.LoadedCarData.Parts[1].position.y;
                    CDP1PosZnumericUpDown.Value = (Decimal)ModMan.LoadedCarData.Parts[1].position.z;
                    CDP1RotXnumericUpDown.Value = (Decimal)ModMan.LoadedCarData.Parts[1].rotation.x;
                    CDP1RotYnumericUpDown.Value = (Decimal)ModMan.LoadedCarData.Parts[1].rotation.y;
                    CDP1RotZnumericUpDown.Value = (Decimal)ModMan.LoadedCarData.Parts[1].rotation.z;
                    CDP1ScalenumericUpDown.Value = (Decimal)ModMan.LoadedCarData.Parts[1].scale;
                    goto case 1;
                case 1:
                    CDP0NametextBox.Text = ModMan.LoadedCarData.Parts[0].name;
                    CDP0PosXnumericUpDown.Value = (Decimal)ModMan.LoadedCarData.Parts[0].position.x;
                    CDP0PosYnumericUpDown.Value = (Decimal)ModMan.LoadedCarData.Parts[0].position.y;
                    CDP0PosZnumericUpDown.Value = (Decimal)ModMan.LoadedCarData.Parts[0].position.z;
                    CDP0RotXnumericUpDown.Value = (Decimal)ModMan.LoadedCarData.Parts[0].rotation.x;
                    CDP0RotYnumericUpDown.Value = (Decimal)ModMan.LoadedCarData.Parts[0].rotation.y;
                    CDP0RotZnumericUpDown.Value = (Decimal)ModMan.LoadedCarData.Parts[0].rotation.z;
                    CDP0ScalenumericUpDown.Value = (Decimal)ModMan.LoadedCarData.Parts[0].scale;
                    break;
                default:
                    //Nothing to do
                    break;

            }
        }

        //Resets the Car Data tab GUI elements
        public void ClearOutCarDataTabGUI()
        {
            //Drop Down box
            AvailableCarsDataComboBox.Text = "";

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
            CDOEngineSoundtextBox.Text = "";
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

            //[Suspension] section
            CDSFrontAxleStartnumericUpDown.Value = 0;
            CDSWheelBasenumericUpDown.Value = 0;
            CDSHeightnumericUpDown.Value = 0;
            CDSFrontTracknumericUpDown.Value = 0;
            CDSRearTracknumericUpDown.Value = 0;
            CDSFrontSpringLengthnumericUpDown.Value = 0;
            CDSScalenumericUpDown.Value = 0;

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
            CDETypetextBox.Text = "";
            CDESoundtextBox.Text = "";

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
            CDWTiretextBox.Text = "";
            CDWRimtextBox.Text = "";

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
            CDLBlockOBDcheckBox.Checked = false;

            //[Parts] section
            //Hardcoded badness, should look into building this up programmatically at somepoint
            //Parts0
            CDP0NametextBox.Text = "";
            CDP0PosXnumericUpDown.Value = 0;
            CDP0PosYnumericUpDown.Value = 0;
            CDP0PosZnumericUpDown.Value = 0;
            CDP0RotXnumericUpDown.Value = 0;
            CDP0RotYnumericUpDown.Value = 0;
            CDP0RotZnumericUpDown.Value = 0;
            CDP0ScalenumericUpDown.Value = 0;
            //Parts1
            CDP1NametextBox.Text = "";
            CDP1PosXnumericUpDown.Value = 0;
            CDP1PosYnumericUpDown.Value = 0;
            CDP1PosZnumericUpDown.Value = 0;
            CDP1RotXnumericUpDown.Value = 0;
            CDP1RotYnumericUpDown.Value = 0;
            CDP1RotZnumericUpDown.Value = 0;
            CDP1ScalenumericUpDown.Value = 0;
            //Parts2
            CDP2NametextBox.Text = "";
            CDP2PosXnumericUpDown.Value = 0;
            CDP2PosYnumericUpDown.Value = 0;
            CDP2PosZnumericUpDown.Value = 0;
            CDP2RotXnumericUpDown.Value = 0;
            CDP2RotYnumericUpDown.Value = 0;
            CDP2RotZnumericUpDown.Value = 0;
            CDP2ScalenumericUpDown.Value = 0;
            //Parts3
            CDP3NametextBox.Text = "";
            CDP3PosXnumericUpDown.Value = 0;
            CDP3PosYnumericUpDown.Value = 0;
            CDP3PosZnumericUpDown.Value = 0;
            CDP3RotXnumericUpDown.Value = 0;
            CDP3RotYnumericUpDown.Value = 0;
            CDP3RotZnumericUpDown.Value = 0;
            CDP3ScalenumericUpDown.Value = 0;
            //Parts4
            CDP4NametextBox.Text = "";
            CDP4PosXnumericUpDown.Value = 0;
            CDP4PosYnumericUpDown.Value = 0;
            CDP4PosZnumericUpDown.Value = 0;
            CDP4RotXnumericUpDown.Value = 0;
            CDP4RotYnumericUpDown.Value = 0;
            CDP4RotZnumericUpDown.Value = 0;
            CDP4ScalenumericUpDown.Value = 0;
            //Parts5
            CDP5NametextBox.Text = "";
            CDP5PosXnumericUpDown.Value = 0;
            CDP5PosYnumericUpDown.Value = 0;
            CDP5PosZnumericUpDown.Value = 0;
            CDP5RotXnumericUpDown.Value = 0;
            CDP5RotYnumericUpDown.Value = 0;
            CDP5RotZnumericUpDown.Value = 0;
            CDP5ScalenumericUpDown.Value = 0;
            //Parts6
            CDP6NametextBox.Text = "";
            CDP6PosXnumericUpDown.Value = 0;
            CDP6PosYnumericUpDown.Value = 0;
            CDP6PosZnumericUpDown.Value = 0;
            CDP6RotXnumericUpDown.Value = 0;
            CDP6RotYnumericUpDown.Value = 0;
            CDP6RotZnumericUpDown.Value = 0;
            CDP6ScalenumericUpDown.Value = 0;
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

        //Handle a call to exit
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Close this all down
            Application.Exit();
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
                ModMan.DirectoryCopy(fbd.SelectedPath, ModMan.GetModMapDir(), true);
            }
        }

        //Handles a call to reset the Car Data tab GUI elements
        private void CDResetbutton_Click(object sender, EventArgs e)
        {
            //Save the current car
            string CurrentCar = AvailableCarsDataComboBox.Text;
            //Clean up the GUI
            ClearOutCarDataTabGUI();
            //Restore the selected car
            AvailableCarsDataComboBox.Text = CurrentCar;
            //Reload the data
            AvailableCarsDataComboBox_SelectedIndexChanged(sender, e);
        }

        //Handles a call to clear out all data and start again
        private void CDNewbutton_Click(object sender, EventArgs e)
        {
            //Reset the GUI elements
            ClearOutCarDataTabGUI();
            //Empty out all existing data
            ModMan.CarDataSectionClearAll();
        }

        //Handles a call to save the updated data as a new file
        private void CDSaveAsNewbutton_Click(object sender, EventArgs e)
        {

        }

        //Handles a call to save the updated data
        private void CDSavebutton_Click(object sender, EventArgs e)
        {

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
    }
}
