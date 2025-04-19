# AegisToolChain
A WIP scb script disassembler written in C#

# Features
- Dissassemble *.scb files into human readable pseudo-assembly
- Reassembles pseudo-assembly back into machine code
- Supports pretty-printing for improved readability

# Usage
```
AegisToolChain.exe <filename> <mode> <output> [--pretty-print]

<mode>:
    d   disassemble
    e   assemble

Options:
    --pretty-print   Format the disassembled output nicely

Example:
    AegisToolChain.exe script.scb d script.txt --pretty-print
```
# Download
[Download here](https://github.com/IruzzArcana/AegisToolChain/releases/latest)