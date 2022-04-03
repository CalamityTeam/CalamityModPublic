using CalamityMod.Projectiles.Typeless;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories
{
    public class LeviathanAmbergris : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Leviathan Ambergris");
            Tooltip.SetDefault("You leave behind poisonous seawater as you move\n" +
                               "30% increased movement speed, 10% increase to all damage and plus 10 defense while submerged in liquid\n" +
                               "If you are damaged while submerged in liquid you will gain a damaging aura for a short time\n" +
                               "Being outside of liquid increases all damage by 5% and increases damage reduction by 5%");
        }

        public override void SetDefaults()
        {
            Item.defense = 20;
            Item.width = 20;
            Item.height = 22;
            Item.value = CalamityGlobalItem.Rarity7BuyPrice;
            Item.accessory = true;
            Item.rare = ItemRarityID.Lime;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.ignoreWater = true;
            if (!player.lavaWet && !player.honeyWet)
            {
                if (!Collision.DrownCollision(player.position, player.width, player.height, player.gravDir))
                {
                    player.endurance += 0.05f;
                    player.allDamage += 0.05f;
                }
                else
                {
                    player.allDamage += 0.1f;
                    player.statDefense += 10;
                    player.moveSpeed += 0.3f;
                }
            }
            if ((double)player.velocity.X > 0 || (double)player.velocity.Y > 0 || (double)player.velocity.X < -0.1 || (double)player.velocity.Y < -0.1)
            {
                if (player.whoAmI == Main.myPlayer)
                {
                    Projectile.NewProjectile(player.Center.X, player.Center.Y, 0f, 0f, ModContent.ProjectileType<PoisonousSeawater>(), (int)(50 * player.AverageDamage()), 5f, player.whoAmI, 0f, 0f);
                }
            }
            int seaCounter = 0;
            Lighting.AddLight((int)(player.Center.X / 16f), (int)(player.Center.Y / 16f), 0f, 0.5f, 1.25f);
            int num = BuffID.Venom;
            float num2 = 200f;
            bool flag = seaCounter % 60 == 0;
            int num3 = (int)(15 * player.AverageDamage());
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
        }
    }
}
