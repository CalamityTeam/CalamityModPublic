using System;
using System.Collections.Generic;
using System.Linq;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.DataStructures;
using CalamityMod.Dusts;
using CalamityMod.Particles;
using CalamityMod.Systems.Collections;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Typeless
{
    public class DebuffSpreadEffect : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Typeless";
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";
        public Player Owner => Main.player[Projectile.owner];
        public List<int> listNPCs = new List<int>();
        public NPC lastNPC;
        public int minCooldown = 48;
        public static Color startingColor = Bane.baneColor2;
        public Color mainColor = Color.White;
        public Color subColor = startingColor;
        public Color extraColor = Color.White;
        public ref float spreadCount => ref Projectile.ai[2];
        public ref float time => ref Projectile.ai[0];
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 1;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 9000;
            Projectile.tileCollide = false;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
        }

        public override void AI()
        {
            NPC npc = (lastNPC == null ? Main.npc[(int)Projectile.ai[1]] : lastNPC);

            if (npc != null && spreadCount != 0)
            {
                if (time % 9 == 0)
                {
                    listNPCs.Add(npc.whoAmI);
                    Projectile.Center = npc.Center;
                    mainColor = Color.White;

                    bool strong = npc.Calamity().apollyonEffected;
                    float areaOfEffect = (strong ? 1400 : 800);
                    int debuffTime = (strong ? 180 : 120);
                    int startDir = Main.rand.NextBool() ? 1 : -1;

                    // Initial infliction lasts longer, hopefully reduces the power imbalance between this accessory with fast weapons vs slow weapons
                    npc.AddBuff(ModContent.BuffType<Bane>(), debuffTime * 2);

                    float distanceFromCenter = areaOfEffect;
                    NPC targeted = null;
                    // Get closest valid target
                    for (int index = 0; index < Main.npc.Length; index++)
                    {
                        NPC closestTarget = Main.npc[index];
                        if (closestTarget.CanBeChasedBy(null, false) && closestTarget != npc && !listNPCs.Contains(closestTarget.whoAmI) && closestTarget.life > 0)
                        {
                            float extraDistance = (closestTarget.width / 2) + (closestTarget.height / 2);
                            if (Vector2.Distance(Projectile.Center, closestTarget.Center) < distanceFromCenter)
                            {
                                distanceFromCenter = Vector2.Distance(Projectile.Center, closestTarget.Center);
                                targeted = closestTarget;
                            }
                        }
                    }

                    if (targeted != null)
                    {
                        targeted.Calamity().apollyonEffected = npc.Calamity().apollyonEffected;
                        targeted.Calamity().abaddonEffected = npc.Calamity().abaddonEffected;

                        float bestDamage = Owner.Calamity().playerBaneDebuffDamage;
                        int heat = 0;
                        Color heatClr = Color.OrangeRed;
                        int sick = 0;
                        Color sickClr = Color.Lime;
                        int cold = 0;
                        Color coldClr = Color.RoyalBlue;
                        int shock = 0;
                        Color shockClr = Color.Gold;
                        int water = 0;
                        Color waterClr = Color.DarkTurquoise;
                        // Find all the debuffs on the enemy
                        int debuffNum = 0;

                        for (int index = 0; index < npc.buffType.Length; index++)
                        {
                            int type = npc.buffType[index];
                            var debuffData = CalamityBuffSets.DebuffDataset[type];

                            // Calculate the Bane damage to see if the debuffs can be transfered
                            int bDamage = Math.Max((int)(Bane.debuffData.EnemyLostRegen * bestDamage), (int)Bane.debuffData.EnemyLostRegen);

                            if (CalamityBuffSets.IsDebuff[type])
                            {
                                if (debuffData == null || debuffData.EnemyLostRegen <= bDamage)
                                {
                                    debuffNum++;
                                    float timeMult = (debuffData == null || (debuffData != null && debuffData == Bane.debuffData)) ? 1 : Utils.Remap(debuffData.EnemyLostRegen, 0, bDamage, 5, 1);
                                    targeted.AddBuff(type, (int)(debuffTime * timeMult));
                                }
                                if (debuffData != null)
                                {
                                    if (debuffData.HeatDebuffScaling > 0) { mainColor = Color.Lerp(mainColor, heatClr, 0.65f); heat++; }
                                    if (debuffData.SicknessDebuffScaling > 0) { mainColor = Color.Lerp(mainColor, sickClr, 0.65f); sick++; }
                                    if (debuffData.ColdDebuffScaling > 0) { mainColor = Color.Lerp(mainColor, coldClr, 0.65f); cold++; }
                                    if (debuffData.ElectricDebuffScaling > 0) { mainColor = Color.Lerp(mainColor, shockClr, 0.65f); shock++; }
                                    if (debuffData.WaterDebuffScaling > 0) { mainColor = Color.Lerp(mainColor, waterClr, 0.65f); water++; }
                                    if (new List<int> { heat, sick, cold, shock, water }.Max() == 0)
                                    {
                                        mainColor = Bane.baneColor1;
                                    }
                                }
                            }
                            // Edge case fixes for Shred and Demonic Flames
                            // Not ideal but not sure how else to do it
                            if (npc.Calamity().somaShredStacks > 0 && Shred.BaseDamage <= bDamage)
                                targeted.AddBuff(ModContent.BuffType<Shred>(), debuffTime);
                            int demonicFlames = ModContent.GetModBuff(ModContent.BuffType<DemonicFlames>()).Type;
                            if (npc.HasBuff(demonicFlames))
                                targeted.Calamity().demonicFlamesBonusDamage = npc.Calamity().demonicFlamesBonusDamage;
                        }
                        if (debuffNum == 0)
                        {
                            KillSelf();
                            return;
                        }

                        List<int> best = new List<int> { heat, sick, cold, shock, water };
                        int highest = best.Max();
                        if (highest != 0)
                        {
                            // Ideally these lines would run in a random order, so that fire isn't prioritsed as an extra color, but I don't know how to do that!
                            if (heat == highest && extraColor == Color.White) { if (subColor != startingColor) { extraColor = heatClr; } else { subColor = heatClr; } }
                            if (sick == highest && extraColor == Color.White) { if (subColor != startingColor) { extraColor = sickClr; } else { subColor = sickClr; } }
                            if (cold == highest && extraColor == Color.White) { if (subColor != startingColor) { extraColor = coldClr; } else { subColor = coldClr; } }
                            if (shock == highest && extraColor == Color.White) { if (subColor != startingColor) { extraColor = shockClr; } else { subColor = shockClr; } }
                            if (water == highest && extraColor == Color.White) { if (subColor != startingColor) { extraColor = waterClr; } else { subColor = waterClr; } }
                        }

                        VisualEffects(npc, targeted, startDir, 1 + (best.Sum() * 0.12f));

                        Projectile.ForceNetUpdate();
                        targeted.ForceNetUpdate();
                        lastNPC = targeted;
                    }
                    else
                    {
                        KillSelf();
                        return;
                    }

                    if (Owner.Calamity().abaddonCooldown <= 0)
                        Owner.Calamity().abaddonCooldown = minCooldown * 2;
                    spreadCount--;
                }
            }
            else
            {
                KillSelf();
                return;
            }
            time++;
        }
        public void KillSelf()
        {
            if (Owner.Calamity().abaddonCooldown > minCooldown)
                Owner.Calamity().abaddonCooldown = minCooldown;
            if (Owner.Calamity().abaddonCooldown < 0)
                Owner.Calamity().abaddonCooldown = 0;
            Projectile.Kill();
        }
        public void VisualEffects(NPC npc, NPC targeted, int startDirection, float effectPower)
        {
            Vector2 start = npc.Center;
            Vector2 end = targeted.Center;
            if (Owner.Calamity().abaddonEffectVisual)
            {
                // Spawn a single particle line between the first target and the new target
                Vector2 lerpVel = Vector2.Lerp(start, end, 0.5f);
                float scale = 0.015f;
                Particle spark = new CustomSpark(lerpVel, npc.SafeDirectionTo(targeted.Center), "CalamityMod/Particles/BloomLineSoftEdge", false, 18, scale, mainColor, new Vector2(1.2f * effectPower, (Utils.Distance(start, end) * 0.038f)), true, true, shrinkSpeed: 0.25f, glowOpacity: 0.75f);
                GeneralParticleHandler.SpawnParticle(spark);

                for (int g = 0; g < 7; g++)
                {
                    int DustID = ModContent.DustType<LightDust>();
                    Dust dust2 = Dust.NewDustDirect(targeted.Center, targeted.width, targeted.height, DustID);
                    dust2.scale = Main.rand.NextFloat(0.6f, 0.75f);
                    dust2.velocity = new Vector2(4, 4).RotatedByRandom(100) * Main.rand.NextFloat(0.5f, 0.8f);
                    dust2.noGravity = true;
                    dust2.color = Main.rand.NextBool() ? subColor : (extraColor != Color.White ? extraColor : mainColor);
                }

                float distance = Vector2.Distance(targeted.Center, npc.Center);

                // This might look like a lot of dust for a thing that can happen to at few enemies in succession, but it
                // does get reduced significantly based on distance and lasts only a few frames.
                int averageDusts = (int)(65 * (float)Math.Pow(effectPower, 0.3f));
                int maxDusts = (int)((float)Math.Pow(Math.Max(distance, 1) / 1000, 0.65f) * averageDusts);

                Vector2 dustLineStart = start;
                Vector2 dustLineEnd = end;
                Vector2 currentDustPos = default;
                Vector2 dustVel = start.DirectionTo(end);
                int startingPoint = Main.rand.Next(0, 400 + 1);
                Vector2 lastDustPos = default;
                for (int i = 0; i < maxDusts; i++)
                {
                    float sineSpeed = Utils.Remap(startingPoint, 0, 400, 1.6f, 0.7f);
                    float sine = (float)Math.Sin((i + startingPoint) * sineSpeed / MathHelper.Pi) * startDirection;
                    float endStartFade = Math.Min(Utils.GetLerpValue(maxDusts * 0.9f, maxDusts * 0.1f, i, true), Utils.GetLerpValue(maxDusts * 0.1f, maxDusts * 0.9f, i, true));
                    currentDustPos = Vector2.Lerp(dustLineStart, dustLineEnd, i / (float)maxDusts) + dustVel.RotatedBy(MathHelper.PiOver2) * 6 * sine * endStartFade;
                    if (i == 0)
                        lastDustPos = currentDustPos;

                    bool wildDust = Main.rand.NextBool(5);
                    currentDustPos = Vector2.Lerp(dustLineStart, dustLineEnd, i / (float)maxDusts) + dustVel.RotatedBy(MathHelper.PiOver2) * 55 * sine * endStartFade * effectPower;
                    Dust dustLinger = Dust.NewDustPerfect(currentDustPos, ModContent.DustType<SquashDust>());
                    dustLinger.position = currentDustPos;
                    dustLinger.velocity = currentDustPos.DirectionTo(lastDustPos) * (wildDust ? 6.5f : Main.rand.NextFloat(0.2f, 0.8f));
                    dustLinger.noGravity = true;
                    dustLinger.scale = Main.rand.NextFloat(0.8f, 1f) * (wildDust ? 1.7f : 1.5f) * effectPower;
                    dustLinger.fadeIn = Main.rand.NextFloat(0.6f, 1f) * 4 * effectPower * (wildDust ? 0.35f : 1);
                    dustLinger.color = Color.Lerp((extraColor != Color.White ? extraColor : mainColor), subColor, Utils.GetLerpValue(0, maxDusts, i));
                    if (wildDust)
                        dustLinger.color = !Main.rand.NextBool(4) ? subColor : (extraColor != Color.White ? extraColor : mainColor);
                    if (!wildDust)
                    {
                        dustLinger.noLightEmittence = true;
                        dustLinger.customData = new Vector2(0.8f, 3);
                    }
                    else
                        dustLinger.customData = new Vector2(0.6f, 1.5f);

                    lastDustPos = currentDustPos;
                }
            }
            for (int u = 0; u < 2; u++)
            {
                Vector2 pos = start;
                if (u == 0) pos = end;
                Particle spark3 = new CustomSpark(pos, Vector2.Zero, "CalamityMod/Particles/BloomCircle", false, 18, u != 0 ? 0.7f : 0.55f, (u == 0 ? subColor : mainColor) * (Owner.Calamity().abaddonEffectVisual ? 1 : 0.3f), Vector2.One, true, true, glowOpacity: 0.85f);
                GeneralParticleHandler.SpawnParticle(spark3);
            }
        }
        public override bool? CanDamage() => false;
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) => false;
    }
}
