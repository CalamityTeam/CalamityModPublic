using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Dusts;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Typeless
{
    public class GodSlayerBlaze : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Typeless";
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetDefaults()
        {
            Projectile.width = 250;
            Projectile.height = 250;
            Projectile.friendly = true;
            Projectile.ignoreWater = false;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 60;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 5;
        }

        public override void AI()
        {
            Lighting.AddLight(Projectile.Center, 0.5f, 0f, 0.75f);
            float projTimer = 25f;
            if (Projectile.ai[0] > 180f)
            {
                projTimer -= (Projectile.ai[0] - 180f) / 2f;
            }
            if (projTimer <= 0f)
            {
                projTimer = 0f;
                Projectile.Kill();
            }
            projTimer *= 0.7f;
            Projectile.ai[0] += 4f;
            int timerCounter = 0;
            float scale = 0.7f;
            int dustType = Main.rand.NextBool() ? ModContent.DustType<AstralOrange>() : ModContent.DustType<AstralBlue>();
            if (Projectile.ai[1] == 0f)
            {
                scale = 1.5f;
                dustType = 173;
            }
            while ((float)timerCounter < projTimer)
            {
                float rand1 = (float)Main.rand.Next(-30, 31);
                float rand2 = (float)Main.rand.Next(-30, 31);
                float rand3 = (float)Main.rand.Next(9, 27);
                float randAdjust = (float)Math.Sqrt((double)(rand1 * rand1 + rand2 * rand2));
                randAdjust = rand3 / randAdjust;
                rand1 *= randAdjust;
                rand2 *= randAdjust;
                int dusty = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, dustType, 0f, 0f, 100, default, scale);
                Dust dust = Main.dust[dusty];
                if (dustType != 173)
                    dust.color = new Color(255, 255, 255, 0);
                dust.noGravity = true;
                dust.position.X = Projectile.Center.X;
                dust.position.Y = Projectile.Center.Y;
                dust.position.X += (float)Main.rand.Next(-10, 11);
                dust.position.Y += (float)Main.rand.Next(-10, 11);
                dust.velocity.X = rand1;
                dust.velocity.Y = rand2;
                timerCounter++;
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Projectile.ai[1] == 1f)
                target.AddBuff(ModContent.BuffType<AstralInfectionDebuff>(), 300);
            else
                target.AddBuff(ModContent.BuffType<GodSlayerInferno>(), 300);
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            if (Projectile.ai[1] == 1f)
                target.AddBuff(ModContent.BuffType<AstralInfectionDebuff>(), 300);
            else
                target.AddBuff(ModContent.BuffType<GodSlayerInferno>(), 300);
        }
    }
}
