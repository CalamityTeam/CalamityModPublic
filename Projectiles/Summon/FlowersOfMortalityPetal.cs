using CalamityMod.Buffs.Summon;
using CalamityMod.CalPlayer;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Summon
{
    public class FlowersOfMortalityPetal : ModProjectile
    {
        public Player Owner => Main.player[projectile.owner];
        public ref float OffsetAngle => ref projectile.ai[0];
        public ref float Time => ref projectile.ai[1];
        public float Hue => OffsetAngle % MathHelper.TwoPi / MathHelper.TwoPi % 1f;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Elemental Petal");
            ProjectileID.Sets.TrailingMode[projectile.type] = 2;
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 8;
            ProjectileID.Sets.MinionSacrificable[projectile.type] = true;
            ProjectileID.Sets.MinionTargettingFeature[projectile.type] = true;
        }

        public override void SetDefaults()
        {
            projectile.width = projectile.height = 30;
            projectile.netImportant = true;
            projectile.friendly = true;
            projectile.ignoreWater = true;
            projectile.minionSlots = 0.5f;
            projectile.timeLeft = 90000;
            projectile.penetrate = -1;
            projectile.tileCollide = false;
            projectile.minion = true;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 4;
        }

        public override void AI()
        {
            bool isCorrectMinion = projectile.type == ModContent.ProjectileType<FlowersOfMortalityPetal>();
            Player player = Main.player[projectile.owner];
            CalamityPlayer modPlayer = player.Calamity();
            player.AddBuff(ModContent.BuffType<FlowersOfMortalityBuff>(), 3600);
            if (isCorrectMinion)
            {
                if (player.dead)
                    modPlayer.flowersOfMortality = false;
                if (modPlayer.flowersOfMortality)
                    projectile.timeLeft = 2;
            }

            SetProjectileDamage();

            Time++;
            NPC potentialTarget = projectile.Center.MinionHoming(1050f, Owner, false);
            if (Time % 50f == 49f && Main.myPlayer == projectile.owner && potentialTarget != null)
            {
                Vector2 shootVelocity = projectile.SafeDirectionTo(potentialTarget.Center) * 10f;
                Projectile.NewProjectile(projectile.Center, shootVelocity, ModContent.ProjectileType<MortalityBeam>(), projectile.damage / 3, projectile.knockBack, projectile.owner);
            }
            projectile.Center = player.Center + OffsetAngle.ToRotationVector2() * (150f + (float)Math.Sin(Time * 0.08f) * 15f);
            projectile.rotation += MathHelper.ToRadians(7f);
            OffsetAngle += MathHelper.ToRadians(6f);
        }

        public void SetProjectileDamage()
        {
            if (projectile.localAI[0] == 0f)
            {
                projectile.Calamity().spawnedPlayerMinionDamageValue = Owner.MinionDamage();
                projectile.Calamity().spawnedPlayerMinionProjectileDamageValue = projectile.damage;
                for (int i = 0; i < 36; i++)
                {
                    Dust dust = Dust.NewDustPerfect(projectile.Center, 261);
                    dust.noGravity = true;
                    dust.color = Main.hslToRgb(Main.rand.NextFloat(), 1f, 0.5f);
                    dust.velocity = Main.rand.NextVector2Unit() * Main.rand.NextFloat(2f, 7f);
                }
                projectile.localAI[0] += 1f;
            }
            if (Owner.MinionDamage() != projectile.Calamity().spawnedPlayerMinionDamageValue)
            {
                int trueDamage = (int)(projectile.Calamity().spawnedPlayerMinionProjectileDamageValue /
                    projectile.Calamity().spawnedPlayerMinionDamageValue *
                    Owner.MinionDamage());
                projectile.damage = trueDamage;
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D petalTexture = Main.projectileTexture[projectile.type];
            Texture2D coreTexture = ModContent.GetTexture("CalamityMod/Projectiles/Summon/FlowersOfMortalityCore");
            Color drawColor = Main.hslToRgb(Hue, 0.95f, 0.5f) * 2.3f;

            Vector2 drawPosition = projectile.Center - Main.screenPosition;
            spriteBatch.Draw(petalTexture, drawPosition, null, projectile.GetAlpha(drawColor), projectile.rotation, petalTexture.Size() * 0.5f, projectile.scale, SpriteEffects.None, 0f);
            spriteBatch.Draw(coreTexture, drawPosition, null, projectile.GetAlpha(lightColor), 0f, coreTexture.Size() * 0.5f, projectile.scale, SpriteEffects.None, 0f);

            return false;
        }
    }
}
