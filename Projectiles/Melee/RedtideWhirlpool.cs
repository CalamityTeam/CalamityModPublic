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
    public class RedtideWhirlpool : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Melee";
        public Player Owner => Main.player[Projectile.owner];

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 8;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 60;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 30;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 15;
        }

        public override void AI()
        {
            Projectile.velocity *= 0.93f;
            Projectile.rotation += MathHelper.PiOver4 / 2.4f * Math.Sign(Projectile.velocity.X);

            int dustCount = Main.rand.Next(4);
            float offset = Main.rand.NextFloat(MathHelper.TwoPi);
            for (int i = 0; i < dustCount; i++)
            {
                float angle = i / (float)dustCount * MathHelper.TwoPi + offset;
                Vector2 dustPos = Projectile.Center + angle.ToRotationVector2() * 46f;
                Dust dust = Dust.NewDustPerfect(dustPos, DustID.BubbleBurst_Blue, (angle - MathHelper.PiOver2 * Math.Sign(Projectile.velocity.X)).ToRotationVector2() * 8f + Projectile.velocity, Scale: Main.rand.NextFloat(1.6f, 3f));
                dust.noGravity = true;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = Terraria.GameContent.TextureAssets.Projectile[Type].Value;
            SpriteEffects flip = Math.Sign(Projectile.velocity.X) < 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, lightColor * 0.3f, Projectile.rotation * 1.2f, texture.Size() * 0.5f, 1.2f, flip);
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, lightColor * 0.4f, Projectile.rotation, texture.Size() * 0.5f, 1.6f, flip);

            // Custom afterimage
            for (int i = 1; i < Projectile.oldPos.Length; i++)
            {
                float afterimageRot = Projectile.oldRot[i];
                Vector2 drawPos = Projectile.oldPos[i] + texture.Size() * 0.5f - Main.screenPosition;
                float intensity = MathHelper.Lerp(0.01f, 0.1f, 1f - i / (float)Projectile.oldPos.Length);
                
                Main.EntitySpriteDraw(texture, drawPos, null, lightColor * intensity, afterimageRot, texture.Size() * 0.5f, 1.6f, flip);
            }
            return false;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) => CalamityUtils.CircularHitboxCollision(Projectile.Center, 56f, targetHitbox);

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => target.AddBuff(ModContent.BuffType<RiptideDebuff>(), 120);
        public override void OnHitPlayer(Player target, Player.HurtInfo info) => target.AddBuff(ModContent.BuffType<RiptideDebuff>(), 120);
        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.NPCDeath19, Projectile.position);

            int dustCount = 36;
            for (int i = 0; i < dustCount; i++)
            {
                Vector2 offset = Vector2.Normalize(Projectile.velocity) * new Vector2(Projectile.width / 2f, Projectile.height) * 0.75f;
                offset = offset.RotatedBy(((i - (dustCount / 2 - 1)) * MathHelper.TwoPi / (float)dustCount), default) + Projectile.Center;
                Vector2 dustDirection = offset - Projectile.Center;
                Dust dust = Dust.NewDustPerfect(offset + dustDirection, DustID.DungeonWater, Vector2.Zero, 100, default, 1.4f);
                dust.noGravity = true;
                dust.noLight = true;
                dust.velocity = dustDirection * 0.5f;
            }
        }
    }
}
