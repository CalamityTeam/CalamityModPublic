using CalamityMod.Buffs.Summon;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;

namespace CalamityMod.Projectiles.Summon
{
    public class PerditionBeacon : ModProjectile
    {
        public ref float AttackTime => ref projectile.ai[0];
        public ref float AttackTimer => ref projectile.ai[1];
        public ref float DownwardCrossFade => ref projectile.localAI[1];
        public Player Owner => Main.player[projectile.owner];

        // Only attack targets if one is explicitly defined. Don't default to a closest target.
        public NPC Target => Owner.HasMinionAttackTargetNPC ? Main.npc[Owner.MinionAttackTargetNPC] : null;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Perdition Beacon");
            Main.projFrames[projectile.type] = 16;
            ProjectileID.Sets.MinionTargettingFeature[projectile.type] = true;
        }

        public override void SetDefaults()
        {
            projectile.width = 48;
            projectile.height = 90;
            projectile.ignoreWater = true;
            projectile.tileCollide = false;
            projectile.sentry = true;
            projectile.light = 1f;
            projectile.timeLeft = Projectile.SentryLifeTime;
            projectile.penetrate = -1;
            projectile.alpha = 255;
        }

        public override void AI()
        {
            Player player = Main.player[projectile.owner];

            if (projectile.localAI[0] == 0f)
            {
                DoInitializationEffects();
                projectile.localAI[0] = 1f;
            }

            // Dynamically adjust damage of the minion.
            if (player.MinionDamage() != projectile.Calamity().spawnedPlayerMinionDamageValue)
            {
                int newDamage = (int)(projectile.Calamity().spawnedPlayerMinionProjectileDamageValue / projectile.Calamity().spawnedPlayerMinionDamageValue * player.MinionDamage());
                projectile.damage = newDamage;
            }

            // Fade in.
            projectile.alpha = Utils.Clamp(projectile.alpha - 8, 0, 255);

            HandleFrames();
            FollowOwner();
            ProvidePlayerMinionBuffs();
            if (Target != null && projectile.WithinRange(Target.Center, 2200f))
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

        internal void DoInitializationEffects()
        {
            projectile.Calamity().spawnedPlayerMinionDamageValue = Owner.MinionDamage();
            projectile.Calamity().spawnedPlayerMinionProjectileDamageValue = projectile.damage;

            // Release a burst of fire dust on spawn.
            if (Main.dedServ)
                return;

            for (int i = 0; i < 55; i++)
            {
                Dust fire = Dust.NewDustPerfect(projectile.Center + Main.rand.NextVector2Circular(35f, 35f), 267);
                fire.velocity = Vector2.Lerp(fire.velocity, Vector2.UnitY * -Main.rand.NextFloat(3.5f, 6f), 0.5f);
                fire.color = Color.Lerp(Color.Orange, Color.Red, Main.rand.NextFloat(0f, 0.67f));
                fire.scale = Main.rand.NextFloat(1.2f, 1.5f);
                fire.noGravity = true;
            }
        }

        internal void ProvidePlayerMinionBuffs()
        {
            Owner.AddBuff(ModContent.BuffType<PerditionBuff>(), 3600);

            // Verify player/minion state integrity. The minion cannot stay alive if the
            // owner is dead or if the caller of the AI is invalid.
            if (projectile.type != ModContent.ProjectileType<PerditionBeacon>())
                return;

            if (Owner.dead)
                Owner.Calamity().perditionBeacon = false;
            if (Owner.Calamity().perditionBeacon)
                projectile.timeLeft = 2;
        }

        internal void HandleFrames()
        {
            projectile.frameCounter++;
            if (projectile.frameCounter % 5 == 4)
                projectile.frame = (projectile.frame + 1) % Main.projFrames[projectile.type];
        }

        internal void FollowOwner()
        {
            Vector2 destination = Owner.Top - Vector2.UnitY * MathHelper.Lerp(20f, 40f, (float)Math.Cos(projectile.timeLeft / 24f) * 0.5f + 0.5f);
            projectile.Center = Vector2.Lerp(projectile.Center, destination, 0.025f);
            projectile.Center += (destination - projectile.Center).SafeNormalize(Vector2.Zero) * 3f;

            if (projectile.WithinRange(destination, 5f) || !projectile.WithinRange(destination, 2200f))
                projectile.Center = destination;
        }

        internal void AttackTarget()
        {
            // Release cinders around the target.
            Dust cinder = Dust.NewDustPerfect(Target.Center + Main.rand.NextVector2Circular(800f, 800f), DustID.Fire);
            cinder.velocity = Vector2.UnitY * -Main.rand.NextFloat(3f, 7f);
            cinder.scale = 1f + cinder.velocity.Length() * 0.17f;
            cinder.noGravity = true;

            int shootRate = (int)MathHelper.Lerp(24f, 6f, Utils.InverseLerp(0f, 300f, AttackTime, true));
            AttackTimer++;

            if (AttackTimer < shootRate || Main.myPlayer != projectile.owner)
                return;

            AttackTimer = 0f;
            projectile.netUpdate = true;

            WeightedRandom<int> rng = new WeightedRandom<int>(projectile.identity * 2167 + (int)(Main.GlobalTime * 20));
            rng.Add(ModContent.ProjectileType<LostSoulGold>(), 0.5f);
            rng.Add(ModContent.ProjectileType<LostSoulGiant>(), 0.6f);
            rng.Add(ModContent.ProjectileType<LostSoulLarge>(), 0.8f);
            rng.Add(ModContent.ProjectileType<LostSoulSmall>(), 1f);

            Vector2 spawnPosition = Target.Center + Vector2.UnitY.RotatedByRandom(0.27f) * 1150f;
            Vector2 shootVelocity = (Target.Center - spawnPosition).SafeNormalize(-Vector2.UnitY).RotatedByRandom(0.09f) * Main.rand.NextFloat(19f, 31f);
            int soul = Projectile.NewProjectile(spawnPosition, shootVelocity, rng.Get(), projectile.damage, projectile.knockBack, projectile.owner);
            if (Main.projectile.IndexInRange(soul))
                Main.projectile[soul].Calamity().forceMinion = true;
        }

        public override void PostDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            if (Target is null)
                return;

            Texture2D crossTexture = ModContent.GetTexture("CalamityMod/Projectiles/Summon/PerditionCross");

            Vector2 drawPosition = Target.Bottom - Main.screenPosition;
            drawPosition.Y -= 12f;
            Color drawColor = Color.White * DownwardCrossFade;
            spriteBatch.Draw(crossTexture, drawPosition, null, drawColor, projectile.rotation, crossTexture.Size() * 0.5f, projectile.scale * 0.85f, SpriteEffects.None, 0f);
        }

        public override bool CanDamage() => false;
    }
}
