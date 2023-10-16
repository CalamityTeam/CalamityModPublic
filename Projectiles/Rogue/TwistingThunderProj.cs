using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using CalamityMod.Sounds;

namespace CalamityMod.Projectiles.Rogue
{
    public class TwistingThunderProj : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Rogue";
        private bool playedSound = false;

        public override void SetStaticDefaults()
        {
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
            Projectile.DamageType = RogueDamageClass.Instance;
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
                SoundEngine.PlaySound(SoundID.Item92, Projectile.position); //electrosphere launcher fire sound
                playedSound = true;
            }
            if (Main.rand.NextBool(8))
            {
                Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, 132, Projectile.velocity.X * 0.5f, Projectile.velocity.Y * 0.5f);
            }
            Projectile.spriteDirection = Projectile.direction = (Projectile.velocity.X > 0).ToDirectionInt();
            Projectile.rotation = Projectile.velocity.ToRotation() + (Projectile.spriteDirection == 1 ? 0f : MathHelper.Pi);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.Electrified, 180);
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(BuffID.Electrified, 180);
        }

        public override void OnKill(int timeLeft)
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
                    SoundEngine.PlaySound(CommonCalamitySounds.LightningSound, Projectile.position);
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
                    int proj = Projectile.NewProjectile(Projectile.GetSource_FromThis(), spawnPoint.X, spawnPoint.Y, velocity.X, velocity.Y, ProjectileID.CultistBossLightningOrbArc, (int)(Projectile.damage * damageMult), Projectile.knockBack, Projectile.owner, ai0.ToRotation(), ai);
                    Main.projectile[proj].extraUpdates += 9;
                    //Does not force to Rogue because lightning is extremely abusable with Moonstone Crown
                    Main.projectile[proj].friendly = true;
                    Main.projectile[proj].hostile = false;
                    Main.projectile[proj].tileCollide = false;
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
