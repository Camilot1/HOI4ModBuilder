using HOI4ModBuilder.src.newParser;
using HOI4ModBuilder.src.newParser.test;
using System;

namespace HOI4ModBuilder.src.parser
{
    public class TestMain
    {
        public static void Execute()
        {
            Console.WriteLine("!!!");

            var file = new TestGameFile(new FileInfo("test.txt", "test.txt", true), true);
            var parser = new GameParser();

            parser.ParseFile(file);
            Console.WriteLine("!");

        }
    }
}
