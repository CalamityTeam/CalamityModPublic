using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Rogue
{
    public class MangroveChakramProjectile : ModProjectile
    {
        public override string Texture => "CalamityMod/Items/Weapons/Rogue/MangroveChakram";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Mangrove Chakram");
        }

        public override void SetDefaults()
        {
            projectile.width = 30;
            projectile.height = 30;
            projectile.friendly = true;
            projectile.ignoreWater = true;
            projectile.penetrate = -1;
            projectile.aiStyle = 3;
            projectile.timeLeft = 600;
            aiType = ProjectileID.WoodenBoomerang;
            projectile.Calamity().rogue = true;
        }

        public override void AI()
        {
            Lighting.AddLight(projectile.Center, 0f, 0.25f, 0f);
            if (Main.rand.NextBool(5))
                Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, 44, projectile.velocity.X * 0.5f, projectile.velocity.Y * 0.5f);
            if (projectile.Calamity().stealthStrike)
            {
                // Die early.
                if (projectile.timeLeft < 240)
                    projectile.Kill();

                projectile.localAI[0] += Main.rand.Next(0,3);
                if (projectile.localAI[0] >= 10f)
                {
                    projectile.localAI[0] = 0f;
                    Vector2 flowerSpawnPosition = projectile.Center + Main.rand.NextVector2Square(-10f, 10f);
                    Vector2 flowerShootVelocity = projectile.velocity.RotatedByRandom(0.1f) * 0.25f;
                    Projectile.NewProjectile(flowerSpawnPosition, flowerShootVelocity, ModContent.ProjectileType<MangroveChakramFlower>(), projectile.damage / 4, 0f, projectile.owner);
                }
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D tex = Main.projectileTexture[projectile.type];
            spriteBatch.Draw(tex, projectile.Center - Main.screenPosition, null, projectile.GetAlpha(lightColor), projectile.rotation, tex.Size() / 2f, projectile.scale, SpriteEffects.None, 0f);
            return false;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.immune[projectile.owner] = 6;
            target.AddBuff(BuffID.CursedInferno, 120);
        }

        public override void OnHitPvp(Player target, int damage, bool crit)
        {
            target.AddBuff(BuffID.CursedInferno, 120);
        }
    }
}
