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
    public class JetpackCooldown : CooldownIndicator
    {
        public override bool DisplayMe => true;
        public override string Name => "Jet Boost Cooldown";
        public override string Texture => "CalamityMod/UI/CooldownIndicators/" + skinTexture;
        public override Color OutlineColor => outlineColor;
        public override Color CooldownColorStart => Color.Lerp(cooldownColorStart, cooldownColorEnd, 1 - Completion);
        public override Color CooldownColorEnd => Color.Lerp(cooldownColorStart, cooldownColorEnd, 1 - Completion);

        //It's the same cooldown with different skins each time, basically.
        public string skinTexture;
        public Color outlineColor;
        public Color cooldownColorStart;
        public Color cooldownColorEnd;

        public JetpackCooldown(int duration, Player player, string skin = "default") : base(duration, player)
        {
            switch (skin)
            {
                case "birb":
                    skinTexture = "BlunderBooster";
                    outlineColor = new Color(210, 180, 100);
                    cooldownColorStart = new Color(90, 67, 76);
                    cooldownColorEnd = new Color(255, 83, 145);
                    break;

                default:
                    skinTexture = "PlaguedFuelPack";
                    outlineColor = new Color(130, 190, 64);
                    cooldownColorStart = new Color(230, 64, 64);
                    cooldownColorEnd = new Color(209, 248, 62);
                    break;
            }
        }
    }
}