using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.CalPlayer;

namespace CalamityMod.Projectiles.Rogue
{
    public class PoisonBol : ModProjectile
    {
        public override string Texture => "CalamityMod/Items/Weapons/Rogue/PoisonPack";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Poison Ball");
        }

        public override void SetDefaults()
        {
            Projectile.width = 14;
            Projectile.height = 14;
            Projectile.friendly = true;
            Projectile.penetrate = 5;
            Projectile.timeLeft = 1200;
            Projectile.DamageType = RogueDamageClass.Instance;
            Projectile.aiStyle = 14;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 45;
        }

        public override void AI()
        {
            if (Main.rand.NextBool(20))
            {
                Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, 44, Projectile.velocity.X * 0.5f, Projectile.velocity.Y * 0.5f);
            }

            if (Projectile.timeLeft < 20)
                Projectile.alpha += 13;
            if (Projectile.Calamity().stealthStrike)
            {
                Projectile.localAI[1]++;
                if (Projectile.localAI[1] % 35f == 0f)
                {
                    Vector2 velocity = CalamityUtils.RandomVelocity(100f, 10f, 20f);
                    if (Main.rand.NextBool(3))
                        velocity *= 2f;
                    velocity += Projectile.velocity * 0.25f;
                    velocity *= 0.8f;
                    int proj = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center - velocity, velocity, Main.rand.Next(569,572), (int)(Projectile.damage * 0.75), Projectile.knockBack, Projectile.owner);
                    if (proj.WithinBounds(Main.maxProjectiles))
                    {
                        Main.projectile[proj].DamageType = RogueDamageClass.Instance;
                        Main.projectile[proj].usesLocalNPCImmunity = true;
                        Main.projectile[proj].localNPCHitCooldown = 45;
                    }
                }
            }

            Player player = Main.player[Projectile.owner];
            CalamityPlayer modPlayer = player.Calamity();
            if (modPlayer.killSpikyBalls)
            {
                Projectile.active = false;
                Projectile.netUpdate = true;
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(BuffID.Poisoned, 240);
        }

        public override void OnHitPvp(Player target, int damage, bool crit)
        {
            target.AddBuff(BuffID.Poisoned, 240);
        }
    }
}

