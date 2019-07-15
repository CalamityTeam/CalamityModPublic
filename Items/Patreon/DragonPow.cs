using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items;

namespace CalamityMod.Items.Patreon
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
            item.GetGlobalItem<CalamityGlobalItem>(mod).postMoonLordRarity = 21;
            item.value = Item.buyPrice(2, 50, 0, 0);

            item.shoot = mod.ProjectileType("DragonPowFlail");
            item.shootSpeed = Speed;
        }

        public override void AddRecipes()
        {
            ModRecipe r = new ModRecipe(mod);
            r.SetResult(this);
            r.AddTile(null, "DraedonsForge");
            r.AddIngredient(null, "Mourningstar");
            r.AddIngredient(ItemID.DaoofPow);
            r.AddIngredient(ItemID.FlowerPow);
            r.AddIngredient(ItemID.Flairon);
            r.AddIngredient(null, "BallOFugu");
            r.AddIngredient(null, "Tumbleweed");
            r.AddIngredient(null, "UrchinFlail");
            r.AddIngredient(null, "CosmiliteBar", 5);
            r.AddIngredient(null, "Phantoplasm", 5);
            r.AddIngredient(null, "NightmareFuel", 5);
            r.AddIngredient(null, "EndothermicEnergy", 5);
            r.AddIngredient(null, "DarksunFragment", 5);
            r.AddIngredient(null, "HellcasterFragment", 10);
            r.AddIngredient(null, "AuricOre", 25);
            r.AddRecipe();
        }
    }
}
