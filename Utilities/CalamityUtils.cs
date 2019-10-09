using CalamityMod.CalPlayer;
using CalamityMod.NPCs;
using CalamityMod.Projectiles;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
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
        public static CalamityGlobalNPC GetCalamityNPC(this NPC npc) => npc.GetGlobalNPC<CalamityGlobalNPC>();

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
            if (npc.boss)
            {
                double HPBoost = (double)Config.BossHealthPercentageBoost * 0.01;
			    npc.lifeMax += (int)((double)npc.lifeMax * HPBoost);
            }
        }
        /// <summary>
        /// Detects nearby hostile NPCs from a given point
        /// </summary>
        /// <param name="origin">The position where we wish to check for nearby NPCs</param>
        /// <param name="maxDistanceToCheck">Maximum amount of pixels to check around the origin</param>
        public static NPC ClosestNPCAt(this Vector2 origin, float maxDistanceToCheck)
        {
            NPC closestTarget = null;
            float distance = maxDistanceToCheck;
            for (int index = 0; index < Main.npc.Length; index++)
            {
                //doesn't matter what the attacker is in CanBeChasedBy? wtf.
                if (Main.npc[index].CanBeChasedBy(null, false) && Collision.CanHit(origin, 1, 1, Main.npc[index].Center, 1, 1))
                {
                    if (Vector2.Distance(origin, Main.npc[index].Center) < distance)
                    {
                        distance = Vector2.Distance(origin, Main.npc[index].Center);
                        closestTarget = Main.npc[index];
                    }
                }
            }
            return closestTarget;
        }
        /// <summary>
        /// Detects nearby hostile NPCs from a given point with minion support
        /// </summary>
        /// <param name="origin">The position where we wish to check for nearby NPCs</param>
        /// <param name="maxDistanceToCheck">Maximum amount of pixels to check around the origin</param>
        /// <param name="owner">Owner of the minion</param>
        public static NPC MinionHoming(this Vector2 origin, float maxDistanceToCheck, Player owner)
        {
            if (owner.HasMinionAttackTargetNPC)
            {
                return Main.npc[owner.MinionAttackTargetNPC];
            }
            return ClosestNPCAt(origin,maxDistanceToCheck);
        }

        /// <summary>
        /// Crude anti-butcher logic based on % max health.
        /// </summary>
        /// <param name="npc">The NPC attacked.</param>
        /// <param name="damage">How much damage the attack would deal.</param>
        /// <returns>Whether or not the anti-butcher was triggered.</returns>
        public static bool AntiButcher(NPC npc, ref double damage, float healthPercent)
        {
            if (damage <= npc.lifeMax * healthPercent)
                return false;
            damage = 0D;
            return true;
        }
    }

    public static class CProjectileUtils
    {
        public static CalamityGlobalProjectile GetCalamityProj(this Projectile proj) => proj.GetGlobalProjectile<CalamityGlobalProjectile>();
    }

    public static class CMiscUtils
    {
        public static void AddWithCondition<T>(this List<T> list, T type, bool condition)
        {
            if (condition)
            {
                list.Add(type);
            }
        }
    }
}
