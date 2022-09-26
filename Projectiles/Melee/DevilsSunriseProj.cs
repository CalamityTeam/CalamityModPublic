using CalamityMod.Buffs.DamageOverTime;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.Projectiles.Melee
{
    public class DevilsSunriseProj : ModProjectile
    {
        private int red;
        private const int greenAndBlue = 100;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Devil's Sunrise");
            Main.projFrames[Projectile.type] = 28;
        }

        public override void SetDefaults()
        {
            Projectile.width = 148;
            Projectile.height = 68;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.DamageType = TrueMeleeNoSpeedDamageClass.Instance;
            Projectile.ownerHitCheck = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 5;
        }

        public override void AI()
        {
            if (Projectile.ai[1] < 255f)
                Projectile.ai[1] += 1f;

            if (Projectile.ai[1] == 255f)
                Projectile.damage = 2 * Projectile.originalDamage;

            red = 64 + (int)(Projectile.ai[1] * 0.75f);
            if (red > 255)
                red = 255;

            Player player = Main.player[Projectile.owner];
            float num = 0f;
            Vector2 vector = player.RotatedRelativePoint(player.MountedCenter, true);

            if (Projectile.spriteDirection == -1)
                num = 3.14159274f;

            if (++Projectile.frame >= Main.projFrames[Projectile.type])
                Projectile.frame = 0;

            Projectile.soundDelay--;
            if (Projectile.soundDelay <= 0)
            {
                SoundEngine.PlaySound(SoundID.Item15, Projectile.Center);
                Projectile.soundDelay = 24;
            }

            if (Main.myPlayer == Projectile.owner)
            {
                if (player.channel && !player.noItems && !player.CCed)
                {
                    float scaleFactor6 = 1f;

                    if (player.ActiveItem().shoot == Projectile.type)
                        scaleFactor6 = player.ActiveItem().shootSpeed * Projectile.scale;

                    Vector2 vector13 = Main.MouseWorld - vector;
                    vector13.Normalize();
                    if (vector13.HasNaNs())
                        vector13 = Vector2.UnitX * (float)player.direction;

                    vector13 *= scaleFactor6;
                    if (vector13.X != Projectile.velocity.X || vector13.Y != Projectile.velocity.Y)
                        Projectile.netUpdate = true;

                    Projectile.velocity = vector13;
                }
                else
                    Projectile.Kill();
            }

            Vector2 vector14 = Projectile.Center + Projectile.velocity * 3f;
            Lighting.AddLight(vector14, (float)((double)red * 0.001), 0.1f, 0.1f);

            if (Main.rand.NextBool(3))
            {
                int dust = Dust.NewDust(vector14 - Projectile.Size / 2f, Projectile.width, Projectile.height, 66, Projectile.velocity.X, Projectile.velocity.Y, 100, new Color(red, greenAndBlue, greenAndBlue), 1f);
                Main.dust[dust].noGravity = true;
                Main.dust[dust].position -= Projectile.velocity;
            }

            Projectile.position = player.RotatedRelativePoint(player.MountedCenter, true) - Projectile.Size / 2f;
            Projectile.rotation = Projectile.velocity.ToRotation() + num;
            Projectile.spriteDirection = Projectile.direction;
            Projectile.timeLeft = 2;
            player.ChangeDir(Projectile.direction);
            player.heldProj = Projectile.whoAmI;
            player.itemTime = 2;
            player.itemAnimation = 2;
            player.itemRotation = (float)Math.Atan2((double)(Projectile.velocity.Y * (float)Projectile.direction), (double)(Projectile.velocity.X * (float)Projectile.direction));
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(red, greenAndBlue, greenAndBlue, Projectile.alpha);
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(ModContent.BuffType<BrimstoneFlames>(), 180);
        }
    }
}
