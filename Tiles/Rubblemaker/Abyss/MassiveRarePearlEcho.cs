using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace CalamityMod.Tiles.Rubblemaker.Abyss
{
    public class MassiveRarePearlEcho : ModTile
    {
        public override string Texture => "CalamityMod/Tiles/Abyss/AbyssAmbient/MassiveRarePearl";
        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileLavaDeath[Type] = true;
            Main.tileWaterDeath[Type] = false;
            Main.tileNoAttach[Type] = true;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style2x1);
            TileObjectData.newTile.DrawYOffset = 2;
            TileObjectData.addTile(Type);
            AddMapEntry(new Color(106, 80, 102));
            DustType = 33;

            RegisterItemDrop(ItemID.WhitePearl);
            FlexibleTileWand.RubblePlacementLarge.AddVariations(ItemID.WhitePearl, Type, 0);

            base.SetStaticDefaults();
        }
    }
}
