using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Rogue
{
	public class Crystalline2 : ModProjectile
    {
        public override string Texture => "CalamityMod/Items/Weapons/Rogue/Crystalline";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Crystalline");
        }

        public override void SetDefaults()
        {
            projectile.width = 10;
            projectile.height = 10;
            projectile.friendly = true;
            projectile.penetrate = 1;
            //projectile.aiStyle = 113;
            projectile.timeLeft = 30;
            //aiType = ProjectileID.BoneJavelin;
            projectile.Calamity().rogue = true;
        }

        public override void AI()
        {
            projectile.localAI[0]++;
            projectile.rotation = projectile.velocity.ToRotation() + MathHelper.ToRadians(45f);
            if (projectile.localAI[0] == 10f && projectile.ai[1] == 1f)
            {
                int numProj = 2;
                float rotation = MathHelper.ToRadians(50);
                if (projectile.owner == Main.myPlayer)
                {
                    for (int i = 0; i < numProj + 1; i++)
                    {
                        Vector2 perturbedSpeed = new Vector2(projectile.velocity.X * 0.8f, projectile.velocity.Y * 0.8f).RotatedBy(MathHelper.Lerp(-rotation, rotation, i / (numProj - 1)));
                        int proj = Projectile.NewProjectile(projectile.Center, perturbedSpeed, ModContent.ProjectileType<Crystalline2>(), (int)(projectile.damage * 0.5f), projectile.knockBack, projectile.owner, 0f, 2f);
                        Main.projectile[proj].timeLeft = 20;
                    }
                }
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
			if (projectile.timeLeft == (projectile.ai[1] == 2f ? 20 : 30))
				return false;
            Texture2D tex = Main.projectileTexture[projectile.type];
            spriteBatch.Draw(tex, projectile.Center - Main.screenPosition, null, projectile.GetAlpha(lightColor), projectile.rotation, tex.Size() / 2f, projectile.scale, SpriteEffects.None, 0f);
            return false;
        }

        public override void Kill(int timeLeft)
        {
            for (int k = 0; k < 5; k++)
            {
                Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, 154, projectile.oldVelocity.X * 0.5f, projectile.oldVelocity.Y * 0.5f);
            }
            if (projectile.ai[1] >= 1f)
            {
                for (int i = 0; i < 3; i++)
                {
                    Vector2 projspeed = new Vector2(Main.rand.NextFloat(-8f, 8f), Main.rand.NextFloat(-8f, 8f));
					int shard = Projectile.NewProjectile(projectile.Center, projspeed, ProjectileID.CrystalShard, (int)(projectile.damage * 0.4f), 2f, projectile.owner);
					if (shard.WithinBounds(Main.maxProjectiles))
						Main.projectile[shard].Calamity().forceRogue = true;
                }
            }
        }
    }
}
