using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.CalPlayer;
using CalamityMod.Projectiles.Typeless;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories
{
    public class PlagueHive : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Plague Hive");
            Tooltip.SetDefault("The power of your bees and wasps will rival the Moon Lord himself\n" +
				"Summons a damaging plague aura around the player to destroy nearby enemies\n" +
                "Releases bees when damaged\n" +
                "Friendly bees inflict the plague\n" +
                "All of your attacks inflict the plague\n" +
				"Reduces the damage caused to you by the plague\n" +
                "Projectiles spawn plague seekers on enemy hits");
        }

        public override void SetDefaults()
        {
            item.width = 30;
            item.height = 38;
            item.value = CalamityGlobalItem.Rarity9BuyPrice;
            item.expert = true;
            item.rare = 9;
            item.accessory = true;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<ToxicHeart>());
            recipe.AddIngredient(ModContent.ItemType<AlchemicalFlask>());
            recipe.AddIngredient(ItemID.HiveBackpack);
            recipe.AddIngredient(ItemID.HoneyComb);
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();
            player.bee = true;
            modPlayer.uberBees = true;
            player.strongBees = true;
            modPlayer.alchFlask = true;
            modPlayer.reducedPlagueDmg = true;
            int plagueCounter = 0;
            Lighting.AddLight((int)(player.Center.X / 16f), (int)(player.Center.Y / 16f), 0.1f, 2f, 0.2f);
            int num = ModContent.BuffType<Plague>();
            float num2 = 300f;
            bool flag = plagueCounter % 60 == 0;
            int num3 = (int)(60 * player.AverageDamage());
            int random = Main.rand.Next(10);
            if (player.whoAmI == Main.myPlayer)
            {
                if (random == 0)
                {
                    for (int l = 0; l < 200; l++)
                    {
                        NPC nPC = Main.npc[l];
                        if (nPC.active && !nPC.friendly && nPC.damage > 0 && !nPC.dontTakeDamage && !nPC.buffImmune[num] && Vector2.Distance(player.Center, nPC.Center) <= num2)
                        {
                            if (nPC.FindBuffIndex(num) == -1)
                            {
                                nPC.AddBuff(num, 120, false);
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
            plagueCounter++;
            if (plagueCounter >= 180)
            {
            }
        }
    }
}
