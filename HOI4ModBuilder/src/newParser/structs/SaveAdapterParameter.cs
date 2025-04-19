using Newtonsoft.Json;

namespace HOI4ModBuilder.src.newParser.structs
{
    public struct SaveAdapterParameter
    {
        [JsonProperty("Name")]
        public string Name { get; set; }
        [JsonProperty("SaveIfEmpty")]
        public bool SaveIfEmpty { get; set; }
        [JsonProperty("IsForceInline")]
        public bool IsForceInline { get; set; }
        [JsonProperty("IsForceMultiline")]
        public bool IsForceMultiline { get; set; }
        [JsonProperty("AddEmptyLineBefore")]
        public bool AddEmptyLineBefore { get; set; }

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
