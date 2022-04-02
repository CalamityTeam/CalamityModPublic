using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Rogue
{
    public class DragonShit : ModProjectile
    {
        public NPC target;
        public Vector2 rotationVector = Vector2.UnitY * -13f;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Fire");
            Main.projFrames[projectile.type] = 5;
        }

        public override void SetDefaults()
        {
            projectile.width = 64;
            projectile.height = 64;
            projectile.friendly = true;
            projectile.ignoreWater = true;
            projectile.tileCollide = false;
            projectile.penetrate = 1;
            projectile.timeLeft = 420;
            projectile.Calamity().rogue = true;
        }

        public override bool? CanHitNPC(NPC target) => projectile.timeLeft < 380 && target.CanBeChasedBy(projectile);

        public override void AI()
        {
            target = projectile.Center.ClosestNPCAt(1200f);
            if (projectile.localAI[0] == 0f)
            {
                projectile.ai[0] = Utils.SelectRandom(Main.rand, -1f, 1f);
                projectile.localAI[0] = 1f;
            }

            projectile.frameCounter++;
            if (projectile.frameCounter > 4)
            {
                projectile.frame++;
                projectile.frameCounter = 0;
            }
            if (projectile.frame >= Main.projFrames[projectile.type])
                projectile.frame = 0;

            if (projectile.timeLeft >= 380)
            {
                projectile.velocity *= 1.07f;
            }
            else if (target != null)
            {
                projectile.velocity = (projectile.velocity * 23f + projectile.SafeDirectionTo(target.Center) * 14.975f) / 24f;
            }
            else
            {
                projectile.timeLeft = Math.Min(projectile.timeLeft, 15);
                projectile.alpha += 17;
                projectile.velocity = rotationVector;
                rotationVector = rotationVector.RotatedBy(MathHelper.ToRadians(14.975f * projectile.ai[0]));
            }
        }

        // Reduce damage of projectiles if more than the cap are active
        public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            int projectileCount = Main.player[projectile.owner].ownedProjectileCounts[projectile.type];
            int cap = 5;
            int oldDamage = damage;
            if (projectileCount > cap)
            {
                damage -= (int)(oldDamage * ((projectileCount - cap) * 0.05));
                if (damage < 1)
                    damage = 1;
            }
        }

        public override Color? GetAlpha(Color lightColor) => new Color(200, 200, 200, projectile.alpha);

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D projectileTexture = Main.projectileTexture[projectile.type];
            int frameHeight = Main.projectileTexture[projectile.type].Height / Main.projFrames[projectile.type];
            int frameY = frameHeight * projectile.frame;
            Main.spriteBatch.Draw(projectileTexture, projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(new Rectangle(0, frameY, projectileTexture.Width, frameHeight)), projectile.GetAlpha(lightColor), projectile.rotation, new Vector2((float)projectileTexture.Width / 2f, (float)frameHeight / 2f), projectile.scale, SpriteEffects.None, 0f);
            return false;
        }

        public override void Kill(int timeLeft)
        {
            Main.PlaySound(SoundID.Item14, projectile.Center);
            CalamityGlobalProjectile.ExpandHitboxBy(projectile, 80);
            for (int d = 0; d < 5; d++)
            {
                int idx = Dust.NewDust(projectile.position, projectile.width, projectile.height, 244, 0f, 0f, 100, default, 1f);
                Main.dust[idx].velocity *= 3f;
                if (Main.rand.NextBool(2))
                {
                    Main.dust[idx].scale = 0.5f;
                    Main.dust[idx].fadeIn = 1f + Main.rand.Next(10) * 0.1f;
                }
            }
            for (int d = 0; d < 8; d++)
            {
                int idx = Dust.NewDust(projectile.position, projectile.width, projectile.height, 244, 0f, 0f, 100, default, 2f);
                Main.dust[idx].noGravity = true;
                Main.dust[idx].velocity *= 5f;
                idx = Dust.NewDust(projectile.position, projectile.width, projectile.height, 244, 0f, 0f, 100, default, 1f);
                Main.dust[idx].velocity *= 2f;
            }
            CalamityUtils.ExplosionGores(projectile.Center, 3);
        }
    }
}
