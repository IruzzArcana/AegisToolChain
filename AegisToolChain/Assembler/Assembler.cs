using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AegisToolChain.Opcodes;
using AegisToolChain.Utils;
using AegisToolChain.Utils.IO;

namespace AegisToolChain.Assembler
{
    internal static class AegisAssembler
    {

        private static BiMap<long, string> Labels = new();
        private static bool isLabel(string val)
        {
            return Regex.IsMatch(val, "^[A-Za-z_][A-Za-z0-9_]*");
        }
        private static bool isLabelDef(string val)
        {
            return Regex.IsMatch(val, "^[A-Za-z_][A-Za-z0-9_]*:");
        }

        private static long GetLabel(byte[] data, string lab)
        {
            if (Labels.ContainsValue(lab))
                return Labels.GetByValue(lab);

            long addr = 0;

            using (var stream = new StreamReader(new MemoryStream(data)))
            {
                while (!stream.EndOfStream)
                {
                    string text = stream.ReadLine()!.Trim();

                    string[] tokens = Regex.Matches(text, @"(?<match>[^\s""]+)|(?<match>\""[^\""]*\"")")
                                           .Cast<Match>()
                                           .Select(m => m.Groups["match"].Value)
                                           .ToArray();

                    if (tokens.Length == 0)
                        continue;

                    string inst = tokens[0];

                    if (isLabelDef(inst))
                    {
                        if (inst == $"{lab}:")
                        {
                            Labels.Add(addr, lab);
                            return addr;
                        }
                        continue;
                    }

                    for (int i = 1; i < tokens.Length; i++)
                        inst += $" {{{i - 1}}}";

                    addr++;

                    OpcodeInfo info = new(false, inst);
                    if (Opcode.Get(info) == 0)
                    {
                        throw new Exception($"Unknown instruction: {info.Format}");
                    }

                    int expectedArgs = Opcode.GetArgCount(info);
                    if (tokens.Length - 1 < expectedArgs)
                    {
                        throw new Exception($"Not enough arguments for instruction: {info.Format}");
                    }

                    for (int i = 0; i < expectedArgs; i++)
                    {
                        if (Opcode.GetOperandTypes(info)[i] < OpcodeInfo.OperandType.String)
                            addr += (long)Opcode.GetOperandTypes(info)[i];
                        else
                        {
                            string val = tokens[i + 1];
                            string str = val.Trim('"').Replace("\\n", "\n");
                            int len = DataOutputStream.ShiftJISEncoding.GetByteCount(str);
                            addr += (long)OpcodeInfo.OperandType.Short + len;
                        }
                    }

                }
                throw new Exception($"Could not find {lab}");
            }
        }

        public static void Assemble(byte[] data, string path)
        {
            int lineCount = 0;
            using (var stream = new StreamReader(new MemoryStream(data)))
            using (var outstream = new DataOutputStream())
            {
                while (!stream.EndOfStream)
                {
                    string text = stream.ReadLine()!.Trim();
                    lineCount++;
                    /*
                     * ([^\s""]+) matches non-whitespace and non-quote characters. 
                     * ("[^""]*") matches quoted strings.
                     */
                    string[] tokens = Regex.Matches(text, @"(?<match>[^\s""]+)|(?<match>\""[^\""]*\"")")
                                           .Cast<Match>()
                                           .Select(m => m.Groups["match"].Value)
                                           .ToArray();

                    if (tokens.Length == 0)
                        continue;

                    string inst = tokens[0];

                    if (isLabelDef(inst))
                        continue;

                    for (int i = 1; i < tokens.Length; i++)
                        inst += $" {{{i - 1}}}";

                    OpcodeInfo info = new(false, inst);
                    if (Opcode.Get(info) == 0)
                    {
                        Console.WriteLine($"Unknown instruction: {info.Format} in line {lineCount}");
                        return;
                    }
                    outstream.WriteByte((sbyte)Opcode.Get(info));

                    int expectedArgs = Opcode.GetArgCount(info);
                    if (tokens.Length - 1 < expectedArgs)
                    {
                        Console.WriteLine($"Not enough arguments for instruction: {info.Format}");
                        return;
                    }
                    for (int i = 0; i < expectedArgs; i++)
                    {
                        string val = tokens[i + 1];

                        if (isLabel(val))
                        {
                            val = GetLabel(data, val).ToString();
                        }

                        switch (Opcode.GetOperandTypes(info)[i])
                        {
                            case OpcodeInfo.OperandType.Int:
                                outstream.WriteInt(int.Parse(val));
                                break;

                            case OpcodeInfo.OperandType.Short:
                                outstream.WriteShort(short.Parse(val));
                                break;

                            case OpcodeInfo.OperandType.Byte:
                                outstream.WriteByte(sbyte.Parse(val));
                                break;

                            case OpcodeInfo.OperandType.String:
                                string str = val.Trim('"').Replace("\\n", "\n");
                                int len = DataOutputStream.ShiftJISEncoding.GetByteCount(str);
                                outstream.WriteShort((short)len);
                                outstream.WriteString(str, DataOutputStream.ShiftJISEncoding);
                                break;

                            default:
                                throw new InvalidOperationException($"Unknown operand type: {Opcode.GetOperandTypes(info)[i]}");
                        }
                    }
                }
                try
                {
                    File.WriteAllBytes(path, outstream.Bytes());
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine($"Failed to write {path}: {ex.Message}");
                }
            }
        }
    }
}
