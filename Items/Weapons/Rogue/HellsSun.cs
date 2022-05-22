using Terraria.DataStructures;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;
using Microsoft.Xna.Framework;
using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Rogue;
using CalamityMod.CalPlayer;

namespace CalamityMod.Items.Weapons.Rogue
{
    public class HellsSun : ModItem
    {
        private static int damage = 85;
        private static int knockBack = 5;
        private static float SdamageMult = 0.6f;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Hell's Sun");
            Tooltip.SetDefault("The Subterranean Sun in the palm of your hand.\n" +
                "Shoots a gravity-defying spiky ball. Stacks up to 10.\n" +
                "Once stationary, periodically emits small suns that explode on hit\n" +
                "Stealth strikes emit suns at a faster rate and last for a longer amount of time\n" +
                "Right click to delete all existing spiky balls");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 10;
        }

        public override void SetDefaults()
        {
            Item.damage = damage;
            Item.Calamity().rogue = true;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.width = 1;
            Item.height = 1;
            Item.useTime = 15;
            Item.useAnimation = 15;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = knockBack;
            Item.value = Item.sellPrice(0, 2, 40, 0);
            Item.rare = ItemRarityID.Purple;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.maxStack = 10;

            Item.shootSpeed = 5f;
            Item.shoot = ModContent.ProjectileType<HellsSunProj>();
        }

        public override bool CanUseItem(Player player)
        {
            if (player.altFunctionUse == 2)
            {
                Item.shoot = ProjectileID.None;
                Item.shootSpeed = 0f;
                return player.ownedProjectileCounts[ModContent.ProjectileType<HellsSunProj>()] > 0;
            }
            else
            {
                Item.shoot = ModContent.ProjectileType<HellsSunProj>();
                Item.shootSpeed = 5f;
                int UseMax = Item.stack;
                return player.ownedProjectileCounts[ModContent.ProjectileType<HellsSunProj>()] < UseMax;
            }
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.killSpikyBalls = false;
            if (modPlayer.StealthStrikeAvailable()) //setting the stealth strike
            {
                int stealth = Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<HellsSunProj>(), (int)(damage * SdamageMult), knockback, player.whoAmI);
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
                AddIngredient(ItemID.SpikyBall, 100).
                AddIngredient<UnholyEssence>(10).
                AddTile(TileID.LunarCraftingStation).
                Register();
        }
    }
}
