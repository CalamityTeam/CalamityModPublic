using CalamityMod.Dusts;
using CalamityMod.Items.Fishing.AstralCatches;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace CalamityMod.Tiles.Astral
{
    public class AstralCrateTile : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileSolidTop[Type] = true;
            Main.tileLighted[Type] = true;
            Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = true;
            Main.tileTable[Type] = true;
            Main.tileLavaDeath[Type] = true;
            Main.tileWaterDeath[Type] = false;

            TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
            TileObjectData.addTile(Type);
            AddMapEntry(new Color(47, 66, 90), CalamityUtils.GetItemName<AstralCrate>());
            DustType = ModContent.DustType<AstralBlue>();
        }

        public override bool CreateDust(int i, int j, ref int type)
        {
            if (Main.rand.NextBool())
            {
                type = ModContent.DustType<AstralOrange>();
            }
            else
            {
                type = ModContent.DustType<AstralBlue>();
            }
            return true;
        }

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 3;
        }
    }
}
