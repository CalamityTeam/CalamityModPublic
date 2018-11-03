using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items;
//using TerrariaOverhaul;

namespace CalamityMod.Items.Weapons 
{
	public class BrimstoneFury : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Brimstone Fury");
		}

	    public override void SetDefaults()
	    {
	        item.damage = 17;
	        item.ranged = true;
	        item.width = 30;
	        item.height = 58;
	        item.useTime = 22;
	        item.useAnimation = 22;
	        item.useStyle = 5;
	        item.noMelee = true;
	        item.knockBack = 3.75f;
	        item.value = 300000;
	        item.rare = 6;
	        item.UseSound = SoundID.Item5;
	        item.autoReuse = true;
	        item.shoot = mod.ProjectileType("BrimstoneBolt");
	        item.shootSpeed = 13f;
	        item.useAmmo = 40;
	    }

        /*public void OverhaulInit()
        {
            this.SetTag("bow");
        }*/

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
		{
            int numProj = 2;
            float rotation = MathHelper.ToRadians(9);
            for (int i = 0; i < numProj + 1; i++)
            {
                Vector2 perturbedSpeed = new Vector2(speedX, speedY).RotatedBy(MathHelper.Lerp(-rotation, rotation, i / (numProj - 1)));
                Projectile.NewProjectile(position.X, position.Y, perturbedSpeed.X, perturbedSpeed.Y, mod.ProjectileType("BrimstoneBolt"), damage, knockBack, player.whoAmI, 0f, 0f);
            }
            return false;
		}
	
	    public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(null, "UnholyCore", 5);
	        recipe.AddTile(TileID.MythrilAnvil);
	        recipe.SetResult(this);
	        recipe.AddRecipe();
		}
	}
}