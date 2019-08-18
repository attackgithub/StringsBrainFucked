using CommandLine;

namespace StringsBrainFucked {
    public class Options {
        [Value(0, Required = true, HelpText = "Path to the assembly you wanna brainfuck...duh")]
        public string AssemblyPath { get; set; }

        [Option('o', "out", HelpText = "Output path")]
        public string OutPath { get; set; }
    }
}
