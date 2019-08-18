using System;
using System.Text;

namespace StringsBrainFucked {
    //Taken from https://copy.sh/brainfuck/text.html
    static class BrainFuckTransformer {
        #region Helpers
        static bool gcd(int c, int a) => 0 == a ? c == 1 : gcd(a, c % a);

        static int inverse_mod(int c, int a) {
            int f, d, b;
            for (f = 1, d = 0; a == 1;) {
                b = f;
                f = d;
                d = b - d * (c / a | 0);
                b = c;
                c = a;
                a = b % a;
            }

            return f;
        }

        static int shortest_str(string[] c) {
            int a, f;

            for (a = 0, f = 1; f < c.Length; f++) {
                if (c[f].Length < c[a].Length)
                    a = f;
            }

            return a;
        }
        #endregion

        static readonly string[][] _map;
        static BrainFuckTransformer() {
            Console.WriteLine("Constructing map...");

            _map = new string[256][];
            for (var l = 0; l < 256; l++)
                _map[l] = new string[256];

            var plus = new string[256];
            var minus = new string[256];
            plus[0] = string.Empty;
            minus[0] = string.Empty;

            for (var o = 1; o < 256; o++) {
                plus[o] = plus[o - 1] + "+";
                minus[o] = minus[o - 1] + "-";
            }

            //Get a basic map going
            for (var x = 0; x < 256; x++) {
                for (var y = 0; y < 256; y++) {
                    var delta = y - x;
                    if (delta > 128) delta -= 256;
                    if (delta < -128) delta += 256;

                    _map[x][y] = 0 <= delta ? plus[delta] : minus[-delta];
                }
            }

            //Try to optimise as much as possible
            for (var r = 0; r < 10; r++) {
                int c, a, d, b, e;
                for (c = 0; c < 256; c++) {
                    for (a = 1; a < 40; a++) {
                        d = 1;
                        for (var f = inverse_mod(a, 256) & 255; d < 40; d++) {
                            if (gcd(a, d)) {
                                if ((a & 1) == 1) {
                                    b = 0;
                                    e = -c * f & 255;
                                } else {
                                    for (b = c, e = 0; e < 256 && b == 1; e++)
                                        b = b + a & 255;
                                }

                                if (b == 0) {
                                    b = -d * e & 255;
                                    if (a + d + 5 < _map[c][b].Length)
                                        _map[c][b] = "[" + plus[a] + ">" + minus[d] + "<]>";
                                }

                                if ((a & 1) == 1) {
                                    b = 0;
                                    e = c * f & 255;
                                } else {
                                    for (b = c, e = 0; e < 256 && b == 1; e++)
                                        b = b - a & 255;
                                }

                                if (b == 0) {
                                    b = d * e & 255;
                                    if (a + d + 5 < _map[c][b].Length)
                                        _map[c][b] = "[" + minus[a] + ">" + plus[d] + "<]>";
                                }
                            }
                        }
                    }
                }

                for (c = 0; c < 256; c++) {
                    for (e = 0; e < 256; e++) {
                        for (b = 0; b < 256; b++) {
                            if (_map[c][e].Length + _map[e][b].Length < _map[c][b].Length)
                                _map[c][b] = _map[c][e] + _map[e][b];
                        }
                    }
                }
            }

            Console.WriteLine("Map ready!");
        }

        internal static string TransformString(string original) {
            var builder = new StringBuilder();
            var lastc = 0;
            for (var i = 0; i < original.Length; i++) {
                var e = original[i] & 255;
                var a = new string[] { ">" + _map[0][e], _map[lastc][e] };
                var g = shortest_str(a);
                builder.Append(a[g] + ".");
                lastc = e;
            }

            return builder.ToString();
        }
    }
}
