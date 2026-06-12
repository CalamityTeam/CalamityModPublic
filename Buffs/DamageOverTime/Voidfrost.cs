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
    public class Voidfrost : ModBuff
    {
        public static DebuffData debuffData = new DebuffData()
        {
            EnemyLostRegen = 500, //250 dps is extra strong for its tier due to being difficult to inflict
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
            player.Calamity().voidfrost = true;
        }

        public override void Update(NPC npc, ref int buffIndex)
        {
            npc.Calamity().voidfrost = true;
        }

        internal static void DrawEffects(PlayerDrawSet drawInfo, bool hasDebuffResistance = false)
        {
            Player Player = drawInfo.drawPlayer;

            if (Main.rand.NextBool(5))
            {
                Vector2 Vect = new Vector2(0f, Main.rand.NextBool(4) ? -5f : -9f).RotatedByRandom(MathHelper.ToRadians(25f)) * Main.rand.NextFloat(0.1f, 1.9f);
                SnowflakeSparkle snowflake = new SnowflakeSparkle(Player.Calamity().RandomDebuffVisualSpot, Vect, Main.rand.NextBool() ? Color.Cyan : Color.DarkBlue, Color.DodgerBlue, 0.4f, 15, 2f, 1.9f);
                GeneralParticleHandler.SpawnParticle(snowflake);
            }
            if (Main.rand.NextBool(40))
            {
                Vector2 Vect = new Vector2(0f, Main.rand.NextBool(4) ? -5f : -9f).RotatedByRandom(MathHelper.ToRadians(25f)) * Main.rand.NextFloat(0.1f, 1.9f);
                MediumMistParticle mist = new MediumMistParticle(Player.Calamity().RandomDebuffVisualSpot, Vect, new Color(172, 238, 255), new Color(145, 170, 188), Main.rand.NextFloat(0.5f, 1.5f), 245 - Main.rand.Next(50), 0.02f);
                GeneralParticleHandler.SpawnParticle(mist);
            }
            Vector2 dustCorner = Player.position - 2f * Vector2.One;
            Vector2 dustVel = Player.velocity + new Vector2(0f, Main.rand.NextFloat(-11f, -2f));
            Dust dust = Dust.NewDustDirect(dustCorner, Player.width + 4, Player.height + 4, Main.rand.NextBool(4) ? 20 : 113, dustVel.X, dustVel.Y);
            dust.noGravity = true;
            dust.scale = Main.rand.NextFloat(1f, 0.3f);
            dust.alpha = 10;
        }

        internal static void DrawEffects(NPC npc, ref Color drawColor)
        {
            Vector2 npcSize = npc.Center + new Vector2(Main.rand.NextFloat(-npc.width / 2, npc.width / 2), Main.rand.NextFloat(-npc.height / 2, npc.height / 2));
            if (Main.rand.NextBool(5))
            {
                Vector2 Vect = new Vector2(0f, Main.rand.NextBool(4) ? -5f : -9f).RotatedByRandom(MathHelper.ToRadians(25f)) * Main.rand.NextFloat(0.1f, 1.9f);
                SnowflakeSparkle snowflake = new SnowflakeSparkle(npcSize, Vect, Main.rand.NextBool() ? Color.Cyan : Color.DarkBlue, Color.DodgerBlue, 0.8f, 15, 2f, 1.9f);
                GeneralParticleHandler.SpawnParticle(snowflake);
            }
            if (Main.rand.NextBool(40))
            {
                Vector2 Vect = new Vector2(0f, Main.rand.NextBool(4) ? -5f : -9f).RotatedByRandom(MathHelper.ToRadians(25f)) * Main.rand.NextFloat(0.1f, 1.9f);
                MediumMistParticle mist = new MediumMistParticle(npcSize, Vect, new Color(172, 238, 255), new Color(145, 170, 188), Main.rand.NextFloat(0.5f, 1.5f), 245 - Main.rand.Next(50), 0.02f);
                GeneralParticleHandler.SpawnParticle(mist);
            }

            Vector2 dustCorner = npc.position - 2f * Vector2.One;
            Vector2 dustVel = npc.velocity + new Vector2(0f, Main.rand.NextFloat(-11f, -2f));
            Dust dust = Dust.NewDustDirect(dustCorner, npc.width + 4, npc.height + 4, Main.rand.NextBool(4) ? 20 : 113, dustVel.X, dustVel.Y);
            dust.noGravity = true;
            dust.scale = Main.rand.NextFloat(1f, 0.3f);
            dust.alpha = 10;
        }
    }
}
