using CalamityMod.Dusts;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.SulphurousSea
{
    public class MicrobialCluster : ModNPC
    {
        public const int ChargeRate = 120;
        public const int SlowdownTime = 45;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Microbial Cluster");
        }

        public override void SetDefaults()
        {
            npc.noGravity = true;
            npc.damage = 0;
            npc.width = 24;
            npc.height = 24;
            npc.lifeMax = 5;
            npc.aiStyle = aiType = -1;
            npc.noTileCollide = false;
            npc.noGravity = true;
            npc.HitSound = SoundID.NPCHit1;
            npc.DeathSound = SoundID.NPCDeath1;
            npc.dontTakeDamageFromHostiles = true;
            npc.knockBackResist = 0f;
        }
        public override void AI()
        {
            if (npc.collideX || npc.collideY)
            {
                npc.velocity *= -1f;
                npc.netUpdate = true;
            }
            DelegateMethods.v3_1 = Color.GreenYellow.ToVector3() * 2f;
            Utils.PlotTileLine(npc.Center, npc.Center + npc.velocity * 10f, 8f, new Utils.PerLinePoint(DelegateMethods.CastLightOpen));
            npc.ai[0]++;
            if (npc.ai[0] % SlowdownTime > SlowdownTime - SlowdownTime)
            {
                npc.velocity *= 0.98f;
            }
            if (npc.ai[0] % SlowdownTime == SlowdownTime - 1)
            {
                npc.velocity = npc.velocity.SafeNormalize(-Vector2.UnitY).RotatedByRandom(MathHelper.PiOver4) * 4f;
            }
            if (npc.ai[0] % 32f == 31f)
            {
                Dust dust = Dust.NewDustPerfect(npc.Center, (int)CalamityDusts.SulfurousSeaAcid);
                dust.velocity = Vector2.One.RotatedByRandom(MathHelper.TwoPi) * Main.rand.NextFloat(1f, 2f);
                dust.noGravity = true;
                dust.scale = 1.6f;
            }
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo) => spawnInfo.player.InSulphur() && spawnInfo.water ? 0.4f : 0f;

        public override void NPCLoot()
        {
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            if (npc.life <= 0)
            {
                for (int k = 0; k < 6; k++)
                {
                    Dust dust = Dust.NewDustPerfect(npc.Center, (int)CalamityDusts.SulfurousSeaAcid);
                    dust.velocity = Vector2.One.RotatedByRandom(MathHelper.TwoPi) * Main.rand.NextFloat(1f, 2f);
                    dust.scale = 1.2f;
                }
            }
        }
    }
}
