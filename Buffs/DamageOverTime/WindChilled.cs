using System;
using CalamityMod.DataStructures;
using CalamityMod.Dusts;
using CalamityMod.Particles;
using CalamityMod.Projectiles.Typeless;
using CalamityMod.Systems.Collections;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Buffs.DamageOverTime
{
    public class WindChilled : ModBuff
    {
        public static DebuffData debuffData = new DebuffData()
        {
            EnemyLostRegen = 12,
            NPCLifeRegenMethod = WindChilledNPCLifeRegen,
            ColdDebuffScaling = 1
        };
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = true;
            BuffID.Sets.LongerExpertDebuff[Type] = true;
            CalamityBuffSets.DebuffDataset[Type] = debuffData;
        }
        public static void WindChilledNPCLifeRegen(NPC npc, int buffType, ref int buffIndex, ref int damage)
        {
            var cnpc = npc.Calamity();

            int baseDoTValue = (int)(cnpc.windChilledMult * npc.Calamity().ActiveColdDebuffMultiplier.ApplyTo(debuffData.EnemyLostRegen));
            cnpc.ApplyDPSDebuff(baseDoTValue, Math.Max((int)(baseDoTValue * debuffData.MultiplierDamageTickSize), debuffData.MinimumDamageTickSize) / 2, ref npc.lifeRegen, ref damage);
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.Calamity().windChilled = true;
        }

        public override void Update(NPC npc, ref int buffIndex)
        {
            npc.Calamity().windChilled = true;
        }

        internal static void DrawEffects(PlayerDrawSet drawInfo, bool hasDebuffResistance = false)
        {
            Player player = drawInfo.drawPlayer;

            float widthMult = 1;
            float hightMult = 1;
            Color color = Elumphant.color1;
            Color color2 = Elumphant.color2;
            float sine = (float)Math.Sin((Main.GlobalTimeWrappedHourly * 9.0f) / MathHelper.Pi);
            if (Main.rand.NextBool(16))
            {
                bool start = !Main.rand.NextBool(4);
                Vector2 particleVel = Vector2.UnitY * Main.rand.NextFloat(-1.25f, 1.25f) * Math.Max(hightMult, widthMult) * (Main.rand.NextBool(5) ? 1.5f : 1) + player.velocity * 0.85f;
                Vector2 particlePos = player.Center + Main.rand.NextVector2Circular(3 * widthMult + 8, 3 * widthMult + 8) * Math.Max(hightMult, widthMult);
                Particle mist = new CustomPulsingSpark(particlePos, particleVel, "CalamityMod/Particles/ThinSparkle", "CalamityMod/Particles/BloomCircle", false, 65, Main.rand.NextFloat(0.95f, 1.35f) * MathF.Pow(widthMult, 0.45f), start ? color : color2, start ? color2 : color,
                    new Vector2(0.6f, 1.2f), true, true, Main.rand.Next(4, 7 + 1), colorFadeSpeed: 0.85f, noShrink: true, extraRotation: -particleVel.ToRotation(), shrinkSpeed: 0.1f, sineRate: Main.rand.NextFloat(0.25f, 0.55f), sineIntensity: (int)(Main.rand.Next(12, 19 + 1) * Math.Max(hightMult, widthMult)));
                GeneralParticleHandler.SpawnParticle(mist, true, Enums.GeneralDrawLayer.AfterPlayers);
            }
            if (Main.rand.NextBool(11))
            {
                Vector2 dustVel = Vector2.One.RotatedByRandom(MathHelper.TwoPi) * Main.rand.NextFloat(0.2f, 1.1f) * Math.Max(hightMult, widthMult) * 2 * (Main.rand.NextBool(5) ? 2f : 1);
                Dust dust2 = Dust.NewDustPerfect(player.Center + dustVel * Main.rand.NextFloat(3, 4), ModContent.DustType<SquashDustPixelated>(),
                        Vector2.Zero, 0, default, Main.rand.NextFloat(0.2f, 0.45f) * Math.Max(hightMult, widthMult));
                dust2.noGravity = false;
                dust2.color = Main.rand.NextBool() ? color : color2;
                dust2.customData = new Vector2(0.7f, 1.4f);
                dust2.fadeIn = 0.1f * Math.Max(hightMult, widthMult);
            }
        }

        internal static void DrawEffects(NPC npc, ref Color drawColor)
        {
            float maxSizeMult = 10f;
            float widthMult = MathF.Pow(Utils.Remap(npc.width, 10, 500, 1f, maxSizeMult, false), 0.8f);
            float hightMult = MathF.Pow(Utils.Remap(npc.height, 10, 500, 1f, maxSizeMult, false), 0.8f);
            Color color = Elumphant.color1;
            Color color2 = Elumphant.color2;
            float sine = (float)Math.Sin((Main.GlobalTimeWrappedHourly * 9.0f) / MathHelper.Pi);
            if (Main.rand.NextBool(16))
            {
                bool start = !Main.rand.NextBool(4);
                Vector2 particleVel = Vector2.UnitY * Main.rand.NextFloat(-1.25f, 1.25f) * Math.Max(hightMult, widthMult) * (Main.rand.NextBool(5) ? 1.5f : 1) + npc.velocity * 0.85f;
                Vector2 particlePos = npc.Center + Main.rand.NextVector2Circular(3 * widthMult + 8, 3 * widthMult + 8) * Math.Max(hightMult, widthMult);
                Particle mist = new CustomPulsingSpark(particlePos, particleVel, "CalamityMod/Particles/ThinSparkle", "CalamityMod/Particles/BloomCircle", false, 65, Main.rand.NextFloat(0.95f, 1.35f) * MathF.Pow(widthMult, 0.45f), start ? color : color2, start ? color2 : color,
                    new Vector2(0.6f, 1.2f), true, true, Main.rand.Next(4, 7 + 1), colorFadeSpeed: 0.85f, noShrink: true, extraRotation: -particleVel.ToRotation(), shrinkSpeed: 0.1f, sineRate: Main.rand.NextFloat(0.25f, 0.55f), sineIntensity: (int)(Main.rand.Next(12, 19 + 1) * Math.Max(hightMult, widthMult)));
                GeneralParticleHandler.SpawnParticle(mist, true, Main.rand.NextBool() ? Enums.GeneralDrawLayer.AfterNPCs : Enums.GeneralDrawLayer.BeforeNPCs);
            }
            if (Main.rand.NextBool(11))
            {
                Vector2 dustVel = Vector2.One.RotatedByRandom(MathHelper.TwoPi) * Main.rand.NextFloat(0.2f, 1.1f) * Math.Max(hightMult, widthMult) * 2 * (Main.rand.NextBool(5) ? 2f : 1);
                Dust dust2 = Dust.NewDustPerfect(npc.Center + dustVel * Main.rand.NextFloat(3, 4), ModContent.DustType<SquashDustPixelated>(),
                        Vector2.Zero, 0, default, Main.rand.NextFloat(0.2f, 0.45f) * Math.Max(hightMult, widthMult));
                dust2.noGravity = false;
                dust2.color = Main.rand.NextBool() ? color : color2;
                dust2.customData = new Vector2(0.7f, 1.4f);
                dust2.fadeIn = 0.1f * Math.Max(hightMult, widthMult);
            }
        }
    }
}
