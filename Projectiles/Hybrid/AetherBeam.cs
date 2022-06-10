using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Hybrid
{
    public class AetherBeam : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/LaserProj";

        private bool split = true;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Beam");
        }

        public override void SetDefaults()
        {
            Projectile.width = 5;
            Projectile.height = 5;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.alpha = 255;
            Projectile.penetrate = 1;
            Projectile.extraUpdates = 5;
            Projectile.timeLeft = 300;
        }

        public override void AI()
        {
            if (Projectile.ai[0] == 1f)
            {
                // projectile.magic = false /* tModPorter - this is redundant, for more info see https://github.com/tModLoader/tModLoader/wiki/Update-Migration-Guide#damage-classes */ ;
                Projectile.DamageType = DamageClass.Ranged;
            }

            if (Projectile.ai[1] == 1f)
            {
                split = false;
                Projectile.tileCollide = false;
                Projectile.ai[1] = 0f;
            }

            Projectile.damage += Projectile.Calamity().defDamage / 200;

            if (Projectile.alpha > 0)
                Projectile.alpha -= 25;
            if (Projectile.alpha < 0)
                Projectile.alpha = 0;

            Lighting.AddLight((int)Projectile.Center.X / 16, (int)Projectile.Center.Y / 16, 1f, 0f, 0.7f);

            float num55 = 100f;
            float num56 = 2f;
            if (Projectile.ai[1] == 0f)
            {
                Projectile.localAI[0] += num56;
                if (Projectile.localAI[0] > num55)
                    Projectile.localAI[0] = num55;
            }
            else
            {
                Projectile.localAI[0] -= num56;
                if (Projectile.localAI[0] <= 0f)
                    Projectile.Kill();
            }
        }

        public override Color? GetAlpha(Color lightColor) => new Color(250, 50, 200, 0);

        public override bool PreDraw(ref Color lightColor) => Projectile.DrawBeam(100f, 2f, lightColor);

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (split)
            {
                float random = Main.rand.Next(30, 90);
                float spread = random * 0.0174f;
                double startAngle = Math.Atan2(Projectile.velocity.X, Projectile.velocity.Y) - spread / 2;
                double deltaAngle = spread / 8f;
                double offsetAngle;
                int i;
                if (Projectile.owner == Main.myPlayer)
                {
                    for (i = 0; i < 4; i++)
                    {
                        offsetAngle = startAngle + deltaAngle * (i + i * i) / 2f + 32f * i;
                        int proj1 = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center.X, Projectile.Center.Y, (float)(Math.Sin(offsetAngle) * 5f), (float)(Math.Cos(offsetAngle) * 5f), ModContent.ProjectileType<AetherBeam>(), Projectile.damage, Projectile.knockBack, Projectile.owner, Projectile.ai[0], 1f);
                        int proj2 = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center.X, Projectile.Center.Y, (float)(-Math.Sin(offsetAngle) * 5f), (float)(-Math.Cos(offsetAngle) * 5f), ModContent.ProjectileType<AetherBeam>(), Projectile.damage, Projectile.knockBack, Projectile.owner, Projectile.ai[0], 1f);
                    }
                }
            }
            return true;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(BuffID.ShadowFlame, 600);
        }
    }
}
