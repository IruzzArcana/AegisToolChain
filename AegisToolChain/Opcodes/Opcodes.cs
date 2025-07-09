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
            [1] = new(true, "JZ {0}", OpcodeInfo.OperandType.Int),
            [2] = new(true, "JUMP {0}", OpcodeInfo.OperandType.Int),
            [3] = new(true, "CALL_INT {0}", OpcodeInfo.OperandType.Int),
            [4] = new(false, "RET"),
            [5] = new(true, "CALL_SHORT {0}", OpcodeInfo.OperandType.Short),
            [10] = new(false, "ADJUST_STACK {0}", OpcodeInfo.OperandType.Byte),
            [11] = new(false, "PUSH_INT {0}", OpcodeInfo.OperandType.Int),
            [12] = new(false, "PUSH_STR {0}", OpcodeInfo.OperandType.String),
            [15] = new(false, "STORE_LOCAL {0}", OpcodeInfo.OperandType.Short),
            [16] = new(false, "LOAD_LOCAL {0}", OpcodeInfo.OperandType.Short),
            [17] = new(false, "STORE_GLOBAL {0}", OpcodeInfo.OperandType.Short),
            [18] = new(false, "LOAD_GLOBAL {0}", OpcodeInfo.OperandType.Short),
            [20] = new(false, "ADD"),
            [21] = new(false, "SUB"),
            [22] = new(false, "MUL"),
            [23] = new(false, "DIV"),
            [24] = new(false, "CMP_EQ"),
            [25] = new(false, "CMP_NEQ"),
            [26] = new(false, "CMP_LT"),
            [27] = new(false, "CMP_GT"),
            [28] = new(false, "CMP_LE"),
            [29] = new(false, "CMP_GE"),
            [30] = new(false, "PUSH_NULL"),
            [31] = new(false, "PUSH_BYTE {0}", OpcodeInfo.OperandType.Byte),
            [32] = new(false, "PUSH_SHORT {0}", OpcodeInfo.OperandType.Short),
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

        public static bool IsJump(OpcodeInfo info)
        {
            Instructions.TryGetByValue(info, out var opcode);
            return Instructions.GetByKey(opcode).IsJump;
        }
    }
}
