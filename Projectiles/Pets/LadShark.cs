using CalamityMod.CalPlayer;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.Projectiles.Pets
{
    public class LadShark : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Lad Shark");
            Main.projPet[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.netImportant = true;
            Projectile.width = 30;
            Projectile.height = 30;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft *= 5;
            Projectile.aiStyle = ProjAIStyleID.Pet;
            AIType = ProjectileID.BabySkeletronHead;
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
                modPlayer.ladShark = false;
            }
            if (modPlayer.ladShark)
            {
                Projectile.timeLeft = 2;
            }
            Projectile.rotation += (Math.Abs(Projectile.velocity.X) + Math.Abs(Projectile.velocity.Y)) * 0.01f * (float)Projectile.direction;

            //occasionally burst in hearts
            if (Main.rand.NextBool(10000))
            {
                if (Projectile.owner == Main.myPlayer)
                {
                    if (Main.netMode != NetmodeID.Server)
                    {
                        int heartCount = Main.rand.Next(20, 31);
                        for (int i = 0; i < heartCount; i++)
                        {
                            Vector2 velocity = new Vector2((float)Main.rand.Next(-10, 11), (float)Main.rand.Next(-10, 11));
                            velocity.Normalize();
                            velocity.X *= 0.66f;
                            int heart = Gore.NewGore(Projectile.GetSource_FromAI(), Projectile.Center, velocity * Main.rand.NextFloat(3f, 5f) * 0.33f, 331, Main.rand.NextFloat(40f, 120f) * 0.01f);
                            Main.gore[heart].sticky = false;
                            Main.gore[heart].velocity *= 5f;
                        }
                    }

                    SoundEngine.PlaySound(SoundID.Zombie15, Projectile.position); //mouse squeak sound

                    float radius = 240f; // 15 blocks
                    for (int j = 0; j < Main.maxNPCs; j++)
                    {
                        NPC npc = Main.npc[j];
                        if (npc.active && !npc.dontTakeDamage && Vector2.Distance(Projectile.Center, npc.Center) <= radius)
                        {
                            if (npc.Calamity().ladHearts <= 0)
                                npc.Calamity().ladHearts = CalamityUtils.SecondsToFrames(9f);
                        }
                    }
                    for (int k = 0; k < Main.maxPlayers; k++)
                    {
                        Player players = Main.player[k];
                        if (!players.dead && Vector2.Distance(Projectile.Center, players.Center) <= radius)
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
