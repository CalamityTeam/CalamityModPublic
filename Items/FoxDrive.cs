using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items;

namespace CalamityMod.Items
{
    public class FoxDrive : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Fox Drive");
            Tooltip.SetDefault("'It contains 1 file on it'\n'Fox.cs'");
        }

        public override void SetDefaults()
        {
            item.CloneDefaults(ItemID.ZephyrFish);
            item.shoot = mod.ProjectileType("Fox");
            item.buffType = mod.BuffType("Fox");
			item.rare = 10;
			item.expert = true;
        }

        public override void UseStyle(Player player)
        {
            if (player.whoAmI == Main.myPlayer && player.itemTime == 0)
            {
                player.AddBuff(item.buffType, 3600, true);
            }
        }
    }
}