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
			if (projectile.Calamity().stealthStrike == true)
			{
				projectile.localAI[1]++;
				if ((double) projectile.localAI[1] >= 35)
				{
					Vector2 velocity = projectile.velocity;
					Vector2 vector2_1 = new Vector2((float) Main.rand.Next(-100, 101), (float) Main.rand.Next(-100, 101));
					vector2_1.Normalize();
					Vector2 vector2_2 = vector2_1 * ((float) Main.rand.Next(10, 21) * 0.1f);
					if (Main.rand.Next(3) == 0)
						vector2_2 *= 2f;
					Vector2 vector2_3 = velocity * 0.25f + vector2_2;
					Vector2 vector2_5 = vector2_3 * 0.8f;
					int proj = Projectile.NewProjectile(projectile.Center.X - vector2_5.X, projectile.Center.Y - vector2_5.Y, vector2_5.X, vector2_5.Y, Main.rand.Next(569,572), (int)(projectile.damage * 0.75), projectile.knockBack, projectile.owner, 0f, 0f);
					Main.projectile[proj].Calamity().forceRogue = true;
					Main.projectile[proj].usesLocalNPCImmunity = true;
					Main.projectile[proj].localNPCHitCooldown = 45;
					projectile.localAI[1] = 0.0f;
				}
            }

            Player player = Main.player[projectile.owner];
            CalamityPlayer modPlayer = player.Calamity();
			if (modPlayer.killSpikyBalls == true)
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

