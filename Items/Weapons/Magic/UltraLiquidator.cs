using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Magic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Magic
{
    public class UltraLiquidator : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Magic";
        public override void SetStaticDefaults()
        {
            Item.staff[Item.type] = true;
        }

        public override void SetDefaults()
        {
            Item.damage = 120;
            Item.knockBack = 5f;
            Item.useTime = 3;
            Item.reuseDelay = Item.useAnimation = 15;
            Item.useLimitPerAnimation = 5;
            Item.mana = 25;
            Item.DamageType = DamageClass.Magic;
            Item.autoReuse = true;
            Item.shootSpeed = 16f;
            Item.shoot = ModContent.ProjectileType<LiquidBlade>();

            Item.width = Item.height = 70;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.UseSound = SoundID.Item9;
            Item.value = CalamityGlobalItem.Rarity11BuyPrice;
            Item.rare = ItemRarityID.Purple;
        }

        // Terraria seems to really dislike high crit values in SetDefaults
        public override void ModifyWeaponCrit(Player player, ref float crit) => crit += 30;

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo projSource, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Vector2 playerPos = player.RotatedRelativePoint(player.MountedCenter, true);
            float speed = Item.shootSpeed;
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
            Vector2 velocityReal = Main.MouseWorld - source;
            Vector2 upperVelocityLimit = new Vector2(xVec, yVec).SafeNormalize(Vector2.UnitY) * speed;
            velocityReal = velocityReal.SafeNormalize(upperVelocityLimit) * speed;
            velocityReal = Vector2.Lerp(velocityReal, upperVelocityLimit, 0.25f);
            Projectile.NewProjectile(projSource, source, velocityReal, type, damage, knockback, player.whoAmI, 0f, 0f);
            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<InfernalRift>().
                AddIngredient(ItemID.AquaScepter).
                AddIngredient(ItemID.LunarBar, 5).
                AddIngredient<GalacticaSingularity>(5).
                AddTile(TileID.LunarCraftingStation).
                Register();
        }
    }
}
