using System.Text;
using AegisToolChain.Assembler;
using AegisToolChain.Disassembler;

class AegisScript
{

    private byte[]? FileStream;

    public static void Main(string[] args)
    {
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

        var script = new AegisScript();
        if (args.Length < 3)
        {
            Console.WriteLine($"""
                USAGE: {Path.GetFileName(Environment.ProcessPath)} <filename> <mode> <output> [--pretty-print]

                <mode>:
                    d   disassemble
                    e   assemble

                Options:
                    --pretty-print   Format the disassembled output nicely

                Example:
                    {Path.GetFileName(Environment.ProcessPath)} script.scb d script.txt --pretty-print

                """);
            return;
        }

        int mode;

        switch (args[1])
        {
            case "d":
                mode = 0; break;
            case "e":
                mode = 1; break;
            default:
                Console.WriteLine($"Invalid mode: {args[1]}");
                return;
        }

        bool prettyPrint = args.Length > 3 && (args[3] == "-p" || args[3] == "--pretty-print");

        try
        {
            script.FileStream = File.ReadAllBytes(args[0]);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to open {args[0]}: {ex.Message}");
            return;
        }

        if (mode == 0)
            AegisDisassembler.Disassemble(script.FileStream, args[2], prettyPrint);
        else
            AegisAssembler.Assemble(script.FileStream, args[2]);
    }
}