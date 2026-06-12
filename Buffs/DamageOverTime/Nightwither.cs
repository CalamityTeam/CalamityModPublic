using CalamityMod.DataStructures;
using CalamityMod.Particles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Systems.Collections;

namespace CalamityMod.Buffs.DamageOverTime
{
    public class Nightwither : ModBuff
    {
        public static DebuffData debuffData = new DebuffData()
        {
            EnemyLostRegen = 200,
            ColdDebuffScaling = 1
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
            player.Calamity().nightwither = true;
        }

        public override void Update(NPC npc, ref int buffIndex)
        {
            npc.Calamity().nightwither = true;
        }

        internal static void DrawEffects(PlayerDrawSet drawInfo)
        {
            Player Player = drawInfo.drawPlayer;

            if (Main.rand.NextBool(2))
            {
                Vector2 Vect = new Vector2(0f, Main.rand.NextBool(4) ? -5f : -9f).RotatedByRandom(MathHelper.ToRadians(25f)) * Main.rand.NextFloat(0.1f, 1.9f);
                CritSpark spark = new CritSpark(Player.Calamity().RandomDebuffVisualSpot, Vect, Main.rand.NextBool() ? Color.Cyan : Color.Turquoise, Color.PaleTurquoise, 0.8f, 15, 2f, 1.9f);
                GeneralParticleHandler.SpawnParticle(spark);
            }
            for (int i = 0; i < 2; i++)
            {
                Vector2 dustCorner = Player.position - 2f * Vector2.One;
                Vector2 dustVel = Player.velocity + new Vector2(0f, Main.rand.NextFloat(-11f, -2f));
                Dust moonDust = Dust.NewDustDirect(dustCorner, Player.width + 4, Player.height + 4, Main.rand.NextBool(4) ? 300 : 323, dustVel.X, dustVel.Y);
                moonDust.noGravity = true;
                moonDust.scale = Main.rand.NextFloat(0.5f, 0.5f);
                moonDust.alpha = 235;
            }
        }

        internal static void DrawEffects(NPC npc, ref Color drawColor)
        {
            Vector2 npcSize = npc.Center + new Vector2(Main.rand.NextFloat(-npc.width / 2, npc.width / 2), Main.rand.NextFloat(-npc.height / 2, npc.height / 2));
            if (Main.rand.NextBool(3))
            {
                Vector2 Vect = new Vector2(0f, Main.rand.NextBool(4) ? -5f : -9f).RotatedByRandom(MathHelper.ToRadians(25f)) * Main.rand.NextFloat(0.1f, 1.9f);
                CritSpark spark = new CritSpark(npcSize, Vect, Main.rand.NextBool() ? Color.Cyan : Color.Turquoise, Color.PaleTurquoise, 0.8f, 15, 2f, 1.9f);
                GeneralParticleHandler.SpawnParticle(spark);
            }
            for (int i = 0; i < 2; i++)
            {
                Vector2 dustCorner = npc.position - 2f * Vector2.One;
                Vector2 dustVel = npc.velocity + new Vector2(0f, Main.rand.NextFloat(-11f, -2f));
                Dust moonDust = Dust.NewDustDirect(dustCorner, npc.width + 4, npc.height + 4, Main.rand.NextBool(4) ? 300 : 323, dustVel.X, dustVel.Y);
                moonDust.noGravity = true;
                moonDust.scale = Main.rand.NextFloat(0.5f, 0.5f);
                moonDust.alpha = 235;
            }
        }
    }
}
