using CalamityMod.Tiles.Ores;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace CalamityMod.Tiles.Crags
{
    public class ScorchedRemainsGrass : ModTile
    {
        private const short subsheetWidth = 450;
        private const short subsheetHeight = 198;

        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileBlockLight[Type] = true;

            CalamityUtils.MergeWithGeneral(Type);
            CalamityUtils.MergeWithHell(Type);
            CalamityUtils.SetMerge(Type, ModContent.TileType<ScorchedRemains>());

            HitSound = SoundID.Dig;
            MineResist = 1f;
            MinPick = 100;
            ItemDrop = ModContent.ItemType<Items.Placeables.ScorchedRemains>();
            AddMapEntry(new Color(204, 55, 153));
        }

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
		{
			r = 0.339f;
			g = 0.069f;
			b = 0.076f;
        }

        public override bool CreateDust(int i, int j, ref int type)
        {
            Dust.NewDust(new Vector2(i, j) * 16f, 16, 16, 1, 0f, 0f, 1, new Color(100, 100, 100), 1f);
            return false;
        }

        public override bool CanExplode(int i, int j)
        {
            return false;
        }
    }
}
