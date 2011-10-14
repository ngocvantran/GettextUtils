using System;
using System.IO;

namespace Resx2Po
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            new DataProcessor(Directory
                .GetCurrentDirectory(), "en", "en")
                .Process();
        }
    }
}