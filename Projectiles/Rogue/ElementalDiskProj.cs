using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Items.Weapons.Rogue;

namespace CalamityMod.Projectiles.Rogue
{
    public class ElementalDiskProj : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Rogue";
        public override string Texture => "CalamityMod/Items/Weapons/Rogue/ElementalDisk";
        private int Lifetime = 400;
        private int ReboundTime = 30;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 10;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 1;
        }

        public override void SetDefaults()
        {
            Projectile.width = 56;
            Projectile.height = 56;
            Projectile.ignoreWater = true;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 6;
            Projectile.penetrate = -1;
            Projectile.timeLeft = Lifetime;
            Projectile.DamageType = RogueDamageClass.Instance;
        }

        public override void AI()
        {
            SpawnProjectilesNearEnemies();
            BoomerangAI();
            LightingAndDust();
        }

        private void BoomerangAI()
        {
            // Boomerang rotation
            Projectile.rotation += 0.4f * Projectile.direction;

            // Boomerang sound
            if (Projectile.soundDelay == 0)
            {
                Projectile.soundDelay = 8;
                SoundEngine.PlaySound(SoundID.Item7, Projectile.position);
            }

            // Returns after some number of frames in the air
            int timeMult = Projectile.Calamity().stealthStrike ? ElementalDisk.stealthTimeMult : 1;
            if (Projectile.timeLeft < Lifetime * timeMult - ReboundTime * timeMult)
                Projectile.ai[0] = 1f;

            if (Projectile.ai[0] == 1f)
            {
                Player player = Main.player[Projectile.owner];
                float returnSpeed = 14f;
                float acceleration = Projectile.Calamity().stealthStrike ? 0.45f : 0.6f;
                Vector2 playerVec = player.Center - Projectile.Center;
                float dist = playerVec.Length();

                // Delete the projectile if it's excessively far away.
                if (dist > 3000f)
                    Projectile.Kill();

                playerVec.Normalize();
                playerVec *= returnSpeed;

                // Home back in on the player.
                if (Projectile.velocity.X < playerVec.X)
                {
                    Projectile.velocity.X += acceleration;
                    if (Projectile.velocity.X < 0f && playerVec.X > 0f)
                        Projectile.velocity.X += acceleration;
                }
                else if (Projectile.velocity.X > playerVec.X)
                {
                    Projectile.velocity.X -= acceleration;
                    if (Projectile.velocity.X > 0f && playerVec.X < 0f)
                        Projectile.velocity.X -= acceleration;
                }
                if (Projectile.velocity.Y < playerVec.Y)
                {
                    Projectile.velocity.Y += acceleration;
                    if (Projectile.velocity.Y < 0f && playerVec.Y > 0f)
                        Projectile.velocity.Y += acceleration;
                }
                else if (Projectile.velocity.Y > playerVec.Y)
                {
                    Projectile.velocity.Y -= acceleration;
                    if (Projectile.velocity.Y > 0f && playerVec.Y < 0f)
                        Projectile.velocity.Y -= acceleration;
                }

                // Delete the projectile if it touches its owner.
                if (Main.myPlayer == Projectile.owner)
                    if (Projectile.Hitbox.Intersects(player.Hitbox))
                        Projectile.Kill();
            }
        }

        private void SpawnProjectilesNearEnemies()
        {
            if (!Projectile.friendly)
                return;

            float maxDistance = 300f;
            bool homeIn = false;

            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC npc = Main.npc[i];
                if (npc.CanBeChasedBy(Projectile, false))
                {
                    float extraDistance = npc.width / 2 + npc.height / 2;

                    bool canHit = true;
                    if (extraDistance < maxDistance)
                        canHit = Collision.CanHit(Projectile.Center, 1, 1, npc.Center, 1, 1);

                    if (Vector2.Distance(npc.Center, Projectile.Center) < maxDistance + extraDistance && canHit)
                    {
                        homeIn = true;
                        break;
                    }
                }
            }

            if (homeIn)
            {
                int counter = Projectile.Calamity().stealthStrike ? 40 : 60;
                if (Main.player[Projectile.owner].miscCounter % counter == 0)
                {
                    int splitProj = ModContent.ProjectileType<ElementalDiskSplit>();
                    if (Projectile.owner == Main.myPlayer && Main.player[Projectile.owner].ownedProjectileCounts[splitProj] < 16)
                    {
                        float spread = 45f * 0.0174f;
                        double startAngle = Math.Atan2(Projectile.velocity.X, Projectile.velocity.Y) - spread / 2;
                        double deltaAngle = spread / 8f;
                        double offsetAngle;
                        for (int i = 0; i < 4; i++)
                        {
                            offsetAngle = startAngle + deltaAngle * (i + i * i) / 2f + 32f * i;
                            int disk = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center.X, Projectile.Center.Y, (float)(Math.Sin(offsetAngle) * 5f), (float)(Math.Cos(offsetAngle) * 5f), splitProj, Projectile.damage, Projectile.knockBack, Projectile.owner);
                            int disk2 = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center.X, Projectile.Center.Y, (float)(-Math.Sin(offsetAngle) * 5f), (float)(-Math.Cos(offsetAngle) * 5f), splitProj, Projectile.damage, Projectile.knockBack, Projectile.owner);
                            if (Projectile.Calamity().stealthStrike)
                            {
                                Main.projectile[disk].idStaticNPCHitCooldown = Main.projectile[disk2].idStaticNPCHitCooldown = 8;
                                Main.projectile[disk].usesIDStaticNPCImmunity = Main.projectile[disk2].usesIDStaticNPCImmunity = true;
                                Main.projectile[disk].timeLeft = Main.projectile[disk2].timeLeft = 60;
                            }
                        }
                    }
                }
            }
        }

        private void LightingAndDust()
        {
            if (Main.rand.NextBool(3))
            {
                int rainbow = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 66, Projectile.direction * 2, 0f, 150, new Color(Main.DiscoR, Main.DiscoG, Main.DiscoB), 1.3f);
                Main.dust[rainbow].noGravity = true;
                Main.dust[rainbow].velocity *= 0f;
            }

            Lighting.AddLight(Projectile.Center, 0.15f, 1f, 0.25f);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<ElementalMix>(), 60);

            if (!Projectile.Calamity().stealthStrike)
                Projectile.ai[0] = 1f;
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            if (!Projectile.Calamity().stealthStrike)
                Projectile.ai[0] = 1f;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], lightColor, 2);
            return false;
        }
    }
}
