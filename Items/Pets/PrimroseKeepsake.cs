using CalamityMod.Buffs.Pets;
using CalamityMod.Projectiles.Pets;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Pets
{
    public class PrimroseKeepsake : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Primrose Keepsake");
            Tooltip.SetDefault("Did they just...");
        }

        public override void SetDefaults()
        {
            item.damage = 0;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.useAnimation = 20;
            item.useTime = 20;
            item.noMelee = true;
            item.width = 30;
            item.height = 30;
            item.shoot = ProjectileID.None; // neither kendra nor bear is the direct "shoot"
            item.buffType = ModContent.BuffType<FurtasticDuoBuff>();
            item.UseSound = SoundID.Item44;

            item.value = Item.sellPrice(platinum: 1);
            item.rare = ItemRarityID.Purple;
            item.Calamity().devItem = true;
        }

        public override void UseStyle(Player player)
        {
            if (player.whoAmI == Main.myPlayer && player.itemTime == 0)
            {
                player.AddBuff(item.buffType, 15, true);
            }
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            List<int> pets = new List<int> { ModContent.ProjectileType<Bear>(), ModContent.ProjectileType<KendraPet>() };
            foreach(int petProjID in pets)
                Projectile.NewProjectile(position, new Vector2(speedX, speedY), petProjID, damage, knockBack, player.whoAmI);
            return false;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<BearEye>());
            recipe.AddIngredient(ModContent.ItemType<RomajedaOrchid>());
            recipe.AddIngredient(ItemID.LovePotion);
            recipe.AddTile(TileID.TinkerersWorkbench);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
