using Microsoft.Xna.Framework;
using Terraria;
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

        internal static void DrawEffects(Player player)
        {

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
