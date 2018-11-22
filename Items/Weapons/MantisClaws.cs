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
	public class MantisClaws : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Mantis Claws");
		}

		public override void SetDefaults()
		{
			item.width = 26;
			item.damage = 90;
			item.melee = true;
			item.useAnimation = 6;
			item.useStyle = 1;
			item.useTime = 6;
			item.useTurn = true;
			item.knockBack = 7f;
			item.UseSound = SoundID.Item1;
			item.autoReuse = true;
			item.height = 20;
			item.value = 500000;
			item.rare = 8;
		}
		
		public override void MeleeEffects(Player player, Rectangle hitbox)
	    {
	        if (Main.rand.Next(4) == 0)
	        {
	        	int dust = Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, 33);
	        }
	    }

        public override void OnHitNPC(Player player, NPC target, int damage, float knockback, bool crit)
        {
            Projectile.NewProjectile(target.Center.X, target.Center.Y, 0f, 0f, 612, (int)((float)item.damage * player.meleeDamage), knockback, Main.myPlayer);
        }
    }
}
