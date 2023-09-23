using CalamityMod.Particles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Buffs.DamageOverTime
{
    public class Dragonfire : ModBuff
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
            player.Calamity().dragonFire = true;
        }

        public override void Update(NPC npc, ref int buffIndex)
        {
            if (npc.Calamity().dragonFire < npc.buffTime[buffIndex])
                npc.Calamity().dragonFire = npc.buffTime[buffIndex];
            npc.DelBuff(buffIndex);
            buffIndex--;
        }

        internal static void DrawEffects(Player player)
        {

        }

        internal static void DrawEffects(NPC npc, ref Color drawColor)
        {
            Vector2 npcSize = npc.Center + new Vector2(Main.rand.NextFloat(-npc.width / 2, npc.width / 2), Main.rand.NextFloat(-npc.height / 2, npc.height / 2));
            Vector2 Vect2 = new Vector2(0f, Main.rand.NextBool(4) ? -2f : -8f).RotatedByRandom(MathHelper.ToRadians(Main.rand.NextBool(3) ? 10 : 35f)) * Main.rand.NextFloat(0.1f, 1.9f);
            SparkParticle spark = new SparkParticle(npcSize, new Vector2(Vect2.X - npc.velocity.X * 0.3f, Vect2.Y), false, 10, Main.rand.NextFloat(0.4f, 0.5f), Main.rand.NextBool(2) ? Color.OrangeRed : Color.Orange);
            GeneralParticleHandler.SpawnParticle(spark);

            if (Main.rand.NextBool(3))
            {
                Vector2 Vect = new Vector2(0f, Main.rand.NextBool(2) ? -3f : -14f).RotatedByRandom(MathHelper.ToRadians(25f)) * Main.rand.NextFloat(0.1f, 1.9f);
                SmallSmokeParticle smoke = new SmallSmokeParticle(npcSize, Vect, Color.DimGray, Main.rand.NextBool(2) ? Color.Black : Color.DimGray, Main.rand.NextFloat(0.2f, 1.2f), 100);
                GeneralParticleHandler.SpawnParticle(smoke);
            }
            Lighting.AddLight(npc.position, 0.1f, 0f, 0.135f);
        }
    }
}
