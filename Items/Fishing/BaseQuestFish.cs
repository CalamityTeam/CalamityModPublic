using System.Text.RegularExpressions;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityMod.Items.Fishing
{
    public abstract class BaseQuestFish : ModItem, ILocalizedModType
    {
        // The following two fields below are required.
        // Condition(s) required for the fish to appear within the Angler quest pool. Defaults to no condition.
        public virtual bool QuestCondition => true;

        // Location where the fish is caught for purpose of tooltips and dialogues. Can be copied from other tooltips as needed.
        public virtual LocalizedText Location => LocalizedText.Empty;

        public new string LocalizationCategory => "Items.Fishing";
        public override LocalizedText Tooltip => Location;

        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 2;
            ItemID.Sets.CanBePlacedOnWeaponRacks[Item.type] = true;
        }

        public override void SetDefaults()
        {
            Item.DefaultToQuestFish();
        }

        public override bool IsQuestFish() => true;
        public override bool IsAnglerQuestAvailable() => QuestCondition;

        public override void AnglerQuestChat(ref string description, ref string catchLocation)
        {
            description = this.GetLocalizedValue("QuestDescription");
            catchLocation = Location.ToString().Replace("'", string.Empty);
        }
    }
}
