using CalamityMod.CalPlayer;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using CalamityMod.Projectiles;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Terraria.ID;
using CalamityMod.Buffs;
namespace CalamityMod.Items.Accessories
{
    public class TheAmalgam : ModItem
    {
        public const int FireProjectiles = 2;
        public const float FireAngleSpread = 120;
        public int FireCountdown = 0;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("The Amalgam");
            Tooltip.SetDefault("15% increased damage\n" +
                               "Shade rains down when you are hit\n" +
                               "You will confuse nearby enemies when you are struck\n" +
                               "Drops brimstone fireballs from the sky occasionally\n" +
                               "Brimstone fire rains down while invincibility is active\n" +
                               "Temporary immunity to lava, greatly reduces lava burn damage, and 15% increased damage while in lava\n" +
                               "Summons a fungal clump to fight for you\n" +
                               "You leave behind poisonous seawater as you move\n" +
                               "75% increased movement speed, 10% increase to all damage, and plus 40 defense while submerged in liquid\n" +
                               "If you are damaged while submerged in liquid you will gain a damaging aura for a short time");
            Main.RegisterItemAnimation(item.type, new DrawAnimationVertical(9, 6));
        }

        public override void SetDefaults()
        {
            item.width = 34;
            item.height = 34;
            item.value = Item.buyPrice(0, 90, 0, 0);
            item.expert = true;
            item.rare = 9;
            item.accessory = true;
        }

