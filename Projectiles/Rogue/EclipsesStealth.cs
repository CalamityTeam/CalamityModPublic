using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Rogue
{
    public class EclipsesStealth : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Rogue";
        public override string Texture => "CalamityMod/Items/Weapons/Rogue/EclipsesFall";

        // For more consistent DPS, always alternates between spawning 1 and 2 spears instead of picking randomly
        private bool spawnTwoSpears = true;
        private bool changedTimeLeft = false;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.width = 25;
            Projectile.height = 25;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 300;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.DamageType = RogueDamageClass.Instance;
        }

        // Uses localAI[1] to decide how many frames until the next spear drops.
        public override void AI()
        {
            // Behavior when not sticking to anything
            if (Projectile.ai[0] == 0f)
            {
                // Keep the spear oriented in the correct direction
                Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver4;

                // Spawn dust while flying
                if (Main.rand.NextBool(8))
                    Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, 138, Projectile.velocity.X * 0.5f, Projectile.velocity.Y * 0.5f);
            }

            // Behavior when having impaled a target
            else
            {
                // Eclipse's Fall is guaranteed to impale for 10 seconds, no more, no less
                if (!changedTimeLeft)
                {
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
                        Projectile.localAI[1] = Main.rand.Next(8, 14); // 8 to 13 frames between each spearfall

                        int type = ModContent.ProjectileType<EclipsesSmol>();
                        int smolDamage = (int)(Projectile.damage * 0.22f);
                        float smolKB = 3f;
                        // Used to be a 50% chance each spearfall for 1 or 2. Now is consistent.
                        int numSpears = spawnTwoSpears ? 2 : 1;
                        spawnTwoSpears = !spawnTwoSpears;
                        for (int i = 0; i < numSpears; ++i)
                            CalamityUtils.ProjectileRain(source, Projectile.Center, 400f, 100f, 500f, 800f, 29f, type, smolDamage, smolKB, Projectile.owner);
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
            CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], lightColor, 1);
            return false;
        }

        public override bool? CanHitNPC(NPC target)
        {
            if (Projectile.ai[0] == 1f)
            {
                return false;
            }
            return null;
        }

        public override bool CanHitPvp(Player target) => Projectile.ai[0] != 1f;
    }
}
