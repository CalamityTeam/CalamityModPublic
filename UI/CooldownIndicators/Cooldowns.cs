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

    public class InkBombCooldown : CooldownIndicator
    {
        public override string SyncID => "InkBomb";
        public override bool DisplayMe => true;
        public override string Name => "Ink Bomb Cooldown";
        public override string Texture => "CalamityMod/UI/CooldownIndicators/InkBomb";
        public override Color OutlineColor => new Color(205, 182, 137);
        public override Color CooldownColorStart => Color.Lerp(new Color(177, 147, 89), new Color(105, 103, 126), Completion);
        public override Color CooldownColorEnd => Color.Lerp(new Color(177, 147, 89), new Color(105, 103, 126), Completion);


        public InkBombCooldown(int duration, Player player) : base(duration, player)
        {
        }
    }

    public class DivingBrokenPlatesCooldown : CooldownIndicator
    {
        public override string SyncID => "AbyssalDivingBrokenPlates";
        public override bool DisplayMe => true;
        public override string Name => "Abyssal Diving Suit Broken Plates";
        public override string Texture => "CalamityMod/UI/CooldownIndicators/BrokenPlates";
        public override Color OutlineColor => new Color(194, 173, 146);
        public override Color CooldownColorStart => Color.Lerp(new Color(91, 121, 150), new Color(30, 50, 77), Completion);
        public override Color CooldownColorEnd => Color.Lerp(new Color(91, 121, 150), new Color(30, 50, 77), Completion);


        public DivingBrokenPlatesCooldown(int duration, Player player) : base(duration, player)
        {
        }

        public override void OnCooldownEnd()
        {
            AfflictedPlayer.Calamity().abyssalDivingSuitPlateHits = 0;
        }
    }

    public class DivingPlatesBreaking : CooldownIndicator
    {
        public override bool DisplayMe => true;
        public override string Name => "Abyssal Diving Suit Plates Durability";
        public override string Texture => "CalamityMod/UI/CooldownIndicators/BreakingPlates";
        public override string TextureOutline => "CalamityMod/UI/CooldownIndicators/BrokenPlatesOutline";
        public override string TextureOverlay => "CalamityMod/UI/CooldownIndicators/BrokenPlatesOverlay";
        public override Color OutlineColor => TimeLeft == 0 ? new Color(147, 218, 183) : TimeLeft == 1 ? new Color(233, 190, 134) : new Color(220, 111, 94);
        public override Color CooldownColorStart => Color.Lerp(new Color(160, 174, 174), new Color(192, 11, 107), Completion);
        public override Color CooldownColorEnd => Color.Lerp(new Color(160, 174, 174), new Color(192, 11, 107), Completion);
        public override bool CanTickDown(Player player) => false;

        public DivingPlatesBreaking(int duration, Player player) : base(duration, player)
        {
        }
    }
}
