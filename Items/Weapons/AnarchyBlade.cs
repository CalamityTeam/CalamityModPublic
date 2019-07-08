using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.NPCs;

namespace CalamityMod.Items.Weapons 
{
	public class AnarchyBlade : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Anarchy Blade");
			Tooltip.SetDefault("The lower your life the more damage this blade does\n" +
				"Your hits will generate a large explosion\n" +
				"If you're below 50% life your hits have a chance to instantly kill regular enemies");
		}

		public override void SetDefaults()
		{
			item.width = 98;
			item.damage = 110;
			item.melee = true;
			item.useAnimation = 19;
			item.useTime = 19;
			item.useTurn = true;
			item.useStyle = 1;
			item.knockBack = 7.5f;
			item.UseSound = SoundID.Item1;
			item.autoReuse = true;
			item.height = 98;
            item.value = Item.buyPrice(0, 80, 0, 0);
            item.rare = 8;
		}
		
		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(null, "UnholyCore", 5);
	        recipe.AddIngredient(null, "CoreofChaos", 3);
	        recipe.AddIngredient(ItemID.BrokenHeroSword);
	        recipe.AddTile(TileID.MythrilAnvil);
	        recipe.SetResult(this);
	        recipe.AddRecipe();
		}
	
	    public override void MeleeEffects(Player player, Rectangle hitbox)
	    {
	        if (Main.rand.Next(3) == 0)
	        {
	        	int dust = Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, 235);
	        }
	    }
	    
	    public override void GetWeaponDamage(Player player, ref int damage)
	    {
	    	int lifeAmount = player.statLifeMax2 - player.statLife;
	    	float damageAdd = (((float)lifeAmount * 0.1f) + (float)item.damage);
	    	damage = (int)(damageAdd * player.meleeDamage);
	    }
	    
	    public override void OnHitNPC(Player player, NPC target, int damage, float knockback, bool crit)
	    {
	    	Projectile.NewProjectile(target.Center.X, target.Center.Y, 0f, 0f, mod.ProjectileType("BrimstoneBoom"), (int)((float)item.damage * player.meleeDamage), knockback, Main.myPlayer);
	    	target.AddBuff(mod.BuffType("BrimstoneFlames"), 300);

	    	if (player.statLife < (player.statLifeMax2 * 0.5f) && Main.rand.Next(5) == 0)
	    	{
				if (!CalamityPlayer.areThereAnyDamnBosses && CalamityGlobalNPC.ShouldAffectNPC(target))
				{
					target.life = 0;
					target.HitEffect(0, 10.0);
					target.active = false;
					target.NPCLoot();
				}
	    	}
		}
	}
}
