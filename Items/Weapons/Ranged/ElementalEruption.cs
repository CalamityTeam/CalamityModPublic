using Terraria.DataStructures;
using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Ranged;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Ranged
{
    public class ElementalEruption : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Ranged";

        public int FlareCounter = 0;

        public override void SetStaticDefaults()
        {
            ItemID.Sets.ItemsThatAllowRepeatedRightClick[Item.type] = true;
        }

        public override void SetDefaults()
        {
            Item.damage = 77;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 64;
            Item.height = 34;
            Item.useAnimation = Item.useTime = 30;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 3.5f;
            Item.UseSound = SoundID.Item34;
            Item.value = CalamityGlobalItem.Rarity11BuyPrice;
            Item.rare = ItemRarityID.Purple;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<ElementalFire>();
            Item.shootSpeed = 9f;
            Item.useAmmo = AmmoID.Gel;
            Item.consumeAmmoOnFirstShotOnly = true;
        }

        public override bool AltFunctionUse(Player player) => true;

        public override Vector2? HoldoutOffset() => new Vector2(-10, 0);

        // Left click functions as clockwork with bursts of three (adjust for rounding)
        public override float UseTimeMultiplier(Player player) => player.altFunctionUse == 2 ? 1f : 0.34f;

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            // Right-click: fires a small cluster of 3 sticky, non-homing flares
            if (player.altFunctionUse == 2f)
            {
                Vector2 newPos = position + velocity.SafeNormalize(Vector2.UnitX) * 48f;
                for (int i = 0; i < 3; i++)
                {
                    newPos += Main.rand.NextVector2Circular(24f, 24f);
                    Projectile flare = Projectile.NewProjectileDirect(source, newPos, velocity, ModContent.ProjectileType<ElementalFlare>(), damage, knockback, player.whoAmI, ai2: 1f);
                    flare.penetrate = -1;
                    flare.usesLocalNPCImmunity = true;
                    flare.localNPCHitCooldown = 30;
                }
                return false;
            }

            // Left-click
            // Fires a pair of homing rainbow flares
            FlareCounter++;
            if (FlareCounter >= 3)
            {
                FlareCounter = 0;
                Vector2 newPos = position + velocity.SafeNormalize(Vector2.UnitX) * 36f;
                Projectile.NewProjectile(source, newPos, velocity, ModContent.ProjectileType<ElementalFlare>(), damage, knockback, player.whoAmI, velocity.Length(), -1f);
                Projectile.NewProjectile(source, newPos, velocity, ModContent.ProjectileType<ElementalFlare>(), damage, knockback, player.whoAmI, velocity.Length(), 1f);
            }

            // Fires three flames, random colors and spread
            for (int i = 0; i < 3; i++)
            {
                Vector2 newVel = velocity.RotatedByRandom(MathHelper.ToRadians(8f));
                Projectile.NewProjectile(source, position, newVel, type, damage, knockback, player.whoAmI, Main.rand.NextFloat());
            }
            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<TerraFlameburster>().
                AddIngredient<BlightSpewer>().
                AddIngredient(ItemID.LunarBar, 5).
                AddIngredient<LifeAlloy>(5).
                AddIngredient<GalacticaSingularity>(5).
                AddTile(TileID.LunarCraftingStation).
                Register();
        }
    }
}
