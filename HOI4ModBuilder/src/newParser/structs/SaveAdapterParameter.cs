using Newtonsoft.Json;

namespace HOI4ModBuilder.src.newParser.structs
{
    public struct SaveAdapterParameter
    {
        [JsonProperty("Name")]
        public string Name { get; private set; }
        [JsonProperty("SaveIfEmpty")]
        public bool SaveIfEmpty { get; private set; }
        [JsonProperty("IsForceInline")]
        public bool IsForceInline { get; private set; }
        [JsonProperty("IsForceMultiline")]
        public bool IsForceMultiline { get; private set; }
        [JsonProperty("AddEmptyLineBefore")]
        public bool AddEmptyLineBefore { get; private set; }

        public SaveAdapterParameter(string name, bool saveIfEmpty, bool isForceInline, bool isForceMultiline, bool addEmptyLineBefore)
        {
            Name = name;
            SaveIfEmpty = saveIfEmpty;
            IsForceInline = isForceInline;
            IsForceMultiline = isForceMultiline;
            AddEmptyLineBefore = addEmptyLineBefore;
        }
    }
}
