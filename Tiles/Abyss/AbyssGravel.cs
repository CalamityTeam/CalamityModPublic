
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Tiles.Abyss
{
    public class AbyssGravel : ModTile
    {
        int subsheetHeight = 270;
        int subsheetWidth = 288;

        public static readonly SoundStyle MineSound = new("CalamityMod/Sounds/Custom/AbyssGravelMine", 3);
        
        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileBlockLight[Type] = true;
            Main.tileMergeDirt[Type] = true;

            CalamityUtils.MergeWithGeneral(Type);
            CalamityUtils.MergeWithAbyss(Type);

            ItemDrop = ModContent.ItemType<Items.Placeables.AbyssGravel>();
            AddMapEntry(new Color(25, 28, 54));
            MineResist = 5f;
            MinPick = 65;
            HitSound = MineSound;
            DustType = 33;
        }

        public override bool CanExplode(int i, int j)
        {
            return false;
        }

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 3;
        }

        public override void AnimateIndividualTile(int type, int i, int j, ref int frameXOffset, ref int frameYOffset)
        {
            frameXOffset = i % 1 * subsheetWidth; //the 1 should be 2, but it looks weird right now, should be looked into later
            frameYOffset = j % 1 * subsheetHeight;
        }

        public override bool TileFrame(int i, int j, ref bool resetFrame, ref bool noBreak)
        {
            TileFraming.CustomMergeFrame(i, j, Type, TileID.Dirt);
            return false;
        }
    }
}
