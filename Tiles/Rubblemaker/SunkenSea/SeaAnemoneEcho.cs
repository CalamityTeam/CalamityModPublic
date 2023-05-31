using CalamityMod.Items.Placeables;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace CalamityMod.Tiles.Rubblemaker.SunkenSea
{
    public class SeaAnemoneEcho : ModTile
    {
        public override string Texture => "CalamityMod/Tiles/SunkenSea/SeaAnemone";
        public override void SetStaticDefaults()
        {
            AnimationFrameHeight = 36;
            Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = true;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
            TileObjectData.addTile(Type);
            DustType = 253;
            AddMapEntry(new Color(54, 69, 72));
            RegisterItemDrop(ModContent.ItemType<Navystone>());
            FlexibleTileWand.RubblePlacementMedium.AddVariations(ModContent.ItemType<Navystone>(), Type, 0);

            base.SetStaticDefaults();
        }

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 3;
        }

        public override void AnimateTile(ref int frame, ref int frameCounter)
        {
            frameCounter++;
            if (frameCounter > 12)
            {
                frameCounter = 0;
                frame++;
                if (frame > 5)
                {
                    frame = 0;
                }
            }
        }
    }
}
