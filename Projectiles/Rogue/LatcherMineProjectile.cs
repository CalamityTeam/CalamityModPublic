using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Rogue
{
    public class LatcherMineProjectile : ModProjectile
    {
        private int projdmg = 0;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Latcher Mine");
            Main.projFrames[projectile.type] = 4;
        }

        public override void SetDefaults()
        {
            projectile.width = 27;
            projectile.height = 15;
            projectile.friendly = true;
            projectile.ignoreWater = true;
            projectile.penetrate = -1;
            projectile.scale = 1.5f;
            projectile.alpha = 0;
            projectile.Calamity().rogue = true;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 10;
        }

        public override void AI()
        {
            if (projectile.ai[0] == 0f)
            {
                projectile.rotation += (projectile.velocity.X > 0) ? 0.3f : -0.3f;
                if (projectile.velocity.Y < 7f)
                {
                    projectile.velocity.Y += 0.24f;
                }
            }
            if (projectile.ai[0] == 1f)
            {
                projectile.rotation = projectile.velocity.ToRotation() - MathHelper.PiOver2;
                projectile.tileCollide = false;
                int secondsToLive = (projectile.Calamity().stealthStrike ? 14 : 6);
                bool readyToKillSelf = false;
                //There was something along the lines of "var_2_2CB4E_cp_0" doing this exact iterative expression, but in 4 times as many lines and barely decipherable variables and copies of arrays.
                //*shudder*
                projectile.localAI[0] += 1f;
                if (projectile.localAI[0] >= (float)(60 * secondsToLive))
                {
                    readyToKillSelf = true;
                }
                else if ((int)projectile.ai[1] < 0 || (int)projectile.ai[1] >= 200)
                {
                    readyToKillSelf = true;
                }
                else if (Main.npc[(int)projectile.ai[1]].active && !Main.npc[(int)projectile.ai[1]].dontTakeDamage)
                {
                    projectile.Center = Main.npc[(int)projectile.ai[1]].Center - projectile.velocity * 2f;
                    projectile.gfxOffY = Main.npc[(int)projectile.ai[1]].gfxOffY;
                    projectile.timeLeft = (int)MathHelper.Min(projectile.timeLeft, 120);
                }
                else
                {
                    readyToKillSelf = true;
                }
                if (readyToKillSelf)
                {
                    projectile.Kill();
                }
                if (projectile.timeLeft == 1)
                {
                    projectile.Kill();
                }
            }
            if (projectile.ai[0] == 2f)
            {
                projectile.velocity = Vector2.UnitY * 3f;
                projectile.rotation = 0f;
            }
            if (projectile.timeLeft == 110 * (projectile.Calamity().stealthStrike ? 2 : 1) ||
                projectile.timeLeft == 60 * (projectile.Calamity().stealthStrike ? 2 : 1) ||
                projectile.timeLeft == 24 * (projectile.Calamity().stealthStrike ? 2 : 1))
            {
                projectile.frame++;
            }
            if (projectile.timeLeft < 24 * (projectile.Calamity().stealthStrike ? 2 : 1))
            {
                projectile.frameCounter += 1;
                if (projectile.frameCounter % 2 == 1)
                {
                    projectile.frame += 1;
                    if (projectile.frame >= 4)
                    {
                        projectile.frame = 0;
                    }
                }
            }
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (projectile.Calamity().stealthStrike)
            {
                projectile.height = 10;
                if (projectile.localAI[1] == 0f)
                {
                    projectile.timeLeft = 14 * 60;
                    projectile.localAI[1] = 1f;
                }
                projectile.ai[0] = 2f;
            }
            return !projectile.Calamity().stealthStrike;
        }
        public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            projdmg = projectile.damage;
            projectile.ModifyHitNPCSticky(6, false);
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (targetHitbox.Width > 8 && targetHitbox.Height > 8)
            {
                targetHitbox.Inflate(-targetHitbox.Width / 8, -targetHitbox.Height / 8);
            }
            return null;
        }

        public override void Kill(int timeLeft)
        {
            Main.PlaySound(SoundID.Dig, projectile.Center);
            projectile.position = projectile.Center;
            projectile.width = projectile.height = (projectile.Calamity().stealthStrike ? 240 : 100);
            projectile.position.X = projectile.position.X - (float)(projectile.width / 2);
            projectile.position.Y = projectile.position.Y - (float)(projectile.height / 2);
            projectile.Damage();
            for (int i = 0; i < (projectile.Calamity().stealthStrike ? 7 : 4); i++)
            {
                if (Main.rand.NextBool(2) && projectile.Calamity().stealthStrike)
                {
                    Vector2 shrapnelVelocity = (Vector2.UnitY * (-16f + Main.rand.NextFloat(-3, 12f))).RotatedByRandom((double)MathHelper.ToRadians(40f));
                    Projectile.NewProjectile(projectile.Top, shrapnelVelocity, ModContent.ProjectileType<BarrelShrapnel>(), projdmg, 3f, projectile.owner);
                }
                else
                {
                    Vector2 fireVelocity = (Vector2.UnitY * (-16f + Main.rand.NextFloat(-3, 12f))).RotatedByRandom((double)MathHelper.ToRadians(40f));
                    int fireIndex = Projectile.NewProjectile(projectile.Top, fireVelocity, ModContent.ProjectileType<TotalityFire>(), projdmg / 3, 1f, projectile.owner);
                    Main.projectile[fireIndex].localNPCHitCooldown = -2;
                }
            }
        }
    }
}
