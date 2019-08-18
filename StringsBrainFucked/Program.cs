using System;
using System.IO;
using System.Linq;
using CommandLine;
using dnlib.DotNet;
using dnlib.DotNet.Emit;
using RuntimeFucker;

namespace StringsBrainFucked {
    class MainClass {
        static void Main(string[] args) => Parser.Default.ParseArguments<Options>(args).WithParsed(RealMain);

        static int _counter;
        static void RealMain(Options opts) {
            Console.WriteLine($"Brainfucking '{opts.AssemblyPath}' ...");
            opts.OutPath = opts.OutPath ?? Path.GetFileNameWithoutExtension(opts.AssemblyPath) + "_brainfucked" + Path.GetExtension(opts.AssemblyPath);
            try {
                ProcessModule(ModuleDefMD.Load(opts.AssemblyPath));
                _mod.Write(opts.OutPath);

                Console.WriteLine($"Brainfucked strings: {_counter}");
                Console.WriteLine($"All done, brainfucked assembly: '{opts.OutPath}'");
            } catch (Exception ex) {
                Console.WriteLine($"An exception occured:\n{ex}");
            }
        }

        static void ProcessModule(ModuleDefMD mod) {
            _mod = mod;
            InitDecryptMethod();

            foreach (var type in mod.Types)
                VisitType(type);
        }

        static void VisitType(TypeDef type) {
            if (type == _decrypt.DeclaringType)
                return;

            foreach (var nest in type.NestedTypes)
                VisitType(nest);

            if (!type.HasMethods)
                return;

            foreach (var method in type.Methods)
                VisitMethod(method);
        }

        static ModuleDefMD _mod;

        static IMethod _decrypt;
        static void InitDecryptMethod() {
            var runtime = ModuleDefMD.Load(typeof(Brain).Module);
            var type = runtime.Types.Single(t => t.Name == "Brain");
            runtime.Types.Remove(type);
            _mod.Types.Add(type);
            type.Namespace = string.Empty;

            _decrypt = type.Methods.Single(m => m.Name == "Fuck");
        }

        static void VisitMethod(MethodDef method) {
            if (!method.HasBody || !method.Body.HasInstructions)
                return;

            var body = method.Body.Instructions;
            for (var i = 0; i < body.Count; i++) {
                var current = body[i];
                if (current.OpCode != OpCodes.Ldstr)
                    continue;

                var op = (string)current.Operand;
                if (op.Length < 1) //Sadly can't brainfuck an empty string...
                    continue;

                current.Operand = BrainFuckTransformer.TransformString(op);
                body.Insert(i + 1, Instruction.Create(OpCodes.Call, _decrypt));
                _counter++;
            }

            method.Body.SimplifyBranches();
            method.Body.OptimizeMacros();
        }
    }
}
