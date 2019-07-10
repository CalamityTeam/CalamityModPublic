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
	public class PurityAxe : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Axe of Purity");
            Tooltip.SetDefault("Right click to cleanse evil");
		}

	    public override void SetDefaults()
	    {
	        item.damage = 43;
	        item.melee = true;
	        item.width = 58;
	        item.height = 46;
	        item.useTime = 19;
	        item.useAnimation = 19;
	        item.useTurn = true;
	        item.axe = 25;
	        item.useStyle = 1;
	        item.knockBack = 5f;
            item.value = Item.buyPrice(0, 36, 0, 0);
            item.rare = 5;
	        item.UseSound = SoundID.Item1;
	        item.autoReuse = true;
		}

        public override bool AltFunctionUse(Player player)
        {
            return true;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            damage = 0;
            knockBack = 0.0f;
            return true;
        }

        public override bool CanUseItem(Player player)
        {
            if (player.altFunctionUse == 2)
            {
                item.axe = 0;
                item.shoot = ProjectileID.PurificationPowder;
                item.shootSpeed = 12f;
            }
            else
            {
                item.axe = 25;
                item.shoot = 0;
                item.shootSpeed = 0f;
            }
            return base.CanUseItem(player);
        }

        public override float MeleeSpeedMultiplier(Player player)
        {
            if (player.altFunctionUse == 2)
                return 1.6f;
            return 1f;
        }

        public override bool? CanHitNPC(Player player, NPC target)
        {
            if (player.altFunctionUse == 2)
                return false;
            return null;
        }

        public override bool CanHitPvp(Player player, Player target)
        {
            if (player.altFunctionUse == 2)
                return false;
            return true;
        }

        public override void MeleeEffects(Player player, Rectangle hitbox)
	    {
	        if (Main.rand.Next(5) == 0)
	        {
	        	int dust = Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, 58);
	        }
	    }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(null, "FellerofEvergreens");
            recipe.AddIngredient(ItemID.PurificationPowder, 20);
            recipe.AddIngredient(ItemID.PixieDust, 10);
            recipe.AddIngredient(ItemID.CrystalShard, 5);
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
