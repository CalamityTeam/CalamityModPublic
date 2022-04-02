using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.World;

namespace CalamityMod.Items.Fishing.SunkenSeaCatches
{
    public class EutrophicSandfish : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Eutrophic Sandfish");
        }

        public override void SetDefaults()
        {
            item.questItem = true;
            item.maxStack = 1;
            item.width = 40;
            item.height = 28;
            item.uniqueStack = true;
            item.rare = ItemRarityID.Quest;
        }

        public override bool IsQuestFish()
        {
            return true;
        }

        public override bool IsAnglerQuestAvailable()
        {
            return CalamityWorld.downedDesertScourge;
        }

        public override void AnglerQuestChat(ref string description, ref string catchLocation)
        {
            description = "You ever get to see what would happen if a lizard that lived in the desert scurried too deep underground? I did, and they sure are cool! But it’s way too slippery for me to get my hands on it now. You go and get it so I can keep it as a pet!";
            catchLocation = "Caught in the Sunken Sea.";
        }
    }
}
