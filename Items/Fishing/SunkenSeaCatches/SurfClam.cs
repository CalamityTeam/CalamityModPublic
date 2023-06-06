using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityMod.Items.Fishing.SunkenSeaCatches
{
    public class SurfClam : BaseQuestFish
    {
        public override bool QuestCondition => DownedBossSystem.downedDesertScourge;
        public override LocalizedText Location => CalamityUtils.GetText("Items.Fishing.CaughtInSunkenSea");
    }
}
