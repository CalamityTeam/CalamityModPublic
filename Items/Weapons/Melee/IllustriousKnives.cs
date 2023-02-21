using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Melee;
using CalamityMod.Rarities;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Melee
{
    [LegacyName("RoyalKnives", "RoyalKnivesMelee", "RoyalKnivesRogue")]
    public class IllustriousKnives : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Illustrious Knives");
            Tooltip.SetDefault("Throws a flurry of homing knives that can heal the user");
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 44;
            Item.damage = 400;
            Item.DamageType = DamageClass.MeleeNoSpeed;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.useAnimation = 10;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTime = 10;
            Item.knockBack = 3f;
            Item.UseSound = SoundID.Item39;
            Item.autoReuse = true;
            Item.height = 62;

            Item.value = CalamityGlobalItem.Rarity16BuyPrice;
            Item.rare = ModContent.RarityType<HotPink>();
            Item.Calamity().devItem = true;

            Item.shoot = ModContent.ProjectileType<IllustriousKnife>();
            Item.shootSpeed = 9f;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            float num72 = Item.shootSpeed;
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
            if (Main.rand.NextBool(6))
            {
                num146++;
            }
            if (Main.rand.NextBool(8))
            {
                num146++;
            }
            for (int num147 = 0; num147 < num146; num147++)
            {
                float num148 = num78;
                float num149 = num79;
                float num150 = 0.05f * (float)num147;
                num148 += (float)Main.rand.Next(-35, 36) * num150;
                num149 += (float)Main.rand.Next(-35, 36) * num150;
                num80 = (float)Math.Sqrt((double)(num148 * num148 + num149 * num149));
                num80 = num72 / num80;
                num148 *= num80;
                num149 *= num80;
                float x4 = vector2.X;
                float y4 = vector2.Y;
                Projectile.NewProjectile(source, x4, y4, num148, num149, type, damage, knockback, player.whoAmI, 0f, 0f);
            }
            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<EmpyreanKnives>().
                AddIngredient<CoreofCalamity>(2).
                AddIngredient<ShadowspecBar>(5).
                AddTile<DraedonsForge>().
                Register();
        }
    }
}
