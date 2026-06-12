using System;
using System.Collections.Generic;
using System.ComponentModel;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Particles;
using CalamityMod.Projectiles.BaseProjectiles;
using CalamityMod.Projectiles.Pets;
using CalamityMod.Projectiles.Rogue;
using CalamityMod.Sounds;
using CalamityMod.Utilities.Daybreak;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Melee.Shortswords
{
    public class GalileoGladiusThrown : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Melee";
        public Player Owner => Main.player[Projectile.owner];
        public override string Texture => "CalamityMod/Items/Weapons/Melee/GalileoGladius";

        public Vector2 NPCOffset = Vector2.Zero;
        public NPC stabbedNPC
        {
            get
            {
                if (Main.npc.IndexInRange((int)Projectile.ai[0] - 1))
                {
                    var npc = Main.npc[(int)Projectile.ai[0] - 1];
                    if (npc.active)
                        return npc;
                    else
                        Projectile.ai[0] = 0;
                }
                return null;
            }
            set
            {
                if (value is null)
                    Projectile.ai[0] = 0;
                else
                    Projectile.ai[0] = value.whoAmI + 1;
            }
        }

        public override void SetDefaults()
        {
            Projectile.width = 44;
            Projectile.height = 46;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.penetrate = 1;
            Projectile.stopsDealingDamageAfterPenetrateHits = true;
            Projectile.extraUpdates = 2;
            Projectile.timeLeft = 300;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            ProjectileID.Sets.DrawScreenCheckFluff[Type] = 48000;
        }
        float intensitymult = 0.2f;
        public override void AI()
        {
            if (Projectile.ai[2] == 0)
                Projectile.ai[2] += Main.rand.Next(1, 10000); //random start configuration for the consteallation trail
            Projectile.ai[2]++; //timer for visuals
            var point1 = Projectile.Center + Projectile.velocity + new Vector2(-17, 18).RotatedBy(Projectile.rotation);
            var point2 = Owner.Center;
            float intensity = MathHelper.Min(32, point1.Distance(point2) * 0.1f);
            float size = intensity / 32f * 0.75f;
            if (Projectile.ai[1] > 0 || stabbedNPC is null)
            {
                intensitymult = MathHelper.Max(0.2f, intensitymult - 0.05f);
            } else
            {
                intensitymult = MathHelper.Min(1f, intensitymult + 0.05f);
            }
            intensity *= intensitymult;
            if (Projectile.FinalExtraUpdate())
                for (var i = 0; i < offsets.Count - 1; i++)
                {
                    var p1 = Vector2.Lerp(point1, point2, i / (float)(offsets.Count - 1)) + new Vector2(0, intensity).RotatedBy(point1.DirectionTo(point2).ToRotation()) * MathF.Sin(Projectile.ai[2] * 0.01f * offsets[i].Item2 + offsets[i].Item1);
                    var star = new BloomParticle(p1, Vector2.Zero, Color.SkyBlue * 0.75f, (i == 0 || i == offsets.Count - 1 ? 0.1f : 0.2f) * size, (i == 0 || i == offsets.Count - 1 ? 0.1f : 0.2f) * size, 2, false);
                    var star2 = new CustomSpark(p1, Vector2.UnitX.RotatedBy(MathHelper.Pi * ((Owner.miscCounter) / 300f)) * 0.1f, "CalamityMod/Particles/Sparkle", false, 2, (i == 0 || i == offsets.Count - 1 ? 0.4f : 0.8f) * size, Color.White, Vector2.One);
                    GeneralParticleHandler.SpawnParticle(star);
                    GeneralParticleHandler.SpawnParticle(star2);
                }
            if (stabbedNPC != null)
            {
                Projectile.Center = stabbedNPC.Center + NPCOffset;
                Projectile.timeLeft++;
                Projectile.velocity = Vector2.Zero;
            }
            else
            {
                Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver4;
                if (Projectile.Distance(Owner.Center) > 1000)
                {
                    Projectile.ai[1] = 1;
                }
                if (Projectile.ai[1] == 1)
                {
                    Projectile.velocity = Vector2.Lerp(Projectile.velocity,Projectile.DirectionTo(Owner.Center) * 15,0.1f);
                    Projectile.rotation += MathHelper.Pi;
                    if (Projectile.Distance(Owner.Center) < 16)
                    {
                        Projectile.Kill();
                        return;
                    }
                }
            }
            if (Projectile.ai[1] == 3)
            {

                Owner.mount.Dismount(Owner);
                Owner.SetImmuneTimeForAllTypes(3);
                Owner.velocity = Owner.DirectionTo(Projectile.Center) * 4;
                Owner.Center += Owner.DirectionTo(Projectile.Center) * 16;
                if (Collision.SolidCollision(Owner.position, Owner.width, Owner.height))
                {
                    Projectile.ai[1] = 2;
                    Owner.velocity *= -2;
                    Owner.Center += Owner.velocity * 2;
                }
                if (Projectile.Distance(Owner.Center) < 64)
                    Projectile.ai[1] = 4;
            }
            if (Projectile.ai[1] == 2)
            {
                Projectile.ai[1] = 1;
                Projectile.penetrate = 1;
                Projectile.damage = Projectile.originalDamage;
                if (Owner.Calamity().AvaliableStarburst >= 10)
                {
                    for (var i = 0; i < 10; i++)
                        GeneralParticleHandler.SpawnParticle(new SparkParticle(Projectile.Center, (Projectile.rotation-MathHelper.PiOver4).ToRotationVector2().RotatedByRandom(0.4f) * -Main.rand.Next(10,20), false, 30, 2, new Color(69, 69, 200)));
                    Projectile.damage = (int)(Projectile.damage*10f);
                    Projectile.Damage();
                    Owner.Calamity().StratusStarburst -= 10;
                    for (int i = 0; i < 3; i++)
                    {
                        float moveDuration = Main.rand.Next(5, 15);
                        var proj = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, (Projectile.rotation + MathHelper.PiOver4*3).ToRotationVector2().RotatedByRandom(0.4f) * Main.rand.NextFloat(0.75f, 1.25f), ModContent.ProjectileType<VegaStar>(), Projectile.originalDamage, Projectile.knockBack, Projectile.owner, 0f, moveDuration);
                        if (Main.projectile.IndexInRange(proj))
                        {
                            Main.projectile[proj].DamageType = DamageClass.Melee;
                            Main.projectile[proj].usesIDStaticNPCImmunity = false;
                            Main.projectile[proj].usesLocalNPCImmunity = true;
                            Main.projectile[proj].localNPCHitCooldown = 20;
                            Main.projectile[proj].timeLeft = Main.projectile[proj].MaxUpdates * 600;
                        }
                    }
                    Projectile.velocity = (Projectile.rotation - MathHelper.PiOver4).ToRotationVector2() * -15;
                    SoundEngine.PlaySound(SoundID.DD2_MonkStaffGroundImpact, Owner.Center);
                    
                }
                stabbedNPC = null;
            }
            if (Projectile.ai[1] > 3)
            {
                Projectile.ai[1]++;
                Owner.Center = Projectile.Center;
                Owner.SetImmuneTimeForAllTypes(3);
                Owner.velocity = Owner.velocity.SafeNormalize(Vector2.Zero) * 2;
                if (Collision.SolidCollision(Owner.position, Owner.width, Owner.height))
                {
                    Projectile.ai[1] = 2;
                    Owner.position = Owner.oldPosition;
                    Owner.SetImmuneTimeForAllTypes(12);
                }
                var particlevel = Owner.DirectionFrom(Owner.Calamity().mouseWorld);
                GeneralParticleHandler.SpawnParticle(new CustomSpark(Owner.Center + particlevel * 96, particlevel, "CalamityMod/Particles/BloomCircle", false, 2, 0.2f, Color.SkyBlue, new Vector2(0.3f, 3f), shrinkSpeed: 0.2f));
            }
            if (Projectile.ai[1] >= 34 && !Owner.controlUseTile || Projectile.ai[1] >= 64)
            {
                Owner.velocity = Owner.DirectionFrom(Owner.Calamity().mouseWorld) * 15;
                Owner.Center = Projectile.Center;
                Projectile.penetrate = 1;
                Projectile.damage = Projectile.originalDamage;
                if (Owner.Calamity().AvaliableStarburst >= 20)
                {
                    Projectile.damage = (int)(Projectile.damage*20f);
                    Owner.SetImmuneTimeForAllTypes(Owner.longInvince ? 40 : 20);
                    Owner.Calamity().StratusStarburst -= 20;
                    SoundEngine.PlaySound(SoundID.DD2_MonkStaffGroundImpact, Owner.Center);
                    SoundEngine.PlaySound(SoundID.DD2_SonicBoomBladeSlash, Owner.Center);
                    for (var i = 0; i < 10; i++)
                        GeneralParticleHandler.SpawnParticle(new SparkParticle(Projectile.Center, Owner.velocity.RotatedByRandom(0.4f) * Main.rand.NextFloat(0.75f, 1.25f), false, 30, 2, new Color(69, 69, 200)));
                    for (int i = 0; i < 3; i++)
                    {
                        float moveDuration = Main.rand.Next(5, 15);
                        var proj = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Owner.velocity.RotatedByRandom(0.4f) * -Main.rand.NextFloat(0.75f, 1.25f), ModContent.ProjectileType<VegaStar>(), Projectile.originalDamage, Projectile.knockBack, Projectile.owner, 0f, moveDuration);
                        if (Main.projectile.IndexInRange(proj))
                        {
                            Main.projectile[proj].DamageType = DamageClass.Melee;
                            Main.projectile[proj].usesIDStaticNPCImmunity = false;
                            Main.projectile[proj].usesLocalNPCImmunity = true;
                            Main.projectile[proj].localNPCHitCooldown = 20;
                            Main.projectile[proj].timeLeft = Main.projectile[proj].MaxUpdates * 600;

                        }
                    }
                }
                Projectile.stopsDealingDamageAfterPenetrateHits = false;
                Projectile.ai[1] = 1;
                Vector2 position = Projectile.Center - Owner.velocity * 5;
                Vector2 velocity = Owner.velocity.SafeNormalize(Vector2.One) * -5;
                int lifetime = 120;
                float scale = 0.1f;
                Color color = Color.SkyBlue;
                Vector2 stretch = new Vector2(0.5f, 0.5f);
                float shrink = -0.3f;
                Particle boostRing = new CustomSpark(position, velocity, "CalamityMod/Particles/HighResHollowCircleHardEdgeAlt", false, lifetime, scale, color, stretch, shrinkSpeed: shrink);
                GeneralParticleHandler.SpawnParticle(boostRing);

                if (stabbedNPC.CanBeMoved())
                {
                    stabbedNPC.velocity = -Owner.velocity;
                }
                stabbedNPC = null;
                return;
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Projectile.ai[1] == 0)
            {
                stabbedNPC = target;
                NPCOffset = Projectile.Center - target.Center;
            }
            target.AddBuff(ModContent.BuffType<Voidfrost>(), 600);
        }

        List<(float, float)> offsets = new()
            {
                (0,0),
                (0,1),
                (4,2),
                (2,1.2f),
                (1,0.2f),
                (0.75f,1.1f),
                (0,0)
            };
        public override bool PreDraw(ref Color lightColor)
        {

            var point1 = Projectile.Center + new Vector2(-17, 18).RotatedBy(Projectile.rotation);
            var point2 = Owner.Center;
            float intensity = MathHelper.Min(32, point1.Distance(point2) * 0.1f);
            float size = intensity/32f;
            intensity *= intensitymult;
            var color = Color.SkyBlue * 0.75f * ((MathF.Sin(Main.GlobalTimeWrappedHourly) + 1) * 0.25f + 0.5f);
            using (Main.spriteBatch.Scope())
            {
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, SamplerState.PointClamp, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
                for (var i = 1; i < offsets.Count; i++)
                {
                    var p1 = Vector2.Lerp(point1, point2, i / (float)(offsets.Count - 1)) + new Vector2(0, intensity).RotatedBy(point1.DirectionTo(point2).ToRotation()) * MathF.Sin(Projectile.ai[2] * 0.01f * offsets[i].Item2 + offsets[i].Item1);
                    var p2 = Vector2.Lerp(point1, point2, (i - 1) / (float)(offsets.Count - 1)) + new Vector2(0, intensity).RotatedBy(point1.DirectionTo(point2).ToRotation()) * MathF.Sin(Projectile.ai[2] * 0.01f * offsets[i - 1].Item2 + offsets[i - 1].Item1);
                    CalamityUtils.DrawLineBetter(Main.spriteBatch, p1, p2, color, 2 * size);
                }
                Main.spriteBatch.End();
            }
            return base.PreDraw(ref lightColor);
        }

    }
}
