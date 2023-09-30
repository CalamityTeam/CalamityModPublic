using CalamityMod.CalPlayer;
using Microsoft.Xna.Framework;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
namespace CalamityMod.Projectiles.Rogue
{
    // The file name is a specific request from the patron
    public class NiceCock : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Rogue";
        public bool homing = false;
        public int Timer = 0;
        private bool initialized = false;
        private Color[] colors = new Color[]
        {
            new Color(255, 0, 0, 50), //Red
            new Color(255, 128, 0, 50), //Orange
            new Color(255, 255, 0, 50), //Yellow
            new Color(128, 255, 0, 50), //Lime
            new Color(0, 255, 0, 50), //Green
            new Color(0, 255, 128, 50), //Turquoise
            new Color(0, 255, 255, 50), //Cyan
            new Color(0, 128, 255, 50), //Light Blue
            new Color(0, 0, 255, 50), //Blue
            new Color(128, 0, 255, 50), //Purple
            new Color(255, 0, 255, 50), //Fuschia
            new Color(255, 0, 128, 50) //Hot Pink
        };

        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 5;
        }

        public override void SetDefaults()
        {
            Projectile.width = 44;
            Projectile.height = 70;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.DamageType = RogueDamageClass.Instance;
            Projectile.timeLeft = 180;
            Projectile.alpha = 255;
        }

        public override void SendExtraAI(BinaryWriter writer) => writer.Write(homing);

        public override void ReceiveExtraAI(BinaryReader reader) => homing = reader.ReadBoolean();

        public override bool? CanHitNPC(NPC target)
        {
            // Do not deal damage before fully opaque
            if (Projectile.alpha >= 10)
                return false;
            int index = (int)Projectile.ai[1];
            if (Main.npc[index] is null || !Main.npc[index].active || Main.npc[index].life < 0)
                return false;
            // Do not deal damage to any NPC that isn't the specified target
            if (index != target.whoAmI)
                return false;
            return null;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            CalamityPlayer modPlayer = player.Calamity();
            // Fireballs disappear if you can't stealth strike
            if (!modPlayer.wearingRogueArmor || modPlayer.rogueStealthMax <= 0)
                Projectile.Kill();

            Projectile.alpha -= 5;

            if (!initialized)
            {
                // Pick a random frame to start on
                Projectile.frame = Main.rand.Next(Main.projFrames[Projectile.type]);
                initialized = true;
            }

            Projectile.frameCounter++;
            if (Projectile.frameCounter > 8)
            {
                Projectile.frame++;
                Projectile.frameCounter = 0;
            }
            if (Projectile.frame >= Main.projFrames[Projectile.type])
                Projectile.frame = 0;

            int index = (int)Projectile.ai[1];

            // If the target doesn't exist or is dead, spontaneously combust
            if (Main.npc[index] is null || !Main.npc[index].active || Main.npc[index].life < 0)
                Projectile.Kill();

            // Decrement the timer. This should last a second as stealth strikes will initialize this at 61.
            if (Timer > 1)
                Timer --;
            if (Timer == 1)
                homing = true;

            NPC target = Main.npc[index];

            if (homing)
            {
                // Home in on the target
                Vector2 moveDirection = Projectile.SafeDirectionTo(target.Center, Vector2.UnitY);
                Projectile.velocity = (Projectile.velocity * 20f + moveDirection * 15f) / 21f;
            }
            else
            {
                // Circle around the target counter-clockwise at 4 radians per frame
                float height = target.getRect().Height;
                float width = target.getRect().Width;
                float circleDist = MathHelper.Min((height > width ? height : width) * 3f, (Main.LogicCheckScreenWidth * Main.LogicCheckScreenHeight) / 2);
                if (circleDist > Main.LogicCheckScreenWidth / 3)
                    circleDist = Main.LogicCheckScreenWidth / 3;
                Projectile.Center = target.Center + Projectile.ai[0].ToRotationVector2() * circleDist;
                Projectile.ai[0] -= MathHelper.ToRadians(4f);
            }
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item74, Projectile.Center);
            Projectile.ExpandHitboxBy(50);

            // Create into some rainbow-colored dust when dead
            for (int d = 0; d < 5; d++)
            {
                int idx = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 267, 0f, 0f, 150, Main.rand.Next(colors), 2f);
                Main.dust[idx].velocity *= 3f;
                Main.dust[idx].noGravity = true;
                if (Main.rand.NextBool())
                {
                    Main.dust[idx].scale = 0.5f;
                    Main.dust[idx].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
                }
            }
            for (int d = 0; d < 8; d++)
            {
                int idx = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 267, 0f, 0f, 150, Main.rand.Next(colors), 3f);
                Main.dust[idx].noGravity = true;
                Main.dust[idx].velocity *= 5f;
                idx = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 267, 0f, 0f, 150, Main.rand.Next(colors), 2f);
                Main.dust[idx].velocity *= 2f;
                Main.dust[idx].noGravity = true;
            }
        }
    }
}
