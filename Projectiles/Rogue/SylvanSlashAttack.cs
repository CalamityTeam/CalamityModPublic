using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.CalPlayer;
using Terraria.Audio;

namespace CalamityMod.Projectiles.Rogue
{
    public class SylvanSlashAttack : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Sylvan Slash");
            Main.projFrames[Projectile.type] = 28;
        }

        public override void SetDefaults()
        {
            Projectile.width = 68;
            Projectile.height = 68;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.Calamity().rogue = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 6;
        }

        public override void AI()
        {
            if (Main.myPlayer != Projectile.owner)
                return;

            Player player = Main.player[Projectile.owner];
            float num = 0f;
            Vector2 vector = player.RotatedRelativePoint(player.MountedCenter, true);
            if (Projectile.spriteDirection == -1)
            {
                num = MathHelper.Pi;
            }
            if (++Projectile.frame >= Main.projFrames[Projectile.type])
            {
                Projectile.frame = 0;
            }
            Projectile.soundDelay--;
            if (Projectile.soundDelay <= 0)
            {
                SoundEngine.PlaySound(SoundID.Item15, Projectile.Center);
                Projectile.soundDelay = 24;
            }
            if (player.channel && !player.noItems && !player.CCed)
            {
                float scaleFactor6 = 1f;
                if (player.ActiveItem().shoot == Projectile.type)
                {
                    scaleFactor6 = player.ActiveItem().shootSpeed * Projectile.scale;
                }
                Vector2 vector13 = Main.MouseWorld - vector;
                vector13.Normalize();
                if (vector13.HasNaNs())
                {
                    vector13 = Vector2.UnitX * (float)player.direction;
                }
                vector13 *= scaleFactor6;
                if (vector13.X != Projectile.velocity.X || vector13.Y != Projectile.velocity.Y)
                {
                    Projectile.netUpdate = true;
                }
                Projectile.velocity = vector13;
            }
            else
            {
                Projectile.Kill();
            }
            Vector2 vector14 = Projectile.Center + Projectile.velocity * 3f;
            Lighting.AddLight(vector14, 0.2f, 2f, 3f);
            if (Main.rand.NextBool(3))
            {
                int num30 = Dust.NewDust(vector14 - Projectile.Size / 2f, Projectile.width, Projectile.height, 111, Projectile.velocity.X, Projectile.velocity.Y, 100, default, 2f);
                Main.dust[num30].noGravity = true;
                Main.dust[num30].position -= Projectile.velocity;
            }
            Projectile.position = player.RotatedRelativePoint(Main.MouseWorld, true) - Projectile.Size / 2f;
            Projectile.rotation = Projectile.velocity.ToRotation() + num;
            Projectile.spriteDirection = Projectile.direction;
            Projectile.timeLeft = 2;
            player.ChangeDir(Projectile.direction);
            player.heldProj = Projectile.whoAmI;
            player.itemTime = 2;
            player.itemAnimation = 2;
            player.itemRotation = (float)Math.Atan2((double)(Projectile.velocity.Y * (float)Projectile.direction), (double)(Projectile.velocity.X * (float)Projectile.direction));

            if (Projectile.ai[0] > 0)
                Projectile.ai[0]--;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            Player player = Main.player[Projectile.owner];
            CalamityPlayer modPlayer = player.Calamity();
            if (Projectile.owner == Main.myPlayer)
            {
                if (Projectile.ai[0] <= 0f)
                {
                    if ((target.damage > 5 || target.boss) && !target.SpawnedFromStatue)
                    {
                        if (modPlayer.wearingRogueArmor && modPlayer.rogueStealthMax != 0)
                        {
                            if (modPlayer.rogueStealth < modPlayer.rogueStealthMax)
                            {
                                modPlayer.rogueStealth += 0.05f;
                                Projectile.ai[0] = 3f;
                                if (modPlayer.rogueStealth > modPlayer.rogueStealthMax)
                                    modPlayer.rogueStealth = modPlayer.rogueStealthMax;
                            }
                        }
                    }
                }
                if (Main.rand.NextBool(8))
                {
                    float speedMult = Main.rand.NextFloat(3,6);
                    Vector2 vector1 = new Vector2(Projectile.Center.X - player.Center.X, Projectile.Center.Y - player.Center.Y);
                    vector1.Normalize();
                    Projectile.NewProjectile(Projectile.GetProjectileSource_FromThis(), player.Center.X, player.Center.Y, -vector1.X * speedMult, -vector1.Y * speedMult, ModContent.ProjectileType<SylvanSlash>(), (int)(Projectile.damage * 0.5), Projectile.knockBack, Projectile.owner, 0f, 0f);
                }
            }
        }

        public override void OnHitPvp(Player target, int damage, bool crit)
        {
            Player player = Main.player[Projectile.owner];
            CalamityPlayer modPlayer = player.Calamity();
            if (Projectile.owner == Main.myPlayer)
            {
                if (Projectile.ai[0] <= 0f)
                {
                    if (modPlayer.wearingRogueArmor && modPlayer.rogueStealthMax != 0)
                    {
                        if (modPlayer.rogueStealth < modPlayer.rogueStealthMax)
                        {
                            modPlayer.rogueStealth += 0.05f;
                            if (modPlayer.rogueStealth > modPlayer.rogueStealthMax)
                                modPlayer.rogueStealth = modPlayer.rogueStealthMax;
                        }
                    }
                }
                if (Main.rand.NextBool(8))
                {
                    float speedMult = Main.rand.NextFloat(3,6);
                    Vector2 vector1 = new Vector2(Projectile.Center.X - player.Center.X, Projectile.Center.Y - player.Center.Y);
                    vector1.Normalize();
                    Projectile.NewProjectile(Projectile.GetProjectileSource_FromThis(), player.Center.X, player.Center.Y, -vector1.X * speedMult, -vector1.Y * speedMult, ModContent.ProjectileType<SylvanSlash>(), (int)(Projectile.damage * 0.5), Projectile.knockBack, Projectile.owner, 0f, 0f);
                }
            }
        }
    }
}
