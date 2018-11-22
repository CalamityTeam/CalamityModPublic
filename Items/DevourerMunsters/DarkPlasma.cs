using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items;

namespace CalamityMod.Items.DevourerMunsters
{
    public class DarkPlasma : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Dark Plasma");
            Main.RegisterItemAnimation(item.type, new DrawAnimationVertical(7, 8));
        }

        public override void SetDefaults()
        {
            item.width = 15;
            item.height = 12;
            item.maxStack = 999;
            item.value = 508750;
        }

        public override void ModifyTooltips(List<TooltipLine> list)
        {
            foreach (TooltipLine line2 in list)
            {
                if (line2.mod == "Terraria" && line2.Name == "ItemName")
                {
                    line2.overrideColor = new Color(0, 255, 0);
                }
            }
        }

        public override void Update(ref float gravity, ref float maxFallSpeed)
        {
            maxFallSpeed = 0f;
            float num = (float)Main.rand.Next(90, 111) * 0.01f;
            num *= Main.essScale;
            Lighting.AddLight((int)((item.position.X + (float)(item.width / 2)) / 16f), (int)((item.position.Y + (float)(item.height / 2)) / 16f), 0f * num, 0.45f * num, 0.7f * num);
        }
    }
}