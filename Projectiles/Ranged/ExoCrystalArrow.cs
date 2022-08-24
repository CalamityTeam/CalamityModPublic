using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using CalamityMod.Sounds;
using CalamityMod.Particles;
using Terraria.Graphics.Shaders;
using Microsoft.Xna.Framework.Graphics;
using CalamityMod.Items.Weapons.Ranged;

namespace CalamityMod.Projectiles.Ranged
{
    public class ExoCrystalArrow : ModProjectile
    {
        public PrimitiveTrail PierceAfterimageDrawer = null;

        public bool CreateLightning => Projectile.ai[0] == 1f;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Exo Crystal");
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 10;
        }

        public override void SetDefaults()
        {
            Projectile.width = 26;
            Projectile.height = 26;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 300;
            Projectile.arrow = true;
            Projectile.Calamity().pointBlankShotDuration = CalamityGlobalProjectile.DefaultPointBlankDuration;
        }

        public override void AI()
        {
            // Fade in.
            Projectile.Opacity = Utils.GetLerpValue(300f, 297f, Projectile.timeLeft, true);

            // Rapidly race towards the nearest target.
            NPC potentialTarget = Projectile.Center.ClosestNPCAt(HeavenlyGale.ArrowTargetingRange);
            if (potentialTarget != null && Projectile.timeLeft < 290)
            {
                Vector2 idealVelocity = Projectile.SafeDirectionTo(potentialTarget.Center) * 33f;
                Projectile.velocity = (Projectile.velocity * 29f + idealVelocity) / 30f;
                Projectile.velocity = Projectile.velocity.MoveTowards(idealVelocity, 3f);
            }

            Projectile.rotation = Projectile.velocity.ToRotation();
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Collision.HitTiles(Projectile.position, Projectile.velocity, Projectile.width, Projectile.height);
            SoundEngine.PlaySound(SoundID.Dig, Projectile.position);
            return true;
        }

        public float PrimitiveWidthFunction(float completionRatio) => Projectile.scale * 30f;

        public Color PrimitiveColorFunction(float _) => Color.Lime * Projectile.Opacity;

        public override bool PreDraw(ref Color lightColor)
        {
            PierceAfterimageDrawer ??= new(PrimitiveWidthFunction, PrimitiveColorFunction, null, GameShaders.Misc["CalamityMod:HeavenlyGaleTrail"]);

            Color mainColor = CalamityUtils.MulticolorLerp((Main.GlobalTimeWrappedHourly * 2f) % 1, Color.Cyan, Color.Lime, Color.GreenYellow, Color.Goldenrod, Color.Orange);
            Color secondaryColor = CalamityUtils.MulticolorLerp((Main.GlobalTimeWrappedHourly * 2f + 0.2f) % 1, Color.Cyan, Color.Lime, Color.GreenYellow, Color.Goldenrod, Color.Orange);

            mainColor = Color.Lerp(Color.White, mainColor, 0.85f);
            secondaryColor = Color.Lerp(Color.White, secondaryColor, 0.85f);

            Vector2 trailOffset = Projectile.Size * 0.5f - Main.screenPosition;
            GameShaders.Misc["CalamityMod:HeavenlyGaleTrail"].SetShaderTexture(ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/EternityStreak"));
            GameShaders.Misc["CalamityMod:HeavenlyGaleTrail"].UseImage2("Images/Extra_189");
            GameShaders.Misc["CalamityMod:HeavenlyGaleTrail"].UseColor(mainColor);
            GameShaders.Misc["CalamityMod:HeavenlyGaleTrail"].UseSecondaryColor(secondaryColor);
            GameShaders.Misc["CalamityMod:HeavenlyGaleTrail"].Apply();
            PierceAfterimageDrawer.Draw(Projectile.oldPos, trailOffset, 53);

            Texture2D tex = ModContent.Request<Texture2D>(Texture).Value;
            Vector2 origin = tex.Size() * 0.5f;
            Vector2 drawPosition = Projectile.Center - Main.screenPosition;
            Main.spriteBatch.Draw(tex, drawPosition, null, Projectile.GetAlpha(Color.White), Projectile.rotation, origin, Projectile.scale, 0, 0f);

            return false;
        }

        public override void Kill(int timeLeft)
        {
            // Play a loud impact sound.
            SoundEngine.PlaySound(CommonCalamitySounds.LargeWeaponFireSound with { Volume = 0.2f }, Projectile.Center);

            // Explode into a bunch of exo particles.
            for (int i = 0; i < 2; i++)
            {
                Vector2 energyVelocity = Projectile.velocity.SafeNormalize(Vector2.UnitY).RotatedByRandom(0.6f) * Main.rand.NextFloat(7f, 11f);
                Color exoEnergyColor = CalamityUtils.MulticolorLerp(Main.rand.NextFloat(), CalamityUtils.ExoPalette);
                SparkParticle exoEnergy = new(Projectile.Center, energyVelocity, false, 30, 1.3f, exoEnergyColor);
                GeneralParticleHandler.SpawnParticle(exoEnergy);
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            // Create lightning from above if necessary.
            if (!CreateLightning)
                return;

            int lightningDamage = (int)(Projectile.damage * HeavenlyGale.LightningDamageFactor);
            Vector2 lightningSpawnPosition = target.Center - Vector2.UnitY.RotatedByRandom(0.24f) * Main.rand.NextFloat(960f, 1020f);
            Vector2 lightningShootVelocity = (target.Center - lightningSpawnPosition).SafeNormalize(Vector2.UnitY) * 13f;
            int lightning = Projectile.NewProjectile(Projectile.GetSource_OnHit(target), lightningSpawnPosition, lightningShootVelocity, ModContent.ProjectileType<ExoLightningBolt>(), lightningDamage, 0f, Projectile.owner);
            if (Main.projectile.IndexInRange(lightning))
            {
                Main.projectile[lightning].ai[0] = lightningShootVelocity.ToRotation();
                Main.projectile[lightning].ai[1] = Main.rand.Next(100);
            }
        }
    }
}
