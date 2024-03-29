﻿using HOI4ModBuilder.hoiDataObjects.map;
using HOI4ModBuilder.src.hoiDataObjects.map.railways;
using HOI4ModBuilder.src.utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HOI4ModBuilder.src.hoiDataObjects.map.supply
{
    class SupplyNode
    {
        private readonly int _hashCode = NextHashCode;
        private static int _nextHashCode;
        private static int NextHashCode = _nextHashCode == int.MaxValue ? _nextHashCode = int.MinValue : _nextHashCode++;
        public override int GetHashCode() => _hashCode;

        private byte level;
        private Province province;

        public SupplyNode(byte level, in Province province)
        {
            this.level = level;
            this.province = province;
        }

        public bool AddToProvince()
        {
            if (province != null)
            {
                if (province.supplyNode != null) return false;
                province.supplyNode = this;
                SupplyManager.NeedToSaveSupplyNodes = true;
                return true;
            }
            return false;
        }

        public bool RemoveFromProvince()
        {
            if (province != null)
            {
                if (province.supplyNode == null) return false;
                province.supplyNode = null;
                SupplyManager.NeedToSaveSupplyNodes = true;
                return true;
            }
            else return false;
        }

        public Province GetProvince()
        {
            return province;
        }

        public void Save(StringBuilder sb)
        {
            sb.Append(level).Append(' ').Append(province.Id).Append(' ').Append(Constants.NEW_LINE);
        }

        public override bool Equals(object obj)
        {
            return obj is SupplyNode node &&
                   level == node.level &&
                   EqualityComparer<Province>.Default.Equals(province, node.province);
        }
    }
}
