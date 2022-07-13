using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Fishing.SunkenSeaCatches
{
    public class SurfClam : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Surf Clam");
            SacrificeTotal = 2;
        }

        public override void SetDefaults()
        {
            Item.questItem = true;
            Item.maxStack = 1;
            Item.width = 20;
            Item.height = 20;
            Item.uniqueStack = true;
            Item.rare = ItemRarityID.Quest;
        }

        public override bool IsQuestFish()
        {
            return true;
        }

        public override bool IsAnglerQuestAvailable()
        {
            return DownedBossSystem.downedDesertScourge;
        }

        public override void AnglerQuestChat(ref string description, ref string catchLocation)
        {
            description = "Did you know that clams are a delicacy? Rumor has it that a big colony lives beneath the Desert. Just talking about them makes me hungry. I need you to fetch one for me, so I can practice my culinary skills and curb my hunger.";
            catchLocation = "Caught in the Sunken Sea.";
        }
    }
}
