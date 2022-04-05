using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.Projectiles.Rogue
{
    public class LatcherMineProjectile : ModProjectile
    {
        private int projdmg = 0;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Latcher Mine");
            Main.projFrames[Projectile.type] = 4;
        }

        public override void SetDefaults()
        {
            Projectile.width = 27;
            Projectile.height = 15;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1;
            Projectile.scale = 1.5f;
            Projectile.alpha = 0;
            Projectile.Calamity().rogue = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
        }

        public override void AI()
        {
            if (Projectile.ai[0] == 0f)
            {
                Projectile.rotation += (Projectile.velocity.X > 0) ? 0.3f : -0.3f;
                if (Projectile.velocity.Y < 7f)
                {
                    Projectile.velocity.Y += 0.24f;
                }
            }
            if (Projectile.ai[0] == 1f)
            {
                Projectile.rotation = Projectile.velocity.ToRotation() - MathHelper.PiOver2;
                Projectile.tileCollide = false;
                int secondsToLive = (Projectile.Calamity().stealthStrike ? 14 : 6);
                bool readyToKillSelf = false;
                //There was something along the lines of "var_2_2CB4E_cp_0" doing this exact iterative expression, but in 4 times as many lines and barely decipherable variables and copies of arrays.
                //*shudder*
                Projectile.localAI[0] += 1f;
                if (Projectile.localAI[0] >= (float)(60 * secondsToLive))
                {
                    readyToKillSelf = true;
                }
                else if ((int)Projectile.ai[1] < 0 || (int)Projectile.ai[1] >= 200)
                {
                    readyToKillSelf = true;
                }
                else if (Main.npc[(int)Projectile.ai[1]].active && !Main.npc[(int)Projectile.ai[1]].dontTakeDamage)
                {
                    Projectile.Center = Main.npc[(int)Projectile.ai[1]].Center - Projectile.velocity * 2f;
                    Projectile.gfxOffY = Main.npc[(int)Projectile.ai[1]].gfxOffY;
                    Projectile.timeLeft = (int)MathHelper.Min(Projectile.timeLeft, 120);
                }
                else
                {
                    readyToKillSelf = true;
                }
                if (readyToKillSelf)
                {
                    Projectile.Kill();
                }
                if (Projectile.timeLeft == 1)
                {
                    Projectile.Kill();
                }
            }
            if (Projectile.ai[0] == 2f)
            {
                Projectile.velocity = Vector2.UnitY * 3f;
                Projectile.rotation = 0f;
            }
            if (Projectile.timeLeft == 110 * (Projectile.Calamity().stealthStrike ? 2 : 1) ||
                Projectile.timeLeft == 60 * (Projectile.Calamity().stealthStrike ? 2 : 1) ||
                Projectile.timeLeft == 24 * (Projectile.Calamity().stealthStrike ? 2 : 1))
            {
                Projectile.frame++;
            }
            if (Projectile.timeLeft < 24 * (Projectile.Calamity().stealthStrike ? 2 : 1))
            {
                Projectile.frameCounter += 1;
                if (Projectile.frameCounter % 2 == 1)
                {
                    Projectile.frame += 1;
                    if (Projectile.frame >= 4)
                    {
                        Projectile.frame = 0;
                    }
                }
            }
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (Projectile.Calamity().stealthStrike)
            {
                Projectile.height = 10;
                if (Projectile.localAI[1] == 0f)
                {
                    Projectile.timeLeft = 14 * 60;
                    Projectile.localAI[1] = 1f;
                }
                Projectile.ai[0] = 2f;
            }
            return !Projectile.Calamity().stealthStrike;
        }
        public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            projdmg = Projectile.damage;
            Projectile.ModifyHitNPCSticky(6, false);
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
            SoundEngine.PlaySound(SoundID.Dig, Projectile.Center);
            Projectile.position = Projectile.Center;
            Projectile.width = Projectile.height = (Projectile.Calamity().stealthStrike ? 240 : 100);
            Projectile.position.X = Projectile.position.X - (float)(Projectile.width / 2);
            Projectile.position.Y = Projectile.position.Y - (float)(Projectile.height / 2);
            Projectile.Damage();
            for (int i = 0; i < (Projectile.Calamity().stealthStrike ? 7 : 4); i++)
            {
                if (Main.rand.NextBool(2) && Projectile.Calamity().stealthStrike)
                {
                    Vector2 shrapnelVelocity = (Vector2.UnitY * (-16f + Main.rand.NextFloat(-3, 12f))).RotatedByRandom((double)MathHelper.ToRadians(40f));
                    Projectile.NewProjectile(Projectile.GetProjectileSource_FromThis(), Projectile.Top, shrapnelVelocity, ModContent.ProjectileType<BarrelShrapnel>(), projdmg, 3f, Projectile.owner);
                }
                else
                {
                    Vector2 fireVelocity = (Vector2.UnitY * (-16f + Main.rand.NextFloat(-3, 12f))).RotatedByRandom((double)MathHelper.ToRadians(40f));
                    int fireIndex = Projectile.NewProjectile(Projectile.GetProjectileSource_FromThis(), Projectile.Top, fireVelocity, ModContent.ProjectileType<TotalityFire>(), projdmg / 3, 1f, Projectile.owner);
                    Main.projectile[fireIndex].localNPCHitCooldown = -2;
                }
            }
        }
    }
}
