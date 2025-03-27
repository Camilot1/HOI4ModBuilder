using HOI4ModBuilder.src.dataObjects.argBlocks;
using HOI4ModBuilder.src.parser.objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HOI4ModBuilder.src.parser.test
{
    public class TestGameFileOld : GameFileOld
    {
        public readonly GameParameterOld<TestObjectOld> TestObject = new GameParameterOld<TestObjectOld>();

        private static readonly Dictionary<string, Func<object, object>> STATIC_ADAPTER = new Dictionary<string, Func<object, object>>
        {
            { "testObject", o => ((TestGameFileOld)o).TestObject }
        };
        public override Dictionary<string, Func<object, object>> GetStaticAdapter() => STATIC_ADAPTER;

        public TestGameFileOld() : base(new FileInfo("test.txt", "test.txt", true), true, STATIC_ADAPTER)
        {
        }
    }
}
