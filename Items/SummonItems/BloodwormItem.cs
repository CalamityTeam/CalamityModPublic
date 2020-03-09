using Terraria.ModLoader;

namespace CalamityMod.Items.SummonItems
{
    public class BloodwormItem : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Bloodworm");
        }

        public override void SetDefaults()
        {
            item.width = 28;
            item.height = 28;
            item.maxStack = 20;
            item.rare = 9;
            item.bait = 69420;
        }
    }
}
