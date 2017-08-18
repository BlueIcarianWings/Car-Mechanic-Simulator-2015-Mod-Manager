using System;
using System.IO;    //File read / write
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMS2015ModManager
{
    class BinarySerialFileReader
    {
        public bool LoadGarageSaveFile(string path)
        {
            bool RetVal = false;    //Setup the return value, default false
            string fullpath = path + "\\garage";    //Setup the full path/file name

            if (File.Exists(fullpath))  //Check if the file exists
            {
                RetVal = true;
                using (BinaryReader b = new BinaryReader(File.Open(fullpath, FileMode.Open)))
                {
                    //b.BaseStream.Seek(FastUnbolting_MemLoc, 0);    //Move to location
                    //FastUnbolting = b.ReadBoolean();                 //Read the value

                    //Data chunks start with 0x7E
                    //Data chunks end with 0x7B

                    //The first object in the record is the string consisting of the internal varriable name of the object encoded by this record.
                    //Strings are encoded as LENGTH (one byte) and value. which is of LENGTH bytes.

                    //The next field is a x86 encoded 32 bit integer denoting the length of the data value.
                    //All data values begin with byte 0xFF and end with 0x7B(the same byte that ends the record). <- data values or chunks?
                    //  The parser should read the specified number of bytes from the stream and confirm that it begins with 0xFF and ends with 0x7B.
                    //  The data packet may then be unwrapped and sent to the data parser. 

                    //The data parser seems to operate based on magic number. 
                    //The integer values(decoded reverse byte x86 integers) are:
                    //0x6E3ED76B->Probably unsigned integer, 32 bits.
                    //0xAD4D7C9C->Boolean(one byte true false)
                    //0xE2A80A56->probably signed integer, 32 bit.
                    //0xFDE9F1FE->String in format length: value, presumably with maximum length of 255

                    //Also see my notes on how arrays work
                    //http://steamcommunity.com/app/320300/discussions/1/135508662492191192/

                    //something that searches for the first 7E, grabs the string length and string
                }
            }

            return RetVal;
        }
    }
}
