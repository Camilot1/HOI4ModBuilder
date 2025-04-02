using HOI4ModBuilder.src.dataObjects.argBlocks;
using HOI4ModBuilder.src.newParser;
using HOI4ModBuilder.src.newParser.test;
using System;

namespace HOI4ModBuilder.src.parser
{
    public class TestMain
    {
        public static void Execute()
        {
            if (true)
                return;

            Console.WriteLine("!!!");
            InfoArgsBlocksManager.Load(null);

            var file = new TestGameFile(new FileInfo("test.txt", "test.txt", true), true);
            var parser = new GameParser();

            parser.ParseFile(file);
            Console.WriteLine("!");

            //if (file.IsNeedToSave())
            file.SaveToFile(parser, new FileInfo("test2.txt", "test2.txt", true));

        }
    }
}
