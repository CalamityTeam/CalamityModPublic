using CalamityMod.Items.Placeables.Furniture.Trophies;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace CalamityMod.Tiles.Furniture.BossTrophies
{
    public class CatastropheTrophyTile : ModTile
    {
        public override void SetStaticDefaults()
        {
            this.SetUpTrophy();
            ModTranslation name = CreateMapEntryName();
            name.SetDefault("Trophy");
            AddMapEntry(new Color(120, 85, 60), name);
            DustType = 7;
        }

        public override void KillMultiTile(int i, int j, int frameX, int frameY)
        {
            Item.NewItem(new EntitySource_TileBreak(i, j), i * 16, j * 16, 48, 48, ModContent.ItemType<CatastropheTrophy>());
        }
    }
}
