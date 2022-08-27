using CalamityMod.Buffs.Summon;
using CalamityMod.CalPlayer;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Summon
{
    public class BabyBloodCrawler : ModProjectile
    {
        public float dust = 0f;
        public int spiderCount = 0;
        public bool countedAlready = false;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Baby Blood Crawler");
            Main.projFrames[Projectile.type] = 11;
            ProjectileID.Sets.MinionSacrificable[Projectile.type] = true;
            ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 26;
            Projectile.height = 26;
            Projectile.netImportant = true;
            Projectile.friendly = true;
            Projectile.minionSlots = 1f;
            Projectile.aiStyle = ProjAIStyleID.Pet;
            Projectile.timeLeft = 18000;
            Projectile.penetrate = -1;
            Projectile.timeLeft *= 5;
            Projectile.minion = true;
            Projectile.tileCollide = false;
            AIType = ProjectileID.VenomSpider;
            Projectile.DamageType = DamageClass.Summon;
        }

        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
        {
            fallThrough = false;
            return true;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            CalamityPlayer modPlayer = player.Calamity();

            for (int j = 0; j < Main.maxProjectiles; j++)
            {
                Projectile proj = Main.projectile[j];
                // Short circuits to make the loop as fast as possible
                if (!proj.active || proj.owner != Projectile.owner || !proj.minion || proj.Calamity().lineColor != 0)
                    continue;
                if (proj.type == Projectile.type)
                {
                    spiderCount += (int)proj.minionSlots;
                    proj.Calamity().lineColor = 1;
                }
            }

            if (dust == 0f)
            {
                int num226 = 16;
                for (int num227 = 0; num227 < num226; num227++)
                {
                    Vector2 vector6 = Vector2.Normalize(Projectile.velocity) * new Vector2((float)Projectile.width / 2f, (float)Projectile.height) * 0.75f;
                    vector6 = vector6.RotatedBy((double)((float)(num227 - (num226 / 2 - 1)) * 6.28318548f / (float)num226), default) + Projectile.Center;
                    Vector2 vector7 = vector6 - Projectile.Center;
                    int num228 = Dust.NewDust(vector6 + vector7, 0, 0, 5, vector7.X * 1f, vector7.Y * 1f, 100, default, 1.1f);
                    Main.dust[num228].noGravity = true;
                    Main.dust[num228].noLight = true;
                    Main.dust[num228].velocity = vector7;
                }
                dust += 1f;
            }
            bool flag64 = Projectile.type == ModContent.ProjectileType<BabyBloodCrawler>();
            player.AddBuff(ModContent.BuffType<BabyBloodCrawlerBuff>(), 3600);
            if (flag64)
            {
                if (player.dead)
                {
                    modPlayer.scabRipper = false;
                }
                if (modPlayer.scabRipper)
                {
                    Projectile.timeLeft = 2;
                }
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity) => false;

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            //1 spider = 15 frames, 5 spiders, 10 frames
            int immuneTime = 16 - spiderCount;
            if (immuneTime < 5)
                immuneTime = 5; //cap to prevent potential insanity
            target.immune[Projectile.owner] = immuneTime;

            OnHitEffects(target.Center);
        }

        public override void OnHitPvp(Player target, int damage, bool crit)
        {
            OnHitEffects(target.Center);
        }

        private void OnHitEffects(Vector2 targetPos)
        {
            if (Main.rand.NextBool(2))
            {
                int projAmt = Main.rand.Next(1, 3);
                var source = Projectile.GetSource_FromThis();
                for (int n = 0; n < projAmt; n++)
                {
                    float damageFactor = Main.rand.NextFloat(0.7f, 1f);
                    Projectile rain = CalamityUtils.ProjectileRain(source, targetPos, 400f, 100f, 500f, 800f, 29f, ModContent.ProjectileType<BloodRain>(), (int)(Projectile.damage * damageFactor), Projectile.knockBack * Main.rand.NextFloat(0.7f, 1f), Projectile.owner);
                    rain.originalDamage = (int)(Projectile.originalDamage * damageFactor);
                }
            }
        }
    }
}
