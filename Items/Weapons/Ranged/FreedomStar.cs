using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Ranged;
using CalamityMod.Rarities;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Ranged
{
    public class FreedomStar : ModItem
    {
        // This is the amount of charge consumed every time the holdout projectile fires various projectiles.
        public const float HoldoutChargeUse_Orb = 0.005f;
        public const float HoldoutChargeUse_OrbLarge = 0.006f;
        public const float HoldoutChargeUse_Laser = 0.0075f;
        public const float HoldoutChargeUse_LaserLarge = 0.015f;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Freedom Star");
            Tooltip.SetDefault(@"Tap to fire energy orbs
Hold to charge and fire a beam that explodes on hit");
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            CalamityGlobalItem modItem = Item.Calamity();
            Item.damage = 200;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 54;
            Item.height = 28;
            Item.useTime = 10;
            Item.useAnimation = 10;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.channel = true;
            Item.noUseGraphic = true;
            Item.knockBack = 3f;
            Item.value = CalamityGlobalItem.Rarity12BuyPrice;
            Item.rare = ModContent.RarityType<Turquoise>();
            modItem.donorItem = true;
            Item.UseSound = SoundID.Item75;
            Item.shoot = ModContent.ProjectileType<FreedomStarHoldout>();
            Item.shootSpeed = 12f;
            modItem.UsesCharge = true;
            modItem.MaxCharge = 150f;
            modItem.ChargePerUse = 0f; // This weapon is a holdout. Charge is consumed by the holdout projectile.
        }

        public override Vector2? HoldoutOffset() => new Vector2(-5, 0);

        public override bool CanUseItem(Player player) => player.ownedProjectileCounts[Item.shoot] <= 0;

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Vector2 shootVelocity = velocity;
            Vector2 shootDirection = shootVelocity.SafeNormalize(Vector2.UnitX * player.direction);
            // Charge-up. Done via a holdout projectile.
            Projectile.NewProjectile(source, position, shootDirection, type, damage, knockback, player.whoAmI);
            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.LunarBar, 4).
                AddIngredient<UelibloomBar>(8).
                AddIngredient<MysteriousCircuitry>(12).
                AddIngredient<DubiousPlating>(18).
                AddTile(TileID.LunarCraftingStation).
                Register();
        }
    }
}
