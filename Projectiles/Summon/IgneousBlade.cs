using CalamityMod.Buffs.Summon;
using CalamityMod.CalPlayer;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Summon
{
    public class IgneousBlade : ModProjectile
    {
        public bool Firing = false;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Igneous Blade");
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 7;
        }

        public override void SetDefaults()
        {
            projectile.width = 52;
            projectile.height = 86;
            projectile.netImportant = true;
            projectile.friendly = true;
            projectile.ignoreWater = true;
            projectile.minionSlots = 1f;
            projectile.timeLeft = 18000;
            projectile.penetrate = -1;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 7;
            projectile.tileCollide = false;
            projectile.timeLeft *= 5;
            projectile.minion = true;
        }
        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(Firing);
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            Firing = reader.ReadBoolean();
        }
        public override void AI()
        {
            Player player = Main.player[projectile.owner];
            CalamityPlayer modPlayer = player.Calamity();
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
            bool isProperProjectile = projectile.type == ModContent.ProjectileType<IgneousBlade>();
            player.AddBuff(ModContent.BuffType<IgneousExaltationBuff>(), 3600);
            if (isProperProjectile)
            {
                if (player.dead)
                {
                    modPlayer.igneousExaltation = false;
                }
                if (modPlayer.igneousExaltation)
                {
                    projectile.timeLeft = 2;
                }
            }

            // Orbiting. 1 = Shooting
            if (!Firing)
            {
                const float outwardPosition = 180f;
                projectile.Center = player.Center + projectile.ai[0].ToRotationVector2() * outwardPosition;
                projectile.rotation = projectile.ai[0] + MathHelper.PiOver2 + MathHelper.PiOver4;
                projectile.ai[0] -= MathHelper.ToRadians(4f);
            }
            else
            {
                if (projectile.penetrate == -1) //limit penetration for worm memes
                    projectile.penetrate = 3;

                projectile.ai[0]--;
                if (projectile.ai[0] == 1)
                    projectile.Kill();

                if (projectile.ai[0] % 10f == 9f)
                {
                    for (int i = 0; i < 20; i++)
                    {
                        float angle = MathHelper.TwoPi / 20f * i;
                        Dust dust = Dust.NewDustPerfect(projectile.position + angle.ToRotationVector2().RotatedBy(projectile.rotation) * new Vector2(14f, 21f), DustID.Fire);
                        dust.velocity = angle.ToRotationVector2().RotatedBy(projectile.rotation) * 2f;
                        dust.noGravity = true;
                    }
                }
            }
        }
        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            if (Firing)
            {
                if (Main.myPlayer == projectile.owner)
                {
                    projectile.ai[0] = 50;
                    for (int i = 0; i < 3; i++)
                    {
                        Vector2 spawnPosition = target.Center - new Vector2(0f, 550f).RotatedByRandom(MathHelper.ToRadians(8f));
                        Projectile.NewProjectile(spawnPosition, Vector2.Normalize(target.Center - spawnPosition) * 24f, ModContent.ProjectileType<IgneousBladeStrike>(),
                            (int)(projectile.damage * 0.666), projectile.knockBack, projectile.owner);
                    }
                    for (int i = 0; i < Main.rand.Next(28, 41); i++)
                    {
                        Dust.NewDustPerfect(
                            projectile.Center + Utils.NextVector2Unit(Main.rand) * Main.rand.NextFloat(10f),
                            6,
                            Utils.NextVector2Unit(Main.rand) * Main.rand.NextFloat(1f, 4f));
                    }
                    projectile.netUpdate = true;
                }
            }
        }
        public override void Kill(int timeLeft)
        {
            for (int j = 0; j < 40; j++)
            {
                Dust dust = Dust.NewDustDirect(projectile.position, projectile.width, projectile.height, 6);
                dust.velocity = Vector2.UnitY * Main.rand.NextFloat(3f, 5.5f) * Main.rand.NextBool(2).ToDirectionInt();
                dust.noGravity = true;
            }
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            if (Firing)
            {
                Texture2D texture = ModContent.GetTexture("CalamityMod/Projectiles/Summon/IgneousBlade");

                Rectangle rectangle = new Rectangle(0, 0, texture.Width, texture.Height);

                SpriteEffects spriteEffects = SpriteEffects.None;
                if (projectile.spriteDirection == -1)
                    spriteEffects = SpriteEffects.FlipHorizontally;

                if (Lighting.NotRetro)
                {
                    for (int i = 0; i < projectile.oldPos.Length; i++)
                    {
                        Vector2 drawPos = projectile.oldPos[i] + projectile.Size / 2f - Main.screenPosition + new Vector2(0f, projectile.gfxOffY);
                        Color color = Color.Lerp(Color.White, Color.Red, i / (float)projectile.oldPos.Length) *
                            ((projectile.oldPos.Length - i) / (float)projectile.oldPos.Length);
                        float scale = MathHelper.Lerp(projectile.scale * 1.35f, projectile.scale * 0.6f, i / (float)projectile.oldPos.Length);
                        Main.spriteBatch.Draw(texture, drawPos, new Rectangle?(rectangle), color,
                            projectile.rotation,
                            rectangle.Size() / 2f, scale, spriteEffects, 0f);
                    }
                }
                Main.spriteBatch.Draw(texture, projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), new Rectangle?(rectangle), Color.White,
                           projectile.rotation,
                           rectangle.Size() / 2f, 1.35f, spriteEffects, 0f);
                return false;
            }
            return true;
        }
    }
}
