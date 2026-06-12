using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.CalPlayer;
using CalamityMod.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Rogue
{
    public class SupernovaBoom : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Rogue";
        public int frameX = 0;
        public int frameY = 0;
        private const int horizontalFrames = 5;
        private const int verticalFrames = 4;
        private const int frameLength = 2;
        private const float radius = 300f;
        public bool damageFrame = false;

        public Color variedColor = Color.White;
        public Color mainColor = Color.LawnGreen;
        public Color randomColor = Color.White;
        public int colorTimer = 0;
        public int time = 0;
        public int currentFrame = 1;

        private Player Owner => Main.player[Projectile.owner];

        public override void SetDefaults()
        {
            Projectile.width = 408;
            Projectile.height = 410;
            Projectile.scale = 1f;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.DamageType = RogueDamageClass.Instance;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }

        public override void AI()
        {
            randomColor = Main.rand.Next(4) switch
            {
                0 => Color.Red,
                1 => Color.MediumTurquoise,
                2 => Color.Orange,
                _ => Color.LawnGreen,
            };
            if (time == 0)
                mainColor = randomColor;

            if (time % 20 == 0)
            {
                variedColor = colorTimer switch
                {
                    0 => Color.Red,
                    1 => Color.MediumTurquoise,
                    2 => Color.Orange,
                    _ => Color.LawnGreen,
                };
                colorTimer++;
                if (colorTimer >= 4)
                    colorTimer = 0;
            }

            mainColor = Color.Lerp(mainColor, variedColor, 0.07f);

            Lighting.AddLight(Projectile.Center, mainColor.ToVector3() * 3);

            Projectile.frameCounter++;
            if (Projectile.frameCounter % frameLength == frameLength - 1)
            {
                currentFrame++;

                frameY++;
                if (frameY >= verticalFrames)
                {
                    frameX++;
                    frameY = 0;
                }
                if (frameX >= horizontalFrames)
                {
                    Projectile.Kill();
                }
            }
            if (currentFrame == 13)
                damageFrame = true;
            else
                damageFrame = false;

            if (currentFrame == 4)
            {
                float numberOfDusts = 20f;
                float rotFactor = 360f / numberOfDusts;
                for (int i = 0; i < numberOfDusts; i++)
                {
                    randomColor = Main.rand.Next(4) switch
                    {
                        0 => Color.Red,
                        1 => Color.MediumTurquoise,
                        2 => Color.Orange,
                        _ => Color.LawnGreen,
                    };

                    float rot = MathHelper.ToRadians(i * rotFactor);
                    Vector2 velOffset = CalamityUtils.RandomVelocity(100f, 70f, 150f, 0.04f);
                    velOffset *= Main.rand.NextFloat(25, 45);
                    SquishyLightParticle exoEnergy = new(Projectile.Center + velOffset * 2.5f, -velOffset * Main.rand.NextFloat(0.08f, 0.12f), Main.rand.NextFloat(0.1f, 0.2f), randomColor, 9);
                    GeneralParticleHandler.SpawnParticle(exoEnergy);
                }
            }
            if (currentFrame == 13)
            {
                Projectile.velocity = Vector2.Zero;
                Owner.SetScreenshake(5f);

                int points = 5;
                float radians = MathHelper.TwoPi / points;
                Vector2 spinningPoint = Vector2.Normalize(new Vector2(-1f, -1f));
                float rotRando = Main.rand.NextFloat(0.1f, 2.5f);
                for (int k = 0; k < points; k++)
                {
                    Vector2 velocity = spinningPoint.RotatedBy(radians * k).RotatedBy(-0.45f * rotRando);
                    SparkParticle subTrail = new(Projectile.Center + velocity * 7.5f, velocity, false, 4, 1.65f, Color.White);
                    GeneralParticleHandler.SpawnParticle(subTrail);
                }

                for (int i = 0; i < 30; i++)
                {
                    Vector2 randVel = new Vector2(15, 15).RotatedByRandom(100) * Main.rand.NextFloat(0.8f, 1.6f);
                    Particle smoke = new HeavySmokeParticle(Projectile.Center + randVel, randVel, new Color(57, 46, 115) * 0.9f, Main.rand.Next(25, 35 + 1), Main.rand.NextFloat(0.9f, 2.3f), 0.4f);
                    GeneralParticleHandler.SpawnParticle(smoke);
                }
                
                float numberOflines = 45;
                float rotFactorlines = 360f / numberOflines;
                for (int e = 0; e < numberOflines; e++)
                {
                    randomColor = Main.rand.Next(4) switch
                    {
                        0 => Color.Red,
                        1 => Color.MediumTurquoise,
                        2 => Color.Orange,
                        _ => Color.LawnGreen,
                    };

                    float rot = MathHelper.ToRadians(e * rotFactorlines);
                    Vector2 offset = (Vector2.UnitX * Main.rand.NextFloat(0.2f, 3.1f)).RotatedBy(rot + Main.rand.NextFloat(0.1f, 5.1f));
                    Vector2 velOffset = (Vector2.UnitX * Main.rand.NextFloat(0.2f, 3.1f)).RotatedBy(rot + Main.rand.NextFloat(0.1f, 5.1f));
                    SparkParticle spark = new SparkParticle(Projectile.Center + offset, velOffset * Main.rand.NextFloat(15.5f, 25.5f), true, 95, Main.rand.NextFloat(0.3f, 1.1f), Color.Lerp(Color.White, randomColor, 0.3f));
                    GeneralParticleHandler.SpawnParticle(spark);
                }

            }
            if (currentFrame < 13) // suck in enemies
            {
                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    NPC target = Main.npc[i];
                    if (target.CanBeMoved() && target.CanBeChasedBy(Projectile, false))
                    {
                        if (Vector2.Distance(target.Center, Projectile.Center) > 40 && Vector2.Distance(target.Center, Projectile.Center) < 600)
                        {
                            target.Center += target.Center.DirectionTo(Projectile.Center).SafeNormalize(Vector2.UnitX) * 22;
                            target.SyncMotionToServer();
                        }
                    }
                    else if (target != null && CalamityPlayer.areThereAnyDamnBosses)
                    {
                        target = Projectile.Center.ClosestNPCAt(600);
                        if (target != null)
                        {
                            if (Vector2.Distance(target.Center, Projectile.Center) > 5)
                            {
                                if (Vector2.Distance(target.Center, Projectile.Center) < 600)
                                {
                                    Projectile.velocity = Projectile.Center.DirectionTo(target.Center).SafeNormalize(Vector2.UnitX) * 30;
                                }
                            }
                            else
                            {
                                Projectile.velocity = Vector2.Zero;
                                Projectile.Center = target.Center;
                            }
                        }
                    }
                }
            }

            time++;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) => CalamityUtils.CircularHitboxCollision(Projectile.Center, radius, targetHitbox);

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = Terraria.GameContent.TextureAssets.Projectile[Type].Value;
            int length = texture.Width / horizontalFrames;
            int height = texture.Height / verticalFrames;
            Vector2 drawPos = Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY);
            Rectangle frame = new Rectangle(frameX * length, frameY * height, length, height);
            Vector2 origin = new Vector2(length / 2f, height / 2f);
            Main.EntitySpriteDraw(texture, drawPos, frame, Color.White with { A = 0 }, Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Vector2 launchVel = Utils.DirectionTo(Projectile.Center, target.Center);
            target.MoveNPC(launchVel, 30, true, Owner);

            target.AddBuff(ModContent.BuffType<MiracleBlight>(), 90);
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            if (Projectile.numHits > 0)
                Projectile.damage = (int)(Projectile.damage * 0.95f);
            if (Projectile.damage < 1)
                Projectile.damage = 1;
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info) => target.AddBuff(ModContent.BuffType<MiracleBlight>(), 300);

        public override bool? CanDamage() => damageFrame ? null : false;
    }
}
