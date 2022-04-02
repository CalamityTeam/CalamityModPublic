using CalamityMod.Buffs.DamageOverTime;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Rogue
{
    public class GodsParanoiaDart : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Kunai");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            projectile.width = 10;
            projectile.height = 10;
            projectile.ignoreWater = true;
            projectile.friendly = true;
            projectile.Calamity().rogue = true;
            projectile.penetrate = 1;
            projectile.tileCollide = false;
            projectile.timeLeft = 600;
            projectile.aiStyle = 27;
        }

        public override void AI()
        {
            projectile.spriteDirection = projectile.direction = (projectile.velocity.X > 0).ToDirectionInt();
            projectile.rotation = projectile.velocity.ToRotation() + (MathHelper.Pi / 2);

            projectile.velocity.X *= 1.025f;
            projectile.velocity.Y *= 1.025f;
            if (projectile.velocity.X > 12f)
            {
                projectile.velocity.X = 12f;
            }
            if (projectile.velocity.Y > 12f)
            {
                projectile.velocity.Y = 12f;
            }

            if (Main.rand.NextBool(3))
            {
                Dust flame = Dust.NewDustDirect(projectile.position, projectile.width, projectile.height, Main.rand.NextBool(3) ? 56 : 242, 0f, 0f, 0, default, 1f);
                flame.velocity *= 0.1f;
                flame.scale = 1.3f;
                flame.noGravity = true;
            }
        }

        public override void Kill(int timeLeft)
        {
            projectile.position = projectile.Center;
            projectile.width = projectile.height = 192;
            projectile.position.X = projectile.position.X - (float)(projectile.width / 2);
            projectile.position.Y = projectile.position.Y - (float)(projectile.height / 2);
            projectile.maxPenetrate = -1;
            projectile.penetrate = -1;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = -1;
            projectile.damage /= 2;
            projectile.Damage();
            for (int i = 0; i < 2; i++)
            {
                Dust flame = Dust.NewDustDirect(projectile.position, projectile.width, projectile.height, Main.rand.NextBool(3) ? 56 : 242, 0f, 0f, 100, default, 2f);
                flame.velocity *= 3f;
                if (Main.rand.NextBool(2))
                    flame.scale = 0.5f;

                if (Main.rand.NextBool(2))
                {
                    flame.scale *= 0.5f;
                    flame.fadeIn = Main.rand.NextFloat(1f, 2f);
                }
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(ModContent.BuffType<GodSlayerInferno>(), 120);
        }

        public override void OnHitPvp(Player target, int damage, bool crit)
        {
            target.AddBuff(ModContent.BuffType<GodSlayerInferno>(), 120);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            if (projectile.timeLeft > 595)
                return false;

            CalamityUtils.DrawAfterimagesCentered(projectile, ProjectileID.Sets.TrailingMode[projectile.type], lightColor, 1);
            return false;
        }
    }
}
