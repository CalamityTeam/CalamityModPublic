
using Microsoft.Xna.Framework;
using Terraria; using CalamityMod.Projectiles; using Terraria.ModLoader;
using Terraria.ModLoader; using CalamityMod.Buffs; using CalamityMod.Items; using CalamityMod.NPCs; using CalamityMod.Projectiles; using CalamityMod.Tiles; using CalamityMod.Walls;

namespace CalamityMod.Tiles
{
    public class CryonicBrick : ModTile
    {
        int subsheetHeight = 90;
        int subsheetWidth = 234;
        public override void SetDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileBlockLight[Type] = true;

            TileMerge.MergeGeneralTiles(Type);
            TileMerge.MergeDecorativeTiles(Type);

            soundType = 21;
            minPick = 100;
            drop = ModContent.ItemType<CryonicBrick>();
            AddMapEntry(new Color(99, 131, 199));
        }

        public override bool CreateDust(int i, int j, ref int type)
        {
            Dust.NewDust(new Vector2(i, j) * 16f, 16, 16, 176, 0f, 0f, 1, new Color(255, 255, 255), 1f);
            return false;
        }

        public override void AnimateIndividualTile(int type, int i, int j, ref int frameXOffset, ref int frameYOffset)
        {
            int xPos = i % 2;
            int yPos = j % 4;
            frameXOffset = xPos * subsheetWidth;
            frameYOffset = yPos * subsheetHeight;
            ;
        }
    }
}
