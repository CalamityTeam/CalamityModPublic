using CalamityMod.Buffs.StatBuffs;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Rogue
{
    public class CorpusAvertorStealth : ModProjectile
    {
        public override string Texture => "CalamityMod/Items/Weapons/Rogue/CorpusAvertor";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Corpus Avertor");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 10;
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            projectile.width = 24;
            projectile.height = 24;
            projectile.friendly = true;
            projectile.penetrate = 1;
            projectile.timeLeft = 300;
            projectile.Calamity().rogue = true;
        }

        public override void AI()
        {
            projectile.rotation += (Math.Abs(projectile.velocity.X) + Math.Abs(projectile.velocity.Y)) * 0.02f;

            if (projectile.ai[0] < 120f)
                projectile.ai[0] += 1f;

            projectile.velocity.X *= 1.01f;
            projectile.velocity.Y *= 1.01f;

            int scale = (int)((projectile.ai[0] - 60f) * 4.25f);
            int dust = Dust.NewDust(projectile.position, projectile.width, projectile.height, 5, 0f, 0f, 100, new Color(scale, 0, 0, 50), 2f);
            Main.dust[dust].velocity *= 0f;
            Main.dust[dust].noGravity = true;
        }

        public override Color? GetAlpha(Color lightColor) => new Color(Main.DiscoR, Main.DiscoG, Main.DiscoB, projectile.alpha);

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(projectile, ProjectileID.Sets.TrailingMode[projectile.type], lightColor, 1);
            return false;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            OnHitEffects(damage);
        }

        public override void OnHitPvp(Player target, int damage, bool crit)
        {
            OnHitEffects(damage);
        }

        private void OnHitEffects(int damage)
        {
            Player player = Main.player[projectile.owner];
            if (Main.rand.NextBool(7))
            {
                int lifeLossAmt = (int)Math.Ceiling(player.statLife * 0.5);
                player.statLife -= lifeLossAmt;
                if (Main.myPlayer == player.whoAmI)
                    player.HealEffect(-lifeLossAmt, true);
                if (player.statLife <= 0)
                    player.KillMe(PlayerDeathReason.ByCustomReason(player.name + " became the blood god's sacrifice."), 1000.0, 0, false);
            }
            else if (Main.LocalPlayer.team == player.team && player.team != 0)
            {
                Main.LocalPlayer.AddBuff(ModContent.BuffType<AvertorBonus>(), CalamityUtils.SecondsToFrames(20f), true);
                player.AddBuff(ModContent.BuffType<AvertorBonus>(), CalamityUtils.SecondsToFrames(20f), true);

                float heal = damage * 0.025f;
                if ((int)heal == 0)
                    return;

                if (Main.player[Main.myPlayer].lifeSteal <= 0f)
                    return;

                if (heal > CalamityMod.lifeStealCap)
                    heal = CalamityMod.lifeStealCap;

                CalamityGlobalProjectile.SpawnLifeStealProjectile(projectile, player, heal, ProjectileID.VampireHeal, 1200f, 3f);
            }
        }
    }
}
