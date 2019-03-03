using System;
using System.IO;
using System.Text;

namespace ScraperTest
{
    public class OutputCapture : TextWriter
    {
        private readonly TextWriter _stdOutWriter;
        public TextWriter Captured { get; }
        public override Encoding Encoding => Encoding.ASCII;

        public OutputCapture()
        {
            _stdOutWriter = Console.Out;
            Console.SetOut(this);
            Captured = new StringWriter();
        }

        public override void Write(string output)
        {
            // Capture the output and also send it to StdOut
            Captured.Write(output);
            _stdOutWriter.Write(output);
        }

        public override void WriteLine(string output)
        {
            // Capture the output and also send it to StdOut
            Captured.WriteLine(output);
            _stdOutWriter.WriteLine(output);
        }
    }
}