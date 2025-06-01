using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Ranged
{
    public class BuzzkillSaw : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Ranged";

        public static readonly SoundStyle TileCollideGFB = new("CalamityMod/Sounds/Custom/MetalPipeFalling");

        public ref float SawLevel => ref Projectile.ai[0];
        public ref float Time => ref Projectile.ai[1];

        public static Asset<Texture2D> SmallSlash;
        public static Asset<Texture2D> LargeSlash;

        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 4;
            ProjectileID.Sets.TrailCacheLength[Type] = 6;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 40;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.timeLeft = 480;
            Projectile.penetrate = 1; // Saw pierce is set when the saw is spawned, due to it being dynamic based on charge.
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
            Projectile.Calamity().pointBlankShotDuration = CalamityGlobalProjectile.DefaultPointBlankDuration;
        }

        public override void AI()
        {
            // dies from cringe (Deadshot Brooch moment)
            if (Projectile.MaxUpdates > 1)
                Projectile.MaxUpdates = 1;

            Time++;
            Projectile.rotation += MathHelper.ToRadians(6f + 18f * SawLevel);

            if (Projectile.frame < 1)
                Projectile.frame = 1;
            Projectile.frameCounter++;
            if (Projectile.frameCounter >= 3)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
                if (Projectile.frame > 3)
                    Projectile.frame = 1;
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            int sparkCount = 6 + 5 * (int)SawLevel;
            for (int s = 0; s < sparkCount; s++)
            {
                Vector2 sparkVelocity = new Vector2();
                if (Projectile.velocity.X != oldVelocity.X && oldVelocity.X < 0)
                    sparkVelocity = Vector2.UnitX * 6.5f;
                else if (Projectile.velocity.X != oldVelocity.X && oldVelocity.X >= 0)
                    sparkVelocity = Vector2.UnitX * -6.5f;
                else if (Projectile.velocity.Y != oldVelocity.Y && oldVelocity.Y < 0)
                    sparkVelocity = Vector2.UnitY * 6.5f;
                else if (Projectile.velocity.Y != oldVelocity.Y && oldVelocity.Y >= 0)
                    sparkVelocity = Vector2.UnitY * -6.5f;

                Vector2 sparkLocation = sparkVelocity.X > 0f ? Projectile.Left : (sparkVelocity.X < 0f ? Projectile.Right : (sparkVelocity.Y > 0f ? Projectile.Top : Projectile.Bottom));
                sparkVelocity = sparkVelocity.RotatedByRandom(MathHelper.PiOver2) * (Main.rand.NextFloat(0.8f, 1.2f) + (Main.rand.NextFloat(0.2f, 0.6f) * SawLevel));
                float scale = Main.rand.NextFloat(0.5f, 0.8f) + Main.rand.NextFloat(0.2f, 0.6f) * SawLevel;
                Particle collisionSparks = new AltLineParticle(sparkLocation, sparkVelocity, false, 30, scale, new Color(250, 250, 107));
                GeneralParticleHandler.SpawnParticle(collisionSparks);
            }

            Projectile.penetrate--;
            Projectile.numHits++;
            if (Projectile.penetrate <= 0)
            {
                Projectile.Kill();
            }
            else
            {
                SoundEngine.PlaySound(Main.zenithWorld ? TileCollideGFB : SoundID.Item178 with { Pitch = 0.15f * Projectile.numHits }, Projectile.Center); // Placeholder sound

                if (Projectile.velocity.X != oldVelocity.X)
                    Projectile.velocity.X = -oldVelocity.X;
                if (Projectile.velocity.Y != oldVelocity.Y)
                    Projectile.velocity.Y = -oldVelocity.Y;
            }

            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<HeavyBleeding>(), 150);
            SoundEngine.PlaySound(new SoundStyle("CalamityMod/Sounds/Custom/SwiftSlice") with { Pitch = 0.15f * Projectile.numHits }, Projectile.Center);

            int bloodCount = 6 + 10 * (int)SawLevel;
            for (int p = 0; p < bloodCount; p++)
            {
                Vector2 velocity = Projectile.velocity.RotatedByRandom(MathHelper.ToRadians(30f)) * (Main.rand.NextFloat(0.4f, 0.6f) + (Main.rand.NextFloat(0.2f, 0.6f) * SawLevel));
                float scale = Main.rand.NextFloat(0.5f, 0.8f) + Main.rand.NextFloat(0.2f, 0.8f) * SawLevel;
                Particle hitSparks = new AltLineParticle(target.Center, velocity, false, 30, scale, new Color(112, 16, 16));
                GeneralParticleHandler.SpawnParticle(hitSparks);
            }
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(new SoundStyle("CalamityMod/Sounds/Custom/CeramicImpact", 2), Projectile.Center);

            // TODO - Change this dust
            for (int d = 0; d < 8; d++)
            {
                Vector2 dustVel = Main.rand.NextVector2CircularEdge(1f, 1f);
                dustVel.SafeNormalize(Vector2.Zero);
                dustVel *= Main.rand.NextFloat(5f, 9f);

                Dust collisionDust = Dust.NewDustPerfect(Projectile.Center, 84, dustVel);
                collisionDust.noGravity = true;
            }

            int goreToExclude = Main.rand.Next(3);
            switch (goreToExclude)
            {
                case 0:
                    Gore.NewGore(Projectile.GetSource_FromThis(), Projectile.Center, Main.rand.NextVector2Circular(4f, 4f), Mod.Find<ModGore>("BuzzkillSaw2").Type, 0.8f);
                    Gore.NewGore(Projectile.GetSource_FromThis(), Projectile.Center, Main.rand.NextVector2Circular(4f, 4f), Mod.Find<ModGore>("BuzzkillSaw3").Type, 0.8f);
                    break;
                case 1:
                    Gore.NewGore(Projectile.GetSource_FromThis(), Projectile.Center, Main.rand.NextVector2Circular(4f, 4f), Mod.Find<ModGore>("BuzzkillSaw1").Type, 0.8f);
                    Gore.NewGore(Projectile.GetSource_FromThis(), Projectile.Center, Main.rand.NextVector2Circular(4f, 4f), Mod.Find<ModGore>("BuzzkillSaw3").Type, 0.8f);
                    break;
                case 2:
                    Gore.NewGore(Projectile.GetSource_FromThis(), Projectile.Center, Main.rand.NextVector2Circular(4f, 4f), Mod.Find<ModGore>("BuzzkillSaw1").Type, 0.8f);
                    Gore.NewGore(Projectile.GetSource_FromThis(), Projectile.Center, Main.rand.NextVector2Circular(4f, 4f), Mod.Find<ModGore>("BuzzkillSaw2").Type, 0.8f);
                    break;
            }
        }

        public override void ModifyDamageHitbox(ref Rectangle hitbox)
        {
            if (SawLevel >= 2f)
                hitbox.Inflate(65, 65);
            else if (SawLevel >= 1f)
                hitbox.Inflate(28, 28);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            LargeSlash ??= ModContent.Request<Texture2D>("CalamityMod/Projectiles/Ranged/BuzzkillSawLargeSlash");
            Texture2D largeSlashTexture = LargeSlash.Value;
            SmallSlash ??= ModContent.Request<Texture2D>("CalamityMod/Projectiles/Ranged/BuzzkillSawSmallSlash");
            Texture2D smallSlashTexture = SmallSlash.Value;
            Color slashColor = new Color(200, 200, 200, 100);

            if (SawLevel >= 2f)
            {
                Main.EntitySpriteDraw(largeSlashTexture, Projectile.Center - Main.screenPosition, null, slashColor, -Projectile.rotation, largeSlashTexture.Size() * 0.5f, 1f, SpriteEffects.None);

                if (Time % 4 == 0)
                {
                    Vector2 randomParticleOffset = new Vector2(Main.rand.NextFloat(-Projectile.width * 1.75f, Projectile.width * 1.75f), Main.rand.NextFloat(-Projectile.width * 1.75f, Projectile.width * 1.75f));
                    float randomParticleScale = Main.rand.NextFloat(0.65f, 0.95f);
                    Particle bloomCircle = new BloomParticle(Projectile.Center + randomParticleOffset, Projectile.velocity, Main.rand.NextBool() ? Color.White : new Color(112, 16, 16), randomParticleScale, randomParticleScale, 4, false);
                    GeneralParticleHandler.SpawnParticle(bloomCircle);
                }
            }
            if (SawLevel >= 1f)
            {
                Main.EntitySpriteDraw(smallSlashTexture, Projectile.Center - Main.screenPosition, null, slashColor, Projectile.rotation, smallSlashTexture.Size() * 0.5f, 1f, SpriteEffects.None);

                if (Time % 4 == 0)
                {
                    Vector2 randomParticleOffset = new Vector2(Main.rand.NextFloat(-Projectile.width, Projectile.width), Main.rand.NextFloat(-Projectile.width, Projectile.width));
                    float randomParticleScale = Main.rand.NextFloat(0.35f, 0.65f);
                    Particle bloomCircle = new BloomParticle(Projectile.Center + randomParticleOffset, Projectile.velocity, Main.rand.NextBool() ? Color.White : new Color(112, 16, 16), randomParticleScale, randomParticleScale, 4, false);
                    GeneralParticleHandler.SpawnParticle(bloomCircle);
                }
            }

            if (!CalamityConfig.Instance.Afterimages)
                return true;

            // Special afterimage drawing to include the slashes
            Texture2D buzzsawTexture = TextureAssets.Projectile[Type].Value;
            Rectangle frame = buzzsawTexture.Frame(1, Main.projFrames[Type], 0, Projectile.frame);
            for (int i = 1; i < Projectile.oldPos.Length; i++)
            {
                float afterimageRot = Projectile.oldRot[i];
                Vector2 drawPos = Projectile.oldPos[i] + frame.Size() * 0.5f - Main.screenPosition;
                float intensity = MathHelper.Lerp(0.1f, 0.6f, 1f - i / (float)Projectile.oldPos.Length);

                Main.EntitySpriteDraw(buzzsawTexture, drawPos, frame, lightColor * intensity, afterimageRot, frame.Size() * 0.5f, 1f, SpriteEffects.None);

                if (SawLevel >= 2f)
                    Main.EntitySpriteDraw(largeSlashTexture, drawPos, null, slashColor * intensity, -afterimageRot, largeSlashTexture.Size() * 0.5f, 1f, SpriteEffects.None);
                if (SawLevel >= 1f)
                    Main.EntitySpriteDraw(smallSlashTexture, drawPos, null, slashColor * intensity, afterimageRot, smallSlashTexture.Size() * 0.5f, 1f, SpriteEffects.None);
            }
            return true;
        }
    }
}
