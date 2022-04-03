using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Magic;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Microsoft.Xna.Framework;
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
            Tooltip.SetDefault("Fires five randomized beams of elemental energy at the cursor\n" +
                               "On enemy and tile hits, beams either explode into a big flash,\n" +
                               "summon an additonal laser from the sky,\n" +
                               "or split into energy orbs\n" +
                               "Its majesty inspires a stroke of unparalleled genius");
            Item.staff[Item.type] = true;
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
            Item.rare = ItemRarityID.Red;
            Item.UseSound = SoundID.Item60;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<VividBeam>();
            Item.shootSpeed = 6f;
            Item.Calamity().customRarity = CalamityRarity.Violet;
        }

        public override Vector2? HoldoutOrigin()
        {
            return new Vector2(20, 20);
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
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
            Vector2 velocity = Main.MouseWorld - source;
            Vector2 velocityVariation = new Vector2(xPos, yPos).SafeNormalize(Vector2.UnitY) * speed;
            velocity = velocity.SafeNormalize(velocityVariation) * speed;
            velocity = Vector2.Lerp(velocity, velocityVariation, 0.25f);
            Projectile.NewProjectile(source, velocity, type, damage, knockBack, player.whoAmI, 0f, Main.rand.Next(3));
            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe(1).AddIngredient(ModContent.ItemType<ElementalRay>()).AddIngredient(ModContent.ItemType<ThornBlossom>()).AddIngredient(ModContent.ItemType<AsteroidStaff>()).AddIngredient(ModContent.ItemType<UltraLiquidator>()).AddIngredient(ModContent.ItemType<PhantasmalFury>()).AddIngredient(ModContent.ItemType<ShadowboltStaff>()).AddIngredient(ModContent.ItemType<MiracleMatter>()).AddTile(ModContent.TileType<DraedonsForge>()).Register();
            CreateRecipe(1).AddIngredient(ModContent.ItemType<ElementalRay>()).AddIngredient(ModContent.ItemType<ThePrince>()).AddIngredient(ModContent.ItemType<AsteroidStaff>()).AddIngredient(ModContent.ItemType<UltraLiquidator>()).AddIngredient(ModContent.ItemType<PhantasmalFury>()).AddIngredient(ModContent.ItemType<ShadowboltStaff>()).AddIngredient(ModContent.ItemType<MiracleMatter>()).AddTile(ModContent.TileType<DraedonsForge>()).Register();
        }
    }
}
