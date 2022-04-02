using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables;
using CalamityMod.Projectiles.Magic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Magic
{
    public class UltraLiquidator : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Ultra Liquidator");
            Tooltip.SetDefault("Summons liquidation blades that summon more blades on enemy hits\n" +
                               "The blades inflict ichor, cursed inferno and brimstone flames");
            Item.staff[item.type] = true;
        }

        public override void SetDefaults()
        {
            item.damage = 120;
            item.knockBack = 5f;
            item.useTime = 3;
            item.reuseDelay = item.useAnimation = 15;
            item.mana = 25;
            item.magic = true;
            item.autoReuse = true;
            item.shootSpeed = 16f;
            item.shoot = ModContent.ProjectileType<LiquidBlade>();

            item.width = item.height = 16;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.noMelee = true;
            item.UseSound = SoundID.Item9;
            item.value = CalamityGlobalItem.Rarity11BuyPrice;
            item.rare = ItemRarityID.Purple;
        }

        // Terraria seems to really dislike high crit values in SetDefaults
        public override void GetWeaponCrit(Player player, ref int crit) => crit += 30;

        public override Vector2? HoldoutOrigin() => new Vector2(15, 15);

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<InfernalRift>());
            recipe.AddIngredient(ItemID.AquaScepter);
            recipe.AddRecipeGroup("CursedFlameIchor", 20);
            recipe.AddIngredient(ModContent.ItemType<SeaPrism>(), 10);
            recipe.AddIngredient(ModContent.ItemType<GalacticaSingularity>(), 5);
            recipe.AddIngredient(ItemID.LunarBar, 5);
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            Vector2 playerPos = player.RotatedRelativePoint(player.MountedCenter, true);
            float speed = item.shootSpeed;
            float xVec = Main.mouseX + Main.screenPosition.X - playerPos.X;
            float yVec = Main.mouseY + Main.screenPosition.Y - playerPos.Y;
            float f = Main.rand.NextFloat() * MathHelper.TwoPi;
            float lowerBoundOffset = 20f;
            float upperBoundOffset = 60f;
            Vector2 source = playerPos + f.ToRotationVector2() * MathHelper.Lerp(lowerBoundOffset, upperBoundOffset, Main.rand.NextFloat());
            for (int i = 0; i < 50; i++)
            {
                source = playerPos + f.ToRotationVector2() * MathHelper.Lerp(lowerBoundOffset, upperBoundOffset, Main.rand.NextFloat());
                if (Collision.CanHit(playerPos, 0, 0, source + (source - playerPos).SafeNormalize(Vector2.UnitX) * 8f, 0, 0))
                {
                    break;
                }
                f = Main.rand.NextFloat() * MathHelper.TwoPi;
            }
            Vector2 velocity = Main.MouseWorld - source;
            Vector2 upperVelocityLimit = new Vector2(xVec, yVec).SafeNormalize(Vector2.UnitY) * speed;
            velocity = velocity.SafeNormalize(upperVelocityLimit) * speed;
            velocity = Vector2.Lerp(velocity, upperVelocityLimit, 0.25f);
            Projectile.NewProjectile(source, velocity, type, damage, knockBack, player.whoAmI, 0f, 0f);
            return false;
        }
    }
}
