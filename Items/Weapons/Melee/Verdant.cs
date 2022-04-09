using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Melee.Yoyos;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Melee
{
    public class Verdant : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Verdant");
            Tooltip.SetDefault("Fires crystal leaves when enemies are near\n" +
            "A very agile yoyo");
            ItemID.Sets.Yoyo[Item.type] = true;
            ItemID.Sets.GamepadExtraRange[Item.type] = 15;
            ItemID.Sets.GamepadSmartQuickReach[Item.type] = true;
        }

        public override void SetDefaults()
        {
            Item.width = 30;
            Item.height = 30;
            Item.DamageType = DamageClass.Melee;
            Item.damage = 86;
            Item.knockBack = 6f;
            Item.useTime = 22;
            Item.useAnimation = 22;
            Item.autoReuse = true;

            Item.useStyle = ItemUseStyleID.Shoot;
            Item.UseSound = SoundID.Item1;
            Item.channel = true;
            Item.noUseGraphic = true;
            Item.noMelee = true;

            Item.shoot = ModContent.ProjectileType<VerdantYoyo>();
            Item.shootSpeed = 16f;

            Item.autoReuse = true;
            Item.value = CalamityGlobalItem.Rarity12BuyPrice;
            Item.rare = ItemRarityID.Purple;
            Item.Calamity().customRarity = CalamityRarity.Turquoise;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<UeliaceBar>(6).
                AddTile(TileID.LunarCraftingStation).
                Register();
        }
    }
}
