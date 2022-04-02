using CalamityMod.Projectiles.Typeless;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Rogue
{
    public class IceStarProjectile : ModProjectile
    {
        public override string Texture => "CalamityMod/Items/Weapons/Rogue/IceStar";

        private bool initStealth = false;
        private Vector2 initialVelocity;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Star");
        }

        public override void SetDefaults()
        {
            projectile.width = 30;
            projectile.height = 30;
            projectile.friendly = true;
            projectile.penetrate = 3;
            projectile.aiStyle = 2;
            projectile.timeLeft = 280;
            aiType = ProjectileID.ThrowingKnife;
            projectile.Calamity().rogue = true;
            projectile.coldDamage = true;
        }

        public override void AI()
        {
            if (!initStealth && projectile.Calamity().stealthStrike)
            {
                projectile.penetrate = -1;
                projectile.usesLocalNPCImmunity = true;
                projectile.localNPCHitCooldown = 9;
                projectile.tileCollide = false;
                initialVelocity = projectile.velocity;
                initStealth = true;
            }

            projectile.rotation += 0.5f;
            if (Main.rand.NextBool(3))
            {
                Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, 67, projectile.velocity.X * 0.5f, projectile.velocity.Y * 0.5f);
            }
            Vector2 center = projectile.Center;
            float maxDistance = projectile.Calamity().stealthStrike ? 800f : 400f;
            bool homeIn = false;

            for (int i = 0; i < Main.maxNPCs; i++)
            {
                if (Main.npc[i].CanBeChasedBy(projectile, false))
                {
                    float extraDistance = (float)(Main.npc[i].width / 2) + (float)(Main.npc[i].height / 2);

                    if (Vector2.Distance(Main.npc[i].Center, projectile.Center) < (maxDistance + extraDistance) && Collision.CanHit(projectile.Center, 1, 1, Main.npc[i].Center, 1, 1))
                    {
                        center = Main.npc[i].Center;
                        homeIn = true;
                        break;
                    }
                }
            }

            if (homeIn)
            {
                Vector2 moveDirection = projectile.SafeDirectionTo(center, Vector2.UnitY);
                projectile.velocity = (projectile.velocity * 20f + moveDirection * 14f) / (21f);
            }
            projectile.velocity = initStealth && !homeIn ? initialVelocity : projectile.velocity;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D tex = Main.projectileTexture[projectile.type];
            spriteBatch.Draw(tex, projectile.Center - Main.screenPosition, null, projectile.GetAlpha(lightColor), projectile.rotation, tex.Size() / 2f, projectile.scale, SpriteEffects.None, 0f);
            return false;
        }

        public override void Kill(int timeLeft)
        {
            Main.PlaySound(SoundID.Item27, projectile.position);
            for (int k = 0; k < 5; k++)
            {
                Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, 67, projectile.oldVelocity.X * 0.5f, projectile.oldVelocity.Y * 0.5f);
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            if (initStealth)
            {
                if (projectile.owner == Main.myPlayer && Main.player[projectile.owner].ownedProjectileCounts[ModContent.ProjectileType<KelvinCatalystStar>()] < 15)
                {
                    float spread = 45f * 0.0174f;
                    double startAngle = Math.Atan2(projectile.velocity.X, projectile.velocity.Y) - spread / 2;
                    double deltaAngle = spread / 8f;
                    double offsetAngle;
                    int i;
                    for (i = 0; i < 4; i++)
                    {
                        offsetAngle = startAngle + deltaAngle * (i + i * i) / 2f + 32f * i;

                        Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, (float)(Math.Sin(offsetAngle) * 4f), (float)(Math.Cos(offsetAngle) * 4f),
                            ModContent.ProjectileType<KelvinCatalystStar>(), projectile.damage / 8, projectile.knockBack * 0.5f, projectile.owner, 0f, 0f);

                        Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, (float)(-Math.Sin(offsetAngle) * 4f), (float)(-Math.Cos(offsetAngle) * 4f),
                            ModContent.ProjectileType<KelvinCatalystStar>(), projectile.damage / 8, projectile.knockBack * 0.5f, projectile.owner, 0f, 0f);
                    }
                }
            }
        }

        public override void OnHitPvp(Player target, int damage, bool crit)
        {
            if (initStealth)
            {
                if (projectile.owner == Main.myPlayer && Main.player[projectile.owner].ownedProjectileCounts[ModContent.ProjectileType<KelvinCatalystStar>()] < 15)
                {
                    float spread = 45f * 0.0174f;
                    double startAngle = Math.Atan2(projectile.velocity.X, projectile.velocity.Y) - spread / 2;
                    double deltaAngle = spread / 8f;
                    double offsetAngle;
                    int i;
                    for (i = 0; i < 4; i++)
                    {
                        offsetAngle = startAngle + deltaAngle * (i + i * i) / 2f + 32f * i;

                        Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, (float)(Math.Sin(offsetAngle) * 4f), (float)(Math.Cos(offsetAngle) * 4f),
                            ModContent.ProjectileType<KelvinCatalystStar>(), projectile.damage / 8, projectile.knockBack * 0.5f, projectile.owner, 0f, 0f);

                        Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, (float)(-Math.Sin(offsetAngle) * 4f), (float)(-Math.Cos(offsetAngle) * 4f),
                            ModContent.ProjectileType<KelvinCatalystStar>(), projectile.damage / 8, projectile.knockBack * 0.5f, projectile.owner, 0f, 0f);
                    }
                }
            }
        }
    }
}
