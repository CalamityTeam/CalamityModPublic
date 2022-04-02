using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Projectiles.Melee;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Rogue
{
    public class BloodsoakedCrashax : ModProjectile
    {
        public override string Texture => "CalamityMod/Items/Weapons/Rogue/BloodsoakedCrasher";

        private int bounce = 3; //number of times it bounces
        private int grind = 0; //used to know when to slow down
        private const float MaxSpeed = 14f;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Bloodsoaked Crasher");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            projectile.width = projectile.height = 30;
            projectile.friendly = true;
            projectile.ignoreWater = true;
            projectile.penetrate = 6;
            projectile.timeLeft = 600; //10 seconds and counting (but not actually because extra updates)
            projectile.Calamity().rogue = true;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 8;
            projectile.extraUpdates = 1;
        }

        public override void AI()
        {
            float speed = projectile.velocity.Length();
            if (grind > 0)
            {
                grind--;
                // Suddenly stop when on top of enemies.
                projectile.velocity.X *= 0.75f;
                projectile.velocity.Y *= 0.75f;
            }
            else
            {
                // Gravity
                projectile.velocity.Y += 0.11f;

                // Cap velocity.
                speed = projectile.velocity.Length();
                if (speed > MaxSpeed)
                    projectile.velocity *= MaxSpeed / speed;
            }

            // Spin constantly, but even faster when grinding or going fast
            float spinRate = grind > 0 ? 0.28f : 0.09f;
            if (grind <= 0)
                spinRate += speed * 0.005f;
            projectile.rotation += spinRate * projectile.direction;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            bounce--;
            if (bounce <= 0)
            {
                projectile.Kill(); //you can only bounce so much 'til death
            }
            else
            {
                if (projectile.velocity.X != oldVelocity.X)
                {
                    projectile.velocity.X = -oldVelocity.X;
                }
                if (projectile.velocity.Y != oldVelocity.Y)
                {
                    projectile.velocity.Y = -oldVelocity.Y;
                }
            }
            return false;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(ModContent.BuffType<BrimstoneFlames>(), 180);
            OnHitEffects(!target.canGhostHeal || Main.player[projectile.owner].moonLeech);
        }

        public override void OnHitPvp(Player target, int damage, bool crit)
        {
            target.AddBuff(ModContent.BuffType<BrimstoneFlames>(), 180);
            OnHitEffects(Main.player[projectile.owner].moonLeech);
        }

        private void OnHitEffects(bool cannotLifesteal)
        {
            grind += 5; //THE GRIND NEVER STOPS
            if (grind > 15)
                grind = 15; // except when it's too much

            if (projectile.Calamity().stealthStrike && projectile.owner == Main.myPlayer) //stealth strike attack
            {
                int projID = ModContent.ProjectileType<Blood>();
                int bloodDamage = projectile.damage;
                float bloodKB = 1f;
                int stealth = Projectile.NewProjectile(projectile.Center, Vector2.Zero, projID, bloodDamage, bloodKB, projectile.owner, 1f, 0.85f + Main.rand.NextFloat() * 1.15f);
                if (stealth.WithinBounds(Main.maxProjectiles))
                {
                    Main.projectile[stealth].Calamity().forceRogue = true;
                    Main.projectile[stealth].extraUpdates = 1;
                }
            }

            if (cannotLifesteal || Main.rand.NextBool(2)) //canGhostHeal be like lol
                return;

            Player player = Main.player[projectile.owner];
            player.statLife += 1;
            player.HealEffect(1);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor) //afterimages
        {
            CalamityUtils.DrawAfterimagesCentered(projectile, ProjectileID.Sets.TrailingMode[projectile.type], lightColor, 1);
            return false;
        }
    }
}
