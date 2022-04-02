using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Melee
{
    public class PwnagehammerProj : ModProjectile
    {
        public override string Texture => "CalamityMod/Items/Weapons/Melee/PwnagehammerMelee";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Pwnagehammer");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 10;
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            projectile.width = projectile.height = 40;
            projectile.friendly = true;
            projectile.timeLeft = 3600;
            projectile.melee = true;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 10;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(projectile.localAI[0]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            projectile.localAI[0] = reader.ReadSingle();
        }

        public override void AI()
        {
            projectile.direction = projectile.spriteDirection = projectile.velocity.X > 0f ? 1 : -1;
            projectile.rotation += MathHelper.ToRadians(22.5f) * projectile.direction;

            if (projectile.ai[0] == 1f)
            {
                float distance = 400f;
                if (projectile.ai[1] == -1)
                {
                    // used for finding a target
                    for (int i = 0; i < Main.maxNPCs; i++)
                    {
                        NPC npc = Main.npc[i];
                        if (npc.CanBeChasedBy(projectile, false))
                        {
                            Vector2 vec = npc.Center - projectile.Center;
                            float distanceTo = vec.Length();
                            distanceTo -= (float)Math.Sqrt(npc.width * npc.width + npc.height * npc.height) * 0.75f;
                            if (distanceTo < distance && Collision.CanHit(projectile.position, projectile.width, projectile.height, npc.position, npc.width, npc.height))
                                projectile.ai[1] = i;
                        }
                    }
                }
                else
                {
                    int index = (int)projectile.ai[1];
                    float speed = 34f;
                    float inertia = 15f;

                    if (!Main.npc[index].CanBeChasedBy(projectile, false))
                        projectile.ai[1] = -1;

                    Vector2 direction = Main.npc[index].Center - projectile.Center;
                    direction.Normalize();
                    direction *= speed;
                    projectile.velocity = (projectile.velocity * (inertia - 1) + direction) / inertia;
                }
            }
            else
            {
                projectile.velocity.X *= 0.9711f;
                projectile.velocity.Y += 0.426f;
            }

            if (Main.rand.NextBool(2))
            {
                Vector2 offset = new Vector2(12, 0).RotatedByRandom(MathHelper.ToRadians(360f));
                Vector2 velOffset = new Vector2(4, 0).RotatedBy(offset.ToRotation());
                Dust dust = Dust.NewDustPerfect(new Vector2(projectile.Center.X, projectile.Center.Y) + offset, DustID.GoldFlame, new Vector2(projectile.velocity.X * 0.2f + velOffset.X, projectile.velocity.Y * 0.2f + velOffset.Y), 100, new Color(255, 245, 198), 2f);
                dust.noGravity = true;
            }

            if (Main.rand.NextBool(6))
            {
                Vector2 offset = new Vector2(12, 0).RotatedByRandom(MathHelper.ToRadians(360f));
                Vector2 velOffset = new Vector2(4, 0).RotatedBy(offset.ToRotation());
                Dust dust = Dust.NewDustPerfect(new Vector2(projectile.Center.X, projectile.Center.Y) + offset, DustID.GoldFlame, new Vector2(projectile.velocity.X * 0.2f + velOffset.X, projectile.velocity.Y * 0.2f + velOffset.Y), 100, new Color(255, 245, 198), 2f);
                dust.noGravity = true;
            }
        }

        public override void Kill(int timeLeft)
        {
            Player player = Main.player[projectile.owner];
            if (projectile.ai[0] == 1f)
            {
                float numberOfDusts = 40f;
                float rotFactor = 360f / numberOfDusts;
                for (int i = 0; i < numberOfDusts; i++)
                {
                    float rot = MathHelper.ToRadians(i * rotFactor);
                    Vector2 offset = new Vector2(15f, 0).RotatedBy(rot);
                    Vector2 velOffset = new Vector2(10f, 0).RotatedBy(rot);
                    Dust dust = Dust.NewDustPerfect(projectile.position + offset, 269, new Vector2(velOffset.X, velOffset.Y));
                    dust.noGravity = true;
                    dust.velocity = velOffset;
                    dust.scale = 2.5f;
                    if (i % 2 == 0)
                    {
                        dust = Dust.NewDustPerfect(projectile.position + offset, 269, new Vector2(velOffset.X, velOffset.Y));
                        dust.noGravity = true;
                        dust.velocity = velOffset * 2f;
                        dust.scale = 2.5f;
                    }
                    if (i % 4 == 0)
                    {
                        dust = Dust.NewDustPerfect(projectile.position + offset, 269, new Vector2(velOffset.X, velOffset.Y));
                        dust.noGravity = true;
                        dust.velocity = velOffset / 2f;
                        dust.scale = 2.5f;
                    }
                }

                float distance = 240f;

                for (int k = 0; k < Main.maxNPCs; k++)
                {
                    NPC npc = Main.npc[k];
                    Vector2 vec = npc.Center - projectile.Center;
                    float distanceTo = (float)Math.Sqrt(vec.X * vec.X + vec.Y * vec.Y);
                    distanceTo -= (float)Math.Sqrt(npc.width * npc.width + npc.height * npc.height) * 0.75f;
                    if (distanceTo < distance && npc.CanBeChasedBy(projectile, false) && k != projectile.localAI[0])
                    {
                        float alldamage = projectile.damage * 1.25f;
                        double damage = npc.StrikeNPC((int)alldamage, projectile.knockBack, projectile.velocity.X > 0f ? 1 : -1);
                        player.addDPS((int)damage);
                    }
                }
                SoundEffectInstance sound1 = Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Item, "Sounds/Item/PwnagehammerHoming"), projectile.Center);
                if (sound1 != null)
                    sound1.Volume = 0.3f;
                SoundEffectInstance sound2 = Main.PlaySound(SoundID.Item14, projectile.Center);
                if (sound2 != null)
                    sound2.Volume = 0.22f;
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
                    Dust dust = Dust.NewDustPerfect(projectile.position + offset, 269, new Vector2(velOffset.X, velOffset.Y));
                    dust.noGravity = true;
                    dust.velocity = velOffset;
                    dust.scale = 2.5f;
                }

                float distance = 168f;

                for (int k = 0; k < Main.maxNPCs; k++)
                {
                    NPC npc = Main.npc[k];
                    Vector2 vec = npc.Center - projectile.Center;
                    float distanceTo = (float)Math.Sqrt(vec.X * vec.X + vec.Y * vec.Y);
                    distanceTo -= (float)Math.Sqrt(npc.width * npc.width + npc.height * npc.height) * 0.75f;
                    if (distanceTo < distance && npc.CanBeChasedBy(projectile, false) && k != projectile.localAI[0])
                    {
                        float alldamage = projectile.damage * 0.75f;
                        double damage = npc.StrikeNPC((int)alldamage, projectile.knockBack, projectile.velocity.X > 0f ? 1 : -1);
                        player.addDPS((int)damage);
                    }
                }
                SoundEffectInstance sound1 = Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Item, "Sounds/Item/PwnagehammerSound"), projectile.Center);
                if (sound1 != null)
                {
                    sound1.Volume = 0.16f;
                    sound1.Pitch = Main.rand.NextFloat(-0.08f, 0.08f);
                }
                SoundEffectInstance sound2 = Main.PlaySound(SoundID.Item14, projectile.Center);
                if (sound2 != null)
                    sound2.Volume = 0.11f;
            }
        }

        public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            if (projectile.ai[0] == 1f && Main.myPlayer == projectile.owner)
            {
                crit = true;
                int hammer = Projectile.NewProjectile(projectile.Center, new Vector2(0, -15f), ModContent.ProjectileType<PwnagehammerEcho>(), projectile.damage * 2, projectile.knockBack, projectile.owner, 0f, projectile.ai[1]);
                Main.projectile[hammer].localAI[0] = Math.Sign(projectile.velocity.X);
                Main.projectile[hammer].netUpdate = true;
            }
        }

        public override void ModifyHitPvp(Player target, ref int damage, ref bool crit)
        {
            if (projectile.ai[0] == 1f && Main.myPlayer == projectile.owner)
            {
                int hammer = Projectile.NewProjectile(projectile.Center, new Vector2(0, -15f), ModContent.ProjectileType<PwnagehammerEcho>(), projectile.damage * 2, projectile.knockBack, projectile.owner, 0f, projectile.ai[1]);
                Main.projectile[hammer].localAI[0] = Math.Sign(projectile.velocity.X);
                Main.projectile[hammer].netUpdate = true;
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            projectile.localAI[0] = target.whoAmI;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(projectile, ProjectileID.Sets.TrailingMode[projectile.type], lightColor, 1);
            return false;
        }
    }
}
