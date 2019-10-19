using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using CalamityMod.Items.Materials;
using Terraria.ID;

namespace CalamityMod.Items.Weapons.Ranged
{
    public class AstrealDefeat : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Astreal Defeat");
            Tooltip.SetDefault("Fires Astreal Arrows that emit flames as they travel\n" +
                       "Ethereal bow of the tyrant king's mother\n" +
                       "The mother strongly discouraged acts of violence throughout her life\n" +
                       "Though she kept this bow close to protect her family in times of great disaster");
        }

        public override void SetDefaults()
        {
            item.damage = 160;
            item.ranged = true;
            item.width = 44;
            item.height = 54;
            item.useTime = 3;
            item.useAnimation = 15;
            item.useStyle = 5;
            item.noMelee = true;
            item.knockBack = 5.5f;
            item.value = Item.buyPrice(1, 20, 0, 0);
            item.rare = 10;
            item.UseSound = SoundID.Item102;
            item.autoReuse = true;
            item.shoot = ModContent.ProjectileType<AstrealArrow>();
            item.shootSpeed = 1f;
            item.useAmmo = 40;
        }

        public override void ModifyTooltips(List<TooltipLine> list)
        {
            foreach (TooltipLine line2 in list)
            {
                if (line2.mod == "Terraria" && line2.Name == "ItemName")
                {
                    line2.overrideColor = new Color(0, 255, 200);
                }
            }
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            Projectile.NewProjectile(position.X, position.Y, speedX, speedY, ModContent.ProjectileType<AstrealArrow>(), (int)(double)damage, knockBack, player.whoAmI, 0f, 0f);
            return false;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.SpiritFlame);
            recipe.AddIngredient(ItemID.ShadowFlameBow);
            recipe.AddIngredient(ModContent.ItemType<GreatbowofTurmoil>());
            recipe.AddIngredient(ModContent.ItemType<BladedgeGreatbow>());
            recipe.AddIngredient(ModContent.ItemType<DarkechoGreatbow>());
            recipe.AddIngredient(ModContent.ItemType<GalacticaSingularity>(), 5);
            recipe.AddIngredient(ItemID.LunarBar, 5);
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
