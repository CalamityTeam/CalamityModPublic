using System;
using System.Diagnostics;
using Terraria;

namespace CalamityMod.Utilities
{
    internal static class DustExtensions
    {
        extension(Dust dust)
        {
            public float dataAsFloat
            {
                get => (float)dust.customData;
                set => dust.customData = value;
            }


            [Obsolete("DO NOT USE. It is an unsafe fix to an even more unsafe vanilla method.")]
            public static Dust BetterCloneDust(Dust rf)
            {
                if (rf.dustIndex == Main.maxDustToDraw)
                {
                    return new Dust();
                }

                int num = Dust.NewDust(rf.position, 0, 0, rf.type);
                if (!Main.dust.IndexInRange(num))
                {
                    return new Dust();
                }
                Dust obj = Main.dust[num];
                obj.position = rf.position;
                obj.velocity = rf.velocity;
                obj.fadeIn = rf.fadeIn;
                obj.noGravity = rf.noGravity;
                obj.scale = rf.scale;
                obj.rotation = rf.rotation;
                obj.noLight = rf.noLight;
                obj.active = rf.active;
                obj.type = rf.type;
                obj.color = rf.color;
                obj.alpha = rf.alpha;
                obj.frame = rf.frame;
                obj.shader = rf.shader;
                obj.customData = rf.customData;
                return obj;
            }

            [Obsolete("DO NOT USE. It is an unsafe fix to an even more unsafe vanilla method.")]
            public static Dust BetterCloneDust(int dustIndex)
            {
                return BetterCloneDust(Main.dust[dustIndex]);
            }
        }
    }
}
