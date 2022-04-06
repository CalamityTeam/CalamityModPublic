using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Tiles.Abyss
{
    public class Tenebris : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileMergeDirt[Type] = true;
            Main.tileBlockLight[Type] = true;

            CalamityUtils.MergeWithGeneral(Type);
            CalamityUtils.MergeWithAbyss(Type);

            DustType = 44;
            ItemDrop = ModContent.ItemType<Items.Placeables.Tenebris>();
            ModTranslation name = CreateMapEntryName();
            name.SetDefault("Tenebris");
            AddMapEntry(new Color(0, 100, 100), name);
            MineResist = 3f;
            MinPick = 200;
            SoundType = SoundID.Tink;
        }

        public override bool CanExplode(int i, int j)
        {
            return false;
        }

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 3;
        }

        public override void NearbyEffects(int i, int j, bool closer)
        {
            if (!closer && j < Main.maxTilesY - 205)
            {
                Tile t = Main.tile[i, j];
                if (t.LiquidAmount <= 0)
                {
                    t.LiquidAmount = 255;
                    t.Get<LiquidData>().LiquidType = LiquidID.Water;
                }
            }
        }

        public override bool TileFrame(int i, int j, ref bool resetFrame, ref bool noBreak)
        {
            TileFraming.CustomMergeFrame(i, j, Type, ModContent.TileType<AbyssGravel>(), false, false, false, false, resetFrame);
            return false;
        }
    }
}
