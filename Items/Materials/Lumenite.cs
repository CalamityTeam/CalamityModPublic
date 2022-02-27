using CalamityMod.Tiles.Abyss;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Items.Materials
{
    public class Lumenite : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Lumenyl");
            Tooltip.SetDefault("A shard of lumenous energy");
        }

        public override void SetDefaults()
        {
            item.createTile = ModContent.TileType<LumenylCrystals>();
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.useTurn = true;
            item.useAnimation = 15;
            item.useTime = 10;
            item.autoReuse = true;
            item.consumable = true;
            item.width = 26;
            item.height = 26;
            item.maxStack = 999;
            item.value = Item.sellPrice(silver: 80);
            item.rare = ItemRarityID.Lime;
        }
    }
}
