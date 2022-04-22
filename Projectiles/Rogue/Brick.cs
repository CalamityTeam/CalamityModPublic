using Terraria;
using Terraria.ModLoader;
using CalamityMod.Items.Weapons.Rogue;
using Terraria.ID;
using Terraria.Audio;

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
            Projectile.width = 19;
            Projectile.height = 19;
            Projectile.friendly = true;
            Projectile.Calamity().rogue = true;
        }

        public override void AI()
        {
            //Constant rotation and gravity
            Projectile.rotation += 0.4f * Projectile.direction;
            Projectile.velocity.Y = Projectile.velocity.Y + 0.3f;
            if (Projectile.velocity.Y > 16f)
            {
                Projectile.velocity.Y = 16f;
            }
            //Dust trail
            if (Main.rand.NextBool(13))
            {
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 22, Projectile.velocity.X * 0.25f, Projectile.velocity.Y * 0.25f, 150, default, 0.9f);
            }
        }

        public override void Kill(int timeLeft)
        {
            if (Main.rand.NextBool(2))
            {
                Item.NewItem(Projectile.GetItemSource_DropAsItem(), (int)Projectile.position.X, (int)Projectile.position.Y, Projectile.width, Projectile.height, ModContent.ItemType<ThrowingBrick>());
            }

            SoundEngine.PlaySound(SoundID.Item, (int)Projectile.position.X, (int)Projectile.position.Y, 50);
            //Dust on impact
            int dust_splash = 0;
            while (dust_splash < 9)
            {
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 9, -Projectile.velocity.X * 0.15f, -Projectile.velocity.Y * 0.15f, 120, default, 1.5f);
                dust_splash += 1;
            }
            // This only triggers if stealth is full
            if (Projectile.ai[0] == 1)
            {
                int split = 0;
                while (split < 3)
                {
                    //Calculate the velocity of the projectile
                    float shardspeedX = -Projectile.velocity.X * Main.rand.NextFloat(.5f, .7f) + Main.rand.NextFloat(-3f, 3f);
                    float shardspeedY = -Projectile.velocity.Y * Main.rand.Next(50, 70) * 0.01f + Main.rand.Next(-8, 9) * 0.2f;
                    //Prevents the projectile speed from being too low
                    if (shardspeedX < 2f && shardspeedX > -2f)
                    {
                        shardspeedX += -Projectile.velocity.X;
                    }
                    if (shardspeedY > 2f && shardspeedY < 2f)
                    {
                        shardspeedY += -Projectile.velocity.Y;
                    }

                    //Spawn the projectile
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.position.X + shardspeedX, Projectile.position.Y + shardspeedY, shardspeedX, shardspeedY, ModContent.ProjectileType<BrickFragment>(), (int)(Projectile.damage * 0.3), Projectile.knockBack / 2f, Projectile.owner);
                    split += 1;
                }
            }
        }
    }
}
