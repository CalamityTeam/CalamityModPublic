using CalamityMod.CalPlayer;
using CalamityMod.NPCs.Other;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.Projectiles.Pets
{
    public class LordePet : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Pets";
        public override string Texture => "CalamityMod/NPCs/Other/THELORDE";
        public override void SetStaticDefaults()
        {
            Main.projPet[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.netImportant = true;
            Projectile.width = Projectile.height = 67;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft *= 5;
            Projectile.aiStyle = ProjAIStyleID.Pet;
            AIType = ProjectileID.BabySkeletronHead;
            Projectile.scale = 0.3f;
            DrawOriginOffsetX -= 150;
            DrawOriginOffsetY -= 150;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            CalamityPlayer modPlayer = player.Calamity();
            if (!player.active)
            {
                Projectile.active = false;
                return;
            }
            if (player.dead)
            {
                modPlayer.lordePet = false;
            }
            if (modPlayer.lordePet)
            {
                Projectile.timeLeft = 2;
            }
            Lighting.AddLight(Projectile.Center, Main.DiscoColor.ToVector3() * 2);
            if (Main.rand.NextBool(1200))
            {
                SoundEngine.PlaySound(THELORDE.DeathSound with { PitchVariance = 2 }, Projectile.Center);
            }
            //Projectile.rotation += (Math.Abs(Projectile.velocity.X) + Math.Abs(Projectile.velocity.Y)) * 0.01f * (float)Projectile.direction;
        }
    }
}
