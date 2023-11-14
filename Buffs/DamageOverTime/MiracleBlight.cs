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
                0 => Color.Red,
                1 => Color.MediumTurquoise,
                2 => Color.Orange,
                _ => Color.LawnGreen,
            };

            if (Main.rand.NextBool(2))
            {
                Dust dust = Dust.NewDustPerfect(Player.Calamity().RandomDebuffVisualSpot, 278, CalamityUtils.RandomVelocity(100f, 70f, 150f, 0.04f));
                dust.noGravity = true;
                dust.scale = Main.rand.NextFloat(0.7f, 0.85f);
                dust.color = sparkColor * 0.45f;
            }

            float numberOfDusts = 1f;
            float rotFactor = 360f / numberOfDusts;
            for (int i = 0; i < numberOfDusts; i++)
            {
                float rot = MathHelper.ToRadians(i * rotFactor);
                Vector2 velOffset = CalamityUtils.RandomVelocity(100f, 70f, 150f, 0.04f);
                velOffset *= Main.rand.NextFloat(2, 13);
                SquishyLightParticle exoEnergy = new(Player.Center + Player.velocity * 3 + velOffset * 1.5f, -velOffset * 0.25f, 0.3f, sparkColor, 8);
                GeneralParticleHandler.SpawnParticle(exoEnergy);
            }
        }

        internal static void DrawEffects(NPC npc, ref Color drawColor)
        {
            Vector2 npcSize = npc.Center + new Vector2(Main.rand.NextFloat(-npc.width / 2, npc.width / 2), Main.rand.NextFloat(-npc.height / 2, npc.height / 2));
            var sparkColor = Main.rand.Next(4) switch
            {
                0 => Color.Red,
                1 => Color.MediumTurquoise,
                2 => Color.Orange,
                _ => Color.LawnGreen,
            };

            if (Main.rand.NextBool(3))
            {
                Dust dust = Dust.NewDustPerfect(npcSize, 278, CalamityUtils.RandomVelocity(100f, 70f, 150f, 0.04f));
                dust.noGravity = true;
                dust.scale = Main.rand.NextFloat(0.7f, 0.85f) + (0.0000007f * npc.width * npc.height);
                dust.color = sparkColor * 0.45f;
            }

            float numberOfDusts = 1f;
            float rotFactor = 360f / numberOfDusts;
            if (Main.rand.NextBool())
            {
                for (int i = 0; i < numberOfDusts; i++)
                {
                    float rot = MathHelper.ToRadians(i * rotFactor);
                    Vector2 velOffset = CalamityUtils.RandomVelocity(100f, 70f, 150f, 0.04f);
                    velOffset *= Main.rand.NextFloat(1, 9) + (0.0002f * npc.width * npc.height);
                    SquishyLightParticle exoEnergy = new(npc.Center + npc.velocity * 3 + velOffset * 1.5f, -velOffset * 0.25f, 0.3f, sparkColor, 8);
                    GeneralParticleHandler.SpawnParticle(exoEnergy);
                }
            }
        }
    }
}
