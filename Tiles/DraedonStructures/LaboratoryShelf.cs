using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Tiles.DraedonStructures
{
    public class LaboratoryShelf : ModTile
    {
        public override void SetStaticDefaults()
        {
            this.SetUpPlatform(ModContent.ItemType<Items.Placeables.DraedonStructures.LaboratoryShelf>(), true);
            HitSound = SoundID.Tink;
            DustType = 30;
        }

        public override bool CanExplode(int i, int j) => false;

        public override void PostSetDefaults()
        {
            Main.tileNoSunLight[Type] = false;
        }
    }
}
