using CalamityMod.CalPlayer;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Summon
{
    public class ColdDivinityPointyThing : ModProjectile
    {
        public int recharging = -1;
        public bool circling = true;
        public bool circlingPlayer = true;
        public float floatyDistance = 90f;
        public NPC target = null;

        private void homingAi()
        {
            if (projectile.timeLeft <= 240)
            {
                if (target != null)
                {
                    target.checkDead();
                    if (target.life <= 0 || !target.active || !target.CanBeChasedBy(this, false))
                        target = null;
                }
                if (target == null)
                    target = CalamityUtils.MinionHoming(projectile.Center, 1000f, Main.player[projectile.owner]);
                if (target != null) //target found
                {
                    float num550 = 40f;
                    Vector2 vector43 = new Vector2(projectile.position.X + (float)projectile.width * 0.5f, projectile.position.Y + (float)projectile.height * 0.5f);
                    float num551 = target.Center.X - vector43.X;
                    float num552 = target.Center.Y - vector43.Y;
                    float num553 = (float)Math.Sqrt((double)(num551 * num551 + num552 * num552));
                    if (num553 < 100f)
                    {
                        num550 = 28f; //14
                    }
                    num553 = num550 / num553;
                    num551 *= num553;
                    num552 *= num553;
                    projectile.velocity.X = (projectile.velocity.X * 20f + num551) / 21f;
                    projectile.velocity.Y = (projectile.velocity.Y * 20f + num552) / 21f;
                }
            }
        }

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Ice Castle Shard");
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 7;
        }

        public override void SetDefaults()
        {
            projectile.width = 28;
            projectile.height = 60;
            projectile.netImportant = true;
            projectile.friendly = true;
            projectile.ignoreWater = true;
            projectile.minionSlots = 0f;
            projectile.timeLeft = 18000;
            projectile.penetrate = 1;
            projectile.tileCollide = false;
            projectile.timeLeft *= 5;
            projectile.minion = true;
        }
        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(recharging);
            writer.Write(circling);
            writer.Write(circlingPlayer);
            writer.Write((double)floatyDistance);
            writer.Write(target is null ? -1 : target.whoAmI);
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            recharging = reader.ReadInt32();
            circling = reader.ReadBoolean();
            circlingPlayer = reader.ReadBoolean();
            floatyDistance = (float)reader.ReadDouble();
            int targ = reader.ReadInt32();
            target = targ == -1 ? null : Main.npc[targ];
        }

        private void dust(int dustAmt)
        {
            for (int i = 0; i < dustAmt; i++)
            {
                Dust.NewDust(projectile.Center, projectile.width, projectile.height, DustID.Ice, Main.rand.NextFloat(1, 3), Main.rand.NextFloat(1, 3), 0, Color.Cyan, Main.rand.NextFloat(0.5f, 1.5f));
            }
        }

        public override bool PreAI()
        {
            if (recharging == -1)
            {
                recharging = projectile.ai[1] == 0f ? 210 : 0;
                dust(30);
            }
            if (projectile.ai[1] == 1f && projectile.timeLeft > 1000)
            {
                projectile.ai[1] = 0f;
                projectile.timeLeft = 250;
                circling = circlingPlayer = false;
                projectile.netUpdate = true;
            }
            else if (projectile.ai[1] >= 2f && projectile.timeLeft > 900)
            {
                target = CalamityUtils.MinionHoming(projectile.Center, 1000f, Main.player[projectile.owner]);
                if (target != null)
                {
                    projectile.timeLeft = 669;
                    projectile.ai[1]++;
                    circlingPlayer = false;
                    float height = target.getRect().Height;
                    float width = target.getRect().Width;
                    floatyDistance = MathHelper.Min((height > width ? height : width) * 3f, (Main.LogicCheckScreenWidth * Main.LogicCheckScreenHeight) / 2);
                    if (floatyDistance > Main.LogicCheckScreenWidth / 3)
                        floatyDistance = Main.LogicCheckScreenWidth / 3;
                    projectile.penetrate = -1;
                    projectile.usesIDStaticNPCImmunity = true;
                    projectile.idStaticNPCHitCooldown = 4;
                    projectile.netUpdate = true;
                }
            }
            if (circlingPlayer)
            {
                ProjectileID.Sets.MinionSacrificable[projectile.type] = true;
                if (projectile.penetrate == 1)
                {
                    projectile.penetrate++;
                }
            }
            return true;
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

            if (player.dead)
            {
                modPlayer.coldDivinity = false;
            }
            if (circlingPlayer)
            {
                projectile.minionSlots = 1f;
                projectile.timeLeft = 2;
                if (!modPlayer.coldDivinity && recharging > 0)
                    projectile.Kill();

            }
            if (circling && !circlingPlayer)
            {
                if (target != null && (!target.active || target.life <= 0))
                {
                    projectile.Kill();
                }
            }
            if (recharging > 0)
            {
                recharging--;
                if (recharging == 0)
                {
                    dust(15);
                    SoundEffectInstance sound = Main.PlaySound(SoundID.Item30, projectile.position);
                    if (sound != null)
                    {
                        sound.Pitch = 0.2f;
                    }
                    projectile.netUpdate = true;
                }
            }
            if (circling)
            {
                if (circling && !circlingPlayer && projectile.timeLeft < 120)
                {
                    recharging = 0;
                    projectile.usesIDStaticNPCImmunity = false;
                    projectile.penetrate = 1;
                    float applicableDist = target.getRect().Width > target.getRect().Height ? target.getRect().Width : target.getRect().Height;
                    if (projectile.timeLeft > 60)
                        floatyDistance += 5;
                    else
                        floatyDistance -= 10;
                }
                if (circlingPlayer)
                {
                    float math = recharging == 0 ? 90f :(300 - recharging) / 3;
                    float regularDistance = math > 90f ? 90f : math;
                    projectile.Center = player.Center + projectile.ai[0].ToRotationVector2() * regularDistance;
                    projectile.rotation = projectile.ai[0] + (float)Math.Atan(90);
                    projectile.ai[0] -= MathHelper.ToRadians(4f);
                    NPC target = recharging > 0 ? null : CalamityUtils.MinionHoming(projectile.Center, 800f, player);
                    if (target != null && projectile.owner == Main.myPlayer)
                    {
                        recharging = 180;
                        Vector2 velocity = projectile.ai[0].ToRotationVector2().RotatedBy(Math.Atan(0));
                        velocity.Normalize();
                        velocity *= 20f;
                        Projectile.NewProjectile(projectile.position, velocity, projectile.type, (int)(projectile.damage * 1.05f), projectile.knockBack, projectile.owner, projectile.ai[0], 1f);

                    }
                    projectile.netUpdate = projectile.owner == Main.myPlayer;
                }
                else
                {
                    projectile.Center = target.Center + projectile.ai[0].ToRotationVector2() * floatyDistance;
                    projectile.rotation = projectile.ai[0] + (float)Math.Atan(90);
                    Vector2 vec = projectile.rotation.ToRotationVector2() - target.Center;
                    vec.Normalize();
                    if (projectile.timeLeft <= 120)
                        projectile.rotation = projectile.timeLeft <= 60 ? projectile.ai[0] - (float)Math.Atan(90) : projectile.rotation - (MathHelper.Distance(projectile.rotation, -projectile.rotation) / (120 - 60));
                    projectile.ai[0] -= MathHelper.ToRadians(4f);
                }
            }
            else
            {
                projectile.rotation = projectile.velocity.ToRotation() + (float)Math.Atan(90);
                homingAi();
            }
        }

        public override bool CanDamage()
        {
            return recharging <= 0 && (circlingPlayer || (circling && (projectile.timeLeft >= 120 || projectile.timeLeft <= 45)) || !circling) && !projectile.hide;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(BuffID.Frostburn, 300);
            int circlers = 0;
            for (int i = 0; i < Main.projectile.Length; i++)
            {
                if (Main.projectile[i].active && Main.projectile[i].owner == projectile.owner && Main.projectile[i].type == projectile.type)
                {
                    ColdDivinityPointyThing pointy = (ColdDivinityPointyThing)Main.projectile[i].modProjectile;
                    if (Main.projectile[i].ai[1] > 2f)
                        circlers = circlers + Main.rand.Next(1, 4);
                }
            }
            circlers = (int)MathHelper.Min(Main.rand.Next(15, 21), circlers);
            if (projectile.ai[1] > 2f)
                projectile.ai[1]++;
            if (projectile.ai[1] >= (30f - circlers) && projectile.timeLeft >= 120)
                recharging = projectile.timeLeft > 121 ? projectile.timeLeft - 121 : 0;

            if (circling && target == this.target && projectile.timeLeft < 60)
            {
                if (projectile.timeLeft < 60)
                    projectile.Kill();
            }
            else if (circlingPlayer)
            {
                recharging = 300;
                dust(20);
            }
        }

        public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            if (circling && target == this.target && projectile.timeLeft < 60)
            {
                dust(30);
                Main.PlaySound(SoundID.NPCHit5, projectile.position);
                damage = (int)(damage * 1.1f);
            }
            else if (circling && target == this.target && projectile.timeLeft > 60)
            {
                dust(5);
                damage = (int)(damage * 0.2f); //nerfffffff the nerf because nerf? nerf.
            }
        }

        public override void OnHitPvp(Player target, int damage, bool crit)
        {
            target.AddBuff(BuffID.Frostburn, 300);
            if (circlingPlayer)
            {
                recharging = 300;
                dust(20);
            }
        }

        public override void Kill(int timeLeft)
        {
            Main.PlaySound(SoundID.NPCHit5, projectile.position);
            dust(20);
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(recharging > 0 ? lightColor.R : 53, recharging > 0 ? lightColor.G : Main.DiscoG, recharging > 0 ? lightColor.B : 255, recharging > 200 ? 255 : 255 - recharging);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            if (!circling || (!circlingPlayer && recharging == 0))
            {
                CalamityUtils.DrawAfterimagesCentered(projectile, ProjectileID.Sets.TrailingMode[projectile.type], lightColor, !circlingPlayer ? 1 : 3);
            }
            return true;
        }
    }
}
