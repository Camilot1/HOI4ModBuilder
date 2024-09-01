using HOI4ModBuilder.hoiDataObjects.map;
using HOI4ModBuilder.src.hoiDataObjects;
using HOI4ModBuilder.src.hoiDataObjects.history.countries;
using HOI4ModBuilder.src.utils;
using System;
using System.Collections.Generic;

namespace HOI4ModBuilder.hoiDataObjects.history.countries
{
    public class Country
    {
        private readonly int _hashCode = NextHashCode;
        private static int _nextHashCode;
        private static int NextHashCode = _nextHashCode == int.MaxValue ? _nextHashCode = int.MinValue : _nextHashCode++;
        public override int GetHashCode() => _hashCode;

        //public String countryFlag;

        public bool HasChangedTag { get; private set; }

        private string _tag;
        public string Tag
        {
            get => _tag;
            set
            {
                if (_tag == value) return;

                if (CountryManager.ContainsCountryTag(value))
                    throw new Exception(GuiLocManager.GetLoc(
                            EnumLocKey.EXCEPTION_COUNTRY_TAG_UPDATE_VALUE_IS_USED,
                            new Dictionary<string, string> { { "{tag}", $"{value}" } }
                        ));
                else CountryManager.RemoveCountryByTag(_tag); //TODO Добавить обработчик внутри менеджена на обновление id провинции и словарей с ВП и постройками

                _tag = value;
                HasChangedTag = true;

                CountryManager.AddCountryByTag(_tag, this);
            }
        }

        public string cosmeticTag;
        public string graphicalCulture;
        public string graphicalCulture2d;
        public int color;
        public int colorUI;

        public State capitalState;
        public List<State> ownStates = new List<State>(0);
        public List<State> controlsStates = new List<State>(0);
        public List<State> hasCoresAtStates = new List<State>(0);
        public List<State> hasClaimsAtState = new List<State>(0);
        public uint convoys;
        public List<Technology> technologies = new List<Technology>(0);
        public List<Idea> ideas = new List<Idea>(0);
        public Faction faction;

        public Country()
        {

        }

        public Country(string tag)
        {
            this._tag = tag;
        }

        public void ClearStates()
        {
            ownStates = new List<State>(0);
            controlsStates = new List<State>(0);
            hasCoresAtStates = new List<State>(0);
            hasClaimsAtState = new List<State>(0);
        }

        public override bool Equals(object obj)
        {
            return obj is Country country &&
                   _tag == country._tag;
        }
    }
}
