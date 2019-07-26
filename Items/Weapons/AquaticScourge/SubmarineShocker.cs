using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.AquaticScourge
{
	public class SubmarineShocker : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Submarine Shocker");
			Tooltip.SetDefault("Enemies release electric sparks on hit");
		}

		public override void SetDefaults()
		{
			item.useStyle = 3;
			item.useTurn = false;
			item.useAnimation = 10;
			item.useTime = 10;
			item.width = 32;
			item.height = 32;
			item.damage = 70;
			item.melee = true;
			item.knockBack = 7f;
			item.UseSound = SoundID.Item1;
			item.useTurn = true;
			item.autoReuse = true;
            item.value = Item.buyPrice(0, 36, 0, 0);
            item.rare = 5;
		}

	    public override void MeleeEffects(Player player, Rectangle hitbox)
	    {
	        if (Main.rand.Next(5) == 0)
	        {
	        	int dust = Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, 226);
	        }
	    }

	    public override void OnHitNPC(Player player, NPC target, int damage, float knockback, bool crit)
	    {
	    	Projectile.NewProjectile(target.Center.X, target.Center.Y, 0f, 0f, mod.ProjectileType("Spark"), (int)((float)item.damage * player.meleeDamage), knockback, Main.myPlayer);
		}
	}
}
