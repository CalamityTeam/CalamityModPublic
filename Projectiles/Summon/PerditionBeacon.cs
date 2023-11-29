using System;
using CalamityMod.Buffs.Summon;
using CalamityMod.Projectiles.BaseProjectiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
using Terraria.Utilities;

namespace CalamityMod.Projectiles.Summon
{
    public class PerditionBeacon : BaseMinionProjectile
    {
        public override int AssociatedProjectileTypeID => ModContent.ProjectileType<PerditionBeacon>();
        public override int AssociatedBuffTypeID => ModContent.BuffType<PerditionBuff>();
        public override ref bool AssociatedMinionBool => ref ModdedOwner.perditionBeacon;
        public override float MinionSlots => 5f;

        public ref float AttackTime => ref Projectile.ai[0];
        public ref float AttackTimer => ref Projectile.ai[1];
        public ref float DownwardCrossFade => ref Projectile.localAI[1];

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            Main.projFrames[Type] = 16;
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 48;
            Projectile.height = 90;
        }

        public override void SetOwnerTarget()
        {
            base.SetOwnerTarget();
            // Only attack targets if one is explicitly defined. Don't default to a closest target.
            Target = Owner.HasMinionAttackTargetNPC ? Main.npc[Owner.MinionAttackTargetNPC] : null;
        }

        public override void MinionAI()
        {
            // Fade in.
            Projectile.alpha = Utils.Clamp(Projectile.alpha - 8, 0, 255);

            FollowOwner();
            if (Target != null && Projectile.WithinRange(Target.Center, 2200f))
            {
                AttackTarget();
                DownwardCrossFade = MathHelper.Clamp(DownwardCrossFade + 0.025f, 0f, 1f);
                AttackTime++;
            }
            else
            {
                DownwardCrossFade = MathHelper.Clamp(DownwardCrossFade - 0.025f, 0f, 1f);
                AttackTime = 0f;
            }

            AttackTime++;
        }

        #region AI Methods

        internal void FollowOwner()
        {
            Vector2 destination = Owner.Top - Vector2.UnitY * MathHelper.Lerp(20f, 40f, (float)Math.Cos(Projectile.timeLeft / 24f) * 0.5f + 0.5f);
            Projectile.Center = Vector2.Lerp(Projectile.Center, destination, 0.025f);
            Projectile.Center += (destination - Projectile.Center).SafeNormalize(Vector2.Zero) * 3f;

            if (Projectile.WithinRange(destination, 5f) || !Projectile.WithinRange(destination, 2200f))
                Projectile.Center = destination;
        }

        internal void AttackTarget()
        {
            // Release cinders around the target.
            Dust cinder = Dust.NewDustPerfect(Target.Center + Main.rand.NextVector2Circular(800f, 800f), 6);
            cinder.velocity = Vector2.UnitY * -Main.rand.NextFloat(3f, 7f);
            cinder.scale = 1f + cinder.velocity.Length() * 0.17f;
            cinder.noGravity = true;

            int shootRate = (int)MathHelper.Lerp(24f, 6f, Utils.GetLerpValue(0f, 300f, AttackTime, true));
            AttackTimer++;

            if (AttackTimer < shootRate || Main.myPlayer != Projectile.owner)
                return;

            AttackTimer = 0f;
            Projectile.netUpdate = true;

            WeightedRandom<int> rng = new WeightedRandom<int>(Projectile.identity * 2167 + (int)(Main.GlobalTimeWrappedHourly * 20));
            rng.Add(ModContent.ProjectileType<LostSoulGold>(), 0.5f);
            rng.Add(ModContent.ProjectileType<LostSoulGiant>(), 0.6f);
            rng.Add(ModContent.ProjectileType<LostSoulLarge>(), 0.8f);
            rng.Add(ModContent.ProjectileType<LostSoulSmall>(), 1f);

            Vector2 spawnPosition = Target.Center + Vector2.UnitY.RotatedByRandom(0.27f) * 1150f;
            Vector2 shootVelocity = (Target.Center - spawnPosition).SafeNormalize(-Vector2.UnitY).RotatedByRandom(0.09f) * Main.rand.NextFloat(19f, 31f);
            int soul = Projectile.NewProjectile(Projectile.GetSource_FromThis(), spawnPosition, shootVelocity, rng.Get(), Projectile.damage, Projectile.knockBack, Projectile.owner);
            if (Main.projectile.IndexInRange(soul))
            {
                Main.projectile[soul].originalDamage = Projectile.originalDamage;
                Main.projectile[soul].DamageType = DamageClass.Summon;
            }
        }

        #endregion

        public override void OnSpawn(IEntitySource source)
        {
            // Release a burst of fire dust on spawn.
            if (Main.dedServ)
                return;

            for (int i = 0; i < 55; i++)
            {
                Dust fire = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(35f, 35f), 267);
                fire.velocity = Vector2.Lerp(fire.velocity, Vector2.UnitY * -Main.rand.NextFloat(3.5f, 6f), 0.5f);
                fire.color = Color.Lerp(Color.Orange, Color.Red, Main.rand.NextFloat(0f, 0.67f));
                fire.scale = Main.rand.NextFloat(1.2f, 1.5f);
                fire.noGravity = true;
            }
        }

        public override bool? CanDamage() => false;

        public override void PostDraw(Color lightColor)
        {
            if (Target is null)
                return;

            Texture2D crossTexture = ModContent.Request<Texture2D>("CalamityMod/Projectiles/Summon/PerditionCross").Value;

            Vector2 drawPosition = Target.Bottom - Main.screenPosition;
            drawPosition.Y -= 12f;
            Color drawColor = Color.White * DownwardCrossFade;
            Main.EntitySpriteDraw(crossTexture, drawPosition, null, drawColor, Projectile.rotation, crossTexture.Size() * 0.5f, Projectile.scale * 0.85f, SpriteEffects.None, 0);
        }
    }
}
