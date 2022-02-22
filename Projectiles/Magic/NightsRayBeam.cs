using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Magic
{
    public class NightsRayBeam : ModProjectile
    {
        public ref float Time => ref projectile.ai[0];
        public bool HasFiredSideBeams
        {
            get => projectile.ai[1] == 1f;
            set => projectile.ai[1] = value.ToInt();
        }
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Ray");
        }

        public override void SetDefaults()
        {
            projectile.width = 4;
            projectile.height = 4;
            projectile.friendly = true;
            projectile.magic = true;
            projectile.ignoreWater = true;
            projectile.penetrate = 10;
            projectile.extraUpdates = 100;
            projectile.timeLeft = 150;
        }

        public override void AI()
        {
            Time++;
            if (Time >= 10f)
            {
                for (int i = 0; i < 2; i++)
                {
                    Vector2 dustSpawnPos = projectile.position - projectile.velocity * i / 2f;
                    Dust corruptMagic = Dust.NewDustPerfect(dustSpawnPos, 27);
                    corruptMagic.color = Color.Lerp(Color.Fuchsia, Color.Magenta, Main.rand.NextFloat(0.6f));
                    corruptMagic.scale = Main.rand.NextFloat(0.96f, 1.04f);
                    corruptMagic.noGravity = true;
                    corruptMagic.velocity *= 0.1f;
                }
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            if (!HasFiredSideBeams && projectile.owner == Main.myPlayer)
            {
                Vector2 baseSpawnPositionOffset = Main.rand.NextVector2CircularEdge(40f, 40f);
                for (int i = 0; i < 4; i++)
                {
                    Vector2 spawnPosition = target.Center + baseSpawnPositionOffset.RotatedBy(MathHelper.TwoPi * i / 4f);
                    Projectile.NewProjectile(spawnPosition, Vector2.Zero, ModContent.ProjectileType<NightOrb>(), (int)(projectile.damage * 0.8), projectile.knockBack, projectile.owner);
                }
                HasFiredSideBeams = true;
                projectile.netUpdate = true;
            }
        }
    }
}
