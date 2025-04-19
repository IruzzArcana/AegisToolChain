using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AegisToolChain.Opcodes;
using AegisToolChain.Utils.IO;

namespace AegisToolChain.Disassembler
{
    internal static class AegisDisassembler
    {
        public static void Disassemble(byte[] data, string path, bool prettyPrint = false)
        {
            try
            {
                using (var stream = new DataInputStream(data))
                using (var writer = new StreamWriter(path, false))
                {
                    if (prettyPrint)
                    {
                        writer.WriteLine("------------------------- AegisToolChain v0.1 -------------------------");
                        writer.WriteLine("{0,-11} {1}", "Address", "Instruction");
                    }
                    while (!stream.IsEOF)
                    {
                        if (prettyPrint)
                        {
                            long pos = stream.Position();
                            long maxAddress = stream.Length();

                            int addressWidth = $"0x{maxAddress:X}".Length;
                            int paddingAfterAddress = 6;

                            string address = $"0x{pos:X}";
                            string padding = new string(' ', addressWidth - address.Length + paddingAfterAddress);

                            writer.Write($"{address}{padding}");
                        }

                        int opcode = stream.ReadByte();

                        if (!Opcode.Instructions.TryGetByKey(opcode, out var info))
                        {
                            writer.WriteLine($"UNKNOWN_BYTE_CODE {opcode}");
                            continue;
                        }

                        object[] args = new object[info.ArgCount];

                        for (int i = 0; i < info.ArgCount; i++)
                        {
                            switch (info.OperandTypes[i])
                            {
                                case OpcodeInfo.OperandType.Int:
                                    args[i] = stream.ReadInt();
                                    break;
                                case OpcodeInfo.OperandType.Short:
                                    args[i] = stream.ReadShort();
                                    break;
                                case OpcodeInfo.OperandType.Byte:
                                    args[i] = stream.ReadByte();
                                    break;
                                case OpcodeInfo.OperandType.String:
                                    short len = stream.ReadShort();
                                    args[i] = $"\"{stream.ReadString(len, DataInputStream.ShiftJISEncoding).Replace("\n", "\\n")}\"";
                                    break;
                                default:
                                    throw new InvalidOperationException($"Unknown operand type: {info.OperandTypes[i]}");
                            }
                        }
                        string line = info.GetFormatted(args);
                        writer.WriteLine(line);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to open file: {ex.Message}");
            }
        }
    }
}
