using CalamityMod.DataStructures;
using CalamityMod.Particles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Systems.Collections;

namespace CalamityMod.Buffs.DamageOverTime
{
    public class HeavyBleeding : ModBuff
    {
        public static DebuffData debuffData = new DebuffData()
        {
            EnemyLostRegen = 80
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
            player.Calamity().heavybleeding = true;
        }

        public override void Update(NPC npc, ref int buffIndex)
        {
            npc.Calamity().heavyBleeding = true;
        }
        internal static void DrawEffects(PlayerDrawSet drawInfo)
        {
            Player player = drawInfo.drawPlayer;
            var modPlayer = player.Calamity();

            if (Main.rand.NextBool(3))
            {
                Vector2 randVel = new Vector2(5, 5).RotatedByRandom(100) * Main.rand.NextFloat(0.3f, 1f);
                Dust dust = Dust.NewDustPerfect(modPlayer.RandomDebuffVisualSpot, (!ChildSafety.Disabled ? DustID.Cloud : DustID.Blood), randVel * Main.rand.NextFloat(0.1f, 0.8f), 100, default, Main.rand.NextFloat(0.6f, 0.9f));
                dust.noGravity = false;
                Particle spark = new AltSparkParticle(modPlayer.RandomDebuffVisualSpot, randVel + new Vector2(0, -4), true, 12, Main.rand.NextFloat(0.25f, 0.6f), (!ChildSafety.Disabled ? Color.CornflowerBlue : Color.DarkRed) * 0.5f);
                GeneralParticleHandler.SpawnParticle(spark);
            }
            if (Main.rand.NextBool(8))
            {
                Particle spark = new GlowOrbParticle(modPlayer.RandomDebuffVisualSpot, new Vector2(0, 4) * Main.rand.NextFloat(0.5f, 0.7f), true, 16, Main.rand.NextFloat(0.55f, 0.8f), (!ChildSafety.Disabled ? Color.CornflowerBlue : Color.DarkRed) * 0.8f, false, false, false);
                GeneralParticleHandler.SpawnParticle(spark);
            }
        }

        internal static void DrawEffects(NPC npc, ref Color drawColor)
        {
            Vector2 npcSize = npc.Center + new Vector2(Main.rand.NextFloat(-npc.width / 2, npc.width / 2), Main.rand.NextFloat(-npc.height / 2, npc.height / 2));
            Vector2 randVel = new Vector2(5, 5).RotatedByRandom(100) * Main.rand.NextFloat(0.3f, 1f);
            if (Main.rand.NextBool(5))
            {
                Particle spark = new AltSparkParticle(npcSize, randVel + new Vector2(0, -4), true, 12, Main.rand.NextFloat(0.25f, 0.6f), (!ChildSafety.Disabled ? Color.CornflowerBlue : Color.DarkRed) * 0.5f);
                GeneralParticleHandler.SpawnParticle(spark);
            }
            else
            {
                Dust dust = Dust.NewDustPerfect(npcSize, (!ChildSafety.Disabled ? DustID.Cloud : DustID.Blood), randVel * Main.rand.NextFloat(0.1f, 0.8f), 100, default, Main.rand.NextFloat(0.2f, 0.6f));
                dust.noGravity = false;
            }
            if (Main.rand.NextBool(8))
            {
                Particle spark = new GlowOrbParticle(npcSize, new Vector2(0, 4) * Main.rand.NextFloat(0.5f, 0.7f), true, 16, Main.rand.NextFloat(0.55f, 0.8f), (!ChildSafety.Disabled ? Color.CornflowerBlue : Color.DarkRed) * 0.8f, false, false, false);
                GeneralParticleHandler.SpawnParticle(spark);
            }
        }
    }
}
