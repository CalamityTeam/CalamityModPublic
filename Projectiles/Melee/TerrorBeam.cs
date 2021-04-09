using CalamityMod.Items.Weapons.Melee;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Melee
{
    public class TerrorBeam : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Terror Beam");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 10;
            ProjectileID.Sets.TrailingMode[projectile.type] = 1;
        }

        public override void SetDefaults()
        {
            projectile.width = 16;
            projectile.height = 16;
            projectile.friendly = true;
            projectile.melee = true;
            projectile.penetrate = 7;
            projectile.alpha = 255;
            projectile.timeLeft = 600;
            projectile.light = 1f;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 10;
        }

        private void SpawnTerrorBlast()
        {
            int projID = ModContent.ProjectileType<TerrorBlast>();
            int blastDamage = (int)(projectile.damage * TerrorBlade.TerrorBlastMultiplier);
            Projectile.NewProjectile(projectile.Center, Vector2.Zero, projID, blastDamage, projectile.knockBack, projectile.owner);
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            projectile.penetrate--;
            if (projectile.penetrate <= 0)
            {
                projectile.Kill();
            }
            else
            {
                if (projectile.owner == Main.myPlayer)
                {
                    SpawnTerrorBlast();
                }
                if (projectile.velocity.X != oldVelocity.X)
                {
                    projectile.velocity.X = -oldVelocity.X;
                }
                if (projectile.velocity.Y != oldVelocity.Y)
                {
                    projectile.velocity.Y = -oldVelocity.Y;
                }
            }
            return false;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            if (projectile.owner == Main.myPlayer && projectile.localAI[0] == 0f)
            {
                projectile.localAI[0] = 1f;
                SpawnTerrorBlast();
            }
        }

        public override void AI()
        {
            if (projectile.localAI[1] == 0f)
            {
                Main.PlaySound(SoundID.Item60, projectile.position);
                projectile.localAI[1] += 1f;
            }
            projectile.alpha -= 40;
            if (projectile.alpha < 0)
            {
                projectile.alpha = 0;
            }
            projectile.rotation = (float)Math.Atan2((double)projectile.velocity.Y, (double)projectile.velocity.X) + 0.785f;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(255, 0, 0, projectile.alpha);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            if (projectile.timeLeft > 595)
                return false;

            CalamityGlobalProjectile.DrawCenteredAndAfterimage(projectile, lightColor, ProjectileID.Sets.TrailingMode[projectile.type], 2);
            return false;
        }

        public override void Kill(int timeLeft)
        {
            // If no on-hit explosion was ever generated, spawn it for free when the beam expires.
            if (projectile.localAI[0] == 0f)
                SpawnTerrorBlast();
        }
    }
}
