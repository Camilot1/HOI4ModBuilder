namespace HOI4ModBuilder.src.openTK.text
{
    public struct TextLayerContext
    {
        public readonly string Parameter;
        public readonly string ParameterValue;

        public TextLayerContext(string parameter, string parameterValue)
        {
            Parameter = parameter;
            ParameterValue = parameterValue;
        }
    }
}
