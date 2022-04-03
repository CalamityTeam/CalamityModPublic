using CalamityMod.Items.Materials;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Mounts.Minecarts
{
    public class DoGCart : ModItem
    {
        public override void SetStaticDefaults() => DisplayName.SetDefault("The Cart of Gods");

        public override void SetDefaults()
        {
            Item.width = 34;
            Item.height = 36;
            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.useStyle = ItemUseStyleID.Swing;

            Item.value = Item.sellPrice(gold: 30);
            Item.rare = ItemRarityID.Purple;
            Item.Calamity().customRarity = CalamityRarity.DarkBlue;
            Item.Calamity().donorItem = true;

            Item.UseSound = SoundID.Item68;
            Item.noMelee = true;
            Item.mountType = ModContent.MountType<DoGCartMount>();
        }

        public override void AddRecipes()
        {
            CreateRecipe(1).AddIngredient(ModContent.ItemType<CosmiliteBar>(), 10).AddIngredient(ModContent.ItemType<AscendantSpiritEssence>()).AddIngredient(ItemID.Wire, 60).AddTile(ModContent.TileType<CosmicAnvil>()).Register();
        }
    }
}
