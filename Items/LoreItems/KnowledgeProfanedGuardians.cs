using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Items.LoreItems
{
    public class KnowledgeProfanedGuardians : LoreItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("The Profaned Guardians");
            Tooltip.SetDefault("The ever-rejuvenating guardians of the profaned flame.\n" +
                "Much like a phoenix from the ashes their deaths are simply a part of their life cycle.\n" +
                "Many times my forces have had to destroy these beings in search of the Profaned Goddess.");
        }

        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.rare = 10;
            item.consumable = false;
            item.Calamity().postMoonLordRarity = 12;
        }

        public override bool CanUseItem(Player player)
        {
            return false;
        }
    }
}
