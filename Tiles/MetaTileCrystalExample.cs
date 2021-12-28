using CalamityMod.World;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Tiles
{
    public class MetaTileCrystalExample : SuperDirectionalTile
    {
        public override bool Autoload(ref string name, ref string texture)
        {
            texture = "CalamityMod/Tiles/MetaTileCrystalExample";
            return base.Autoload(ref name, ref texture);
        }

        public override void SetDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileBlockLight[Type] = true;

            CalamityUtils.MergeWithGeneral(Type);
            CalamityUtils.MergeAstralTiles(Type);
            CalamityUtils.SetMerge(Type, TileID.LeafBlock);
            CalamityUtils.SetMerge(Type, TileID.LivingMahoganyLeaves);
            CalamityUtils.SetMerge(Type, TileID.LivingWood);
            CalamityUtils.SetMerge(Type, TileID.LivingMahogany);

            drop = ModContent.ItemType<Items.Placeables.MetaTile>();
            AddMapEntry(new Color(45, 36, 63));
        }
    }
}
