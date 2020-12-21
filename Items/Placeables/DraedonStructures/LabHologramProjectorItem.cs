using CalamityMod.Items.Materials;
using CalamityMod.Tiles.DraedonStructures;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Placeables.DraedonStructures
{
    public class LabHologramProjectorItem : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Lab Hologram Projector");
        }

        public override void SetDefaults()
        {
            item.width = 22;
            item.height = 32;
            item.maxStack = 99;
            item.useTurn = true;
            item.autoReuse = true;
            item.useAnimation = 15;
            item.useTime = 10;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.consumable = true;
            item.value = Item.buyPrice(gold: 5);
            item.rare = ItemRarityID.Orange;
            item.Calamity().customRarity = CalamityRarity.DraedonRust;
            item.createTile = ModContent.TileType<LabHologramProjector>();
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<MysteriousCircuitry>(), 6);
            recipe.AddIngredient(ModContent.ItemType<DubiousPlating>(), 6);
            recipe.AddIngredient(ModContent.ItemType<CalamityMod.Items.Placeables.DraedonStructures.LaboratoryPlating>(), 20);
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
