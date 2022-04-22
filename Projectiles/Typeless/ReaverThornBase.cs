using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Typeless
{
    public class ReaverThornBase : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/Melee/ThornBase";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Thorn");
        }

        public override void SetDefaults()
        {
            Projectile.width = 28;
            Projectile.height = 28;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.alpha = 255;
            Projectile.ignoreWater = true;
            Projectile.aiStyle = 4;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 10;
        }

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            if (Projectile.ai[0] == 0f)
            {
                Projectile.alpha -= 50;
                if (Projectile.alpha > 0)
                    return;
                Projectile.alpha = 0;
                Projectile.ai[0] = 1f;
                if (Projectile.ai[1] == 0f)
                {
                    Projectile.ai[1] += 1f;
                    Projectile.position += Projectile.velocity * 1f;
                }
                if (Main.myPlayer == Projectile.owner)
                {
                    int type = Projectile.type;
                    if (Projectile.ai[1] >= 6f)
                        type = ModContent.ProjectileType<ReaverThornTip>();
                    int thorn = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center + Projectile.velocity, Projectile.velocity, type, Projectile.damage, Projectile.knockBack, Projectile.owner);
                    Main.projectile[thorn].ai[1] = Projectile.ai[1] + 1f;
                }
            }
            else
            {
                if (Projectile.alpha < 170 && Projectile.alpha + 5 >= 170)
                {
                    for (int index = 0; index < 3; ++index)
                    {
                        Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 18, Projectile.velocity.X * 0.025f, Projectile.velocity.Y * 0.025f, 170, new Color(), 1.2f);
                    }
                    Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 14, 0f, 0f, 170, new Color(), 1.1f);
                }
                Projectile.alpha += 3;
                if (Projectile.alpha < 255)
                    return;
                Projectile.Kill();
            }
        }
    }
}
