using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Melee;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Melee
{
    public class EmpyreanKnives : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Empyrean Knives");
            Tooltip.SetDefault("Throws a flurry of bouncing knives that can heal the user");
        }

        public override void SetDefaults()
        {
            item.width = 18;
            item.damage = 230;
            item.melee = true;
            item.noMelee = true;
            item.noUseGraphic = true;
            item.useAnimation = 15;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.useTime = 15;
            item.knockBack = 3f;
            item.UseSound = SoundID.Item39;
            item.autoReuse = true;
            item.height = 20;
            item.value = CalamityGlobalItem.Rarity14BuyPrice;
            item.rare = ItemRarityID.Purple;
            item.Calamity().customRarity = CalamityRarity.DarkBlue;
            item.shoot = ModContent.ProjectileType<EmpyreanKnife>();
            item.shootSpeed = 15f;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            float num72 = item.shootSpeed;
            Vector2 vector2 = player.RotatedRelativePoint(player.MountedCenter, true);
            Vector2 value = Vector2.UnitX.RotatedBy((double)player.fullRotation, default);
            Vector2 vector3 = Main.MouseWorld - vector2;
            float num78 = (float)Main.mouseX + Main.screenPosition.X - vector2.X;
            float num79 = (float)Main.mouseY + Main.screenPosition.Y - vector2.Y;
            if (player.gravDir == -1f)
            {
                num79 = Main.screenPosition.Y + (float)Main.screenHeight - (float)Main.mouseY - vector2.Y;
            }
            float num80 = (float)Math.Sqrt((double)(num78 * num78 + num79 * num79));
            if ((float.IsNaN(num78) && float.IsNaN(num79)) || (num78 == 0f && num79 == 0f))
            {
                num78 = (float)player.direction;
                num79 = 0f;
                num80 = num72;
            }
            else
            {
                num80 = num72 / num80;
            }
            num78 *= num80;
            num79 *= num80;
            int num146 = 4;
            if (Main.rand.NextBool(2))
            {
                num146++;
            }
            if (Main.rand.NextBool(4))
            {
                num146++;
            }
            if (Main.rand.NextBool(8))
            {
                num146++;
            }
            if (Main.rand.NextBool(16))
            {
                num146++;
            }
            for (int num147 = 0; num147 < num146; num147++)
            {
                float num148 = num78;
                float num149 = num79;
                float num150 = 0.05f * (float)num147;
                num148 += (float)Main.rand.Next(-25, 26) * num150;
                num149 += (float)Main.rand.Next(-25, 26) * num150;
                num80 = (float)Math.Sqrt((double)(num148 * num148 + num149 * num149));
                num80 = num72 / num80;
                num148 *= num80;
                num149 *= num80;
                float x4 = vector2.X;
                float y4 = vector2.Y;
                Projectile.NewProjectile(x4, y4, num148, num149, type, damage, knockBack, player.whoAmI, 0f, 0f);
            }
            return false;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.VampireKnives);
            recipe.AddIngredient(ModContent.ItemType<MonstrousKnives>());
            recipe.AddIngredient(ModContent.ItemType<CosmiliteBar>(), 8);
            recipe.AddIngredient(ModContent.ItemType<DarksunFragment>(), 8);
            recipe.AddTile(ModContent.TileType<CosmicAnvil>());
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
