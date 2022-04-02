using CalamityMod.Buffs.DamageOverTime;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Rogue
{
    public class PlaguenadeProj : ModProjectile
    {
        public override string Texture => "CalamityMod/Items/Weapons/Rogue/Plaguenade";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Plaguenade");
        }

        public override void SetDefaults()
        {
            projectile.width = 20;
            projectile.height = 20;
            projectile.friendly = true;
            projectile.penetrate = 1;
            projectile.extraUpdates = 1;
            projectile.timeLeft = 180;
            projectile.Calamity().rogue = true;
        }

        public override void AI()
        {
            projectile.ai[0] += 1f;
            if (projectile.ai[0] > 5f)
            {
                projectile.ai[0] = 5f;
                if (projectile.velocity.Y == 0f && projectile.velocity.X != 0f)
                {
                    projectile.velocity.X *= 0.97f;
                    if (projectile.velocity.X > -0.01f && projectile.velocity.X < 0.01f)
                    {
                        projectile.velocity.X = 0f;
                        projectile.netUpdate = true;
                    }
                }
                projectile.velocity.Y += 0.2f;
            }
            projectile.rotation += projectile.velocity.X * 0.1f;
            if (projectile.velocity.Y > 16f)
            {
                projectile.velocity.Y = 16f;
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (projectile.velocity.X != oldVelocity.X)
            {
                projectile.velocity.X = oldVelocity.X * -0.5f;
            }
            if (projectile.velocity.Y != oldVelocity.Y && oldVelocity.Y > 1f)
            {
                projectile.velocity.Y = oldVelocity.Y * -0.5f;
            }
            return false;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D tex = Main.projectileTexture[projectile.type];
            spriteBatch.Draw(tex, projectile.Center - Main.screenPosition, null, projectile.GetAlpha(lightColor), projectile.rotation, tex.Size() / 2f, projectile.scale, SpriteEffects.None, 0f);
            return false;
        }

        public override void Kill(int timeLeft)
        {
            if (projectile.owner == Main.myPlayer)
            {
                Player player = Main.player[projectile.owner];
                int projAmt = projectile.Calamity().stealthStrike ? 28 : 20;
                for (int projIndex = 0; projIndex < projAmt; projIndex++)
                {
                    float speedX = (float)Main.rand.Next(-35, 36) * 0.02f;
                    float speedY = (float)Main.rand.Next(-35, 36) * 0.02f;
                    Projectile.NewProjectile(projectile.position.X, projectile.position.Y, speedX, speedY, ModContent.ProjectileType<PlaguenadeBee>(), player.beeDamage(projectile.damage), player.beeKB(0f), projectile.owner, 0f, 0f);
                }
                Main.PlaySound(SoundID.Item14, projectile.position);
                if (projectile.Calamity().stealthStrike)
                {
                    projectile.position = projectile.Center;
                    projectile.width = projectile.height = 75;
                }
                projectile.position.X = projectile.position.X - (float)(projectile.width / 2);
                projectile.position.Y = projectile.position.Y - (float)(projectile.height / 2);
                projectile.maxPenetrate = -1;
                projectile.penetrate = -1;
                projectile.usesLocalNPCImmunity = true;
                projectile.localNPCHitCooldown = 10;
                projectile.Damage();
                for (int i = 0; i < 10; i++)
                {
                    int smoke = Dust.NewDust(projectile.position, projectile.width, projectile.height, 31, 0f, 0f, 100, default, 2f);
                    Main.dust[smoke].velocity *= 3f;
                    if (Main.rand.NextBool(2))
                    {
                        Main.dust[smoke].scale = 0.5f;
                        Main.dust[smoke].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
                    }
                }
                for (int j = 0; j < 15; j++)
                {
                    int plague = Dust.NewDust(projectile.position, projectile.width, projectile.height, 89, 0f, 0f, 100, default, 3f);
                    Main.dust[plague].noGravity = true;
                    Main.dust[plague].velocity *= 5f;
                    int fire = Dust.NewDust(projectile.position, projectile.width, projectile.height, DustID.Fire, 0f, 0f, 100, default, 2f);
                    Main.dust[fire].velocity *= 2f;
                }
                CalamityUtils.ExplosionGores(projectile.Center, 3);
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(ModContent.BuffType<Plague>(), 240);
        }

        public override void OnHitPvp(Player target, int damage, bool crit)
        {
            target.AddBuff(ModContent.BuffType<Plague>(), 240);
        }
    }
}
