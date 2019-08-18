namespace RuntimeFucker {
    public static class Brain {
        internal static string Fuck(string bfProgram) => Brainfuck.Start(bfProgram);

        //Taken from https://gist.github.com/gszauer/f1a2e0beef15a73ac107
        class Brainfuck {
            string _out;
            internal static string Start(string bfProgram) {
                var bf = new Brainfuck(bfProgram);
                bf.Fuck();

                return bf._out;
            }

            internal Brainfuck(string program) => _program = program;

            string _program;
            int _ptr;
            readonly byte[] _memory = new byte[5096];
            internal void Fuck() {
                for (var i = 0; i < _program.Length; i++) {
                    switch (_program[i]) {
                        case '.':
                            _out += (char)_memory[_ptr];
                            break;
                        case '<':
                            if (_ptr == 0)
                                _ptr = _memory.Length - 1;
                            else _ptr--;
                            break;
                        case '>':
                            if (_ptr == _memory.Length - 1)
                                _ptr = 0;
                            else _ptr++;
                            break;
                        case '+':
                            unchecked { _memory[_ptr]++; }
                            break;
                        case '-':
                            unchecked { _memory[_ptr]--; }
                            break;
                        case '[':
                            if (_memory[_ptr] == 0) {
                                var skip = 0;
                                var ptr = i + 1;
                                while (_program[ptr] != ']' || skip > 0) {
                                    if (_program[ptr] == '[')
                                        skip++;
                                    else if (_program[ptr] == ']')
                                        skip--;

                                    ptr++;
                                    i = ptr;
                                }
                            }
                            break;
                        case ']':
                            if (_memory[_ptr] != 0) {
                                var skip = 0;
                                var ptr = i - 1;
                                while (_program[ptr] != '[' || skip > 0) {
                                    if (_program[ptr] == ']')
                                        skip++;
                                    else if (_program[ptr] == '[')
                                        skip--;

                                    ptr--;
                                    i = ptr;
                                }
                            }
                            break;
                    }
                }
            }
        }
    }
}
