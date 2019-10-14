using CalamityMod.CalPlayer;
using CalamityMod.Items;
using CalamityMod.NPCs;
using CalamityMod.Projectiles;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;
using Terraria;

namespace CalamityMod
{
    public static class CalamityUtils
    {
        #region Object Extensions
        public static CalamityPlayer Calamity(this Player player) => player.GetModPlayer<CalamityPlayer>();
        public static CalamityGlobalNPC Calamity(this NPC npc) => npc.GetGlobalNPC<CalamityGlobalNPC>();
        public static CalamityGlobalItem Calamity(this Item item) => item.GetGlobalItem<CalamityGlobalItem>();
        public static CalamityGlobalProjectile Calamity(this Projectile proj) => proj.GetGlobalProjectile<CalamityGlobalProjectile>();
        #endregion

        #region Player Utilities
        public static bool InCalamity(this Player player) => player.Calamity().ZoneCalamity;
        public static bool InAstral(this Player player) => player.Calamity().ZoneAstral;
        public static bool InSunkenSea(this Player player) => player.Calamity().ZoneSunkenSea;
        public static bool InSulphur(this Player player) => player.Calamity().ZoneSulphur;
        public static bool InAbyss(this Player player, int layer = 0)
        {
            switch (layer)
            {
                case 1:
                    return player.Calamity().ZoneAbyssLayer1;

                case 2:
                    return player.Calamity().ZoneAbyssLayer2;

                case 3:
                    return player.Calamity().ZoneAbyssLayer3;

                case 4:
                    return player.Calamity().ZoneAbyssLayer4;

                default:
                    return player.Calamity().ZoneAbyss;
            }
        }

        public static bool InventoryHas(this Player player, params int[] items)
        {
            return player.inventory.Any(item => items.Contains(item.type));
        }
        #endregion

        #region NPC Utilities
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
            return ClosestNPCAt(origin, maxDistanceToCheck);
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
        #endregion

        #region Miscellaneous Utilities
        public static void AddWithCondition<T>(this List<T> list, T type, bool condition)
        {
            if (condition)
                list.Add(type);
        }
        public static Rectangle FixSwingHitbox(float hitboxWidth, float hitboxHeight)
        {
            Player player = Main.player[Main.myPlayer];
            Item item = player.inventory[player.selectedItem];
            float hitbox_X = 0, hitbox_Y = 0;
            float num = player.mount.PlayerOffsetHitbox;
            //Third hitbox
            if (player.itemAnimation < player.itemAnimationMax * 0.333)
            {
                float num38 = 10f;
                if (hitboxWidth >= 92)
                    num38 = 38f;
                else if (hitboxWidth >= 64)
                    num38 = 28f;
                else if (hitboxWidth >= 52)
                    num38 = 24f;
                else if (hitboxWidth > 32)
                    num38 = 14f;
                hitbox_X = player.position.X + player.width * 0.5f + (hitboxWidth * 0.5f - num38) * player.direction;
                hitbox_Y = player.position.Y + 24f + num;
            }
            //Second hitbox
            else if (player.itemAnimation < player.itemAnimationMax * 0.666)
            {
                float num39 = 10f;
                if (hitboxWidth >= 92)
                    num39 = 38f;
                else if (hitboxWidth >= 64)
                    num39 = 28f;
                else if (hitboxWidth >= 52)
                    num39 = 24f;
                else if (hitboxWidth > 32)
                    num39 = 18f;
                hitbox_X = player.position.X + (player.width * 0.5f + (hitboxWidth * 0.5f - num39) * player.direction);

                num39 = 10f;
                if (hitboxHeight > 64)
                    num39 = 14f;
                else if (hitboxHeight > 52)
                    num39 = 12f;
                else if (hitboxHeight > 32)
                    num39 = 8f;

                hitbox_Y = player.position.Y + num39 + num;
            }
            //First hitbox
            else
            {
                    float num40 = 6f;
                if (hitboxWidth >= 92)
                    num40 = 38f;
                else if (hitboxWidth >= 64)
                    num40 = 28f;
                else if (hitboxWidth >= 52)
                    num40 = 24f;
                else if (hitboxWidth >= 48)
                    num40 = 18f;
                else if (hitboxWidth > 32) 
                    num40 = 14f;
                hitbox_X = player.position.X + player.width * 0.5f - (hitboxWidth * 0.5f - num40) * player.direction;

                num40 = 10f;
                if (hitboxHeight > 64)
                    num40 = 14f;
                else if (hitboxHeight > 52)
                    num40 = 12f;
                else if (hitboxHeight > 32) 
                    num40 = 10f;      
                hitbox_Y = player.position.Y + num40 + num;
            }
            if (player.gravDir == -1f)
            {
                hitbox_Y = player.position.Y + player.height + (player.position.Y - hitbox_Y);
            }
            //Hitbox size
            Rectangle hitbox = new Rectangle((int)hitbox_X, (int)hitbox_Y, 32, 32);
            if (item.damage >= 0 && item.type > 0 && !item.noMelee && player.itemAnimation > 0) 
            {

                if (!Main.dedServ) 
                {
                    hitbox = new Rectangle((int)hitbox_X, (int)hitbox_Y, (int)hitboxWidth, (int)hitboxHeight);
                }
                hitbox.Width = (int)(hitbox.Width * item.scale);
                hitbox.Height = (int)(hitbox.Height * item.scale);
                if (player.direction == -1) 
                {
                    hitbox.X -= hitbox.Width;
                }
                if (player.gravDir == 1f) 
                {
                    hitbox.Y -= hitbox.Height;
                }
                if (item.useStyle == 1) 
                {
                    //Third hitbox
                    if (player.itemAnimation < player.itemAnimationMax * 0.333)
                    {
                        if (player.direction == -1)
                        {
                            hitbox.X -= (int)(hitbox.Width * 1.4 - hitbox.Width);
                        }
                        hitbox.Width = (int)(hitbox.Width * 1.4);
                        hitbox.Y += (int)(hitbox.Height * 0.5 * player.gravDir);
                        hitbox.Height = (int)(hitbox.Height * 1.1);
                    }
                    //First hitbox
                    else if (player.itemAnimation >= player.itemAnimationMax * 0.666) 
                    {
                        if (player.direction == 1) 
                        {
                            hitbox.X -= (int)(hitbox.Width * 1.2);
                        }
                        hitbox.Width *= 2;
                        hitbox.Y -= (int)((hitbox.Height * 1.4 - hitbox.Height) * player.gravDir);
                        hitbox.Height = (int)(hitbox.Height * 1.4);
                    }
                }
            }
                return hitbox;
        }

        #endregion

        #region Projectile Utilities
        public static int CountProjectiles(int Type) => Main.projectile.Count(proj => proj.type == Type && proj.active);
        #endregion
    }
}
