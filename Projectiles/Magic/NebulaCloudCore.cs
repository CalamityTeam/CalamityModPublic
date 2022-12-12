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
    public class NebulaCloudCore : ModProjectile
    {
        private const float IntendedVelocity = 4f;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Nebula Cloud Core");
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
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 8;
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
                        if (Main.rand.NextBool(2))
                        {
                            Vector2 vector93 = Vector2.UnitY.RotatedByRandom(MathHelper.TwoPi);
                            Dust dust45 = Main.dust[Dust.NewDust(Projectile.Center - vector93 * 45f, 0, 0, Utils.SelectRandom<int>(Main.rand, 86, 90))];
                            dust45.noGravity = true;
                            dust45.position = Projectile.Center - vector93 * Main.rand.Next(20, 31);
                            dust45.velocity = vector93.RotatedBy(MathHelper.PiOver2) * 9f;
                            dust45.scale = 0.7f + Main.rand.NextFloat();
                            dust45.fadeIn = 0.5f;
                            dust45.customData = this;
                        }
                        else
                        {
                            Vector2 vector94 = Vector2.UnitY.RotatedByRandom(MathHelper.TwoPi);
                            Dust dust46 = Main.dust[Dust.NewDust(Projectile.Center - vector94 * 45f, 0, 0, 240)];
                            dust46.noGravity = true;
                            dust46.position = Projectile.Center - vector94 * 45f;
                            dust46.velocity = vector94.RotatedBy(-MathHelper.PiOver2) * 4f;
                            dust46.scale = 0.7f + Main.rand.NextFloat();
                            dust46.fadeIn = 0.5f;
                            dust46.customData = this;
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
                            Vector2 vector88 = Vector2.UnitY.RotatedByRandom(MathHelper.TwoPi);
                            Dust dust43 = Main.dust[Dust.NewDust(Projectile.Center - vector88 * 45f, 0, 0, 86)];
                            dust43.noGravity = true;
                            dust43.position = Projectile.Center - vector88 * Main.rand.Next(20, 31);
                            dust43.velocity = vector88.RotatedBy(MathHelper.PiOver2) * 9f;
                            dust43.scale = 1.2f + Main.rand.NextFloat();
                            dust43.fadeIn = 0.5f;
                            dust43.customData = this;
                            vector88 = Vector2.UnitY.RotatedByRandom(MathHelper.TwoPi);
                            dust43 = Main.dust[Dust.NewDust(Projectile.Center - vector88 * 45f, 0, 0, 90)];
                            dust43.noGravity = true;
                            dust43.position = Projectile.Center - vector88 * Main.rand.Next(20, 31);
                            dust43.velocity = vector88.RotatedBy(MathHelper.PiOver2) * 9f;
                            dust43.scale = 1.2f + Main.rand.NextFloat();
                            dust43.fadeIn = 0.5f;
                            dust43.customData = this;
                            dust43.color = Color.Purple;
                        }
                        else
                        {
                            Vector2 vector89 = Vector2.UnitY.RotatedByRandom(MathHelper.TwoPi);
                            Dust dust44 = Main.dust[Dust.NewDust(Projectile.Center - vector89 * 45f, 0, 0, 240)];
                            dust44.noGravity = true;
                            dust44.position = Projectile.Center - vector89 * Main.rand.Next(30, 41);
                            dust44.velocity = vector89.RotatedBy(-MathHelper.PiOver2) * 6f;
                            dust44.scale = 1.2f + Main.rand.NextFloat();
                            dust44.fadeIn = 0.5f;
                            dust44.customData = this;
                        }

                        // Create the vortexes.
                        if (Projectile.ai[0] % 30f == 0f && Projectile.ai[0] < 241f && Main.myPlayer == Projectile.owner)
                        {
                            Vector2 vector90 = Vector2.UnitY.RotatedByRandom(MathHelper.TwoPi) * 12f;
                            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, vector90, ModContent.ProjectileType<NebulaCloud>(), Projectile.damage / 2, 0f, Projectile.owner, 0f, Projectile.whoAmI);
                        }

                        // Homing.
                        Vector2 vector91 = Projectile.Center;
                        float num915 = 1200f;
                        bool flag49 = false;
                        int num916 = 0;
                        if (Projectile.ai[1] == 0f)
                        {
                            for (int i = 0; i < Main.maxNPCs; i++)
                            {
                                if (Main.npc[i].CanBeChasedBy(this))
                                {
                                    Vector2 center12 = Main.npc[i].Center;
                                    if (Projectile.Distance(center12) < num915 && Collision.CanHit(new Vector2(Projectile.position.X + Projectile.width / 2, Projectile.position.Y + Projectile.height / 2), 1, 1, Main.npc[i].position, Main.npc[i].width, Main.npc[i].height))
                                    {
                                        num915 = Projectile.Distance(center12);
                                        vector91 = center12;
                                        flag49 = true;
                                        num916 = i;
                                    }
                                }
                            }

                            if (flag49)
                            {
                                if (Projectile.ai[1] != num916 + 1)
                                    Projectile.netUpdate = true;

                                Projectile.ai[1] = num916 + 1;
                            }

                            flag49 = false;
                        }

                        if (Projectile.ai[1] != 0f)
                        {
                            int num918 = (int)(Projectile.ai[1] - 1f);
                            if (Main.npc[num918].active && Main.npc[num918].CanBeChasedBy(this, ignoreDontTakeDamage: true) && Projectile.Distance(Main.npc[num918].Center) < 1000f)
                            {
                                flag49 = true;
                                vector91 = Main.npc[num918].Center;
                            }
                        }

                        if (flag49)
                        {
                            float num919 = IntendedVelocity;
                            int inertia = 12;
                            Vector2 vector92 = Projectile.Center;
                            float num921 = vector91.X - vector92.X;
                            float num922 = vector91.Y - vector92.Y;
                            float num923 = (float)Math.Sqrt(num921 * num921 + num922 * num922);
                            num923 = num919 / num923;
                            num921 *= num923;
                            num922 *= num923;
                            Projectile.velocity.X = (Projectile.velocity.X * (inertia - 1) + num921) / inertia;
                            Projectile.velocity.Y = (Projectile.velocity.Y * (inertia - 1) + num922) / inertia;
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
        public override void Kill(int timeLeft)
        {
            Projectile.position = Projectile.Center;
            Projectile.width = Projectile.height = 264;
            Projectile.Center = Projectile.position;

            Projectile.maxPenetrate = -1;
            Projectile.penetrate = -1;
            Projectile.Damage();

            SoundEngine.PlaySound(SoundID.Item14, Projectile.position);

            for (int num145 = 0; num145 < 6; num145++)
            {
                int num146 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 240, 0f, 0f, 100, default(Color), 1.75f);
                Main.dust[num146].position = Projectile.Center + Vector2.UnitY.RotatedByRandom(Math.PI) * (float)Main.rand.NextDouble() * Projectile.width / 2f;
            }

            for (int num147 = 0; num147 < 45; num147++)
            {
                int num148 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 62, 0f, 0f, 200, default(Color), 5.05f);
                Main.dust[num148].position = Projectile.Center + Vector2.UnitY.RotatedByRandom(Math.PI) * (float)Main.rand.NextDouble() * Projectile.width / 2f;
                Main.dust[num148].noGravity = true;
                Dust dust = Main.dust[num148];
                dust.velocity *= 4f;
                num148 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 90, 0f, 0f, 100, default(Color), 1.75f);
                Main.dust[num148].position = Projectile.Center + Vector2.UnitY.RotatedByRandom(Math.PI) * (float)Main.rand.NextDouble() * Projectile.width / 2f;
                dust = Main.dust[num148];
                dust.velocity *= 2.5f;
                Main.dust[num148].noGravity = true;
                Main.dust[num148].fadeIn = 1f;
                Main.dust[num148].color = Color.Purple * 0.5f;
            }

            for (int num149 = 0; num149 < 15; num149++)
            {
                int num150 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 62, 0f, 0f, 0, default(Color), 3.55f);
                Main.dust[num150].position = Projectile.Center + Vector2.UnitX.RotatedByRandom(Math.PI).RotatedBy(Projectile.velocity.ToRotation()) * Projectile.width / 2f;
                Main.dust[num150].noGravity = true;
                Dust dust = Main.dust[num150];
                dust.velocity *= 4f;
            }

            for (int num151 = 0; num151 < 15; num151++)
            {
                int num152 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 240, 0f, 0f, 0, default(Color), 1.75f);
                Main.dust[num152].position = Projectile.Center + Vector2.UnitX.RotatedByRandom(Math.PI).RotatedBy(Projectile.velocity.ToRotation()) * Projectile.width / 2f;
                Main.dust[num152].noGravity = true;
                Dust dust = Main.dust[num152];
                dust.velocity *= 4f;
            }

            for (int num153 = 0; num153 < 3; num153++)
            {
                int num154 = Gore.NewGore(Projectile.GetSource_FromThis(), Projectile.position + new Vector2(Projectile.width * Main.rand.Next(100) / 100f, Projectile.height * Main.rand.Next(100) / 100f) - Vector2.One * 10f, default(Vector2), Main.rand.Next(61, 64));
                Main.gore[num154].position = Projectile.Center + Vector2.UnitY.RotatedByRandom(Math.PI) * (float)Main.rand.NextDouble() * Projectile.width / 2f;
                Gore gore = Main.gore[num154];
                gore.velocity *= 0.5f;
                Main.gore[num154].velocity.X += Main.rand.Next(-10, 11) * 0.075f;
                Main.gore[num154].velocity.Y += Main.rand.Next(-10, 11) * 0.075f;
            }

            if (Main.myPlayer == Projectile.owner)
            {
                for (int num155 = 0; num155 < Main.maxProjectiles; num155++)
                {
                    if (Main.projectile[num155].active && Main.projectile[num155].type == ModContent.ProjectileType<NebulaCloud>() && Main.projectile[num155].ai[1] == Projectile.whoAmI)
                        Main.projectile[num155].Kill();
                }

                int num156 = Main.rand.Next(3, 5);
                int num157 = Main.rand.Next(3, 5);
                for (int num160 = 0; num160 < num156; num160++)
                {
                    Vector2 vector10 = Projectile.Center + Utils.RandomVector2(Main.rand, -30f, 30f);
                    Vector2 vector11 = new Vector2(Main.rand.Next(-100, 101), Main.rand.Next(-100, 101));
                    while (vector11.X == 0f && vector11.Y == 0f)
                        vector11 = new Vector2(Main.rand.Next(-100, 101), Main.rand.Next(-100, 101));

                    vector11.Normalize();
                    if (vector11.Y > 0.2f)
                        vector11.Y *= -1f;

                    vector11 *= Main.rand.Next(70, 101) * 0.1f;
                    int proj = Projectile.NewProjectile(Projectile.GetSource_FromThis(), vector10, vector11, ModContent.ProjectileType<NebulaNova>(), (int)(Projectile.damage * 0.5), Projectile.knockBack * 0.8f, Projectile.owner);
                    Main.projectile[proj].DamageType = DamageClass.Magic;
                }

                for (int num161 = 0; num161 < num157; num161++)
                {
                    Vector2 vector12 = Projectile.Center + Utils.RandomVector2(Main.rand, -30f, 30f);
                    Vector2 vector13 = new Vector2(Main.rand.Next(-100, 101), Main.rand.Next(-100, 101));
                    while (vector13.X == 0f && vector13.Y == 0f)
                        vector13 = new Vector2(Main.rand.Next(-100, 101), Main.rand.Next(-100, 101));

                    vector13.Normalize();
                    if (vector13.Y > 0.4f)
                        vector13.Y *= -1f;

                    vector13 *= Main.rand.Next(40, 81) * 0.1f;
                    int proj = Projectile.NewProjectile(Projectile.GetSource_FromThis(), vector12, vector13, ModContent.ProjectileType<NebulaNova>(), (int)(Projectile.damage * 0.5), Projectile.knockBack * 0.8f, Projectile.owner);
                    Main.projectile[proj].DamageType = DamageClass.Magic;
                }
            }
        }
    }
}
