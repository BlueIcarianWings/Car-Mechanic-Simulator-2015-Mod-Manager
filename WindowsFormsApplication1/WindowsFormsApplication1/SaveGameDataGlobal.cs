using System;
using System.IO;    //File read / write
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMS2015ModManager
{
    //Class to hold and manage save game data for the global file
    class SaveGameDataGlobal
    {
        //Class data
        //Values
        private int Stats_PartsRepaired;
        private int Stats_MoneyIncomeParts;
        private int Stats_MoneyIncomeCars;
        private int Stats_CarsSold;
        private int Stats_JobsCompleted;
        private int Stats_CarsOwned;
        private int Stats_MoneyIncome;
        private int Stats_PartsUnmounted;
        private int Stats_Bolts;
        private int bankLoan;
        private int xp;
        private int money;
        //Memory location (values are set in the constructor, and is the bit that will need the most maintenance)
        private int Stats_PartsRepaired_MemLoc;
        private int Stats_MoneyIncomeParts_MemLoc;
        private int Stats_MoneyIncomeCars_MemLoc;
        private int Stats_CarsSold_MemLoc;
        private int Stats_JobsCompleted_MemLoc;
        private int Stats_CarsOwned_MemLoc;
        private int Stats_MoneyIncome_MemLoc;
        private int Stats_PartsUnmounted_MemLoc;
        private int Stats_Bolts_MemLoc;
        private int bankLoan_MemLoc;
        private int xp_MemLoc;
        private int money_MemLoc;

        //--------------------
        //Class methods

        // Constructor
        public SaveGameDataGlobal()
        {
            //Set the memory locations of the values
            Stats_PartsRepaired_MemLoc = 158;//9e
            Stats_MoneyIncomeParts_MemLoc = 196;//c4
            Stats_MoneyIncomeCars_MemLoc = 233;//e9
            Stats_CarsSold_MemLoc = 263;//107
            Stats_JobsCompleted_MemLoc = 299;//12b
            Stats_CarsOwned_MemLoc = 330;//14a
            Stats_MoneyIncome_MemLoc = 363;//16b
            Stats_PartsUnmounted_MemLoc = 399;//18f
            Stats_Bolts_MemLoc = 426;//1aa
            bankLoan_MemLoc = 450;//1c2
            xp_MemLoc = 468;//1d4
            money_MemLoc = 489;//1e9
        }

        //Loads a car data file into the object from the fullpath and filename given
        public void LoadGlobalSaveFile(string path)
        {
            string fullpath = path + "\\global";
            using (BinaryReader b = new BinaryReader(File.Open(fullpath, FileMode.Open)))
            {
                b.BaseStream.Seek(Stats_PartsRepaired_MemLoc, 0);    //Move to location
                Stats_PartsRepaired = b.ReadInt32();                 //Read the value

                b.BaseStream.Seek(Stats_MoneyIncomeParts_MemLoc, 0);    //Move to location
                Stats_MoneyIncomeParts = b.ReadInt32();                 //Read the value

                b.BaseStream.Seek(Stats_MoneyIncomeCars_MemLoc, 0);    //Move to location
                Stats_MoneyIncomeCars = b.ReadInt32();                 //Read the value

                b.BaseStream.Seek(Stats_CarsSold_MemLoc, 0);    //Move to location
                Stats_CarsSold = b.ReadInt32();                 //Read the value

                b.BaseStream.Seek(Stats_JobsCompleted_MemLoc, 0);    //Move to location
                Stats_JobsCompleted = b.ReadInt32();                 //Read the value

                b.BaseStream.Seek(Stats_CarsOwned_MemLoc, 0);    //Move to location
                Stats_CarsOwned = b.ReadInt32();                 //Read the value

                b.BaseStream.Seek(Stats_MoneyIncome_MemLoc, 0);    //Move to location
                Stats_MoneyIncome = b.ReadInt32();                 //Read the value

                b.BaseStream.Seek(Stats_PartsUnmounted_MemLoc, 0);    //Move to location
                Stats_PartsUnmounted = b.ReadInt32();                 //Read the value

                b.BaseStream.Seek(Stats_Bolts_MemLoc, 0);    //Move to location
                Stats_Bolts = b.ReadInt32();                 //Read the value

                b.BaseStream.Seek(bankLoan_MemLoc, 0);    //Move to location
                bankLoan_MemLoc = b.ReadInt32();                 //Read the value

                b.BaseStream.Seek(xp_MemLoc, 0);    //Move to location
                xp = b.ReadInt32();                 //Read the value

                b.BaseStream.Seek(money_MemLoc, 0);    //Move to location
                money = b.ReadInt32();                 //Read the value
            }
        }

        //Writes the car data file from the object
        public void WriteGlobalSaveFile(string path)
        {
            string fullpath = path + "\\global";
            using (BinaryWriter b = new BinaryWriter(File.Open(fullpath, FileMode.Open)))
            {
                b.BaseStream.Seek(Stats_PartsRepaired_MemLoc, 0);   //Move to location
                b.Write(Stats_PartsRepaired);                       //Write the value

                b.BaseStream.Seek(Stats_MoneyIncomeParts_MemLoc, 0);    //Move to location
                b.Write(Stats_MoneyIncomeParts);                        //Write the value

                b.BaseStream.Seek(Stats_MoneyIncomeCars_MemLoc, 0);     //Move to location
                b.Write(Stats_MoneyIncomeCars);                         //Write the value

                b.BaseStream.Seek(Stats_CarsSold_MemLoc, 0);    //Move to location
                b.Write(Stats_CarsSold);                        //Write the value

                b.BaseStream.Seek(Stats_JobsCompleted_MemLoc, 0);   //Move to location
                b.Write(Stats_JobsCompleted);                       //Write the value

                b.BaseStream.Seek(Stats_CarsOwned_MemLoc, 0);   //Move to location
                b.Write(Stats_CarsOwned);                       //Write the value

                b.BaseStream.Seek(Stats_MoneyIncome_MemLoc, 0); //Move to location
                b.Write(Stats_MoneyIncome);                     //Write the value

                b.BaseStream.Seek(Stats_PartsUnmounted_MemLoc, 0);  //Move to location
                b.Write(Stats_PartsUnmounted);                      //Write the value

                b.BaseStream.Seek(Stats_Bolts_MemLoc, 0);   //Move to location
                b.Write(Stats_Bolts);                       //Write the value

                b.BaseStream.Seek(bankLoan_MemLoc, 0);  //Move to location
                b.Write(bankLoan_MemLoc);               //Write the value

                b.BaseStream.Seek(xp_MemLoc, 0);    //Move to location
                b.Write(xp);                        //Write the value

                b.BaseStream.Seek(money_MemLoc, 0); //Move to location
                b.Write(money);                     //Write the value
            }
        }

        #region Getters and Setters
        public int _Stats_PartsRepaired
        {
            get { return Stats_PartsRepaired; }
            set { Stats_PartsRepaired = value; }
        }

        public int _Stats_MoneyIncomeParts
        {
            get { return Stats_MoneyIncomeParts; }
            set { Stats_MoneyIncomeParts = value; }
        }

        public int _Stats_MoneyIncomeCars
        {
            get { return Stats_MoneyIncomeCars; }
            set { Stats_MoneyIncomeCars = value; }
        }

        public int _Stats_CarsSold
        {
            get { return Stats_CarsSold; }
            set { Stats_CarsSold = value; }
        }

        public int _Stats_JobsCompletted
        {
            get { return Stats_JobsCompleted; }
            set { Stats_JobsCompleted = value; }
        }

        public int _Stats_CarsOwned
        {
            get { return Stats_CarsOwned; }
            set { Stats_CarsOwned = value; }
        }

        public int _Stats_MoneyIncome
        {
            get { return Stats_MoneyIncome; }
            set { Stats_MoneyIncome = value; }
        }

        public int _Stats_PartsUnmounted
        {
            get { return Stats_PartsUnmounted; }
            set { Stats_PartsUnmounted = value; }
        }

        public int _Stats_Bolts
        {
            get { return Stats_Bolts; }
            set { Stats_Bolts = value; }
        }

        public int _bankLoan
        {
            get { return bankLoan; }
            set { bankLoan = value; }
        }

        public int _xp
        {
            get { return xp; }
            set { xp = value; }
        }

        public int _money
        {
            get { return money; }
            set { money = value; }
        }
        #endregion
    }
}
