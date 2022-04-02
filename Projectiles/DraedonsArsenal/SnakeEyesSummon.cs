using CalamityMod.Buffs.Summon;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.DraedonsArsenal
{
    public class SnakeEyesSummon : ModProjectile
    {
        public bool SufferingFromSeparationAnxiety = false;
        public Vector2 OldCenter;
        public Vector2 Destination;

        public float Time
        {
            get => projectile.ai[0];
            set => projectile.ai[0] = value;
        }

        public float EyeOutwardness = 1f;
        public float EyeRotation = 0f;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Snake Eyes");
            ProjectileID.Sets.MinionSacrificable[projectile.type] = true;
            ProjectileID.Sets.MinionTargettingFeature[projectile.type] = true;
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 4;
        }

        public override void SetDefaults()
        {
            projectile.width = projectile.height = 22;
            projectile.netImportant = true;
            projectile.friendly = true;
            projectile.minionSlots = 1;
            projectile.timeLeft = 18000;
            projectile.penetrate = -1;
            projectile.timeLeft *= 5;
            projectile.minion = true;
            projectile.tileCollide = false;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(SufferingFromSeparationAnxiety);
            writer.Write(EyeOutwardness);
            writer.Write(EyeRotation);
            writer.WriteVector2(OldCenter);
            writer.WriteVector2(Destination);
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            SufferingFromSeparationAnxiety = reader.ReadBoolean();
            EyeOutwardness = reader.ReadSingle();
            EyeRotation = reader.ReadSingle();
            OldCenter = reader.ReadVector2();
            Destination = reader.ReadVector2();
        }

        public override void AI()
        {
            Player player = Main.player[projectile.owner];
            if (projectile.localAI[0] == 0f)
            {
                Initialize(player);
                projectile.localAI[0] = 1f;
            }
            AdjustDamage(player);
            GrantBuffs(player);
            NPC potentialTarget = projectile.Center.MinionHoming(820f, player);
            if (potentialTarget == null || SufferingFromSeparationAnxiety)
            {
                projectile.ai[1] = 0f;
                if (projectile.localAI[1] == 1f)
                {
                    Time = 0f;
                    projectile.localAI[1] = 0f;
                }
                PlayerMovement(player);
            }
            else
            {
                if (projectile.localAI[1] == 0f)
                {
                    Time = 0f;
                    projectile.localAI[1] = 1f;
                }
                NPCMovement(potentialTarget);
            }
            if (!SufferingFromSeparationAnxiety && projectile.Distance(player.Center) > (potentialTarget is null ? 360f : 1800f))
            {
                SufferingFromSeparationAnxiety = true;
                projectile.netUpdate = true;
            }
            else if (SufferingFromSeparationAnxiety && projectile.Distance(player.Center) < (potentialTarget is null ? 10f : 120f))
            {
                SufferingFromSeparationAnxiety = false;
                projectile.netUpdate = true;
            }
            if (!Main.dedServ)
            {
                GenerateAfterimageDust();
            }
            Time++;
        }

        public void Initialize(Player player)
        {
            projectile.Calamity().spawnedPlayerMinionDamageValue = player.MinionDamage();
            projectile.Calamity().spawnedPlayerMinionProjectileDamageValue = projectile.damage;
            Destination = projectile.Center - Vector2.UnitY * 180f;
            for (int i = 0; i < 45; i++)
            {
                float angle = MathHelper.TwoPi / 45f * i;
                Vector2 velocity = angle.ToRotationVector2() * 4f;
                Dust dust = Dust.NewDustPerfect(projectile.Center + velocity * 2.75f, 39, velocity);
                dust.noGravity = true;
            }
        }

        // While this projectile cannot attack, the projectiles it shoots derive from the damage.
        public void AdjustDamage(Player player)
        {
            if (player.MinionDamage() != projectile.Calamity().spawnedPlayerMinionDamageValue)
            {
                int trueDamage = (int)(projectile.Calamity().spawnedPlayerMinionProjectileDamageValue /
                    projectile.Calamity().spawnedPlayerMinionDamageValue *
                    player.MinionDamage());
                projectile.damage = trueDamage;
            }
        }

        public void GrantBuffs(Player player)
        {
            bool isCorrectProjectile = projectile.type == ModContent.ProjectileType<SnakeEyesSummon>();
            player.AddBuff(ModContent.BuffType<SnakeEyesBuff>(), 3600);
            if (isCorrectProjectile)
            {
                if (player.dead)
                {
                    player.Calamity().snakeEyes = false;
                }
                if (player.Calamity().snakeEyes)
                {
                    projectile.timeLeft = 2;
                }
            }
        }

        public void PlayerMovement(Player player)
        {
            projectile.velocity = Vector2.Zero;
            if (!SufferingFromSeparationAnxiety)
            {
                projectile.Center = player.Center - Vector2.UnitY * (80f + (float)Math.Sin(Time / 120f * MathHelper.TwoPi) * 30f);
                EyeOutwardness = MathHelper.Lerp(EyeOutwardness, 0f, 0.15f);
                return;
            }
            if (Time % 150f == 0f)
            {
                OldCenter = projectile.Center;
            }
            else if (Time % 150f <= 35f)
            {
                EyeRotation = projectile.AngleTo(player.Center);
                EyeOutwardness = MathHelper.Lerp(EyeOutwardness, 1f, 0.15f);
                projectile.Center = Vector2.SmoothStep(OldCenter, player.Center - Vector2.UnitY * 80f, Utils.InverseLerp(0f, 35f, Time % 150f));
            }
            else
            {
                projectile.Center = player.Center - Vector2.UnitY * (80f + (float)Math.Sin((Time % 150f - 35f) / 115f * MathHelper.TwoPi) * 30f);
            }
            EyeOutwardness = MathHelper.Lerp(EyeOutwardness, 0f, 0.15f);
        }

        public void NPCMovement(NPC npc)
        {
            projectile.velocity = Vector2.Zero;
            Vector2 offsetMultiplier = Vector2.UnitX;
            switch ((int)projectile.ai[1] % 4)
            {
                case 0:
                    offsetMultiplier = new Vector2(-1f, -1f);
                    break;
                case 1:
                    offsetMultiplier = new Vector2(-1f, 1f);
                    break;
                case 2:
                    offsetMultiplier = new Vector2(1f, 1f);
                    break;
                case 3:
                    offsetMultiplier = new Vector2(1f, -1f);
                    break;
            }
            if (Time % 20f == 0f)
            {
                if (projectile.ai[1] > 0 && Main.myPlayer == projectile.owner)
                {
                    Vector2 shootPosition = projectile.Center + Utils.Vector2FromElipse(EyeRotation.ToRotationVector2(), projectile.Size * 0.5f * EyeOutwardness);
                    Vector2 shootVelocity = projectile.SafeDirectionTo(npc.Center, Main.rand.NextVector2Unit()) * 4f;
                    int laser = Projectile.NewProjectile(shootPosition, shootVelocity, ProjectileID.UFOLaser, projectile.damage, projectile.knockBack, projectile.owner);
                    if (laser.WithinBounds(Main.maxProjectiles))
                    {
                        Main.projectile[laser].timeLeft *= 2;
                        Main.projectile[laser].tileCollide = false;
                        Main.projectile[laser].netUpdate = true;
                        Main.projectile[laser].Calamity().forceMinion = true;
                    }
                }
                projectile.ai[1]++;
                OldCenter = projectile.Center;
            }
            else
            {
                EyeOutwardness = MathHelper.Lerp(EyeOutwardness, 1f, 0.15f);
                EyeRotation = EyeRotation.AngleTowards(projectile.AngleTo(npc.Center), MathHelper.TwoPi / 20f);
                projectile.Center = Vector2.SmoothStep(OldCenter, npc.Center + (new Vector2(300f) + projectile.Size * 0.5f) * offsetMultiplier, Time % 20f / 20f);
            }
        }

        public void GenerateAfterimageDust()
        {
            for (int i = 1; i < projectile.oldPos.Length; i++)
            {
                if (i == 1)
                {
                    for (int j = 0; j < 14; j++)
                    {
                        Dust dust = Dust.NewDustPerfect(projectile.Center, 261);
                        dust.position += (j / 14f * MathHelper.TwoPi).ToRotationVector2() * projectile.Size * 0.5f * 1.3f;
                        dust.velocity = Vector2.Zero;
                        dust.scale = 0.6f;
                        dust.noGravity = true;
                    }
                }
                if (Vector2.Distance(projectile.oldPos[i - 1], projectile.oldPos[i]) > 3f)
                {
                    for (int j = 0; j < 7; j++)
                    {
                        Dust dust = Dust.NewDustPerfect(projectile.oldPos[i] + projectile.Size * 0.5f, 261);
                        dust.position += (j / 7f * MathHelper.TwoPi).ToRotationVector2() * ((projectile.oldPos.Length - i) * 1.2f + 3f);
                        dust.velocity = Vector2.Zero;
                        dust.scale = 0.6f;
                        dust.noGravity = true;
                    }
                }
            }
        }

        public override bool CanDamage() => false;

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(projectile, 0, Color.White, ProjectileID.Sets.TrailCacheLength[projectile.type]);
            Texture2D eyeTexture = ModContent.GetTexture("CalamityMod/ExtraTextures/SnakeEye");
            Vector2 offsetVector = Utils.Vector2FromElipse(EyeRotation.ToRotationVector2(), projectile.Size * 0.5f * EyeOutwardness);
            spriteBatch.Draw(eyeTexture,
                             projectile.Center + offsetVector - Main.screenPosition,
                             null,
                             Color.White,
                             0f,
                             eyeTexture.Size() * 0.5f,
                             1f,
                             SpriteEffects.None,
                             0f);
            return false;
        }
    }
}
