using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Summon;
using CalamityMod.Rarities;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Summon
{
    public class StellarTorusStaff : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Summon";

        public static float EnemyDetectionDistance = 1200f;
        public static int IFrames = 10;

        // In frames.
        public static float TimeBeforeCharging = 45f;
        public static float TimeCharging = 60f;
        public static float TimeShooting = 120f;

        public override void SetDefaults()
        {
            Item.damage = 185;
            Item.useTime = Item.useAnimation = 30;
            Item.knockBack = 4f;
            Item.mana = 10;
            Item.shoot = ModContent.ProjectileType<StellarTorusSummon>();

            Item.width = 42;
            Item.height = 42;
            Item.noMelee = true;
            Item.autoReuse = true;
            Item.Calamity().donorItem = true;
            Item.DamageType = DamageClass.Summon;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.UseSound = SoundID.Item15;
            Item.rare = ModContent.RarityType<Turquoise>();
            Item.value = CalamityGlobalItem.Rarity12BuyPrice;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            int minion = Projectile.NewProjectile(source, Main.MouseWorld, Main.rand.NextVector2Circular(2f, 2f), ModContent.ProjectileType<StellarTorusSummon>(), damage, knockback, player.whoAmI);

            if (Main.projectile.IndexInRange(minion))
                Main.projectile[minion].originalDamage = Item.OriginalDamage;
            
            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.XenoStaff).
                AddIngredient<ArmoredShell>(3).
                AddIngredient(ItemID.FragmentStardust, 6).
                AddTile(TileID.LunarCraftingStation).
                Register();
        }
    }
}
