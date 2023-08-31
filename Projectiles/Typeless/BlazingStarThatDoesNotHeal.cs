using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.NPCs.Providence;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using CalamityMod.Dusts;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.Projectiles.Typeless
{
    public class BlazingStarThatDoesNotHeal : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Typeless";
        public override string Texture => "CalamityMod/Projectiles/StarProj";

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 30;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.alpha = 255;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 200;
        }

        public override void AI()
        {
            if (Projectile.timeLeft % 4 == 0) //only once per 4 frames
                Lighting.AddLight(Projectile.Center, 0.45f, 0.35f, 0f);

            if (Projectile.ai[0] < 240f)
            {
                Projectile.ai[0] += 1f;

                if (Projectile.timeLeft < 160)
                    Projectile.timeLeft = 160;
            }
            
            if (Projectile.velocity.Length() < 16f)
                Projectile.velocity *= 1.01f;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            float lerpMult = Utils.GetLerpValue(15f, 30f, Projectile.timeLeft, clamped: true) * Utils.GetLerpValue(240f, 200f, Projectile.timeLeft, clamped: true) * (1f + 0.2f * (float)Math.Cos(Main.GlobalTimeWrappedHourly % 30f / 0.5f * (MathHelper.Pi * 2f) * 3f)) * 0.8f;

            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Vector2 drawPos = Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY);
            Color baseColor = new Color(250, 100, 100, 255);
            baseColor *= 0.5f;
            baseColor.A = 0;
            Color colorA = baseColor;
            Color colorB = baseColor * 0.5f;
            colorA *= lerpMult;
            colorB *= lerpMult;
            Vector2 origin = texture.Size() / 2f;
            Vector2 scale = new Vector2(0.5f, 1.5f) * lerpMult;

            SpriteEffects spriteEffects = SpriteEffects.None;
            if (Projectile.spriteDirection == -1)
                spriteEffects = SpriteEffects.FlipHorizontally;

            float upRight = MathHelper.PiOver4;
            float up = MathHelper.PiOver2;
            float upLeft = 3f * MathHelper.PiOver4;
            float left = MathHelper.Pi;
            Main.EntitySpriteDraw(texture, drawPos, null, colorA, upLeft, origin, scale, spriteEffects, 0);
            Main.EntitySpriteDraw(texture, drawPos, null, colorA, upRight, origin, scale, spriteEffects, 0);
            Main.EntitySpriteDraw(texture, drawPos, null, colorB, upLeft, origin, scale * 0.6f, spriteEffects, 0);
            Main.EntitySpriteDraw(texture, drawPos, null, colorB, upRight, origin, scale * 0.6f, spriteEffects, 0);
            Main.EntitySpriteDraw(texture, drawPos, null, colorA, up, origin, scale * 0.6f, spriteEffects, 0);
            Main.EntitySpriteDraw(texture, drawPos, null, colorA, left, origin, scale * 0.6f, spriteEffects, 0);
            Main.EntitySpriteDraw(texture, drawPos, null, colorB, up, origin, scale * 0.36f, spriteEffects, 0);
            Main.EntitySpriteDraw(texture, drawPos, null, colorB, left, origin, scale * 0.36f, spriteEffects, 0);

            return false;
        }

        public override void Kill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item14, Projectile.Center);
            Projectile.ExpandHitboxBy(50);
            int dustType = (int)CalamityDusts.ProfanedFire;
            for (int d = 0; d < 5; d++)
            {
                int holy = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, dustType, 0f, 0f, 100, default, 2f);
                Main.dust[holy].velocity *= 3f;
                Main.dust[holy].noGravity = true;
                if (Main.rand.NextBool(2))
                {
                    Main.dust[holy].scale = 0.5f;
                    Main.dust[holy].fadeIn = 1f + Main.rand.Next(10) * 0.1f;
                }
            }
            for (int d = 0; d < 8; d++)
            {
                int fire = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, dustType, 0f, 0f, 100, default, 3f);
                Main.dust[fire].noGravity = true;
                Main.dust[fire].velocity *= 5f;
                fire = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, dustType, 0f, 0f, 100, default, 2f);
                Main.dust[fire].velocity *= 2f;
                Main.dust[fire].noGravity = true;
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<HolyFlames>(), 60 * 5);

            Projectile.Kill();
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            // If the player is dodging, don't apply debuffs
            if ((info.Damage <= 0 && Projectile.maxPenetrate < (int)Providence.BossMode.Red) || target.creativeGodMode)
                return;

            target.AddBuff(ModContent.BuffType<HolyFlames>(), 60 * 5, false);

            Projectile.Kill();
        }
    }
}
