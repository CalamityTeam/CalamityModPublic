using CalamityMod.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Rogue
{
    public class EclipsesStealth : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Rogue";
        public override string Texture => "CalamityMod/Items/Weapons/Rogue/EclipsesFall";

        // these also affect KB
        public const float RainDamageMult = 0.2f;
        public const float ExplosionDamageMult = 0.5f;

        // For more consistent DPS, always alternates between spawning 1 and 2 spears instead of picking randomly
        private bool spawnTwoSpears = true;
        private bool changedTimeLeft = false;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 6;
            ProjectileID.Sets.TrailingMode[Type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 40;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;            
            Projectile.DamageType = RogueDamageClass.Instance;
            Projectile.MaxUpdates = 2;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.timeLeft = 150 * Projectile.MaxUpdates;
        }

        // Uses localAI[1] to decide how many frames until the next spear drops.
        public override void AI()
        {
            Lighting.AddLight(Projectile.Center, 1f, 0.8f, 0.3f);
            // Behavior when not sticking to anything
            if (Projectile.ai[0] == 0f)
            {
                // Keep the spear oriented in the correct direction
                Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver4;
                if (Main.rand.NextBool(5))
                {
                    Vector2 trailPos = Projectile.Center + Vector2.UnitY.RotatedBy(Projectile.rotation) * Main.rand.NextFloat(-16f, 16f);
                    float trailScale = Main.rand.NextFloat(0.8f, 1.2f);
                    Color trailColor = Main.rand.NextBool() ? Color.Indigo : Color.DarkOrange;
                    Particle eclipseTrail = new SparkParticle(trailPos, Projectile.velocity * 0.2f, false, 60, trailScale, trailColor);
                    GeneralParticleHandler.SpawnParticle(eclipseTrail);
                }
            }

            // Behavior when having impaled a target
            else
            {
                // Eclipse's Fall is guaranteed to impale for 10 seconds, no more, no less
                if (!changedTimeLeft)
                {
                    Projectile.MaxUpdates = 1;
                    Projectile.timeLeft = 600;
                    changedTimeLeft = true;
                }

                // Spawn spears. As this uses local AI, it's done client-side only.
                if (Projectile.owner == Main.myPlayer)
                {
                    Projectile.localAI[1] -= 1f;

                    var source = Projectile.GetSource_FromThis();
                    if (Projectile.localAI[1] <= 0f)
                    {
                        // Set up the spear counter for next time. Used to be every 5 frames there was a 50% chance; now it's more reliable but slower.
                        Projectile.localAI[1] = Main.rand.Next(10, 13); // 10 to 12 frames between each spearfall

                        int type = ModContent.ProjectileType<EclipsesSmol>();
                        // Used to be a 50% chance each spearfall for 1 or 2. Now is consistent.
                        int numSpears = spawnTwoSpears ? 2 : 1;
                        spawnTwoSpears = !spawnTwoSpears;
                        for (int i = 0; i < numSpears; ++i)
                            CalamityUtils.ProjectileRain(source, Projectile.Center, 400f, 100f, 500f, 800f, 29f, type, (int)(Projectile.damage * RainDamageMult), Projectile.knockBack * RainDamageMult, Projectile.owner);
                    }
                }
            }

            // Sticky behavior. Lets the projectile impale enemies and sticks to its impaled enemy automatically.
            Projectile.StickyProjAI(10);
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers) => Projectile.ModifyHitNPCSticky(1);

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (targetHitbox.Width > 8 && targetHitbox.Height > 8)
            {
                targetHitbox.Inflate(-targetHitbox.Width / 8, -targetHitbox.Height / 8);
            }
            return null;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Type], lightColor, 1);
            return false;
        }

        public override void PostDraw(Color lightColor)
        {
            Texture2D glow = ModContent.Request<Texture2D>(Texture + "Glow").Value;
            Main.EntitySpriteDraw(glow, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation, glow.Size() * 0.5f, Projectile.scale, SpriteEffects.None);
        }

        public override bool? CanHitNPC(NPC target) => Projectile.ai[0] == 1f ? false : base.CanHitNPC(target);

        public override bool CanHitPvp(Player target) => Projectile.ai[0] != 1f;

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item14, Projectile.Center);
            if (Main.myPlayer == Projectile.owner)
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<EclipseStealthBoom>(), (int)(Projectile.damage * ExplosionDamageMult), Projectile.knockBack * ExplosionDamageMult, Projectile.owner);
        }
    }
}
