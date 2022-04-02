using CalamityMod.Dusts;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Melee
{
    public class SulphurousGrabberBubble2 : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Bubble");
        }

        public override void SetDefaults()
        {
            projectile.width = 18;
            projectile.height = 18;
            projectile.friendly = true;
            projectile.alpha = 60;
            projectile.penetrate = 1;
            projectile.tileCollide = false;
            projectile.timeLeft = 120;
            projectile.melee = true;
        }

        public override void AI()
        {
            projectile.ai[0]++;
            projectile.velocity.X *= 0.9f;
            projectile.velocity.Y *= 0.9f;
            if (Main.rand.NextBool(5))
                Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, (int)CalamityDusts.SulfurousSeaAcid, projectile.velocity.X * 0.5f, projectile.velocity.Y * 0.5f);
        }

        public override void Kill(int timeLeft)
        {
            Main.PlaySound(SoundID.Item54, projectile.position);
            int num190 = Main.rand.Next(5, 9);
            for (int num191 = 0; num191 < num190; num191++)
            {
                int num192 = Dust.NewDust(projectile.Center, 0, 0, (int)CalamityDusts.SulfurousSeaAcid, 0f, 0f, 100, default, 1.4f);
                Main.dust[num192].velocity *= 0.8f;
                Main.dust[num192].position = Vector2.Lerp(Main.dust[num192].position, projectile.Center, 0.5f);
                Main.dust[num192].noGravity = true;
            }
        }
    }
}
