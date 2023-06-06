using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityMod.Items.Fishing.SunkenSeaCatches
{
    public class Serpentuna : BaseQuestFish
    {
        public override bool QuestCondition => Main.hardMode;
        public override LocalizedText Location => CalamityUtils.GetText("Items.Fishing.CaughtInSunkenSea");
    }
}
