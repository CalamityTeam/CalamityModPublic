using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Melee
{
    public class ExoGladSpears : ModProjectile
    {
        public override string Texture => "CalamityMod/Items/Weapons/Melee/ExoGladius";

        private Color currentColor = Color.Black;

        internal ref float FlySpeedMultiplier => ref projectile.ai[1];

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Gladius Beam");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 7;
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            projectile.width = 28;
            projectile.height = 28;
            projectile.friendly = true;
            projectile.timeLeft = 600;
            projectile.melee = true;
            projectile.tileCollide = false;
            projectile.extraUpdates = 1;
        }

        public override void AI()
        {
            projectile.rotation = projectile.velocity.ToRotation() + MathHelper.PiOver4;
            if (Main.player[projectile.owner].active && !Main.player[projectile.owner].dead)
            {
                NPC potentialTarget = projectile.Center.ClosestNPCAt(1000f);

                if (potentialTarget != null)
                    projectile.velocity = (projectile.velocity * 20f + projectile.SafeDirectionTo(potentialTarget.Center) * 8f) / 21f;
                else if (projectile.Distance(Main.player[projectile.owner].Center) > 1000f)
                {
                    float inertia = 25f * FlySpeedMultiplier;
                    Vector2 directionToOwner = (Main.player[projectile.owner].Center - projectile.Center).SafeNormalize(Vector2.UnitY);
                    projectile.velocity = (projectile.velocity * (inertia - 1f) + directionToOwner * 5f * FlySpeedMultiplier) / inertia;
                }
            }
            else if (projectile.timeLeft > 30)
                projectile.timeLeft = 30;

            Lighting.AddLight(projectile.Center, Main.DiscoR * 0.5f / 255f, Main.DiscoG * 0.5f / 255f, Main.DiscoB * 0.5f / 255f);
            if (currentColor == Color.Black)
            {
                int startPoint = Main.rand.Next(6);
                projectile.localAI[0] = startPoint;
                currentColor = GetStartingColor(startPoint);
            }
            Visuals(projectile, ref currentColor);
        }

        internal static void Visuals(Projectile projectile, ref Color c)
        {
            CalamityUtils.IterateDisco(ref c, ref projectile.localAI[0], 15);
            Vector3 compositeColor = 0.1f * Color.White.ToVector3() + 0.05f * c.ToVector3();
            Lighting.AddLight(projectile.Center, compositeColor);
        }

        internal static Color GetStartingColor(int startPoint = 0)
        {
            switch (startPoint)
            {
                default: return new Color(1f, 0f, 0f, 0f); // Color.Red
                case 1: return new Color(1f, 1f, 0f, 0f); // Color.Yellow
                case 2: return new Color(0f, 1f, 0f, 0f); // Color.Green
                case 3: return new Color(0f, 1f, 1f, 0f); // Color.Turquoise
                case 4: return new Color(0f, 0f, 1f, 0f); // Color.Blue
                case 5: return new Color(1f, 0f, 1f, 0f); // Color.Violet
            }
        }

        // This projectile is always fullbright.
        public override Color? GetAlpha(Color lightColor) => currentColor;

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(projectile, ProjectileID.Sets.TrailingMode[projectile.type], lightColor, 1);
            return false;
        }

        public override void Kill(int timeLeft)
        {
            CalamityGlobalProjectile.ExpandHitboxBy(projectile, 64);
            projectile.maxPenetrate = -1;
            projectile.penetrate = -1;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 10;
            projectile.Damage();
            Main.PlaySound(SoundID.Item74, (int)projectile.position.X, (int)projectile.position.Y);
            int dustType = Utils.SelectRandom(Main.rand, new int[]
            {
                107,
                234,
                269
            });
            for (int i = 0; i < 6; i++)
            {
                Dust.NewDust(projectile.position, projectile.width, projectile.height, dustType, 0, 0);
            }
            for (int i = 0; i < 10; i++)
            {
                Dust dust = Dust.NewDustDirect(projectile.position, projectile.width, projectile.height, dustType, 0, 0);
                dust.noGravity = true;
                dust.velocity *= 3f;

                dust = Dust.NewDustDirect(projectile.position, projectile.width, projectile.height, dustType, 0, 0);
                dust.velocity *= 2f;
                dust.noGravity = true;
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.ExoDebuffs();
        }

        public override void OnHitPvp(Player target, int damage, bool crit)
        {
            target.ExoDebuffs();
        }
    }
}
