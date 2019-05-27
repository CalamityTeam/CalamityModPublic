using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items;
using CalamityMod.Projectiles;

namespace CalamityMod.Items.Weapons
{
    public class TheSwarmer : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("The Swarmer");
        }

        public override void SetDefaults()
        {
            item.damage = 30;
            item.magic = true;
            item.mana = 7;
            item.width = 74;
            item.height = 36;
            item.useTime = 8;
            item.useAnimation = 8;
            item.useStyle = 5;
            item.noMelee = true;
            item.value = Item.buyPrice(0, 95, 0, 0);
            item.rare = 9;
            item.UseSound = SoundID.Item11;
            item.autoReuse = true;
            item.shoot = 189;
            item.shootSpeed = 12f;
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-15, -5);
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.FragmentVortex, 20);
            recipe.AddIngredient(ItemID.BeeGun);
            recipe.AddIngredient(ItemID.WaspGun);
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }

        public override bool Shoot(Player player, ref Microsoft.Xna.Framework.Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            for (int i = 0; i <= 3; i++)
            {
                float SpeedX = speedX + (float)Main.rand.Next(-35, 36) * 0.05f;
                float SpeedY = speedY + (float)Main.rand.Next(-35, 36) * 0.05f;
                int wasps = Projectile.NewProjectile(position.X, position.Y, SpeedX, SpeedY, type, damage, 0f, player.whoAmI, 0.0f, 0.0f);
                Main.projectile[wasps].penetrate = 1;
				Main.projectile[wasps].GetGlobalProjectile<CalamityGlobalProjectile>(mod).forceMagic = true;
			}
            for (int i = 0; i <= 3; i++)
            {
                float SpeedX2 = speedX + (float)Main.rand.Next(-35, 36) * 0.05f;
                float SpeedY2 = speedY + (float)Main.rand.Next(-35, 36) * 0.05f;
                int bees = Projectile.NewProjectile(position.X, position.Y, SpeedX2, SpeedY2, player.beeType(), player.beeDamage(item.damage), player.beeKB(0f), player.whoAmI, 0.0f, 0.0f);
                Main.projectile[bees].penetrate = 1;
				Main.projectile[bees].GetGlobalProjectile<CalamityGlobalProjectile>(mod).forceMagic = true;
			}
            return false;
        }
    }
}