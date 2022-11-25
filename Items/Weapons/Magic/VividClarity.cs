using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Magic;
using CalamityMod.Rarities;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Magic
{
    public class VividClarity : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Vivid Clarity");
            Tooltip.SetDefault("Fires five randomized beams of elemental energy at the cursor\n" +
                               "On enemy and tile hits, beams either explode into a big flash,\n" +
                               "summon an additonal laser from the sky,\n" +
                               "or split into energy orbs\n" +
                               "Its majesty inspires a stroke of unparalleled genius");
            Item.staff[Item.type] = true;
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.damage = 155;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 42;
            Item.width = 90;
            Item.height = 112;
            Item.useAnimation = 20;
            Item.useTime = 4;
            Item.reuseDelay = Item.useAnimation;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 7.5f;
            Item.value = CalamityGlobalItem.Rarity15BuyPrice;
            Item.UseSound = SoundID.Item60;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<VividBeam>();
            Item.shootSpeed = 6f;
            Item.rare = ModContent.RarityType<Violet>();
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo projSource, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Vector2 playerPos = player.RotatedRelativePoint(player.MountedCenter, true);
            float speed = Item.shootSpeed;
            float xPos = (float)Main.mouseX + Main.screenPosition.X - playerPos.X;
            float yPos = (float)Main.mouseY + Main.screenPosition.Y - playerPos.Y;
            float f = Main.rand.NextFloat() * MathHelper.TwoPi;
            float sourceVariationLow = 20f;
            float sourceVariationHigh = 60f;
            Vector2 source = playerPos + f.ToRotationVector2() * MathHelper.Lerp(sourceVariationLow, sourceVariationHigh, Main.rand.NextFloat());
            for (int num202 = 0; num202 < 50; num202++)
            {
                source = playerPos + f.ToRotationVector2() * MathHelper.Lerp(sourceVariationLow, sourceVariationHigh, Main.rand.NextFloat());
                if (Collision.CanHit(playerPos, 0, 0, source + (source - playerPos).SafeNormalize(Vector2.UnitX) * 8f, 0, 0))
                {
                    break;
                }
                f = Main.rand.NextFloat() * MathHelper.TwoPi;
            }
            Vector2 velocityReal = Main.MouseWorld - source;
            Vector2 velocityVariation = new Vector2(xPos, yPos).SafeNormalize(Vector2.UnitY) * speed;
            velocityReal = velocityReal.SafeNormalize(velocityVariation) * speed;
            velocityReal = Vector2.Lerp(velocityReal, velocityVariation, 0.25f);
            Projectile.NewProjectile(projSource, source, velocityReal, type, damage, knockback, player.whoAmI, 0f, Main.rand.Next(3));
            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<ElementalRay>().
                AddIngredient<PhantasmalFury>().
                AddIngredient<ShadowboltStaff>().
                AddIngredient<MiracleMatter>().
                AddTile<DraedonsForge>().
                Register();
        }
    }
}
