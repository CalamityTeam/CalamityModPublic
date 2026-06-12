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
    public class HolyFlames : ModBuff
    {
        public static DebuffData debuffData = new DebuffData()
        {
            EnemyLostRegen = 300,
            HeatDebuffScaling = 1
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
            player.Calamity().holyFlames = true;
        }

        public override void Update(NPC npc, ref int buffIndex)
        {
            npc.Calamity().holyFlames = true;
        }

        internal static void DrawEffects(PlayerDrawSet drawInfo)
        {
            Player Player = drawInfo.drawPlayer;

            if (Main.rand.NextBool(2))
            {
                Vector2 Vect = new Vector2(0f, Main.rand.NextBool(4) ? -5f : -9f).RotatedByRandom(MathHelper.ToRadians(25f)) * Main.rand.NextFloat(0.1f, 1.9f);
                CritSpark spark = new CritSpark(Player.Calamity().RandomDebuffVisualSpot, Vect, Main.rand.NextBool() ? Color.Coral : Color.OrangeRed, Color.Orange, 0.8f, 15, 2f, 1.9f);
                GeneralParticleHandler.SpawnParticle(spark);
            }

            if (Main.rand.NextBool(2))
            {
                Vector2 dustCorner = Player.position - 2f * Vector2.One;
                Vector2 dustVel = Player.velocity + new Vector2(0f, Main.rand.NextFloat(-5f, -1f));
                Dust fire = Dust.NewDustDirect(dustCorner, Player.width + 4, Player.height + 4, DustID.GemTopaz, dustVel.X, dustVel.Y);
                fire.noGravity = true;
                fire.scale = Main.rand.NextFloat(0.7f, 1.2f);
                fire.alpha = 235;
            }
        }

        internal static void DrawEffects(NPC npc, ref Color drawColor)
        {
            if (Main.rand.NextBool(4))
            {
                Vector2 npcSize = npc.Center + new Vector2(Main.rand.NextFloat(-npc.width / 2, npc.width / 2), Main.rand.NextFloat(-npc.height / 2, npc.height / 2));
                Vector2 Vect = new Vector2(0f, Main.rand.NextBool(4) ? -5f : -9f).RotatedByRandom(MathHelper.ToRadians(25f)) * Main.rand.NextFloat(0.1f, 1.9f);
                CritSpark spark = new CritSpark(npcSize, Vect, Main.rand.NextBool() ? Color.Coral : Color.OrangeRed, Color.Orange, 0.8f, 15, 2f, 1.9f);
                GeneralParticleHandler.SpawnParticle(spark);
            }

            if (Main.rand.NextBool(4))
            {
                Vector2 dustCorner = npc.position - 2f * Vector2.One;
                Vector2 dustVel = npc.velocity + new Vector2(0f, Main.rand.NextFloat(-5f, -1f));
                Dust fire = Dust.NewDustDirect(dustCorner, npc.width + 4, npc.height + 4, DustID.GemTopaz, dustVel.X, dustVel.Y);
                fire.noGravity = true;
                fire.scale = Main.rand.NextFloat(0.7f, 1.2f);
                fire.alpha = 235;
            }
            Lighting.AddLight(npc.position, 0.25f, 0.25f, 0.1f);
        }
    }
}
