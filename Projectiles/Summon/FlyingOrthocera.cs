using CalamityMod.Dusts;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Summon
{
    public class FlyingOrthocera : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Summon";
        public const float SearchDistance = 850f;

        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 4;
            ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 46;
            Projectile.height = 42;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.sentry = true;
            Projectile.timeLeft = Projectile.SentryLifeTime;
            Projectile.penetrate = -1;
            Projectile.DamageType = DamageClass.Summon;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            if (Projectile.localAI[0] == 0f)
            {
                for (int i = 0; i < 56; i++)
                {
                    float angle = MathHelper.TwoPi / 56f * i;
                    Dust dust = Dust.NewDustPerfect(Projectile.Center, (int)CalamityDusts.SulfurousSeaAcid);
                    dust.scale = 1.5f;
                    dust.velocity = angle.ToRotationVector2() * 7f;
                    dust.noGravity = true;
                }
                Projectile.localAI[0] += 1f;
            }
            Projectile.frameCounter++;
            if (Projectile.frameCounter % 5f == 4f)
            {
                Projectile.frame++;
            }
            if (Projectile.frame >= Main.projFrames[Projectile.type])
            {
                Projectile.frame = 0;
            }
            NPC potentialTarget = Projectile.Center.MinionHoming(SearchDistance, player);
            if (potentialTarget != null)
            {
                Projectile.rotation = Projectile.rotation.AngleTowards(Projectile.AngleTo(potentialTarget.Center) - MathHelper.PiOver4, 0.085f);
                Projectile.spriteDirection = (Projectile.rotation < MathHelper.Pi).ToDirectionInt();
                if (Projectile.ai[0]++ % 30f == 29f)
                {
                    if (Projectile.owner == Main.myPlayer)
                    {
                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Projectile.SafeDirectionTo(potentialTarget.Center, Vector2.UnitY) * 11f, ModContent.ProjectileType<FlyingOrthoceraStream>(), Projectile.damage, 4f, Projectile.owner);
                    }
                }
            }
            else
            {
                Projectile.rotation = Projectile.rotation.AngleTowards(0f, 0.15f);
            }
        }
    }
}
