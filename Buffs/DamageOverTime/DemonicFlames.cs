using System;
using CalamityMod.DataStructures;
using CalamityMod.Dusts;
using CalamityMod.Particles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Systems.Collections;

namespace CalamityMod.Buffs.DamageOverTime
{
    public class DemonicFlames : ModBuff
    {
        public static DebuffData debuffData = new DebuffData()
        {
            EnemyLostRegen = 60, //Unused in the method, this is the amount of DoT from Forbidden Oathblade demon flames
            HeatDebuffScaling = 1, //Unused in the method, but kept so other things can know this is a heat debuff
            NPCLifeRegenMethod = DemonicFlamesNPCLifeRegen
        };
        public static void DemonicFlamesNPCLifeRegen(NPC npc, int buffType, ref int buffIndex, ref int damage)
        {
            int baseDemonicFlamesDoTValue = (int)Math.Max(Math.Max(npc.Calamity().ActiveHeatDebuffMultiplier.ApplyTo(npc.Calamity().demonicFlamesBonusDamage), npc.Calamity().demonicFlamesBonusDamage), (int)debuffData.EnemyLostRegen);
            npc.Calamity().ApplyDPSDebuff(baseDemonicFlamesDoTValue, baseDemonicFlamesDoTValue / 15, ref npc.lifeRegen, ref damage);
        }
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = true;
            BuffID.Sets.LongerExpertDebuff[Type] = true;
            CalamityBuffSets.DebuffDataset[Type] = debuffData;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.Calamity().demonicFlames = true;
        }

        public override void Update(NPC npc, ref int buffIndex)
        {
            npc.Calamity().demonicFlames = true;
        }

        internal static void DrawEffects(PlayerDrawSet drawInfo, bool hasDebuffResistance = false)
        {
            Player Player = drawInfo.drawPlayer;

            if (Main.rand.NextBool())
            {
                Dust dust = Dust.NewDustDirect(Player.position, Player.width, Player.height, ModContent.DustType<LightDust>());
                dust.noGravity = true;
                dust.velocity = new Vector2(0, Main.rand.NextFloat(-4f, -8f)).RotatedByRandom(0.3f) + Player.velocity;
                dust.scale = Main.rand.NextFloat(0.8f, 1.2f);
                dust.color = Main.rand.NextBool() ? Color.MediumOrchid : Color.BlueViolet;
                dust.noLightEmittence = true;
                for (int i = 0; i < 2; i++)
                {
                    Vector2 sparkVel = new Vector2(Main.rand.NextFloat(-6f, 6f), Main.rand.NextFloat(-1f, -3f));
                    Particle sparks = new VelChangingSpark(Player.Center + new Vector2(Main.rand.NextFloat(-10f, 10f), Player.height / 2), sparkVel + Player.velocity, new Vector2(-sparkVel.X * 0.5f, sparkVel.Y * 2) * 3.5f, "CalamityMod/Particles/SmallBloom", Main.rand.Next(13, 20 + 1), Main.rand.NextFloat(0.1f, 0.25f) * 0.5f, (Main.rand.NextBool() ? Color.MediumOrchid : Color.BlueViolet) * 0.75f, new Vector2(0.7f, 1), true, false, 0, false, 0.45f, 0.055f);
                    GeneralParticleHandler.SpawnParticle(sparks);
                }
            }
            Lighting.AddLight(Player.Center, Color.MediumOrchid.ToVector3());
        }

        internal static void DrawEffects(NPC npc, ref Color drawColor)
        {
            if (Main.rand.NextBool(3))
            {
                Dust dust = Dust.NewDustDirect(npc.position, npc.width, npc.height, ModContent.DustType<LightDust>());
                dust.noGravity = true;
                dust.velocity = new Vector2(0, Main.rand.NextFloat(-4f, -8f)).RotatedByRandom(0.3f) + npc.velocity;
                dust.scale = Main.rand.NextFloat(0.8f, 1.2f);
                dust.color = Main.rand.NextBool() ? Color.MediumOrchid : Color.BlueViolet;
                dust.noLightEmittence = true;
                for (int i = 0; i < 2; i++)
                {
                    Vector2 sparkVel = new Vector2(Main.rand.NextFloat(-npc.width / 6, npc.width / 6), Main.rand.NextFloat(-npc.height / 20, -npc.height / 17));
                    Particle sparks = new VelChangingSpark(npc.Center + new Vector2(Main.rand.NextFloat(-10f, 10f), npc.height / 2) + sparkVel * 0.5f, sparkVel + npc.velocity, new Vector2(-sparkVel.X * 0.5f, sparkVel.Y * 2) * 3.5f, "CalamityMod/Particles/SmallBloom", Main.rand.Next(13, 20 + 1), Main.rand.NextFloat(0.1f, 0.25f) * MathHelper.Lerp(Math.Max(npc.height, npc.width) / 120, 0.5f, 0.7f), (Main.rand.NextBool() ? Color.MediumOrchid : Color.BlueViolet) * 0.75f, new Vector2(0.7f, 1), true, false, 0, false, 0.3f, 0.055f);
                    GeneralParticleHandler.SpawnParticle(sparks);
                }
            }
            Lighting.AddLight(npc.Center, Color.MediumOrchid.ToVector3());
        }
    }
}
