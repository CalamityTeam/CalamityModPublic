using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Dusts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Ranged
{
    public class AstralRound : ModProjectile
    {
        private float speed = 0f;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Astral Round");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            projectile.width = 4;
            projectile.height = 4;
            projectile.aiStyle = 1;
            projectile.friendly = true;
            projectile.ranged = true;
            projectile.penetrate = 1;
            projectile.timeLeft = 600;
            projectile.light = 0.25f;
            projectile.extraUpdates = 2;
            aiType = ProjectileID.Bullet;
            projectile.Calamity().pointBlankShotDuration = CalamityGlobalProjectile.basePointBlankShotDuration;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Collision.HitTiles(projectile.position, projectile.velocity, projectile.width, projectile.height);
            Main.PlaySound(SoundID.Dig, (int)projectile.position.X, (int)projectile.position.Y, 1, 1f, 0f);
            return true;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(projectile, ProjectileID.Sets.TrailingMode[projectile.type], lightColor, 1);
            return false;
        }

        public override bool PreAI()
        {
            projectile.rotation = projectile.velocity.ToRotation() + MathHelper.ToRadians(90f);
            projectile.spriteDirection = projectile.direction;

            projectile.localAI[0] += 1f;
            if (projectile.localAI[0] > 4f)
            {
                if (Main.rand.NextBool(2))
                {
                    int randomDust = Utils.SelectRandom(Main.rand, new int[]
                    {
                    ModContent.DustType<AstralOrange>(),
                    ModContent.DustType<AstralBlue>()
                    });
                    int astral = Dust.NewDust(projectile.position, 1, 1, randomDust, 0f, 0f, 0, default, 0.5f);
                    Main.dust[astral].alpha = projectile.alpha;
                    Main.dust[astral].velocity *= 0f;
                    Main.dust[astral].noGravity = true;
                }
            }

            if (speed == 0f)
                speed = projectile.velocity.Length();

            CalamityGlobalProjectile.HomeInOnNPC(projectile, !projectile.tileCollide, 200f, speed, 12f);
            return false;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(ModContent.BuffType<AstralInfectionDebuff>(), 120);
        }
    }
}