        public override bool CanEquipAccessory(Player player, int slot)
        {
            CalamityPlayer modPlayer = player.Calamity();
            if (modPlayer.fungalClump)
            {
                return false;
            }
            return true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.aBrain = true;
            modPlayer.fungalClump = true;
            if (player.whoAmI == Main.myPlayer)
            {
                if (player.FindBuffIndex(ModContent.BuffType<FungalClumpBuff>()) == -1)
                {
                    player.AddBuff(ModContent.BuffType<FungalClumpBuff>(), 3600, true);
                }
                if (player.ownedProjectileCounts[ModContent.ProjectileType<FungalClumpMinion>()] < 1)
                {
                    Projectile.NewProjectile(player.Center.X, player.Center.Y, 0f, -1f, ModContent.ProjectileType<FungalClumpMinion>(), (int)(1000f * player.minionDamage), 1f, Main.myPlayer, 0f, 0f);
                }
            }
            player.allDamage += 0.15f;
            player.ignoreWater = true;
            player.lavaRose = true;
            player.lavaMax += 240;
            if (player.lavaWet)
            {
                player.allDamage += 0.15f;
            }
            if (Collision.DrownCollision(player.position, player.width, player.height, player.gravDir))
            {
                player.allDamage += 0.1f;
                player.statDefense += 40;
                player.moveSpeed += 0.75f;
            }
            if ((double)player.velocity.X > 0 || (double)player.velocity.Y > 0 || (double)player.velocity.X < -0.1 || (double)player.velocity.Y < -0.1)
            {
                if (player.whoAmI == Main.myPlayer)
                {
                    Projectile.NewProjectile(player.Center.X, player.Center.Y, 0f, 0f, ModContent.ProjectileType<PoisonousSeawater>(), 2000, 0f, player.whoAmI, 0f, 0f);
                }
            }
            if (player.immune)
            {
                if (Main.rand.NextBool(20))
                {
                    if (player.whoAmI == Main.myPlayer)
                    {
                        for (int l = 0; l < 1; l++)
                        {
                            float x = player.position.X + (float)Main.rand.Next(-400, 400);
                            float y = player.position.Y - (float)Main.rand.Next(500, 800);
                            Vector2 vector = new Vector2(x, y);
                            float num15 = player.position.X + (float)(player.width / 2) - vector.X;
                            float num16 = player.position.Y + (float)(player.height / 2) - vector.Y;
                            num15 += (float)Main.rand.Next(-100, 101);
                            int num17 = 22;
                            float num18 = (float)Math.Sqrt((double)(num15 * num15 + num16 * num16));
                            num18 = (float)num17 / num18;
                            num15 *= num18;
                            num16 *= num18;
                            int type = Main.rand.NextBool(2) ? ModContent.ProjectileType<AuraRain>() : ModContent.ProjectileType<StandingFire>();
                            int num19 = Projectile.NewProjectile(x, y, num15, num16, type, 2000, 1f, player.whoAmI, 0f, 0f);
                            Main.projectile[num19].tileCollide = false;
                        }
                    }
                }
            }
            int seaCounter = 0;
            Lighting.AddLight((int)(player.Center.X / 16f), (int)(player.Center.Y / 16f), 0f, 0.5f, 1.25f);
            int num = BuffID.Venom;
            float num2 = 300f;
            bool flag = seaCounter % 60 == 0;
            int num3 = 320;
            int random = Main.rand.Next(5);
            if (player.whoAmI == Main.myPlayer)
            {
                if (random == 0 && player.immune && Collision.DrownCollision(player.position, player.width, player.height, player.gravDir))
                {
                    for (int l = 0; l < 200; l++)
                    {
                        NPC nPC = Main.npc[l];
                        if (nPC.active && !nPC.friendly && nPC.damage > 0 && !nPC.dontTakeDamage && !nPC.buffImmune[num] && Vector2.Distance(player.Center, nPC.Center) <= num2)
                        {
                            if (nPC.FindBuffIndex(num) == -1)
                            {
                                nPC.AddBuff(num, 300, false);
                            }
                            if (flag)
                            {
                                nPC.StrikeNPC(num3, 0f, 0, false, false, false);
                                if (Main.netMode != NetmodeID.SinglePlayer)
                                {
                                    NetMessage.SendData(28, -1, -1, null, l, (float)num3, 0f, 0f, 0, 0, 0);
                                }
                            }
                        }
                    }
                }
            }
            seaCounter++;
            if (seaCounter >= 180)
            {
            }
            if (FireCountdown == 0)
            {
                FireCountdown = 600;
            }
            if (FireCountdown > 0)
            {
                FireCountdown--;
                if (FireCountdown == 0)
                {
                    if (player.whoAmI == Main.myPlayer)
                    {
                        int speed2 = 25;
                        float spawnX = Main.rand.Next(1000) - 500 + player.Center.X;
                        float spawnY = -1000 + player.Center.Y;
                        Vector2 baseSpawn = new Vector2(spawnX, spawnY);
                        Vector2 baseVelocity = player.Center - baseSpawn;
                        baseVelocity.Normalize();
                        baseVelocity *= speed2;
                        for (int i = 0; i < FireProjectiles; i++)
                        {
                            Vector2 spawn = baseSpawn;
                            spawn.X = spawn.X + i * 30 - (FireProjectiles * 15);
                            Vector2 velocity = baseVelocity.RotatedBy(MathHelper.ToRadians(-FireAngleSpread / 2 + (FireAngleSpread * i / (float)FireProjectiles)));
                            velocity.X = velocity.X + 3 * Main.rand.NextFloat() - 1.5f;
                            int projectile = Projectile.NewProjectile(spawn.X, spawn.Y, velocity.X, velocity.Y, ModContent.ProjectileType<BrimstoneHellfireballFriendly2>(), 2000, 5f, Main.myPlayer, 0f, 0f);
                            Main.projectile[projectile].tileCollide = false;
                            Main.projectile[projectile].timeLeft = 50;
                        }
                    }
                }
            }
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(null, "AmalgamatedBrain");
            recipe.AddIngredient(null, "VoidofExtinction");
            recipe.AddIngredient(null, "FungalClump");
            recipe.AddIngredient(null, "LeviathanAmbergris");
            recipe.AddIngredient(null, "CosmiliteBar", 5);
            recipe.AddIngredient(null, "Phantoplasm", 5);
            recipe.AddTile(null, "DraedonsForge");
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
