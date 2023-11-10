using CalamityMod.Dusts;
using CalamityMod.Particles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Buffs.DamageOverTime
{
    public class ElementalMix : ModBuff
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
            player.Calamity().elementalMix = true;
        }

        public override void Update(NPC npc, ref int buffIndex)
        {
            if (npc.Calamity().elementalMix < npc.buffTime[buffIndex])
                npc.Calamity().elementalMix = npc.buffTime[buffIndex];
            npc.DelBuff(buffIndex);
            buffIndex--;
        }

        internal static void DrawEffects(PlayerDrawSet drawInfo)
        {
            Player player = drawInfo.drawPlayer;
            var modPlayer = player.Calamity();
            var effectcolor = Main.rand.Next(4) switch
            {
                0 => Color.DeepSkyBlue,
                1 => Color.MediumSpringGreen,
                2 => Color.DarkOrange,
                _ => Color.Violet,
            };
            Vector2 speed = new Vector2(Main.rand.NextFloat(-3f, 3f), Main.rand.NextFloat(-2.5f, -8.3f));

            GeneralParticleHandler.SpawnParticle(new TechyHoloysquareParticle(player.Calamity().RandomDebuffVisualSpot, speed, Main.rand.NextFloat(1.2f, 1.8f), effectcolor, Main.rand.Next(8, 14)));

            int dustType = Main.rand.NextBool() ? 66 : 247;
            Dust dust = Dust.NewDustPerfect(player.Calamity().RandomDebuffVisualSpot, dustType);
            dust.scale = (dustType == 66 ? 1.4f : 1.2f);
            dust.velocity = Vector2.Zero + new Vector2(Main.rand.NextFloat(-1, 1), Main.rand.NextFloat(4f, 8f)) - player.velocity / 2;
            dust.noGravity = true;
            dust.alpha = Main.rand.Next(90, 150);
            dust.color = effectcolor;
        }

        internal static void DrawEffects(NPC npc, ref Color drawColor)
        {
            if (Main.rand.NextBool())
            {
                var effectcolor = Main.rand.Next(4) switch
                {
                    0 => Color.DeepSkyBlue,
                    1 => Color.MediumSpringGreen,
                    2 => Color.DarkOrange,
                    _ => Color.Violet,
                };
                Vector2 npcSize = npc.Center + new Vector2(Main.rand.NextFloat(-npc.width / 2, npc.width / 2), Main.rand.NextFloat(-npc.height / 2, npc.height / 2));
                Vector2 speed = new Vector2(Main.rand.NextFloat(-3f, 3f), Main.rand.NextFloat(-2.5f, -8.3f));
                
                GeneralParticleHandler.SpawnParticle(new TechyHoloysquareParticle(npcSize, speed, Main.rand.NextFloat(1.2f, 3.1f), effectcolor, Main.rand.Next(8, 14)));
                
                int dustType = Main.rand.NextBool() ? 66 : 247;
                Dust dust = Dust.NewDustPerfect(npcSize, dustType);
                dust.scale = (dustType == 66 ? 1.4f : 1.2f);
                dust.noGravity = true;
                dust.velocity = Vector2.Zero + new Vector2(Main.rand.NextFloat(-2, 2), Main.rand.NextFloat(2f, 4f)) - npc.velocity / 2;
                dust.alpha = Main.rand.Next(90, 150);
                dust.color = effectcolor;
            }
            if (Main.rand.NextBool(5))
            {
                var effectcolor = Main.rand.Next(4) switch
                {
                    0 => Color.DeepSkyBlue,
                    1 => Color.MediumSpringGreen,
                    2 => Color.DarkOrange,
                    _ => Color.Violet,
                };
                Vector2 npcSize = npc.Center + new Vector2(Main.rand.NextFloat(-npc.width / 2, npc.width / 2), Main.rand.NextFloat(-npc.height / 2, npc.height / 2));
                int dustType = Main.rand.NextBool() ? 66 : 247;
                Dust dust = Dust.NewDustPerfect(npcSize, dustType);
                dust.scale = (dustType == 66 ? 0.9f : 1.4f);
                dust.noGravity = true;
                dust.velocity = Vector2.Zero + new Vector2(Main.rand.NextFloat(-2, 2), Main.rand.NextFloat(2f, 4f));
                dust.alpha = Main.rand.Next(35, 90);
                dust.color = effectcolor;
            }
        }
    }
}
