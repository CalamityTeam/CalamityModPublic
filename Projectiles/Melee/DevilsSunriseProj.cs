using CalamityMod.Buffs.DamageOverTime;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Melee
{
    public class DevilsSunriseProj : ModProjectile
    {
        private int red;
        private const int greenAndBlue = 100;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Devil's Sunrise");
            Main.projFrames[projectile.type] = 28;
        }

        public override void SetDefaults()
        {
            projectile.width = 148;
            projectile.height = 68;
            projectile.friendly = true;
            projectile.penetrate = -1;
            projectile.tileCollide = false;
            projectile.melee = true;
            projectile.ownerHitCheck = true;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 5;
            projectile.Calamity().trueMelee = true;
        }

        public override void AI()
        {
            if (projectile.ai[1] < 255f)
                projectile.ai[1] += 1f;

            if (projectile.ai[1] == 255f)
                projectile.damage = (int)((double)projectile.Calamity().defDamage * 2.0);

            red = 64 + (int)(projectile.ai[1] * 0.75f);
            if (red > 255)
                red = 255;

            Player player = Main.player[projectile.owner];
            float num = 0f;
            Vector2 vector = player.RotatedRelativePoint(player.MountedCenter, true);

            if (projectile.spriteDirection == -1)
                num = 3.14159274f;

            if (++projectile.frame >= Main.projFrames[projectile.type])
                projectile.frame = 0;

            projectile.soundDelay--;
            if (projectile.soundDelay <= 0)
            {
                Main.PlaySound(SoundID.Item15, projectile.Center);
                projectile.soundDelay = 24;
            }

            if (Main.myPlayer == projectile.owner)
            {
                if (player.channel && !player.noItems && !player.CCed)
                {
                    float scaleFactor6 = 1f;

                    if (player.ActiveItem().shoot == projectile.type)
                        scaleFactor6 = player.ActiveItem().shootSpeed * projectile.scale;

                    Vector2 vector13 = Main.MouseWorld - vector;
                    vector13.Normalize();
                    if (vector13.HasNaNs())
                        vector13 = Vector2.UnitX * (float)player.direction;

                    vector13 *= scaleFactor6;
                    if (vector13.X != projectile.velocity.X || vector13.Y != projectile.velocity.Y)
                        projectile.netUpdate = true;

                    projectile.velocity = vector13;
                }
                else
                    projectile.Kill();
            }

            Vector2 vector14 = projectile.Center + projectile.velocity * 3f;
            Lighting.AddLight(vector14, (float)((double)red * 0.001), 0.1f, 0.1f);

            if (Main.rand.NextBool(3))
            {
                int dust = Dust.NewDust(vector14 - projectile.Size / 2f, projectile.width, projectile.height, 66, projectile.velocity.X, projectile.velocity.Y, 100, new Color(red, greenAndBlue, greenAndBlue), 1f);
                Main.dust[dust].noGravity = true;
                Main.dust[dust].position -= projectile.velocity;
            }

            projectile.position = player.RotatedRelativePoint(player.MountedCenter, true) - projectile.Size / 2f;
            projectile.rotation = projectile.velocity.ToRotation() + num;
            projectile.spriteDirection = projectile.direction;
            projectile.timeLeft = 2;
            player.ChangeDir(projectile.direction);
            player.heldProj = projectile.whoAmI;
            player.itemTime = 2;
            player.itemAnimation = 2;
            player.itemRotation = (float)Math.Atan2((double)(projectile.velocity.Y * (float)projectile.direction), (double)(projectile.velocity.X * (float)projectile.direction));
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(red, greenAndBlue, greenAndBlue, projectile.alpha);
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(ModContent.BuffType<BrimstoneFlames>(), 180);
        }
    }
}
