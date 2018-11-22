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
			item.useAnimation = 19;
			item.useStyle = 1;
			item.useTime = 19;
			item.useTurn = true;
			item.knockBack = 7.25f;
			item.UseSound = SoundID.Item8;
			item.autoReuse = true;
			item.height = 66;
			item.value = 355000;
			item.rare = 6;
			item.shoot = 8;
			item.shootSpeed = 16f;
		}
	
		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(null, "DraedonBar", 12);
	        recipe.AddTile(TileID.MythrilAnvil);
	        recipe.SetResult(this);
	        recipe.AddRecipe();
		}

        public override bool Shoot(Player player, ref Microsoft.Xna.Framework.Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            int thorn = Projectile.NewProjectile(position.X, position.Y, speedX, speedY, type, damage, knockBack, player.whoAmI, 0.0f, 0.0f);
            Main.projectile[thorn].magic = false;
            Main.projectile[thorn].melee = true;
            return false;
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
