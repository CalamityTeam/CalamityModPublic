using System;
using CalamityMod.NPCs;
using CalamityMod.Particles;
using CalamityMod.Projectiles.Typeless;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Buffs.DamageOverTime
{
    public class Shred : ModBuff
    {
        internal const int StackFalloffFrames = 320;

        // 150 DPS (30x5) per stack may seem low, but it gets boosted by ranged stats and can supercrit.
        internal static int BaseDamage = 30;
        internal static int FramesPerDamageTick = 12;

        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = true;
        }

        public override void Update(NPC npc, ref int buffIndex)
        {
            CalamityGlobalNPC cgn = npc.Calamity();
            if (cgn.somaShredStacks <= 0)
            {
                cgn.somaShredStacks = 1;
                cgn.somaShredFalloff = StackFalloffFrames;
            }
            else
            {
                ++cgn.somaShredStacks;
            }

            // As soon as the debuff is applied to the NPC, subsume it into Calamity's tracking system
            npc.DelBuff(buffIndex);
            buffIndex--;
        }

        internal static void TickDebuff(NPC target, CalamityGlobalNPC cgn)
        {
            // Decrement the falloff counter every frame.
            cgn.somaShredFalloff -= cgn.somaShredStacks;
            if (cgn.somaShredFalloff <= 0)
            {
                cgn.somaShredFalloff += StackFalloffFrames;
                --cgn.somaShredStacks;
            }

            // If a valid player applied this debuff, then let it deal damage.
            if (cgn.somaShredApplicator >= 0 && cgn.somaShredApplicator < Main.maxPlayers && Main.myPlayer == cgn.somaShredApplicator)
            {
                Player applicator = Main.player[cgn.somaShredApplicator];

                // Only deal damage once every several frames.
                if (applicator.miscCounter % FramesPerDamageTick == 0)
                {
                    // This cannot be done with SimpleStrikeNPC because that would not allow for supercrits.
                    int bleedTickDamage = (int)applicator.GetTotalDamage<RangedDamageClass>().ApplyTo(BaseDamage * cgn.somaShredStacks);
                    Projectile tick = Projectile.NewProjectileDirect(target.GetSource_FromThis(), target.Center, Vector2.Zero, ModContent.ProjectileType<DirectStrike>(), bleedTickDamage, 0f, applicator.whoAmI, target.whoAmI);
                    tick.DamageType = DamageClass.Ranged; // Uncommon for DirectStrikes, but it needs to be able to crit.
                    tick.Calamity().supercritHits = -1;
                }
            }
        }

        internal static void DrawEffects(Player player)
        {

        }

        // Enemies suffering from Soma Prime's Shred spray blood like Violence
        internal static void DrawEffects(NPC npc, CalamityGlobalNPC cgn, ref Color drawColor)
        {
            // The amount of blood particles spawned by Soma Prime's Shred scales loosely with the number of stacks.
            float roughBloodCount = (float)Math.Sqrt(0.8f * cgn.somaShredStacks);
            int exactBloodCount = (int)roughBloodCount;
            // Chance for the final blood particle
            if (Main.rand.NextFloat() < roughBloodCount - exactBloodCount)
                ++exactBloodCount;

            // Velocity of the spurting blood also slightly increases with stacks.
            float velStackMult = 1f + (float)Math.Log(cgn.somaShredStacks);

            // Code copied from Violence.
            for (int i = 0; i < exactBloodCount; ++i)
            {
                int bloodLifetime = Main.rand.Next(22, 36);
                float bloodScale = Main.rand.NextFloat(0.6f, 0.8f);
                Color bloodColor = Color.Lerp(Color.Red, Color.DarkRed, Main.rand.NextFloat());
                bloodColor = Color.Lerp(bloodColor, new Color(51, 22, 94), Main.rand.NextFloat(0.65f));

                if (Main.rand.NextBool(20))
                    bloodScale *= 2f;

                float randomSpeedMultiplier = Main.rand.NextFloat(1.25f, 2.25f);
                Vector2 bloodVelocity = Main.rand.NextVector2Unit() * velStackMult * randomSpeedMultiplier;
                bloodVelocity.Y -= 5f;
                BloodParticle blood = new BloodParticle(npc.Center, bloodVelocity, bloodLifetime, bloodScale, bloodColor);
                GeneralParticleHandler.SpawnParticle(blood);
            }
            for (int i = 0; i < exactBloodCount / 3; ++i)
            {
                float bloodScale = Main.rand.NextFloat(0.2f, 0.33f);
                Color bloodColor = Color.Lerp(Color.Red, Color.DarkRed, Main.rand.NextFloat(0.5f, 1f));
                Vector2 bloodVelocity = Main.rand.NextVector2Unit() * velStackMult * Main.rand.NextFloat(1f, 2f);
                bloodVelocity.Y -= 2.3f;
                BloodParticle2 blood = new BloodParticle2(npc.Center, bloodVelocity, 20, bloodScale, bloodColor);
                GeneralParticleHandler.SpawnParticle(blood);
            }
        }
    }
}
