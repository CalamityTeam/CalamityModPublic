using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
namespace CalamityMod.Projectiles.Rogue
{
    public class SeafoamBombProj : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Seafoam Bomb");
        }

        public override void SetDefaults()
        {
            Projectile.width = 14;
            Projectile.height = 20;
            Projectile.friendly = true;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 240;
            Projectile.Calamity().rogue = true;
        }

        public override void AI()
        {
            Projectile.rotation += Projectile.velocity.X * 0.1f;
            Projectile.velocity.Y = Projectile.velocity.Y + 0.15f;
            Projectile.velocity.X = Projectile.velocity.X * 0.99f;
        }

        public override void Kill(int timeLeft)
        {
            Projectile.position = Projectile.Center;
            Projectile.width = Projectile.height = Projectile.Calamity().stealthStrike ? 256 : 128;
            Projectile.position.X = Projectile.position.X - (float)(Projectile.width / 2);
            Projectile.position.Y = Projectile.position.Y - (float)(Projectile.height / 2);
            Projectile.maxPenetrate = -1;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
            Projectile.Damage();
            SoundEngine.PlaySound(SoundID.Item14, Projectile.Center);

            for (int i = 0; i < (Projectile.Calamity().stealthStrike ? 5 : 1); i++)
            {
                float posX = Projectile.Center.X + (Projectile.Calamity().stealthStrike ? Main.rand.Next(-50, 51) : 0);
                float posY = Projectile.Center.Y + (Projectile.Calamity().stealthStrike ? Main.rand.Next(-50, 51) : 0);
                Projectile.NewProjectile(posX, posY, 0f, 0f, ModContent.ProjectileType<SeafoamBubble>(), (int)((double)Projectile.damage * 0.4), 0f, Projectile.owner, 0f, 0f);
            }

            CalamityUtils.ExplosionGores(Projectile.Center, (Projectile.Calamity().stealthStrike ? 6 : 3));
        }
    }
}
