using Terraria;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Melee
{
    public class WhiteBoltAura : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Melee";
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetDefaults()
        {
            Projectile.width = 100;
            Projectile.height = 2;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 600;
        }

        public override void AI()
        {
            Projectile.ai[1] += 1f;
            if (Projectile.ai[1] >= 7200f)
            {
                Projectile.alpha += 5;
                if (Projectile.alpha > 255)
                {
                    Projectile.alpha = 255;
                    Projectile.Kill();
                }
            }
            else
            {
                Projectile.ai[0] += 1f;
                if (Projectile.ai[0] > 12f)
                {
                    Projectile.ai[0] = 0f;
                    if (Projectile.owner == Main.myPlayer)
                    {
                        int randOrbXOffset = (int)(Projectile.position.X + 14f + (float)Main.rand.Next(Projectile.width - 28));
                        int randOrbYOffset = (int)(Projectile.position.Y + (float)Projectile.height + 4f);
                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), (float)randOrbXOffset, (float)randOrbYOffset, 0f, 5f, ModContent.ProjectileType<Whiterain>(), Projectile.damage, 0f, Projectile.owner, 0f, 0f);
                    }
                }
            }
            Projectile.localAI[0] += 1f;
            if (Projectile.localAI[0] >= 10f)
            {
                Projectile.localAI[0] = 0f;
                int projTimer = 0;
                int incTracker = 0;
                float aiTracker = 0f;
                int theProjectile = Projectile.type;
                for (int j = 0; j < 1000; j++)
                {
                    if (Main.projectile[j].active && Main.projectile[j].owner == Projectile.owner && Main.projectile[j].type == theProjectile && Main.projectile[j].ai[1] < 3600f)
                    {
                        projTimer++;
                        if (Main.projectile[j].ai[1] > aiTracker)
                        {
                            incTracker = j;
                            aiTracker = Main.projectile[j].ai[1];
                        }
                    }
                }
                if (projTimer > 1)
                {
                    Main.projectile[incTracker].netUpdate = true;
                    Main.projectile[incTracker].ai[1] = 36000f;
                }
            }
        }
    }
}
