using HOI4ModBuilder.src.utils.structs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static HOI4ModBuilder.utils.Enums;
using static HOI4ModBuilder.utils.Structs;

namespace HOI4ModBuilder.hoiDataObjects.gfx
{
    public class Sprite
    {
        public string name;
        public string textureFile; //texturefile
        public uint framesCount; //noOfFrames
        public string effectFile;

        public bool isAllwaysTransparent; //allwaystransparent
        public bool isLegacyLazeLoad; //legacy_lazy_load;
        public bool isTransparenceCheck; //transparencecheck 
        public Animation animation;
        public class Animation
        {
            public string maskFile; //animationmaskfile 
            public string textureFile; //animationtexturefile
            public float rotation; //animationrotation 
            public Value2F rotationOffset; //animationrotationoffset 
            public bool isLooped; //animationlooping 
            public float time; //animationtime //[secs]
            public float delay; //animationdelay //[secs]
            public EnumBlendMode blendMode; //animationblendmode
            public EnumAnimationType type; //animationtype
            public Value2F textureScale; //animationtexturescale 
            public uint[] frames; //animationframes
        }
    }
}
