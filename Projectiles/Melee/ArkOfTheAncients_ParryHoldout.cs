using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.CalPlayer;
using CalamityMod.DataStructures;
using CalamityMod.Particles;
using CalamityMod.Items.Weapons.Melee;
using Terraria.Graphics.Shaders;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;
using static CalamityMod.CalamityUtils;


namespace CalamityMod.Projectiles.Melee
{
    public class ArkoftheAncientsParryHoldout : ModProjectile
    {
        public override string Texture => "CalamityMod/Items/Weapons/Melee/ArkoftheAncients";

        private bool initialized = false;
        const float MaxTime = 340;
        static float ParryTime = 15;
        public Vector2 DistanceFromPlayer => projectile.velocity * 10 * (1f + ((float)Math.Sin(Timer / ParryTime * MathHelper.Pi) * 0.8f));
        public float Timer => MaxTime - projectile.timeLeft;
        public ref float AlreadyParried => ref projectile.ai[1];
        public Player Owner => Main.player[projectile.owner];

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Fractured Ark");
        }
        public override void SetDefaults()
        {
            projectile.melee = true;
            projectile.width = projectile.height = 75;
            projectile.width = projectile.height = 75;
            projectile.tileCollide = false;
            projectile.friendly = true;
            projectile.penetrate = -1;
        }

