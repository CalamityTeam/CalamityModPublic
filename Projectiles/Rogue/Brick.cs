using Terraria;
using Terraria.ModLoader;
using CalamityMod.Items.Weapons.Rogue;
using Terraria.ID;

namespace CalamityMod.Projectiles.Rogue
{
    public class Brick : ModProjectile
    {
        public override string Texture => "CalamityMod/Items/Weapons/Rogue/ThrowingBrick";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Brick");
        }

        public override void SetDefaults()
        {
            projectile.width = 19;
            projectile.height = 19;
            projectile.friendly = true;
            projectile.Calamity().rogue = true;
        }

        public override void AI()
        {
            //Constant rotation and gravity
            projectile.rotation += 0.4f * projectile.direction;
            projectile.velocity.Y = projectile.velocity.Y + 0.3f;
            if (projectile.velocity.Y > 16f)
            {
                projectile.velocity.Y = 16f;
            }
            //Dust trail
            if (Main.rand.NextBool(13))
            {
                Dust.NewDust(projectile.position, projectile.width, projectile.height, 22, projectile.velocity.X * 0.25f, projectile.velocity.Y * 0.25f, 150, default, 0.9f);
            }
        }

        public override void Kill(int timeLeft)
        {
            if (Main.rand.NextBool(2))
            {
                Item.NewItem((int)projectile.position.X, (int)projectile.position.Y, projectile.width, projectile.height, ModContent.ItemType<ThrowingBrick>());
            }

            Main.PlaySound(SoundID.Item, (int)projectile.position.X, (int)projectile.position.Y, 50);
            //Dust on impact
            int dust_splash = 0;
            while (dust_splash < 9)
            {
                Dust.NewDust(projectile.position, projectile.width, projectile.height, 9, -projectile.velocity.X * 0.15f, -projectile.velocity.Y * 0.15f, 120, default, 1.5f);
                dust_splash += 1;
            }
            // This only triggers if stealth is full
            if (projectile.ai[0] == 1)
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
                    Projectile.NewProjectile(projectile.position.X + shardspeedX, projectile.position.Y + shardspeedY, shardspeedX, shardspeedY, ModContent.ProjectileType<BrickFragment>(), (int)(projectile.damage * 0.3), projectile.knockBack / 2f, projectile.owner);
                    split += 1;
                }
            }
        }
    }
}
