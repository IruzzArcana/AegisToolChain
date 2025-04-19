using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using AegisToolChain.Utils;

namespace AegisToolChain.Opcodes
{
    internal class Opcode
    {
        public static readonly BiMap<int, OpcodeInfo> Instructions = new()
        {
            [1] = new("JZ {0}", OpcodeInfo.OperandType.Int),
            [2] = new("JUMP {0}", OpcodeInfo.OperandType.Int),
            [3] = new("CALL_INT {0}", OpcodeInfo.OperandType.Int),
            [4] = new("RET"),
            [5] = new("CALL_SHORT {0}", OpcodeInfo.OperandType.Short),
            [10] = new("ADJUST_STACK {0}", OpcodeInfo.OperandType.Byte),
            [11] = new("PUSH_INT {0}", OpcodeInfo.OperandType.Int),
            [12] = new("PUSH_STR {0}", OpcodeInfo.OperandType.String),
            [15] = new("STORE_SHORT {0}", OpcodeInfo.OperandType.Short),
            [16] = new("LOAD_SHORT {0}", OpcodeInfo.OperandType.Short),
            [17] = new("STORE_GLOBAL {0}", OpcodeInfo.OperandType.Short),
            [18] = new("LOAD_GLOBAL {0}", OpcodeInfo.OperandType.Short),
            [20] = new("ADD"),
            [21] = new("SUB"),
            [22] = new("MUL"),
            [23] = new("DIV"),
            [24] = new("CMP_EQ"),
            [25] = new("CMP_NEQ"),
            [26] = new("CMP_LT"),
            [27] = new("CMP_GT"),
            [28] = new("CMP_LE"),
            [29] = new("CMP_GE"),
            [30] = new("PUSH_NULL"),
            [31] = new("PUSH_BYTE {0}", OpcodeInfo.OperandType.Byte),
            [32] = new("PUSH_SHORT {0}", OpcodeInfo.OperandType.Short),
        };

        public static OpcodeInfo? Get(int opcode)
        {
            Instructions.TryGetByKey(opcode, out var info);
            return info;
        }

        public static int Get(OpcodeInfo info)
        {
            Instructions.TryGetByValue(info, out var opcode);
            return opcode;
        }

        public static OpcodeInfo.OperandType[] GetOperandTypes(OpcodeInfo info)
        {
            Instructions.TryGetByValue(info, out var opcode);
            return Instructions.GetByKey(opcode).OperandTypes;
        }

        public static int GetArgCount(OpcodeInfo info)
        {
            Instructions.TryGetByValue(info, out var opcode);
            return Instructions.GetByKey(opcode).ArgCount;
        }
    }
}
