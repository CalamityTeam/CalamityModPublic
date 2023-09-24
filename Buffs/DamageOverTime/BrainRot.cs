using CalamityMod.Particles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Buffs.DamageOverTime
{
    public class BrainRot : ModBuff
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
            player.Calamity().brainRot = true;
        }

        public override void Update(NPC npc, ref int buffIndex)
        {
            if (npc.Calamity().brainRot < npc.buffTime[buffIndex])
                npc.Calamity().brainRot = npc.buffTime[buffIndex];
            npc.DelBuff(buffIndex);
            buffIndex--;
        }

        internal static void DrawEffects(PlayerDrawSet drawInfo)
        {
            Player player = drawInfo.drawPlayer;
            var modPlayer = player.Calamity();
            if (Main.rand.NextBool())
            {
                int dustType = Main.rand.NextBool() ? 184 : 18;
                Dust dust = Dust.NewDustPerfect(player.Center + new Vector2(Main.rand.NextFloat(-5f, 5f), -16 + Main.rand.NextFloat(-5f, 5f)), dustType);
                dust.scale = (dustType == 18 ? 0.6f : 1.2f);
                dust.velocity = new Vector2(2, 2).RotatedByRandom(360) * Main.rand.NextFloat(0.3f, 0.7f) + player.velocity;
                dust.noGravity = true;
                dust.alpha = 35;
            }
            if (Main.rand.NextBool(4))
            {
                Dust dust = Dust.NewDustPerfect(modPlayer.RandomDebuffVisualSpot, 18);
                dust.noGravity = true;
                dust.velocity = Vector2.Zero;
                dust.alpha = 90;
            }
        }

        internal static void DrawEffects(NPC npc, ref Color drawColor)
        {
            if (Main.rand.NextBool())
            {
                Vector2 npcSize = npc.Center + new Vector2(Main.rand.NextFloat(-npc.width / 2, npc.width / 2), Main.rand.NextFloat(-npc.height / 2, npc.height / 2));
                int dustType = Main.rand.NextBool() ? 184 : 18;
                Dust dust = Dust.NewDustPerfect(npcSize, dustType);
                dust.scale = (dustType == 18 ? 0.6f : 1.2f);
                dust.noGravity = true;
                dust.velocity = Vector2.Zero;
                dust.alpha = Main.rand.Next(35, 90);
            }
        }
    }
}
