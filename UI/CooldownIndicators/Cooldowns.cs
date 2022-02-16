using CalamityMod.DataStructures;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.Graphics.Shaders;

namespace CalamityMod.UI.CooldownIndicators
{
    public class NebulousCoreCooldown : CooldownIndicator
    {
        public override bool DisplayMe => true; 
        public override string Name => "Nebulous Core Cooldown";
        public override string Texture => "CalamityMod/UI/CooldownIndicators/NebulousCore";
        public override Color OutlineColor => Color.Lerp(new Color(252, 109, 203), new Color(58, 91, 146), Completion);
        public override Color CooldownColorStart => new Color(148, 62, 216);
        public override Color CooldownColorEnd => new Color(255, 187, 207);

        public NebulousCoreCooldown(int duration, Player player) : base(duration, player)
        {
        }
    }

    public class GlobalDodgeCooldown : CooldownIndicator
    {
        public override string SyncID => "GlobalDodge";
        public override bool DisplayMe => true;
        public override string Name => "Dodge Cooldown";
        public override string Texture => "CalamityMod/UI/CooldownIndicators/" + skinTexture;
        public override Color OutlineColor => outlineColor;
        public override Color CooldownColorStart => cooldownColorStart;
        public override Color CooldownColorEnd => cooldownColorEnd;

        //It's the same cooldown with different skins each time, basically.
        public string skinTexture;
        public Color outlineColor;
        public Color cooldownColorStart;
        public Color cooldownColorEnd;

        public GlobalDodgeCooldown(int duration, Player player, string skin = "default") : base(duration, player)
        {
            switch (skin)
            {
                case "abyssmirror":
                    skinTexture = "AbyssEvade";
                    outlineColor = new Color(125, 157, 149);
                    cooldownColorStart = new Color(167, 147, 151);
                    cooldownColorEnd = new Color(217, 209, 195);
                    break;

                case "eclipsemirror":
                    skinTexture = "EclipseEvade";
                    outlineColor = new Color(152, 206, 248);
                    cooldownColorStart = new Color(255, 192, 71);
                    cooldownColorEnd = new Color(255, 255, 151);
                    break;

                default:
                    skinTexture = "GlobalDodge";
                    outlineColor = Color.White;
                    cooldownColorStart = Color.CornflowerBlue;
                    cooldownColorEnd = Color.White;
                    break;
            }
        }
    }

    public class IceShieldCooldown : CooldownIndicator
    {
        public override string SyncID => "SirenHeartShield";
        public override bool DisplayMe => true;
        public override string Name => "Ice Shield Cooldown";
        public override string Texture => "CalamityMod/UI/CooldownIndicators/SirenIceShield";
        public override Color OutlineColor => Color.Lerp(new Color(163, 186, 198), new Color(146, 187, 255), (float)Math.Sin(Main.GlobalTime) * 0.5f + 0.5f);
        public override Color CooldownColorStart => new Color(124, 195, 214);
        public override Color CooldownColorEnd => new Color(147, 230, 253);


        public IceShieldCooldown(int duration, Player player) : base(duration, player)
        {
        }
    }
}
