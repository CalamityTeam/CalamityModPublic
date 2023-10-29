using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;

namespace CalamityMod.Projectiles.Magic
{
    public class NebulaCloudCore : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Magic";
        private const float IntendedVelocity = 4f;

        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 4;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 4;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.width = 32;
            Projectile.height = 32;
            Projectile.friendly = true;
            Projectile.alpha = 255;
            Projectile.penetrate = 4;
            Projectile.ignoreWater = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.MaxUpdates = 3;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 8 * Projectile.MaxUpdates; // 8 effective, 24 total
        }

        public override void AI()
        {
            // Timer.
            Projectile.ai[0]++;

            // Animation.
            Projectile.frameCounter++;
            if (Projectile.frameCounter > 4)
            {
                Projectile.frame++;
                Projectile.frameCounter = 0;
            }
            if (Projectile.frame >= Main.projFrames[Projectile.type])
                Projectile.frame = 0;

            // Decide what state the projectile is in.
            int projectileState = 0;
            if (Projectile.velocity.Length() <= IntendedVelocity)
                projectileState = 1;

            // Become visible.
            Projectile.alpha -= 15;
            if (Projectile.alpha < 0)
                Projectile.alpha = 0;

            switch (projectileState)
            {
                // Do this if moving faster than 4 units per frame.
                case 0:

                    // Rotate.
                    Projectile.rotation -= (float)Math.PI / 30f;

                    // Emit pretty as fuck dust.
                    if (Main.rand.NextBool(3))
                    {
                        if (Main.rand.NextBool())
                        {
                            Vector2 prettyDustDirect = Vector2.UnitY.RotatedByRandom(MathHelper.TwoPi);
                            Dust prettyDust = Main.dust[Dust.NewDust(Projectile.Center - prettyDustDirect * 45f, 0, 0, Utils.SelectRandom<int>(Main.rand, 86, 90))];
                            prettyDust.noGravity = true;
                            prettyDust.position = Projectile.Center - prettyDustDirect * Main.rand.Next(20, 31);
                            prettyDust.velocity = prettyDustDirect.RotatedBy(MathHelper.PiOver2) * 9f;
                            prettyDust.scale = 0.7f + Main.rand.NextFloat();
                            prettyDust.fadeIn = 0.5f;
                            prettyDust.customData = this;
                        }
                        else
                        {
                            Vector2 prettyDustDirect2 = Vector2.UnitY.RotatedByRandom(MathHelper.TwoPi);
                            Dust prettyDust2 = Main.dust[Dust.NewDust(Projectile.Center - prettyDustDirect2 * 45f, 0, 0, 240)];
                            prettyDust2.noGravity = true;
                            prettyDust2.position = Projectile.Center - prettyDustDirect2 * 45f;
                            prettyDust2.velocity = prettyDustDirect2.RotatedBy(-MathHelper.PiOver2) * 4f;
                            prettyDust2.scale = 0.7f + Main.rand.NextFloat();
                            prettyDust2.fadeIn = 0.5f;
                            prettyDust2.customData = this;
                        }
                    }

                    // Scale up, decelerate and rotate.
                    if (Projectile.ai[0] >= 30f)
                    {
                        Projectile.velocity *= 0.98f;
                        Projectile.scale += 0.0074468083f;
                        if (Projectile.scale > 1.3f)
                            Projectile.scale = 1.3f;

                        Projectile.rotation -= (float)Math.PI / 180f;
                    }

                    // Set velocity to intended velocity if moving slower than intended velocity.
                    if (Projectile.velocity.Length() < IntendedVelocity + 0.1f)
                    {
                        Projectile.velocity.Normalize();
                        Projectile.velocity *= IntendedVelocity;
                        Projectile.ai[0] = 0f;
                    }

                    break;

                // Do this if moving slower or equal to 4 units per frame.
                case 1:
                    {
                        // Rotate.
                        Projectile.rotation -= (float)Math.PI / 30f;

                        // Emit pretty as fuck dust.
                        if (Main.rand.NextBool())
                        {
                            Vector2 slowPrettyDustDirect = Vector2.UnitY.RotatedByRandom(MathHelper.TwoPi);
                            Dust slowPrettyDust = Main.dust[Dust.NewDust(Projectile.Center - slowPrettyDustDirect * 45f, 0, 0, 86)];
                            slowPrettyDust.noGravity = true;
                            slowPrettyDust.position = Projectile.Center - slowPrettyDustDirect * Main.rand.Next(20, 31);
                            slowPrettyDust.velocity = slowPrettyDustDirect.RotatedBy(MathHelper.PiOver2) * 9f;
                            slowPrettyDust.scale = 1.2f + Main.rand.NextFloat();
                            slowPrettyDust.fadeIn = 0.5f;
                            slowPrettyDust.customData = this;
                            slowPrettyDustDirect = Vector2.UnitY.RotatedByRandom(MathHelper.TwoPi);
                            slowPrettyDust = Main.dust[Dust.NewDust(Projectile.Center - slowPrettyDustDirect * 45f, 0, 0, 90)];
                            slowPrettyDust.noGravity = true;
                            slowPrettyDust.position = Projectile.Center - slowPrettyDustDirect * Main.rand.Next(20, 31);
                            slowPrettyDust.velocity = slowPrettyDustDirect.RotatedBy(MathHelper.PiOver2) * 9f;
                            slowPrettyDust.scale = 1.2f + Main.rand.NextFloat();
                            slowPrettyDust.fadeIn = 0.5f;
                            slowPrettyDust.customData = this;
                            slowPrettyDust.color = Color.Purple;
                        }
                        else
                        {
                            Vector2 slowPrettyDustDirect2 = Vector2.UnitY.RotatedByRandom(MathHelper.TwoPi);
                            Dust slowPrettyDust2 = Main.dust[Dust.NewDust(Projectile.Center - slowPrettyDustDirect2 * 45f, 0, 0, 240)];
                            slowPrettyDust2.noGravity = true;
                            slowPrettyDust2.position = Projectile.Center - slowPrettyDustDirect2 * Main.rand.Next(30, 41);
                            slowPrettyDust2.velocity = slowPrettyDustDirect2.RotatedBy(-MathHelper.PiOver2) * 6f;
                            slowPrettyDust2.scale = 1.2f + Main.rand.NextFloat();
                            slowPrettyDust2.fadeIn = 0.5f;
                            slowPrettyDust2.customData = this;
                        }

                        // Create the vortexes.
                        if (Projectile.ai[0] % 30f == 0f && Projectile.ai[0] < 241f && Main.myPlayer == Projectile.owner)
                        {
                            Vector2 randomProjRotate = Vector2.UnitY.RotatedByRandom(MathHelper.TwoPi) * 12f;
                            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, randomProjRotate, ModContent.ProjectileType<NebulaCloud>(), Projectile.damage / 2, 0f, Projectile.owner, 0f, Projectile.whoAmI);
                        }

                        // Homing.
                        Vector2 projCenter = Projectile.Center;
                        float homingRange = 1200f;
                        bool isHoming = false;
                        int npcTrack = 0;
                        if (Projectile.ai[1] == 0f)
                        {
                            for (int i = 0; i < Main.maxNPCs; i++)
                            {
                                if (Main.npc[i].CanBeChasedBy(this))
                                {
                                    Vector2 npcCenter = Main.npc[i].Center;
                                    if (Projectile.Distance(npcCenter) < homingRange && Collision.CanHit(new Vector2(Projectile.position.X + Projectile.width / 2, Projectile.position.Y + Projectile.height / 2), 1, 1, Main.npc[i].position, Main.npc[i].width, Main.npc[i].height))
                                    {
                                        homingRange = Projectile.Distance(npcCenter);
                                        projCenter = npcCenter;
                                        isHoming = true;
                                        npcTrack = i;
                                    }
                                }
                            }

                            if (isHoming)
                            {
                                if (Projectile.ai[1] != npcTrack + 1)
                                    Projectile.netUpdate = true;

                                Projectile.ai[1] = npcTrack + 1;
                            }

                            isHoming = false;
                        }

                        if (Projectile.ai[1] != 0f)
                        {
                            int npcID = (int)(Projectile.ai[1] - 1f);
                            if (Main.npc[npcID].active && Main.npc[npcID].CanBeChasedBy(this, ignoreDontTakeDamage: true) && Projectile.Distance(Main.npc[npcID].Center) < 1000f)
                            {
                                isHoming = true;
                                projCenter = Main.npc[npcID].Center;
                            }
                        }

                        if (isHoming)
                        {
                            float intendedSpeed = IntendedVelocity;
                            int inertia = 12;
                            Vector2 projCenterHome = Projectile.Center;
                            float projXDirection = projCenter.X - projCenterHome.X;
                            float projYDirection = projCenter.Y - projCenterHome.Y;
                            float projDistance = (float)Math.Sqrt(projXDirection * projXDirection + projYDirection * projYDirection);
                            projDistance = intendedSpeed / projDistance;
                            projXDirection *= projDistance;
                            projYDirection *= projDistance;
                            Projectile.velocity.X = (Projectile.velocity.X * (inertia - 1) + projXDirection) / inertia;
                            Projectile.velocity.Y = (Projectile.velocity.Y * (inertia - 1) + projYDirection) / inertia;
                        }

                        break;
                    }
            }

            // Emit light if visible enough.
            if (Projectile.alpha < 150)
                Lighting.AddLight(Projectile.Center, 1.4f, 0.4f, 1.2f);

            // Kill after 15 seconds.
            if (Projectile.ai[0] >= 900f)
                Projectile.Kill();
        }

        // Bounce off tiles.
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (Projectile.velocity.X != oldVelocity.X)
                Projectile.velocity.X = (0f - oldVelocity.X) * 0.25f;

            if (Projectile.velocity.Y != oldVelocity.Y)
                Projectile.velocity.Y = (0f - oldVelocity.Y) * 0.25f;

            return false;
        }

        // Draw all the clouds and other crap.
        public override bool PreDraw(ref Color lightColor)
        {
            SpriteEffects spriteEffects = SpriteEffects.None;
            if (Projectile.spriteDirection == -1)
                spriteEffects = SpriteEffects.FlipHorizontally;

            Color fuckYou = Projectile.GetAlpha(lightColor);
            Color coreColor = fuckYou * 0.8f;
            coreColor.A /= 2;
            Color cloudColor = Color.Lerp(fuckYou, Color.Black, 0.5f);
            cloudColor.A = fuckYou.A;
            float rotationScale = 0.95f + (Projectile.rotation * 0.75f).ToRotationVector2().Y * 0.1f;
            cloudColor *= rotationScale;
            float cloudScale = 0.6f + Projectile.scale * 0.6f * rotationScale;
            Texture2D coreTexture = ModContent.Request<Texture2D>(Texture).Value;
            Texture2D nebulaCloudTexture = ModContent.Request<Texture2D>("CalamityMod/Projectiles/Magic/NebulaCloud", AssetRequestMode.ImmediateLoad).Value;
            Vector2 position = Projectile.Center - Main.screenPosition;
            Vector2 coreOrigin = coreTexture.Size() / new Vector2(0, Main.projFrames[Projectile.type]) * 0.5f;
            Vector2 cloudOrigin = nebulaCloudTexture.Size() * 0.5f;
            Main.EntitySpriteDraw(nebulaCloudTexture, position, null, cloudColor, 0f - Projectile.rotation + 0.35f, cloudOrigin, cloudScale, spriteEffects ^ SpriteEffects.FlipHorizontally, 0);
            Main.EntitySpriteDraw(nebulaCloudTexture, position, null, fuckYou, 0f - Projectile.rotation, cloudOrigin, Projectile.scale, spriteEffects ^ SpriteEffects.FlipHorizontally, 0);
            Main.EntitySpriteDraw(coreTexture, position, new Rectangle?(new Rectangle(0, Main.projFrames[Projectile.type] * Projectile.frame, coreTexture.Width, Main.projFrames[Projectile.type])), coreColor, (0f - Projectile.rotation) * 0.7f, coreOrigin, Projectile.scale, spriteEffects ^ SpriteEffects.FlipHorizontally, 0);
            Main.EntitySpriteDraw(nebulaCloudTexture, position, null, fuckYou * 0.8f, Projectile.rotation * 0.5f, cloudOrigin, Projectile.scale * 0.9f, spriteEffects, 0);
            fuckYou.A = 0;

            return false;
        }

        public override Color? GetAlpha(Color lightColor) => new Color(255 - Projectile.alpha, 255 - Projectile.alpha, 255 - Projectile.alpha, 255 - Projectile.alpha);

        // Explode.
        public override void OnKill(int timeLeft)
        {
            Projectile.position = Projectile.Center;
            Projectile.width = Projectile.height = 264;
            Projectile.Center = Projectile.position;

            Projectile.maxPenetrate = -1;
            Projectile.penetrate = -1;
            Projectile.Damage();

            SoundEngine.PlaySound(SoundID.Item14, Projectile.position);

            for (int i = 0; i < 6; i++)
            {
                int blackDust = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 240, 0f, 0f, 100, default(Color), 1.75f);
                Main.dust[blackDust].position = Projectile.Center + Vector2.UnitY.RotatedByRandom(Math.PI) * (float)Main.rand.NextDouble() * Projectile.width / 2f;
            }

            for (int j = 0; j < 45; j++)
            {
                int purpleDust = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 62, 0f, 0f, 200, default(Color), 5.05f);
                Main.dust[purpleDust].position = Projectile.Center + Vector2.UnitY.RotatedByRandom(Math.PI) * (float)Main.rand.NextDouble() * Projectile.width / 2f;
                Main.dust[purpleDust].noGravity = true;
                Dust dust = Main.dust[purpleDust];
                dust.velocity *= 4f;
                purpleDust = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 90, 0f, 0f, 100, default(Color), 1.75f);
                Main.dust[purpleDust].position = Projectile.Center + Vector2.UnitY.RotatedByRandom(Math.PI) * (float)Main.rand.NextDouble() * Projectile.width / 2f;
                dust = Main.dust[purpleDust];
                dust.velocity *= 2.5f;
                Main.dust[purpleDust].noGravity = true;
                Main.dust[purpleDust].fadeIn = 1f;
                Main.dust[purpleDust].color = Color.Purple * 0.5f;
            }

            for (int k = 0; k < 15; k++)
            {
                int purpleDust2 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 62, 0f, 0f, 0, default(Color), 3.55f);
                Main.dust[purpleDust2].position = Projectile.Center + Vector2.UnitX.RotatedByRandom(Math.PI).RotatedBy(Projectile.velocity.ToRotation()) * Projectile.width / 2f;
                Main.dust[purpleDust2].noGravity = true;
                Dust dust = Main.dust[purpleDust2];
                dust.velocity *= 4f;
            }

            for (int l = 0; l < 15; l++)
            {
                int blackDust2 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 240, 0f, 0f, 0, default(Color), 1.75f);
                Main.dust[blackDust2].position = Projectile.Center + Vector2.UnitX.RotatedByRandom(Math.PI).RotatedBy(Projectile.velocity.ToRotation()) * Projectile.width / 2f;
                Main.dust[blackDust2].noGravity = true;
                Dust dust = Main.dust[blackDust2];
                dust.velocity *= 4f;
            }

            for (int m = 0; m < 3; m++)
            {
                int gored = Gore.NewGore(Projectile.GetSource_FromThis(), Projectile.position + new Vector2(Projectile.width * Main.rand.Next(100) / 100f, Projectile.height * Main.rand.Next(100) / 100f) - Vector2.One * 10f, default(Vector2), Main.rand.Next(61, 64));
                Main.gore[gored].position = Projectile.Center + Vector2.UnitY.RotatedByRandom(Math.PI) * (float)Main.rand.NextDouble() * Projectile.width / 2f;
                Gore gore = Main.gore[gored];
                gore.velocity *= 0.5f;
                Main.gore[gored].velocity.X += Main.rand.Next(-10, 11) * 0.075f;
                Main.gore[gored].velocity.Y += Main.rand.Next(-10, 11) * 0.075f;
            }

            if (Main.myPlayer == Projectile.owner)
            {
                for (int r = 0; r < Main.maxProjectiles; r++)
                {
                    if (Main.projectile[r].active && Main.projectile[r].type == ModContent.ProjectileType<NebulaCloud>() && Main.projectile[r].ai[1] == Projectile.whoAmI)
                        Main.projectile[r].Kill();
                }

                int totalProjectiles = Main.rand.Next(6, 9);
                float radians = MathHelper.TwoPi / totalProjectiles;
                int type = ModContent.ProjectileType<NebulaNova>();
                float velocity = Main.rand.Next(70, 101) * 0.1f;
                Vector2 spinningPoint = new Vector2(0f, -velocity);
                for (int k = 0; k < totalProjectiles; k++)
                {
                    Vector2 velocity2 = spinningPoint.RotatedBy(radians * k);
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center + Utils.RandomVector2(Main.rand, -30f, 30f), velocity2, type, (int)(Projectile.damage * 0.5), Projectile.knockBack * 0.8f, Projectile.owner);
                }
            }
        }
    }
}
