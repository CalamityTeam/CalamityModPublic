using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items;

namespace CalamityMod.Items.Calamitas
{
    public class CalamityDust : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Ashes of Calamity");
        }

        public override void SetDefaults()
        {
            item.width = 26;
            item.height = 20;
            item.maxStack = 999;
			item.value = Item.buyPrice(0, 4, 50, 0);
			item.rare = 7;
        }

        public override void Update(ref float gravity, ref float maxFallSpeed)
        {
            float num = (float)Main.rand.Next(90, 111) * 0.01f;
            num *= Main.essScale;
            Lighting.AddLight((int)((item.position.X + (float)(item.width / 2)) / 16f), (int)((item.position.Y + (float)(item.height / 2)) / 16f), 0.25f * num, 0.05f * num, 0.25f * num);
        }
    }
}