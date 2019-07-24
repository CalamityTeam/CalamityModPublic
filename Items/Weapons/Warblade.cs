using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items;

namespace CalamityMod.Items.Weapons
{
    public class Warblade : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Warblade");
			Tooltip.SetDefault("Critical hits cleave enemy armor, reducing their defense by 15 and protection by 25%");
		}

        public override void SetDefaults()
        {
            item.damage = 27;
            item.melee = true;
            item.width = 32;
            item.height = 48;
            item.useTime = 19;
            item.useAnimation = 19;
            item.useTurn = true;
            item.useStyle = 1;
            item.knockBack = 4.25f;
            item.value = Item.buyPrice(0, 1, 0, 0);
            item.rare = 1;
            item.UseSound = SoundID.Item1;
            item.autoReuse = true;
        }

		public override void OnHitNPC(Player player, NPC target, int damage, float knockback, bool crit)
		{
			if (crit)
			{
				target.AddBuff(mod.BuffType("WarCleave"), 450);
			}
		}
	}
}