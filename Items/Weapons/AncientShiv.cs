using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons 
{
	public class AncientShiv : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Ancient Shiv");
			Tooltip.SetDefault("Enemies release a blue aura cloud on death");
		}

		public override void SetDefaults()
		{
			item.useStyle = 3;
			item.useTurn = false;
			item.useAnimation = 12;
			item.useTime = 12;
			item.width = 30;
			item.height = 30;
			item.damage = 35;
			item.melee = true;
			item.knockBack = 6f;
			item.UseSound = SoundID.Item1;
			item.useTurn = true;
			item.autoReuse = true;
            item.value = Item.buyPrice(0, 4, 0, 0);
            item.rare = 3;
		}
	
	    public override void MeleeEffects(Player player, Rectangle hitbox)
	    {
	        if (Main.rand.Next(5) == 0)
	        {
	        	int dust = Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, 15);
	        }
	    }
	    
	    public override void OnHitNPC(Player player, NPC target, int damage, float knockback, bool crit)
	    {
	    	if (target.life <= 0)
	    	{
	    		Projectile.NewProjectile(target.Center.X, target.Center.Y, 0f, 0f, mod.ProjectileType("BlueAura"), (int)((float)item.damage * player.meleeDamage), knockback, Main.myPlayer);
	    	}
		}
	}
}
