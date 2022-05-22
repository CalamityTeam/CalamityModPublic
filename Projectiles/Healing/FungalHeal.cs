using Terraria;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Healing
{
    public class FungalHeal : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Heal");
        }

        public override void SetDefaults()
        {
            Projectile.width = 6;
            Projectile.height = 6;
            Projectile.alpha = 255;
            Projectile.tileCollide = false;
            Projectile.extraUpdates = 10;
            Projectile.timeLeft = 300;
        }

        public override void AI()
        {
            if (Projectile.localAI[0] == 0f)
            {
                Player player = Main.player[Projectile.owner];
                Item item = player.ActiveItem();
                bool summonNotWhip = item.CountsAsClass<SummonDamageClass>() && !item.CountsAsClass<SummonMeleeSpeedDamageClass>();
                if (summonNotWhip || item.hammer > 0 || item.pick > 0 || item.axe > 0)
                    Projectile.timeLeft = 600;
                Projectile.localAI[0] += 1f;
            }

            Projectile.HealingProjectile((int)Projectile.ai[1], (int)Projectile.ai[0], 5f, 15f);
            float num494 = Projectile.velocity.X * 0.334f;
            float num495 = -(Projectile.velocity.Y * 0.334f);
            int num496 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 56, 0f, 0f, 100, default, 0.5f);
            Dust dust = Main.dust[num496];
            dust.noGravity = true;
            dust.position.X -= num494;
            dust.position.Y -= num495;
            float num498 = Projectile.velocity.X * 0.2f;
            float num499 = -(Projectile.velocity.Y * 0.2f);
            int num500 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 56, 0f, 0f, 100, default, 0.7f);
            Dust dust2 = Main.dust[num500];
            dust2.noGravity = true;
            dust2.position.X -= num498;
            dust2.position.Y -= num499;
        }
    }
}
