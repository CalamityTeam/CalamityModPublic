using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.Projectiles.Rogue
{
    public class DeificThunderboltProj : ModProjectile
    {
        private bool playedSound = false;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Deific Thunderbolt");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
            Main.projFrames[Projectile.type] = 8;
        }

        public override void SetDefaults()
        {
            Projectile.width = 26;
            Projectile.height = 26;
            Projectile.friendly = true;
            Projectile.penetrate = 1;
            Projectile.extraUpdates = 1;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 300;
            Projectile.Calamity().rogue = true;
        }

        public override void AI()
        {
            Projectile.frameCounter++;
            if (Projectile.frameCounter > 6)
            {
                Projectile.frame++;
                Projectile.frameCounter = 0;
            }
            if (Projectile.frame >= Main.projFrames[Projectile.type])
            {
                Projectile.frame = 0;
            }
            if (!playedSound)
            {
                SoundEngine.PlaySound(SoundID.Item92, (int)Projectile.position.X, (int)Projectile.position.Y); //electrosphere launcher fire sound
                playedSound = true;
            }
            if (Main.rand.NextBool(8))
            {
                Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, 132, Projectile.velocity.X * 0.5f, Projectile.velocity.Y * 0.5f);
            }
            Projectile.spriteDirection = Projectile.direction = (Projectile.velocity.X > 0).ToDirectionInt();
            Projectile.rotation = Projectile.velocity.ToRotation() + (Projectile.spriteDirection == 1 ? 0f : MathHelper.Pi);
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
                Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, 132, Projectile.oldVelocity.X * 0.5f, Projectile.oldVelocity.Y * 0.5f);
            }

            if (Main.myPlayer == Projectile.owner)
            {
                bool stealthStrike = Projectile.Calamity().stealthStrike;
                if (stealthStrike)
                {
                    SoundEngine.PlaySound(SoundLoader.GetLegacySoundSlot(Mod, "Sounds/Custom/LightningStrike"), (int)Projectile.position.X, (int)Projectile.position.Y);
                }
                int amt = stealthStrike ? 5 : 1;
                float damageMult = stealthStrike ? 0.5f : 1f;
                for (int n = 0; n < amt; n++)
                {
                    Vector2 spawnPoint = new Vector2(Projectile.Center.X + (float)Main.rand.Next(-100, 101), Projectile.Center.Y - (float)Main.rand.Next(700, 801));
                    float randomVelocity = Main.rand.NextFloat() - 0.5f;
                    Vector2 fireTo = new Vector2(spawnPoint.X + 100f * randomVelocity, spawnPoint.Y + 900);
                    Vector2 ai0 = fireTo - spawnPoint;
                    float ai = (float)Main.rand.Next(100);
                    Vector2 velocity = Vector2.Normalize(ai0.RotatedByRandom(0.78539818525314331)) * 9f;
                    int proj = Projectile.NewProjectile(Projectile.GetProjectileSource_FromThis(), spawnPoint.X, spawnPoint.Y, velocity.X, velocity.Y, ProjectileID.CultistBossLightningOrbArc, (int)(Projectile.damage * damageMult), Projectile.knockBack, Projectile.owner, ai0.ToRotation(), ai);
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

        public override bool PreDraw(ref Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], lightColor, 1);
            return false;
        }
    }
}
