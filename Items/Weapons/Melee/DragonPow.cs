using Terraria;
using Terraria.ModLoader;
using CalamityMod.Items.Placeables.Ores;
using CalamityMod.Tiles.Furniture.CraftingStations;
using CalamityMod.Items.Materials;
using Terraria.ID;
using CalamityMod.Projectiles.Melee;

namespace CalamityMod.Items.Weapons.Melee
{
    public class DragonPow : ModItem
    {
        public static int BaseDamage = 11800;
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
        }

        public override void SetDefaults()
        {
            item.width = 76;
            item.height = 82;
            item.melee = true;
            item.damage = BaseDamage;
            item.knockBack = 9f;
            item.useAnimation = 20;
            item.useTime = 20;
            item.autoReuse = true;
            item.noMelee = true;
            item.noUseGraphic = true;

            item.useStyle = 5;
            item.UseSound = mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Custom/YharonRoarShort");
            item.channel = true;

            item.rare = 10;
            item.Calamity().postMoonLordRarity = 21;
            item.value = Item.buyPrice(2, 50, 0, 0);

            item.shoot = ModContent.ProjectileType<DragonPowFlail>();
            item.shootSpeed = Speed;
        }

        public override void AddRecipes()
        {
            ModRecipe r = new ModRecipe(mod);
            r.SetResult(this);
            r.AddTile(ModContent.TileType<DraedonsForge>());
            r.AddIngredient(ModContent.ItemType<Mourningstar>());
            r.AddIngredient(ItemID.DaoofPow);
            r.AddIngredient(ItemID.FlowerPow);
            r.AddIngredient(ItemID.Flairon);
            r.AddIngredient(ModContent.ItemType<BallOFugu>());
            r.AddIngredient(ModContent.ItemType<Tumbleweed>());
            r.AddIngredient(ModContent.ItemType<UrchinFlail>());
            r.AddIngredient(ModContent.ItemType<CosmiliteBar>(), 5);
            r.AddIngredient(ModContent.ItemType<Phantoplasm>(), 5);
            r.AddIngredient(ModContent.ItemType<NightmareFuel>(), 5);
            r.AddIngredient(ModContent.ItemType<EndothermicEnergy>(), 5);
            r.AddIngredient(ModContent.ItemType<DarksunFragment>(), 5);
            r.AddIngredient(ModContent.ItemType<HellcasterFragment>(), 10);
            r.AddIngredient(ModContent.ItemType<AuricOre>(), 25);
            r.AddRecipe();
        }
    }
}
