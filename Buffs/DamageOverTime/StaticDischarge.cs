using CalamityMod.DataStructures;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Systems.Collections;

namespace CalamityMod.Buffs.DamageOverTime
{
    public class StaticDischarge : ModBuff
    {
        public static DebuffData debuffData = new DebuffData(DebuffData.DebuffBehavior.Electric)
        {
            EnemyLostRegen = 5,
            ElectricDebuffScaling = 1
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
            player.Calamity().staticDischarge = true;
        }

        public override void Update(NPC npc, ref int buffIndex)
        {
            npc.Calamity().staticDischarge = true;
        }

        internal static void DrawEffects(PlayerDrawSet drawInfo)
        {
            Player player = drawInfo.drawPlayer;
            var modPlayer = player.Calamity();

            bool moving = (player.controlLeft || player.controlRight);

            if (!moving && Main.rand.NextBool(3) || moving)
            {
                if (Main.rand.NextBool(3))
                {
                    Dust dust = Dust.NewDustPerfect(modPlayer.RandomDebuffVisualSpot, DustID.FireworksRGB, new Vector2(2f, 2f).RotatedByRandom(100) * Main.rand.NextFloat(0.3f, 0.7f), 0, default, Main.rand.NextFloat(0.2f, 0.6f));
                    dust.color = (Main.rand.NextBool(3) ? Color.Yellow : Color.LightSkyBlue);
                }
            }
        }

        internal static void DrawEffects(NPC npc, ref Color drawColor)
        {
            Vector2 npcSize = npc.Center + new Vector2(Main.rand.NextFloat(-npc.width / 2, npc.width / 2), Main.rand.NextFloat(-npc.height / 2, npc.height / 2));
            Vector2 Vect2 = new Vector2(0f, Main.rand.NextBool(4) ? -2f : -8f).RotatedByRandom(MathHelper.ToRadians(Main.rand.NextBool(3) ? 10 : 35f)) * Main.rand.NextFloat(0.1f, 1.9f);

            if (Main.rand.NextBool(4))
            { 
                Dust dust = Dust.NewDustPerfect(npcSize, DustID.FireworksRGB, new Vector2(2f, 2f).RotatedByRandom(100) * Main.rand.NextFloat(0.3f, 0.7f), 0, default, Main.rand.NextFloat(0.2f, 0.6f));
                dust.color = (Main.rand.NextBool(3) ? Color.Yellow : Color.LightSkyBlue);
            }
        }
    }
}
