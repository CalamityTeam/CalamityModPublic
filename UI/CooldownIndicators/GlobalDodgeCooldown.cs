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
}