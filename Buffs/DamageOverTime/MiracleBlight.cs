using CalamityMod.Particles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Buffs.DamageOverTime
{
    public class MiracleBlight : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = true;
            BuffID.Sets.LongerExpertDebuff[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.Calamity().miracleBlight = true;
        }

        public override void Update(NPC npc, ref int buffIndex)
        {
            if (npc.Calamity().miracleBlight < npc.buffTime[buffIndex])
                npc.Calamity().miracleBlight = npc.buffTime[buffIndex];
            npc.DelBuff(buffIndex);
            buffIndex--;
        }

        internal static void DrawEffects(PlayerDrawSet drawInfo)
        {
            Player Player = drawInfo.drawPlayer;

            var sparkColor = Main.rand.Next(4) switch
            {
                0 => Color.DarkRed,
                1 => Color.MediumTurquoise,
                2 => Color.Orange,
                _ => Color.LawnGreen,
            };
            DirectionalPulseRing pulse = new DirectionalPulseRing(Player.Calamity().RandomDebuffVisualSpot, Vector2.Zero, sparkColor, new Vector2(1, 1), 0, Main.rand.NextFloat(0.07f, 0.1f), 0f, 25);
            GeneralParticleHandler.SpawnParticle(pulse);

            float numberOfDusts = 3f;
            float rotFactor = 360f / numberOfDusts;
            if (Player.miscCounter % 4 == 0)
            {
                for (int i = 0; i < numberOfDusts; i++)
                {
                    var DustType = Main.rand.Next(4) switch
                    {
                        0 => 219,
                        1 => 220,
                        2 => 226,
                        _ => 222,
                    };
                    float rot = MathHelper.ToRadians(i * rotFactor);
                    Vector2 offset = new Vector2(0.3f, 0).RotatedBy(rot * Main.rand.NextFloat(0.2f, 0.3f));
                    Vector2 velOffset = CalamityUtils.RandomVelocity(100f, 70f, 150f, 0.04f);
                    Dust dust = Dust.NewDustPerfect(Player.Calamity().RandomDebuffVisualSpot + offset, DustType, new Vector2(velOffset.X, velOffset.Y));
                    dust.noGravity = true;
                    dust.velocity = velOffset;
                    velOffset *= 10;
                    dust.position = Player.Center - velOffset;
                    dust.scale = Main.rand.NextFloat(0.7f, 0.8f);
                }
            }
        }

        internal static void DrawEffects(NPC npc, ref Color drawColor)
        {
            Vector2 npcSize = npc.Center + new Vector2(Main.rand.NextFloat(-npc.width / 2, npc.width / 2), Main.rand.NextFloat(-npc.height / 2, npc.height / 2));
            var sparkColor = Main.rand.Next(4) switch
            {
                0 => Color.DarkRed,
                1 => Color.MediumTurquoise,
                2 => Color.Orange,
                _ => Color.LawnGreen,
            };
            DirectionalPulseRing pulse = new DirectionalPulseRing(npcSize, Vector2.Zero, sparkColor, new Vector2(1, 1), 0, 0.1f + (0.000004f * npc.width * npc.height), 0f, 25);
            GeneralParticleHandler.SpawnParticle(pulse);

            float numberOfDusts = 2f;
            float rotFactor = 360f / numberOfDusts;
            if (Main.rand.NextBool(3))
            {
                for (int i = 0; i < numberOfDusts; i++)
                {
                    var DustType = Main.rand.Next(4) switch
                    {
                        0 => 219,
                        1 => 220,
                        2 => 226,
                        _ => 222,
                    };
                    float rot = MathHelper.ToRadians(i * rotFactor);
                    Vector2 offset = new Vector2(0.3f, 0).RotatedBy(rot * Main.rand.NextFloat(0.2f, 0.3f));
                    Vector2 velOffset = CalamityUtils.RandomVelocity(100f, 70f, 150f, 0.04f);
                    Dust dust = Dust.NewDustPerfect(npc.Center + offset, DustType, new Vector2(velOffset.X, velOffset.Y));
                    dust.noGravity = true;
                    dust.velocity = velOffset;
                    velOffset *= 10 + (0.0005f * npc.width * npc.height);
                    dust.position = npc.Center - velOffset;
                    dust.scale = Main.rand.NextFloat(0.7f, 0.8f);
                }
            }
        }
    }
}
