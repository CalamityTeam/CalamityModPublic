using CalamityMod.Buffs.DamageOverTime;
using Microsoft.Xna.Framework;
using System;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Boss
{
    public class DarkOrb : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Boss";
        public override string Texture => "CalamityMod/Projectiles/LightningProj";

        private bool start = true;
        private Vector2 center = Vector2.Zero;
        private Vector2 velocity = Vector2.Zero;
        private double[] distances = new double[4];

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 2;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.width = 20;
            Projectile.height = 20;
            Projectile.hostile = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 3600;
            CooldownSlot = ImmunityCooldownID.Bosses;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(Projectile.localAI[0]);
            writer.Write(Projectile.localAI[1]);
            writer.Write(start);
            writer.WriteVector2(center);
            writer.Write(distances[0]);
            writer.Write(distances[1]);
            writer.Write(distances[2]);
            writer.Write(distances[3]);
            writer.WriteVector2(velocity);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            Projectile.localAI[0] = reader.ReadSingle();
            Projectile.localAI[1] = reader.ReadSingle();
            start = reader.ReadBoolean();
            center = reader.ReadVector2();
            distances[0] = reader.ReadDouble();
            distances[1] = reader.ReadDouble();
            distances[2] = reader.ReadDouble();
            distances[3] = reader.ReadDouble();
            velocity = reader.ReadVector2();
        }

        public override void AI()
        {
            if (Projectile.ai[0] != 8f)
            {
                // Pulse in and out
                if (Projectile.localAI[1] == 0f)
                {
                    Projectile.scale -= 0.012f;
                    if (Projectile.scale <= 0.8f)
                    {
                        Projectile.scale = 0.8f;
                        Projectile.localAI[1] = 1f;
                    }
                }
                else
                {
                    Projectile.scale += 0.012f;
                    if (Projectile.scale >= 1.2f)
                    {
                        Projectile.scale = 1.2f;
                        Projectile.localAI[1] = 0f;
                    }
                }
            }

            switch ((int)Projectile.ai[0])
            {
                // No AI
                case -1:
                    break;

                // Rotate around a point, spread outward and move
                case 0:
                case 1:

                    if (start)
                    {
                        center = Projectile.Center;
                        velocity = Vector2.Normalize(Main.player[Player.FindClosest(Projectile.Center, 1, 1)].Center - Projectile.Center) * 2f;
                        start = false;
                    }

                    center += velocity;

                    double rad = MathHelper.ToRadians(Projectile.ai[1]);

                    float amount = 1f - Projectile.localAI[0] / 360f;
                    if (amount < 0f)
                        amount = 0f;

                    distances[0] += MathHelper.Lerp(1f, 6f, amount);

                    if (Projectile.ai[0] == 0f)
                    {
                        Projectile.position.X = center.X - (int)(Math.Sin(rad) * distances[0]) - Projectile.width / 2;
                        Projectile.position.Y = center.Y - (int)(Math.Cos(rad) * distances[0]) - Projectile.height / 2;
                    }
                    else
                    {
                        Projectile.position.X = center.X - (int)(Math.Cos(rad) * distances[0]) - Projectile.width / 2;
                        Projectile.position.Y = center.Y - (int)(Math.Sin(rad) * distances[0]) - Projectile.height / 2;
                    }

                    Projectile.ai[1] += 0.25f + amount;
                    Projectile.localAI[0] += 1f;

                    break;

                // Lurch forward
                case 2:

                    LurchForward(0.05f, 0.95f, 1.05f);

                    break;

                // Wavy motion
                case 3:
                case 4:

                    bool useSin = Projectile.ai[0] == 3f;

                    WavyMotion(0.1f, 0f, useSin, false);

                    break;

                // Speed up then slow down on a timer
                case 5:
                case 16:

                    float fastGateValue = Projectile.ai[0] == 5f ? 30f : 60f;
                    float maxVelocity = Projectile.ai[0] == 5f ? 12f : 6f;

                    LurchForwardOnTimer(90f, fastGateValue, 3f, maxVelocity, 0.95f, 1.2f);

                    break;

                // Split into 4 projectiles that can use any of the first 6 AIs
                case 6:

                    Projectile.ai[1] += 1f;

                    if (Projectile.ai[1] >= 90f)
                    {
                        if (Projectile.owner == Main.myPlayer)
                        {
                            int totalProjectiles = 4;
                            float radians = MathHelper.TwoPi / totalProjectiles;
                            for (int i = 0; i < totalProjectiles; i++)
                            {
                                Vector2 vector = new Vector2(0f, -8f).RotatedBy(radians * i);
                                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, vector, Projectile.type, Projectile.damage, 0f, Main.myPlayer, Main.rand.Next(6), 0f);
                            }
                        }

                        Projectile.Kill();
                    }

                    break;

                // Split into 8 projectiles that can use either of the first 2 AIs
                case 7:

                    Projectile.ai[1] += 1f;

                    if (Projectile.ai[1] >= 120f)
                    {
                        if (Projectile.owner == Main.myPlayer)
                        {
                            int totalProjectiles = 8;
                            float radians = MathHelper.TwoPi / totalProjectiles;
                            for (int i = 0; i < totalProjectiles; i++)
                            {
                                Vector2 vector = new Vector2(0f, -8f).RotatedBy(radians * i);
                                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, vector, Projectile.type, Projectile.damage, 0f, Main.myPlayer, Main.rand.Next(2), 0f);
                            }
                        }

                        Projectile.Kill();
                    }

                    break;

                // Split into 3 wavy projectiles multiple times
                case 8:

                    bool splitOnce = Projectile.timeLeft < 1800;
                    bool splitTwice = Projectile.timeLeft < 900;
                    bool splitThrice = Projectile.timeLeft < 450;

                    if (splitOnce)
                        WavyMotion(0.05f, 2f, true, true);

                    Projectile.localAI[0] += 1f;

                    if (Projectile.localAI[0] >= 180f)
                    {
                        if (Projectile.owner == Main.myPlayer)
                        {
                            int totalProjectiles = 3;
                            float radians = MathHelper.TwoPi / totalProjectiles;
                            int spread = 8;

                            if (splitOnce)
                            {
                                spread *= (splitTwice ? 3 : 0) +
                                    (splitThrice ? 3 : 0);
                            }

                            Vector2 vector = Main.player[Player.FindClosest(Projectile.Center, 1, 1)].Center - Projectile.Center;
                            vector.Normalize();
                            vector *= 3f;

                            for (int i = 0; i < totalProjectiles; i++)
                            {
                                Vector2 vector2 = splitOnce ? Projectile.velocity.RotatedBy(MathHelper.ToRadians(i * spread)) : new Vector2(0f, -3f).RotatedBy(radians * i);

                                int proj = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, vector + vector2, Projectile.type, Projectile.damage, 0f, Main.myPlayer, 8f, 0f);

                                Main.projectile[proj].timeLeft = Projectile.timeLeft / 2;
                                if (Main.projectile[proj].timeLeft < 150)
                                    Main.projectile[proj].timeLeft = 150;

                                Main.projectile[proj].scale = Projectile.scale * 0.6f;
                            }
                        }

                        Projectile.Kill();
                    }

                    break;

                // idk yet
                case 9:
                case 10:

                    bool useSin2 = Projectile.ai[0] == 9f;

                    OscillationMotion(0.05f, useSin2);

                    break;

                // Homing
                case 11:

                    Projectile.ai[1] += 1f;

                    if (Projectile.ai[1] >= 180f)
                    {
                        Projectile.localAI[0] += 1f;
                        if (Projectile.localAI[0] < 180f)
                        {
                            Vector2 vector = Main.player[Player.FindClosest(Projectile.Center, 1, 1)].Center - Projectile.Center;
                            float scaleFactor = Projectile.velocity.Length();
                            vector.Normalize();
                            vector *= scaleFactor;
                            Projectile.velocity = (Projectile.velocity * 15f + vector) / 16f;
                            Projectile.velocity.Normalize();
                            Projectile.velocity *= scaleFactor;
                        }
                        else if (Projectile.velocity.Length() < 18f)
                            Projectile.velocity *= 1.01f;
                    }

                    break;

                // Rotate around a moving point and spread outward then inward in an oval pattern
                case 12:
                case 13:

                    if (start)
                    {
                        center = Projectile.Center;
                        velocity = Vector2.Normalize(Main.player[Player.FindClosest(Projectile.Center, 1, 1)].Center - Projectile.Center);
                        start = false;
                    }

                    center += velocity;

                    float velocityGateValue3 = 240f;
                    float amount2 = 1f;

                    bool flyOutward = Projectile.localAI[0] < velocityGateValue3;

                    if (flyOutward)
                        amount2 -= Projectile.localAI[0] / velocityGateValue3;
                    else
                        amount2 = (Projectile.localAI[0] - velocityGateValue3) / velocityGateValue3;

                    amount2 = MathHelper.Clamp(0f, 1f, amount2);

                    double distanceVariable = MathHelper.Lerp(0f, 6f, amount2);

                    distances[0] += flyOutward ? distanceVariable : -distanceVariable;

                    double rad2 = MathHelper.ToRadians(Projectile.ai[1]);

                    if (Projectile.ai[0] == 12f)
                    {
                        Projectile.position.X = center.X - (int)(Math.Sin(rad2) * distances[0] * 2D) - Projectile.width / 2;
                        Projectile.position.Y = center.Y - (int)(Math.Cos(rad2) * distances[0]) - Projectile.height / 2;
                    }
                    else
                    {
                        Projectile.position.X = center.X - (int)(Math.Cos(rad2) * distances[0]) - Projectile.width / 2;
                        Projectile.position.Y = center.Y - (int)(Math.Sin(rad2) * distances[0] * 2D) - Projectile.height / 2;
                    }

                    Projectile.ai[1] += 0.25f + amount2;
                    Projectile.localAI[0] += 1f;

                    break;

                // Spread out then drift inward after some time and then drift outward
                case 14:
                case 15:

                    float velocityMult = 12f;

                    if (start)
                    {
                        center = Projectile.Center;
                        velocity = Vector2.Normalize(Main.player[Player.FindClosest(Projectile.Center, 1, 1)].Center - Projectile.Center) * velocityMult;
                        start = false;
                    }

                    center += velocity;

                    float flyInwardGateValue = 240f;

                    float amount3 = 1f - Projectile.localAI[0] / flyInwardGateValue;
                    if (amount3 < 0f)
                        amount3 = 0f;

                    bool changeXPos = Projectile.ai[0] == 14f;

                    float units = 6f;

                    distances[0] += MathHelper.Lerp(1f, units, amount3);

                    if (Projectile.localAI[0] > flyInwardGateValue)
                    {
                        distances[2] = changeXPos ? Math.Abs(center.X - Projectile.Center.X) : Math.Abs(center.Y - Projectile.Center.Y);

                        if (distances[3] == 0D)
                            distances[3] = distances[2] - units * 10f;

                        distances[3] += changeXPos ? velocity.X : velocity.Y;

                        distances[1] -= distances[2] < (units * 10f) ? distances[2] * 0.1f : MathHelper.Lerp(units, units + 2f, (float)(distances[2] / distances[3]));
                    }
                    else
                    {
                        float slowDownGateValue = 30f;

                        if (Projectile.localAI[0] > flyInwardGateValue - slowDownGateValue)
                            velocity *= 1f - velocityMult / slowDownGateValue;

                        distances[1] = distances[0];
                    }

                    double distanceX = changeXPos ? distances[1] : distances[0];
                    double distanceY = changeXPos ? distances[0] : distances[1];

                    double rad3 = MathHelper.ToRadians(Projectile.ai[1]);

                    Projectile.position.X = center.X - (int)(Math.Sin(rad3) * distanceX) - Projectile.width / 2;
                    Projectile.position.Y = center.Y - (int)(Math.Cos(rad3) * distanceY) - Projectile.height / 2;

                    Projectile.localAI[0] += 1f;

                    if (Projectile.localAI[0] > 600f)
                        Projectile.Kill();

                    break;
            }
        }

        private void WavyMotion(float frequency, float amplitude, bool useSin, bool waveWithVelocity)
        {
            if (start)
            {
                velocity = Projectile.velocity;
                start = false;
            }

            Projectile.ai[1] += frequency;

            if (amplitude == 0f)
                amplitude = velocity.Length();

            float wavyVelocity = useSin ? (float)Math.Sin(Projectile.ai[1]) : (float)Math.Cos(Projectile.ai[1]);

            if (waveWithVelocity)
                Projectile.velocity = velocity + new Vector2(wavyVelocity, wavyVelocity) * amplitude;
            else
                Projectile.velocity = velocity + new Vector2(wavyVelocity, wavyVelocity).RotatedBy(MathHelper.ToRadians(velocity.ToRotation())) * amplitude;
        }

        private void LurchForward(float frequncy, float deceleration, float acceleration)
        {
            Projectile.ai[1] += frequncy;

            Projectile.velocity *= MathHelper.Lerp(deceleration, acceleration, (float)Math.Abs(Math.Sin(Projectile.ai[1])));
        }

        private void LurchForwardOnTimer(float slowGateValue, float fastGateValue, float minVelocity, float maxVelocity, float deceleration, float acceleration)
        {
            Projectile.ai[1] += 1f;

            if (Projectile.ai[1] <= slowGateValue)
            {
                if (Projectile.velocity.Length() > minVelocity)
                    Projectile.velocity *= deceleration;
            }
            else if (Projectile.ai[1] < slowGateValue + fastGateValue)
            {
                if (Projectile.velocity.Length() < maxVelocity)
                    Projectile.velocity *= acceleration;
            }
            else
                Projectile.ai[1] = 0f;
        }

        private void OscillationMotion(float frequncy, bool useSin)
        {
            Projectile.ai[1] += frequncy;

            float oscillation = useSin ? (float)Math.Sin(Projectile.ai[1]) : (float)Math.Cos(Projectile.ai[1]);

            if (start)
            {
                velocity = Projectile.velocity;
                start = false;
            }
            else if (oscillation == 0f)
            {
                Player target = Main.player[Player.FindClosest(Projectile.Center, 1, 1)];
                Vector2 vector = target.Center + target.velocity * 20f - Projectile.Center;
                vector.Normalize();
                vector *= velocity.Length();
                Projectile.velocity = vector;
            }
            else
            {
                float amplitude = velocity.Length();

                Projectile.velocity.Normalize();
                Projectile.velocity *= amplitude * oscillation;
            }
        }

        public override Color? GetAlpha(Color lightColor)
        {
            switch ((int)Projectile.ai[0])
            {
                // No AI
                case -1:

                    return new Color(0, 100, 255, Projectile.alpha);

                // Rotate around a point, spread outward and move
                case 0:
                case 1:

                    return new Color(0, 100, 255, Projectile.alpha);

                // Lurch forward
                case 2:

                    return new Color(255, 200, 0, Projectile.alpha);

                // Wavy motion
                case 3:
                case 4:

                    return new Color(0, 255, 200, Projectile.alpha);

                // Speed up then slow down on a timer
                case 5:
                case 16:

                    return new Color(0, 255, 100, Projectile.alpha);

                // Split into 4 projectiles that can use any of the first 6 AIs
                case 6:

                    return new Color(200, 0, 255, Projectile.alpha);

                // Split into 8 projectiles that can use either of the first 2 AIs
                case 7:

                    return new Color(255, 0, 150, Projectile.alpha);

                // Split into 3 wavy projectiles multiple times
                case 8:

                    return new Color(200, 0, 0, Projectile.alpha);

                // idk yet
                case 9:
                case 10:

                    return new Color(255, 255, 0, Projectile.alpha);

                // Homing
                case 11:

                    return new Color(150, 0, 200, Projectile.alpha);

                // Rotate around a moving point and spread outward then inward in an oval pattern
                case 12:
                case 13:

                    return new Color(0, 100, 255, Projectile.alpha);

                // Spread out then drift inward after some time and then drift outward
                case 14:
                case 15:

                    return new Color(0, 100, 255, Projectile.alpha);
            }

            return null;
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            if (info.Damage <= 0)
                return;

            target.AddBuff(ModContent.BuffType<GodSlayerInferno>(), 200);
            //target.AddBuff(ModContent.BuffType<Delirium>(), 300, true);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], lightColor, 1);
            return false;
        }
    }
}
