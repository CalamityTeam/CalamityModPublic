using CalamityMod.Dusts;
using CalamityMod.Items.Materials;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace CalamityMod.Tiles.Rubblemaker.SulphurousSea
{
    public class SulphuricFossil1Echo : ModTile
    {
        public override string Texture => "CalamityMod/Tiles/Abyss/SulphuricFossil1";
        public override void SetStaticDefaults()
        {
            Main.tileLighted[Type] = true;
            Main.tileFrameImportant[Type] = true;
            Main.tileLavaDeath[Type] = true;
            Main.tileWaterDeath[Type] = false;

            TileObjectData.newTile.CopyFrom(TileObjectData.Style3x2);
            TileObjectData.addTile(Type);
            AddMapEntry(new Color(113, 90, 71), CalamityUtils.GetText("Tiles.Fossil"));
            DustType = (int)CalamityDusts.SulfurousSeaAcid;

            RegisterItemDrop(ModContent.ItemType<CorrodedFossil>());
            FlexibleTileWand.RubblePlacementLarge.AddVariations(ModContent.ItemType<CorrodedFossil>(), Type, 0);
        }

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 3;
        }
    }
}
