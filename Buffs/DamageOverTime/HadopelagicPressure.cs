using CalamityMod.DataStructures;
using CalamityMod.Particles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Systems.Collections;

namespace CalamityMod.Buffs.DamageOverTime
{
    public class HadopelagicPressure : ModBuff
    {
        public static DebuffData debuffData = new DebuffData()
        {
            EnemyLostRegen = 350, //175 dps
            WaterDebuffScaling = 1
        };
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
            player.Calamity().hadopelagicPressure = true;
        }

        public override void Update(NPC npc, ref int buffIndex)
        {
            npc.Calamity().hadopelagicPressure = true;
        }

        internal static void DrawEffects(PlayerDrawSet drawInfo)
        {
            Player Player = drawInfo.drawPlayer;
            // Blue n black dust
            Dust water = Dust.NewDustDirect(drawInfo.Position - new Vector2(2f), Player.width + 4, Player.height + 4, 390, Player.velocity.X * 0.4f, Player.velocity.Y * 0.4f, 100, default, 1.4f);
            water.noGravity = true;
            water.velocity *= 0.75f;
            water.velocity.X *= 0.75f;
            water.velocity.Y -= 1f;
            if (Main.rand.NextBool(4))
            {
                water.noGravity = false;
                water.scale *= 0.5f;
            }
            if (Main.rand.NextBool(4))
            {
                DirectionalPulseRing pulse = new DirectionalPulseRing(Player.Calamity().RandomDebuffVisualSpot, new Vector2(Main.rand.NextFloat(-1f, 1f), Main.rand.NextFloat(-3f, -4f)), Main.rand.NextBool() ? Color.DeepSkyBlue : Color.MediumBlue, new Vector2(0.8f, 1), 0, 0.09f, 0f, 45);
                GeneralParticleHandler.SpawnParticle(pulse);
            }
            if (Main.rand.NextBool(10))
            {
                Color smokeColor = Color.MediumBlue;
                Particle smoke = new HeavySmokeParticle(Player.Calamity().RandomDebuffVisualSpot, new Vector2(2f, 2f).RotatedByRandom(100) * Main.rand.NextFloat(0.3f, 0.7f), smokeColor, 40, Main.rand.NextFloat(0.3f, 0.4f), 0.5f, Main.rand.NextFloat(-0.2f, 0.2f), false, required: true);
                GeneralParticleHandler.SpawnParticle(smoke);
            }
        }

        internal static void DrawEffects(NPC npc, ref Color drawColor)
        {
            Vector2 npcSize = npc.Center + new Vector2(Main.rand.NextFloat(-npc.width / 2, npc.width / 2), Main.rand.NextFloat(-npc.height / 2, npc.height / 2));
            if (Main.rand.NextBool(13))
            {
                Color smokeColor = Color.MediumBlue;
                Particle smoke = new HeavySmokeParticle(npcSize, new Vector2(2f, 2f).RotatedByRandom(100) * Main.rand.NextFloat(0.3f, 0.7f), smokeColor, 40, Main.rand.NextFloat(0.3f, 0.4f) + (0.00000013f * npc.width * npc.height), 0.5f, Main.rand.NextFloat(-0.2f, 0.2f), false, required: true);
                GeneralParticleHandler.SpawnParticle(smoke);
            }
            Dust water = Dust.NewDustDirect(npc.position - new Vector2(2f), npc.width + 4, npc.height + 4, 390, npc.velocity.X * 0.4f, npc.velocity.Y * 0.4f, 100, default, 1.4f);
            water.noGravity = true;
            water.velocity *= 0.75f;
            water.velocity.X = water.velocity.X * 0.75f;
            water.velocity.Y = water.velocity.Y - 1f;
            if (Main.rand.NextBool(4))
            {
                water.noGravity = false;
                water.scale *= 0.5f;
            }
            if (Main.rand.NextBool(6))
            {
                DirectionalPulseRing pulse = new DirectionalPulseRing(npcSize, new Vector2(Main.rand.NextFloat(-1f, 1f), Main.rand.NextFloat(-4.5f, -6f)), Main.rand.NextBool() ? Color.DeepSkyBlue : Color.MediumBlue, new Vector2(1f), 0, 0.12f + (0.0000007f * npc.width * npc.height), 0f, 35);
                GeneralParticleHandler.SpawnParticle(pulse);
            }
        }
    }
}
