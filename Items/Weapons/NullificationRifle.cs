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
	public class NullificationRifle : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Nullification Pistol");
			Tooltip.SetDefault("Is it nullable or not? Let's find out!\n" +
				"Fires a fast null bullet that distorts NPC stats\n" +
				"Uses your life as ammo");
		}

	    public override void SetDefaults()
	    {
	        item.damage = 135;
	        item.ranged = true;
	        item.width = 64;
	        item.height = 30;
	        item.useTime = 20;
	        item.useAnimation = 20;
	        item.useStyle = 5;
	        item.noMelee = true;
	        item.knockBack = 7f;
            item.value = Item.buyPrice(0, 80, 0, 0);
            item.rare = 8;
	        item.UseSound = SoundID.Item33;
	        item.autoReuse = true;
	        item.shootSpeed = 25f;
	        item.shoot = mod.ProjectileType("NullShot");
	    }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-5, 0);
        }

        public override bool Shoot(Player player, ref Microsoft.Xna.Framework.Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
		{
	    	player.statLife -= 5;
			if (Main.myPlayer == player.whoAmI)
			{
				player.HealEffect(-5, true);
			}
			if (player.statLife <= 0)
			{
				player.KillMe(PlayerDeathReason.ByOther(10), 1000.0, 0, false);
			}
	    	Projectile.NewProjectile(position.X, position.Y, speedX, speedY, mod.ProjectileType("NullShot"), damage, knockBack, player.whoAmI, 0.0f, 0.0f);
	    	return false;
		}
	}
}