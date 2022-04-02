using CalamityMod.Buffs.Pets;
using CalamityMod.CalPlayer;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Pets
{
    public class PerforaMini : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Perforamini");
            Main.projFrames[projectile.type] = 8;
            Main.projPet[projectile.type] = true;
        }

        public override void SetDefaults()
        {
            projectile.netImportant = true;
            projectile.width = 32;
            projectile.height = 32;
            projectile.friendly = true;
            projectile.penetrate = -1;
            projectile.timeLeft *= 5;
            projectile.tileCollide = false;
        }

        public override void AI()
        {
            Player player = Main.player[projectile.owner];
            Vector2 perfcenter = projectile.Center;
            Vector2 vectorperf = player.Center - perfcenter;
            float playerdistance = vectorperf.Length();
            if (!player.active)
            {
                projectile.active = false;
                return;
            }

            //Delete the projectile if the player doesnt have the buff or is very far away (dunno if this needs to be deleted)
            if (!player.HasBuff(ModContent.BuffType<BloodBound>()) || playerdistance >= 4000f)
            {
                projectile.Kill();
            }

            CalamityPlayer modPlayer = player.Calamity();
            if (player.dead)
            {
                modPlayer.perfmini = false;
            }
            if (modPlayer.perfmini)
            {
                projectile.timeLeft = 2;
            }

            projectile.FloatingPetAI(true, 0.1f);

            //Dust
            if (Main.rand.NextBool(50))
            {
                int d1 = Dust.NewDust(projectile.Center, projectile.width, projectile.height, 5, 0f, 0f, 100, default, 1.5f);
                int d2 = Dust.NewDust(projectile.Center, projectile.width, projectile.height, 170, 0f, 0f, 170, default, 0.5f);
                Main.dust[d2].noLight = true;
                Main.dust[d1].position = projectile.Center;
                Main.dust[d2].position = projectile.Center;
            }

            //Animation
            projectile.frameCounter++;
            if (projectile.frameCounter > 6)
            {
                projectile.frame++;
                projectile.frameCounter = 0;
            }
            if (projectile.frame > 5)
            {
                projectile.frame = 0;
            }
        }
    }
}
