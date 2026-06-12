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
    public class AuricRebuke : ModBuff
    {
        public static DebuffData debuffData = new DebuffData(DebuffData.DebuffBehavior.Electric)
        {
            EnemyLostRegen = 200,
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
            player.Calamity().auricRebuke = true;
        }

        public override void Update(NPC npc, ref int buffIndex)
        {
            npc.Calamity().auricRebuke = true;
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
                    int sparkLifetime = Main.rand.Next(11, 14);
                    Vector2 sparkVel = new Vector2(8, 8).RotatedByRandom(100);
                    Particle spark = new GlowSparkParticle(modPlayer.RandomDebuffVisualSpot, sparkVel, false, sparkLifetime, Main.rand.NextFloat(0.008f, 0.012f), Color.Lerp(Color.Cyan, Color.Lavender, Main.rand.NextFloat(0, 0.6f)), new Vector2(1f, 0.7f), true);
                    GeneralParticleHandler.SpawnParticle(spark);
                }

                Dust dust = Dust.NewDustPerfect(modPlayer.RandomDebuffVisualSpot, DustID.FireworksRGB, new Vector2(1.5f, 1.5f).RotatedByRandom(100) * Main.rand.NextFloat(0.3f, 0.7f), 0, default, Main.rand.NextFloat(0.1f, 0.6f));
                dust.color = Color.Lerp(Color.Cyan, Color.Lavender, Main.rand.NextFloat(0, 0.6f));

                if (Main.rand.NextBool(6))
                {
                    Dust dust2 = Dust.NewDustPerfect(modPlayer.RandomDebuffVisualSpot, DustID.FireworksRGB, new Vector2(4.5f, 4.5f).RotatedByRandom(100) * Main.rand.NextFloat(0.3f, 0.7f), 0, default, Main.rand.NextFloat(0.8f, 0.95f));
                    dust2.color = Main.rand.NextBool(4) ? Color.Lavender : Color.Cyan;
                }
            }
        }

        internal static void DrawEffects(NPC npc, ref Color drawColor)
        {
            Vector2 npcSize = npc.Center + new Vector2(Main.rand.NextFloat(-npc.width / 2, npc.width / 2), Main.rand.NextFloat(-npc.height / 2, npc.height / 2));
            Vector2 Vect2 = new Vector2(0f, Main.rand.NextBool(4) ? -2f : -8f).RotatedByRandom(MathHelper.ToRadians(Main.rand.NextBool(3) ? 10 : 35f)) * Main.rand.NextFloat(0.1f, 1.9f);

            if (Main.rand.NextBool(4))
            {
                int sparkLifetime = Main.rand.Next(11, 14);
                Vector2 sparkVel = new Vector2(8, 8).RotatedByRandom(100);
                Particle spark = new GlowSparkParticle(npcSize, sparkVel, false, sparkLifetime, Main.rand.NextFloat(0.008f, 0.012f), Color.Lerp(Color.Cyan, Color.Lavender, Main.rand.NextFloat(0, 0.6f)), new Vector2(1f, 0.7f), true);
                GeneralParticleHandler.SpawnParticle(spark);
            }

            if (Main.rand.NextBool())
            {
                Dust dust = Dust.NewDustPerfect(npcSize, DustID.FireworksRGB, new Vector2(1.5f, 1.5f).RotatedByRandom(100) * Main.rand.NextFloat(0.3f, 0.7f), 0, default, Main.rand.NextFloat(0.1f, 0.6f));
                dust.color = Color.Lerp(Color.Cyan, Color.Lavender, Main.rand.NextFloat(0, 0.6f));
            }
            if (Main.rand.NextBool(10))
            {
                Dust dust2 = Dust.NewDustPerfect(npcSize, DustID.FireworksRGB, new Vector2(4.5f, 4.5f).RotatedByRandom(100) * Main.rand.NextFloat(0.3f, 0.7f), 0, default, Main.rand.NextFloat(0.8f, 0.95f));
                dust2.color = Main.rand.NextBool(4) ? Color.Lavender : Color.Cyan;
            }
        }
    }
}
