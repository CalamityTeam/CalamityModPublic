using Microsoft.Xna.Framework;
using System;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using CalamityMod.Buffs.DamageOverTime;

namespace CalamityMod.Projectiles.Melee
{
    public class GayBeam : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Melee";
        private int alpha = 50;
        private bool playedSound = false;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 4;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.width = 20;
            Projectile.height = 20;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.ignoreWater = true;
            Projectile.penetrate = 3;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 8;
            Projectile.timeLeft = 1200;
            Projectile.extraUpdates = 3;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(Projectile.localAI[0]);
            writer.Write(Projectile.localAI[1]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            Projectile.localAI[0] = reader.ReadSingle();
            Projectile.localAI[1] = reader.ReadSingle();
        }

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.ToRadians(45f);
            if (!playedSound)
            {
                SoundEngine.PlaySound(SoundID.Item60, Projectile.Center);
                playedSound = true;
            }
            if (Projectile.localAI[0] == 0f)
            {
                Projectile.scale -= 0.01f;
                Projectile.alpha += 15;
                if (Projectile.alpha >= 250)
                {
                    Projectile.alpha = 255;
                    Projectile.localAI[0] = 1f;
                }
            }
            else if (Projectile.localAI[0] == 1f)
            {
                Projectile.scale += 0.01f;
                Projectile.alpha -= 15;
                if (Projectile.alpha <= 0)
                {
                    Projectile.alpha = 0;
                    Projectile.localAI[0] = 0f;
                }
            }

            Color color = new Color(255, 0, 0, alpha);
            switch ((int)Projectile.ai[0])
            {
                case 0: // Red, normal beam
                    break;

                case 1: // Orange, curve back to player
                    color = new Color(255, 128, 0, alpha);

                    int p = (int)Player.FindClosest(Projectile.Center, 1, 1);
                    Projectile.ai[1] += 1f;
                    if (Projectile.ai[1] < 220f && Projectile.ai[1] > 60f)
                    {
                        float homeSpeed = Projectile.velocity.Length();
                        Vector2 vecToPlayer = Main.player[p].Center - Projectile.Center;
                        vecToPlayer.Normalize();
                        vecToPlayer *= homeSpeed;
                        Projectile.velocity = (Projectile.velocity * 24f + vecToPlayer) / 25f;
                        Projectile.velocity.Normalize();
                        Projectile.velocity *= homeSpeed;
                    }

                    if (Projectile.velocity.Length() < 24f)
                    {
                        Projectile.velocity *= 1.02f;
                    }

                    break;

                case 2: // Yellow, split after a certain time
                    color = new Color(255, 255, 0, alpha);

                    Projectile.localAI[1] += 1f;
                    if (Projectile.localAI[1] >= 180f)
                    {
                        Projectile.localAI[1] = 0f;
                        int numProj = 2;
                        float rotation = MathHelper.ToRadians(10);
                        if (Projectile.owner == Main.myPlayer)
                        {
                            for (int i = 0; i < numProj + 1; i++)
                            {
                                Vector2 perturbedSpeed = Projectile.velocity.RotatedBy(MathHelper.Lerp(-rotation, rotation, i / (numProj - 1)));
                                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, perturbedSpeed * 0.5f, ModContent.ProjectileType<GayBeam>(), (int)(Projectile.damage * 0.5), Projectile.knockBack * 0.5f, Projectile.owner, 0f, 0f);
                            }
                        }
                        Projectile.Kill();
                    }

                    break;

                case 3: // Lime, home in on player within certain distance
                    color = new Color(128, 255, 0, alpha);

                    float inertia = 75f;
                    float homingSpeed = 7.5f;
                    float minDist = 80f;
                    if (Main.player[Projectile.owner].active && !Main.player[Projectile.owner].dead)
                    {
                        if (Projectile.Distance(Main.player[Projectile.owner].Center) > minDist)
                        {
                            Vector2 moveDirection = Projectile.SafeDirectionTo(Main.player[Projectile.owner].Center, Vector2.UnitY);
                            Projectile.velocity = (Projectile.velocity * (inertia - 1f) + moveDirection * homingSpeed) / inertia;
                        }
                    }
                    else
                    {
                        if (Projectile.timeLeft > 30)
                        {
                            Projectile.timeLeft = 30;
                        }
                    }

