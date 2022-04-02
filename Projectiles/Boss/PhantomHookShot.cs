using CalamityMod.Buffs.DamageOverTime;
using Microsoft.Xna.Framework;
using System;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Boss
{
    public class PhantomHookShot : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Phantom Hook Shot");
        }

        public override void SetDefaults()
        {
            projectile.width = 14;
            projectile.height = 14;
            projectile.hostile = true;
            projectile.alpha = 255;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.penetrate = -1;
            projectile.timeLeft = 600;
            cooldownSlot = 1;
            projectile.Calamity().affectedByMaliceModeVelocityMultiplier = true;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(projectile.localAI[0]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            projectile.localAI[0] = reader.ReadSingle();
        }

        public override void AI()
        {
            if (projectile.ai[1] == 0f)
            {
                projectile.ai[1] = 1f;
                Main.PlaySound(SoundID.Item20, projectile.position);
            }

            projectile.rotation = (float)Math.Atan2(projectile.velocity.Y, projectile.velocity.X) + MathHelper.PiOver2;

            projectile.localAI[0] += 1f;
            if (projectile.localAI[0] == 6f)
            {
                for (int num151 = 0; num151 < 40; num151++)
                {
                    int num152 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 180, 0f, 0f, 100, default, 1f);
                    Main.dust[num152].velocity *= 3f;
                    Main.dust[num152].velocity += projectile.velocity * 0.75f;
                    Main.dust[num152].scale *= 1.2f;
                    Main.dust[num152].noGravity = true;
                }
            }

            if (projectile.localAI[0] > 9f)
            {
                projectile.alpha -= 5;
                if (projectile.alpha < 30)
                    projectile.alpha = 30;
            }
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            target.AddBuff(ModContent.BuffType<Nightwither>(), 120);
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(100, 250, 250, projectile.alpha);
        }

        public override void ModifyHitPlayer(Player target, ref int damage, ref bool crit)
        {
            target.Calamity().lastProjectileHit = projectile;
        }
    }
}
