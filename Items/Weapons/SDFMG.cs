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
    public class SDFMG : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("SDFMG");
            Tooltip.SetDefault("It came from the edge of Terraria\n50% chance to not consume ammo");
        }

        public override void SetDefaults()
        {
            item.damage = 250;
            item.ranged = true;
            item.width = 74;
            item.height = 34;
            item.crit += 16;
            item.useTime = 2;
            item.useAnimation = 4;
            item.useStyle = 5;
            item.noMelee = true;
            item.knockBack = 2.75f;
            item.value = 3000000;
            item.UseSound = SoundID.Item11;
            item.autoReuse = true;
            item.shoot = 10;
            item.shootSpeed = 16f;
            item.useAmmo = 97;
        }

        public override void ModifyTooltips(List<TooltipLine> list)
        {
            foreach (TooltipLine line2 in list)
            {
                if (line2.mod == "Terraria" && line2.Name == "ItemName")
                {
                    line2.overrideColor = new Color(43, 96, 222);
                }
            }
        }

        public override bool Shoot(Player player, ref Microsoft.Xna.Framework.Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            float SpeedX = speedX + (float)Main.rand.Next(-5, 6) * 0.05f;
            float SpeedY = speedY + (float)Main.rand.Next(-5, 6) * 0.05f;
            if (Main.rand.Next(5) == 0)
            {
                Projectile.NewProjectile(position.X, position.Y, SpeedX, SpeedY, mod.ProjectileType("FishronRPG"), damage, knockBack, player.whoAmI, 0.0f, 0.0f);
            }
            Projectile.NewProjectile(position.X, position.Y, SpeedX, SpeedY, type, damage, knockBack, player.whoAmI, 0.0f, 0.0f);
            return false;
        }

        public override bool ConsumeAmmo(Player player)
        {
            if (Main.rand.Next(0, 100) <= 50)
                return false;
            return true;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.SDMG);
            recipe.AddIngredient(ItemID.ShrimpyTruffle);
            recipe.AddIngredient(null, "CosmiliteBar", 4);
            recipe.AddIngredient(null, "Phantoplasm", 4);
            recipe.AddIngredient(null, "DepthCells", 10);
            recipe.AddIngredient(null, "Lumenite", 10);
            recipe.AddIngredient(null, "Tenebris", 5);
            recipe.AddTile(null, "DraedonsForge");
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}