using CalamityMod.NPCs;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Buffs.StatDebuffs
{
    public class Eutrophication : ModBuff
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
            player.Calamity().eutrophication = true;
        }

        public override void Update(NPC npc, ref int buffIndex)
        {
            if (npc.Calamity().eutrophication < npc.buffTime[buffIndex])
                npc.Calamity().eutrophication = npc.buffTime[buffIndex];
            if ((CalamityLists.enemyImmunityList.Contains(npc.type) || npc.boss) && npc.Calamity().debuffResistanceTimer <= 0)
                npc.Calamity().debuffResistanceTimer = CalamityGlobalNPC.slowingDebuffResistanceMin + npc.Calamity().eutrophication;
            npc.DelBuff(buffIndex);
            buffIndex--;
        }

        internal static void DrawEffects(PlayerDrawSet drawInfo)
        {
            Player Player = drawInfo.drawPlayer;

            if (Main.rand.NextBool(5))
            {
                Dust dust = Dust.NewDustDirect(Player.position - new Vector2(2f, 2f), Player.width + 4, Player.height + 4, Main.rand.NextBool(4) ? 56 : 33, Player.velocity.X * 0.4f, Player.velocity.Y * 0.4f, 100, default, Main.rand.NextFloat(0.2f, 1.5f));
                if (dust.type == 56)
                {
                    dust.velocity.Y += 2;
                    dust.velocity.X *= 0.7f;
                    dust.noGravity = false;
                }
                else
                {
                    dust.velocity.Y += 1;
                    dust.noGravity = false;
                }
            }
        }

        internal static void DrawEffects(NPC npc, ref Color drawColor)
        {
            if (Main.rand.NextBool(5))
            {
                Dust dust = Dust.NewDustDirect(npc.position - new Vector2(2f, 2f), npc.width + 4, npc.height + 4, Main.rand.NextBool(4) ? 56 : 33, npc.velocity.X * 0.4f, npc.velocity.Y * 0.4f, 100, default, Main.rand.NextFloat(0.2f, 1.5f));
                if (dust.type == 56)
                {
                    dust.velocity.Y += 2;
                    dust.velocity.X *= 0.7f;
                    dust.noGravity = false;
                }
                else
                {
                    dust.velocity.Y += 1;
                    dust.noGravity = false;
                }
            }
        }
    }
}
