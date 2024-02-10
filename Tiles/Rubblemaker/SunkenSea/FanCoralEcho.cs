using CalamityMod.Items.Placeables;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent;
using Terraria.Enums;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace CalamityMod.Tiles.Rubblemaker.SunkenSea
{
    public class FanCoralEcho : ModTile
    {
        public override string Texture => "CalamityMod/Tiles/SunkenSea/FanCoral";
        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = true;
            Main.tileLighted[Type] = true;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style3x3);
            TileObjectData.newTile.Direction = TileObjectDirection.None;
            TileObjectData.addTile(Type);
            DustType = 253;
            AddMapEntry(new Color(54, 69, 72));
            RegisterItemDrop(ModContent.ItemType<SeaPrism>());
            FlexibleTileWand.RubblePlacementLarge.AddVariations(ModContent.ItemType<SeaPrism>(), Type, 0);

            base.SetStaticDefaults();
        }

        public override void NearbyEffects(int i, int j, bool closer)
        {
            if (!Main.dedServ)
                Main.SceneMetrics.ActiveFountainColor = ModContent.Find<ModWaterStyle>("CalamityMod/SunkenSeaWater").Slot;
        }

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 3;
        }

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            r = 0.3f;
            g = 0.75f;
            b = 0.75f;
        }
    }
}
