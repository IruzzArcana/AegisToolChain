using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AegisToolChain.Opcodes
{
    internal class OpcodeInfo : IEquatable<OpcodeInfo>
    {
        public enum OperandType
        {
            Int,
            Short,
            Byte,
            String
        }
        public string Format { get; }
        public OperandType[] OperandTypes { get; }

        public OpcodeInfo(string format, params OperandType[] operandTypes)
        {
            Format = format;
            OperandTypes = operandTypes;
        }

        public string GetFormatted(params object[] args)
        {
            return string.Format(Format, args);
        }

        public int ArgCount => OperandTypes.Length;


        public bool Equals(OpcodeInfo? other)
        {
            return other is not null &&
                   Format == other.Format;
        }

        public override bool Equals(object? obj) => Equals(obj as OpcodeInfo);

        public override int GetHashCode() => Format.GetHashCode();
    }
}
