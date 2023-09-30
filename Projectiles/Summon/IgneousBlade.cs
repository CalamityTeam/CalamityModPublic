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
    public class IgneousBlade : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Summon";
        public bool Firing = false;
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 7;
        }

        public override void SetDefaults()
        {
            Projectile.width = 52;
            Projectile.height = 86;
            Projectile.netImportant = true;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.minionSlots = 1f;
            Projectile.timeLeft = 18000;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 7;
            Projectile.tileCollide = false;
            Projectile.timeLeft *= 5;
            Projectile.minion = true;
            Projectile.DamageType = DamageClass.Summon;
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
            Player player = Main.player[Projectile.owner];
            CalamityPlayer modPlayer = player.Calamity();
            bool isProperProjectile = Projectile.type == ModContent.ProjectileType<IgneousBlade>();
            player.AddBuff(ModContent.BuffType<IgneousExaltationBuff>(), 3600);
            if (isProperProjectile)
            {
                if (player.dead)
                {
                    modPlayer.igneousExaltation = false;
                }
                if (modPlayer.igneousExaltation)
                {
                    Projectile.timeLeft = 2;
                }
            }

            // Orbiting. 1 = Shooting
            if (!Firing)
            {
                const float outwardPosition = 180f;
                Projectile.Center = player.Center + Projectile.ai[0].ToRotationVector2() * outwardPosition;
                Projectile.rotation = Projectile.ai[0] + MathHelper.PiOver2 + MathHelper.PiOver4;
                Projectile.ai[0] -= MathHelper.ToRadians(4f);
            }
            else
            {
                if (Projectile.penetrate == -1) //limit penetration for worm memes
                    Projectile.penetrate = 3;

                Projectile.ai[0]--;
                if (Projectile.ai[0] == 1)
                    Projectile.Kill();

                if (Projectile.ai[0] % 10f == 9f)
                {
                    for (int i = 0; i < 20; i++)
                    {
                        float angle = MathHelper.TwoPi / 20f * i;
                        Dust dust = Dust.NewDustPerfect(Projectile.position + angle.ToRotationVector2().RotatedBy(Projectile.rotation) * new Vector2(14f, 21f), 6);
                        dust.velocity = angle.ToRotationVector2().RotatedBy(Projectile.rotation) * 2f;
                        dust.noGravity = true;
                    }
                }
            }
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Firing)
            {
                if (Main.myPlayer == Projectile.owner)
                {
                    Projectile.ai[0] = 50;
                    for (int i = 0; i < 3; i++)
                    {
                        Vector2 spawnPosition = target.Center - new Vector2(0f, 550f).RotatedByRandom(MathHelper.ToRadians(8f));
                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), spawnPosition, Vector2.Normalize(target.Center - spawnPosition) * 24f, ModContent.ProjectileType<IgneousBladeStrike>(),
                            Projectile.damage, Projectile.knockBack, Projectile.owner);
                    }
                    for (int i = 0; i < Main.rand.Next(28, 41); i++)
                    {
                        Dust.NewDustPerfect(
                            Projectile.Center + Utils.NextVector2Unit(Main.rand) * Main.rand.NextFloat(10f),
                            6,
                            Utils.NextVector2Unit(Main.rand) * Main.rand.NextFloat(1f, 4f));
                    }
                    Projectile.netUpdate = true;
                }
            }
        }
        public override void OnKill(int timeLeft)
        {
            for (int j = 0; j < 40; j++)
            {
                Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, 6);
                dust.velocity = Vector2.UnitY * Main.rand.NextFloat(3f, 5.5f) * Main.rand.NextBool().ToDirectionInt();
                dust.noGravity = true;
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            if (Firing)
            {
                Texture2D texture = ModContent.Request<Texture2D>("CalamityMod/Projectiles/Summon/IgneousBlade").Value;

                Rectangle rectangle = new Rectangle(0, 0, texture.Width, texture.Height);

                if (Lighting.NotRetro)
                {
                    for (int i = 0; i < Projectile.oldPos.Length; i++)
                    {
                        Vector2 drawPos = Projectile.oldPos[i] + Projectile.Size / 2f - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY);
                        Color color = Color.Lerp(Color.White, Color.Red, i / (float)Projectile.oldPos.Length) *
                            ((Projectile.oldPos.Length - i) / (float)Projectile.oldPos.Length);
                        float scale = MathHelper.Lerp(Projectile.scale * 1.35f, Projectile.scale * 0.6f, i / (float)Projectile.oldPos.Length);
                        Main.EntitySpriteDraw(texture, drawPos, new Rectangle?(rectangle), color,
                            Projectile.rotation,
                            rectangle.Size() / 2f, scale, SpriteEffects.None, 0);
                    }
                }
                Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Rectangle?(rectangle), Color.White,
                           Projectile.rotation,
                           rectangle.Size() / 2f, 1.35f, SpriteEffects.None, 0);
                return false;
            }
            return true;
        }
    }
}
