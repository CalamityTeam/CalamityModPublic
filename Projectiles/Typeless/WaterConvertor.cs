using CalamityMod.World;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Typeless
{
    public class WaterConvertor : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetDefaults()
        {
            Projectile.penetrate = -1;
            Projectile.width = 160;
            Projectile.height = 160;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 10;
        }

        public override void AI()
        {
            ConvertShit(Projectile);
        }

        public override void PostAI()
        {
            ConvertShit(Projectile);
        }

        public static void ConvertShit(Projectile projectile)
        {
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                int x = (int)(projectile.Center.X / 16f);
                int y = (int)(projectile.Center.Y / 16f);
                for (int i = x - 5; i <= x + 5; i++)
                {
                    for (int j = y - 5; j <= y + 5; j++)
                    {
                        if (projectile.ai[0] == 0f)
                        {
                            AstralBiome.ConvertFromAstral(i, j, ConvertType.Pure);
                        }
                        if (projectile.ai[0] == 1f)
                        {
                            AstralBiome.ConvertFromAstral(i, j, ConvertType.Corrupt);
                        }
                        if (projectile.ai[0] == 2f)
                        {
                            AstralBiome.ConvertFromAstral(i, j, ConvertType.Crimson);
                        }
                        if (projectile.ai[0] == 3f)
                        {
                            AstralBiome.ConvertFromAstral(i, j, ConvertType.Hallow);
                        }
                        if (projectile.ai[0] == 4f)
                        {
                            AstralBiome.ConvertToAstral(i, j);
                            NetMessage.SendTileSquare(-1, i, j, 1, 1);
                        }
                    }
                }
            }
        }

        public override bool? CanDamage() => false;
    }
}
