using CalamityMod.CalPlayer;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Pets
{
    public class LadShark : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Lad Shark");
            Main.projPet[projectile.type] = true;
        }

        public override void SetDefaults()
        {
            projectile.netImportant = true;
            projectile.width = 30;
            projectile.height = 30;
            projectile.friendly = true;
            projectile.penetrate = -1;
            projectile.timeLeft *= 5;
            projectile.aiStyle = 26;
            aiType = ProjectileID.BabySkeletronHead;
        }

        public override void AI()
        {
            Player player = Main.player[projectile.owner];
            CalamityPlayer modPlayer = player.Calamity();
            if (!player.active)
            {
                projectile.active = false;
                return;
            }
            if (player.dead)
            {
                modPlayer.ladShark = false;
            }
            if (modPlayer.ladShark)
            {
                projectile.timeLeft = 2;
            }
            projectile.rotation += (Math.Abs(projectile.velocity.X) + Math.Abs(projectile.velocity.Y)) * 0.01f * (float)projectile.direction;

            //occasionally burst in hearts
            if (Main.rand.NextBool(10000))
            {
                if (projectile.owner == Main.myPlayer)
                {
                    int heartCount = Main.rand.Next(20, 31);
                    for (int i = 0; i < heartCount; i++)
                    {
                        Vector2 velocity = new Vector2((float)Main.rand.Next(-10, 11), (float)Main.rand.Next(-10, 11));
                        velocity.Normalize();
                        velocity.X *= 0.66f;
                        int heart = Gore.NewGore(projectile.Center, velocity * Main.rand.NextFloat(3f, 5f) * 0.33f, 331, Main.rand.NextFloat(40f, 120f) * 0.01f);
                        Main.gore[heart].sticky = false;
                        Main.gore[heart].velocity *= 5f;
                    }

                    Main.PlaySound(SoundID.Zombie, (int)projectile.position.X, (int)projectile.position.Y, 15); //mouse squeak sound

                    float radius = 240f; // 15 blocks
                    for (int j = 0; j < Main.maxNPCs; j++)
                    {
                        NPC npc = Main.npc[j];
                        if (npc.active && !npc.dontTakeDamage && Vector2.Distance(projectile.Center, npc.Center) <= radius)
                        {
                            if (npc.Calamity().ladHearts <= 0)
                                npc.Calamity().ladHearts = CalamityUtils.SecondsToFrames(9f);
                        }
                    }
                    for (int k = 0; k < Main.maxPlayers; k++)
                    {
                        Player players = Main.player[k];
                        if (!players.dead && Vector2.Distance(projectile.Center, players.Center) <= radius)
                        {
                            if (players.Calamity().ladHearts <= 0)
                                players.Calamity().ladHearts = CalamityUtils.SecondsToFrames(9f);
                        }
                    }
                }
            }
        }
    }
}