                    break;

                case 4: // Green, home in on enemies
                    color = new Color(0, 255, 0, alpha);

                    CalamityUtils.HomeInOnNPC(Projectile, !Projectile.tileCollide, 300f, 6f, 20f);

                    break;

                case 5: // Turquoise, speed up and don't collide with tiles
                    color = new Color(0, 255, 128, alpha);

                    Projectile.tileCollide = false;
                    if (Math.Abs(Projectile.velocity.X) + Math.Abs(Projectile.velocity.Y) < 28f)
                    {
                        Projectile.velocity *= 1.035f;
                    }

                    break;

                case 6: // Cyan, upon death create several turquoise swords
                    color = new Color(0, 255, 255, alpha);
                    break;

                case 7: // Light Blue, slow down on hit
                    color = new Color(0, 128, 255, alpha);
                    break;

                case 8: // Blue, bounce off of tiles and enemies
                    color = new Color(0, 0, 255, alpha);
                    break;

                case 9: // Purple, speed up and slow down over and over
                    color = new Color(128, 0, 255, alpha);

                    Projectile.localAI[1] += 1f;
                    if (Projectile.localAI[1] <= 40f)
                    {
                        Projectile.velocity *= 0.95f;
                    }
                    else if (Projectile.localAI[1] > 40f && Projectile.localAI[1] <= 79f)
                    {
                        Projectile.velocity *= 1.05f;
                    }
                    else if (Projectile.localAI[1] == 80f)
                    {
                        Projectile.localAI[1] = 0f;
                    }

                    break;

                case 10: // Fuschia, start slow then get faster
                    color = new Color(255, 0, 255, alpha);

                    if (Projectile.localAI[1] == 0f)
                    {
                        Projectile.velocity *= 0.1f;
                        Projectile.localAI[1] += 1f;
                    }
                    Projectile.velocity *= 1.01f;

                    break;

                case 11: // Hot Pink, split into fuschia while travelling
                    color = new Color(255, 0, 128, alpha);

