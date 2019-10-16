using CalamityMod.CalPlayer;
using Microsoft.Xna.Framework;
using System;
using Terraria; using CalamityMod.Projectiles; using Terraria.ModLoader;
using Terraria.ID;
using Terraria.ModLoader; using CalamityMod.Buffs; using CalamityMod.Items; using CalamityMod.NPCs; using CalamityMod.Projectiles; using CalamityMod.Tiles; using CalamityMod.Walls;

namespace CalamityMod.Items
{
    public class CosmicImmaterializer : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Cosmic Immaterializer");
            Tooltip.SetDefault("Summons a cosmic energy spiral to fight for you\n" +
                               "The orb will fire swarms of homing energy bolts when enemies are detected by it\n" +
                               "Requires 10 minion slots to use\n" +
                               "There can only be one\n" +
                               "Without a summoner armor set bonus this minion will deal less damage");
        }

        public override void SetDefaults()
        {
            item.mana = 100;
            item.damage = 3000;
            item.useStyle = 1;
            item.width = 74;
            item.height = 72;
            item.useTime = 36;
            item.useAnimation = 36;
            item.noMelee = true;
            item.knockBack = 0f;
            item.value = Item.buyPrice(2, 50, 0, 0);
            item.rare = 10;
            item.UseSound = SoundID.Item60;
            item.autoReuse = true;
            item.shoot = ModContent.ProjectileType<CosmicEnergySpiral>();
            item.shootSpeed = 10f;
            item.summon = true;
            item.Calamity().postMoonLordRarity = 15;
        }

        public override bool CanUseItem(Player player)
        {
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                Projectile p = Main.projectile[i];
                if (p.active && p.type == ModContent.ProjectileType<CosmicEnergySpiral>() && p.owner == player.whoAmI)
                {
                    return false;
                }
            }
            return true;
        }

        public override bool Shoot(Player player, ref Microsoft.Xna.Framework.Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            CalamityPlayer modPlayer = player.Calamity();
            bool hasSummonerSet = modPlayer.tarraSummon || modPlayer.bloodflareSummon || modPlayer.godSlayerSummon || modPlayer.silvaSummon;
            player.itemTime = item.useTime;
            Vector2 vector2 = player.RotatedRelativePoint(player.MountedCenter, true);
            float num78 = (float)Main.mouseX + Main.screenPosition.X - vector2.X;
            float num79 = (float)Main.mouseY + Main.screenPosition.Y - vector2.Y;
            if (player.gravDir == -1f)
            {
                num79 = Main.screenPosition.Y + (float)Main.screenHeight - (float)Main.mouseY - vector2.Y;
            }
            float num80 = (float)Math.Sqrt((double)(num78 * num78 + num79 * num79));
            if ((float.IsNaN(num78) && float.IsNaN(num79)) || (num78 == 0f && num79 == 0f))
            {
                num78 = (float)player.direction;
                num80 = item.shootSpeed;
            }
            else
            {
                num80 = item.shootSpeed / num80;
            }
            num78 = 0f;
            num79 = 0f;
            vector2.X = (float)Main.mouseX + Main.screenPosition.X;
            vector2.Y = (float)Main.mouseY + Main.screenPosition.Y;
            Projectile.NewProjectile(vector2.X, vector2.Y, num78, num79, type, (int)((double)damage * (hasSummonerSet ? 1.0 : 0.66)), knockBack, player.whoAmI, 0f, 0f);
            return false;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(null, "SunGodStaff");
            recipe.AddIngredient(null, "AncientIceChunk");
            recipe.AddIngredient(null, "ElementalAxe");
            recipe.AddIngredient(null, "EnergyStaff");
            recipe.AddIngredient(null, "NightmareFuel", 5);
            recipe.AddIngredient(null, "EndothermicEnergy", 5);
            recipe.AddIngredient(null, "CosmiliteBar", 5);
            recipe.AddIngredient(null, "DarksunFragment", 5);
            recipe.AddIngredient(null, "HellcasterFragment", 3);
            recipe.AddIngredient(null, "Phantoplasm", 5);
            recipe.AddIngredient(null, "AuricOre", 25);
            recipe.AddTile(null, "DraedonsForge");
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
