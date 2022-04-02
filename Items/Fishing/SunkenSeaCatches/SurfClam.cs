using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.World;

namespace CalamityMod.Items.Fishing.SunkenSeaCatches
{
    public class SurfClam : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Surf Clam");
        }

        public override void SetDefaults()
        {
            item.questItem = true;
            item.maxStack = 1;
            item.width = 20;
            item.height = 20;
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
            description = "Did you know that clams are a delicacy? Rumor has it that a big colony lives beneath the Desert. Just talking about them makes me hungry. I need you to fetch one for me, so I can practice my culinary skills and curb my hunger.";
            catchLocation = "Caught in the Sunken Sea.";
        }
    }
}
