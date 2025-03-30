using System.Collections.Generic;

namespace HOI4ModBuilder.src.newParser.structs
{
    public struct SaveAdapterParameter
    {
        public string Name { get; private set; }
        public bool IsForceInline { get; private set; }
        public List<SaveAdapterParameter> InnerParameters { get; private set; }

        public SaveAdapterParameter(string name, bool isForceInline)
        {
            Name = name;
            IsForceInline = isForceInline;
            InnerParameters = new List<SaveAdapterParameter>();
        }
    }
}
