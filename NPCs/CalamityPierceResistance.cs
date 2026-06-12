using System;
using System.Collections.Generic;
using System.Reflection;
using CalamityMod.Balancing;
using CalamityMod.Items.Accessories;
using CalamityMod.Systems.Collections;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace CalamityMod.NPCs
{
    public sealed class PierceResistNPC : GlobalNPC
    {
        internal static HashSet<int> exemptProjectiles;
        internal static HashSet<int> pierceResistNPC;
        internal static HashSet<int> singleHitboxNPC;
        internal static Dictionary<int, bool> singleHitboxExemptProjectiles;

        public override void Load()
        {
            exemptProjectiles = new();
            pierceResistNPC = new();
            singleHitboxNPC = new();
            singleHitboxExemptProjectiles = new();
        }

        public override void Unload()
        {
            exemptProjectiles?.Clear();
            exemptProjectiles = null;
            pierceResistNPC?.Clear();
            pierceResistNPC = null;
            singleHitboxNPC?.Clear();
            singleHitboxNPC = null;
            singleHitboxExemptProjectiles?.Clear();
            singleHitboxExemptProjectiles = null;
        }

        public override void SetStaticDefaults()
        {
            // Specific vanilla NPC's with pierce resistance
            pierceResistNPC.Add(NPCID.EaterofWorldsHead);
            pierceResistNPC.Add(NPCID.EaterofWorldsBody);
            pierceResistNPC.Add(NPCID.EaterofWorldsTail);
            pierceResistNPC.Add(NPCID.Creeper);
            pierceResistNPC.Add(NPCID.TheDestroyer);
            pierceResistNPC.Add(NPCID.TheDestroyerBody);
            pierceResistNPC.Add(NPCID.TheDestroyerTail);

            // Specific vanilla projectile exemptions
            exemptProjectiles.Add(ProjectileID.ChargedBlasterLaser);
            exemptProjectiles.Add(ProjectileID.ClingerStaff);
            exemptProjectiles.Add(ProjectileID.FinalFractal);
            exemptProjectiles.Add(ProjectileID.FlyingKnife);
            exemptProjectiles.Add(ProjectileID.HallowJoustingLance); // "Why not make jousting lances exempt via AI style?" Because they use spear AI. Thanks vanilla!
            exemptProjectiles.Add(ProjectileID.JoustingLance);
            exemptProjectiles.Add(ProjectileID.LastPrismLaser);
            exemptProjectiles.Add(ProjectileType<MarniteRepulsionHitbox>()); // Included here as it does not have a projectile
            exemptProjectiles.Add(ProjectileID.ShadowJoustingLance);

            // Specific vanilla projectile single hitbox exemptions
            singleHitboxExemptProjectiles[ProjectileID.NettleBurstEnd] = true;
            singleHitboxExemptProjectiles[ProjectileID.NettleBurstLeft] = true;
            singleHitboxExemptProjectiles[ProjectileID.NettleBurstRight] = true;
            singleHitboxExemptProjectiles[ProjectileID.PrincessWeapon] = true;
            singleHitboxExemptProjectiles[ProjectileID.ToxicCloud] = true;
            singleHitboxExemptProjectiles[ProjectileID.ToxicCloud2] = true;
            singleHitboxExemptProjectiles[ProjectileID.ToxicCloud3] = true;

            var projectiles = GetContent<ModProjectile>();
            foreach (var projectile in projectiles)
            {
                try
                {
                    var type = projectile.GetType();
                    var pierceResistException = type.GetCustomAttribute<PierceResistExceptionAttribute>();
                    if (pierceResistException == null)
                        continue;

                    int projectileType = projectile.Type;

                    if (pierceResistException.OnlyForSingleHitbox)
                    {
                        singleHitboxExemptProjectiles[projectileType] = true;
                        continue;
                    }
                    else
                    {
                        exemptProjectiles.Add(projectileType);
                    }
                }
                catch (Exception e)
                {
                    CalamityMod.Log.Error($"Exception thrown while evaluating type \"{projectile.FullName}\": {e}");
                }
            }

            var npcs = GetContent<ModNPC>();
            foreach (var npc in npcs)
            {
                try
                {
                    var type = npc.GetType();
                    var hasPierceResist = type.GetCustomAttribute<HasPierceResistAttribute>();
                    if (hasPierceResist == null)
                        continue;

                    int npcType = npc.Type;

                    pierceResistNPC.Add(npcType);
                    if (hasPierceResist.SingleHitbox)
                        singleHitboxNPC.Add(npcType);
                }
                catch (Exception e)
                {
                    CalamityMod.Log.Error($"Exception thrown while evaluating type \"{npc.FullName}\": {e}");
                }
            }
        }

        public override void ModifyHitByProjectile(NPC npc, Projectile projectile, ref NPC.HitModifiers modifiers)
        {
            // Skip if NPC does not have pierce resistance or projectile is exempt
            if (!pierceResistNPC.Contains(npc.type) || exemptProjectiles.Contains(projectile.type))
                return;

            // Skip if the projectile is exempt on single hitboxes and the NPC is a single hitbox
            if (singleHitboxExemptProjectiles.TryGetValue(projectile.type, out bool isSingleHitboxExempt) && isSingleHitboxExempt && singleHitboxNPC.Contains(npc.type))
                return;

            // Skip specific vanilla AI styles, or if the projectile sets heldProj (meaning it's a holdout).
            if (projectile.aiStyle == ProjAIStyleID.Flail || projectile.aiStyle == ProjAIStyleID.MechanicalPiranha || projectile.aiStyle == ProjAIStyleID.Yoyo ||
                Main.player[projectile.owner].heldProj == projectile.whoAmI)
                return;

            PierceResistGlobal(projectile, npc, ref modifiers);
        }

        // Generalized pierce resistance that stacks with all other resistances for some specific bosses defined in a list.
        private void PierceResistGlobal(Projectile projectile, NPC npc, ref NPC.HitModifiers modifiers)
        {
            // Thanatos segments do not trigger pierce resistance if they are closed
            if (CalamityNPCTypeSets.Thanatos.Contains(npc.type) && npc.GetGlobalNPC<CalamityGlobalNPC>().unbreakableDR)
                return;

            float damageReduction = projectile.Calamity().timesPierced * BalancingConstants.PierceResistHarshness;
            if (damageReduction > BalancingConstants.PierceResistCap)
                damageReduction = BalancingConstants.PierceResistCap;

            modifiers.FinalDamage *= 1f - damageReduction;

            if ((projectile.penetrate > 1 || projectile.penetrate == -1) && !projectile.CountsAsClass<SummonDamageClass>())
                projectile.Calamity().timesPierced++;
        }
    }
}
