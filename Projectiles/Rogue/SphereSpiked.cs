using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
namespace CalamityMod.Projectiles.Rogue
{
    public class SphereSpiked : ModProjectile
    {
        public override string Texture => "CalamityMod/Items/Weapons/Rogue/DefectiveSphere";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Spiked Sphere");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 5;
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            projectile.width = 20;
            projectile.height = 20;
            projectile.friendly = true;
            projectile.penetrate = -1;
            projectile.ignoreWater = true;
            projectile.aiStyle = 3;
            projectile.timeLeft = 300;
            aiType = ProjectileID.WoodenBoomerang;
            projectile.Calamity().rogue = true;
        }

        public override void AI()
        {
            Lighting.AddLight(projectile.Center, 1f, 0f, 0f);
            if (Main.rand.NextBool(5))
            {
                Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, 229, projectile.velocity.X * 0.5f, projectile.velocity.Y * 0.5f, 100);
            }
            if (projectile.ai[0] == 1f)
            {
                projectile.extraUpdates = 1;
            }
        }

        public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            damage = (int)(damage * 1.2);
            crit |= Main.rand.NextBool(10);
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            Main.PlaySound(SoundID.NPCHit34, projectile.position);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(projectile, ProjectileID.Sets.TrailingMode[projectile.type], lightColor, 1);
            return false;
        }
    }
}
