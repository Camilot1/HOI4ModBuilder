using HOI4ModBuilder.src.hoiDataObjects.history.units.oobs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace HOI4ModBuilder.src.utils
{
    class CheckAndLogUnit
    {
        public static readonly CheckAndLogUnit WARNINGS = new CheckAndLogUnit(
            (prevLayer, enumLocKey) => Logger.LogLayeredWarning(prevLayer, enumLocKey),
            (prevLayer, enumLocKey, replaceValues) => Logger.LogLayeredWarning(prevLayer, enumLocKey, replaceValues),
            (prevLayer, currentLayer, enumLocKey, replaceValues) => Logger.LogLayeredWarning(prevLayer, currentLayer, enumLocKey, replaceValues)
        );

        public static readonly CheckAndLogUnit ERRORS = new CheckAndLogUnit(
            (prevLayer, enumLocKey) => Logger.LogLayeredError(prevLayer, enumLocKey),
            (prevLayer, enumLocKey, replaceValues) => Logger.LogLayeredError(prevLayer, enumLocKey, replaceValues),
            (prevLayer, currentLayer, enumLocKey, replaceValues) => Logger.LogLayeredError(prevLayer, currentLayer, enumLocKey, replaceValues)
        );

        private Action<LinkedLayer, EnumLocKey> _action0;
        private Action<LinkedLayer, EnumLocKey, Dictionary<string, string>> _action1;
        private Action<LinkedLayer, string, EnumLocKey, Dictionary<string, string>> _action2;

        public CheckAndLogUnit(
            Action<LinkedLayer, EnumLocKey> action0,
            Action<LinkedLayer, EnumLocKey, Dictionary<string, string>> action1,
            Action<LinkedLayer, string, EnumLocKey, Dictionary<string, string>> action2
        )
        {
            _action0 = action0;
            _action1 = action1;
            _action2 = action2;
        }

        public CheckAndLogUnit Check(ref bool result, LinkedLayer prevLayer, bool checkValue, EnumLocKey enumLocKey)
        {
            if (checkValue) return this;

            _action0(prevLayer, enumLocKey);
            result = false;

            return this;
        }

        public CheckAndLogUnit HasMandatory<T>(ref bool result, LinkedLayer prevLayer, string parameterName, ref T value)
        {
            if (value == null)
            {
                _action1(
                    prevLayer, EnumLocKey.BLOCK_DOESNT_HAVE_MANDATORY_INNER_PARAMETER,
                    new Dictionary<string, string> { { "{parameterName}", parameterName } }
                );
                result = false;
            }

            return this;
        }

        public CheckAndLogUnit HasAtLeastOneMandatory(ref bool result, LinkedLayer prevLayer, string currentLayer, string[] parametersNames, bool hasAtLeastOneValue)
        {
            if (!hasAtLeastOneValue)
            {
                _action2(
                    prevLayer, currentLayer, EnumLocKey.BLOCK_MUST_HAVE_AT_LEAST_ONE_MANDATORY_PARAMETER,
                    new Dictionary<string, string> { { "{parametersNames}", $"{string.Join(", ", parametersNames)}" } }
                ); ;
                result = false;
            }

            return this;
        }

        public CheckAndLogUnit HasOnlyOneMutuallyExclusiveMandatory(ref bool result, LinkedLayer prevLayer, string currentLayer, string[] parametersNames, bool[] values)
        {
            byte counter = 0;

            foreach (var value in values)
            {
                if (value)
                {
                    counter++;
                    if (counter == 2) break;
                }
            }

            if (counter == 0)
            {
                _action2(
                    prevLayer, currentLayer, EnumLocKey.BLOCK_DOESNT_HAVE_ANY_OF_SEVERAL_MUTUALLY_EXCLUSIVE_MANDATORY_INNER_PARAMETERS,
                    new Dictionary<string, string> { { "{parametersNames}", $"{string.Join(", ", parametersNames)}" } }
                ); ;
                result = false;
            }
            if (counter == 2)
            {
                _action2(
                    prevLayer, currentLayer, EnumLocKey.BLOCK_HAS_SEVERAL_MUTUALLY_EXCLUSIVE_MANDATORY_INNER_PARAMETERS,
                    new Dictionary<string, string> { { "{parametersNames}", $"{string.Join(", ", parametersNames)}" } }
                ); ;
                result = false;
            }

            return this;
        }

        public CheckAndLogUnit CheckRangeAndClamp<T>(ref bool result, LinkedLayer prevLayer, string parameterName, Func<T, T, T, T> func, ref T value, T min, T max)
        {
            T product = func(value, min, max);
            if (product != null)
            {
                _action1(
                    prevLayer, EnumLocKey.BLOCK_DOESNT_HAVE_MANDATORY_INNER_PARAMETER,
                    new Dictionary<string, string>
                    {
                        { "{parameterName}", parameterName },
                        { "{parameterValue}", $"{value}" },
                        { "{allowedRange}", $"[{min}; {max}]" },
                        { "{newParameterValue}", $"{product}" }
                    }
                );
                value = product;
                result = false;
            }

            return this;
        }
    }
}
