using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AegisToolChain.Opcodes;
using AegisToolChain.Utils;
using AegisToolChain.Utils.IO;

namespace AegisToolChain.Disassembler
{
    internal static class AegisDisassembler
    {
        private static BiMap<long, string> Labels = new();

        private static bool isLabelDef(string val)
        {
            return Regex.IsMatch(val, "^[A-Za-z_][A-Za-z0-9_]*:");
        }

        private static void InsertLabels(byte[] data, string path, bool prettyPrint)
        {
            var lines = File.ReadAllLines(path).ToList();

            using (var stream = new DataInputStream(data))
            {
                foreach (var kvp in Labels.ForwardMap)
                {
                    stream.Seek(0, SeekOrigin.Begin);

                    int line_count = prettyPrint ? 2 : 0;

                    while ((stream.Position() !=  kvp.Key) && !stream.IsEOF)
                    {
                        line_count++;

                        if (isLabelDef(lines[line_count]))
                            continue;

                        int opcode = stream.ReadByte();

                        if (!Opcode.Instructions.TryGetByKey(opcode, out var info))
                        {
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
                    }

                    if (stream.Position() != kvp.Key)
                    {
                        throw new InvalidOperationException($"Failed to find jump target 0x{kvp.Key:X}");
                    }

                    lines.Insert(line_count, $"{kvp.Value}:");

                    File.WriteAllLines(path, lines);
                }
            }
        }

        public static void Disassemble(byte[] data, string path, bool prettyPrint = false)
        {
            try
            {
                using (var stream = new DataInputStream(data))
                using (var fs = new FileStream(path, FileMode.Create, FileAccess.Write))
                using (var writer = new StreamWriter(fs))
                {
                    if (prettyPrint)
                    {
                        writer.WriteLine("------------------------- AegisToolChain v0.1 -------------------------");
                        writer.WriteLine("{0,-11} {1, -11} {2}", "Label", "Address", "Instruction");
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
                            string paddingl = new string(' ', 12);
                            string padding = new string(' ', addressWidth - address.Length + paddingAfterAddress);

                            writer.Write($"{paddingl}{address}{padding}");
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
                            if (info.IsJump && Convert.ToInt64(args[0]) >= 0)
                            {
                                Labels.Add((int)args[0], $"LAB_{args[0]}");
                                args[0] = $"LAB_{args[0]}";
                            }
                        }

                        string line = info.GetFormatted(args);
                        writer.WriteLine(line);
                    }
                }
                InsertLabels(data, path, prettyPrint);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to open file: {ex.Message}");
            }
        }
    }
}
