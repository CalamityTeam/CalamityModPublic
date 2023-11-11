using CalamityMod.Buffs.DamageOverTime;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Melee
{
    public class SpatialSpear2 : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Melee";
        public override void SetDefaults()
        {
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.tileCollide = false;
            Projectile.penetrate = 2;
            Projectile.timeLeft = 60;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 6;
        }

        public override void AI()
        {
            Lighting.AddLight(Projectile.Center, 0.05f, 0.05f, 1f);
            Projectile.rotation = (float)Math.Atan2(Projectile.velocity.Y, Projectile.velocity.X) + MathHelper.PiOver4;
            if (Projectile.localAI[1] == 0f)
            {
                Projectile.scale -= 0.01f;
                Projectile.alpha += 15;
                if (Projectile.alpha >= 125)
                {
                    Projectile.alpha = 130;
                    Projectile.localAI[1] = 1f;
                }
            }
            else if (Projectile.localAI[1] == 1f)
            {
                Projectile.scale += 0.01f;
                Projectile.alpha -= 15;
                if (Projectile.alpha <= 0)
                {
                    Projectile.alpha = 0;
                    Projectile.localAI[1] = 0f;
                }
            }
            if (Main.rand.NextBool(8))
            {
                Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, 56, Projectile.velocity.X * 0.1f, Projectile.velocity.Y * 0.1f);
            }
            Projectile.localAI[0] += 1f;
            if (Projectile.localAI[0] >= 30f)
            {
                Projectile.localAI[0] = 0f;
                int numProj = 2;
                float rotation = MathHelper.ToRadians(10);
                if (Projectile.owner == Main.myPlayer)
                {
                    for (int i = 0; i < numProj + 1; i++)
                    {
                        Vector2 perturbedSpeed = Projectile.velocity.RotatedBy(MathHelper.Lerp(-rotation, rotation, i / (numProj - 1)));
                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, perturbedSpeed, ModContent.ProjectileType<SpatialSpear3>(), (int)(Projectile.damage * 0.5), Projectile.knockBack * 0.5f, Projectile.owner, 0f, 0f);
                    }
                }
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (Projectile.timeLeft > 55)
                return false;

            Texture2D tex = ModContent.Request<Texture2D>(Texture).Value;
            Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(lightColor), Projectile.rotation, tex.Size() / 2f, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<ElementalMix>(), 30);
        }
        public override void OnKill(int timeLeft)
        {
            for (int i = 4; i < 12; i++)
            {
                float projOldX = Projectile.oldVelocity.X * (30f / i);
                float projOldY = Projectile.oldVelocity.Y * (30f / i);
                int spatial = Dust.NewDust(new Vector2(Projectile.oldPosition.X - projOldX, Projectile.oldPosition.Y - projOldY), 8, 8, 56, Projectile.oldVelocity.X, Projectile.oldVelocity.Y, 100, default, 1.8f);
                Main.dust[spatial].noGravity = true;
                Dust dust = Main.dust[spatial];
                dust.velocity *= 0.5f;
                spatial = Dust.NewDust(new Vector2(Projectile.oldPosition.X - projOldX, Projectile.oldPosition.Y - projOldY), 8, 8, 56, Projectile.oldVelocity.X, Projectile.oldVelocity.Y, 100, default, 1.4f);
                dust = Main.dust[spatial];
                dust.velocity *= 0.05f;
            }
        }
    }
}
