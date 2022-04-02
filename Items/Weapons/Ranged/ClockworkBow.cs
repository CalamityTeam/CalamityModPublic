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
        }

        public override void SetDefaults()
        {
            item.damage = 808;
            item.ranged = true;
            item.width = 48;
            item.height = 96;
            item.useTime = 60;
            item.useAnimation = 30;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.noMelee = true;
            item.channel = true;
            item.knockBack = 4.25f;
            item.value = CalamityGlobalItem.Rarity10BuyPrice;
            item.rare = ItemRarityID.Red;
            item.noUseGraphic = true;
            item.UseSound = SoundID.Item20;
            item.autoReuse = true;
            item.shoot = ProjectileID.PurificationPowder;
            item.shootSpeed = 15f;
            item.useAmmo = AmmoID.Arrow;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.LunarBar, 5);
            recipe.AddIngredient(ItemID.Cog, 50);
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
        public override bool CanUseItem(Player player) => player.ownedProjectileCounts[ModContent.ProjectileType<ClockworkBowHoldout>()] <= 0;

        public override float UseTimeMultiplier(Player player)
        {
            return 1f;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            Vector2 shootVelocity = new Vector2(speedX, speedY);
            Vector2 shootDirection = shootVelocity.SafeNormalize(Vector2.UnitX * player.direction);
            // Charge-up. Done via a holdout projectile.
            Projectile.NewProjectile(position, shootDirection, ModContent.ProjectileType<ClockworkBowHoldout>(), damage, knockBack, player.whoAmI);
            return false;
        }
    }
}
