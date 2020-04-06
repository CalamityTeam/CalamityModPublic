using CalamityMod.CalPlayer;
using CalamityMod.Projectiles.Typeless;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories
{
    //Dedicated to Dzicozan
    [AutoloadEquip(EquipType.Back)]
    public class TheCamper : ModItem
    {
        int auraCounter = 0;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("The Camper");
            Tooltip.SetDefault("You deal no damage unless stationary\n" +
                "Standing still grants buff(s) dependent on what weapon you're holding\n" +
                "Standing still provides a damaging aura around you\n" +
                "While moving, you regenerate health as if standing still\n" +
				"Provides a small amount of light in the Abyss\n" +
				"Provides cold protection in Death Mode\n" +
                "In rest may we find victory.");
        }

        public override void SetDefaults()
        {
            item.width = 26;
            item.height = 26;
            item.value = Item.buyPrice(0, 12, 0, 0); 
            item.rare = 7;
            item.accessory = true;
            item.defense = 10;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.camper = true;
            player.AddBuff(BuffID.HeartLamp, 60, true);
            player.AddBuff(BuffID.Campfire, 60, true);
            player.AddBuff(BuffID.WellFed, 60, true);
            player.lifeRegen += 2;
            Lighting.AddLight(player.Center, 0.825f, 0.66f, 0f);
            if (Main.myPlayer == player.whoAmI)
            {
                if ((double)Math.Abs(player.velocity.X) < 0.05 && (double)Math.Abs(player.velocity.Y) < 0.05)
                {
                    player.allDamage += 0.15f;
                    auraCounter++;
                    float range = 200f;
                    if (auraCounter == 9)
                    {
                        auraCounter = 0;
                        for (int i = 0; i < Main.maxNPCs; i++)
                        {
                            NPC npc = Main.npc[i];
                            if (npc.active && !npc.friendly && npc.damage > -1 && !npc.dontTakeDamage && Vector2.Distance(player.Center, npc.Center) <= range)
                            {
                                Projectile p = Projectile.NewProjectileDirect(npc.Center, Vector2.Zero, ModContent.ProjectileType<DirectStrike>(), (int)(Main.rand.Next(20, 41) * player.AverageDamage()), 0f, player.whoAmI, i);
                                if (!npc.buffImmune[BuffID.OnFire])
                                {
                                    npc.AddBuff(BuffID.OnFire, 120);
                                }
                            }
                        }
                    }
                    if (player.HeldItem != null && !player.HeldItem.IsAir && player.HeldItem.stack > 0)
                    {
                        bool summon = player.HeldItem.summon;
                        bool rogue = player.HeldItem.Calamity().rogue;
                        bool melee = player.HeldItem.melee;
                        bool ranged = player.HeldItem.ranged;
                        bool magic = player.HeldItem.magic;
                        if (summon)
                        {
                            player.minionKB += 0.10f;
                            player.AddBuff(BuffID.Bewitched, 60, true);
                        }
                        else if (rogue)
                        {
                            modPlayer.throwingVelocity += 0.10f;
                        }
                        else if (melee)
                        {
                            player.meleeSpeed += 0.10f;
                            player.AddBuff(BuffID.Sharpened, 60, true);
                        }
                        else if (ranged)
                        {
                            player.AddBuff(BuffID.AmmoBox, 60, true);
                        }
                        else if (magic)
                        {
                            player.AddBuff(BuffID.Clairvoyance, 60, true);
                        }
                    }
                }
                else
                {
                    auraCounter = 0;
                }
            }
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.Campfire, 10);
            recipe.AddIngredient(ItemID.HeartLantern, 5);
            recipe.AddRecipeGroup("AnyFood", 50);
            recipe.AddIngredient(ItemID.ShinyStone);
            recipe.AddIngredient(ItemID.SharpeningStation);
            recipe.AddIngredient(ItemID.CrystalBall);
            recipe.AddIngredient(ItemID.AmmoBox);
            recipe.AddIngredient(ItemID.BewitchingTable);

            recipe.AddTile(TileID.TinkerersWorkbench);
            recipe.SetResult(this);
            recipe.AddRecipe();

        }
    }
}