                    if (Projectile.localAI[1] == 0f)
                    {
                        Projectile.velocity *= 0.33f;
                    }
                    Projectile.localAI[1] += 1f;
                    if (Projectile.localAI[1] >= 181f)
                    {
                        Projectile.localAI[1] = 1f;
                        if (Main.myPlayer == Projectile.owner)
                            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center + Projectile.velocity, Projectile.velocity, ModContent.ProjectileType<GayBeam>(), Projectile.damage / 2, Projectile.knockBack * 0.5f, Projectile.owner, 10f, 0f);
                    }

                    break;

                default:
                    break;
            }

            if (Main.rand.NextBool(2))
            {
                int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 267, 0f, 0f, alpha, color, 1.5f);
                Main.dust[dust].noGravity = true;
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<MiracleBlight>(), 600);

            if (Projectile.ai[0] == 7f)
            {
                Projectile.velocity *= 0.25f;
            }
            else if (Projectile.ai[0] == 8f)
            {
                Projectile.velocity *= -1f;
            }
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(ModContent.BuffType<MiracleBlight>(), 600);

            if (Projectile.ai[0] == 7f)
            {
                Projectile.velocity *= 0.25f;
            }
            else if (Projectile.ai[0] == 8f)
            {
                Projectile.velocity *= -1f;
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (Projectile.ai[0] == 8f)
            {
                if (Projectile.velocity.X != oldVelocity.X)
                {
                    Projectile.velocity.X = -oldVelocity.X;
                }
                if (Projectile.velocity.Y != oldVelocity.Y)
                {
                    Projectile.velocity.Y = -oldVelocity.Y;
                }
                return false;
            }
            return true;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            Color color = new Color(255, 0, 0, alpha);
            switch ((int)Projectile.ai[0])
            {
                case 0: // Red
                    break;
                case 1: // Orange
                    color = new Color(255, 128, 0, alpha);
                    break;
                case 2: // Yellow
                    color = new Color(255, 255, 0, alpha);
                    break;
                case 3: // Lime
                    color = new Color(128, 255, 0, alpha);
                    break;
                case 4: // Green
                    color = new Color(0, 255, 0, alpha);
                    break;
                case 5: // Turquoise
                    color = new Color(0, 255, 128, alpha);
                    break;
                case 6: // Cyan
                    color = new Color(0, 255, 255, alpha);
                    break;
                case 7: // Light Blue
                    color = new Color(0, 128, 255, alpha);
                    break;
                case 8: // Blue
                    color = new Color(0, 0, 255, alpha);
                    break;
                case 9: // Purple
                    color = new Color(128, 0, 255, alpha);
                    break;
                case 10: // Fuschia
                    color = new Color(255, 0, 255, alpha);
                    break;
                case 11: // Hot Pink
                    color = new Color(255, 0, 128, alpha);
                    break;
                default:
                    break;
            }
            return color;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (Projectile.timeLeft > 1195)
                return false;

            CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], lightColor, 1);
            return false;
        }

        public override void Kill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item10, Projectile.Center);
            Projectile.ExpandHitboxBy(64);
            Projectile.maxPenetrate = Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
            Projectile.Damage();

            Color color = new Color(255, 0, 0, alpha);
            switch ((int)Projectile.ai[0])
            {
                case 0: // Red
                    break;
                case 1: // Orange
                    color = new Color(255, 128, 0, alpha);
                    break;
                case 2: // Yellow
                    color = new Color(255, 255, 0, alpha);
                    break;
                case 3: // Lime
                    color = new Color(128, 255, 0, alpha);
                    break;
                case 4: // Green
                    color = new Color(0, 255, 0, alpha);
                    break;
                case 5: // Turquoise
                    color = new Color(0, 255, 128, alpha);
                    break;

                case 6: // Cyan
                    color = new Color(0, 255, 255, alpha);

                    for (int x = 0; x < 3; x++)
                    {
                        bool fromRight = x == 1;
                        if (x == 2)
                            fromRight = Main.rand.NextBool(2);
                        if (Projectile.owner == Main.myPlayer)
                        {
                            var source = Projectile.GetSource_FromThis();
                            CalamityUtils.ProjectileBarrage(source, Projectile.Center, Projectile.Center, fromRight, 500f, 500f, 0f, 500f, 5f, ModContent.ProjectileType<GayBeam>(), (int)(Projectile.damage * 0.2), Projectile.knockBack * 0.2f, Projectile.owner, false, 0f).ai[0] = 5f;
                        }
                    }

                    break;

                case 7: // Light Blue
                    color = new Color(0, 128, 255, alpha);
                    break;
                case 8: // Blue
                    color = new Color(0, 0, 255, alpha);
                    break;
                case 9: // Purple
                    color = new Color(128, 0, 255, alpha);
                    break;
                case 10: // Fuschia
                    color = new Color(255, 0, 255, alpha);
                    break;
                case 11: // Hot Pink
                    color = new Color(255, 0, 128, alpha);
                    break;
                default:
                    break;
            }

            for (int d = 0; d < 3; d++)
            {
                int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 267, 0f, 0f, alpha, color, 1.5f);
                Main.dust[dust].noGravity = true;
            }
            for (int d = 0; d < 30; d++)
            {
                int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 267, 0f, 0f, alpha, color, 2.5f);
                Main.dust[dust].noGravity = true;
                Main.dust[dust].velocity *= 3f;
                dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 267, 0f, 0f, alpha, color, 1.5f);
                Main.dust[dust].velocity *= 2f;
                Main.dust[dust].noGravity = true;
            }
        }
    }
}
