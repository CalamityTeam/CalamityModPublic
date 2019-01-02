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
	public class FeralthornClaymore : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Feralthorn Claymore");
		}

		public override void SetDefaults()
		{
			item.width = 66;
			item.damage = 63;
			item.melee = true;
			item.useAnimation = 13;
			item.useStyle = 1;
			item.useTime = 13;
			item.useTurn = true;
			item.knockBack = 7.25f;
			item.UseSound = SoundID.Item8;
			item.autoReuse = true;
			item.height = 66;
			item.value = 355000;
			item.rare = 6;
		}
	
		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(null, "DraedonBar", 12);
	        recipe.AddTile(TileID.MythrilAnvil);
	        recipe.SetResult(this);
	        recipe.AddRecipe();
		}

        public override void MeleeEffects(Player player, Rectangle hitbox)
	    {
	        if (Main.rand.Next(4) == 0)
	        {
	        	int dust = Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, 44);
	        }
	    }
	    
	    public override void OnHitNPC(Player player, NPC target, int damage, float knockback, bool crit)
	    {
			target.AddBuff(BuffID.Venom, 200);
		}
	}
}
