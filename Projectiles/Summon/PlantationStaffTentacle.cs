using System.IO;
using CalamityMod.CalPlayer;
using CalamityMod.Items.Weapons.Summon;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Summon
{
    public class PlantationStaffTentacle : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Summon";

        public Player Owner => Main.player[Projectile.owner];
        public CalamityPlayer ModdedOwner => Owner.Calamity();
        public NPC Target => Projectile.Center.MinionHoming(PlantationStaff.EnemyDistanceDetection, Owner);
        public Projectile MainMinion => Main.projectile[(int)MainMinionIndex];

        public ref float TentacleIndex => ref Projectile.ai[0];
        public ref float MainMinionIndex => ref Projectile.ai[1];
        public ref float AITimer => ref Projectile.localAI[0];
        public enum AIState
        {
            Attached,
            Seeking
        }
        public AIState State
        {
            get => (AIState)Projectile.ai[2];
            set => Projectile.ai[2] = (int)value;
        }

        public Vector2 DesiredLocation;

        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 4;
            ProjectileID.Sets.MinionShot[Type] = true;
            ProjectileID.Sets.TrailingMode[Type] = 2;
            ProjectileID.Sets.TrailCacheLength[Type] = 4;
        }

        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Summon;
            Projectile.localNPCHitCooldown = -1;
            Projectile.width = Projectile.height = 22;
            Projectile.penetrate = -1;

            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.netImportant = true;
        }

        #region Variable Syncing

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(AITimer);
            writer.WritePackedVector2(DesiredLocation);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            AITimer = reader.ReadSingle();
            DesiredLocation = reader.ReadPackedVector2();
        }

        #endregion

        public override void AI()
        {
            CheckMinionExistence();
            DoAnimation();

            switch (State)
            {
                case AIState.Attached:
                    AttachedState();
                    break;
                case AIState.Seeking:
                    SeekingState();
                    break;
            }
        }

        #region AI Methods

        private void AttachedState()
        {
            AITimer++;
            float interpolant = Utils.Remap(AITimer, 0f, PlantationStaff.TimeBeforeRamming, 0f, .4f);

            Projectile.Center = Vector2.Lerp(Projectile.Center, MainMinion.Center + DesiredLocation, interpolant);
            Projectile.rotation = (Projectile.Center - MainMinion.Center).ToRotation();

            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                Projectile proj = Main.projectile[i];

                if (proj is null || !proj.active || proj.owner != Owner.whoAmI || proj.type != ModContent.ProjectileType<PlantationStaffSummon>() || proj.ModProjectile<PlantationStaffSummon>().State == PlantationStaffSummon.AIState.Ramming)
                    continue;

                State = AIState.Seeking;
                AITimer = 0f;
                Projectile.velocity = Vector2.Zero;
                Projectile.penetrate = 1;

                for (int dustIndex = 0; dustIndex < 20; dustIndex++)
                    Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, 40);

                SoundEngine.PlaySound(SoundID.NPCDeath1, Projectile.Center);

                Projectile.netUpdate = true;
            }
        }

        private void SeekingState()
        {
            if (Target is not null)
            {
                AITimer++;

                if (AITimer <= 30f)
                {
                    Projectile.velocity = Projectile.rotation.ToRotationVector2() * 10f;
                }
                else
                {
                    Projectile.velocity = (Projectile.velocity * 35f + Projectile.SafeDirectionTo(Target.Center) * PlantationStaff.TentacleSpeed) / 36f;
                    Projectile.rotation = Projectile.velocity.ToRotation();
                }
            }
            else
                Projectile.Kill();
        }

        private void CheckMinionExistence()
        {
            if (Projectile.ai[1] < 0 || Projectile.ai[1] >= Main.maxProjectiles)
            {
                Projectile.Kill();
                return;
            }

            // If something has gone wrong with either the tentacle or the host plant, destroy the projectile.
            if (Projectile.type != ModContent.ProjectileType<PlantationStaffTentacle>() || !MainMinion.active || MainMinion.type != ModContent.ProjectileType<PlantationStaffSummon>())
            {
                Projectile.Kill();
                return;
            }

            if (ModdedOwner.PlantationSummon)
                Projectile.timeLeft = 2;
        }

        private void DoAnimation()
        {
            Projectile.frameCounter++;
            if (Projectile.frameCounter >= 3)
            {
                Projectile.frameCounter = 0;
                Projectile.frame = (Projectile.frame + 1) % Main.projFrames[Type];
            }
        }

        #endregion

        public override void OnSpawn(IEntitySource source)
        {
            DesiredLocation = (MathHelper.TwoPi / 6f * TentacleIndex).ToRotationVector2().RotatedByRandom(MathHelper.PiOver4 / 1.5f) * 100f;
            Projectile.netUpdate = true;
        }

        public override bool? CanDamage() => (State == AIState.Seeking) ? null : false;

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 10; i++)
                Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, 40);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (MainMinionIndex < 0 || MainMinionIndex >= Main.maxProjectiles)
                return false;

            // If something has gone wrong with either the tentacle or the host plant, return.
            if (Type != ModContent.ProjectileType<PlantationStaffTentacle>() || !MainMinion.active || MainMinion.type != ModContent.ProjectileType<PlantationStaffSummon>())
                return false;

            if (State == AIState.Attached)
            {
                Vector2 source = MainMinion.Center;
                Texture2D chain = ModContent.Request<Texture2D>("CalamityMod/Projectiles/Summon/PlantationStaffTentacleChain").Value;
                Vector2 goal = Projectile.Center;
                Rectangle? sourceRectangle = null;
                float textureHeight = chain.Height;
                Vector2 drawVector = source - goal;
                float rotation = drawVector.ToRotation() - MathHelper.PiOver2;
                bool shouldDraw = true;
                if (float.IsNaN(goal.X) && float.IsNaN(goal.Y))
                {
                    shouldDraw = false;
                }
                if (float.IsNaN(drawVector.X) && float.IsNaN(drawVector.Y))
                {
                    shouldDraw = false;
                }
                while (shouldDraw)
                {
                    if (drawVector.Length() < textureHeight + 1f)
                    {
                        shouldDraw = false;
                    }
                    else
                    {
                        Vector2 value2 = drawVector;
                        value2.Normalize();
                        goal += value2 * textureHeight;
                        drawVector = source - goal;
                        Color color = Lighting.GetColor((int)goal.X / 16, (int)(goal.Y / 16f));
                        Main.EntitySpriteDraw(chain, goal - Main.screenPosition, sourceRectangle, color, rotation, chain.Size() / 2f, 1f, SpriteEffects.None, 0);
                    }
                }
            }
            else if (CalamityConfig.Instance.Afterimages)
                CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Type], lightColor);

            return true;
        }

        // It draws the host plant in here in order to have it draw over the tentacles.
        public override void PostDraw(Color lightColor)
        {
            // Only 1 tentacle needs to draw this, the last one spawned because it's latest in the projectile array.
            if (TentacleIndex < 5)
                return;

            if (MainMinionIndex < 0 || MainMinionIndex >= Main.maxProjectiles)
                return;

            // If something has gone wrong with either the tentacle or the host plant, return.
            if (Projectile.type != ModContent.ProjectileType<PlantationStaffTentacle>() || !MainMinion.active || MainMinion.type != ModContent.ProjectileType<PlantationStaffSummon>())
                return;

            Texture2D texture = TextureAssets.Projectile[MainMinion.type].Value;
            int height = texture.Height / Main.projFrames[MainMinion.type];
            int frameHeight = height * MainMinion.frame;
            SpriteEffects spriteEffects = SpriteEffects.None;
            if (MainMinion.spriteDirection == -1)
                spriteEffects = SpriteEffects.FlipHorizontally;
            Color color = Lighting.GetColor((int)MainMinion.Center.X / 16, (int)(MainMinion.Center.Y / 16f));

            Main.EntitySpriteDraw(texture, MainMinion.Center - Main.screenPosition + new Vector2(0f, MainMinion.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(new Rectangle(0, frameHeight, texture.Width, height)), color, MainMinion.rotation, new Vector2((float)texture.Width / 2f, (float)height / 2f), MainMinion.scale, spriteEffects, 0);
        }
    }
}
