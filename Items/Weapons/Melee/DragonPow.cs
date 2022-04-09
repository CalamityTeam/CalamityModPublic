using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Melee;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Melee
{
    public class DragonPow : ModItem
    {
        public static float Speed = 13f;
        public static float ReturnSpeed = 20f;
        public static float SparkSpeed = 0.6f;
        public static float MinPetalSpeed = 24f;
        public static float MaxPetalSpeed = 30f;
        public static float MinWaterfallSpeed = 12f;
        public static float MaxWaterfallSpeed = 15.5f;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Dragon Pow");
            Tooltip.SetDefault(@"Fires a dragon head that releases draconic sparks
Summons a barrage of petals and waterfalls on enemy hits");
        }

        public override void SetDefaults()
        {
            Item.width = 76;
            Item.height = 82;
            Item.DamageType = DamageClass.Melee;
            Item.damage = 660;
            Item.knockBack = 9f;
            Item.useAnimation = 20;
            Item.useTime = 20;
            Item.autoReuse = true;
            Item.noMelee = true;
            Item.noUseGraphic = true;

            Item.useStyle = ItemUseStyleID.Shoot;
            Item.UseSound = SoundLoader.GetLegacySoundSlot(Mod, "Sounds/Custom/YharonRoarShort");
            Item.channel = true;

            Item.value = CalamityGlobalItem.Rarity15BuyPrice;
            Item.Calamity().customRarity = CalamityRarity.Violet;
            Item.Calamity().donorItem = true;

            Item.shoot = ModContent.ProjectileType<DragonPowFlail>();
            Item.shootSpeed = Speed;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<Mourningstar>().
                AddIngredient(ItemID.DaoofPow).
                AddIngredient(ItemID.FlowerPow).
                AddIngredient(ItemID.Flairon).
                AddIngredient<BallOFugu>().
                AddIngredient<Tumbleweed>().
                AddIngredient<UrchinFlail>().
                AddIngredient<HellcasterFragment>(4).
                AddIngredient<AuricBar>(5).
                AddTile<CosmicAnvil>().
                Register();
        }
    }
}
