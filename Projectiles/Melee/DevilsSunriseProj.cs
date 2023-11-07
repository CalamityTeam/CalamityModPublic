using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Items.Weapons.Melee;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Melee
{
    public class DevilsSunriseProj : ModProjectile
    {
        public override LocalizedText DisplayName => CalamityUtils.GetItemName<DevilsSunrise>();
        private int red;
        private const int greenAndBlue = 100;

        public override void SetStaticDefaults()
        {
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
            float pi = 0f;
            Vector2 playerRotate = player.RotatedRelativePoint(player.MountedCenter, true);

            if (Projectile.spriteDirection == -1)
                pi = 3.14159274f;

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

                    Vector2 slashDirection = Main.MouseWorld - playerRotate;
                    slashDirection.Normalize();
                    if (slashDirection.HasNaNs())
                        slashDirection = Vector2.UnitX * (float)player.direction;

                    slashDirection *= scaleFactor6;
                    if (slashDirection.X != Projectile.velocity.X || slashDirection.Y != Projectile.velocity.Y)
                        Projectile.netUpdate = true;

                    Projectile.velocity = slashDirection;
                }
                else
                    Projectile.Kill();
            }

            Vector2 dustSpawn = Projectile.Center + Projectile.velocity * 3f;
            Lighting.AddLight(dustSpawn, (float)((double)red * 0.001), 0.1f, 0.1f);

            if (Main.rand.NextBool(3))
            {
                int dust = Dust.NewDust(dustSpawn - Projectile.Size / 2f, Projectile.width, Projectile.height, 66, Projectile.velocity.X, Projectile.velocity.Y, 100, new Color(red, greenAndBlue, greenAndBlue), 1f);
                Main.dust[dust].noGravity = true;
                Main.dust[dust].position -= Projectile.velocity;
            }

            Projectile.position = player.RotatedRelativePoint(player.MountedCenter, true) - Projectile.Size / 2f;
            Projectile.rotation = Projectile.velocity.ToRotation() + pi;
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

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<BrimstoneFlames>(), 180);
        }
    }
}
