using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Summon;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Summon
{
    public class PlantationStaff : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Summon";

        public static float EnemyDistanceDetection = 1600f;
        public static float ChargingSpeed = 35f;

        public static int ThornballAmount = 2;
        public static float ThornballFireRate = 90f; // In frames.
        public static float ThornballSpeed = 20f;

        public static float SeedBurstDelay = 30f; // In frames.
        public static float SeedBetweenBurstDelay = 10f; // In frames.
        public static float SeedSpeed = 25f;
        public static int SeedAmountPerBurst = 3;
        public static int SeedBurstAmount = 3;

        public static float SporeStartVelocity = 3f;

        public static float TimeBeforeRamming = 15f; // In frames.
        public static float RamTime = 240f; // In frames.

        public static float TentacleSpeed = 25f;

        public override void SetDefaults()
        {
            Item.damage = 58;
            Item.DamageType = DamageClass.Summon;
            Item.shoot = ModContent.ProjectileType<PlantationStaffSummon>();
            Item.knockBack = 1f;

            Item.mana = 10;
            Item.useTime = Item.useAnimation = 20;
            Item.width = 46;
            Item.height = 48;
            Item.noMelee = true;
            Item.value = CalamityGlobalItem.Rarity8BuyPrice;
            Item.rare = ItemRarityID.Yellow;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.UseSound = SoundID.Item76;
        }

        public override bool CanUseItem(Player player) => player.ownedProjectileCounts[Item.shoot] <= 0;

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Projectile.NewProjectile(source, Main.MouseWorld, Main.rand.NextVector2Circular(2f, 2f), type, damage, knockback, player.whoAmI);
            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<EyeOfNight>().
                AddIngredient(ItemID.Smolstar). // Blade Staff.
                AddIngredient<LivingShard>(12).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}
