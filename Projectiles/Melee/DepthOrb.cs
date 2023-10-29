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
    public class DepthOrb : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Melee";
        public override void SetDefaults()
        {
            Projectile.width = 20;
            Projectile.height = 20;
            Projectile.aiStyle = ProjAIStyleID.Beam;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.ignoreWater = true;
            Projectile.penetrate = 1;
            Projectile.extraUpdates = 1;
            Projectile.timeLeft = 75;
        }

        public override void AI()
        {
            Lighting.AddLight(Projectile.Center, 0f, 0f, 0.5f);
            int waterDust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 33, 0f, 0f, 100, default, 0.6f);
            Main.dust[waterDust].noGravity = true;
            Main.dust[waterDust].velocity *= 0.5f;
            Main.dust[waterDust].velocity += Projectile.velocity * 0.1f;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (Projectile.timeLeft > 70)
                return false;

            Texture2D tex = ModContent.Request<Texture2D>(Texture).Value;
            Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(lightColor), Projectile.rotation, tex.Size() / 2f, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(50, 50, 255, Projectile.alpha);
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item10, Projectile.position);
            Projectile.position = Projectile.Center;
            Projectile.width = Projectile.height = 64;
            Projectile.Center = Projectile.position;
            for (int dustIndex = 0; dustIndex < 30; dustIndex++)
            {
                float rando1 = (float)Main.rand.Next(-10, 11);
                float rando2 = (float)Main.rand.Next(-10, 11);
                float rando3 = (float)Main.rand.Next(3, 9);
                float randoAdjust = (float)Math.Sqrt((double)(rando1 * rando1 + rando2 * rando2));
                randoAdjust = rando3 / randoAdjust;
                rando1 *= randoAdjust;
                rando2 *= randoAdjust;
                int killWaterDust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 33, 0f, 0f, 100, default, 1.2f);
                Dust dust = Main.dust[killWaterDust];
                dust.noGravity = true;
                dust.position.X = Projectile.Center.X;
                dust.position.Y = Projectile.Center.Y;
                dust.position.X += (float)Main.rand.Next(-10, 11);
                dust.position.Y += (float)Main.rand.Next(-10, 11);
                dust.velocity.X = rando1;
                dust.velocity.Y = rando2;
            }
            Projectile.maxPenetrate = -1;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
            Projectile.damage /= 2;
            Projectile.Damage();
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<CrushDepth>(), 120);
        }
    }
}
