using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Melee
{
    public class JudgementProj : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Melee";
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        int whiteLightTimer = 15;

        public override void SetStaticDefaults() => ProjectileID.Sets.CultistIsResistantTo[Type] = true;
        public override void SetDefaults()
        {
            Projectile.width = 12;
            Projectile.height = 12;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 90;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 10;
        }

        public override void AI()
        {
            whiteLightTimer--;
            if (whiteLightTimer == 0)
            {
                float spread = 180f * 0.0174f;
                double startAngle = Math.Atan2(Projectile.velocity.X, Projectile.velocity.Y) - spread / 2;
                double deltaAngle = spread / 8f;
                double offsetAngle;
                int i;
                if (Projectile.owner == Main.myPlayer)
                {
                    for (i = 0; i < 1; i++)
                    {
                        offsetAngle = startAngle + deltaAngle * (i + i * i) / 2f + 32f * i;
                        int projectile1 = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center.X, Projectile.Center.Y, (float)(Math.Sin(offsetAngle) * 5f), (float)(Math.Cos(offsetAngle) * 5f), ModContent.ProjectileType<WhiteOrb>(), Projectile.damage / 2, Projectile.knockBack * 0.5f, Projectile.owner);
                        int projectile2 = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center.X, Projectile.Center.Y, (float)(-Math.Sin(offsetAngle) * 5f), (float)(-Math.Cos(offsetAngle) * 5f), ModContent.ProjectileType<WhiteOrb>(), Projectile.damage / 2, Projectile.knockBack * 0.5f, Projectile.owner);
                        Main.projectile[projectile1].velocity.X *= 0.1f;
                        Main.projectile[projectile1].velocity.Y *= 0.1f;
                        Main.projectile[projectile2].velocity.X *= 0.1f;
                        Main.projectile[projectile2].velocity.Y *= 0.1f;
                    }
                }
                whiteLightTimer = 15;
            }

            Lighting.AddLight(Projectile.Center, (255 - Projectile.alpha) * 0.5f / 255f, (255 - Projectile.alpha) * 0.5f / 255f, (255 - Projectile.alpha) * 0.5f / 255f);

            for (int i = 0; i < 2; i++)
            {
                int shinyDust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.GemDiamond, 0f, 0f, 100, default, 1.25f);
                Main.dust[shinyDust].noGravity = true;
                Main.dust[shinyDust].velocity *= 0.5f;
                Main.dust[shinyDust].velocity += Projectile.velocity * 0.1f;
            }

            CalamityUtils.HomeInOnNPC(Projectile, !Projectile.tileCollide, 200f, 10f, 20f);
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item122, Projectile.position);
            if (Projectile.owner == Main.myPlayer)
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center.X, Projectile.Center.Y - 100, 0f, 0f, ModContent.ProjectileType<WhiteBoltAura>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
        }
    }
}
