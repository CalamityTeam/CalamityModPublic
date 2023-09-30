using System;
using CalamityMod.Items.Weapons.Magic;
using CalamityMod.Particles;
using Microsoft.Xna.Framework;
using ReLogic.Utilities;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Magic
{
    public class WulfrumManaDrain : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Magic";
        private SlotId SuccSoundSlot;
        public Player Owner => Main.player[Projectile.owner];
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public ref float Timer => ref Projectile.ai[0];

        public override void SetDefaults()
        {
            Projectile.width = 8;
            Projectile.height = 8;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.timeLeft = 2;
        }
        public override bool ShouldUpdatePosition() => false;
        public override bool? CanDamage() => false;



        public override void AI()
        {
            Owner.itemTime = 2;
            Owner.itemAnimation = 2;

            if (!Owner.Calamity().mouseRight || Owner.dead || Owner.frozen || !Owner.active || Owner.statMana == Owner.statManaMax2)
                return;

            //Once again the sound volume scaling does NOT work for whatever reason? Help.

            if (!SoundEngine.TryGetActiveSound(SuccSoundSlot, out var idleSoundOut) || !idleSoundOut.IsPlaying)
            {
                SuccSoundSlot = SoundEngine.PlaySound(WulfrumProsthesis.SuckSound with { Volume = WulfrumProsthesis.SuckSound.Volume * 0.01f ,IsLooped = true }, Owner.Center);

            }

            else if (idleSoundOut != null)
            {
                idleSoundOut.Position = Owner.Center;
                idleSoundOut.Volume = Math.Clamp((Timer / 30f) + 0.001f, 0f, 1f) * 100f;
            }
            

            Projectile.timeLeft = 2;
            Projectile.Center = Owner.MountedCenter;
            Projectile.velocity = (Owner.Calamity().mouseWorld - Owner.MountedCenter).SafeNormalize(Vector2.One);

            if (Main.rand.NextBool(6))
            {
                Particle streak = new ManaDrainStreak(Owner, Main.rand.NextFloat(0.2f, 0.5f), Projectile.velocity.RotatedByRandom(MathHelper.PiOver2 * 0.6f) * Main.rand.NextFloat(70f, 150f), Main.rand.NextFloat(30f, 44f), Color.GreenYellow, Color.DeepSkyBlue, Main.rand.Next(13, 20));
                GeneralParticleHandler.SpawnParticle(streak);
            }

            NPC target = GetSuccTarget();

            if (target != null)
            {
                Owner.GetModPlayer<WulfrumProsthesisPlayer>().ManaDrainActive = true;

                if (!Main.rand.NextBool(3))
                {
                    Vector2 center = target.Center;
                    center.X += (float)Main.rand.Next(-100, 100) * 0.1f;
                    center.Y += (float)Main.rand.Next(-100, 100) * 0.1f;

                    center += target.velocity;
                    //Drain ganggg

                    Particle bloom = new ManaDrainBlob(Owner, center, Main.rand.NextVector2Circular(4f, 4f), Main.rand.NextFloat(0.7f, 0.9f), Color.DeepSkyBlue);
                    GeneralParticleHandler.SpawnParticle(bloom);
                }
            }

            Timer++;
        }

        public NPC GetSuccTarget()
        {
            float collisionPoint = 0f;

            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC struckNPC = Main.npc[i];

                if (!struckNPC.active || struckNPC.townNPC || struckNPC.friendly)
                    continue;

                float distance = struckNPC.Distance(Projectile.Center);
                float extraDistance = struckNPC.width / 2 + struckNPC.height / 2;

                if (distance - extraDistance < 400f)
                {
                    if (!Collision.CheckAABBvLineCollision(struckNPC.Hitbox.TopLeft(), struckNPC.Hitbox.Size(), Owner.MountedCenter, Owner.MountedCenter + Projectile.velocity * 400f, 110f, ref collisionPoint))
                        continue;

                    if (!Collision.CanHit(Projectile.Center, 1, 1, struckNPC.Center, 1, 1) && extraDistance < distance)
                        continue;

                    return struckNPC;
                }
            }

            return null;
        }

        public override void OnKill(int timeLeft)
        {
            if (Timer > 2)
            {
                NPC target = GetSuccTarget();

                if (target != null)
                {
                    int particlesCount = Main.rand.Next(5, 10);
                    for (int i = 0; i < particlesCount; i++)
                    {
                        Vector2 center = target.Center;
                        center.X += (float)Main.rand.Next(-100, 100) * 0.1f;
                        center.Y += (float)Main.rand.Next(-100, 100) * 0.1f;

                        center += target.velocity;
                        //Drain ganggg

                        Particle bloom = new ManaDrainBlob(Owner, center, Main.rand.NextVector2Circular(4f, 4f), Main.rand.NextFloat(0.76f, 1f), Color.DeepSkyBlue);
                        GeneralParticleHandler.SpawnParticle(bloom);
                    }
                }
            }

            if (SoundEngine.TryGetActiveSound(SuccSoundSlot, out var soundOut))
            {
                soundOut.Stop();

                SoundEngine.PlaySound(WulfrumProsthesis.SuckStopSound with { Volume = WulfrumProsthesis.SuckStopSound.Volume * Timer / 30f}, Projectile.Center);

            }
        }
    }
}
