using CalamityMod.Dusts;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Magic
{
    public class AcidGunStream : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Acid Stream");
        }

        public override void SetDefaults()
        {
            projectile.width = 32;
            projectile.height = 32;
            projectile.friendly = true;
            projectile.magic = true;
            projectile.ignoreWater = true;
            projectile.penetrate = 3;
            projectile.extraUpdates = 2;
            projectile.tileCollide = false;
        }

        public override void AI()
        {
            projectile.ai[1]++;
            if (projectile.ai[1] >= 4f)
            {
                projectile.tileCollide = true;
            }
            projectile.scale -= 0.002f;
            if (projectile.scale <= 0f)
            {
                projectile.Kill();
            }
            if (projectile.ai[0] <= 3f)
            {
                projectile.ai[0] += 1f;
                return;
            }
            projectile.velocity.Y = projectile.velocity.Y + 0.075f;
            for (int i = 0; i < 3; i++)
            {
                Vector2 positionDelta = projectile.velocity / 3f * i;
                int spawnDelta = 14;
                int dustIdx = Dust.NewDust(new Vector2(projectile.position.X + spawnDelta, projectile.position.Y + spawnDelta), projectile.width - spawnDelta * 2, projectile.height - spawnDelta * 2, (int)CalamityDusts.SulfurousSeaAcid, 0f, 0f, 100, default, 1f);
                Dust dust = Main.dust[dustIdx];
                dust.noGravity = true;
                dust.velocity *= 0.1f;
                dust.velocity += projectile.velocity * 0.5f;
                dust.position -= positionDelta;
            }
            if (Main.rand.NextBool(8))
            {
                int spawnDelta = 16;
                int dustIdx = Dust.NewDust(new Vector2(projectile.position.X + spawnDelta, projectile.position.Y + spawnDelta), projectile.width - spawnDelta * 2, projectile.height - spawnDelta * 2, (int)CalamityDusts.SulfurousSeaAcid, 0f, 0f, 100, default, 0.5f);
                Main.dust[dustIdx].velocity *= 0.25f;
                Main.dust[dustIdx].velocity += projectile.velocity * 0.5f;
            }
        }
    }
}
