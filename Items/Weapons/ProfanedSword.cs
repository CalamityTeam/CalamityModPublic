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
    public class ProfanedSword : ModItem
    {
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Brimstone Sword");
			Tooltip.SetDefault("Summons brimstone geysers on enemy hits");
		}

        public override void SetDefaults()
        {
            item.damage = 61;
            item.melee = true;
            item.width = 42;
            item.height = 50;
            item.useTime = 23;
            item.useAnimation = 23;
			item.useTurn = true;
            item.useStyle = 1;
            item.knockBack = 7.5f;
            item.value = 300000;
            item.rare = 6;
            item.UseSound = SoundID.Item1;
            item.autoReuse = true;
        }
        
        public override void OnHitNPC(Player player, NPC target, int damage, float knockback, bool crit)
        {
        	target.AddBuff(mod.BuffType("BrimstoneFlames"), 100);
            Projectile.NewProjectile(target.Center.X, target.Center.Y, 0f, 0f, mod.ProjectileType("Brimblast"), (int)((float)item.damage * player.meleeDamage), knockback, Main.myPlayer);
        }
        
        public override void MeleeEffects(Player player, Rectangle hitbox)
        {
            if (Main.rand.Next(4) == 0)
            {
                int dust = Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, 235);
            }
        }
        
        public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(null, "UnholyCore", 6);
	        recipe.AddTile(TileID.MythrilAnvil);
	        recipe.SetResult(this);
	        recipe.AddRecipe();
		}
    }
}