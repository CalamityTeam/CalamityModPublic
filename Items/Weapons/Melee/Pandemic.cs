using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Melee.Yoyos;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Melee
{
    [LegacyName("ThePlaguebringer")]
    public class Pandemic : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Melee";

        //Changing these first 3 values requires restarting the game or hijacking some function to do the yoyo's SetStaticDefaults again.
        public static float Lifetime => 15;
        public static float Reach => 480f;
        public static float Speed => 24f;
        public static float DamageMultiplier(int count) => 1 + count * 0.2f;
        public static float SizeMultiplier(int count) => 0.5f + DamageMultiplier(count) * 0.5f;
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(Reach.ToTiles(), Speed, Lifetime);

        public override void SetStaticDefaults()
        {
            ItemID.Sets.Yoyo[Type] = true;
            ItemID.Sets.GamepadExtraRange[Type] = 15;
            ItemID.Sets.GamepadSmartQuickReach[Type] = true;
        }

        public override void SetDefaults()
        {
            Item.width = 30;
            Item.height = 32;
            Item.DamageType = DamageClass.MeleeNoSpeed;
            Item.damage = 125;
            Item.knockBack = 2.5f;
            Item.useTime = 22;
            Item.useAnimation = 22;
            Item.autoReuse = true;

            Item.useStyle = ItemUseStyleID.Shoot;
            Item.UseSound = SoundID.Item1;
            Item.channel = true;
            Item.noUseGraphic = true;
            Item.noMelee = true;

            Item.shoot = ModContent.ProjectileType<PandemicYoyo>();
            Item.shootSpeed = 14f;

            Item.rare = ItemRarityID.Yellow;
            Item.value = CalamityGlobalItem.RarityYellowBuyPrice;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.HiveFive).
                AddIngredient<InfectedArmorPlating>(6).
                AddIngredient<PlagueCellCanister>(10).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}
