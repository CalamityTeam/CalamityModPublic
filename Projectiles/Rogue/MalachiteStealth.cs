using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.Projectiles.Rogue
{
    public class MalachiteStealth : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Rogue";
        private const int lifeSpan = 300;
        public override string Texture => "CalamityMod/Items/Weapons/Rogue/Malachite";

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 10;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.width = 12;
            Projectile.height = 12;
            Projectile.alpha = 255;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1;
            Projectile.alpha = 255;
            Projectile.DamageType = RogueDamageClass.Instance;
            Projectile.tileCollide = false;
            Projectile.timeLeft = lifeSpan;
            Projectile.extraUpdates = 2;
        }

        public override void AI()
        {
            if (Projectile.ai[0] == 0f)
            {
                Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
                CalamityUtils.HomeInOnNPC(Projectile, true, 300f, 10f, 25f);
                Projectile.localAI[1] += 1f;
                if (Projectile.localAI[1] > 4f)
                {
                    for (int k = 0; k < 3; k++)
                    {
                        int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 107, 0f, 0f, 100, new Color(Main.DiscoR, 203, 103), 0.75f);
                        Main.dust[dust].noGravity = true;
                        Main.dust[dust].velocity *= 0f;
                    }
                }
            }
            else
            {
                int id = (int)Projectile.ai[1];
                if (id.WithinBounds(Main.maxNPCs) && Main.npc[id].active && !Main.npc[id].dontTakeDamage)
                {
                    Projectile.Center = Main.npc[id].Center - Projectile.velocity * 2f;
                    Projectile.gfxOffY = Main.npc[id].gfxOffY;
                }
                else
                {
                    Projectile.Kill();
                }
            }

            Projectile.alpha -= 3;
            if (Projectile.alpha < 30)
            {
                Projectile.alpha = 30;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], lightColor, 1);
            return false;
        }

        public override Color? GetAlpha(Color lightColor) => new Color(Main.DiscoR, 203, 103, Projectile.alpha);

        public override void OnKill(int timeLeft)
        {
            Projectile.ai[0] = 2f;
            Projectile.ExpandHitboxBy(112);
            SoundEngine.PlaySound(SoundID.Item14, Projectile.position);
            for (int i = 0; i < 7; i++)
            {
                int dusty = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 107, 0f, 0f, 100, new Color(Main.DiscoR, 203, 103), 1.2f);
                Main.dust[dusty].velocity *= 3f;
                if (Main.rand.NextBool())
                {
                    Main.dust[dusty].scale = 0.5f;
                    Main.dust[dusty].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
                }
            }
            for (int j = 0; j < 15; j++)
            {
                int green = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 107, 0f, 0f, 100, new Color(Main.DiscoR, 203, 103), 1.7f);
                Main.dust[green].noGravity = true;
                Main.dust[green].velocity *= 5f;
                green = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 107, 0f, 0f, 100, new Color(Main.DiscoR, 203, 103), 1f);
                Main.dust[green].velocity *= 2f;
            }
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
            Projectile.Damage();
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.immune[Projectile.owner] = 0;
            Projectile.extraUpdates = 0;
            Projectile.ai[0] = 1f;
            Projectile.ai[1] = target.whoAmI;
            Projectile.velocity = target.Center - Projectile.Center;
            Projectile.velocity *= 0.75f;
            Projectile.netUpdate = true;

            const int maxKunai = 3;
            int kunaiFound = 0;
            int oldestKunai = -1;
            int oldestKunaiTimeLeft = lifeSpan;
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                if (Main.projectile[i].active && Main.projectile[i].owner == Main.myPlayer && Main.projectile[i].type == Projectile.type && i != Projectile.whoAmI && Main.projectile[i].ai[1] == target.whoAmI)
                {
                    kunaiFound++;
                    if (Main.projectile[i].timeLeft < oldestKunaiTimeLeft)
                    {
                        oldestKunaiTimeLeft = Main.projectile[i].timeLeft;
                        oldestKunai = Main.projectile[i].whoAmI;
                    }
                    if (kunaiFound >= maxKunai)
                        break;
                }
            }
            if (kunaiFound >= maxKunai && oldestKunai >= 0)
            {
                Main.projectile[oldestKunai].Kill();
            }
        }

        public override bool? CanDamage() => Projectile.ai[0] != 1f ? null : false;
    }
}
