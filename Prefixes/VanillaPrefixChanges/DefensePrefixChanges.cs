using System.Collections.Generic;
using CalamityMod.Prefixes.VanillaPrefixChanges.Stats;
using Terraria.ID;

namespace CalamityMod.Prefixes.VanillaPrefixChanges
{
    public class HardPrefixChange : VanillaPrefixChange
    {
        public override int TargetPrefix => PrefixID.Hard;
        public override string TargetTooltipName => "PrefixAccDefense";

        public override IEnumerator<IVanillaPrefixStat> PopulateStats()
        {
            yield return new PrefixDRStat(0.03f);
        }
    }

    public class GuardingsPrefixChange : VanillaPrefixChange
    {
        public override int TargetPrefix => PrefixID.Guarding;
        public override string TargetTooltipName => "PrefixAccDefense";

        public override IEnumerator<IVanillaPrefixStat> PopulateStats()
        {
            yield return new PrefixDefenseStat(2);
            yield return new PrefixMovementSpeedStat(0.02f);
        }
    }

    public class ArmoredPrefixChange : VanillaPrefixChange
    {
        public override int TargetPrefix => PrefixID.Armored;
        public override string TargetTooltipName => "PrefixAccDefense";

        public override IEnumerator<IVanillaPrefixStat> PopulateStats()
        {
            yield return new PrefixDefenseStat(2);
            yield return new PrefixDRStat(0.015f);
        }
    }

    public class WardingPrefixChange : VanillaPrefixChange
    {
        public override int TargetPrefix => PrefixID.Warding;
        public override string TargetTooltipName => "PrefixAccDefense";

        public override IEnumerator<IVanillaPrefixStat> PopulateStats()
        {
            yield return new PrefixDefenseStat(4);
        }
    }
}
