using CalamityMod.Items.Materials;
using CalamityMod.Tiles.Furniture.CraftingStations;
using CalamityMod.Projectiles.Melee.Yoyos;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Melee
{
    public class TheGodsGambit : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("The God's Gambit");
            Tooltip.SetDefault("Fires a stream of slime when enemies are near\n" +
            "A very agile yoyo");
            ItemID.Sets.Yoyo[item.type] = true;
            ItemID.Sets.GamepadExtraRange[item.type] = 15;
            ItemID.Sets.GamepadSmartQuickReach[item.type] = true;
        }

        public override void SetDefaults()
        {
            item.width = 40;
            item.height = 26;
            item.melee = true;
            item.damage = 29;
            item.knockBack = 3.5f;
            item.useTime = 21;
            item.useAnimation = 21;
            item.autoReuse = true;

            item.useStyle = ItemUseStyleID.HoldingOut;
            item.UseSound = SoundID.Item1;
            item.channel = true;
            item.noUseGraphic = true;
            item.noMelee = true;

            item.shoot = ModContent.ProjectileType<GodsGambitYoyo>();
            item.shootSpeed = 10f;

            item.rare = ItemRarityID.LightRed;
            item.value = Item.buyPrice(gold: 12);
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<PurifiedGel>(), 30);
            recipe.AddTile(ModContent.TileType<StaticRefiner>());
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
