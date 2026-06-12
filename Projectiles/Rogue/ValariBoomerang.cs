using System;
using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.Items.Weapons.Rogue;
using CalamityMod.Particles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Rogue
{
    public class ValariBoomerang : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Rogue";
        public override string Texture => "CalamityMod/Items/Weapons/Rogue/FrostcrushValari";

        // Used for the stealth strike
        public int TileCollideDelay = 0;
        public ref float State => ref Projectile.ai[0];
        public ref float Timer => ref Projectile.ai[1];

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailingMode[Type] = 2;
            ProjectileID.Sets.TrailCacheLength[Type] = 5;
        }
        public override void SetDefaults()
        {
            Projectile.width = 40;
            Projectile.height = 40;
            Projectile.DamageType = RogueDamageClass.Instance;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 240;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 30;
            Projectile.coldDamage = true;
        }

        public override void AI()
        {
            Timer++;

            //Constant rotation
            Projectile.rotation += 0.2f;
            //Dust trail
            if (Main.rand.NextBool(5))
                Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, DustID.IceRod, Projectile.velocity.X * 0.5f, Projectile.velocity.Y * 0.5f);
            //Constant sound effects
            if (Projectile.soundDelay == 0)
            {
                Projectile.soundDelay = 15;
                SoundEngine.PlaySound(SoundID.Item7, Projectile.position);
            }
            //Slopes REEEEEEEEEEEE
            if (Timer == 3f)
                Projectile.tileCollide = true;

            // Stealth strikes follow the cursor
            if (Projectile.Calamity().stealthStrike)
            {
                Projectile.tileCollide = true;
                if (TileCollideDelay > 0)
                    TileCollideDelay--;

                Vector2 mousePos = Main.player[Projectile.owner].ClampedMouseWorld();
                if (Vector2.Distance(Projectile.Center, mousePos) > 115f && TileCollideDelay == 0)
                {
                    float accelerationFactor = 9.5f; // Higher number = Takes longer to turn around.
                    Projectile.velocity += (mousePos - Projectile.Center).SafeNormalize(Vector2.UnitX) * FrostcrushValari.Speed / accelerationFactor;
                    if (Projectile.velocity.Length() > FrostcrushValari.Speed)
                    {
                        Projectile.velocity.Normalize();
                        Projectile.velocity *= FrostcrushValari.Speed;
                    }
                }
                else // Accelerate to top speed if close enough to prevent it staying slow if you move in the cursor while it's slowing down.
                {
                    Projectile.velocity *= 1.1f;
                    if (Projectile.velocity.Length() > FrostcrushValari.Speed)
                    {
                        Projectile.velocity.Normalize();
                        Projectile.velocity *= FrostcrushValari.Speed;
                    }
                }
            }
            else
            {
                // If State is 0, it is outgoing. If 1, it is returning.
                if (State == 0f)
                {
                    if (Timer >= 50f) // Return to the player
                    {
                        State = 1f;
                        Projectile.netUpdate = true;
                    }
                }
                else
                {
                    Projectile.tileCollide = false;
                    float returnSpeed = FrostcrushValari.Speed * 1.5f;
                    float acceleration = 3.2f;
                    Player owner = Main.player[Projectile.owner];

                    // Delete the boomerang if it's excessively far away.
                    Vector2 playerCenter = owner.Center;
                    float xDist = playerCenter.X - Projectile.Center.X;
                    float yDist = playerCenter.Y - Projectile.Center.Y;
                    float dist = (float)Math.Sqrt((double)(xDist * xDist + yDist * yDist));
                    if (dist > 3000f)
                        Projectile.Kill();

                    dist = returnSpeed / dist;
                    xDist *= dist;
                    yDist *= dist;

                    // Home back in on the player.
                    if (Projectile.velocity.X < xDist)
                    {
                        Projectile.velocity.X += acceleration;
                        if (Projectile.velocity.X < 0f && xDist > 0f)
                            Projectile.velocity.X += acceleration;
                    }
                    else if (Projectile.velocity.X > xDist)
                    {
                        Projectile.velocity.X -= acceleration;
                        if (Projectile.velocity.X > 0f && xDist < 0f)
                            Projectile.velocity.X -= acceleration;
                    }
                    if (Projectile.velocity.Y < yDist)
                    {
                        Projectile.velocity.Y += acceleration;
                        if (Projectile.velocity.Y < 0f && yDist > 0f)
                            Projectile.velocity.Y += acceleration;
                    }
                    else if (Projectile.velocity.Y > yDist)
                    {
                        Projectile.velocity.Y -= acceleration;
                        if (Projectile.velocity.Y > 0f && yDist < 0f)
                            Projectile.velocity.Y -= acceleration;
                    }

                    // Delete the projectile if it touches its owner.
                    if (Main.myPlayer == Projectile.owner)
                        if (Projectile.Hitbox.Intersects(owner.Hitbox))
                            Projectile.Kill();
                }
            }
        }

        private void OnHitEffects(Entity target)
        {
            // Start homing at player if you hit an enemy
            State = 1f;

            if (Projectile.owner == Main.myPlayer)
            {
                for (int i = 0; i < 3; i++)
                {
                    Vector2 velocity = CalamityUtils.RandomVelocity(100f, 70f, 100f);
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, velocity, ModContent.ProjectileType<Valaricicle>(), Projectile.damage / 4, 0f, Projectile.owner, ai2: Main.rand.NextBool() ? 1f : 0f);
                }
            }

            // Stealth visuals and hit sound
            if (Projectile.Calamity().stealthStrike)
            {
                SoundStyle freeze = new("CalamityMod/Sounds/NPCHit/CryogenPhaseTransitionCrack");
                SoundEngine.PlaySound(freeze with { Volume = 0.4f, Pitch = 1f }, Projectile.Center);

                float randOffset = Main.rand.NextFloat(MathHelper.TwoPi);
                for (int i = 0; i < 6; i++)
                {
                    Vector2 particleVel = Vector2.UnitX.RotatedBy(MathHelper.Pi / 3f * i + randOffset) * 7.5f;
                    WaterFlavoredParticle flashed = new(target.Center, particleVel, false, 10, 0.7f, Color.AliceBlue);
                    GeneralParticleHandler.SpawnParticle(flashed);
                }
            }
        }


        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            OnHitEffects(target);
            target.AddBuff(BuffID.Frostburn2, 120);
            if (Projectile.Calamity().stealthStrike)
                target.AddBuff(BuffID.Frozen, 45);

        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            OnHitEffects(target);
            target.AddBuff(BuffID.Frostburn2, 120);
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            //Bounce off tiles and return to player if it hits a tile
            Collision.HitTiles(Projectile.position + Projectile.velocity, Projectile.velocity, Projectile.width, Projectile.height);
            SoundEngine.PlaySound(SoundID.Item50, Projectile.position);
            if (Projectile.velocity.X != oldVelocity.X)
            {
                Projectile.velocity.X = -oldVelocity.X;
            }
            if (Projectile.velocity.Y != oldVelocity.Y)
            {
                Projectile.velocity.Y = -oldVelocity.Y;
            }
            State = 1f;
            if (TileCollideDelay == 0)
                TileCollideDelay = 10;
            return false;
        }

        // Stealth strike shatter effects on death
        public override void OnKill(int timeLeft)
        {
            if (Projectile.Calamity().stealthStrike)
            {
                SoundEngine.PlaySound(SoundID.Item27, Projectile.Center);
                Vector2 splinterVel = Projectile.velocity.RotatedByRandom(MathHelper.Pi / 12f);

                for (int i = 1; i <= 5; i++)
                    Gore.NewGore(Projectile.GetSource_Death(), Projectile.Center, splinterVel, Mod.Find<ModGore>($"FrostcrushValariGore{i}").Type);
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (Projectile.Calamity().stealthStrike)
            {
                CalamityUtils.DrawAfterimagesCentered(Projectile, 2, lightColor);
                return false;
            }
            return true;
        }
    }
}
