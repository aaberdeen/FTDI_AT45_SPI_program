using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WindowsFormsApplication1
{
    public static class Const_FTDI
    {
        public static int OPCODE_MSB_DO_BYTES_POS { get { return 0x10; } }
        public static int OPCODE_MSB_DO_BYTES_NEG { get { return 0x11; } }
        public static int OPCODE_MSB_DO_BITS_POS { get { return 0x12; } }
        public static int OPCODE_MSB_DO_BITS_NEG { get { return 0x13; } }
        public static int OPCODE_LSB_DO_BYTES_POS { get { return 0x18; } }
        public static int OPCODE_LSB_DO_BYTES_NEG { get { return 0x19; } }
        public static int OPCODE_LSB_DO_BITS_POS { get { return 0x1A; } }
        public static int OPCODE_LSB_DO_BITS_NEG { get { return 0x1B; } }

        public static int OPCODE_MSB_DI_BYTES_POS { get { return 0x20; } }
        public static int OPCODE_MSB_DI_BYTES_NEG { get { return 0x24; } }
        public static int OPCODE_MSB_DI_BITS_POS { get { return 0x22; } }
        public static int OPCODE_MSB_DI_BITS_NEG { get { return 0x26; } }
        public static int OPCODE_LSB_DI_BYTES_POS { get { return 0x28; } }
        public static int OPCODE_LSB_DI_BYTES_NEG { get { return 0x2c; } }
        public static int OPCODE_LSB_DI_BITS_POS { get { return 0x2a; } }
        public static int OPCODE_LSB_DI_BITS_NEG { get { return 0x2e; } }

        public static int OPCODE_MSB_DIO_BYTES_POS { get { return 0x30; } }
        public static int OPCODE_MSB_DIO_BYTES_POSNEG { get { return 0x31; } }
        public static int OPCODE_MSB_DIO_BITS_POS { get { return 0x32; } }
        public static int OPCODE_MSB_DIO_BITS_POSNEG { get { return 0x33; } }
        public static int OPCODE_MSB_DIO_BYTES_NEGPOS { get { return 0x34; } }
        public static int OPCODE_MSB_DIO_BYTES_NEG { get { return 0x35; } }
        public static int OPCODE_MSB_DIO_BITS_NEGPOS { get { return 0x36; } }
        public static int OPCODE_MSB_DIO_BITS_NEG { get { return 0x37; } }
        public static int OPCODE_LSB_DIO_BYTES_POS { get { return 0x38; } }
        public static int OPCODE_LSB_DIO_BYTES_POSNEG { get { return 0x39; } }
        public static int OPCODE_LSB_DIO_BITS_POS { get { return 0x3a; } }
        public static int OPCODE_LSB_DIO_BITS_POSNEG { get { return 0x3b; } }
        public static int OPCODE_LSB_DIO_BYTES_NEGPOS { get { return 0x3c; } }
        public static int OPCODE_LSB_DIO_BYTES_NEG { get { return 0x3d; } }
        public static int OPCODE_LSB_DIO_BITS_NEGPOS { get { return 0x3e; } }
        public static int OPCODE_LSB_DIO_BITS_NEG { get { return 0x3f; } }

        public static int OPCODE_TMS_DO_POS { get { return 0x4a; } }
        public static int OPCODE_TMS_DO_NEG { get { return 0x4b; } }
        public static int OPCODE_TMS_DIO_POS { get { return 0x6a; } }
        public static int OPCODE_TMS_DIO_POSNEG { get { return 0x6b; } }
        public static int OPCODE_TMS_DIO_NEGPOS { get { return 0x6e; } }
        public static int OPCODE_TMS_DIO_NEG { get { return 0x6f; } }

        public static int OPCODE_SET_LOBYTE_AND_DIR { get { return 0x80; } }
        public static int OPCODE_SET_HIBYTE_AND_DIR { get { return 0x82; } }
        public static int OPCODE_GET_LOBYTE { get { return 0x81; } }
        public static int OPCODE_GET_HIBYTE { get { return 0x83; } }

        public static int OPCODE_ENABLE_LOOPBACK { get { return 0x84; } }
        public static int OPCODE_DISABLE_LOOPBACK { get { return 0x85; } }

        public static int OPCODE_SET_CLOCK_DIVISOR { get { return 0x86; } }
        public static int OPCODE_SEND_IMMEDIATE { get { return 0x87; } }
        public static int OPCODE_WAIT_ON_IO_HI { get { return 0x88; } }
        public static int OPCODE_WAIT_ON_IO_LO { get { return 0x89; } }

        public static int OPCODE_CPU_DI_ADDR8 { get { return 0x90; } }
        public static int OPCODE_CPU_DI_ADDR16 { get { return 0x91; } }
        public static int OPCODE_CPU_DO_ADDR8 { get { return 0x92; } }
        public static int OPCODE_CPU_DO_ADDR16 { get { return 0x93; } }
    }
}
