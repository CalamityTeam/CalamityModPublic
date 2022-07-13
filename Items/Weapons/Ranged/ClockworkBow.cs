using Terraria.DataStructures;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Projectiles.Ranged;

namespace CalamityMod.Items.Weapons.Ranged
{
    public class ClockworkBow : ModItem
    {
        public const int MaxBolts = 6;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Clockwork Bow");
            Tooltip.SetDefault("Hold left click to load up to six precision bolts\n" +
                "The more precision bolts are loaded, the harder they hit");
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.damage = 808;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 48;
            Item.height = 96;
            Item.useTime = 60;
            Item.useAnimation = 30;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.channel = true;
            Item.knockBack = 4.25f;
            Item.value = CalamityGlobalItem.Rarity10BuyPrice;
            Item.rare = ItemRarityID.Red;
            Item.noUseGraphic = true;
            Item.UseSound = SoundID.Item20;
            Item.autoReuse = true;
            Item.shoot = ProjectileID.PurificationPowder;
            Item.shootSpeed = 15f;
            Item.useAmmo = AmmoID.Arrow;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.LunarBar, 5).
                AddIngredient(ItemID.Cog, 50).
                AddTile(TileID.LunarCraftingStation).
                Register();
        }
        public override bool CanUseItem(Player player) => player.ownedProjectileCounts[ModContent.ProjectileType<ClockworkBowHoldout>()] <= 0;

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Vector2 shootVelocity = velocity;
            Vector2 shootDirection = shootVelocity.SafeNormalize(Vector2.UnitX * player.direction);
            // Charge-up. Done via a holdout projectile.
            Projectile.NewProjectile(source, position, shootDirection, ModContent.ProjectileType<ClockworkBowHoldout>(), damage, knockback, player.whoAmI);
            return false;
        }
    }
}
