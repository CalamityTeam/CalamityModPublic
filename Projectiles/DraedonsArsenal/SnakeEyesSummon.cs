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
            get => Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }

        public float EyeOutwardness = 1f;
        public float EyeRotation = 0f;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Snake Eyes");
            ProjectileID.Sets.MinionSacrificable[Projectile.type] = true;
            ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 4;
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 22;
            Projectile.netImportant = true;
            Projectile.friendly = true;
            Projectile.minionSlots = 1;
            Projectile.timeLeft = 18000;
            Projectile.penetrate = -1;
            Projectile.timeLeft *= 5;
            Projectile.minion = true;
            Projectile.tileCollide = false;
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
            Player player = Main.player[Projectile.owner];
            if (Projectile.localAI[0] == 0f)
            {
                Initialize(player);
                Projectile.localAI[0] = 1f;
            }
            AdjustDamage(player);
            GrantBuffs(player);
            NPC potentialTarget = Projectile.Center.MinionHoming(820f, player);
            if (potentialTarget == null || SufferingFromSeparationAnxiety)
            {
                Projectile.ai[1] = 0f;
                if (Projectile.localAI[1] == 1f)
                {
                    Time = 0f;
                    Projectile.localAI[1] = 0f;
                }
                PlayerMovement(player);
            }
            else
            {
                if (Projectile.localAI[1] == 0f)
                {
                    Time = 0f;
                    Projectile.localAI[1] = 1f;
                }
                NPCMovement(potentialTarget);
            }
            if (!SufferingFromSeparationAnxiety && Projectile.Distance(player.Center) > (potentialTarget is null ? 360f : 1800f))
            {
                SufferingFromSeparationAnxiety = true;
                Projectile.netUpdate = true;
            }
            else if (SufferingFromSeparationAnxiety && Projectile.Distance(player.Center) < (potentialTarget is null ? 10f : 120f))
            {
                SufferingFromSeparationAnxiety = false;
                Projectile.netUpdate = true;
            }
            if (!Main.dedServ)
            {
                GenerateAfterimageDust();
            }
            Time++;
        }

        public void Initialize(Player player)
        {
            Projectile.Calamity().spawnedPlayerMinionDamageValue = player.MinionDamage();
            Projectile.Calamity().spawnedPlayerMinionProjectileDamageValue = Projectile.damage;
            Destination = Projectile.Center - Vector2.UnitY * 180f;
            for (int i = 0; i < 45; i++)
            {
                float angle = MathHelper.TwoPi / 45f * i;
                Vector2 velocity = angle.ToRotationVector2() * 4f;
                Dust dust = Dust.NewDustPerfect(Projectile.Center + velocity * 2.75f, 39, velocity);
                dust.noGravity = true;
            }
        }

        // While this projectile cannot attack, the projectiles it shoots derive from the damage.
        public void AdjustDamage(Player player)
        {
            if (player.MinionDamage() != Projectile.Calamity().spawnedPlayerMinionDamageValue)
            {
                int trueDamage = (int)(Projectile.Calamity().spawnedPlayerMinionProjectileDamageValue /
                    Projectile.Calamity().spawnedPlayerMinionDamageValue *
                    player.MinionDamage());
                Projectile.damage = trueDamage;
            }
        }

        public void GrantBuffs(Player player)
        {
            bool isCorrectProjectile = Projectile.type == ModContent.ProjectileType<SnakeEyesSummon>();
            player.AddBuff(ModContent.BuffType<SnakeEyesBuff>(), 3600);
            if (isCorrectProjectile)
            {
                if (player.dead)
                {
                    player.Calamity().snakeEyes = false;
                }
                if (player.Calamity().snakeEyes)
                {
                    Projectile.timeLeft = 2;
                }
            }
        }

        public void PlayerMovement(Player player)
        {
            Projectile.velocity = Vector2.Zero;
            if (!SufferingFromSeparationAnxiety)
            {
                Projectile.Center = player.Center - Vector2.UnitY * (80f + (float)Math.Sin(Time / 120f * MathHelper.TwoPi) * 30f);
                EyeOutwardness = MathHelper.Lerp(EyeOutwardness, 0f, 0.15f);
                return;
            }
            if (Time % 150f == 0f)
            {
                OldCenter = Projectile.Center;
            }
            else if (Time % 150f <= 35f)
            {
                EyeRotation = Projectile.AngleTo(player.Center);
                EyeOutwardness = MathHelper.Lerp(EyeOutwardness, 1f, 0.15f);
                Projectile.Center = Vector2.SmoothStep(OldCenter, player.Center - Vector2.UnitY * 80f, Utils.GetLerpValue(0f, 35f, Time % 150f));
            }
            else
            {
                Projectile.Center = player.Center - Vector2.UnitY * (80f + (float)Math.Sin((Time % 150f - 35f) / 115f * MathHelper.TwoPi) * 30f);
            }
            EyeOutwardness = MathHelper.Lerp(EyeOutwardness, 0f, 0.15f);
        }

        public void NPCMovement(NPC npc)
        {
            Projectile.velocity = Vector2.Zero;
            Vector2 offsetMultiplier = Vector2.UnitX;
            switch ((int)Projectile.ai[1] % 4)
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
                if (Projectile.ai[1] > 0 && Main.myPlayer == Projectile.owner)
                {
                    Vector2 shootPosition = Projectile.Center + Utils.Vector2FromElipse(EyeRotation.ToRotationVector2(), Projectile.Size * 0.5f * EyeOutwardness);
                    Vector2 shootVelocity = Projectile.SafeDirectionTo(npc.Center, Main.rand.NextVector2Unit()) * 4f;
                    int laser = Projectile.NewProjectile(Projectile.GetProjectileSource_FromThis(), shootPosition, shootVelocity, ProjectileID.UFOLaser, Projectile.damage, Projectile.knockBack, Projectile.owner);
                    if (laser.WithinBounds(Main.maxProjectiles))
                    {
                        Main.projectile[laser].timeLeft *= 2;
                        Main.projectile[laser].tileCollide = false;
                        Main.projectile[laser].netUpdate = true;
                        Main.projectile[laser].Calamity().forceMinion = true;
                    }
                }
                Projectile.ai[1]++;
                OldCenter = Projectile.Center;
            }
            else
            {
                EyeOutwardness = MathHelper.Lerp(EyeOutwardness, 1f, 0.15f);
                EyeRotation = EyeRotation.AngleTowards(Projectile.AngleTo(npc.Center), MathHelper.TwoPi / 20f);
                Projectile.Center = Vector2.SmoothStep(OldCenter, npc.Center + (new Vector2(300f) + Projectile.Size * 0.5f) * offsetMultiplier, Time % 20f / 20f);
            }
        }

        public void GenerateAfterimageDust()
        {
            for (int i = 1; i < Projectile.oldPos.Length; i++)
            {
                if (i == 1)
                {
                    for (int j = 0; j < 14; j++)
                    {
                        Dust dust = Dust.NewDustPerfect(Projectile.Center, 261);
                        dust.position += (j / 14f * MathHelper.TwoPi).ToRotationVector2() * Projectile.Size * 0.5f * 1.3f;
                        dust.velocity = Vector2.Zero;
                        dust.scale = 0.6f;
                        dust.noGravity = true;
                    }
                }
                if (Vector2.Distance(Projectile.oldPos[i - 1], Projectile.oldPos[i]) > 3f)
                {
                    for (int j = 0; j < 7; j++)
                    {
                        Dust dust = Dust.NewDustPerfect(Projectile.oldPos[i] + Projectile.Size * 0.5f, 261);
                        dust.position += (j / 7f * MathHelper.TwoPi).ToRotationVector2() * ((Projectile.oldPos.Length - i) * 1.2f + 3f);
                        dust.velocity = Vector2.Zero;
                        dust.scale = 0.6f;
                        dust.noGravity = true;
                    }
                }
            }
        }

        public override bool? CanDamage() => false;

        public override bool PreDraw(ref Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(Projectile, 0, Color.White, ProjectileID.Sets.TrailCacheLength[Projectile.type]);
            Texture2D eyeTexture = ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/SnakeEye").Value;
            Vector2 offsetVector = Utils.Vector2FromElipse(EyeRotation.ToRotationVector2(), Projectile.Size * 0.5f * EyeOutwardness);
            Main.EntitySpriteDraw(eyeTexture,
                             Projectile.Center + offsetVector - Main.screenPosition,
                             null,
                             Color.White,
                             0f,
                             eyeTexture.Size() * 0.5f,
                             1f,
                             SpriteEffects.None,
                             0);
            return false;
        }
    }
}
