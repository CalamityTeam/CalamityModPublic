using CalamityMod.Particles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Buffs.DamageOverTime
{
    public class Nightwither : ModBuff
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
            player.Calamity().nightwither = true;
        }

        public override void Update(NPC npc, ref int buffIndex)
        {
            if (npc.Calamity().nightwither < npc.buffTime[buffIndex])
                npc.Calamity().nightwither = npc.buffTime[buffIndex];
            npc.DelBuff(buffIndex);
            buffIndex--;
        }

        internal static void DrawEffects(Player player)
        {

        }

        internal static void DrawEffects(NPC npc, ref Color drawColor)
        {
            Vector2 npcSize = npc.Center + new Vector2(Main.rand.NextFloat(-npc.width / 2, npc.width / 2), Main.rand.NextFloat(-npc.height / 2, npc.height / 2));
            if (Main.rand.NextBool(3))
            {
                Vector2 Vect = new Vector2(0f, Main.rand.NextBool(4) ? -5f : -9f).RotatedByRandom(MathHelper.ToRadians(25f)) * Main.rand.NextFloat(0.1f, 1.9f);
                CritSpark spark = new CritSpark(npcSize, Vect, Main.rand.NextBool(2) ? Color.Cyan : Color.DarkBlue, Color.DodgerBlue, 0.8f, 15, 2f, 1.9f);
                GeneralParticleHandler.SpawnParticle(spark);
            }
            for (int i = 0; i < 2; i++)
            {
                Vector2 dustCorner = npc.position - 2f * Vector2.One;
                Vector2 dustVel = npc.velocity + new Vector2(0f, Main.rand.NextFloat(-11f, -2f));
                int d = Dust.NewDust(dustCorner, npc.width + 4, npc.height + 4, Main.rand.NextBool(4) ? 160 : 206, dustVel.X, dustVel.Y);
                Main.dust[d].noGravity = true;
                Main.dust[d].scale = Main.rand.NextFloat(0.6f, 0.8f);
                Main.dust[d].alpha = 235;
            }
        }
    }
}
