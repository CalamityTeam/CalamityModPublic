using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.Projectiles.Rogue
{
    public class DragonShit : ModProjectile
    {
        public NPC target;
        public Vector2 rotationVector = Vector2.UnitY * -13f;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Fire");
            Main.projFrames[Projectile.type] = 5;
        }

        public override void SetDefaults()
        {
            Projectile.width = 64;
            Projectile.height = 64;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 420;
            Projectile.Calamity().rogue = true;
        }

        public override bool? CanHitNPC(NPC target) => Projectile.timeLeft < 380 && target.CanBeChasedBy(Projectile);

        public override void AI()
        {
            target = Projectile.Center.ClosestNPCAt(1200f);
            if (Projectile.localAI[0] == 0f)
            {
                Projectile.ai[0] = Utils.SelectRandom(Main.rand, -1f, 1f);
                Projectile.localAI[0] = 1f;
            }

            Projectile.frameCounter++;
            if (Projectile.frameCounter > 4)
            {
                Projectile.frame++;
                Projectile.frameCounter = 0;
            }
            if (Projectile.frame >= Main.projFrames[Projectile.type])
                Projectile.frame = 0;

            if (Projectile.timeLeft >= 380)
            {
                Projectile.velocity *= 1.07f;
            }
            else if (target != null)
            {
                Projectile.velocity = (Projectile.velocity * 23f + Projectile.SafeDirectionTo(target.Center) * 14.975f) / 24f;
            }
            else
            {
                Projectile.timeLeft = Math.Min(Projectile.timeLeft, 15);
                Projectile.alpha += 17;
                Projectile.velocity = rotationVector;
                rotationVector = rotationVector.RotatedBy(MathHelper.ToRadians(14.975f * Projectile.ai[0]));
            }
        }

        // Reduce damage of projectiles if more than the cap are active
        public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            int projectileCount = Main.player[Projectile.owner].ownedProjectileCounts[Projectile.type];
            int cap = 5;
            int oldDamage = damage;
            if (projectileCount > cap)
            {
                damage -= (int)(oldDamage * ((projectileCount - cap) * 0.05));
                if (damage < 1)
                    damage = 1;
            }
        }

        public override Color? GetAlpha(Color lightColor) => new Color(200, 200, 200, Projectile.alpha);

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D projectileTexture = ModContent.Request<Texture2D>(Texture).Value;
            int frameHeight = ModContent.Request<Texture2D>(Texture).Value.Height / Main.projFrames[Projectile.type];
            int frameY = frameHeight * Projectile.frame;
            Main.spriteBatch.Draw(projectileTexture, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(new Rectangle(0, frameY, projectileTexture.Width, frameHeight)), Projectile.GetAlpha(lightColor), Projectile.rotation, new Vector2((float)projectileTexture.Width / 2f, (float)frameHeight / 2f), Projectile.scale, SpriteEffects.None, 0);
            return false;
        }

        public override void Kill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item14, Projectile.Center);
            CalamityGlobalProjectile.ExpandHitboxBy(Projectile, 80);
            for (int d = 0; d < 5; d++)
            {
                int idx = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 244, 0f, 0f, 100, default, 1f);
                Main.dust[idx].velocity *= 3f;
                if (Main.rand.NextBool(2))
                {
                    Main.dust[idx].scale = 0.5f;
                    Main.dust[idx].fadeIn = 1f + Main.rand.Next(10) * 0.1f;
                }
            }
            for (int d = 0; d < 8; d++)
            {
                int idx = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 244, 0f, 0f, 100, default, 2f);
                Main.dust[idx].noGravity = true;
                Main.dust[idx].velocity *= 5f;
                idx = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 244, 0f, 0f, 100, default, 1f);
                Main.dust[idx].velocity *= 2f;
            }
            CalamityUtils.ExplosionGores(Projectile.Center, 3);
        }
    }
}
