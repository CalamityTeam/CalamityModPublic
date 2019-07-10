using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Permafrost
{
	public class IcicleArrow : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Icicle Arrow");
			Tooltip.SetDefault("Shatters into shards on impact");
		}

		public override void SetDefaults()
		{
			item.damage = 14;
			item.ranged = true;
            item.consumable = true;
            item.width = 14;
			item.height = 50;
			item.knockBack = 2.5f;
			item.value = Item.buyPrice(0, 0, 0, 15);
            item.rare = 3;
			item.shoot = mod.ProjectileType("IcicleArrow");
            item.shootSpeed = 1.0f;
			item.ammo = AmmoID.Arrow;
            item.maxStack = 999;
		}
    }
}
