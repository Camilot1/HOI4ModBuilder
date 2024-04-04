using HOI4ModBuilder.src.Pdoxcl2Sharp;
using HOI4ModBuilder.src.utils;
using HOI4ModBuilder.src.utils.exceptions;
using Pdoxcl2Sharp;
using System.Text;

namespace HOI4ModBuilder.src.hoiDataObjects.history.units.oobs.land
{

    class DivisionName : IParadoxObject
    {
        public static readonly string BLOCK_NAME = "division_name";

        public bool _needToSave;
        public bool NeedToSave { get => _needToSave; }

        private static readonly string TOKEN_IS_NAME_ORDERED = "is_name_ordered";
        public bool? _isNameOrdered;
        public bool? IsNameOrdered { get => _isNameOrdered; set => Utils.Setter(ref _isNameOrdered, ref value, ref _needToSave); }

        private static readonly string TOKEN_NAME_ORDER = "name_order";
        public int? _nameOrder;
        public int? NameOrder { get => _nameOrder; set => Utils.Setter(ref _nameOrder, ref value, ref _needToSave); }

        public bool Save(StringBuilder sb, string outTab, string tab)
        {
            string newOutTab = outTab + tab;

            ParadoxUtils.StartBlock(sb, outTab, BLOCK_NAME);

            ParadoxUtils.Save(sb, newOutTab, TOKEN_IS_NAME_ORDERED, _isNameOrdered);
            ParadoxUtils.Save(sb, newOutTab, TOKEN_NAME_ORDER, _nameOrder);

            ParadoxUtils.EndBlock(sb, outTab);

            return true;
        }

        public void TokenCallback(ParadoxParser parser, LinkedLayer prevLayer, string token)
        {
            Logger.WrapTokenCallbackExceptions(BLOCK_NAME, () =>
            {
                if (token == TOKEN_IS_NAME_ORDERED)
                    Logger.CheckLayeredValueOverrideAndSet(prevLayer, token, ref _isNameOrdered, parser.ReadBool());
                else if (token == TOKEN_NAME_ORDER)
                    Logger.CheckLayeredValueOverrideAndSet(prevLayer, token, ref _nameOrder, parser.ReadInt32());
                else
                    throw new UnknownTokenException(token);
            });
        }

        public bool Validate(LinkedLayer prevLayer)
        {
            bool result = true;

            CheckAndLogUnit.WARNINGS
                .HasMandatory(ref result, prevLayer, TOKEN_IS_NAME_ORDERED, ref _isNameOrdered);
            //.HasMandatory(ref result, prevLayer, TOKEN_NAME_ORDER, ref _nameOrder);

            return result;
        }
    }
}
