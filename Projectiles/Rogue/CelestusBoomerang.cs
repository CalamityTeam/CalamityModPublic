using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Rogue
{
    public class CelestusBoomerang : ModProjectile
    {
        public override string Texture => "CalamityMod/Items/Weapons/Rogue/Celestus";

        private bool initialized = false;
        private float speed = 25f;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Celestus");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 8;
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            projectile.width = projectile.height = 94;
            projectile.friendly = true;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.penetrate = -1;
            projectile.extraUpdates = 3;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 30;
            projectile.Calamity().rogue = true;
        }

        public override void AI()
        {
            Player player = Main.player[projectile.owner];

            if (!initialized)
            {
                speed = projectile.velocity.Length();
                initialized = true;
            }

            Lighting.AddLight(projectile.Center, Main.DiscoR * 0.5f / 255f, Main.DiscoG * 0.5f / 255f, Main.DiscoB * 0.5f / 255f);
            projectile.rotation += 1f;

            if (projectile.soundDelay == 0)
            {
                projectile.soundDelay = 8;
                Main.PlaySound(SoundID.Item7, projectile.position);
            }

            switch (projectile.ai[0])
            {
                case 0f:
                    projectile.ai[1] += 1f;
                    if (projectile.ai[1] >= 40f)
                    {
                        projectile.ai[0] = 1f;
                        projectile.ai[1] = 0f;
                        projectile.netUpdate = true;
                    }
                    break;
                case 1f:
                    float returnSpeed = 25f;
                    float acceleration = 5f;
                    Vector2 playerVec = player.Center - projectile.Center;
                    if (playerVec.Length() > 4000f)
                    {
                        projectile.Kill();
                    }
                    playerVec.Normalize();
                    playerVec *= returnSpeed;
                    if (projectile.velocity.X < playerVec.X)
                    {
                        projectile.velocity.X += acceleration;
                        if (projectile.velocity.X < 0f && playerVec.X > 0f)
                        {
                            projectile.velocity.X += acceleration;
                        }
                    }
                    else if (projectile.velocity.X > playerVec.X)
                    {
                        projectile.velocity.X -= acceleration;
                        if (projectile.velocity.X > 0f && playerVec.X < 0f)
                        {
                            projectile.velocity.X -= acceleration;
                        }
                    }
                    if (projectile.velocity.Y < playerVec.Y)
                    {
                        projectile.velocity.Y += acceleration;
                        if (projectile.velocity.Y < 0f && playerVec.Y > 0f)
                        {
                            projectile.velocity.Y += acceleration;
                        }
                    }
                    else if (projectile.velocity.Y > playerVec.Y)
                    {
                        projectile.velocity.Y -= acceleration;
                        if (projectile.velocity.Y > 0f && playerVec.Y < 0f)
                        {
                            projectile.velocity.Y -= acceleration;
                        }
                    }
                    if (Main.myPlayer == projectile.owner)
                    {
                        Rectangle projHitbox = new Rectangle((int)projectile.position.X, (int)projectile.position.Y, projectile.width, projectile.height);
                        Rectangle playerHitbox = new Rectangle((int)player.position.X, (int)player.position.Y, player.width, player.height);
                        if (projHitbox.Intersects(playerHitbox))
                        {
                            if (projectile.Calamity().stealthStrike)
                            {
                                projectile.velocity *= -1f;
                                projectile.timeLeft = 600;
                                projectile.penetrate = 1;
                                projectile.localNPCHitCooldown = -1;
                                projectile.ai[0] = 2f;
                                projectile.netUpdate = true;
                            }
                            else
                                projectile.Kill();
                        }
                    }
                    break;
                case 2f:
                    CalamityGlobalProjectile.HomeInOnNPC(projectile, true, 250f, speed, 20f);
                    break;
                default:
                    break;
            }
        }

        public override Color? GetAlpha(Color lightColor) => new Color(250, 250, 250, 50);

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.ExoDebuffs();
            OnHitEffects();
        }

        public override void OnHitPvp(Player target, int damage, bool crit)
        {
            target.ExoDebuffs();
            OnHitEffects();
        }

        private void OnHitEffects()
        {
            if (projectile.owner == Main.myPlayer)
            {
                float spread = 45f * 0.0174f;
                double startAngle = Math.Atan2(projectile.velocity.X, projectile.velocity.Y) - spread / 2;
                double deltaAngle = spread / 8f;
                double offsetAngle;
                for (int i = 0; i < 4; i++)
                {
                    offsetAngle = startAngle + deltaAngle * (i + i * i) / 2f + 32f * i;
                    Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, (float)(Math.Sin(offsetAngle) * 2f), (float)(Math.Cos(offsetAngle) * 2f), ModContent.ProjectileType<Celestus2>(), (int)(projectile.damage * 0.7), projectile.knockBack, projectile.owner);
                    Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, (float)(-Math.Sin(offsetAngle) * 2f), (float)(-Math.Cos(offsetAngle) * 2f), ModContent.ProjectileType<Celestus2>(), (int)(projectile.damage * 0.7), projectile.knockBack, projectile.owner);
                }
            }
            Main.PlaySound(SoundID.Item122, projectile.Center);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(projectile, ProjectileID.Sets.TrailingMode[projectile.type], lightColor, 1);
            return false;
        }
    }
}
