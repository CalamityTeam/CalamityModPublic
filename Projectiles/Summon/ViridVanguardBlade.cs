using System;
using System.IO;
using CalamityMod.Buffs.Summon;
using CalamityMod.CalPlayer;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
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
            get => Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Vanguard Blade");
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 7;
        }

        public override void SetDefaults()
        {
            Projectile.width = 60;
            Projectile.height = 156;
            Projectile.netImportant = true;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.minionSlots = 1.5f;
            Projectile.timeLeft = 18000;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 14;
            Projectile.tileCollide = false;
            Projectile.timeLeft *= 5;
            Projectile.minion = true;
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
            Player player = Main.player[Projectile.owner];
            CalamityPlayer modPlayer = player.Calamity();
            Projectile.spriteDirection = AltTexture.ToDirectionInt();
            if (Projectile.localAI[0] == 0f)
            {
                Projectile.Calamity().spawnedPlayerMinionDamageValue = player.MinionDamage();
                Projectile.Calamity().spawnedPlayerMinionProjectileDamageValue = Projectile.damage;
                Projectile.localAI[0] = 1f;
            }
            if (player.MinionDamage() != Projectile.Calamity().spawnedPlayerMinionDamageValue)
            {
                int trueDamage = (int)(Projectile.Calamity().spawnedPlayerMinionProjectileDamageValue /
                    Projectile.Calamity().spawnedPlayerMinionDamageValue *
                    player.MinionDamage());
                Projectile.damage = trueDamage;
            }
            bool isProperProjectile = Projectile.type == ModContent.ProjectileType<ViridVanguardBlade>();
            player.AddBuff(ModContent.BuffType<ViridVanguardBuff>(), 3600);
            if (isProperProjectile)
            {
                if (player.dead)
                {
                    modPlayer.viridVanguard = false;
                }
                if (modPlayer.viridVanguard)
                {
                    Projectile.timeLeft = 2;
                }
            }

            if (FiringTime <= 0f)
            {
                Projectile.ai[1]++;
                float outwardPosition = 320f;
                outwardPosition += (float)Math.Sin(Angle) * 200f;
                Projectile.Center = player.Center + Angle.ToRotationVector2().RotatedBy((float)Math.Cos(Angle) * MathHelper.PiOver4 + AltTexture.ToInt() * MathHelper.Pi) * outwardPosition;
                Projectile.rotation = player.AngleTo(Projectile.Center) + MathHelper.PiOver4;
                Angle += MathHelper.ToRadians(4f);
            }
            else
            {
                Projectile.spriteDirection = 1;
                if (FiringTime >= 239f)
                {
                    RemainingSlices = Projectile.penetrate = 3;
                    IdealPosition = player.Center - Vector2.UnitY.RotatedBy(RedirectAngle) * 550f;
                }
                else if (FiringTime > 120f)
                {
                    Projectile.Center = Vector2.Lerp(Projectile.Center, IdealPosition, 1f / 40f);
                    Projectile.rotation = Projectile.rotation.AngleLerp(RedirectAngle + MathHelper.ToRadians(82f), 1f / 60f);
                }
                else if (FiringTime > 80f)
                {
                    Projectile.Center = IdealPosition;
                    Projectile.rotation = Projectile.rotation.AngleLerp(Projectile.AngleTo(Main.MouseWorld) + MathHelper.ToRadians(82f), 1f / 20f);
                }
                else if (FiringTime == 80f)
                {
                    PostSliceDeathTimer = 80;
                    Projectile.velocity = Projectile.SafeDirectionTo(Main.MouseWorld) * 30f;
                }
                else if (FiringTime == 1f)
                {
                    if (RemainingSlices <= 0)
                    {
                        Projectile.Kill();
                    }
                    else if (RemainingSlices <= 2)
                    {
                        Projectile.velocity *= -1;
                        FiringTime = 20f;
                    }
                }
                FiringTime--;
            }

            if (RemainingSlices > 0)
                PostSliceDeathTimer--;
            if (PostSliceDeathTimer == 1)
                Projectile.Kill();

            if (Projectile.localAI[1] > 0)
            {
                Projectile.alpha = (int)MathHelper.Lerp(255, 0, 1f - Projectile.localAI[1] / 80f);
                Projectile.localAI[1]--;
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
                Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, 66);
                dust.velocity = Vector2.UnitY * Main.rand.NextFloat(3f, 5.5f) * Main.rand.NextBool(2).ToDirectionInt();
                dust.noGravity = true;
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>("CalamityMod/Projectiles/Summon/ViridVanguardBlade" + (AltTexture ? "" : "Alt")).Value;

            Rectangle rectangle = new Rectangle(0, 0, texture.Width, texture.Height);

            SpriteEffects spriteEffects = SpriteEffects.None;
            if (Projectile.spriteDirection == -1)
                spriteEffects = SpriteEffects.FlipHorizontally;

            if (CalamityConfig.Instance.Afterimages && FiringTime > 0f && FiringTime < 80f)
            {
                for (int i = 0; i < Projectile.oldPos.Length; i++)
                {
                    Vector2 drawPos = Projectile.oldPos[i] + Projectile.Size / 2f - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY);
                    Color color = Color.Lerp(Color.White, Color.DarkOliveGreen, i / (float)Projectile.oldPos.Length) *
                        ((Projectile.oldPos.Length - i) / (float)Projectile.oldPos.Length);
                    float scale = MathHelper.Lerp(Projectile.scale * 1f, Projectile.scale * 0.5f, i / (float)Projectile.oldPos.Length);
                    Main.spriteBatch.Draw(texture, drawPos, new Rectangle?(rectangle), color,
                        Projectile.rotation,
                        rectangle.Size() / 2f, scale, SpriteEffects.None, 0);
                }
            }
            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Rectangle?(rectangle), Color.White,
                       Projectile.rotation,
                       rectangle.Size() / 2f, 1f, spriteEffects, 0);
            return false;
        }
    }
}
