using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityMod.Items.Fishing.BrimstoneCragCatches
{
    public class Brimlish : BaseQuestFish
    {
        public override bool QuestCondition => DownedBossSystem.downedBrimstoneElemental;
        public override LocalizedText Location => CalamityUtils.GetText("Items.Fishing.CaughtInBrimstoneCrag");
    }
}
