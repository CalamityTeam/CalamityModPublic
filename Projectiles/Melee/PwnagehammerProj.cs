using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.Projectiles.Melee
{
    public class PwnagehammerProj : ModProjectile
    {
        public override string Texture => "CalamityMod/Items/Weapons/Melee/PwnagehammerMelee";

        public static readonly SoundStyle UseSound = new("CalamityMod/Sounds/Item/PwnagehammerSound") { Volume = 0.16f, PitchVariance = 0.16f };
        public static readonly SoundStyle HomingSound = new("CalamityMod/Sounds/Item/PwnagehammerHoming") { Volume = 0.3f };

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Pwnagehammer");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 10;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 40;
            Projectile.friendly = true;
            Projectile.timeLeft = 3600;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(Projectile.localAI[0]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            Projectile.localAI[0] = reader.ReadSingle();
        }

        public override void AI()
        {
            Projectile.direction = Projectile.spriteDirection = Projectile.velocity.X > 0f ? 1 : -1;
            Projectile.rotation += MathHelper.ToRadians(22.5f) * Projectile.direction;

            if (Projectile.ai[0] == 1f)
            {
                float distance = 400f;
                if (Projectile.ai[1] == -1)
                {
                    // used for finding a target
                    for (int i = 0; i < Main.maxNPCs; i++)
                    {
                        NPC npc = Main.npc[i];
                        if (npc.CanBeChasedBy(Projectile, false))
                        {
                            Vector2 vec = npc.Center - Projectile.Center;
                            float distanceTo = vec.Length();
                            distanceTo -= (float)Math.Sqrt(npc.width * npc.width + npc.height * npc.height) * 0.75f;
                            if (distanceTo < distance && Collision.CanHit(Projectile.position, Projectile.width, Projectile.height, npc.position, npc.width, npc.height))
                                Projectile.ai[1] = i;
                        }
                    }
                }
                else
                {
                    int index = (int)Projectile.ai[1];
                    float speed = 34f;
                    float inertia = 15f;

                    if (!Main.npc[index].CanBeChasedBy(Projectile, false))
                        Projectile.ai[1] = -1;

                    Vector2 direction = Main.npc[index].Center - Projectile.Center;
                    direction.Normalize();
                    direction *= speed;
                    Projectile.velocity = (Projectile.velocity * (inertia - 1) + direction) / inertia;
                }
            }
            else
            {
                Projectile.velocity.X *= 0.9711f;
                Projectile.velocity.Y += 0.426f;
            }

            if (Main.rand.NextBool(2))
            {
                Vector2 offset = new Vector2(12, 0).RotatedByRandom(MathHelper.ToRadians(360f));
                Vector2 velOffset = new Vector2(4, 0).RotatedBy(offset.ToRotation());
                Dust dust = Dust.NewDustPerfect(new Vector2(Projectile.Center.X, Projectile.Center.Y) + offset, DustID.GoldFlame, new Vector2(Projectile.velocity.X * 0.2f + velOffset.X, Projectile.velocity.Y * 0.2f + velOffset.Y), 100, new Color(255, 245, 198), 2f);
                dust.noGravity = true;
            }

            if (Main.rand.NextBool(6))
            {
                Vector2 offset = new Vector2(12, 0).RotatedByRandom(MathHelper.ToRadians(360f));
                Vector2 velOffset = new Vector2(4, 0).RotatedBy(offset.ToRotation());
                Dust dust = Dust.NewDustPerfect(new Vector2(Projectile.Center.X, Projectile.Center.Y) + offset, DustID.GoldFlame, new Vector2(Projectile.velocity.X * 0.2f + velOffset.X, Projectile.velocity.Y * 0.2f + velOffset.Y), 100, new Color(255, 245, 198), 2f);
                dust.noGravity = true;
            }
        }

        public override void Kill(int timeLeft)
        {
            Player player = Main.player[Projectile.owner];
            if (Projectile.ai[0] == 1f)
            {
                float numberOfDusts = 40f;
                float rotFactor = 360f / numberOfDusts;
                for (int i = 0; i < numberOfDusts; i++)
                {
                    float rot = MathHelper.ToRadians(i * rotFactor);
                    Vector2 offset = new Vector2(15f, 0).RotatedBy(rot);
                    Vector2 velOffset = new Vector2(10f, 0).RotatedBy(rot);
                    Dust dust = Dust.NewDustPerfect(Projectile.position + offset, 269, new Vector2(velOffset.X, velOffset.Y));
                    dust.noGravity = true;
                    dust.velocity = velOffset;
                    dust.scale = 2.5f;
                    if (i % 2 == 0)
                    {
                        dust = Dust.NewDustPerfect(Projectile.position + offset, 269, new Vector2(velOffset.X, velOffset.Y));
                        dust.noGravity = true;
                        dust.velocity = velOffset * 2f;
                        dust.scale = 2.5f;
                    }
                    if (i % 4 == 0)
                    {
                        dust = Dust.NewDustPerfect(Projectile.position + offset, 269, new Vector2(velOffset.X, velOffset.Y));
                        dust.noGravity = true;
                        dust.velocity = velOffset / 2f;
                        dust.scale = 2.5f;
                    }
                }

                float distance = 240f;

                for (int k = 0; k < Main.maxNPCs; k++)
                {
                    NPC npc = Main.npc[k];
                    Vector2 vec = npc.Center - Projectile.Center;
                    float distanceTo = (float)Math.Sqrt(vec.X * vec.X + vec.Y * vec.Y);
                    distanceTo -= (float)Math.Sqrt(npc.width * npc.width + npc.height * npc.height) * 0.75f;
                    if (distanceTo < distance && npc.CanBeChasedBy(Projectile, false) && k != Projectile.localAI[0])
                    {
                        float alldamage = Projectile.damage * 1.25f;
                        double damage = npc.StrikeNPC((int)alldamage, Projectile.knockBack, Projectile.velocity.X > 0f ? 1 : -1);
                        player.addDPS((int)damage);
                    }
                }
                SoundEngine.PlaySound(HomingSound, Projectile.Center);
                SoundEngine.PlaySound(SoundID.Item14 with { Volume = 0.22f }, Projectile.Center);
            }
            else
            {
                float numberOfDusts = 32f;
                float rotFactor = 360f / numberOfDusts;
                for (int i = 0; i < numberOfDusts; i++)
                {
                    float rot = MathHelper.ToRadians(i * rotFactor);
                    Vector2 offset = new Vector2(15f, 0).RotatedBy(rot);
                    Vector2 velOffset = new Vector2(12.5f, 0).RotatedBy(rot);
                    Dust dust = Dust.NewDustPerfect(Projectile.position + offset, 269, new Vector2(velOffset.X, velOffset.Y));
                    dust.noGravity = true;
                    dust.velocity = velOffset;
                    dust.scale = 2.5f;
                }

                float distance = 168f;

                for (int k = 0; k < Main.maxNPCs; k++)
                {
                    NPC npc = Main.npc[k];
                    Vector2 vec = npc.Center - Projectile.Center;
                    float distanceTo = (float)Math.Sqrt(vec.X * vec.X + vec.Y * vec.Y);
                    distanceTo -= (float)Math.Sqrt(npc.width * npc.width + npc.height * npc.height) * 0.75f;
                    if (distanceTo < distance && npc.CanBeChasedBy(Projectile, false) && k != Projectile.localAI[0])
                    {
                        float alldamage = Projectile.damage * 0.75f;
                        double damage = npc.StrikeNPC((int)alldamage, Projectile.knockBack, Projectile.velocity.X > 0f ? 1 : -1);
                        player.addDPS((int)damage);
                    }
                }

                SoundEngine.PlaySound(UseSound, Projectile.Center);
                SoundEngine.PlaySound(SoundID.Item14 with { Volume = 0.11f }, Projectile.Center);
            }
        }

        public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            if (Projectile.ai[0] == 1f && Main.myPlayer == Projectile.owner)
            {
                crit = true;
                int hammer = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, new Vector2(0, -15f), ModContent.ProjectileType<PwnagehammerEcho>(), Projectile.damage * 2, Projectile.knockBack, Projectile.owner, 0f, Projectile.ai[1]);
                Main.projectile[hammer].localAI[0] = Math.Sign(Projectile.velocity.X);
                Main.projectile[hammer].netUpdate = true;
            }
        }

        public override void ModifyHitPvp(Player target, ref int damage, ref bool crit)
        {
            if (Projectile.ai[0] == 1f && Main.myPlayer == Projectile.owner)
            {
                int hammer = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, new Vector2(0, -15f), ModContent.ProjectileType<PwnagehammerEcho>(), Projectile.damage * 2, Projectile.knockBack, Projectile.owner, 0f, Projectile.ai[1]);
                Main.projectile[hammer].localAI[0] = Math.Sign(Projectile.velocity.X);
                Main.projectile[hammer].netUpdate = true;
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            Projectile.localAI[0] = target.whoAmI;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], lightColor, 1);
            return false;
        }
    }
}
