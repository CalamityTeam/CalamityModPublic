using CalamityMod.Particles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Buffs.DamageOverTime
{
    public class SulphuricPoisoning : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = true;
            BuffID.Sets.LongerExpertDebuff[Type] = true;

            // Acid Venom immunity is granted automatically if you are immune to Sulphuric Poisoning.
            BuffID.Sets.GrantImmunityWith[BuffID.Venom].Add(Type);

            // The same but backwards: Sulphuric Poisoning immunity is granted automatically if you are immune to Acid Venom.
            BuffID.Sets.GrantImmunityWith[Type].Add(BuffID.Venom);

            // Sulphuric Poisoning is a STRONGER poison-type debuff. Anything immune to it is surely immune to regular Poisoned.
            BuffID.Sets.GrantImmunityWith[BuffID.Poisoned].Add(Type);
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.Calamity().sulphurPoison = true;
        }

        public override void Update(NPC npc, ref int buffIndex)
        {
            if (npc.Calamity().sulphurPoison < npc.buffTime[buffIndex])
                npc.Calamity().sulphurPoison = npc.buffTime[buffIndex];
            npc.DelBuff(buffIndex);
            buffIndex--;
        }

        internal static void DrawEffects(PlayerDrawSet drawInfo)
        {
            Player Player = drawInfo.drawPlayer;

            if (Main.rand.Next(5) < 4)
            {
                int dust = Dust.NewDust(drawInfo.Position - new Vector2(2f), Player.width + 4, Player.height + 4, 298, Player.velocity.X * 0.4f, Player.velocity.Y * 0.4f, 100, default, 0.6f);
                Main.dust[dust].noGravity = true;
                Main.dust[dust].velocity *= 0.75f;
                Main.dust[dust].velocity.X = Main.dust[dust].velocity.X * 0.75f;
                Main.dust[dust].velocity.Y = Main.dust[dust].velocity.Y - 1f;
                if (Main.rand.NextBool(4))
                {
                    Main.dust[dust].noGravity = false;
                    Main.dust[dust].scale *= 0.2f;
                }
                if (Main.rand.NextBool(5))
                {
                    DirectionalPulseRing pulse = new DirectionalPulseRing(Player.Calamity().RandomDebuffVisualSpot, new Vector2(Main.rand.NextFloat(-1f, 1f), Main.rand.NextFloat(-3f, -4f)), Main.rand.NextBool() ? Color.OliveDrab : Color.GreenYellow, new Vector2(0.8f, 1), 0, 0.09f, 0f, 45);
                    GeneralParticleHandler.SpawnParticle(pulse);
                }
            }
        }

        internal static void DrawEffects(NPC npc, ref Color drawColor)
        {
            if (Main.rand.NextBool())
            {
                Vector2 npcSize = npc.Center + new Vector2(Main.rand.NextFloat(-npc.width / 2, npc.width / 2), Main.rand.NextFloat(-npc.height / 2, npc.height / 2));
                int dust = Dust.NewDust(npc.position - new Vector2(2f), npc.width + 4, npc.height + 4, 298, npc.velocity.X * 0.4f, npc.velocity.Y * 0.4f, 100, default, 1.2f + +(0.000003f * npc.width * npc.height));
                Main.dust[dust].noGravity = true;
                Main.dust[dust].velocity *= 0.75f;
                Main.dust[dust].velocity.X = Main.dust[dust].velocity.X * 0.75f;
                Main.dust[dust].velocity.Y = Main.dust[dust].velocity.Y - 1f;
                if (Main.rand.NextBool(4))
                {
                    Main.dust[dust].noGravity = false;
                    Main.dust[dust].scale *= 0.4f;
                }
                if (Main.rand.NextBool())
                {
                    DirectionalPulseRing pulse = new DirectionalPulseRing(npcSize, new Vector2(Main.rand.NextFloat(-1f, 1f), Main.rand.NextFloat(-4.5f, -6f)), Main.rand.NextBool() ? Color.OliveDrab : Color.GreenYellow, new Vector2(0.8f, 1), 0, 0.12f + (0.0000007f * npc.width * npc.height), 0f, 45);
                    GeneralParticleHandler.SpawnParticle(pulse);
                }
            }
        }
    }
}
