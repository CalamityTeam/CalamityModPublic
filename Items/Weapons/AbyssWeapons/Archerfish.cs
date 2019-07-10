using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items;

namespace CalamityMod.Items.Weapons.AbyssWeapons
{
	public class Archerfish : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Archerfish");
			Tooltip.SetDefault("Fires a stream of water");
		}

	    public override void SetDefaults()
	    {
	        item.damage = 23;
	        item.ranged = true;
	        item.width = 78;
	        item.height = 36;
	        item.useTime = 10;
	        item.useAnimation = 10;
	        item.useStyle = 5;
	        item.noMelee = true;
	        item.knockBack = 2f;
            item.value = Item.buyPrice(0, 4, 0, 0);
            item.rare = 3;
	        item.UseSound = SoundID.Item11;
	        item.autoReuse = true;
	        item.shoot = mod.ProjectileType("Archerfish");
	        item.shootSpeed = 11f;
	        item.useAmmo = 97;
	    }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-5, 0);
        }

        public override bool Shoot(Player player, ref Microsoft.Xna.Framework.Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
		    Projectile.NewProjectile(position.X, position.Y, speedX, speedY, mod.ProjectileType("Archerfish"), damage, knockBack, player.whoAmI, 0f, 0f);
		    return false;
		}
	}
}
