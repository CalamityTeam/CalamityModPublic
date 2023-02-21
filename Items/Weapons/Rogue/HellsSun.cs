using Terraria.DataStructures;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Rogue;
using CalamityMod.CalPlayer;

namespace CalamityMod.Items.Weapons.Rogue
{
    public class HellsSun : RogueWeapon
    {
        private static int damage = 240;
        private static int knockBack = 5;
        private static float SdamageMult = 0.12f;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Hell's Sun");
            Tooltip.SetDefault("The Subterranean Sun in the palm of your hand\n" +
                "Hurls up to 10 gravity-defying spiky balls\n" +
                "Once stationary, periodically emit small suns that explode on hit\n" +
                "Stealth strikes emit suns at a faster rate and last for a longer amount of time\n" +
                "Right click to delete all existing spiky balls");
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.damage = damage;
            Item.DamageType = RogueDamageClass.Instance;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.width = 22;
            Item.height = 18;
            Item.useTime = 15;
            Item.useAnimation = 15;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = knockBack;
            Item.value = CalamityGlobalItem.Rarity11BuyPrice;
            Item.rare = ItemRarityID.Purple;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;

            Item.shootSpeed = 5f;
            Item.shoot = ModContent.ProjectileType<HellsSunProj>();
        }

        public override bool CanUseItem(Player player)
        {
            if (player.altFunctionUse == 2)
            {
                Item.shoot = ProjectileID.None;
                return player.ownedProjectileCounts[ModContent.ProjectileType<HellsSunProj>()] > 0;
            }
            else
            {
                Item.shoot = ModContent.ProjectileType<HellsSunProj>();
                return player.ownedProjectileCounts[ModContent.ProjectileType<HellsSunProj>()] < 10;
            }
        }

		public override float StealthDamageMultiplier => SdamageMult;

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.killSpikyBalls = false;
            if (modPlayer.StealthStrikeAvailable()) //setting the stealth strike
            {
                int stealth = Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<HellsSunProj>(), damage, knockback, player.whoAmI);
                if (stealth.WithinBounds(Main.maxProjectiles))
                {
                    Main.projectile[stealth].Calamity().stealthStrike = true;
                    Main.projectile[stealth].penetrate = -1;
                    Main.projectile[stealth].timeLeft = 2400;
                }
                return false;
            }
            return true;
        }

        public override bool AltFunctionUse(Player player)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.killSpikyBalls = true;
            return true;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.LunarBar, 10).
                AddIngredient<UnholyEssence>(20).
                AddTile(TileID.LunarCraftingStation).
                Register();
        }
    }
}
