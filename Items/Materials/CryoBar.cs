using CalamityMod.Tiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Materials
{
    public class CryoBar : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Frigid Bar");
            Tooltip.SetDefault("Cold to the touch");
        }

        public override void SetDefaults()
        {
			item.createTile = ModContent.TileType<FrigidBar>();
            item.width = 30;
            item.height = 24;
            item.maxStack = 999;
            item.value = Item.buyPrice(0, 3, 0, 0);
            item.rare = ItemRarityID.Pink;
			item.useStyle = ItemUseStyleID.SwingThrow;
			item.useTurn = true;
			item.useAnimation = 15;
			item.useTime = 10;
			item.autoReuse = true;
			item.consumable = true;
        }
    }
}
