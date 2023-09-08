using CalamityMod.Projectiles.Melee;
using Microsoft.Xna.Framework;
using Mono.Cecil;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Rogue
{
    public class PhantasmalRuinProj : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Rogue";
        public override string Texture => "CalamityMod/Items/Weapons/Rogue/PhantasmalRuin";
        public static readonly SoundStyle HitSound = new("CalamityMod/Sounds/Item/WulfrumKnifeThrowSingle") { Volume = 0.8f};
        private const int Lifetime = 600;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 12;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 1;
        }

        public override void SetDefaults()
        {
            Projectile.width = 30;
            Projectile.height = 30;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.penetrate = 1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = Lifetime;
            Projectile.extraUpdates = 2;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 20 * Projectile.MaxUpdates;
            Projectile.DamageType = RogueDamageClass.Instance;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], lightColor, 3);
            return false;
        }

        public override void AI()
        {
            // Set the projectile's direction correctly
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver4;

            // Dust and light
            Dust d = Dust.NewDustDirect(Projectile.position + Projectile.velocity, Projectile.width - (Projectile.Calamity().stealthStrike ? 6 : 0), Projectile.height - (Projectile.Calamity().stealthStrike ? 6 : 0), Projectile.Calamity().stealthStrike ? 132 : 180, Projectile.velocity.X * -0.8f, Projectile.velocity.Y * -0.8f, 0, default, Projectile.Calamity().stealthStrike ? 1.2f : 0.8f);
            d.noLight = true;
            d.noGravity = true;
            Lighting.AddLight(Projectile.Center + Projectile.velocity * 0.2f, 0.2f, 0.7f, 0.9f);

            if (Projectile.Calamity().stealthStrike)
            {
                Projectile.extraUpdates = 3;
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => OnHitEffects();
        public override void OnHitPlayer(Player target, Player.HurtInfo info) => OnHitEffects();

        private void OnHitEffects()
        {
            if (Projectile.owner != Main.myPlayer)
                return;

            SoundEngine.PlaySound(HitSound with { PitchVariance = 0.4f }, Projectile.position);

            if (Projectile.Calamity().stealthStrike)
            {
                for (int i = 0; i < 5; i += 2)
                {
                    int soulDamage = (int)(Projectile.damage * 0.3f);
                    Vector2 velocity = new Vector2(0f, -15f);
                    velocity = velocity.RotatedByRandom(0.5f);
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center + new Vector2(0, 600f), velocity, ModContent.ProjectileType<PhantasmalSoulBlue>(), soulDamage, 0f, Projectile.owner, 0f);
                }
            }
            else
            {
                int numSouls = 4;
                int projID = ModContent.ProjectileType<PhantasmalSoulBlue>();
                int soulDamage = (int)(Projectile.damage * 0.2f);
                float soulKB = 0f;
                float speed = 4f;
                float startAngle = Main.rand.NextFloat(-0.07f, 0.07f) + MathHelper.PiOver4;
                Vector2 velocity = (Vector2.UnitX * speed).RotatedBy(startAngle);
                for (int i = 0; i < numSouls; i += 2)
                {
                    Vector2 velocityrandom = velocity.RotatedByRandom(1.5f);
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, velocity + velocityrandom, projID, soulDamage, soulKB, Projectile.owner, 0f);
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, -velocity - velocityrandom, projID, soulDamage, soulKB, Projectile.owner, 0f);
                    // Rotate direction for the next pair of souls.
                    velocity = velocity.RotatedBy(MathHelper.TwoPi / numSouls);
                }
                for (int i = 0; i < 8; i += 2)
                {
                    // d, du, dus, dust :)
                    Dust du = Dust.NewDustPerfect(Projectile.Center, 180, velocity, 0, default, Main.rand.NextFloat(1.1f, 1.4f));
                    Dust dus = Dust.NewDustPerfect(Projectile.Center, 180, -velocity, 0, default, Main.rand.NextFloat(1.1f, 1.4f));
                    du.noGravity = true;
                    dus.noGravity = true;
                    // Rotate direction for the next dust.
                    velocity = velocity.RotatedBy(MathHelper.TwoPi / numSouls);
                }
            }
        }
    }
}
