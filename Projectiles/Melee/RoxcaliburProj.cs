using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items.Weapons.Melee;

namespace CalamityMod.Projectiles.Melee
{
    public class RoxcaliburProj : ModProjectile, ILocalizedModType
    {
        public string LocalizationCategory => "Projectiles.Melee";

        public override string Texture => "CalamityMod/Items/Weapons/Melee/Roxcalibur";

        public override void SetDefaults()
        {
            Projectile.penetrate = -1;
            Projectile.width = 100;
            Projectile.height = 100;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
	    Projectile.scale = 1.3f;
            Projectile.timeLeft = 3600;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 150;
        }
        private const int Charging = 0, Swinging = 1, Plunging = 2, Recoil = 3;
        private int Animation = Charging;

        const int plungeSpeed = 20;

        public float BaseChargeTime = 60 * 2;
        ref float ChargeLevel => ref Projectile.ai[0];
        ref float HitTimer => ref Projectile.ai[1];
        private Vector2 RockOffset() => Projectile.velocity * 0.25f * Projectile.Size.Length();

        public override bool? CanDamage() => HitTimer < 5 && Animation != Charging ? base.CanDamage() : false; //returning true means it can damage friendlies etc

        //large "explosion" hitbox that lasts for 5 frames after bonk hit
        public override void ModifyDamageHitbox(ref Rectangle hitbox)
        {
            if (HitTimer > 0 && HitTimer < 5)
            {
                float hitboxModifier = 2f + 1.5f * ChargeLevel;
                Vector2 HitboxSize = new(Projectile.width * hitboxModifier, Projectile.height * hitboxModifier);
                Vector2 HitboxCenter = Projectile.Center + RockOffset();
                hitbox = new Rectangle((int)(HitboxCenter.X - HitboxSize.X / 2f), (int)(HitboxCenter.Y - HitboxSize.Y / 2f), (int)HitboxSize.X, (int)HitboxSize.Y);
            }
            
        }

        //more precise hitbox. makes the weapon harder to use but looks visually better: use it if you want.
/*
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (HitTimer <= 0)
            {
                float size = Projectile.Size.Length();
                Vector2 hiltPos = Projectile.Center - Projectile.velocity * size / 2;
                Vector2 endPos = Projectile.Center + Projectile.velocity * size / 2;
                float useless = 0f;
                return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), hiltPos, endPos, size / 2, ref useless);
            }
            return base.Colliding(projHitbox, targetHitbox);
        }
*/

