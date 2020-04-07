using CalamityMod.Buffs.Summon;
using CalamityMod.CalPlayer;
using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Magic;
using CalamityMod.Projectiles.Summon;
using CalamityMod.Projectiles.Typeless;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

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
                               "If you are damaged while submerged in liquid you will gain a damaging aura for a short time\n" +
							   "Provides heat protection in Death Mode");
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
                    Projectile.NewProjectile(player.Center.X, player.Center.Y, 0f, -1f, ModContent.ProjectileType<FungalClumpMinion>(), (int)(1000f * player.MinionDamage()), 1f, Main.myPlayer, 0f, 0f);
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
                    Projectile.NewProjectile(player.Center.X, player.Center.Y, 0f, 0f, ModContent.ProjectileType<PoisonousSeawater>(), (int)(2000 * player.AverageDamage()), 0f, player.whoAmI, 0f, 0f);
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
                            int num19 = Projectile.NewProjectile(x, y, num15, num16, type, (int)(2000 * player.AverageDamage()), 1f, player.whoAmI, 0f, 0f);
                            Main.projectile[num19].tileCollide = false;
							Main.projectile[num19].usesLocalNPCImmunity = true;
							Main.projectile[num19].localNPCHitCooldown = 10;
							Main.projectile[num19].usesIDStaticNPCImmunity = false;
							Main.projectile[num19].Calamity().forceTypeless = true;
                        }
                    }
                }
            }
            int seaCounter = 0;
            Lighting.AddLight((int)(player.Center.X / 16f), (int)(player.Center.Y / 16f), 0f, 0.5f, 1.25f);
            int num = BuffID.Venom;
            float num2 = 300f;
            bool flag = seaCounter % 60 == 0;
            int num3 = (int)(320 * player.AverageDamage());
            int random = Main.rand.Next(5);
            if (player.whoAmI == Main.myPlayer)
            {
                if (random == 0 && player.immune && Collision.DrownCollision(player.position, player.width, player.height, player.gravDir))
                {
                    for (int l = 0; l < Main.maxNPCs; l++)
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
                                if (player.whoAmI == Main.myPlayer)
                                {
                                    Projectile p = Projectile.NewProjectileDirect(nPC.Center, Vector2.Zero, ModContent.ProjectileType<DirectStrike>(), num3, 0f, player.whoAmI, l);
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
                            Projectile.NewProjectile(spawn.X, spawn.Y, velocity.X, velocity.Y, ModContent.ProjectileType<BrimstoneHellfireballFriendly2>(), (int)(2000 * player.AverageDamage()), 5f, Main.myPlayer, 0f, 0f);
                        }
                    }
                }
            }
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<AmalgamatedBrain>());
            recipe.AddIngredient(ModContent.ItemType<VoidofExtinction>());
            recipe.AddIngredient(ModContent.ItemType<FungalClump>());
            recipe.AddIngredient(ModContent.ItemType<LeviathanAmbergris>());
            recipe.AddIngredient(ModContent.ItemType<CosmiliteBar>(), 5);
            recipe.AddIngredient(ModContent.ItemType<Phantoplasm>(), 5);
            recipe.AddTile(ModContent.TileType<DraedonsForge>());
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
