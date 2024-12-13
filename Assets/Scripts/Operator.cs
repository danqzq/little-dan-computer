namespace Danqzq
{
    public enum Operator : byte
    {
        HLT = 0x00,
        ADD = 0x01,
        SUB = 0x02,
        MUL = 0x03,
        DIV = 0x04,
        MOD = 0x05,
        LDA = 0x06,
        LDD = 0x07,
        STA = 0x08,
        JMP = 0x09,
        JEQ = 0x0A,
        JNE = 0x0B,
        JGT = 0x0C,
        JLT = 0x0D,
        INP = 0x0E,
        OUT = 0x0F,
        OTC = 0x10,
        CLR = 0x11
    }

    public static class PostOperator
    {
        public const string DAT = "DAT";
        public const string FN = "FN";
    }
}