        public override bool CanDamage() => Timer <= ParryTime && AlreadyParried == 0f;

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            //The hitbox is simplified into a line collision.
            float collisionPoint = 0f;
            float bladeLenght = 80f * projectile.scale;
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Owner.Center + DistanceFromPlayer, Owner.Center + DistanceFromPlayer + (projectile.velocity * bladeLenght), 24, ref collisionPoint);
        }

        public void GeneralParryEffects()
        {
            ArkoftheAncients sword = (Owner.HeldItem.modItem as ArkoftheAncients);
            if (sword != null)
                sword.Charge = 10f;

            Main.PlaySound(SoundID.DD2_WitherBeastCrystalImpact);
            Main.PlaySound(SoundID.Item67);
            CombatText.NewText(projectile.Hitbox, new Color(111, 247, 200), "Parry!", true);
            AlreadyParried = 1f;
        }
        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            if (AlreadyParried > 0)
                return;

            GeneralParryEffects();

            //only get iframes if the enemy has contact damage :)
            if (target.damage > 0)
                Owner.GiveIFrames(35);

            Vector2 particleOrigin = target.Hitbox.Size().Length() < 140 ? target.Center : projectile.Center + projectile.rotation.ToRotationVector2() * 60f;
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
                projectile.timeLeft = (int)MaxTime;
                Main.PlaySound(SoundID.DD2_SkyDragonsFuryShot, projectile.Center);

                projectile.velocity = Owner.DirectionTo(Owner.Calamity().mouseWorld);
                projectile.velocity.Normalize();
                projectile.rotation = projectile.velocity.ToRotation();

                initialized = true;
                projectile.netUpdate = true;
                projectile.netSpam = 0;
            }

            //Manage position and rotation
            projectile.Center = Owner.Center + DistanceFromPlayer ;
            projectile.scale = 1.4f + ((float)Math.Sin(Timer / 160f * MathHelper.Pi) * 0.6f); //SWAGGER

            if (Timer > ParryTime)
                return;

            if (AlreadyParried == 0)
            {
                float collisionPoint = 0f;
                float bladeLenght = 80f * projectile.scale;

                for (int k = 0; k < Main.maxProjectiles; k++)
                {
                    Projectile proj = Main.projectile[k];

                    if (proj.active && proj.hostile && proj.damage > 1 && //Only parry harmful projectiles
                        proj.velocity.Length() * (proj.extraUpdates + 1) > 1f && //Only parry projectiles that move semi-quickly
                        proj.Size.Length() < 300 && //Only parry projectiles that aren't too large 
                        Collision.CheckAABBvLineCollision(proj.Hitbox.TopLeft(), proj.Hitbox.Size(), Owner.Center + DistanceFromPlayer, Owner.Center + DistanceFromPlayer + (projectile.velocity * bladeLenght), 24, ref collisionPoint))
                    {
                        GeneralParryEffects();

                        //Reduce the projectile's damage by 100 for a second.
                        if (proj.Calamity().damageReduction < 100)
                            proj.Calamity().damageReduction = 100;
                        if (proj.Calamity().damageReductionTimer < 60)
                            proj.Calamity().damageReductionTimer = 60;

                        //Bounce off the player if they are in the air
                        if (Owner.velocity.Y != 0)
                            Owner.velocity += Vector2.Normalize(Owner.Center - proj.Center) * 2;
                        break;
                    }
                }
            }

            //Make the owner look like theyre holding the sword bla bla
            Owner.heldProj = projectile.whoAmI;
            Owner.direction = Math.Sign(projectile.velocity.X);
            Owner.itemRotation = projectile.rotation;
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

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            //Stop drawing the sword. Draw a recharge bar instead
            if (Timer > ParryTime)
            {
                if (Main.myPlayer == Owner.whoAmI)
                {
                    var barBG = GetTexture("CalamityMod/ExtraTextures/GenericBarBack");
                    var barFG = GetTexture("CalamityMod/ExtraTextures/GenericBarFront");

                    Vector2 drawPos = Owner.Center - Main.screenPosition + new Vector2(0, -36) - barBG.Size() / 2;
                    Rectangle frame = new Rectangle(0, 0, (int)((Timer - ParryTime) / (MaxTime - ParryTime) * barFG.Width), barFG.Height);

                    float opacity = Timer <= ParryTime + 25f ? (Timer - ParryTime) / 25f : (MaxTime - Timer <= 8) ? projectile.timeLeft / 8f : 1f;
                    Color color = Main.hslToRgb((Main.GlobalTime * 0.6f) % 1, 1, 0.85f + (float)Math.Sin(Main.GlobalTime * 3f) * 0.1f);

                    spriteBatch.Draw(barBG, drawPos, color * opacity);
                    spriteBatch.Draw(barFG, drawPos, frame, color * opacity * 0.8f);


                }
                return false;
            }
            Texture2D sword = GetTexture("CalamityMod/Items/Weapons/Melee/ArkoftheAncients");
            Texture2D glowmask = GetTexture("CalamityMod/Items/Weapons/Melee/ArkoftheAncientsGlow");

            float drawRotation = projectile.rotation + MathHelper.PiOver4;
            Vector2 drawOrigin = new Vector2(0f, sword.Height);
            Vector2 drawOffset = Owner.Center + projectile.velocity * DistanceFromPlayer.Length() - Main.screenPosition;


            spriteBatch.Draw(sword, drawOffset, null, lightColor, drawRotation, drawOrigin, projectile.scale, 0f, 0f);
            spriteBatch.Draw(glowmask, drawOffset, null, Color.Lerp(lightColor, Color.White, 0.75f), drawRotation, drawOrigin, projectile.scale, 0f, 0f);

            if (AlreadyParried > 0)
            {
                drawOrigin = new Vector2(0f, 36f);
                Rectangle frame = new Rectangle(24, 0, 36, 36);
                drawOffset = Owner.Center + projectile.velocity * (DistanceFromPlayer.Length() + 33) - Main.screenPosition;
                spriteBatch.Draw(glowmask, drawOffset, frame, Main.hslToRgb(Main.GlobalTime % 1, 1, 0.8f) * (1 - AlreadyParried / ParryTime), drawRotation, drawOrigin, projectile.scale + AlreadyParried / ParryTime, 0f, 0f);
            }

            return false;
        }

        public override void Kill(int timeLeft)
        {
            //Play a blip when it dies, to indicate to the player its ready to get used again
            if (Main.myPlayer == Owner.whoAmI)
                Main.PlaySound(SoundID.Item35);
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