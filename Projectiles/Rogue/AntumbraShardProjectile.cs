using CalamityMod.Items.Weapons.Rogue;
using CalamityMod.Projectiles.Typeless;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Rogue
{
    public class AntumbraShardProjectile : ModProjectile
    {
        public override string Texture => "CalamityMod/Items/Weapons/Rogue/XerocPitchfork";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Antumbra Shard");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 5;
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            projectile.width = 20;
            projectile.height = 20;
            projectile.friendly = true;
            projectile.ignoreWater = true;
            projectile.penetrate = -1;
            projectile.extraUpdates = 1;
            projectile.aiStyle = 113;
            projectile.timeLeft = 600;
            aiType = ProjectileID.BoneJavelin;
            projectile.Calamity().rogue = true;
        }

        public override void AI()
        {
            projectile.rotation = projectile.velocity.ToRotation() + MathHelper.ToRadians(45f);
            if (projectile.spriteDirection == -1)
            {
                projectile.rotation -= 1.57f;
            }
            if (Main.rand.NextBool(3))
            {
                Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, 62, projectile.velocity.X * 0.5f, projectile.velocity.Y * 0.5f);
            }
            if (projectile.timeLeft % 12 == 0)
            {
                if (projectile.owner == Main.myPlayer)
                {
                    if (projectile.Calamity().stealthStrike)
                    {
                        int star = Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, projectile.velocity.X * 0.25f, projectile.velocity.Y * 0.25f, ModContent.ProjectileType<XerocStar>(), (int)(projectile.damage * 0.5), projectile.knockBack, projectile.owner, 0f, 0f);
                        Main.projectile[star].Calamity().rogue = true;
                    }
                }
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(projectile, ProjectileID.Sets.TrailingMode[projectile.type], lightColor, 1);
            return false;
        }

        public override void Kill(int timeLeft)
        {
            if (Main.rand.NextBool(2))
            {
                Item.NewItem((int)projectile.position.X, (int)projectile.position.Y, projectile.width, projectile.height, ModContent.ItemType<XerocPitchfork>());
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            projectile.damage /= 2;
            if (projectile.damage < 1)
            {
                projectile.damage = 1;
            }
            target.immune[projectile.owner] = 2;
        }

        public override void OnHitPvp(Player target, int damage, bool crit)
        {
            projectile.damage /= 2;
            if (projectile.damage < 1)
            {
                projectile.damage = 1;
            }
        }
    }
}
