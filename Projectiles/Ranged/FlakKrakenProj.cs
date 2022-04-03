using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Ranged
{
    public class FlakKrakenProj : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Kraken");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 10;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.width = 54;
            Projectile.height = 54;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.scale = 0.002f;
            Projectile.timeLeft = 36000;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.ignoreWater = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 4;
        }

        public override void AI()
        {
            if (Projectile.type != ModContent.ProjectileType<FlakKrakenProj>() ||
                !Main.projectile[(int)Projectile.ai[1]].active ||
                Main.projectile[(int)Projectile.ai[1]].type != ModContent.ProjectileType<FlakKrakenGun>())
            {
                Projectile.Kill();
                return;
            }


            // This code uses player-specific fields (such as the mouse), and does not need to be run for anyone
            // other than its owner.
            if (Main.myPlayer != Projectile.owner)
                return;

            Projectile.rotation += 0.2f;
            if (Projectile.localAI[0] < 1f)
            {
                Projectile.localAI[0] += 0.002f;
                Projectile.scale += 0.002f;
                Projectile.width = Projectile.height = (int)(50f * Projectile.scale);
            }
            else
            {
                Projectile.width = Projectile.height = 50;
            }
            Player player = Main.player[Projectile.owner];
            Vector2 vector2 = player.RotatedRelativePoint(player.MountedCenter, true);
            float num78 = (float)Main.mouseX + Main.screenPosition.X - vector2.X;
            float num79 = (float)Main.mouseY + Main.screenPosition.Y - vector2.Y;
            if (player.gravDir == -1f)
            {
                num79 = Main.screenPosition.Y + (float)Main.screenHeight - (float)Main.mouseY - vector2.Y;
            }
            if ((float.IsNaN(num78) && float.IsNaN(num79)) || (num78 == 0f && num79 == 0f))
            {
                num78 = (float)player.direction;
                num79 = 0f;
            }
            vector2 += new Vector2(num78, num79);
            float speed = 30f;
            float speedScale = 3f;
            Vector2 vectorPos = Projectile.Center;
            if (Vector2.Distance(vector2, vectorPos) < 90f)
            {
                speed = 10f;
                speedScale = 1f;
            }
            if (Vector2.Distance(vector2, vectorPos) < 30f)
            {
                speed = 3f;
                speedScale = 0.3f;
            }
            if (Vector2.Distance(vector2, vectorPos) < 10f)
            {
                speed = 1f;
                speedScale = 0.1f;
            }
            float num678 = vector2.X - vectorPos.X;
            float num679 = vector2.Y - vectorPos.Y;
            float num680 = (float)Math.Sqrt((double)(num678 * num678 + num679 * num679));
            num680 = speed / num680;
            num678 *= num680;
            num679 *= num680;
            if (Projectile.velocity.X < num678)
            {
                Projectile.velocity.X = Projectile.velocity.X + speedScale;
                if (Projectile.velocity.X < 0f && num678 > 0f)
                {
                    Projectile.velocity.X = Projectile.velocity.X + speedScale;
                }
            }
            else if (Projectile.velocity.X > num678)
            {
                Projectile.velocity.X = Projectile.velocity.X - speedScale;
                if (Projectile.velocity.X > 0f && num678 < 0f)
                {
                    Projectile.velocity.X = Projectile.velocity.X - speedScale;
                }
            }
            if (Projectile.velocity.Y < num679)
            {
                Projectile.velocity.Y = Projectile.velocity.Y + speedScale;
                if (Projectile.velocity.Y < 0f && num679 > 0f)
                {
                    Projectile.velocity.Y = Projectile.velocity.Y + speedScale;
                }
            }
            else if (Projectile.velocity.Y > num679)
            {
                Projectile.velocity.Y = Projectile.velocity.Y - speedScale;
                if (Projectile.velocity.Y > 0f && num679 < 0f)
                {
                    Projectile.velocity.Y = Projectile.velocity.Y - speedScale;
                }
            }
        }

        public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            damage = (int)(damage * Projectile.localAI[0]);
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(255, 200, 50, Projectile.alpha);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], lightColor, 1);
            return false;
        }
    }
}
