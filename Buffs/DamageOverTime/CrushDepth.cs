using CalamityMod.Particles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Buffs.DamageOverTime
{
    public class CrushDepth : ModBuff
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
            player.Calamity().cDepth = true;
        }

        public override void Update(NPC npc, ref int buffIndex)
        {
            if (npc.Calamity().cDepth < npc.buffTime[buffIndex])
                npc.Calamity().cDepth = npc.buffTime[buffIndex];
            npc.DelBuff(buffIndex);
            buffIndex--;
        }

        internal static void DrawEffects(PlayerDrawSet drawInfo)
        {
            Player Player = drawInfo.drawPlayer;

            // A bit of blood
            if (Main.rand.NextBool(30))
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

            // Blue n black dust
            int dust = Dust.NewDust(drawInfo.Position - new Vector2(2f), Player.width + 4, Player.height + 4, Main.rand.NextBool(3) ? 104 : 186, Player.velocity.X * 0.4f, Player.velocity.Y * 0.4f, 100, default, 1.4f);
            Main.dust[dust].noGravity = true;
            Main.dust[dust].velocity *= 0.75f;
            Main.dust[dust].velocity.X = Main.dust[dust].velocity.X * 0.75f;
            Main.dust[dust].velocity.Y = Main.dust[dust].velocity.Y - 1f;
            if (Main.rand.NextBool(4))
            {
                Main.dust[dust].noGravity = false;
                Main.dust[dust].scale *= 0.2f;
            }
        }

        internal static void DrawEffects(NPC npc, ref Color drawColor)
        {
            if (Main.rand.NextBool(30))
            {
                int bloodLifetime = Main.rand.Next(22, 36);
                float bloodScale = Main.rand.NextFloat(0.6f, 0.8f);
                Color bloodColor = Color.Lerp(Color.Red, Color.DarkRed, Main.rand.NextFloat());
                bloodColor = Color.Lerp(bloodColor, new Color(51, 22, 94), Main.rand.NextFloat(0.65f));

                if (Main.rand.NextBool(15))
                    bloodScale *= 1.3f;

                Vector2 npcSize = npc.Center + new Vector2(Main.rand.NextFloat(-npc.width / 2, npc.width / 2), Main.rand.NextFloat(-npc.height / 2, npc.height / 2));
                float randomSpeedMultiplier = Main.rand.NextFloat(1.25f, 1.5f);
                Vector2 bloodVelocity = Main.rand.NextVector2Unit() * 2 * randomSpeedMultiplier;
                bloodVelocity.Y -= 5f;
                BloodParticle blood = new BloodParticle(npcSize, bloodVelocity, bloodLifetime, bloodScale, bloodColor);
                GeneralParticleHandler.SpawnParticle(blood);
            }
            int dust = Dust.NewDust(npc.position - new Vector2(2f), npc.width + 4, npc.height + 4, Main.rand.NextBool(3) ? 104 : 186, npc.velocity.X * 0.4f, npc.velocity.Y * 0.4f, 100, default, 1.4f);
            Main.dust[dust].noGravity = true;
            Main.dust[dust].velocity *= 0.75f;
            Main.dust[dust].velocity.X = Main.dust[dust].velocity.X * 0.75f;
            Main.dust[dust].velocity.Y = Main.dust[dust].velocity.Y - 1f;
            if (Main.rand.NextBool(4))
            {
                Main.dust[dust].noGravity = false;
                Main.dust[dust].scale *= 0.2f;
            }
        }
    }
}
