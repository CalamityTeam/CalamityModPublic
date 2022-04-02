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
            projectile.width = 14;
            projectile.height = 14;
            projectile.friendly = true;
            projectile.penetrate = 5;
            projectile.timeLeft = 1200;
            projectile.Calamity().rogue = true;
            projectile.aiStyle = 14;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 45;
        }

        public override void AI()
        {
            if (Main.rand.NextBool(20))
            {
                Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, 44, projectile.velocity.X * 0.5f, projectile.velocity.Y * 0.5f);
            }

            if (projectile.timeLeft < 20)
                projectile.alpha += 13;
            if (projectile.Calamity().stealthStrike)
            {
                projectile.localAI[1]++;
                if (projectile.localAI[1] % 35f == 0f)
                {
                    Vector2 velocity = CalamityUtils.RandomVelocity(100f, 10f, 20f);
                    if (Main.rand.NextBool(3))
                        velocity *= 2f;
                    velocity += projectile.velocity * 0.25f;
                    velocity *= 0.8f;
                    int proj = Projectile.NewProjectile(projectile.Center - velocity, velocity, Main.rand.Next(569,572), (int)(projectile.damage * 0.75), projectile.knockBack, projectile.owner);
                    if (proj.WithinBounds(Main.maxProjectiles))
                    {
                        Main.projectile[proj].Calamity().forceRogue = true;
                        Main.projectile[proj].usesLocalNPCImmunity = true;
                        Main.projectile[proj].localNPCHitCooldown = 45;
                    }
                }
            }

            Player player = Main.player[projectile.owner];
            CalamityPlayer modPlayer = player.Calamity();
            if (modPlayer.killSpikyBalls)
            {
                projectile.active = false;
                projectile.netUpdate = true;
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

