using CalamityMod.Dusts;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Buffs.DamageOverTime
{
    public class BrimstoneFlames : ModBuff
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
            player.Calamity().bFlames = true;
        }

        public override void Update(NPC npc, ref int buffIndex)
        {
            if (npc.Calamity().bFlames < npc.buffTime[buffIndex])
                npc.Calamity().bFlames = npc.buffTime[buffIndex];
            npc.DelBuff(buffIndex);
            buffIndex--;
        }

        internal static void DrawEffects(Player player)
        {

        }

        internal static void DrawEffects(NPC npc, ref Color drawColor)
        {
            if (Main.rand.NextBool(3))
            {
                Dust dust = Dust.NewDustDirect(npc.position, npc.width, npc.height, Main.rand.NextBool(3) ? 114 : ModContent.DustType<BrimstoneFlame>());
                dust.noGravity = true;
                dust.velocity = new Vector2(0, Main.rand.NextFloat(-3f, -5f)) + npc.velocity;
                dust.scale = 1.6f;
                for (int i = 0; i < 3; i++)
                {
                    Dust dust2 = Dust.NewDustDirect(npc.position + new Vector2(Main.rand.NextFloat(-10f, 10f), npc.height / 2), npc.width, npc.height, Main.rand.NextBool(2) ? 90 : ModContent.DustType<BrimstoneFlame>());
                    dust2.noGravity = true;
                    dust2.velocity = new Vector2(Main.rand.NextFloat(-4f, 4f), Main.rand.NextFloat(-1f, -3f)) + npc.velocity;
                    dust2.scale = 1.4f;
                }
                Lighting.AddLight(npc.position, 0.05f, 0.01f, 0.01f);
            }
        }
    }
}
