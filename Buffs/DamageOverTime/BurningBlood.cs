using CalamityMod.Particles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Buffs.DamageOverTime
{
    public class BurningBlood : ModBuff
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
            player.Calamity().bBlood = true;
        }

        public override void Update(NPC npc, ref int buffIndex)
        {
            if (npc.Calamity().bBlood < npc.buffTime[buffIndex])
                npc.Calamity().bBlood = npc.buffTime[buffIndex];
            npc.DelBuff(buffIndex);
            buffIndex--;
        }

        internal static void DrawEffects(PlayerDrawSet drawInfo)
        {
            Player Player = drawInfo.drawPlayer;

            if (Main.rand.NextBool(11))
            {
                int bloodLifetime = Main.rand.Next(22, 36);
                float bloodScale = Main.rand.NextFloat(0.6f, 0.8f);
                Color bloodColor = Color.Lerp(Color.Red, Color.DarkRed, Main.rand.NextFloat());
                bloodColor = Color.Lerp(bloodColor, new Color(51, 22, 94), Main.rand.NextFloat(0.65f));

                if (Main.rand.NextBool(15))
                    bloodScale *= 1.3f;

                float randomSpeedMultiplier = Main.rand.NextFloat(1.25f, 1.5f);
                Vector2 bloodVelocity = Main.rand.NextVector2Unit() * 2 * randomSpeedMultiplier;
                bloodVelocity.Y -= 5f;
                BloodParticle blood = new BloodParticle(Player.Center, bloodVelocity, bloodLifetime, bloodScale, bloodColor);
                GeneralParticleHandler.SpawnParticle(blood);
            }
            for (int i = 0; i < 2; i++)
            {
                float rot = MathHelper.ToRadians(i * 280);
                Vector2 offset = new Vector2(0.1f, 0).RotatedBy(rot * Main.rand.NextFloat(0.08f, 0.05f));
                Dust dust2 = Dust.NewDustPerfect(Player.Calamity().RandomDebuffVisualSpot + offset, 5);
                dust2.scale = Main.rand.NextFloat(0.6f, 0.7f);
            }
        }

        internal static void DrawEffects(NPC npc, ref Color drawColor)
        {
            if (Main.rand.NextBool(3))
            {
                int dust = Dust.NewDust(npc.position - new Vector2(2f), npc.width + 4, npc.height + 4, Main.rand.NextBool(8) ? 296 : 5, npc.velocity.X * 0.4f, npc.velocity.Y * 0.4f, 100, default, 1.25f);
                Main.dust[dust].noGravity = true;
                Main.dust[dust].velocity *= 1.3f;
                Main.dust[dust].velocity.Y -= 0.5f;
            }
            Lighting.AddLight(npc.Center, 0.08f, 0f, 0f);
        }
    }
}
