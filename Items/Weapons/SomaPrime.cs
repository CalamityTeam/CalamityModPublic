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
    public class SomaPrime : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Soma Prime");
        }

        public override void SetDefaults()
        {
            item.damage = 600;
            item.ranged = true;
            item.width = 94;
            item.height = 34;
            item.crit += 40;
            item.useTime = 1;
            item.useAnimation = 3;
            item.useStyle = 5;
            item.noMelee = true;
            item.knockBack = 2f;
            item.value = Item.buyPrice(5, 0, 0, 0);
            item.rare = 10;
            item.UseSound = SoundID.Item40;
            item.autoReuse = true;
            item.shoot = mod.ProjectileType("SlashRound");
            item.shootSpeed = 30f;
            item.useAmmo = 97;
			item.GetGlobalItem<CalamityGlobalItem>(mod).postMoonLordRarity = 16;
		}

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-25, 0);
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(null, "ShadowspecBar", 5);
            recipe.AddIngredient(null, "P90");
            recipe.AddIngredient(null, "Minigun");
            recipe.AddTile(null, "DraedonsForge");
            recipe.SetResult(this);
            recipe.AddRecipe();
        }

        public override bool Shoot(Player player, ref Microsoft.Xna.Framework.Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            float SpeedX = speedX + (float)Main.rand.Next(-10, 11) * 0.05f;
            float SpeedY = speedY + (float)Main.rand.Next(-10, 11) * 0.05f;
            Projectile.NewProjectile(position.X, position.Y, SpeedX, SpeedY, mod.ProjectileType("SlashRound"), damage, knockBack, player.whoAmI, 0.0f, 0.0f);
            return false;
        }

        public override bool ConsumeAmmo(Player player)
        {
            if (Main.rand.Next(0, 100) < 50)
                return false;
            return true;
        }
    }
}