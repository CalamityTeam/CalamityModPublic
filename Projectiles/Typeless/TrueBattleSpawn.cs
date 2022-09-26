using CalamityMod.NPCs.Calamitas;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Typeless
{
    public class TrueBattleSpawn : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Spawn");
        }

        public override void SetDefaults()
        {
            Projectile.width = 6;
            Projectile.height = 6;
            Projectile.aiStyle = ProjAIStyleID.Arrow;
            Projectile.scale = 1f;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 20;
            Projectile.tileCollide = false;
            AIType = ProjectileID.Bullet;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            Projectile.ai[1]++;

            if (Projectile.ai[1] >= 0)
            {
                NPC.SpawnOnPlayer(player.whoAmI, ModContent.NPCType<CalamitasClone>());
                Projectile.ai[1] = -30;
            }
        }
    }
}
