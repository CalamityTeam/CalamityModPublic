using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Projectiles.Melee;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Rogue
{
    public class BloodsoakedCrashax : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Rogue";
        public override string Texture => "CalamityMod/Items/Weapons/Rogue/BloodsoakedCrasher";

        private int bounce = 3; //number of times it bounces
        private int grind = 0; //used to know when to slow down
        private const float MaxSpeed = 14f;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 30;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.penetrate = 6;
            Projectile.timeLeft = 600; //10 seconds and counting (but not actually because extra updates)
            Projectile.DamageType = RogueDamageClass.Instance;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 8;
            Projectile.extraUpdates = 1;
        }

        public override void AI()
        {
            float speed = Projectile.velocity.Length();
            if (grind > 0)
            {
                grind--;
                // Suddenly stop when on top of enemies.
                Projectile.velocity.X *= 0.75f;
                Projectile.velocity.Y *= 0.75f;
            }
            else
            {
                // Gravity
                Projectile.velocity.Y += 0.11f;

                // Cap velocity.
                speed = Projectile.velocity.Length();
                if (speed > MaxSpeed)
                    Projectile.velocity *= MaxSpeed / speed;
            }

            // Spin constantly, but even faster when grinding or going fast
            float spinRate = grind > 0 ? 0.28f : 0.09f;
            if (grind <= 0)
                spinRate += speed * 0.005f;
            Projectile.rotation += spinRate * Projectile.direction;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            bounce--;
            if (bounce <= 0)
            {
                Projectile.Kill(); //you can only bounce so much 'til death
            }
            else
            {
                if (Projectile.velocity.X != oldVelocity.X)
                {
                    Projectile.velocity.X = -oldVelocity.X;
                }
                if (Projectile.velocity.Y != oldVelocity.Y)
                {
                    Projectile.velocity.Y = -oldVelocity.Y;
                }
            }
            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            OnHitEffects(!target.canGhostHeal || Main.player[Projectile.owner].moonLeech);
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            OnHitEffects(Main.player[Projectile.owner].moonLeech);
        }

        private void OnHitEffects(bool cannotLifesteal)
        {
            grind += 5; //THE GRIND NEVER STOPS
            if (grind > 15)
                grind = 15; // except when it's too much

            if (Projectile.Calamity().stealthStrike && Projectile.owner == Main.myPlayer) //stealth strike attack
            {
                int projID = ModContent.ProjectileType<Blood>();
                int bloodDamage = Projectile.damage;
                float bloodKB = 1f;
                int stealth = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, projID, bloodDamage, bloodKB, Projectile.owner, 1f, 0.85f + Main.rand.NextFloat() * 1.15f);
                if (stealth.WithinBounds(Main.maxProjectiles))
                {
                    Main.projectile[stealth].DamageType = RogueDamageClass.Instance;
                    Main.projectile[stealth].extraUpdates = 1;
                }
            }

            if (cannotLifesteal || Main.rand.NextBool()) //canGhostHeal be like lol
                return;

            Player player = Main.player[Projectile.owner];
            player.statLife += 1;
            player.HealEffect(1);
        }

        public override bool PreDraw(ref Color lightColor) //afterimages
        {
            CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], lightColor, 1);
            return false;
        }
    }
}
