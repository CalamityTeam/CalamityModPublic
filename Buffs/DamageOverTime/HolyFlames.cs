using CalamityMod.Particles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Buffs.DamageOverTime
{
    public class HolyFlames : ModBuff
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
            player.Calamity().hFlames = true;
        }

        public override void Update(NPC npc, ref int buffIndex)
        {
            if (npc.Calamity().hFlames < npc.buffTime[buffIndex])
                npc.Calamity().hFlames = npc.buffTime[buffIndex];
            npc.DelBuff(buffIndex);
            buffIndex--;
        }

        internal static void DrawEffects(PlayerDrawSet drawInfo, bool hasDebuffResistance = false)
        {
            Player Player = drawInfo.drawPlayer;

            if (Main.rand.NextBool(hasDebuffResistance ? 4 : 2))
            {
                Vector2 Vect = new Vector2(0f, Main.rand.NextBool(4) ? -5f : -9f).RotatedByRandom(MathHelper.ToRadians(25f)) * Main.rand.NextFloat(0.1f, 1.9f);
                CritSpark spark = new CritSpark(Player.Calamity().RandomDebuffVisualSpot, Vect, Main.rand.NextBool() ? Color.Coral : Color.OrangeRed, Color.Orange, (hasDebuffResistance ? 0.4f : 0.8f), 15, 2f, 1.9f);
                GeneralParticleHandler.SpawnParticle(spark);
            }

            if (Main.rand.NextBool(hasDebuffResistance ? 4 : 2))
            {
                Vector2 dustCorner = Player.position - 2f * Vector2.One;
                Vector2 dustVel = Player.velocity + new Vector2(0f, Main.rand.NextFloat(-5f, -1f));
                int d = Dust.NewDust(dustCorner, Player.width + 4, Player.height + 4, 87, dustVel.X, dustVel.Y);
                Main.dust[d].noGravity = true;
                Main.dust[d].scale = hasDebuffResistance ? Main.rand.NextFloat(0.3f, 0.5f) : Main.rand.NextFloat(0.7f, 1.2f);
                Main.dust[d].alpha = 235;
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
                int d = Dust.NewDust(dustCorner, npc.width + 4, npc.height + 4, 87, dustVel.X, dustVel.Y);
                Main.dust[d].noGravity = true;
                Main.dust[d].scale = Main.rand.NextFloat(0.7f, 1.2f);
                Main.dust[d].alpha = 235;
            }
            Lighting.AddLight(npc.position, 0.25f, 0.25f, 0.1f);
        }
    }
}
