using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Rogue
{
    public class DeificThunderboltProj : ModProjectile
    {
        private bool playedSound = false;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Deific Thunderbolt");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;
            Main.projFrames[projectile.type] = 8;
        }

        public override void SetDefaults()
        {
            projectile.width = 26;
            projectile.height = 26;
            projectile.friendly = true;
            projectile.penetrate = 1;
            projectile.extraUpdates = 1;
            projectile.ignoreWater = true;
            projectile.tileCollide = false;
            projectile.timeLeft = 300;
            projectile.Calamity().rogue = true;
        }

        public override void AI()
        {
            projectile.frameCounter++;
            if (projectile.frameCounter > 6)
            {
                projectile.frame++;
                projectile.frameCounter = 0;
            }
            if (projectile.frame >= Main.projFrames[projectile.type])
            {
                projectile.frame = 0;
            }
            if (!playedSound)
            {
                Main.PlaySound(SoundID.Item92, (int)projectile.position.X, (int)projectile.position.Y); //electrosphere launcher fire sound
                playedSound = true;
            }
            if (Main.rand.NextBool(8))
            {
                Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, 132, projectile.velocity.X * 0.5f, projectile.velocity.Y * 0.5f);
            }
            projectile.spriteDirection = projectile.direction = (projectile.velocity.X > 0).ToDirectionInt();
            projectile.rotation = projectile.velocity.ToRotation() + (projectile.spriteDirection == 1 ? 0f : MathHelper.Pi);
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(BuffID.Electrified, 180);
        }

        public override void OnHitPvp(Player target, int damage, bool crit)
        {
            target.AddBuff(BuffID.Electrified, 180);
        }

        public override void Kill(int timeLeft)
        {
            for (int i = 0; i <= 10; i++)
            {
                Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, 132, projectile.oldVelocity.X * 0.5f, projectile.oldVelocity.Y * 0.5f);
            }

            if (Main.myPlayer == projectile.owner)
            {
                bool stealthStrike = projectile.Calamity().stealthStrike;
                if (stealthStrike)
                {
                    Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Custom/LightningStrike"), (int)projectile.position.X, (int)projectile.position.Y);
                }
                int amt = stealthStrike ? 5 : 1;
                float damageMult = stealthStrike ? 0.5f : 1f;
                for (int n = 0; n < amt; n++)
                {
                    Vector2 spawnPoint = new Vector2(projectile.Center.X + (float)Main.rand.Next(-100, 101), projectile.Center.Y - (float)Main.rand.Next(700, 801));
                    float randomVelocity = Main.rand.NextFloat() - 0.5f;
                    Vector2 fireTo = new Vector2(spawnPoint.X + 100f * randomVelocity, spawnPoint.Y + 900);
                    Vector2 ai0 = fireTo - spawnPoint;
                    float ai = (float)Main.rand.Next(100);
                    Vector2 velocity = Vector2.Normalize(ai0.RotatedByRandom(0.78539818525314331)) * 9f;
                    int proj = Projectile.NewProjectile(spawnPoint.X, spawnPoint.Y, velocity.X, velocity.Y, ProjectileID.CultistBossLightningOrbArc, (int)(projectile.damage * damageMult), projectile.knockBack, projectile.owner, ai0.ToRotation(), ai);
                    Main.projectile[proj].extraUpdates += 9;
                    //Does not force to Rogue because lightning is extremely abusable with Moonstone Crown
                    Main.projectile[proj].friendly = true;
                    Main.projectile[proj].hostile = false;
                    Main.projectile[proj].penetrate = -1;
                    Main.projectile[proj].usesLocalNPCImmunity = true;
                    Main.projectile[proj].localNPCHitCooldown = -1;
                }
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(projectile, ProjectileID.Sets.TrailingMode[projectile.type], lightColor, 1);
            return false;
        }
    }
}
