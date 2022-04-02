using CalamityMod.Buffs.DamageOverTime;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Melee
{
    public class TenebreusTidesWaterProjectile : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Water Spear");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 4;
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            projectile.width = 20;
            projectile.height = 20;
            projectile.aiStyle = 27;
            projectile.friendly = true;
            projectile.melee = true;
            projectile.penetrate = -1;
            projectile.timeLeft = 300;
            projectile.ignoreWater = true;
            projectile.usesIDStaticNPCImmunity = true;
            projectile.idStaticNPCHitCooldown = 10;
            projectile.tileCollide = false;
            aiType = ProjectileID.LightBeam;
        }

        public override void AI()
        {
            Lighting.AddLight((int)projectile.Center.X / 16, (int)projectile.Center.Y / 16, 0f, 0f, (255 - projectile.alpha) * 1f / 255f);
            int water = Dust.NewDust(projectile.position, projectile.width, projectile.height, 33, 0f, 0f, 100, default, 0.4f);
            Main.dust[water].noGravity = true;
            Main.dust[water].velocity *= 0.5f;
            Main.dust[water].velocity += projectile.velocity * 0.1f;

            projectile.rotation = projectile.velocity.ToRotation() + MathHelper.ToRadians(135f);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            if (projectile.timeLeft == 300)
                return false;
            CalamityUtils.DrawAfterimagesCentered(projectile, ProjectileID.Sets.TrailingMode[projectile.type], lightColor, 1);
            return false;
        }

        public override Color? GetAlpha(Color lightColor) => new Color(50, 50, 255, projectile.alpha);

        public override void Kill(int timeLeft)
        {
            Main.PlaySound(SoundID.Item10, projectile.position);
            projectile.position = projectile.Center;
            projectile.width = projectile.height = 64;
            projectile.position.X -= (float)(projectile.width / 2);
            projectile.position.Y -= (float)(projectile.height / 2);
            for (int dustIndex = 0; dustIndex <= 30; dustIndex++)
            {
                Vector2 dustVel = new Vector2(Main.rand.Next(-10, 11), Main.rand.Next(-10, 11));
                float dustSpeed = (float)Main.rand.Next(3, 9);
                float dist = dustVel.Length();
                dist = dustSpeed / dist;
                dustVel.X *= dist;
                dustVel.Y *= dist;
                int water = Dust.NewDust(projectile.position, projectile.width, projectile.height, 33, 0f, 0f, 100, default, 1.2f);
                Dust dust = Main.dust[water];
                dust.noGravity = true;
                dust.position.X = projectile.Center.X;
                dust.position.Y = projectile.Center.Y;
                dust.position.X += (float)Main.rand.Next(-10, 11);
                dust.position.Y += (float)Main.rand.Next(-10, 11);
                dust.velocity = dustVel;
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(ModContent.BuffType<CrushDepth>(), 180);
            SwordSpam(target.Center);
        }

        public override void OnHitPvp(Player target, int damage, bool crit)
        {
            target.AddBuff(ModContent.BuffType<CrushDepth>(), 180);
            SwordSpam(target.Center);
        }

        // Spawns a storm of water projectiles on-hit.
        public void SwordSpam(Vector2 targetPos)
        {
            int projAmt = 3;
            for (int i = 0; i < projAmt; ++i)
            {
                int type = Main.rand.NextBool() ? ModContent.ProjectileType<TenebreusTidesWaterSword>() : ModContent.ProjectileType<TenebreusTidesWaterSpear>();
                if (projectile.owner == Main.myPlayer)
                {
                    CalamityUtils.ProjectileBarrage(projectile.Center, targetPos, Main.rand.NextBool(), 1000f, 1400f, 80f, 900f, Main.rand.NextFloat(25f, 35f), type, projectile.damage / 2, projectile.knockBack * 0.5f, projectile.owner);
                }
            }
        }
    }
}
