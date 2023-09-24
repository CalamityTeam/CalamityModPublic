using CalamityMod.NPCs;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Buffs.StatDebuffs
{
    public class GlacialState : ModBuff
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
            player.Calamity().gState = true;
        }

        public override void Update(NPC npc, ref int buffIndex)
        {
            if (npc.Calamity().gState < npc.buffTime[buffIndex])
                npc.Calamity().gState = npc.buffTime[buffIndex];
            if ((CalamityLists.enemyImmunityList.Contains(npc.type) || npc.boss) && npc.Calamity().debuffResistanceTimer <= 0)
                npc.Calamity().debuffResistanceTimer = CalamityGlobalNPC.slowingDebuffResistanceMin + npc.Calamity().gState;
            npc.DelBuff(buffIndex);
            buffIndex--;
        }

        internal static void DrawEffects(PlayerDrawSet drawInfo)
        {
            Player Player = drawInfo.drawPlayer;

            if (Main.rand.NextBool(4))
            {
                Dust d = Dust.NewDustDirect(Player.Center - 10 * Vector2.One, 20, 20, 137);
                d.noGravity = Main.rand.NextBool();
                d.noLight = false;
                d.velocity *= 0.2f;
                d.velocity.Y -= Main.rand.NextFloat(0.4f, 0.6f);
                d.scale = Main.rand.NextFloat(0.4f, 0.7f);
            }
        }
    }
}
