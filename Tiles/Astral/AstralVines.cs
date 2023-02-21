using CalamityMod.Dusts;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Metadata;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Tiles.Astral
{
    public class AstralVines : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileLighted[Type] = true;
            Main.tileCut[Type] = true;
            Main.tileBlockLight[Type] = true;
            Main.tileLavaDeath[Type] = true;
            Main.tileNoFail[Type] = true;
			TileID.Sets.IsVine[Type] = true;
			TileID.Sets.ReplaceTileBreakDown[Type] = true;
            TileID.Sets.VineThreads[Type] = true;
			TileID.Sets.DrawFlipMode[Type] = 1;
			TileMaterials.SetForTileId(Type, TileMaterials._materialsByName["Plant"]);

            DustType = ModContent.DustType<AstralBasic>();

            HitSound = SoundID.Grass;

            ModTranslation name = CreateMapEntryName();
            name.SetDefault("Astral Vines");
            AddMapEntry(new Color(65, 56, 83), name);
        }

        public override void KillTile(int i, int j, ref bool fail, ref bool effectOnly, ref bool noItem)
        {
            //GIVE VINE ROPE IF SPECIAL VINE BOOK
            if (WorldGen.genRand.NextBool() && Main.player[(int)Player.FindClosest(new Vector2((float)(i * 16), (float)(j * 16)), 16, 16)].cordage)
            {
                Item.NewItem(new EntitySource_TileBreak(i, j), new Vector2(i * 16 + 8f, j * 16 + 8f), ItemID.VineRope);
            }
        }

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            r = Main.DiscoR / 255f * 0.5f;
            g = 0.5f;
            b = (255 - Main.DiscoR) / 255f * 0.5f;
        }
    }
}
