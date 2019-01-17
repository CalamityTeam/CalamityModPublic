using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Astral
{
	public class AstralStaff : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Astral Staff");
			Tooltip.SetDefault("Summons a large crystal from the sky that has a large area of effect on impact.");
            Item.staff[item.type] = true;
		}

	    public override void SetDefaults()
	    {
	        item.damage = 180;
	        item.crit += 15;
	        item.magic = true;
	        item.mana = 26;
	        item.width = 86;
	        item.height = 72;
	        item.useTime = 35;
	        item.useAnimation = 35;
	        item.useStyle = 5;
	        item.noMelee = true;
	        item.knockBack = 5f;
            item.value = Item.buyPrice(0, 60, 0, 0);
            item.rare = 7;
	        item.UseSound = SoundID.Item105;
	        item.autoReuse = true;
	        item.shoot = mod.ProjectileType("AstralCrystal");
	        item.shootSpeed = 15f;
	    }

        public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
	        recipe.AddIngredient(null, "AstralBar", 6);
	        recipe.AddTile(TileID.MythrilAnvil);
	        recipe.SetResult(this);
	        recipe.AddRecipe();
		}

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            Vector2 spawnPos = new Vector2(player.MountedCenter.X + Main.rand.Next(-200, 201), player.MountedCenter.Y - 600f);
            Vector2 targetPos = Main.MouseWorld + new Vector2(Main.rand.Next(-30, 31), Main.rand.Next(-30, 31));
            Vector2 velocity = targetPos - spawnPos;
            velocity.Normalize();
            velocity *= 13f;

            int p = Projectile.NewProjectile(spawnPos, velocity, type, damage, knockBack, player.whoAmI);
            Main.projectile[p].ai[0] = targetPos.Y - 120;

            return false;
		}
	}
}