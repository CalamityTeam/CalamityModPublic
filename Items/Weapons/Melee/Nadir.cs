using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Melee.Spears;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Melee
{
    public class Nadir : ModItem
    {
        public static int BaseDamage = 280;
        public static float ShootSpeed = 12f;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Nadir");
            Tooltip.SetDefault("Fires void essences which flay nearby enemies with tentacles\n" + "Ignores immunity frames\n" +
                "'The abyss has stared back at you long enough. It now speaks, and it does not speak softly.'");
        }

        public override void SetDefaults()
        {
            Item.width = 144;
            Item.height = 144;
            Item.noUseGraphic = true;
            Item.DamageType = DamageClass.Melee;
            Item.damage = BaseDamage;
            Item.knockBack = 8f;
            Item.useAnimation = 18;
            Item.useTime = 18;
            Item.autoReuse = true;
            Item.noMelee = true;

            Item.useStyle = ItemUseStyleID.Shoot;
            Item.UseSound = SoundID.Item1;

            Item.value = CalamityGlobalItem.Rarity14BuyPrice;
            Item.Calamity().customRarity = CalamityRarity.DarkBlue;
            Item.Calamity().donorItem = true;

            Item.shoot = ModContent.ProjectileType<NadirSpear>();
            Item.shootSpeed = ShootSpeed;
        }

        public override bool CanUseItem(Player player) => player.ownedProjectileCounts[Item.shoot] <= 0;

        public override void AddRecipes()
        {
            CreateRecipe(1).AddIngredient(ModContent.ItemType<SpatialLance>()).AddIngredient(ModContent.ItemType<TwistingNether>(), 5).AddIngredient(ModContent.ItemType<CosmiliteBar>(), 8).AddIngredient(ModContent.ItemType<DarksunFragment>(), 8).AddTile(ModContent.TileType<CosmicAnvil>()).Register();
        }
    }
}
