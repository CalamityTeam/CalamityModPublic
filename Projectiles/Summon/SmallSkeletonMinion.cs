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
    public class SmallSkeletonMinion : ModProjectile
    {
        public int Variant;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Small Skeleton");
            ProjectileID.Sets.MinionSacrificable[projectile.type] = true;
            ProjectileID.Sets.MinionTargettingFeature[projectile.type] = true;
            Main.projFrames[projectile.type] = 7;
        }

        public override void SetDefaults()
        {
            projectile.width = 24;
            projectile.height = 34;
            projectile.netImportant = true;
            projectile.friendly = true;
            projectile.ignoreWater = true;
            projectile.aiStyle = aiType = -1;
            projectile.minionSlots = 1f;
            projectile.timeLeft = 18000;
            projectile.penetrate = -1;
            projectile.tileCollide = true;
            projectile.timeLeft *= 5;
            projectile.minion = true;
        }
        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(Variant);
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            Variant = reader.ReadInt32();
        }
        public override void AI()
        {
            Player player = Main.player[projectile.owner];
            CalamityPlayer modPlayer = player.Calamity();

            if (projectile.localAI[0] == 0f)
            {
                projectile.Calamity().spawnedPlayerMinionDamageValue = player.MinionDamage();
                projectile.Calamity().spawnedPlayerMinionProjectileDamageValue = projectile.damage;
                Variant = Main.rand.Next(3);
                projectile.netUpdate = true;
                projectile.localAI[0] += 1f;
            }
            if (player.MinionDamage() != projectile.Calamity().spawnedPlayerMinionDamageValue)
            {
                int trueDamage = (int)(projectile.Calamity().spawnedPlayerMinionProjectileDamageValue /
                    projectile.Calamity().spawnedPlayerMinionDamageValue *
                    player.MinionDamage());
                projectile.damage = trueDamage;
            }

            projectile.frameCounter++;
            if (projectile.frameCounter % 6f == 5f)
            {
                projectile.frame++;
                projectile.frameCounter = 0;
            }
            if (projectile.frame < 1)
            {
                projectile.frame = 1;
            }
            if (projectile.frame >= Main.projFrames[projectile.type])
            {
                projectile.frame = 1;
            }

            bool isCorrectProjectile = projectile.type == ModContent.ProjectileType<SmallSkeletonMinion>();
            player.AddBuff(ModContent.BuffType<StaffOfNecrosteocytesBuff>(), 3600);
            if (isCorrectProjectile)
            {
                if (player.dead)
                {
                    modPlayer.necrosteocytesDudes = false;
                }
                if (modPlayer.necrosteocytesDudes)
                {
                    projectile.timeLeft = 2;
                }
            }
            NPC potentialTarget = projectile.Center.MinionHoming(900f, player);
            if (projectile.velocity.Y == 0 && (HoleBelow() || (projectile.Distance(player.Center) > 205f && projectile.position.X == projectile.oldPosition.X)))
            {
                projectile.velocity.Y = -10f;
            }
            else if (projectile.velocity.Y != 0f)
            {
                projectile.frame = 2;
            }
            if (projectile.velocity.Y > -16f)
            {
                projectile.velocity.Y += 0.3f;
            }

            projectile.ai[0]++;
            if (projectile.ai[0] % 60f == 59f && Main.myPlayer == projectile.owner)
            {
                int type = Utils.SelectRandom(Main.rand, ModContent.ProjectileType<BoneMatter>(), ModContent.ProjectileType<BoneMatter2>());
                Projectile.NewProjectile(projectile.Center, Vector2.Zero, type, projectile.damage, projectile.knockBack, projectile.owner);
            }
            if (potentialTarget is null)
            {
                if (Math.Abs(player.Center.X - projectile.Center.X + 40f * projectile.minionPos) > 160f)
                {
                    projectile.velocity.X += Main.rand.NextFloat(0.11f, 0.16f) * (player.Center.X - projectile.Center.X + 40f * projectile.minionPos > 0f).ToDirectionInt();
                    projectile.velocity.X = MathHelper.Clamp(projectile.velocity.X, -13f, 13f);
                }
                else
                {
                    projectile.velocity.X *= 0.95f;
                }
                if (projectile.Distance(player.Center) <= 150f)
                {
                    projectile.frame = 0;
                }
                if (projectile.Distance(player.Center) > 1600f)
                {
                    projectile.Center = player.Center;
                    projectile.netUpdate = true;
                }
            }
            else
            {
                if (Math.Abs(potentialTarget.Center.X - projectile.Center.X + (int)MathHelper.Min(potentialTarget.width / 2, 40f)) > (int)MathHelper.Min(potentialTarget.width / 2, 40f))
                {
                    projectile.velocity.X += Main.rand.NextFloat(0.08f, 0.15f) * (potentialTarget.Center.X - projectile.Center.X > 0f).ToDirectionInt();
                    projectile.velocity.X = MathHelper.Clamp(projectile.velocity.X, -16f, 16f);
                }
                else
                {
                    projectile.velocity.X *= 0.95f;
                }
                if (projectile.Distance(player.Center) > 1400f)
                {
                    projectile.Center = player.Center;
                    projectile.netUpdate = true;
                }
                projectile.MinionAntiClump(0.075f);
            }
            if (projectile.velocity.X > 0.25f)
                projectile.spriteDirection = 1;
            else if (projectile.velocity.X < -0.25f)
                projectile.spriteDirection = -1;
        }
        public bool HoleBelow()
        {
            int tileWidth = 4;
            int tileX = (int)(projectile.Center.X / 16f) - tileWidth;
            if (projectile.velocity.X > 0)
            {
                tileX += tileWidth;
            }
            int tileY = (int)((projectile.position.Y + projectile.height) / 16f);
            for (int y = tileY; y < tileY + 2; y++)
            {
                for (int x = tileX; x < tileX + tileWidth; x++)
                {
                    if (Main.tile[x, y].active())
                    {
                        return false;
                    }
                }
            }
            return true;
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Rectangle frame = new Rectangle(Variant * projectile.width, projectile.frame * projectile.height, projectile.width, projectile.height);
            SpriteEffects spriteEffects = (projectile.spriteDirection == 1) ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            spriteBatch.Draw(ModContent.GetTexture(Texture), projectile.Center - Main.screenPosition, frame, Color.White, projectile.rotation, projectile.Size / 2f, 1f, spriteEffects, 0f);
            return false;
        }
        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough)
        {
            fallThrough = projectile.Bottom.Y < Main.player[projectile.owner].Top.Y - 120f;
            return base.TileCollideStyle(ref width, ref height, ref fallThrough);
        }
        public override bool OnTileCollide(Vector2 oldVelocity) => false;
    }
}
