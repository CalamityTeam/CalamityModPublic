using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityMod.Items.Fishing.BrimstoneCragCatches
{
    public class Slurpfish : BaseQuestFish
    {
        public override bool QuestCondition => NPC.downedBoss3;
        public override LocalizedText Location => CalamityUtils.GetText("Items.Fishing.CaughtInBrimstoneCrag");
    }
}
