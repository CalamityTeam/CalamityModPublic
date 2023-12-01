using CalamityMod.Particles;
using CalamityMod.Items.Weapons.Melee;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;
using Terraria.Audio;


namespace CalamityMod.Projectiles.Melee
{
    public class ArkoftheAncientsParryHoldout : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Melee";
        public override string Texture => "CalamityMod/Items/Weapons/Melee/FracturedArk";

        private bool initialized = false;
        const float MaxTime = 340;
        static float ParryTime = 15;
        public Vector2 DistanceFromPlayer => Projectile.velocity * 10 * (1f + ((float)Math.Sin(Timer / ParryTime * MathHelper.Pi) * 0.8f));
        public float Timer => MaxTime - Projectile.timeLeft;
        public ref float AlreadyParried => ref Projectile.ai[1];
        public Player Owner => Main.player[Projectile.owner];

        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.MeleeNoSpeed;
            Projectile.width = Projectile.height = 75;
            Projectile.width = Projectile.height = 75;
            Projectile.tileCollide = false;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.noEnchantmentVisuals = true;
        }

        public override bool? CanDamage() => Timer <= ParryTime && AlreadyParried == 0f;

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            //The hitbox is simplified into a line collision.
            float collisionPoint = 0f;
            float bladeLength = 80f * Projectile.scale;
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Owner.Center + DistanceFromPlayer, Owner.Center + DistanceFromPlayer + (Projectile.velocity * bladeLength), 24, ref collisionPoint);
        }

        public void GeneralParryEffects()
        {
            FracturedArk sword = (Owner.HeldItem.ModItem as FracturedArk);
            if (sword != null)
                sword.Charge = 10f;

            SoundEngine.PlaySound(SoundID.DD2_WitherBeastCrystalImpact);
            SoundEngine.PlaySound(SoundID.Item67);
            CombatText.NewText(Projectile.Hitbox, new Color(111, 247, 200), CalamityUtils.GetTextValue("Misc.ArkParry"), true);
            AlreadyParried = 1f;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (AlreadyParried > 0)
                return;

            GeneralParryEffects();

            //only get iframes if the enemy has contact damage :)
            if (target.damage > 0)
                Owner.GiveIFrames(35);

            Vector2 particleOrigin = target.Hitbox.Size().Length() < 140 ? target.Center : Projectile.Center + Projectile.rotation.ToRotationVector2() * 60f;
            Particle spark = new GenericSparkle(particleOrigin, Vector2.Zero, Color.White, Color.HotPink, 1.2f, 35, 0.1f, 2);
            GeneralParticleHandler.SpawnParticle(spark);

            for (int i = 0; i < 10; i++)
            {
                Vector2 particleSpeed = Main.rand.NextVector2CircularEdge(1, 1) * Main.rand.NextFloat(2.6f, 4f);
                Particle energyLeak = new SquishyLightParticle(particleOrigin, particleSpeed, Main.rand.NextFloat(0.3f, 0.6f), Color.Cyan, 60, 1, 1.5f, hueShift: 0.02f);
                GeneralParticleHandler.SpawnParticle(energyLeak);
            }
        }

        public override void AI()
        {
            if (!initialized) //Initialization
            {
                Projectile.timeLeft = (int)MaxTime;
                SoundEngine.PlaySound(SoundID.DD2_SkyDragonsFuryShot, Projectile.Center);

                Projectile.velocity = Owner.SafeDirectionTo(Owner.Calamity().mouseWorld, Vector2.Zero);
                Projectile.velocity.Normalize();
                Projectile.rotation = Projectile.velocity.ToRotation();

                initialized = true;
                Projectile.netUpdate = true;
                Projectile.netSpam = 0;
            }

            //Manage position and rotation
            Projectile.Center = Owner.Center + DistanceFromPlayer ;
            Projectile.scale = 1.4f + ((float)Math.Sin(Timer / 160f * MathHelper.Pi) * 0.6f); //SWAGGER

            if (Timer > ParryTime)
                return;

            if (AlreadyParried == 0)
            {
                float collisionPoint = 0f;
                float bladeLength = 80f * Projectile.scale;

                for (int k = 0; k < Main.maxProjectiles; k++)
                {
                    Projectile proj = Main.projectile[k];

                    if (proj.active && proj.hostile && proj.damage > 1 && //Only parry harmful projectiles
                        proj.velocity.Length() * (proj.extraUpdates + 1) > 1f && //Only parry projectiles that move semi-quickly
                        proj.Size.Length() < 300 && //Only parry projectiles that aren't too large
                        Collision.CheckAABBvLineCollision(proj.Hitbox.TopLeft(), proj.Hitbox.Size(), Owner.Center + DistanceFromPlayer, Owner.Center + DistanceFromPlayer + (Projectile.velocity * bladeLength), 24, ref collisionPoint))
                    {
                        GeneralParryEffects();

                        //Reduce the projectile's damage by 50 for a second.
                        if (proj.Calamity().flatDR < 50)
                            proj.Calamity().flatDR = 50;
                        if (proj.Calamity().flatDRTimer < 60)
                            proj.Calamity().flatDRTimer = 60;

                        //Bounce off the player if they are in the air
                        if (Owner.velocity.Y != 0)
                            Owner.velocity += Utils.SafeNormalize(Owner.Center - proj.Center, Vector2.Zero) * 2;
                        break;
                    }
                }
            }

            //Make the owner look like theyre holding the sword bla bla
            Owner.heldProj = Projectile.whoAmI;
            Owner.ChangeDir(Math.Sign(Projectile.velocity.X));
            Owner.itemRotation = Projectile.rotation;
            if (Owner.direction != 1)
            {
                Owner.itemRotation -= MathHelper.Pi;
            }
            Owner.itemRotation = MathHelper.WrapAngle(Owner.itemRotation);

            if (AlreadyParried > 0)
            {
                AlreadyParried++;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            //Stop drawing the sword. Draw a recharge bar instead
            if (Timer > ParryTime)
            {
                if (Main.myPlayer == Owner.whoAmI)
                {
                    var barBG = Request<Texture2D>("CalamityMod/UI/MiscTextures/GenericBarBack").Value;
                    var barFG = Request<Texture2D>("CalamityMod/UI/MiscTextures/GenericBarFront").Value;

                    Vector2 drawPos = Owner.Center - Main.screenPosition + new Vector2(0, -36) - barBG.Size() / 2;
                    Rectangle frame = new Rectangle(0, 0, (int)((Timer - ParryTime) / (MaxTime - ParryTime) * barFG.Width), barFG.Height);

                    float opacity = Timer <= ParryTime + 25f ? (Timer - ParryTime) / 25f : (MaxTime - Timer <= 8) ? Projectile.timeLeft / 8f : 1f;
                    Color color = Main.hslToRgb((Main.GlobalTimeWrappedHourly * 0.6f) % 1, 1, 0.85f + (float)Math.Sin(Main.GlobalTimeWrappedHourly * 3f) * 0.1f);

                    Main.spriteBatch.Draw(barBG, drawPos, color * opacity);
                    Main.spriteBatch.Draw(barFG, drawPos, frame, color * opacity * 0.8f);
                }
                return false;
            }
            Texture2D sword = Request<Texture2D>("CalamityMod/Items/Weapons/Melee/FracturedArk").Value;
            Texture2D glowmask = Request<Texture2D>("CalamityMod/Items/Weapons/Melee/FracturedArkGlow").Value;

            float drawRotation = Projectile.rotation + MathHelper.PiOver4;
            Vector2 drawOrigin = new Vector2(0f, sword.Height);
            Vector2 drawOffset = Owner.Center + Projectile.velocity * DistanceFromPlayer.Length() - Main.screenPosition;

            Main.EntitySpriteDraw(sword, drawOffset, null, lightColor, drawRotation, drawOrigin, Projectile.scale, 0f, 0);
            Main.EntitySpriteDraw(glowmask, drawOffset, null, Color.Lerp(lightColor, Color.White, 0.75f), drawRotation, drawOrigin, Projectile.scale, 0f, 0);

            if (AlreadyParried > 0)
            {
                drawOrigin = new Vector2(0f, 36f);
                Rectangle frame = new Rectangle(24, 0, 36, 36);
                drawOffset = Owner.Center + Projectile.velocity * (DistanceFromPlayer.Length() + 33) - Main.screenPosition;
                Main.EntitySpriteDraw(glowmask, drawOffset, frame, Main.hslToRgb(Main.GlobalTimeWrappedHourly % 1, 1, 0.8f) * (1 - AlreadyParried / ParryTime), drawRotation, drawOrigin, Projectile.scale + AlreadyParried / ParryTime, 0f, 0);
            }

            return false;
        }

        public override void OnKill(int timeLeft)
        {
            //Play a blip when it dies, to indicate to the player its ready to get used again
            if (Main.myPlayer == Owner.whoAmI)
                SoundEngine.PlaySound(SoundID.Item35);
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(initialized);
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            initialized = reader.ReadBoolean();
        }
    }
}
