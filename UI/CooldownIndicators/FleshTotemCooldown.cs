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
    public class FleshTotemCooldown : CooldownIndicator
    {
        public override string SyncID => "FleshTotem";
        public override bool DisplayMe => true;
        public override string Name => "Contact Damage Halving Cooldown";
        public override string Texture => "CalamityMod/UI/CooldownIndicators/" + skinTexture;
        public override Color OutlineColor => outlineColor;
        public override Color CooldownColorStart => cooldownColorStart;
        public override Color CooldownColorEnd => cooldownColorEnd;

        //It's the same cooldown with different skins each time, basically.
        public string skinTexture;
        public Color outlineColor;
        public Color cooldownColorStart;
        public Color cooldownColorEnd;

        public FleshTotemCooldown(int duration, Player player, string skin = "default") : base(duration, player)
        {
            switch (skin)
            {
                case "bloodgod":
                    skinTexture = "BloodGodTotem";
                    outlineColor = new Color(255, 162, 205);
                    cooldownColorStart = new Color(193, 205, 255);
                    cooldownColorEnd = new Color(255, 193, 219);
                    break;

                default:
                    skinTexture = "FleshTotem";
                    outlineColor = new Color(157, 248, 234);
                    cooldownColorStart = new Color(111, 169, 241);
                    cooldownColorEnd = new Color(111, 169, 241);
                    break;
            }
        }
    }
}