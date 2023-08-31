using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Tiles.FurniturePlaguedPlate
{
    public class PlaguedPlateTable : ModTile
    {
        public override void SetStaticDefaults() => this.SetUpTable(ModContent.ItemType<Items.Placeables.FurniturePlagued.PlaguedPlateTable>(), true);

        public override bool CreateDust(int i, int j, ref int type)
        {
            Dust.NewDust(new Vector2(i, j) * 16f, 16, 16, 178, 0f, 0f, 1, new Color(255, 255, 255), 1f);
            return false;
        }

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 3;
        }
    }
}
