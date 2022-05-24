using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Tiles.AstralSnow
{
    public class AstralIce : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileBlockLight[Type] = true;
            Main.tileBrick[Type] = true;

            CalamityUtils.MergeWithGeneral(Type);
            CalamityUtils.MergeWithSnow(Type);
            CalamityUtils.MergeAstralTiles(Type);

            DustType = 173;
            ItemDrop = ModContent.ItemType<Items.Placeables.AstralIce>();

            HitSound = SoundID.Item50;

            AddMapEntry(new Color(153, 143, 168));

            TileID.Sets.Ices[Type] = true;
            TileID.Sets.IcesSlush[Type] = true;
            TileID.Sets.IcesSnow[Type] = true;
            TileID.Sets.ChecksForMerge[Type] = true;
            TileID.Sets.Conversion.Ice[Type] = true;
            TileID.Sets.CanBeClearedDuringOreRunner[Type] = true;
        }

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 3;
        }

        public override void FloorVisuals(Player player)
        {
            player.slippy = true;
            base.FloorVisuals(player);
        }

        public override bool TileFrame(int i, int j, ref bool resetFrame, ref bool noBreak)
        {
            TileFraming.CustomMergeFrame(i, j, Type, ModContent.TileType<AstralSnow>(), false, false, false, false, resetFrame);
            return false;
        }
    }
}
