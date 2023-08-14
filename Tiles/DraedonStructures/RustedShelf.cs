using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Tiles.DraedonStructures
{
    public class RustedShelf : ModTile
    {
        public override void SetStaticDefaults()
        {
            this.SetUpPlatform(ModContent.ItemType<Items.Placeables.DraedonStructures.RustedShelf>(), true);
            HitSound = SoundID.Tink;
            DustType = 32;
        }

        public override bool CanExplode(int i, int j) => false;

        public override void PostSetDefaults()
        {
            Main.tileNoSunLight[Type] = false;
        }
    }
}
