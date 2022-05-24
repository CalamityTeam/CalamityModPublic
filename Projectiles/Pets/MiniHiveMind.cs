using CalamityMod.Buffs.Pets;
using CalamityMod.CalPlayer;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
namespace CalamityMod.Projectiles.Pets
{
    public class MiniHiveMind : ModProjectile
    {
        private int reelBackCooldown = 0;
        private int charging = 0;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Mini Hive Mind");
            Main.projFrames[Projectile.type] = 16;
            Main.projPet[Projectile.type] = true;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.netImportant = true;
            Projectile.width = 38;
            Projectile.height = 44;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft *= 5;
            Projectile.tileCollide = false;
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

            //Delete the projectile if the player doesnt have the buff
            if (!player.HasBuff(ModContent.BuffType<HiveMindPet>()))
            {
                Projectile.Kill();
            }

            if (player.dead)
            {
                modPlayer.hiveMindPet = false;
            }
            if (modPlayer.hiveMindPet)
            {
                Projectile.timeLeft = 2;
            }

            if (charging <= 0)
                Projectile.FloatingPetAI(true, 0.05f);

            Vector2 playerVec = player.Center - Projectile.Center;
            float playerDist = playerVec.Length();
            if (reelBackCooldown > 0)
                reelBackCooldown--;
            if (charging > 0)
                charging--;
            if (reelBackCooldown <= 0 && Main.rand.NextBool(500) && playerDist < 100f && charging <= 0)
            {
                if (Main.myPlayer == Projectile.owner)
                {
                    //Do a backwards charge away from the player
                    reelBackCooldown = 300;
                    playerVec.Normalize();
                    Projectile.velocity = playerVec * 8f;
                    charging = 50;
                    SoundEngine.PlaySound(SoundID.ForceRoar, Projectile.Center, 0.5f, 0f);
                    Projectile.netUpdate = true;
                }
            }
            if (charging < 22 && charging > 0)
                Projectile.alpha += 12;
            if (charging == 1)
            {
                float xOffset = Main.rand.NextFloat(400f, 600f) * (Main.rand.NextBool() ? -1f : 1f);
                float yOffset = Main.rand.NextFloat(400f, 600f) * (Main.rand.NextBool() ? -1f : 1f);
                Vector2 teleportPos = new Vector2(player.Center.X + xOffset, player.Center.Y + yOffset);
                Projectile.Center = teleportPos;
                Projectile.alpha = 255;
                Projectile.netUpdate = true;
            }
            if (Projectile.alpha > 0 && charging <= 0)
                Projectile.alpha -= 12;
            if (Projectile.alpha < 0)
                Projectile.alpha = 0;
            if (Projectile.alpha > 255)
                Projectile.alpha = 255;
            if (charging > 0)
            {
                Projectile.rotation.AngleTowards(0f, 0.1f);
            }

            //Animation
            if (Projectile.frameCounter++ % 6 == 0)
            {
                Projectile.frame++;
            }
            if (Projectile.frame >= Main.projFrames[Projectile.type])
            {
                Projectile.frame = 0;
            }
        }
    }
}
