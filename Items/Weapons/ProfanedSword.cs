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
			Tooltip.SetDefault("Summons brimstone geysers on enemy hits\n" +
				"Right click to throw like a javelin");
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
            item.value = Item.buyPrice(0, 48, 0, 0);
            item.rare = 6;
            item.UseSound = SoundID.Item1;
            item.autoReuse = true;
			item.shoot = mod.ProjectileType("NobodyKnows");
			item.shootSpeed = 20f;
		}

		public override bool AltFunctionUse(Player player)
		{
			return true;
		}

		public override bool CanUseItem(Player player)
		{
			if (player.altFunctionUse == 2)
			{
				item.noMelee = true;
				item.noUseGraphic = true;
			}
			else
			{
				item.noMelee = false;
				item.noUseGraphic = false;
			}
			return base.CanUseItem(player);
		}

		public override bool Shoot(Player player, ref Microsoft.Xna.Framework.Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
		{
			if (player.altFunctionUse == 2)
			{
				Projectile.NewProjectile(position.X, position.Y, speedX, speedY, mod.ProjectileType("ProfanedSword"), damage, knockBack, player.whoAmI, 0f, 0f);
			}
			return false;
		}

		public override void OnHitNPC(Player player, NPC target, int damage, float knockback, bool crit)
        {
        	target.AddBuff(mod.BuffType("BrimstoneFlames"), 120);
            Projectile.NewProjectile(target.Center.X, target.Center.Y, 0f, 0f, mod.ProjectileType("Brimblast"), (int)((float)item.damage * player.meleeDamage), knockback, Main.myPlayer);
        }
        
        public override void MeleeEffects(Player player, Rectangle hitbox)
        {
            if (Main.rand.Next(4) == 0)
            {
                Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, 235);
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