using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using CalamityMod.Items.Potions.Alcohol;

namespace CalamityMod.Projectiles.Rogue
{
    public class InkBombProjectile : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Rogue";
        public override void SetDefaults()
        {
            Projectile.width = 14;
            Projectile.height = 22;
            Projectile.friendly = true;
            Projectile.alpha = 0;
            Projectile.penetrate = 1;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 50;
            Projectile.DamageType = RogueDamageClass.Instance;
        }

        public override void AI()
        {
            Projectile.velocity.Y += 0.1f;
            Projectile.rotation += Projectile.velocity.X * 0.1f;

            if (Projectile.timeLeft == 1)
                CreateInk();
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (!target.friendly)
                CreateInk();
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info) => CreateInk();

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            CreateInk();
            return true;
        }

        private void CreateInk()
        {
            Player player = Main.player[Projectile.owner];
            SoundEngine.PlaySound(SoundID.Item14, Projectile.position);
            for (int i = 0; i < 20; i++)
            {
                int projType = Main.rand.Next(0, 3);
                int inkType;
                switch (projType)
                {
                    case 0:
                        inkType = ModContent.ProjectileType<InkCloud>();
                        break;
                    case 1:
                        inkType = ModContent.ProjectileType<InkCloud2>();
                        break;
                    default:
                        inkType = ModContent.ProjectileType<InkCloud3>();
                        break;
                }
                int damage = (int)player.GetTotalDamage<RogueDamageClass>().ApplyTo(22);
                damage = player.ApplyArmorAccDamageBonusesTo(damage);

                int inkID = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center.X, Projectile.Center.Y, Main.rand.NextFloat(-2f, 2f), Main.rand.NextFloat(-2f, 2f), inkType, damage, 7, Projectile.owner);
                Main.projectile[inkID].timeLeft += Main.rand.Next(-20, 25);
            }
            Projectile.Kill();
        }
    }
}
