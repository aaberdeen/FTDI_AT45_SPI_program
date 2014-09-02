using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WindowsFormsApplication1
{
    public static class Const_AT45
    {
        //ReadCommands
        public static byte mainMemoryPageRead { get { return 0xd2; } }
        public static byte continuousArrayReadLegacy { get { return 0xe8; } }
        public static byte continuousArrayReadLowFrequ { get { return 0x03; } }
        public static byte continuousArrayReadHighFrequ { get { return 0x0b; } }
        public static byte buffer1ReadLowFrequ { get { return 0xd1; } }
        public static byte buffer2ReadLowFrequ { get { return 0xd3; } }
        public static byte buffer1Read { get { return 0xd4; } }
        public static byte buffer2Read { get { return 0xd6; } }

        //Program and Erase Commands
        public static byte buffer1Write { get { return 0x84; } }
        public static byte buffer2Write { get { return 0x87; } }
        public static byte buffer1toMainMemoryPageProgramBuiltInErase { get { return 0x83; } }
        public static byte buffer2toMainMemoryPageProgramBuiltInErase { get { return 0x86; } }
        public static byte buffer1toMainMemoryPageProgram { get { return 0x88; } }
        public static byte buffer2toMainMemoryPageProgram { get { return 0x89; } }
        public static byte pageErase { get { return 0x81; } }
        public static byte blockErase { get { return 0x50; } }
        public static byte sectorErase { get { return 0x7c; } }
        public static UInt32 chipErase { get { return 0xc794809a; } }
        public static byte mainMemoryPageProgramThroughBuffer1 { get { return 0x82; } }
        public static byte mainMemoryPageProgramThroughBuffer2 { get { return 0x85; } }

        //Protectr and Security Commands
        public static UInt32 enableSectorProtection { get { return 0x3d2a7fa9; } }
        public static UInt32 disableSectorProtection { get { return 0x3d2a7f9a; } }
        public static UInt32 eraseSectorProtectionRegister { get { return 0x3d2a7fcf; } }
        public static UInt32 programSectorProtectionRegister { get { return 0x3d2a7ffc; } }
        public static byte readSectorProtectionRegister { get { return 0x32; } }
        public static UInt32 sectorLockdown { get { return 0x3d2a7f30; } }

        public static byte readSectorLockdownRegister { get { return 0x35; } }
        public static UInt32 programSecurityRegister { get { return 0x9b000000; } }
        public static byte readSecurityRegister { get { return 0x77; } }

        // Additional Commands
        public static byte mainMemoryPagetoBuffer1Transfer { get { return 0x53; } }
        public static byte mainMemoryPagetoBUffer2Transfer { get { return 0x55; } }
        public static byte mainMemoryPagetoBuffer1Compare { get { return 0x60; } }
        public static byte mainMemoryPagetoBuffer2Compare { get { return 0x61; } }
        public static byte autoPageRewriteThroughBuffer1 { get { return 0x58; } }
        public static byte autoPageRewriteThroughBuffer2 { get { return 0x59; } }
        public static byte deepPowerDown { get { return 0xb9; } }
        public static byte resumeFromDeepPowerDown { get { return 0xab; } }
        public static byte statusRegisterRead { get { return 0xd7; } }
        public static byte manufacturerAndDeviceIDRead { get { return 0x9f; } }

        //Legacy Commands NOT for new Designs
        public static byte buffer1ReadLegacyNFND { get { return 0x54; } }
        public static byte buffer2ReadLegacyNFND { get { return 0x56; } }
        public static byte mainMemoryPagereadLegacyNFND { get { return 0x52; } }
        public static byte continuousArrayReadLegacyNFND { get { return 0x68; } }
        public static byte statusRegisterReadLegacyNFND { get { return 0x57; } }
    }
}
