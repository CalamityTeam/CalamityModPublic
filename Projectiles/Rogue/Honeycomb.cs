using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items.Weapons.Rogue;

namespace CalamityMod.Projectiles.Rogue
{
    public class Honeycomb : ModProjectile
    {
        public override string Texture => "CalamityMod/Items/Weapons/Rogue/HardenedHoneycomb";

        private const float radius = 15f;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Honeycomb");
        }

        public override void SetDefaults()
        {
            projectile.width = 30;
            projectile.height = 30;
            projectile.friendly = true;
            projectile.penetrate = 1;
            projectile.aiStyle = 2;
            projectile.timeLeft = 300;
            aiType = ProjectileID.ThrowingKnife;
            projectile.Calamity().rogue = true;
        }

        public override void AI()
        {
            Player player = Main.player[Main.myPlayer];
            Vector2 posDiff = player.Center - projectile.Center;
            if (posDiff.Length() <= radius)
            {
                player.AddBuff(BuffID.Honey, 300);
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            int split = 0;
            while (split < 3)
            {
                //Calculate the velocity of the projectile
                float shardspeedX = -projectile.velocity.X * Main.rand.NextFloat(.5f, .7f) + Main.rand.NextFloat(-3f, 3f);
                float shardspeedY = -projectile.velocity.Y * Main.rand.Next(50, 70) * 0.01f + Main.rand.Next(-8, 9) * 0.2f;
                //Prevents the projectile speed from being too low
                if (shardspeedX < 2f && shardspeedX > -2f)
                {
                    shardspeedX += -projectile.velocity.X;
                }
                if (shardspeedY > 2f && shardspeedY < 2f)
                {
                    shardspeedY += -projectile.velocity.Y;
                }

                //Spawn the projectile
                Projectile.NewProjectile(projectile.position.X + shardspeedX, projectile.position.Y + shardspeedY, shardspeedX, shardspeedY, ModContent.ProjectileType<HoneycombFragment>(), (int)(projectile.damage * 0.3), 2f, projectile.owner, Main.rand.Next(3), 0f);
                split += 1;
            }
            projectile.penetrate--;
            if (projectile.penetrate <= 0)
            {
                projectile.Kill();
            }
            else
            {
                if (projectile.velocity.X != oldVelocity.X)
                {
                    projectile.velocity.X = -oldVelocity.X;
                }
                if (projectile.velocity.Y != oldVelocity.Y)
                {
                    projectile.velocity.Y = -oldVelocity.Y;
                }
            }
            return false;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            projectile.velocity.X = -projectile.velocity.X;
            projectile.velocity.Y = -projectile.velocity.Y;
            int split = 0;
            while (split < 3)
            {
                //Calculate the velocity of the projectile
                float shardspeedX = -projectile.velocity.X * Main.rand.NextFloat(.5f, .7f) + Main.rand.NextFloat(-3f, 3f);
                float shardspeedY = -projectile.velocity.Y * Main.rand.Next(50, 70) * 0.01f + Main.rand.Next(-8, 9) * 0.2f;
                //Prevents the projectile speed from being too low
                if (shardspeedX < 2f && shardspeedX > -2f)
                {
                    shardspeedX += -projectile.velocity.X;
                }
                if (shardspeedY > 2f && shardspeedY < 2f)
                {
                    shardspeedY += -projectile.velocity.Y;
                }

                //Spawn the projectile
                Projectile.NewProjectile(projectile.position.X + shardspeedX, projectile.position.Y + shardspeedY, shardspeedX, shardspeedY, ModContent.ProjectileType<HoneycombFragment>(), (int)(projectile.damage * 0.3), 2f, projectile.owner, Main.rand.Next(3), 0f);
                split += 1;
            }
        }

        public override void OnHitPvp(Player target, int damage, bool crit)
        {
            projectile.velocity.X = -projectile.velocity.X;
            projectile.velocity.Y = -projectile.velocity.Y;
            int split = 0;
            while (split < 3)
            {
                //Calculate the velocity of the projectile
                float shardspeedX = -projectile.velocity.X * Main.rand.NextFloat(.5f, .7f) + Main.rand.NextFloat(-3f, 3f);
                float shardspeedY = -projectile.velocity.Y * Main.rand.Next(50, 70) * 0.01f + Main.rand.Next(-8, 9) * 0.2f;
                //Prevents the projectile speed from being too low
                if (shardspeedX < 2f && shardspeedX > -2f)
                {
                    shardspeedX += -projectile.velocity.X;
                }
                if (shardspeedY > 2f && shardspeedY < 2f)
                {
                    shardspeedY += -projectile.velocity.Y;
                }

                //Spawn the projectile
                Projectile.NewProjectile(projectile.position.X + shardspeedX, projectile.position.Y + shardspeedY, shardspeedX, shardspeedY, ModContent.ProjectileType<HoneycombFragment>(), (int)(projectile.damage * 0.3), 2f, projectile.owner, Main.rand.Next(3), 0f);
                split += 1;
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D tex = Main.projectileTexture[projectile.type];
            spriteBatch.Draw(tex, projectile.Center - Main.screenPosition, null, projectile.GetAlpha(lightColor), projectile.rotation, tex.Size() / 2f, projectile.scale, SpriteEffects.None, 0f);
            return false;
        }

        public override void Kill(int timeLeft)
        {
            Main.PlaySound(SoundID.NPCDeath1, projectile.position);
            //Dust on impact
            int dust_splash = 0;
            while (dust_splash < 9)
            {
                Dust.NewDust(projectile.position, projectile.width, projectile.height, 9, -projectile.velocity.X * 0.15f, -projectile.velocity.Y * 0.15f, 159, default, 1.5f);
                dust_splash += 1;
            }
            if (Main.rand.NextBool(2))
            {
                Item.NewItem((int)projectile.position.X, (int)projectile.position.Y, projectile.width, projectile.height, ModContent.ItemType<HardenedHoneycomb>());
            }
        }
    }
}
