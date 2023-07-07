using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Summon
{
    public class Hive : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Summon";
        public override string Texture => "CalamityMod/NPCs/Astral/HiveEnemy";

        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 6;
            ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 38;
            Projectile.height = 62;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
            Projectile.sentry = true;
            Projectile.timeLeft = Projectile.SentryLifeTime;
            Projectile.penetrate = -1;
            Projectile.DamageType = DamageClass.Summon;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            Projectile.frameCounter++;
            if (Projectile.frameCounter > 5)
            {
                Projectile.frame++;
                Projectile.frameCounter = 0;
            }
            if (Projectile.frame > 5)
            {
                Projectile.frame = 0;
            }
            Projectile.velocity.Y += 0.5f;

            if (Projectile.velocity.Y > 10f)
            {
                Projectile.velocity.Y = 10f;
            }

            Projectile.StickToTiles(false, false);

            int target = 0;
            float num633 = 800f;
            Vector2 vector46 = Projectile.position;
            bool flag25 = false;
            if (Main.player[Projectile.owner].HasMinionAttackTargetNPC)
            {
                NPC npc = Main.npc[Main.player[Projectile.owner].MinionAttackTargetNPC];
                if (npc.CanBeChasedBy(Projectile, false))
                {
                    float num646 = Vector2.Distance(npc.Center, Projectile.Center);
                    if (!flag25 && num646 < num633)
                    {
                        vector46 = npc.Center;
                        flag25 = true;
                        target = npc.whoAmI;
                    }
                }
            }
            if (!flag25)
            {
                for (int num645 = 0; num645 < Main.maxNPCs; num645++)
                {
                    NPC nPC2 = Main.npc[num645];
                    if (nPC2.CanBeChasedBy(Projectile, false))
                    {
                        float num646 = Vector2.Distance(nPC2.Center, Projectile.Center);
                        if (!flag25 && num646 < num633)
                        {
                            num633 = num646;
                            vector46 = nPC2.Center;
                            flag25 = true;
                            target = num645;
                        }
                    }
                }
            }
            if (Projectile.owner == Main.myPlayer && flag25)
            {
                if (Projectile.ai[0] != 0f)
                {
                    Projectile.ai[0] -= 1f;
                    return;
                }
                Projectile.ai[1] += 1f;
                if ((Projectile.ai[1] % 30f) == 0f)
                {
                    float velocityX = Main.rand.NextFloat(-0.4f, 0.4f);
                    float velocityY = Main.rand.NextFloat(-0.3f, -0.5f);
                    int p = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center.X, Projectile.Center.Y, velocityX, velocityY, ModContent.ProjectileType<Hiveling>(), Projectile.damage, Projectile.knockBack, Projectile.owner, (float)target, 0f);
                    if (Main.projectile.IndexInRange(p))
                        Main.projectile[p].originalDamage = Projectile.originalDamage;
                }
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity) => false;

        public override bool? CanDamage() => false;
    }
}