        public override void AI()
        {
            ref float origAnimMax = ref Projectile.ai[2];

            ref float rotation = ref Projectile.localAI[0];
            ref float playerDirection = ref Projectile.localAI[1];
            ref float PlayedChargeSound = ref Projectile.localAI[2];

            Player player = Main.player[Projectile.owner];
            if (player == null || !player.active || player.dead)
            {
                Projectile.Kill();
                return;
            }
            player.heldProj = Projectile.whoAmI;
            if (origAnimMax == 0)
            {
                origAnimMax = player.itemAnimationMax;
            }
            float HoldoutRadius = (float)Projectile.Size.Length() / 2; //since sprite is diagonal

            float attackSpeedModifier = origAnimMax / Roxcalibur.BaseUseTime;
            float chargeTime = attackSpeedModifier * BaseChargeTime;

            if (player.channel)
            {
                Animation = Charging;
            }
            else if (Animation == Charging) //first frame of release
            {
                Animation = Main.MouseWorld.Y > player.Center.Y ? Plunging : Swinging;
                float chargeModifier = ChargeLevel * 2.5f; //modify this to balance damage bonus from charging

                if (Animation == Plunging)
                {
                    Projectile.timeLeft = 60 * 2; //maximum 2 second plunge time
		    chargeModifier = ChargeLevel * 3.5f;
                }
                    
                Projectile.damage = (int)(Projectile.damage * (1 + chargeModifier));
                Projectile.netUpdate = true;
                SoundEngine.PlaySound(new SoundStyle("CalamityMod/Sounds/Custom/LoudSwingWoosh") with { Pitch = -0.5f }, Projectile.Center);
            }
            switch (Animation)
            {
                case Charging:
                    {
                        if (ChargeLevel < 1)
                            ChargeLevel += 1f / chargeTime;
                        else
                            ChargeLevel = 1;

                        int direction = -Math.Sign(player.DirectionTo(Main.MouseWorld).X);
                        playerDirection = -direction;
                        float rotModifier = (float)Math.Pow(ChargeLevel, 0.4f);
                        rotation = Utils.SmoothStep(-MathHelper.PiOver2 / 4, MathHelper.PiOver2 / 1.5f, rotModifier);
                        rotation *= direction;
                        Projectile.timeLeft = (int)origAnimMax;
                        

                        if (ChargeLevel >= 1 && player.whoAmI == Main.myPlayer && PlayedChargeSound == 0)
                        {
                            PlayedChargeSound = 1;
                            SoundEngine.PlaySound(SoundID.NPCHit42 with { Pitch = 0.4f }, Projectile.Center);
                        }
                    }
                    break;
                case Swinging:
                    {
                        if (Projectile.timeLeft > origAnimMax)
                        {
                            Projectile.timeLeft = (int)origAnimMax;
                        }

                        float progress = Projectile.timeLeft / origAnimMax;

                        float chargeRotModifier = (ChargeLevel + 1f) / 2f;
                        float animRotModifier = Math.Clamp(progress, 0, 1);
                        if (animRotModifier > 0.5f)
                            animRotModifier = 1 - animRotModifier;
                        animRotModifier *= animRotModifier;
                        animRotModifier += 0.02f;
                        rotation += playerDirection * animRotModifier * chargeRotModifier * MathHelper.TwoPi * 0.2f;

                        Projectile.localNPCHitCooldown = (int)Math.Ceiling(origAnimMax); //only hit once per swing
                        //int shardTimer = (int)Math.Ceiling(((1 - ChargeLevel) * 0.6f + 0.4f) * 9);
                        int shardTimer = ChargeLevel >= 0.95f ? 2 : ChargeLevel >= 0.5f ? 4 : 6;
                        if (progress < 0.8f && progress > 0.2f && Projectile.timeLeft % shardTimer == 0)
                        {
                            Vector2 roxSpeed = (Projectile.velocity * 8f).RotatedByRandom((double)MathHelper.ToRadians(10f));
                            int rox = ModContent.ProjectileType<Rox1>();
                            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center + RockOffset(), roxSpeed, rox, (int)(Projectile.damage * 0.4f), 1f, player.whoAmI, (float)Main.rand.Next(3), 0f, 0f);
                        }
                    }
                    break;
                case Plunging:
                    {
                        //I would make this set your velocity.Y if it wasn't capped, tried uncapping it by changing Player.maxFallSpeed but it does not work properly!
                        //this also makes you go through platforms which is cool and good imo
                        Vector2 plunge = Vector2.UnitY * player.gravDir * plungeSpeed * (ChargeLevel + 1f) / 2f;
                        if ((player.velocity.Y == 0 && Projectile.velocity.Y * player.gravDir > 0) || Collision.SolidCollision(player.position + plunge, player.width, player.height))
                        {
                            CollisionEffects();
                            break;
                        }
                        player.position += plunge;
                        player.velocity.X *= 0.95f;
                        player.velocity.Y = 0;

                        float chargeRotModifier = (ChargeLevel + 1f) / 2f;

                        float anglediff = RotationDifference(Projectile.velocity, Vector2.UnitY * player.gravDir);
                        int dir = Math.Abs(anglediff) > MathHelper.PiOver2 ? (int)playerDirection : Math.Sign(anglediff);
                        rotation += dir * Math.Abs(anglediff) * 0.12f * chargeRotModifier;
                        //rotation += playerDirection * animRotModifier * chargeRotModifier * MathHelper.TwoPi * 6 / origAnimMax;

                        Projectile.localNPCHitCooldown = (int)Math.Ceiling(origAnimMax); //only hit once per swing
                        if (Collision.SolidTiles(Projectile.position, Projectile.width, Projectile.height, false))
                        {
                            CollisionEffects();
                            break;
                        }
                    }
                    break;
                case Recoil:
                    {
                        if (Projectile.timeLeft > origAnimMax / 3)
                            Projectile.timeLeft = (int)(origAnimMax / 3);
                        float modifier = Math.Clamp(Projectile.timeLeft / origAnimMax, 0, 1);
                        rotation -= playerDirection * modifier * MathHelper.TwoPi * 2 / origAnimMax;

                    }
                    break;
            }

