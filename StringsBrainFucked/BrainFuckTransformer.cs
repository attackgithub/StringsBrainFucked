using System.Text;

namespace StringsBrainFucked {
    //Taken from https://codegolf.stackexchange.com/questions/5418/brainf-golfer/5440#5440
    static class BrainFuckTransformer {
        static readonly string[][] _g;

        internal static string TransformString(string original) {
            var builder = new StringBuilder();
            var lastc = 0;
            foreach (var c in original) {
                var a = _g[lastc][c];
                var b = _g[0][c];
                if (a.Length <= b.Length)
                    builder.Append(a);
                else builder.Append(">" + b);

                builder.Append(".");
                lastc = c;
            }

            return builder.ToString();
        }

        static BrainFuckTransformer() {
            _g = new string[256][];
            for (var i = 0; i < 256; i++)
                _g[i] = new string[256];

            for (var x = 0; x < 256; x++) {
                for (var y = 0; y < 256; y++) {
                    var delta = y - x;
                    if (delta > 128) delta -= 256;
                    if (delta < -128) delta += 256;

                    if (delta >= 0)
                        _g[x][y] = new string('+', delta);
                    else _g[x][y] = new string('-', -delta);
                }
            }

            bool iter = true;
            while (iter) {
                iter = false;

                for (var x = 0; x < 256; x++) {
                    for (var n = 1; n < 40; n++) {
                        for (var d = 1; d < 40; d++) {
                            var j = x;
                            var y = 0;

                            for (var i = 0; i < 256; i++) {
                                if (j == 0) break;
                                j = (j - d + 256) & 255;
                                y = (y + n) & 255;
                            }

                            if (j == 0) {
                                var s = "[" + new string('-', d) + ">" + new string('+', n) + "<]>";
                                if (s.Length < _g[x][y].Length) {
                                    _g[x][y] = s;
                                    iter = true;
                                }
                            }

                            j = x;
                            y = 0;
                            for (var i = 0; i < 256; i++) {
                                if (j == 0) break;
                                j = (j + d) & 255;
                                y = (y - n + 256) & 255;
                            }

                            if (j == 0) {
                                var s = "[" + new string('+', d) + ">" + new string('-', n) + "<]>";
                                if (s.Length < _g[x][y].Length) {
                                    _g[x][y] = s;
                                    iter = true;
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
