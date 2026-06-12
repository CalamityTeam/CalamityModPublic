using CalamityMod.DataStructures;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Systems.Collections;

namespace CalamityMod.Buffs.DamageOverTime
{
    public class RiptideDebuff : ModBuff
    {
        public static DebuffData debuffData = new DebuffData()
        {
            EnemyLostRegen = 30,
            WaterDebuffScaling = 1
        };
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = true;
            BuffID.Sets.LongerExpertDebuff[Type] = true;
            CalamityBuffSets.DebuffDataset[Type] = debuffData;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.Calamity().riptide = true;
        }

        public override void Update(NPC npc, ref int buffIndex)
        {
            npc.Calamity().riptide = true;
        }

        internal static void DrawEffects(PlayerDrawSet drawInfo)
        {
            Player Player = drawInfo.drawPlayer;
            var modPlayer = Player.Calamity();

            if (Main.rand.NextBool(14))
            {
                Dust dust = Dust.NewDustDirect(drawInfo.Position - new Vector2(2f), Player.width + 4, Player.height + 4, DustID.FungiHit, Player.velocity.X * 0.4f, Player.velocity.Y * 0.4f, 100, default, 1f);
                dust.noGravity = false;
                dust.velocity *= 1.2f;
                dust.velocity.Y += 0.8f;
                drawInfo.DustCache.Add(dust.dustIndex);
            }
            if (Main.rand.NextBool(9))
            {
                Gore bubble = Gore.NewGorePerfect(Player.GetSource_FromAI(), modPlayer.RandomDebuffVisualSpot, new Vector2(2f, 2f).RotatedByRandom(100) * Main.rand.NextFloat(0.3f, 0.7f), 411);
                bubble.timeLeft = 4 + Main.rand.Next(7);
                bubble.scale = Main.rand.NextFloat(0.6f, 1f);
                bubble.type = Main.rand.NextBool(3) ? 412 : 411;
            }
        }

        internal static void DrawEffects(NPC npc, ref Color drawColor)
        {
            Vector2 npcSize = npc.Center + new Vector2(Main.rand.NextFloat(-npc.width / 2, npc.width / 2), Main.rand.NextFloat(-npc.height / 2, npc.height / 2));
            if (Main.rand.NextBool(9))
            {
                Dust dust = Dust.NewDustPerfect(npcSize, DustID.Snow, new Vector2(2f, 2f).RotatedByRandom(100) * Main.rand.NextFloat(0.3f, 0.7f), 0, default, Main.rand.NextFloat(0.2f, 0.6f));
                dust.color = (Main.rand.NextBool(3) ? Color.LightBlue : Color.LightSkyBlue);
            }
            if (Main.rand.NextBool(8))
            {
                Gore bubble = Gore.NewGorePerfect(npc.GetSource_FromAI(), npcSize, new Vector2(2f, 2f).RotatedByRandom(100) * Main.rand.NextFloat(0.3f, 0.7f), 411);
                bubble.timeLeft = 4 + Main.rand.Next(7);
                bubble.scale = Main.rand.NextFloat(0.6f, 1f);
                bubble.type = Main.rand.NextBool(3) ? 412 : 411;
            }
        }
    }
}
