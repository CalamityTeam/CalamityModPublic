using System;
using CalamityMod.Buffs.DamageOverTime;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Melee
{
    public class TyphonsGreedBubble : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Melee";
        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 4;
            ProjectileID.Sets.CultistIsResistantTo[Type] = true;
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
        public override void ModifyDamageHitbox(ref Rectangle hitbox)
        {
            Player Owner = Main.player[Projectile.owner];
            float scale = Projectile.scale;
            Vector2 newSize = new Point(hitbox.Width, hitbox.Height).ToVector2() * scale;
            hitbox = new Rectangle(hitbox.X - (int)((newSize.X - hitbox.Width) / 2f), hitbox.Y - (int)((newSize.Y - hitbox.Height) / 2f), (int)newSize.X, (int)newSize.Y);
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

            if (Projectile.timeLeft < 210) // making it not home right away so the projectiles get closer to the character first, to make it look more natural and overall better.
            {
                CalamityUtils.HomeInOnNPC(Projectile, true, 450f, 20f, 20f);
            }

            float inertia = 50f * Projectile.ai[1]; //100
            float speed = 10f * Projectile.ai[1]; //5
            if (Math.Abs(Projectile.velocity.X) > 0.2f)
            {
                Projectile.spriteDirection = -Projectile.direction;
            }
            Projectile.rotation = Projectile.velocity.ToRotation();
            Lighting.AddLight(Projectile.Center, 0f, 0.1f, 0.7f);

            Player player = Main.player[Projectile.owner];
            if (player.active && !player.dead)
            {
                if (Projectile.Distance(player.Center) > 40f)
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
            Texture2D texture2D13 = Terraria.GameContent.TextureAssets.Projectile[Type].Value;
            int framing = Terraria.GameContent.TextureAssets.Projectile[Type].Value.Height / Main.projFrames[Type];
            int y6 = framing * Projectile.frame;
            Main.spriteBatch.Draw(texture2D13,
                Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY),
                new Microsoft.Xna.Framework.Rectangle?(new Rectangle(0, y6, texture2D13.Width, framing)),
                Projectile.GetAlpha(lightColor), Projectile.rotation, new Vector2((float)texture2D13.Width / 2f, (float)framing / 2f), Projectile.scale, SpriteEffects.None, 0);
            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<CrushDepth>(), 180);
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item21, Projectile.position);
            Projectile.position = Projectile.Center;
            Projectile.width *= (int)(2 * Projectile.scale); Projectile.height *= (int)(2 * Projectile.scale);
            Projectile.position.X -= (float)(Projectile.width / (int)(2 * Projectile.scale));
            Projectile.position.Y -= (float)(Projectile.height / (int)(2 * Projectile.scale));
            for (int i = 0; i < 2; i++)
            {
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Water, 0f, 0f, 100, new Color(0, 255, 255), 1.5f);
            }
            for (int j = 0; j < 6; j++)
            {
                int bubblyDust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.RedsWingsRun, 0f, 0f, 0, new Color(0, 255, 255), 2.5f);
                Main.dust[bubblyDust].noGravity = true;
                Main.dust[bubblyDust].velocity *= 3f;
                bubblyDust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.RedsWingsRun, 0f, 0f, 100, new Color(0, 255, 255), 1.5f);
                Main.dust[bubblyDust].velocity *= 2f;
                Main.dust[bubblyDust].noGravity = true;
            }
            Projectile.maxPenetrate = -1;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
            Projectile.damage /= 1;
            Projectile.Damage();
        }
    }
}
