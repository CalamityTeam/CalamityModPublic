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
    public class VermillionFlux : ModBuff
    {
        public static DebuffData debuffData = new DebuffData(DebuffData.DebuffBehavior.Electric)
        {
            EnemyLostRegen = 75, //150 dps when moving, 37.5 dps when stationary
            ElectricDebuffScaling = 1
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
            player.Calamity().vermillionFlux = true;
        }

        public override void Update(NPC npc, ref int buffIndex)
        {
            npc.Calamity().vermillionFlux = true;
        }

        internal static void DrawEffects(PlayerDrawSet drawInfo)
        {
            Player player = drawInfo.drawPlayer;
            var modPlayer = player.Calamity();

            bool moving = (player.controlLeft || player.controlRight);

            if (!moving && Main.rand.NextBool(3) || moving)
            {
                if (Main.rand.NextBool())
                {
                    int sparkLifetime = Main.rand.Next(15, 23);

                    Vector2 sparkVel = Vector2.UnitY * -10f;
                    float maxRotationDeviance = 0.4f;
                    float rotationAngle = Main.rand.NextFloat(-maxRotationDeviance, maxRotationDeviance);
                    sparkVel = sparkVel.RotatedBy(rotationAngle) * Main.rand.NextFloat(0.3f, 1.0f);

                    float sparkScale = Main.rand.NextFloat(0.007f, 0.015f);

                    // Sparks curve against your horizontal movement
                    Vector2 compensatedSparkVel = new Vector2(sparkVel.X - player.velocity.X * 0.12f, sparkVel.Y);
                    Particle spark = new GlowSparkParticle(modPlayer.RandomDebuffVisualSpot, compensatedSparkVel, true, sparkLifetime, sparkScale, Main.rand.NextBool() ? Color.Red : Color.Crimson, new Vector2(0.5f, 1.3f));
                    GeneralParticleHandler.SpawnParticle(spark);
                }

                if (Main.rand.NextBool())
                    Dust.NewDustPerfect(modPlayer.RandomDebuffVisualSpot, DustID.Fireworks, new Vector2(4f, 4f).RotatedByRandom(100) * Main.rand.NextFloat(0.3f, 0.7f), 0, default, Main.rand.NextFloat(0.2f, 0.6f));
            }
        }

        internal static void DrawEffects(NPC npc, ref Color drawColor)
        {
            Vector2 npcSize = npc.Center + new Vector2(Main.rand.NextFloat(-npc.width / 2, npc.width / 2), Main.rand.NextFloat(-npc.height / 2, npc.height / 2));
            Vector2 Vect2 = new Vector2(0f, Main.rand.NextBool(4) ? -2f : -8f).RotatedByRandom(MathHelper.ToRadians(Main.rand.NextBool(3) ? 10 : 35f)) * Main.rand.NextFloat(0.1f, 1.9f);

            if (Main.rand.NextBool(4))
            {
                int sparkLifetime = Main.rand.Next(15, 23);

                Vector2 sparkVel = Vector2.UnitY * -9f;
                float maxRotationDeviance = 0.4f;
                float rotationAngle = Main.rand.NextFloat(-maxRotationDeviance, maxRotationDeviance);
                sparkVel = sparkVel.RotatedBy(rotationAngle) * Main.rand.NextFloat(0.3f, 1.0f);

                float sparkScale = Main.rand.NextFloat(0.007f, 0.015f);

                Vector2 compensatedSparkVel = new Vector2(sparkVel.X - npc.velocity.X * 0.12f, sparkVel.Y);
                Particle spark = new GlowSparkParticle(npcSize, compensatedSparkVel, true, sparkLifetime, sparkScale, Main.rand.NextBool() ? Color.Red : Color.Crimson, new Vector2(0.5f, 1.3f));
                GeneralParticleHandler.SpawnParticle(spark);
            }

            if (Main.rand.NextBool(3))
                Dust.NewDustPerfect(npcSize, DustID.Fireworks, new Vector2(4f, 4f).RotatedByRandom(100) * Main.rand.NextFloat(0.3f, 0.7f), 0, default, Main.rand.NextFloat(0.2f, 0.6f));

            Lighting.AddLight(npc.Center, Color.Red.ToVector3());
        }
    }
}
