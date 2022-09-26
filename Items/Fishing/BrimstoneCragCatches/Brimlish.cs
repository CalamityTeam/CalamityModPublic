using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Fishing.BrimstoneCragCatches
{
    public class Brimlish : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Brimlish");
            SacrificeTotal = 2;
            ItemID.Sets.CanBePlacedOnWeaponRacks[Item.type] = true;
        }

        public override void SetDefaults()
        {
            Item.questItem = true;
            Item.maxStack = 1;
            Item.width = 36;
            Item.height = 30;
            Item.uniqueStack = true;
            Item.rare = ItemRarityID.Quest;
        }

        public override bool IsQuestFish()
        {
            return true;
        }

        public override bool IsAnglerQuestAvailable()
        {
            return DownedBossSystem.downedBrimstoneElemental;
        }

        public override void AnglerQuestChat(ref string description, ref string catchLocation)
        {
            description = "When you defeated the ancient goddess, her children fell in the lava as a result of the aftermath. Rumors say they mutated a fish, and I'd love if you fetched one for my new lava-filled fish tank.";
            catchLocation = "Caught in the Brimstone Crag.";
        }
    }
}
