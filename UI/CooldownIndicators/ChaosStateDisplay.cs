using CalamityMod.DataStructures;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.Graphics.Shaders;
using static Terraria.ModLoader.ModContent;
using static CalamityMod.CalamityUtils;
using CalamityMod.CalPlayer;

namespace CalamityMod.UI.CooldownIndicators
{
    public class ChaosStateDisplay : CooldownIndicator
    {
        public override bool DisplayMe => CalamityConfig.Instance.VanillaCooldownDisplay && AfflictedPlayer.chaosState;
        public override string Name => "Teleportation Cooldown";
        public override string Texture => "CalamityMod/UI/CooldownIndicators/ChaosState" + skinTexture;
        public override Color OutlineColor => outlineColor;
        public override Color CooldownColorStart => Color.Lerp(cooldownColorStart, cooldownColorEnd, 1 - Completion);
        public override Color CooldownColorEnd => Color.Lerp(cooldownColorStart, cooldownColorEnd, 1 - Completion);

        //It's the same cooldown with different skins each time, basically.
        public string skinTexture;
        public Color outlineColor;
        public Color cooldownColorStart;
        public Color cooldownColorEnd;

        public ChaosStateDisplay(int duration, Player player, string skin = "default") : base(duration, player)
        {
            switch (skin)
            {
                case "spectralveil":
                    skinTexture = "Veil";
                    outlineColor = new Color(138, 120, 222);
                    cooldownColorStart = new Color(46, 46, 134);
                    cooldownColorEnd = new Color(81, 90, 156);
                    break;

                case "normalityrelocator":
                    skinTexture = "NR";
                    outlineColor = new Color(129, 239, 246);
                    cooldownColorStart = new Color(134, 143, 151);
                    cooldownColorEnd = new Color(129, 239, 246);
                    break;

                default:
                    skinTexture = "";
                    outlineColor = new Color(246, 116, 181);
                    cooldownColorStart = new Color(223, 58, 140);
                    cooldownColorEnd = new Color(255, 179, 218);
                    break;
            }
        }
    }
}