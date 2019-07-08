using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items;
using CalamityMod.World;

namespace CalamityMod.Items.Weapons 
{
	public class Murasama : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Murasama");
			Tooltip.SetDefault("There will be blood!\n" +
                "ID and power-level locked\n" +
				"Prove your strength or have the correct user ID to wield this sword");
		}

		public override void SetDefaults()
		{
			item.width = 72;
			item.damage = 999;
            item.crit += 30;
            item.melee = true;
			item.noMelee = true;
			item.noUseGraphic = true;
			item.channel = true;
			item.useAnimation = 25;
			item.useStyle = 5;
			item.useTime = 5;
			item.knockBack = 6.5f;
			item.autoReuse = false;
			item.height = 78;
            item.value = Item.buyPrice(2, 50, 0, 0);
            item.rare = 10;
            item.shoot = mod.ProjectileType("Murasama");
			item.shootSpeed = 15f;
			item.GetGlobalItem<CalamityGlobalItem>(mod).postMoonLordRarity = 15;
		}

        public override bool Shoot(Player player, ref Microsoft.Xna.Framework.Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            bool yharon = CalamityWorld.downedYharon;
            if (player.name == "Sam" || player.name == "Samuel Rodrigues" || yharon)
            {
                Projectile.NewProjectile(position.X, position.Y, speedX, speedY, type, damage, knockBack, player.whoAmI, 0.0f, 0.0f);
                return false;
            }
            else
            {
                Projectile.NewProjectile(position.X, position.Y, 0f, 0f, 504, 6, 0f, player.whoAmI, 0.0f, 0.0f);
                return false;
            }
        }
    }
}
