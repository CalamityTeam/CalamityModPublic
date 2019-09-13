using CalamityMod.World;
using Terraria;

namespace CalamityMod
{
    public static class CPlayerUtils
    {
        public static CalamityPlayer GetCalamityPlayer(this Player player) => player.GetModPlayer<CalamityPlayer>();

        public static bool InCalamity(this Player player) => player.GetCalamityPlayer().ZoneCalamity;
        public static bool InAstral(this Player player) => player.GetCalamityPlayer().ZoneAstral;
        public static bool InSunkenSea(this Player player) => player.GetCalamityPlayer().ZoneSunkenSea;
        public static bool InSulphur(this Player player) => player.GetCalamityPlayer().ZoneSulphur;
        public static bool InAbyss(this Player player, int layer = 0)
        {
            switch (layer)
            {
                case 1:
                    return player.GetCalamityPlayer().ZoneAbyssLayer1;

                case 2:
                    return player.GetCalamityPlayer().ZoneAbyssLayer2;

                case 3:
                    return player.GetCalamityPlayer().ZoneAbyssLayer3;

                case 4:
                    return player.GetCalamityPlayer().ZoneAbyssLayer4;

                default:
                    return player.GetCalamityPlayer().ZoneAbyss;
            }
        }
    }

    public static class CNPCUtils
    {
        /// <summary>
        /// Allows you to set the lifeMax value of a NPC to different values based on the mode. Called instead of npc.lifeMax = X.
        /// </summary>
        /// <param name="npc">The NPC whose lifeMax value you are trying to set.</param>
        /// <param name="normal">The value lifeMax will be set to in normal mode, this value gets doubled automatically in Expert mode.</param>
        /// <param name="revengeance">The value lifeMax will be set to in Revegeneance mode.</param>
        /// <param name="death">The value lifeMax will be set to in Death mode.</param>
        /// <param name="bossRush">The value lifeMax will be set to during the Boss Rush.</param>
        /// <param name="bossRushDeath">The value lifeMax will be set to during the Boss Rush, if Death mode is active.</param>
        public static void LifeMaxNERD(this NPC npc, int normal, int? revengeance = null, int? death = null, int? bossRush = null, int? bossRushDeath = null)
        {
            npc.lifeMax = normal;

            if (bossRush.HasValue && CalamityWorld.bossRushActive)
            {
                npc.lifeMax = bossRushDeath.HasValue && CalamityWorld.death ? bossRushDeath.Value : bossRush.Value;
            }
            else if (death.HasValue && CalamityWorld.death)
            {
                npc.lifeMax = death.Value;
            }
            else if (revengeance.HasValue && CalamityWorld.revenge)
            {
                npc.lifeMax = revengeance.Value;
            }
        }
    }

}
