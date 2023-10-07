using CalamityMod.Buffs.DamageOverTime;
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
    public class FlowersOfMortalityPetal : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Summon";
        public Player Owner => Main.player[Projectile.owner];
        public ref float OffsetAngle => ref Projectile.ai[0];
        public ref float Time => ref Projectile.ai[1];
        public float Hue => OffsetAngle % MathHelper.TwoPi / MathHelper.TwoPi % 1f;
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 8;
            ProjectileID.Sets.MinionSacrificable[Projectile.type] = true;
            ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 30;
            Projectile.netImportant = true;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.minionSlots = 0.6f;
            Projectile.timeLeft = 90000;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.minion = true;
            Projectile.DamageType = DamageClass.Summon;
        }

        public override void AI()
        {
            bool isCorrectMinion = Projectile.type == ModContent.ProjectileType<FlowersOfMortalityPetal>();
            Player player = Main.player[Projectile.owner];
            CalamityPlayer modPlayer = player.Calamity();
            player.AddBuff(ModContent.BuffType<FlowersOfMortalityBuff>(), 3600);
            if (isCorrectMinion)
            {
                if (player.dead)
                    modPlayer.flowersOfMortality = false;
                if (modPlayer.flowersOfMortality)
                    Projectile.timeLeft = 2;
            }

            SetProjectileDamage();

            Time++;
            NPC potentialTarget = Projectile.Center.MinionHoming(1050f, Owner);
            if (Time % 50f == 49f && Main.myPlayer == Projectile.owner && potentialTarget != null)
            {
                Vector2 shootVelocity = Projectile.SafeDirectionTo(potentialTarget.Center) * 10f;
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, shootVelocity, ModContent.ProjectileType<MortalityBeam>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
            }
            Projectile.Center = player.Center + OffsetAngle.ToRotationVector2() * (150f + (float)Math.Sin(Time * 0.08f) * 15f);
            Projectile.rotation += MathHelper.ToRadians(7f);
            OffsetAngle += MathHelper.ToRadians(6f);
        }

        public void SetProjectileDamage()
        {
            if (Projectile.localAI[0] == 0f)
            {
                for (int i = 0; i < 36; i++)
                {
                    Dust dust = Dust.NewDustPerfect(Projectile.Center, 261);
                    dust.noGravity = true;
                    dust.color = Main.hslToRgb(Main.rand.NextFloat(), 1f, 0.5f);
                    dust.velocity = Main.rand.NextVector2Unit() * Main.rand.NextFloat(2f, 7f);
                }
                Projectile.localAI[0] += 1f;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D petalTexture = ModContent.Request<Texture2D>(Texture).Value;
            Texture2D coreTexture = ModContent.Request<Texture2D>("CalamityMod/Projectiles/Summon/FlowersOfMortalityCore").Value;
            Color drawColor = Main.hslToRgb(Hue, 0.95f, 0.5f) * 2.3f;

            Vector2 drawPosition = Projectile.Center - Main.screenPosition;
            Main.EntitySpriteDraw(petalTexture, drawPosition, null, Projectile.GetAlpha(drawColor), Projectile.rotation, petalTexture.Size() * 0.5f, Projectile.scale, SpriteEffects.None, 0);
            Main.EntitySpriteDraw(coreTexture, drawPosition, null, Projectile.GetAlpha(lightColor), 0f, coreTexture.Size() * 0.5f, Projectile.scale, SpriteEffects.None, 0);

            return false;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<ElementalMix>(), 30);
        }
        public override bool? CanDamage() => false;
    }
}
