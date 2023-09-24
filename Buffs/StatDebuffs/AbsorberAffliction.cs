using CalamityMod.Dusts;
using CalamityMod.Particles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Buffs.StatDebuffs
{
    public class AbsorberAffliction : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = true;
        }

        public override void Update(NPC npc, ref int buffIndex)
        {
            if (npc.Calamity().absorberAffliction < npc.buffTime[buffIndex])
                npc.Calamity().absorberAffliction = npc.buffTime[buffIndex];
            npc.DelBuff(buffIndex);
            buffIndex--;
        }

        internal static void DrawEffects(NPC npc, ref Color drawColor)
        {
            Vector2 npcSize = npc.Center + new Vector2(Main.rand.NextFloat(-npc.width / 2, npc.width / 2), Main.rand.NextFloat(-npc.height / 2, npc.height / 2));

            DirectionalPulseRing pulse = new DirectionalPulseRing(npcSize, Vector2.Zero, Main.rand.NextBool(3) ? Color.PaleGreen : Color.DarkSeaGreen, new Vector2(Main.rand.NextFloat(0.5f, 1.5f), Main.rand.NextFloat(0.5f, 1.5f)), 0, Main.rand.NextFloat(0.03f, 0.17f), 0f, 35);
            GeneralParticleHandler.SpawnParticle(pulse);

            if (Main.rand.Next(5) >= 0)
            {
                int dust = Dust.NewDust(npc.position - new Vector2(2f), npc.width + 4, npc.height + 4, ModContent.DustType<AbsorberDust>(), npc.velocity.X * 0.4f, npc.velocity.Y * 0.4f, 100, default, 2.5f);
                Main.dust[dust].noGravity = true;
                Main.dust[dust].velocity.Y -= 1.8f;
                Main.dust[dust].velocity.Y *= 2.5f;
                Main.dust[dust].noGravity = true;
            }
        }
    }
}
