using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AegisToolChain.Opcodes;
using AegisToolChain.Utils.IO;

namespace AegisToolChain.Assembler
{
    internal static class AegisAssembler
    {
        public static void Assemble(byte[] data, string path)
        {
            var stream = new StreamReader(new MemoryStream(data));
            using (var outstream = new DataOutputStream())
            {
                while (!stream.EndOfStream)
                {
                    string text = stream.ReadLine()!;
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

                    if (tokens.Length > 1)
                    {
                        for (int i = 1; i < tokens.Length; i++)
                            inst += $" {{{i - 1}}}";

                        OpcodeInfo info = new(inst);
                        if (Opcode.Get(info) == 0)
                        {
                            Console.WriteLine($"Unknown instruction: {info.Format}");
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
                    else
                    {
                        OpcodeInfo info = new(inst);
                        outstream.WriteByte((sbyte)Opcode.Get(info));
                    }
                }
                File.WriteAllBytes(path, outstream.Bytes());
            }
        }
    }
}
