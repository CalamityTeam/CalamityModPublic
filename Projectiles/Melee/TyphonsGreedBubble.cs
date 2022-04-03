using CalamityMod.Buffs.DamageOverTime;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
namespace CalamityMod.Projectiles.Melee
{
    public class TyphonsGreedBubble : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Typhon's Greed");
            Main.projFrames[Projectile.type] = 4;
        }

        public override void SetDefaults()
        {
            Projectile.width = 32;
            Projectile.height = 32;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 300;
        }

        public override void AI()
        {
            Projectile.frameCounter++;
            if (Projectile.frameCounter > 4)
            {
                Projectile.frame++;
                Projectile.frameCounter = 0;
            }
            if (Projectile.frame > 3)
            {
                Projectile.frame = 0;
            }
            float inertia = 50f * Projectile.ai[1]; //100
            float speed = 10f * Projectile.ai[1]; //5
            float num954 = 40f;
            if (Math.Abs(Projectile.velocity.X) > 0.2f)
            {
                Projectile.spriteDirection = -Projectile.direction;
            }
            Projectile.rotation = Projectile.velocity.ToRotation();
            Lighting.AddLight(Projectile.Center, 0f, 0.1f, 0.7f);

            Player player = Main.player[Projectile.owner];
            if (player.active && !player.dead)
            {
                if (Projectile.Distance(player.Center) > num954)
                {
                    Vector2 moveDirection = Projectile.SafeDirectionTo(player.Center, Vector2.UnitY);
                    Projectile.velocity = (Projectile.velocity * (inertia - 1f) + moveDirection * speed) / inertia;
                }
            }
            else
            {
                if (Projectile.timeLeft > 30)
                {
                    Projectile.timeLeft = 30;
                }
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture2D13 = ModContent.Request<Texture2D>(Texture).Value;
            int num214 = ModContent.Request<Texture2D>(Texture).Value.Height / Main.projFrames[Projectile.type];
            int y6 = num214 * Projectile.frame;
            Main.spriteBatch.Draw(texture2D13,
                Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY),
                new Microsoft.Xna.Framework.Rectangle?(new Rectangle(0, y6, texture2D13.Width, num214)),
                Projectile.GetAlpha(lightColor), Projectile.rotation, new Vector2((float)texture2D13.Width / 2f, (float)num214 / 2f), Projectile.scale, SpriteEffects.None, 0f);
            return false;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(ModContent.BuffType<CrushDepth>(), 180);
        }

        public override void Kill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item21, Projectile.position);
            Projectile.position = Projectile.Center;
            Projectile.width = Projectile.height = 64;
            Projectile.position.X -= (float)(Projectile.width / 2);
            Projectile.position.Y -= (float)(Projectile.height / 2);
            for (int num193 = 0; num193 < 2; num193++)
            {
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 33, 0f, 0f, 100, new Color(0, 255, 255), 1.5f);
            }
            for (int num194 = 0; num194 < 6; num194++)
            {
                int num195 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 186, 0f, 0f, 0, new Color(0, 255, 255), 2.5f);
                Main.dust[num195].noGravity = true;
                Main.dust[num195].velocity *= 3f;
                num195 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 186, 0f, 0f, 100, new Color(0, 255, 255), 1.5f);
                Main.dust[num195].velocity *= 2f;
                Main.dust[num195].noGravity = true;
            }
            Projectile.maxPenetrate = -1;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
            Projectile.damage /= 2;
            Projectile.Damage();
        }
    }
}
