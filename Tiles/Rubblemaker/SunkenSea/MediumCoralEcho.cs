using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace CalamityMod.Tiles.Rubblemaker.SunkenSea
{
    public class MediumCoralEcho : ModTile
    {
        public override string Texture => "CalamityMod/Tiles/SunkenSea/MediumCoral";
        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileLavaDeath[Type] = true;
            Main.tileWaterDeath[Type] = false;
            Main.tileNoAttach[Type] = true;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
            TileObjectData.newTile.DrawYOffset = 2;
            TileObjectData.addTile(Type);
            AddMapEntry(new Color(233, 132, 58));
            DustType = DustID.Coralstone;
            RegisterItemDrop(ItemID.CoralstoneBlock);
            FlexibleTileWand.RubblePlacementMedium.AddVariations(ItemID.CoralstoneBlock, Type, 0);

            base.SetStaticDefaults();
        }
        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 2;
        }
    }

    public class MediumCoral2Echo : MediumCoralEcho
    {
        public override string Texture => "CalamityMod/Tiles/SunkenSea/MediumCoral2";
    }
}
