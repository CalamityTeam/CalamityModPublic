using CalamityMod.Items.Materials;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Tiles.Abyss
{
    public class LumenylCrystals : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileLighted[Type] = true;
            Main.tileNoFail[Type] = true;
            Main.tileFrameImportant[Type] = true;
            Main.tileObsidianKill[Type] = true;
            ModTranslation name = CreateMapEntryName();
            name.SetDefault("Lumenyl");
            AddToArray(ref TileID.Sets.RoomNeeds.CountsAsTorch);
            AddMapEntry(new Color(0, 150, 200), name);
            SoundType = SoundID.Item;
            soundStyle = 27;
            DustType = 67;
            ItemDrop = ModContent.ItemType<Lumenite>();
            Main.tileSpelunker[Type] = true;
        }

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 3;
        }

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            r = 0.00f;
            g = 0.3f;
            b = 0.4f;
        }

        public override bool CanPlace(int i, int j)
        {
            if (Main.tile[i, j + 1].slope() == 0 && !Main.tile[i, j + 1].IsHalfBlock)
                return true;
            if (Main.tile[i, j - 1].slope() == 0 && !Main.tile[i, j - 1].IsHalfBlock)
                return true;
            if (Main.tile[i + 1, j].slope() == 0 && !Main.tile[i + 1, j].IsHalfBlock)
                return true;
            if (Main.tile[i - 1, j].slope() == 0 && !Main.tile[i - 1, j].IsHalfBlock)
                return true;
            return false;
        }

        public override void PlaceInWorld(int i, int j, Item item)
        {
            if (Main.tile[i, j + 1].HasTile && Main.tileSolid[Main.tile[i, j + 1].TileType] && Main.tile[i, j + 1].slope() == 0 && !Main.tile[i, j + 1].IsHalfBlock)
            {
                Main.tile[i, j].TileFrameY = (short)(0 * 18);
            }
            else if (Main.tile[i, j - 1].HasTile && Main.tileSolid[Main.tile[i, j - 1].TileType] && Main.tile[i, j - 1].slope() == 0 && !Main.tile[i, j - 1].IsHalfBlock)
            {
                Main.tile[i, j].TileFrameY = (short)(1 * 18);
            }
            else if (Main.tile[i + 1, j].HasTile && Main.tileSolid[Main.tile[i + 1, j].TileType] && Main.tile[i + 1, j].slope() == 0 && !Main.tile[i + 1, j].IsHalfBlock)
            {
                Main.tile[i, j].TileFrameY = (short)(2 * 18);
            }
            else if (Main.tile[i - 1, j].HasTile && Main.tileSolid[Main.tile[i - 1, j].TileType] && Main.tile[i - 1, j].slope() == 0 && !Main.tile[i - 1, j].IsHalfBlock)
            {
                Main.tile[i, j].TileFrameY = (short)(3 * 18);
            }
            Main.tile[i, j].TileFrameX = (short)(WorldGen.genRand.Next(18) * 18);
        }
    }
}
