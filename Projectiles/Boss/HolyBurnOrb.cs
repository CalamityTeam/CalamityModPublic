using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Events;
using CalamityMod.Dusts;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.Projectiles.Boss
{
    public class HolyBurnOrb : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/StarProj";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Holy Orb");
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 30;
            Projectile.hostile = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.alpha = 255;
            cooldownSlot = 1;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 200;
            Projectile.Calamity().canBreakPlayerDefense = true;
        }

        public override void AI()
        {
            if (Projectile.ai[0] == 0f && (CalamityWorld.malice || BossRushEvent.BossRushActive))
                Projectile.velocity *= 1.25f;

            if (Projectile.ai[0] < 240f)
            {
                Projectile.ai[0] += 1f;

                if (Projectile.timeLeft < 160)
                    Projectile.timeLeft = 160;
            }

            if (Projectile.velocity.Length() < 16f)
                Projectile.velocity *= 1.01f;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            float lerpMult = Utils.InverseLerp(15f, 30f, Projectile.timeLeft, clamped: true) * Utils.InverseLerp(240f, 200f, Projectile.timeLeft, clamped: true) * (1f + 0.2f * (float)Math.Cos(Main.GlobalTime % 30f / 0.5f * (MathHelper.Pi * 2f) * 3f)) * 0.8f;

            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Vector2 drawPos = Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY);
            Color baseColor = (Main.dayTime && !CalamityWorld.malice) ? new Color(255, 200, 100, 255) : new Color(100, 200, 255, 255);
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
            spriteBatch.Draw(texture, drawPos, null, colorA, upLeft, origin, scale, spriteEffects, 0);
            spriteBatch.Draw(texture, drawPos, null, colorA, upRight, origin, scale, spriteEffects, 0);
            spriteBatch.Draw(texture, drawPos, null, colorB, upLeft, origin, scale * 0.6f, spriteEffects, 0);
            spriteBatch.Draw(texture, drawPos, null, colorB, upRight, origin, scale * 0.6f, spriteEffects, 0);
            spriteBatch.Draw(texture, drawPos, null, colorA, up, origin, scale * 0.6f, spriteEffects, 0);
            spriteBatch.Draw(texture, drawPos, null, colorA, left, origin, scale * 0.6f, spriteEffects, 0);
            spriteBatch.Draw(texture, drawPos, null, colorB, up, origin, scale * 0.36f, spriteEffects, 0);
            spriteBatch.Draw(texture, drawPos, null, colorB, left, origin, scale * 0.36f, spriteEffects, 0);

            return false;
        }

        public override void Kill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item14, Projectile.Center);
            CalamityGlobalProjectile.ExpandHitboxBy(Projectile, 50);
            int dustType = (Main.dayTime && !CalamityWorld.malice) ? (int)CalamityDusts.ProfanedFire : (int)CalamityDusts.Nightwither;
            for (int d = 0; d < 5; d++)
            {
                int holy = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, dustType, 0f, 0f, 100, default, 2f);
                Main.dust[holy].velocity *= 3f;
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
            }
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            int buffType = (Main.dayTime && !CalamityWorld.malice) ? ModContent.BuffType<HolyFlames>() : ModContent.BuffType<Nightwither>();
            target.AddBuff(buffType, 180);
            Projectile.Kill();
        }

        public override void ModifyHitPlayer(Player target, ref int damage, ref bool crit)
        {
            target.Calamity().lastProjectileHit = projectile;
        }
    }
}
