using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Particles;
using CalamityMod.Projectiles.Summon;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Utilities;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Melee.Yoyos
{
    public class ObliteratorYoyo : ModProjectile
    {
        public override LocalizedText DisplayName => CalamityUtils.GetItemName<TheObliterator>();
        private static int DashStartup => 90;
        private static int DashCooldown => 30;
        public SlotId GFB;
        public int GFBCounter = 0;
        public int time = 0;

        public int DashQueue = 0;
        int dashTimer = 0;
        int timerToStartDash = 0;
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.YoyosLifeTimeMultiplier[Type] = -1f;
            ProjectileID.Sets.YoyosMaximumRange[Type] = TheObliterator.Reach;
            ProjectileID.Sets.YoyosTopSpeed[Type] = TheObliterator.Speed / 3f;

            ProjectileID.Sets.TrailCacheLength[Type] = 8;
            ProjectileID.Sets.TrailingMode[Type] = 1;
        }

        public override void SetDefaults()
        {
            Projectile.aiStyle = ProjAIStyleID.Yoyo;
            Projectile.width = Projectile.height = 16;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.MeleeNoSpeed;
            Projectile.penetrate = -1;
            Projectile.MaxUpdates = 3;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 7 * Projectile.MaxUpdates;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(DashQueue);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            DashQueue = reader.ReadInt32();
        }

        public override bool PreAI()
        {
            if (Projectile.FinalExtraUpdate())
            {
                timerToStartDash++;
                if (timerToStartDash % DashStartup == DashStartup - 1)
                    DashQueue++;
            }

            if (dashTimer <= 0)
                return true;
            Projectile.aiStyle = -1;
            Projectile.timeLeft++;
            dashTimer--;
            return true;
        }

        public override void AI()
        {
            time++;
            if (Main.zenithWorld)
            {
                if (time == 1)
                {
                    GFB = SoundEngine.PlaySound(new SoundStyle("CalamityMod/Sounds/Custom/BoomBoomKawaii") with { IsLooped = true });
                    GFBCounter++;
                }
                if (time % 30 == 0 && GFBCounter > 0)
                    GFBCounter--;
                for (int i = 0; i < 13; i++)
                {
                    Vector2 dustPos = Projectile.Center + (i * MathHelper.Pi + Projectile.rotation + MathHelper.PiOver2).ToRotationVector2() * 2f;
                    Dust dust = Dust.NewDustPerfect(dustPos, Main.rand.Next(130, 134 + 1), (i * MathHelper.Pi + Projectile.rotation * Math.Sign(Projectile.velocity.X)).ToRotationVector2() * Main.rand.NextFloat(1f, 90f));
                    dust.noGravity = true;
                    dust.scale = Main.rand.NextFloat(0.1f, 1.7f);
                }
                if (SoundEngine.TryGetActiveSound(GFB, out var RumblePitch) && RumblePitch.IsPlaying)
                {
                    RumblePitch.Pitch = MathHelper.Lerp(0f, 1f, MathHelper.Clamp(GFBCounter * 0.1f, 0, 1));
                    RumblePitch.Volume = MathHelper.Lerp(1f, 1.5f, GFBCounter * 0.1f);
                }
            }


            if (Projectile.FinalExtraUpdate())
            {
                if (DashQueue > 0 && dashTimer <= 0)
                {
                    DashQueue--;

                    List<NPC> targets = new List<NPC>();
                    float laserRange = 600f;
                    foreach (NPC n in Main.ActiveNPCs)
                    {
                        if (n.CanBeChasedBy(Projectile, false) && (n.Center - Projectile.Center).Length() <= laserRange && Collision.CanHit(Projectile.Center, 1, 1, n.Center, 1, 1))
                        {
                            targets.Add(n);
                        }
                    }
                    if (targets.Count == 0)
                        return;
                    targets = targets.OrderBy(x => x.Distance(Projectile.Center)).ToList();
                    Projectile.velocity = Projectile.DirectionTo(targets[0].Center);
                    Projectile.Center = targets[0].Center - new Vector2(Projectile.velocity.X * targets[0].width*0.5f, Projectile.velocity.Y * targets[0].height*0.5f);
                    Projectile.velocity *= 15;
                    for (var i = 0; i < 20; i++)
                    {
                        Projectile.Center -= Projectile.velocity;
                        if (Collision.SolidCollision(Projectile.position,Projectile.width,Projectile.height))
                        {
                            Projectile.Center += Projectile.velocity;
                            break;
                        }
                    }
                    if (Main.myPlayer == Projectile.owner)
                    {
                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<DoGWeaponTeleportRift>(), 0, 0, Projectile.owner);
                    }
                    int laserAmount = 8;
                    for (int i = 0; i < laserAmount; i++)
                    {
                        int sparkLifetime = Main.rand.Next(30, 45);
                        float sparkScale = Main.rand.NextFloat(0.8f, 1f) + 1f * 0.05f;
                        Color sparkColor = Color.Lerp(Color.Fuchsia, Color.AliceBlue, Main.rand.NextFloat(0.5f));
                        sparkColor = Color.Lerp(sparkColor, Color.Cyan, Main.rand.NextFloat());

                        if (Main.rand.NextBool(5))
                            sparkScale *= 1.4f;

                        Vector2 sparkVelocity = Projectile.velocity.SafeNormalize(Vector2.UnitX).RotatedBy(MathHelper.TwoPi * (i / (float)laserAmount)) * 4;
                        SparkParticle spark = new SparkParticle(Projectile.Center, sparkVelocity, false, sparkLifetime, sparkScale, sparkColor);
                        GeneralParticleHandler.SpawnParticle(spark);
                    }
                    dashTimer = DashCooldown;
                    Projectile.ResetLocalNPCHitImmunity();
                }
            }

            Projectile.aiStyle = ProjAIStyleID.Yoyo;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Type], lightColor, 1);
            return false;
        }

        public override void PostDraw(Color lightColor)
        {
            Vector2 origin = new Vector2(10f, 10f);
            Main.EntitySpriteDraw(ModContent.Request<Texture2D>("CalamityMod/Projectiles/Melee/Yoyos/ObliteratorYoyoGlow").Value, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation, origin, 2f, SpriteEffects.None, 0);

            if (dashTimer <= 0)
                return;
            var tex = ModContent.Request<Texture2D>("CalamityMod/Particles/Jaws").Value;
            Main.spriteBatch.SetBlendState(BlendState.Additive);
            Main.spriteBatch.Draw(tex,Projectile.Center+ Projectile.velocity.SafeNormalize(Vector2.Zero)*16f- Main.screenPosition,null,Color.Fuchsia,Projectile.velocity.ToRotation() + MathHelper.PiOver2,tex.Size()*0.5f,0.33f,SpriteEffects.None,0);
            Main.spriteBatch.Draw(tex, Projectile.Center + Projectile.velocity.SafeNormalize(Vector2.Zero) * 16f - Main.screenPosition, null, Color.Aqua, Projectile.velocity.ToRotation() + MathHelper.PiOver2, tex.Size() * 0.5f, 0.25f, SpriteEffects.None, 0);
            Main.spriteBatch.SetBlendState(BlendState.AlphaBlend);
        }
        public override void OnKill(int timeLeft)
        {
            if (Main.zenithWorld && SoundEngine.TryGetActiveSound(GFB, out var RumblePlaying) && RumblePlaying.IsPlaying)
            {
                RumblePlaying?.Stop();
            }
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            if (dashTimer > 0)
            {
                modifiers.SourceDamage *= 6;
            }
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {

            if (Projectile.localAI[1] > DashStartup)
            {
                dashTimer = 0;
                target.AddBuff(ModContent.BuffType<GodSlayerInferno>(), 180);
                for (int i = 0; i < 10; i++)
                {
                    Vector2 sparkVelocity = -Projectile.velocity.SafeNormalize(Vector2.Zero).RotatedByRandom(MathHelper.ToRadians(20f)) * Main.rand.NextFloat(26f, 32f);
                    int sparkLifetime = Main.rand.Next(10, 20);
                    float sparkScale = Main.rand.NextFloat(1.4f, 1.8f);
                    Color sparkColor = Color.Lerp(Main.rand.NextBool() ? Color.Cyan : Color.Purple, Color.White, Main.rand.NextFloat(0f, 0.3f));

                    SparkParticle chompSpark = new(Projectile.Center, sparkVelocity, false, sparkLifetime, sparkScale, sparkColor);
                    GeneralParticleHandler.SpawnParticle(chompSpark);
                }
                SoundEngine.PlaySound(new SoundStyle("CalamityMod/Sounds/NPCHit/OtherworldlyHit") with { Pitch = -0.45f, Volume = 0.33f }, Projectile.Center);
            }
            GFBCounter = 15;
        }
    }
}
