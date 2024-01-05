using HOI4ModBuilder.hoiDataObjects.map;
using HOI4ModBuilder.src.hoiDataObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static HOI4ModBuilder.utils.Structs;

namespace HOI4ModBuilder.hoiDataObjects.history.countries
{
    class Country
    {
        //public String countryFlag;
        public string tag;
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
            this.tag = tag;
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
                   tag == country.tag;
        }

        public override int GetHashCode()
        {
            return -1573750901 + EqualityComparer<string>.Default.GetHashCode(tag);
        }
    }
}
