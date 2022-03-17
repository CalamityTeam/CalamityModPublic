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
            Item.staff[item.type] = true;
        }

        public override void SetDefaults()
        {
            item.damage = 155;
            item.magic = true;
            item.mana = 42;
            item.width = 90;
            item.height = 112;
            item.useAnimation = 20;
            item.useTime = 4;
            item.reuseDelay = item.useAnimation;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.noMelee = true;
            item.knockBack = 7.5f;
            item.value = CalamityGlobalItem.Rarity15BuyPrice;
            item.rare = ItemRarityID.Red;
            item.UseSound = SoundID.Item60;
            item.autoReuse = true;
            item.shoot = ModContent.ProjectileType<VividBeam>();
            item.shootSpeed = 6f;
            item.Calamity().customRarity = CalamityRarity.Violet;
        }

        public override Vector2? HoldoutOrigin()
        {
            return new Vector2(20, 20);
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            Vector2 playerPos = player.RotatedRelativePoint(player.MountedCenter, true);
            float speed = item.shootSpeed;
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
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<ElementalRay>());
            recipe.AddIngredient(ModContent.ItemType<ThornBlossom>());
            recipe.AddIngredient(ModContent.ItemType<AsteroidStaff>());
            recipe.AddIngredient(ModContent.ItemType<UltraLiquidator>());
            recipe.AddIngredient(ModContent.ItemType<PhantasmalFury>());
            recipe.AddIngredient(ModContent.ItemType<ShadowboltStaff>());
            recipe.AddIngredient(ModContent.ItemType<MiracleMatter>());
            recipe.AddTile(ModContent.TileType<DraedonsForge>());
            recipe.SetResult(this);
            recipe.AddRecipe();
            recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<ElementalRay>());
            recipe.AddIngredient(ModContent.ItemType<ThePrince>());
            recipe.AddIngredient(ModContent.ItemType<AsteroidStaff>());
            recipe.AddIngredient(ModContent.ItemType<UltraLiquidator>());
            recipe.AddIngredient(ModContent.ItemType<PhantasmalFury>());
            recipe.AddIngredient(ModContent.ItemType<ShadowboltStaff>());
            recipe.AddIngredient(ModContent.ItemType<MiracleMatter>());
            recipe.AddTile(ModContent.TileType<DraedonsForge>());
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
