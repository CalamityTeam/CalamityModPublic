using CalamityMod.Particles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Buffs.DamageOverTime
{
    public class Plague : ModBuff
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
            player.Calamity().pFlames = true;
        }

        public override void Update(NPC npc, ref int buffIndex)
        {
            if (npc.Calamity().pFlames < npc.buffTime[buffIndex])
                npc.Calamity().pFlames = npc.buffTime[buffIndex];
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
                DirectionalPulseRing pulse = new DirectionalPulseRing(npcSize, Vector2.Zero, Main.rand.NextBool(3) ? Color.LimeGreen : Color.Green, new Vector2(1, 1), 0, Main.rand.NextFloat(0.07f, 0.18f), 0f, 35);
                GeneralParticleHandler.SpawnParticle(pulse);

                for (int i = 0; i < 4; i++)
                {
                    int DustID = Main.rand.NextBool(30) ? 220 : 89;
                    Dust dust2 = Dust.NewDustDirect(npc.position, npc.width, npc.height, DustID);
                    dust2.scale = Main.rand.NextFloat(0.3f, 0.4f);
                    if (DustID == 220)
                        dust2.scale = Main.rand.NextFloat(1f, 1.2f);
                }
            }
            Lighting.AddLight(npc.position, 0.07f, 0.15f, 0.01f);
        }
    }
}
