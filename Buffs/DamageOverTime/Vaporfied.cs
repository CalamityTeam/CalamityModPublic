using CalamityMod.DataStructures;
using CalamityMod.NPCs;
using CalamityMod.Systems.Collections;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Systems.Collections;

namespace CalamityMod.Buffs.DamageOverTime
{
    public class Vaporfied : ModBuff
    {
        public static DebuffData debuffData = new DebuffData()
        {
            EnemyLostRegen = 30,
            MinimumDamageTickSize = 6,
            MultiplierDamageTickSize = 0
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
            player.Calamity().vaporfied = true;
        }

        public override void Update(NPC npc, ref int buffIndex)
        {
            npc.Calamity().vaporfied = true;
        }

        internal static void DrawEffects(PlayerDrawSet drawInfo)
        {
            Player Player = drawInfo.drawPlayer;

            int dustType = Utils.SelectRandom(Main.rand, new int[]
            {
                246,
                242,
                229,
                226,
                247,
                187,
                234
            });
            if (Main.rand.NextBool(4))
            {
                Dust dust = Dust.NewDustDirect(drawInfo.Position - new Vector2(2f), Player.width + 4, Player.height + 4, dustType, Player.velocity.X * 0.4f, Player.velocity.Y * 0.4f, 100, default, 3f);
                dust.noGravity = true;
                dust.velocity *= 1.8f;
                dust.velocity.Y -= 0.5f;
                if (Main.rand.NextBool(4))
                {
                    dust.noGravity = false;
                    dust.scale *= 0.5f;
                }
                drawInfo.DustCache.Add(dust.dustIndex);
            }
        }

        internal static void DrawEffects(NPC npc, ref Color drawColor)
        {
            int dustType = Utils.SelectRandom(Main.rand, new int[]
            {
                246,
                242,
                229,
                226,
                247,
                187,
                234
            });

            if (Main.rand.Next(5) < 4)
            {
                Dust dust = Dust.NewDustDirect(npc.position - new Vector2(2f, 2f), npc.width + 4, npc.height + 4, dustType, npc.velocity.X * 0.4f, npc.velocity.Y * 0.4f, 100, default, 3f);
                dust.noGravity = true;
                dust.velocity *= 1.8f;
                dust.velocity.Y -= 0.5f;
                if (Main.rand.NextBool(4))
                {
                    dust.noGravity = false;
                    dust.scale *= 0.5f;
                }
            }
        }
    }
}
