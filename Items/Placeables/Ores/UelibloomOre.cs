using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Placeables.Ores
{
    public class UelibloomOre : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Uelibloom Ore");
        }

        public override void SetDefaults()
        {
            item.createTile = ModContent.TileType<Tiles.Ores.UelibloomOre>();
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.useTurn = true;
            item.useAnimation = 15;
            item.useTime = 10;
            item.autoReuse = true;
            item.consumable = true;
            item.width = 10;
            item.height = 10;
            item.maxStack = 999;
            item.rare = ItemRarityID.Red;
            item.value = Item.sellPrice(silver: 50);
        }

        public override void ModifyTooltips(List<TooltipLine> list)
        {
            foreach (TooltipLine line2 in list)
            {
                if (line2.mod == "Terraria" && line2.Name == "ItemName")
                {
                    line2.overrideColor = new Color(0, 255, 200);
                }
            }
        }
    }
}