            if (HitTimer > 0 && HitTimer <= 5)
            {
                HitTimer++;
            }

            Projectile.velocity = (-Vector2.UnitY * player.gravDir).RotatedBy(rotation);
            Projectile.Center = (Vector2)player.HandPosition + Projectile.velocity * HoldoutRadius;

            Projectile.rotation = Projectile.velocity.ToRotation();
            Projectile.spriteDirection = Projectile.direction = (int)playerDirection;
            player.ChangeDir(Projectile.direction);
            player.itemRotation = (Projectile.velocity * Projectile.direction).ToRotation();
            player.itemTime = Math.Max(player.itemTime, 2);
            player.itemAnimation = Math.Max(player.itemAnimation, 2);
            if (Projectile.spriteDirection == -1)
            {
                Projectile.rotation += MathHelper.ToRadians(-45f) + MathHelper.Pi;
            }
            else
            {
                Projectile.rotation += MathHelper.ToRadians(-135f) + MathHelper.Pi;
            }

        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            CollisionEffects();
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            Projectile.damage = (int)(Projectile.damage * 0.75f);
            if (Projectile.damage < 1)
                Projectile.damage = 1;
        }

        private void CollisionEffects()
        {
            if (HitTimer == 0)
            {
                HitTimer = 1;
                SoundEngine.PlaySound(SoundID.Item14, Projectile.Center);
                if (Animation == Plunging)
                {
                    Player player = Main.player[Projectile.owner];
                    player.velocity.Y = -player.gravDir * plungeSpeed * (ChargeLevel + 1f) / 2f;
                }
                Animation = Recoil;
                //dust ring
                const int dusts = 24;
                for (int i = 0; i < dusts; i++)
                {
                    Vector2 dir = Vector2.UnitY.RotatedBy(MathHelper.TwoPi * i / dusts);
                    Vector2 pos = Projectile.Center + RockOffset() + dir * 20 * Projectile.scale;
                    int d = Dust.NewDust(pos, 0, 0, DustID.Stone, dir.X * 12 * Projectile.scale, dir.Y * 12 * Projectile.scale, newColor: Color.Black, Scale: Projectile.scale);
                    if (d.WithinBounds(Main.maxDust))
                    {
                        Main.dust[d].noGravity = true;
                    }
                }
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Rectangle rectangle = texture.Bounds; 
            Vector2 origin2 = rectangle.Size() / 2f;
            SpriteEffects effects = Projectile.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

            Vector2 pos = Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY);

            if (Animation == Charging && ChargeLevel > 0.95f)
            {
                float shake = 2.5f * (ChargeLevel - 0.95f) / 0.05f;
                pos += Main.rand.NextVector2Circular(shake, shake);
            }

            Main.EntitySpriteDraw(texture, pos, new Microsoft.Xna.Framework.Rectangle?(rectangle), Projectile.GetAlpha(lightColor), Projectile.rotation, origin2, Projectile.scale, effects, 0);
            return false;
        }

        public static float RotationDifference(Vector2 from, Vector2 to) => (float)Math.Atan2(to.Y * from.X - to.X * from.Y, from.X * to.X + from.Y * to.Y);
    }
}
