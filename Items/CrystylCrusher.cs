using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items;

namespace CalamityMod.Items
{
    public class CrystylCrusher : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Crystyl Crusher");
            Tooltip.SetDefault("Gotta dig faster, gotta go deeper");
        }

        public override void SetDefaults()
        {
            item.damage = 255;
            item.melee = true;
            item.width = 70;
            item.height = 70;
            item.useTime = 1;
            item.useAnimation = 30;
            item.useTurn = true;
            item.pick = 5000;
            item.useStyle = 1;
            item.knockBack = 9f;
            item.value = Item.buyPrice(5, 0, 0, 0);
            item.rare = 10;
            item.UseSound = SoundID.Item1;
            item.autoReuse = true;
            item.tileBoost += 50;
			item.GetGlobalItem<CalamityGlobalItem>(mod).postMoonLordRarity = 16;
		}

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(null, "GallantPickaxe");
            recipe.AddIngredient(null, "BlossomPickaxe");
            recipe.AddIngredient(null, "ShadowspecBar", 5);
            recipe.AddTile(null, "DraedonsForge");
            recipe.SetResult(this);
            recipe.AddRecipe();
        }

        public override void MeleeEffects(Player player, Rectangle hitbox)
        {
            if (Main.rand.Next(3) == 0)
            {
                int num307 = Main.rand.Next(3);
                if (num307 == 0)
                {
                    num307 = 173;
                }
                else if (num307 == 1)
                {
                    num307 = 57;
                }
                else
                {
                    num307 = 58;
                }
                int dust = Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, num307);
            }
        }
    }
}