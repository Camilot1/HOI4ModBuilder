using HOI4ModBuilder.hoiDataObjects.history.countries;
using HOI4ModBuilder.hoiDataObjects.map;
using HOI4ModBuilder.managers;
using HOI4ModBuilder.src.dataObjects.argBlocks;
using HOI4ModBuilder.src.dataObjects;
using HOI4ModBuilder.src.utils;
using Pdoxcl2Sharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace HOI4ModBuilder.src.hoiDataObjects.map.adjacencies
{
    class AdjacencyRule : IParadoxRead
    {
        private readonly int _hashCode = NextHashCode;
        private static int _nextHashCode;
        private static int NextHashCode = _nextHashCode == int.MaxValue ? _nextHashCode = int.MinValue : _nextHashCode++;
        public override int GetHashCode() => _hashCode;

        public string name;
        public RelationRule[] relationRules;
        public Dictionary<Country, Dictionary<string, List<object>>> effectsByCountries = new Dictionary<Country, Dictionary<string, List<object>>>(0);
        public List<Province> requiredProvinces = new List<Province>(0);
        public AdjacencyRuleDisableTrigger disableTrigger;
        public Province iconProvince;
        public List<int> iconOffset;

        public AdjacencyRule()
        {
            relationRules = new RelationRule[]
            {
                new RelationRule(this, "contested"),
                new RelationRule(this, "enemy"),
                new RelationRule(this, "friend"),
                new RelationRule(this, "neutral")
            };
        }

        public virtual void Save(StringBuilder sb, string tab)
        {
            string outTab = tab + tab;
            sb.Append("adjacency_rule = {").Append(Constants.NEW_LINE)
                .Append(tab).Append("name = \"").Append(name).Append("\"").Append(Constants.NEW_LINE).Append(Constants.NEW_LINE);

            bool hasRelationTriggers = false;
            foreach (var relationRule in relationRules)
            {
                if (relationRule.disableTrigger.isDisableArgBlocks.Count > 0)
                {
                    relationRule.disableTrigger.Save(sb, tab, tab);
                    hasRelationTriggers = true;
                }
            }
            if (hasRelationTriggers) sb.Append(Constants.NEW_LINE);

            foreach (var relationRule in relationRules) relationRule.Save(sb, tab, tab);
            sb.Append(Constants.NEW_LINE);

            if (requiredProvinces.Count > 0)
            {
                sb.Append(tab).Append("required_provinces = { ");
                foreach (var p in requiredProvinces) sb.Append(p.Id).Append(' ');
                sb.Append('}').Append(Constants.NEW_LINE).Append(Constants.NEW_LINE);
            }

            if (disableTrigger != null && (disableTrigger.tooltip != null || disableTrigger.tooltip != "" || disableTrigger.dataArgsBlocks.Count > 0))
            {
                sb.Append(tab).Append("is_disabled = {").Append(Constants.NEW_LINE);
                disableTrigger.Save(sb, outTab, tab);
                sb.Append(tab).Append('}').Append(Constants.NEW_LINE).Append(Constants.NEW_LINE);
            }

            if (iconProvince != null) sb.Append(tab).Append("icon = ").Append(iconProvince.Id).Append(Constants.NEW_LINE);
            if (iconOffset != null && iconOffset.Count == 3) sb.Append(tab).Append("offset = { ")
                    .Append(iconOffset[0]).Append(' ').Append(iconOffset[1]).Append(' ').Append(iconOffset[2]).Append(" }").Append(Constants.NEW_LINE);

            sb.Append("}").Append(Constants.NEW_LINE).Append(Constants.NEW_LINE);
        }

        public void TokenCallback(ParadoxParser parser, string token)
        {
            switch (token)
            {
                case "name": name = parser.ReadString(); break;
                case "is_contested": parser.Parse(relationRules[0].disableTrigger); break;
                case "contested": parser.Parse(relationRules[0]); break;
                case "is_enemy": parser.Parse(relationRules[1].disableTrigger); break;
                case "enemy": parser.Parse(relationRules[1]); break;
                case "is_friend": parser.Parse(relationRules[2].disableTrigger); break;
                case "friend": parser.Parse(relationRules[2]); break;
                case "is_neutral": parser.Parse(relationRules[3].disableTrigger); break;
                case "neutral": parser.Parse(relationRules[3]); break;
                case "required_provinces":
                    foreach (int id in parser.ReadIntList())
                    {
                        if (ProvinceManager.TryGetProvince((ushort)id, out Province p))
                            requiredProvinces.Add(p);
                        else Logger.LogError(
                                EnumLocKey.ERROR_ADJACENCY_RULE_LOADING_REQUIRED_PROVINCE_DOESNT_EXISTS,
                                new Dictionary<string, string>
                                {
                                    { "{adjacencyRuleName", name },
                                    { "{requiredProvinceId}", $"{id}"}
                                }
                            );
                    }
                    break;
                case "is_disabled":
                    disableTrigger = new AdjacencyRuleDisableTrigger(this);
                    parser.Parse(disableTrigger);
                    break;
                case "icon":
                    ushort iconProvinceId = parser.ReadUInt16();
                    if (!ProvinceManager.TryGetProvince(iconProvinceId, out iconProvince))
                        Logger.LogError(
                            EnumLocKey.ERROR_ADJACENCY_RULE_LOADING_ICON_PROVINCE_DOESNT_EXISTS,
                            new Dictionary<string, string>
                            {
                                { "{adjacencyRuleName", name },
                                { "{iconProvinceId}", $"{iconProvinceId}"}
                            }
                        );
                    break;
                case "offset":
                    var tempIconOffset = parser.ReadStringList();
                    if (tempIconOffset.Count != 3)
                        Logger.LogError(
                            EnumLocKey.ERROR_ADJACENCY_RULE_LOADING_ICON_OFFSET_INCORRECT_VALUES_COUNT,
                            new Dictionary<string, string>
                            {
                                { "{adjacencyRuleName", name },
                                { "{currentCount}", $"{iconOffset.Count}"},
                                { "{correctCount}", "3"},
                            }
                        );

                    iconOffset = new List<int>(3);
                    foreach (var value in tempIconOffset)
                    {
                        if (int.TryParse(value, out int offsetIntValue)) iconOffset.Add(offsetIntValue);
                        else if (float.TryParse(value.Replace('.', ','), out float offsetFloatValue)) iconOffset.Add((int)Math.Round(offsetFloatValue));
                        else Logger.LogError(
                                EnumLocKey.ERROR_ADJACENCY_RULE_LOADING_ICON_OFFSET_INCORRECT_VALUE,
                                new Dictionary<string, string>
                                {
                                    { "{adjacencyRuleName", name },
                                    { "{currentCount}", $"{iconOffset.Count}"},
                                    { "{correctCount}", "3"},
                                }
                            );
                    }
                    break;
                default:
                    Logger.LogWarning(
                            EnumLocKey.ERROR_ADJACENCY_RULE_LOADING_UNKNOWN_TOKEN,
                            new Dictionary<string, string>
                            {
                                { "{adjacencyRuleName", name },
                                { "{token}", token}
                            }
                        );
                    break;
            }
        }

        public override bool Equals(object obj)
        {
            return obj is AdjacencyRule rule &&
                   name == rule.name;
        }
    }

    class AdjacencyRuleDisableTrigger : IParadoxRead
    {
        public AdjacencyRule rule;
        public string tooltip;
        public List<DataArgsBlock> dataArgsBlocks;

        public AdjacencyRuleDisableTrigger(AdjacencyRule rule)
        {
            this.rule = rule;
            dataArgsBlocks = new List<DataArgsBlock>(0);
        }

        public void Save(StringBuilder sb, string outTab, string tab)
        {
            if (dataArgsBlocks.Count == 1 && dataArgsBlocks[0].innerArgsBlocks.Count == 0) sb.Append(outTab);
            foreach (var block in dataArgsBlocks) block.Save(sb, outTab, tab);
            if (dataArgsBlocks.Count == 1) sb.Append(Constants.NEW_LINE);
            if (tooltip != null && tooltip != "")
                sb.Append(outTab).Append("tooltip = ").Append(tooltip).Append(Constants.NEW_LINE);
        }

        public void TokenCallback(ParadoxParser parser, string token)
        {
            if (token == "tooltip")
            {
                tooltip = parser.ReadString();
            }
            else
            {
                try
                {
                    DataArgsBlocksManager.ParseDataArgsBlock(parser, null, token, null, dataArgsBlocks);
                }
                catch (Exception ex)
                {
                    throw new Exception(GuiLocManager.GetLoc(
                        EnumLocKey.EXCEPTION_WHILE_ADJACENCY_RULE_IS_DISABLED_BLOCK_LOADING,
                        new Dictionary<string, string>
                        {
                            { "{adjacencyRuleName}", rule.name },
                            { "{token}", token }
                        }
                    ), ex);
                }
            }
        }
    }

    class RelationRule : IParadoxRead
    {
        public AdjacencyRule rule;
        public string relation;
        public bool army, navy, submarine, trade;
        public RelationRuleDisableTrigger disableTrigger;

        public RelationRule(AdjacencyRule rule, string relation)
        {
            this.rule = rule;
            this.relation = relation;
            disableTrigger = new RelationRuleDisableTrigger(rule, relation);
        }

        public virtual void Save(StringBuilder sb, string outTab, string tab)
        {
            sb.Append(outTab).Append(relation).Append(" = {").Append(Constants.NEW_LINE);
            sb.Append(outTab).Append(tab).Append("army = ").Append(army ? "yes" : "no").Append(Constants.NEW_LINE);
            sb.Append(outTab).Append(tab).Append("navy = ").Append(navy ? "yes" : "no").Append(Constants.NEW_LINE);
            sb.Append(outTab).Append(tab).Append("submarine = ").Append(submarine ? "yes" : "no").Append(Constants.NEW_LINE);
            sb.Append(outTab).Append(tab).Append("trade = ").Append(trade ? "yes" : "no").Append(Constants.NEW_LINE);
            sb.Append(outTab).Append('}').Append(Constants.NEW_LINE);
        }

        public void TokenCallback(ParadoxParser parser, string token)
        {
            switch (token)
            {
                case "army": army = parser.ReadBool(); break;
                case "navy": navy = parser.ReadBool(); break;
                case "submarine": submarine = parser.ReadBool(); break;
                case "trade": trade = parser.ReadBool(); break;
                default:
                    Logger.LogError(
                        EnumLocKey.ERROR_ADJACENCY_RULE_RELATION_UNKNOWN_TOKEN,
                        new Dictionary<string, string>
                        {
                            { "{adjacencyRuleName}", rule.name },
                            { "{relationName}", relation },
                            { "{token}", token }
                        }
                    );
                    break;
            }
        }
    }

    class RelationRuleDisableTrigger : IParadoxRead
    {
        private AdjacencyRule _rule;
        private string _relation;
        public List<DataArgsBlock> isDisableArgBlocks = new List<DataArgsBlock>(0);

        public RelationRuleDisableTrigger(AdjacencyRule rule, string relation)
        {
            _rule = rule;
            _relation = relation;
        }

        public virtual void Save(StringBuilder sb, string outTab, string tab)
        {
            if (isDisableArgBlocks.Count == 0) return;

            sb.Append(outTab).Append("is_").Append(_relation).Append(" = {").Append(Constants.NEW_LINE);

            string newTab = outTab + tab;
            foreach (var block in isDisableArgBlocks) block.Save(sb, newTab, tab);

            sb.Append(outTab).Append('}').Append(Constants.NEW_LINE);
        }

        public void TokenCallback(ParadoxParser parser, string token)
        {
            try
            {
                DataArgsBlocksManager.ParseDataArgsBlock(parser, null, token, null, isDisableArgBlocks);
            }
            catch (Exception ex)
            {
                throw new Exception(GuiLocManager.GetLoc(
                        EnumLocKey.EXCEPTION_WHILE_ADJACENCY_RULE_RELATION_BLOCK_LOADING,
                        new Dictionary<string, string>
                        {
                            { "{relationName}", _relation },
                            { "{adjacencyRuleName}", _rule.name },
                            { "{token}", token }
                        }
                    ), ex);
            }
        }
    }


    class AdjacencyRuleList : IParadoxRead
    {
        public Dictionary<string, AdjacencyRule> rules;

        public AdjacencyRuleList(Dictionary<string, AdjacencyRule> rules)
        {
            this.rules = rules;
        }

        public void TokenCallback(ParadoxParser parser, string token)
        {
            if (token == "adjacency_rule")
            {
                var rule = new AdjacencyRule();
                parser.Parse(rule);
                if (!rules.ContainsKey(rule.name)) rules[rule.name] = rule;
                else Logger.LogError(
                    EnumLocKey.ERROR_ADJACENCY_RULE_DUPLICATE_NAME,
                    new Dictionary<string, string> { { "{adjacencyRuleName}", rule.name } }
                );
            }
        }
    }

}
