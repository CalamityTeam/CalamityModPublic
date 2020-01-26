using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables.Ores;
using CalamityMod.Projectiles.Magic;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Magic
{
    public class VividClarity : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Vivid Clarity");
            Tooltip.SetDefault("Fires three randomized beams of elemental energy at the cursor\n" +
							   "On enemy and tile hits, beams either explode into a big flash,\n" +
							   "summon additonal lasers from the sky,\n" +
							   "or split into energy orbs\n" +
                               "High IQ increases the weapon's potential");
            Item.staff[item.type] = true;
        }

        public override void SetDefaults()
        {
            item.damage = 500;
            item.magic = true;
            item.mana = 101;
            item.width = 90;
            item.height = 112;
            item.useAnimation = 21;
            item.useTime = 7;
            item.reuseDelay = item.useAnimation;
            item.useStyle = 5;
            item.noMelee = true;
            item.knockBack = 7.5f;
            item.value = Item.buyPrice(2, 50, 0, 0);
            item.rare = 10;
            item.UseSound = SoundID.Item60;
            item.autoReuse = true;
            item.shoot = ModContent.ProjectileType<VividBeam>();
            item.shootSpeed = 6f;
            item.Calamity().postMoonLordRarity = 15;
        }

        public override Vector2? HoldoutOrigin()
        {
            return new Vector2(20, 20);
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            Vector2 vector2 = player.RotatedRelativePoint(player.MountedCenter, true);
            float num72 = item.shootSpeed;
            float num78 = (float)Main.mouseX + Main.screenPosition.X - vector2.X;
            float num79 = (float)Main.mouseY + Main.screenPosition.Y - vector2.Y;
            float f = Main.rand.NextFloat() * 6.28318548f;
            float value12 = 20f;
            float value13 = 60f;
            Vector2 vector13 = vector2 + f.ToRotationVector2() * MathHelper.Lerp(value12, value13, Main.rand.NextFloat());
            for (int num202 = 0; num202 < 50; num202++)
            {
                vector13 = vector2 + f.ToRotationVector2() * MathHelper.Lerp(value12, value13, Main.rand.NextFloat());
                if (Collision.CanHit(vector2, 0, 0, vector13 + (vector13 - vector2).SafeNormalize(Vector2.UnitX) * 8f, 0, 0))
                {
                    break;
                }
                f = Main.rand.NextFloat() * 6.28318548f;
            }
            Vector2 mouseWorld = Main.MouseWorld;
            Vector2 vector14 = mouseWorld - vector13;
            Vector2 vector15 = new Vector2(num78, num79).SafeNormalize(Vector2.UnitY) * num72;
            vector14 = vector14.SafeNormalize(vector15) * num72;
            vector14 = Vector2.Lerp(vector14, vector15, 0.25f);
            Projectile.NewProjectile(vector13, vector14, type, damage, knockBack, player.whoAmI, 0f, Main.rand.Next(3));
            return false;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<ElementalRay>());
            recipe.AddIngredient(ModContent.ItemType<ArchAmaryllis>());
            recipe.AddIngredient(ModContent.ItemType<AsteroidStaff>());
            recipe.AddIngredient(ModContent.ItemType<UltraLiquidator>());
            recipe.AddIngredient(ModContent.ItemType<PhantasmalFury>());
            recipe.AddIngredient(ModContent.ItemType<ShadowboltStaff>());
            recipe.AddIngredient(ModContent.ItemType<HeliumFlash>());
			recipe.AddIngredient(ModContent.ItemType<AuricBar>(), 4);
			recipe.AddTile(ModContent.TileType<DraedonsForge>());
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
