using CalamityMod.DataStructures;
using CalamityMod.Dusts;
using CalamityMod.Particles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using CalamityMod.Systems.Collections;

namespace CalamityMod.Buffs.StatDebuffs
{
    public class AbsorberAffliction : ModBuff
    {
        public static DebuffData debuffData = new DebuffData()
        {
            EnemyLostRegen = 400,
            SicknessDebuffScaling = 1,
            MultiplierDamageTickSize = 1 / 20f

        };
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = true;
            CalamityBuffSets.DebuffDataset[Type] = debuffData;
        }

        public override void Update(NPC npc, ref int buffIndex)
        {
            npc.Calamity().absorberAffliction = true;
        }

        internal static void DrawEffects(NPC npc, ref Color drawColor)
        {
            Vector2 npcSize = npc.Center + new Vector2(Main.rand.NextFloat(-npc.width / 2, npc.width / 2), Main.rand.NextFloat(-npc.height / 2, npc.height / 2));

            Color fxColor = Color.Lerp(Color.DarkSeaGreen, Color.MediumSeaGreen, Main.rand.NextFloat(1f));

            if (Main.rand.NextBool(3))
            {
                Particle fx = new CustomSpark(npcSize, Vector2.UnitY * Main.rand.NextFloat(4, -4), "CalamityMod/Particles/Sparkle", false, (int)(Main.rand.Next(16, 26 + 1)), Main.rand.NextFloat(1.5f, 2f), fxColor, new Vector2(0.5f, 1.1f), extraRotation: 0, shrinkSpeed: Main.rand.NextFloat(0.1f, 0.3f) + 0.3f);
                GeneralParticleHandler.SpawnParticle(fx);
            }

            if (Main.rand.Next(5) >= 0)
            {
                Dust dust = Dust.NewDustDirect(npc.position - new Vector2(2f), npc.width + 4, npc.height + 4, ModContent.DustType<LightDust>(), npc.velocity.X * 0.4f, npc.velocity.Y * 0.4f, 100, default, Main.rand.NextFloat(0.8f, 1.8f));
                dust.noGravity = true;
                dust.velocity.Y -= 1.8f;
                dust.velocity.Y *= 2.5f;
                dust.color = Main.rand.NextBool(3) ? Color.PaleGreen : Color.DarkSeaGreen;
            }
        }
    }
}
