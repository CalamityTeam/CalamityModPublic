using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Buffs.DamageOverTime
{
    public class RiptideDebuff : ModBuff
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
            player.Calamity().rTide = true;
        }

        public override void Update(NPC npc, ref int buffIndex)
        {
            if (npc.Calamity().rTide < npc.buffTime[buffIndex])
                npc.Calamity().rTide = npc.buffTime[buffIndex];
            npc.DelBuff(buffIndex);
            buffIndex--;
        }

        internal static void DrawEffects(PlayerDrawSet drawInfo)
        {
            Player Player = drawInfo.drawPlayer;

            if (Main.rand.NextBool(7))
            {
                int dust = Dust.NewDust(drawInfo.Position - new Vector2(2f), Player.width + 4, Player.height + 4, 165, Player.velocity.X * 0.4f, Player.velocity.Y * 0.4f, 100, default, 1f);
                Main.dust[dust].noGravity = false;
                Main.dust[dust].velocity *= 1.2f;
                Main.dust[dust].velocity.Y += 0.8f;
                drawInfo.DustCache.Add(dust);
            }
        }

        internal static void DrawEffects(NPC npc, ref Color drawColor)
        {
            if (Main.rand.Next(7) < 3)
            {
                int dust = Dust.NewDust(npc.position - new Vector2(2f, 2f), npc.width + 4, npc.height + 4, 165, npc.velocity.X * 0.4f, npc.velocity.Y * 0.4f, 100, default, 1f);
                Main.dust[dust].noGravity = false;
                Main.dust[dust].velocity *= 1.2f;
                Main.dust[dust].velocity.Y += 0.5f;
            }
        }
    }
}
