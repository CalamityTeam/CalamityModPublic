using CalamityMod.Particles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Buffs.DamageOverTime
{
    public class GodSlayerInferno : ModBuff
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
            player.Calamity().gsInferno = true;
        }

        public override void Update(NPC npc, ref int buffIndex)
        {
            if (npc.Calamity().gsInferno < npc.buffTime[buffIndex])
                npc.Calamity().gsInferno = npc.buffTime[buffIndex];
            npc.DelBuff(buffIndex);
            buffIndex--;
        }

        internal static void DrawEffects(PlayerDrawSet drawInfo)
        {
            Player Player = drawInfo.drawPlayer;

            SparkParticle spark = new SparkParticle(Player.Calamity().RandomDebuffVisualSpot, new Vector2(0, Main.rand.NextFloat(-5f, 5f)), false, Main.rand.Next(11, 13), Main.rand.NextFloat(0.2f, 0.5f), Main.rand.NextBool(7) ? Color.Aqua : Color.Fuchsia);
            GeneralParticleHandler.SpawnParticle(spark);

        }

        internal static void DrawEffects(NPC npc, ref Color drawColor)
        {
            if (Main.rand.NextBool())
            {
                Vector2 npcSize = npc.Center + new Vector2(Main.rand.NextFloat(-npc.width / 2, npc.width / 2), Main.rand.NextFloat(-npc.height / 2, npc.height / 2));
                SparkParticle spark = new SparkParticle(npcSize, new Vector2(0, Main.rand.NextFloat(-5f, 5f)), false, Main.rand.Next(11, 13), Main.rand.NextFloat(0.2f, 0.5f), Main.rand.NextBool(7) ? Color.Aqua : Color.Fuchsia);
                GeneralParticleHandler.SpawnParticle(spark);
            }
            Lighting.AddLight(npc.position, 0.1f, 0f, 0.135f);
        }
    }
}
