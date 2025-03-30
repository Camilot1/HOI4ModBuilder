using HOI4ModBuilder.src.newParser.structs;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace HOI4ModBuilder.src.newParser.objects
{
    public class SaveAdapter
    {
        private readonly Type _type;
        private List<SaveAdapterParameter> _parameters;
        public List<SaveAdapterParameter> Parameters => _parameters;

        public SaveAdapter(Type type)
        {
            _type = type;
            _parameters = new List<SaveAdapterParameter>();
        }
        public SaveAdapter(Type type, List<SaveAdapterParameter> parameters) : this(type)
        {
            _parameters.AddRange(parameters);
        }

        public SaveAdapter(Type type, string[] parameters) : this(type)
        {
            foreach (var parameter in parameters)
                _parameters.Add(new SaveAdapterParameter(parameter, false));
        }

        public SaveAdapter Add(IEnumerable<string> parameters)
        {
            foreach (var parameter in parameters)
                _parameters.Add(new SaveAdapterParameter(parameter, false));
            return this;
        }
    }
}
