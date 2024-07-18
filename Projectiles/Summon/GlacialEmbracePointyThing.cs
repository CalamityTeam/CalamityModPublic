using System;
using System.IO;
using CalamityMod.CalPlayer;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Summon
{
    public class GlacialEmbracePointyThing : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Summon";
        public int recharging = -1;
        public bool circling = true;
        public bool circlingPlayer = true;
        public float floatyDistance = 90f;
        public NPC target = null;

        private void homingAi()
        {
            if (Projectile.timeLeft <= 240)
            {
                if (target != null)
                {
                    target.checkDead();
                    if (target.life <= 0 || !target.active || !target.CanBeChasedBy(this, false))
                        target = null;
                }
                if (target == null)
                    target = CalamityUtils.MinionHoming(Projectile.Center, 1000f, Main.player[Projectile.owner]);
                if (target != null) //target found
                {
                    float projVel = 40f;
                    Vector2 targetDirection = Projectile.Center;
                    float targetX = target.Center.X - targetDirection.X;
                    float targetY = target.Center.Y - targetDirection.Y;
                    float targetDist = (float)Math.Sqrt((double)(targetX * targetX + targetY * targetY));
                    if (targetDist < 100f)
                    {
                        projVel = 28f; //14
                    }
                    targetDist = projVel / targetDist;
                    targetX *= targetDist;
                    targetY *= targetDist;
                    Projectile.velocity.X = (Projectile.velocity.X * 20f + targetX) / 21f;
                    Projectile.velocity.Y = (Projectile.velocity.Y * 20f + targetY) / 21f;
                }
            }
        }

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 7;
        }

        public override void SetDefaults()
        {
            Projectile.width = 28;
            Projectile.height = 60;
            Projectile.netImportant = true;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.minionSlots = 0f;
            Projectile.timeLeft = 18000;
            Projectile.penetrate = 1;
            Projectile.tileCollide = false;
            Projectile.timeLeft *= 5;
            Projectile.minion = true;
            Projectile.DamageType = DamageClass.Summon;
        }
        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(recharging);
            writer.Write(circling);
            writer.Write(circlingPlayer);
            writer.Write(floatyDistance);
            writer.Write(target is null ? -1 : target.whoAmI);
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            recharging = reader.ReadInt32();
            circling = reader.ReadBoolean();
            circlingPlayer = reader.ReadBoolean();
            floatyDistance = reader.ReadSingle();
            int targ = reader.ReadInt32();
            target = targ == -1 ? null : Main.npc[targ];
        }

        private void dust(int dustAmt)
        {
            for (int i = 0; i < dustAmt; i++)
            {
                Dust.NewDust(Projectile.Center, Projectile.width, Projectile.height, DustID.Ice, Main.rand.NextFloat(1, 3), Main.rand.NextFloat(1, 3), 0, Color.Cyan, Main.rand.NextFloat(0.5f, 1.5f));
            }
        }

        public override bool PreAI()
        {
            if (recharging == -1)
            {
                recharging = Projectile.ai[1] == 0f ? 210 : 0;
                dust(30);
            }
            if (Projectile.ai[1] == 1f && Projectile.timeLeft > 1000)
            {
                Projectile.ai[1] = 0f;
                Projectile.timeLeft = 250;
                circling = circlingPlayer = false;
                Projectile.netUpdate = true;
            }
            else if (Projectile.ai[1] >= 2f && Projectile.timeLeft > 900)
            {
                target = CalamityUtils.MinionHoming(Projectile.Center, 1000f, Main.player[Projectile.owner]);
                if (target != null)
                {
                    Projectile.timeLeft = 669;
                    Projectile.ai[1]++;
                    circlingPlayer = false;
                    float height = target.getRect().Height;
                    float width = target.getRect().Width;
                    floatyDistance = MathHelper.Min((height > width ? height : width) * 3f, (Main.LogicCheckScreenWidth * Main.LogicCheckScreenHeight) / 2);
                    if (floatyDistance > Main.LogicCheckScreenWidth / 3)
                        floatyDistance = Main.LogicCheckScreenWidth / 3;
                    Projectile.penetrate = -1;
                    Projectile.usesIDStaticNPCImmunity = true;
                    Projectile.idStaticNPCHitCooldown = 4;
                    Projectile.netUpdate = true;
                }
            }
            if (circlingPlayer)
            {
                ProjectileID.Sets.MinionSacrificable[Projectile.type] = true;
                if (Projectile.penetrate == 1)
                {
                    Projectile.penetrate++;
                }
            }
            return true;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            CalamityPlayer modPlayer = player.Calamity();
            if (player.dead)
            {
                modPlayer.GlacialEmbrace = false;
            }

            if (!modPlayer.GlacialEmbrace)
            {
                Projectile.active = false;
                return;
            }
            if (circlingPlayer)
            {
                Projectile.minionSlots = 1f;
                Projectile.timeLeft = 2;
                if (!modPlayer.GlacialEmbrace && recharging > 0)
                    Projectile.Kill();

            }
            if (circling && !circlingPlayer)
            {
                if (target != null && (!target.active || target.life <= 0))
                {
                    Projectile.Kill();
                }
            }
            if (recharging > 0)
            {
                recharging--;
                if (recharging == 0)
                {
                    dust(15);
                    SoundEngine.PlaySound(SoundID.Item30 with { Pitch = 0.2f }, Projectile.position);
                    Projectile.netUpdate = true;
                }
            }
            if (circling)
            {
                if (circling && !circlingPlayer && Projectile.timeLeft < 120)
                {
                    recharging = 0;
                    Projectile.usesIDStaticNPCImmunity = false;
                    Projectile.penetrate = 1;
                    float applicableDist = target.getRect().Width > target.getRect().Height ? target.getRect().Width : target.getRect().Height;
                    if (Projectile.timeLeft > 60)
                        floatyDistance += 5;
                    else
                        floatyDistance -= 10;
                }
                if (circlingPlayer)
                {
                    float math = recharging == 0 ? 90f : (300 - recharging) / 3;
                    float regularDistance = math > 90f ? 90f : math;
                    Projectile.Center = player.Center + Projectile.ai[0].ToRotationVector2() * regularDistance;
                    Projectile.rotation = Projectile.ai[0] + (float)Math.Atan(90);
                    Projectile.ai[0] -= MathHelper.ToRadians(4f);
                    NPC target = recharging > 0 ? null : CalamityUtils.MinionHoming(Projectile.Center, 800f, player);
                    if (target != null && Projectile.owner == Main.myPlayer)
                    {
                        recharging = 180;
                        Vector2 velocity = Projectile.ai[0].ToRotationVector2().RotatedBy(Math.Atan(0));
                        velocity.Normalize();
                        velocity *= 20f;
                        int shard = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.position, velocity, Projectile.type, (int)(Projectile.damage * 1.05f), Projectile.knockBack, Projectile.owner, Projectile.ai[0], 1f);
                        if (Main.projectile.IndexInRange(shard))
                            Main.projectile[shard].originalDamage = (int)(Projectile.originalDamage * 1.05f);
                    }
                    Projectile.netUpdate = Projectile.owner == Main.myPlayer;
                }
                else
                {
                    Projectile.Center = target.Center + Projectile.ai[0].ToRotationVector2() * floatyDistance;
                    Projectile.rotation = Projectile.ai[0] + (float)Math.Atan(90);
                    Vector2 vec = Projectile.rotation.ToRotationVector2() - target.Center;
                    vec.Normalize();
                    if (Projectile.timeLeft <= 120)
                        Projectile.rotation = Projectile.timeLeft <= 60 ? Projectile.ai[0] - (float)Math.Atan(90) : Projectile.rotation - (MathHelper.Distance(Projectile.rotation, -Projectile.rotation) / (120 - 60));
                    Projectile.ai[0] -= MathHelper.ToRadians(4f);
                }
            }
            else
            {
                Projectile.rotation = Projectile.velocity.ToRotation() + (float)Math.Atan(90);
                homingAi();
            }
        }

        public override bool? CanDamage()
        {
            return recharging <= 0 && (circlingPlayer || (circling && (Projectile.timeLeft >= 120 || Projectile.timeLeft <= 45)) || !circling) && !Projectile.hide ? null : false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.Frostburn2, 300);
            int circlers = 0;
            foreach (Projectile p in Main.ActiveProjectiles)
            {
                if (p.owner == Projectile.owner && p.type == Projectile.type)
                {
                    GlacialEmbracePointyThing pointy = (GlacialEmbracePointyThing)p.ModProjectile;
                    if (p.ai[1] > 2f)
                        circlers += Main.rand.Next(1, 4);
                }
            }
            circlers = (int)MathHelper.Min(Main.rand.Next(15, 21), circlers);
            if (Projectile.ai[1] > 2f)
                Projectile.ai[1]++;
            if (Projectile.ai[1] >= (30f - circlers) && Projectile.timeLeft >= 120)
                recharging = Projectile.timeLeft > 121 ? Projectile.timeLeft - 121 : 0;

            if (circling && target == this.target && Projectile.timeLeft < 60)
            {
                if (Projectile.timeLeft < 60)
                    Projectile.Kill();
            }
            else if (circlingPlayer)
            {
                recharging = 300;
                dust(20);
            }
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            if (circling && target == this.target && Projectile.timeLeft < 60)
            {
                dust(30);
                SoundEngine.PlaySound(SoundID.NPCHit5, Projectile.position);
                modifiers.SourceDamage *= 1.1f;
            }
            else if (circling && target == this.target && Projectile.timeLeft > 60)
            {
                dust(5);
                modifiers.SourceDamage *= 0.2f; //nerfffffff the nerf because nerf? nerf.
            }
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(BuffID.Frostburn2, 300);
            if (circlingPlayer)
            {
                recharging = 300;
                dust(20);
            }
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.NPCHit5, Projectile.position);
            dust(20);
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(recharging > 0 ? lightColor.R : 53, recharging > 0 ? lightColor.G : Main.DiscoG, recharging > 0 ? lightColor.B : 255, recharging > 200 ? 255 : 255 - recharging);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (!circling || (!circlingPlayer && recharging == 0))
            {
                CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], lightColor, !circlingPlayer ? 1 : 3);
            }
            return true;
        }
    }
}
