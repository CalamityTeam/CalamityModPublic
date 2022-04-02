using CalamityMod.Buffs.Summon;
using CalamityMod.CalPlayer;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Summon
{
    public class ViridVanguardBlade : ModProjectile
    {
        public bool AltTexture = false;

        public int RemainingSlices = 0;
        public int PostSliceDeathTimer;
        public float RedirectAngle;
        public float FiringTime = 0f;
        public Vector2 IdealPosition;
        public float Angle 
        { 
            get => projectile.ai[0]; 
            set => projectile.ai[0] = value; 
        }
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Vanguard Blade");
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 7;
        }

        public override void SetDefaults()
        {
            projectile.width = 60;
            projectile.height = 156;
            projectile.netImportant = true;
            projectile.friendly = true;
            projectile.ignoreWater = true;
            projectile.minionSlots = 1.5f;
            projectile.timeLeft = 18000;
            projectile.penetrate = -1;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 14;
            projectile.tileCollide = false;
            projectile.timeLeft *= 5;
            projectile.minion = true;
        }
        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(RedirectAngle);
            writer.Write(FiringTime);
            writer.Write(AltTexture);
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            RedirectAngle = reader.ReadSingle();
            FiringTime = reader.ReadSingle();
            AltTexture = reader.ReadBoolean();
        }
        public override void AI()
        {
            Player player = Main.player[projectile.owner];
            CalamityPlayer modPlayer = player.Calamity();
            projectile.spriteDirection = AltTexture.ToDirectionInt();
            if (projectile.localAI[0] == 0f)
            {
                projectile.Calamity().spawnedPlayerMinionDamageValue = player.MinionDamage();
                projectile.Calamity().spawnedPlayerMinionProjectileDamageValue = projectile.damage;
                projectile.localAI[0] = 1f;
            }
            if (player.MinionDamage() != projectile.Calamity().spawnedPlayerMinionDamageValue)
            {
                int trueDamage = (int)(projectile.Calamity().spawnedPlayerMinionProjectileDamageValue /
                    projectile.Calamity().spawnedPlayerMinionDamageValue *
                    player.MinionDamage());
                projectile.damage = trueDamage;
            }
            bool isProperProjectile = projectile.type == ModContent.ProjectileType<ViridVanguardBlade>();
            player.AddBuff(ModContent.BuffType<ViridVanguardBuff>(), 3600);
            if (isProperProjectile)
            {
                if (player.dead)
                {
                    modPlayer.viridVanguard = false;
                }
                if (modPlayer.viridVanguard)
                {
                    projectile.timeLeft = 2;
                }
            }

            if (FiringTime <= 0f)
            {
                projectile.ai[1]++;
                float outwardPosition = 320f;
                outwardPosition += (float)Math.Sin(Angle) * 200f;
                projectile.Center = player.Center + Angle.ToRotationVector2().RotatedBy((float)Math.Cos(Angle) * MathHelper.PiOver4 + AltTexture.ToInt() * MathHelper.Pi) * outwardPosition;
                projectile.rotation = player.AngleTo(projectile.Center) + MathHelper.PiOver4;
                Angle += MathHelper.ToRadians(4f);
            }
            else
            {
                projectile.spriteDirection = 1;
                if (FiringTime >= 239f)
                {
                    RemainingSlices = projectile.penetrate = 3;
                    IdealPosition = player.Center - Vector2.UnitY.RotatedBy(RedirectAngle) * 550f;
                }
                else if (FiringTime > 120f)
                {
                    projectile.Center = Vector2.Lerp(projectile.Center, IdealPosition, 1f / 40f);
                    projectile.rotation = projectile.rotation.AngleLerp(RedirectAngle + MathHelper.ToRadians(82f), 1f / 60f);
                }
                else if (FiringTime > 80f)
                {
                    projectile.Center = IdealPosition;
                    projectile.rotation = projectile.rotation.AngleLerp(projectile.AngleTo(Main.MouseWorld) + MathHelper.ToRadians(82f), 1f / 20f);
                }
                else if (FiringTime == 80f)
                {
                    PostSliceDeathTimer = 80;
                    projectile.velocity = projectile.SafeDirectionTo(Main.MouseWorld) * 30f;
                }
                else if (FiringTime == 1f)
                {
                    if (RemainingSlices <= 0)
                    {
                        projectile.Kill();
                    }
                    else if (RemainingSlices <= 2)
                    {
                        projectile.velocity *= -1;
                        FiringTime = 20f;
                    }
                }
                FiringTime--;
            }

            if (RemainingSlices > 0)
                PostSliceDeathTimer--;
            if (PostSliceDeathTimer == 1)
                projectile.Kill();

            if (projectile.localAI[1] > 0)
            {
                projectile.alpha = (int)MathHelper.Lerp(255, 0, 1f - projectile.localAI[1] / 80f);
                projectile.localAI[1]--;
            }
        }
        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            if (RemainingSlices > 0 && FiringTime <= 80f)
            {
                FiringTime = 10f;
                RemainingSlices--;
            }
        }
        public override void Kill(int timeLeft)
        {
            for (int j = 0; j < 40; j++)
            {
                Dust dust = Dust.NewDustDirect(projectile.position, projectile.width, projectile.height, 66);
                dust.velocity = Vector2.UnitY * Main.rand.NextFloat(3f, 5.5f) * Main.rand.NextBool(2).ToDirectionInt();
                dust.noGravity = true;
            }
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D texture = ModContent.GetTexture("CalamityMod/Projectiles/Summon/ViridVanguardBlade" + (AltTexture ? "" : "Alt"));

            Rectangle rectangle = new Rectangle(0, 0, texture.Width, texture.Height);

            SpriteEffects spriteEffects = SpriteEffects.None;
            if (projectile.spriteDirection == -1)
                spriteEffects = SpriteEffects.FlipHorizontally;

            if (CalamityConfig.Instance.Afterimages && FiringTime > 0f && FiringTime < 80f)
            {
                for (int i = 0; i < projectile.oldPos.Length; i++)
                {
                    Vector2 drawPos = projectile.oldPos[i] + projectile.Size / 2f - Main.screenPosition + new Vector2(0f, projectile.gfxOffY);
                    Color color = Color.Lerp(Color.White, Color.DarkOliveGreen, i / (float)projectile.oldPos.Length) *
                        ((projectile.oldPos.Length - i) / (float)projectile.oldPos.Length);
                    float scale = MathHelper.Lerp(projectile.scale * 1f, projectile.scale * 0.5f, i / (float)projectile.oldPos.Length);
                    Main.spriteBatch.Draw(texture, drawPos, new Rectangle?(rectangle), color,
                        projectile.rotation,
                        rectangle.Size() / 2f, scale, spriteEffects, 0f);
                }
            }
            Main.spriteBatch.Draw(texture, projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), new Rectangle?(rectangle), Color.White,
                       projectile.rotation,
                       rectangle.Size() / 2f, 1f, spriteEffects, 0f);
            return false;
        }
    }
}
