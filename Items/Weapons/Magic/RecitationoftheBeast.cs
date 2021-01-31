using CalamityMod.Projectiles.Magic;
using CalamityMod.Items.Materials;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Magic
{
    public class RecitationoftheBeast : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Recitation of the Beast");
            Tooltip.SetDefault("A thousand years sealed in the demon's realm will teach you a thing or two\n" +
							   "Summons beast scythes around the player in a small circle,\n" +
                               "before firing toward the cursor and home in to nearby enemies");
        }

        public override void SetDefaults()
        {
			item.mana = 24;
            item.width = 38;
            item.height = 34;
            item.damage = 300;
            item.noMelee = true;
            item.useAnimation = 18;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.useTime = 18;
            item.knockBack = 8.5f;
            item.UseSound = SoundID.Item8;
            item.autoReuse = true;
            item.value = Item.buyPrice(1, 80, 0, 0);
            item.rare = ItemRarityID.Red;
            item.shoot = ModContent.ProjectileType<BeastScythe>();
            item.shootSpeed = 10f;
            item.magic = true;
            item.Calamity().customRarity = CalamityRarity.DarkBlue;
        }

		// Terraria seems to really dislike high crit values in SetDefaults
		public override void GetWeaponCrit(Player player, ref int crit) => crit += 20;

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
			float spread = 60f * 0.0174f;
			double startAngle = Math.Atan2(speedX, speedY) - spread / 2;
			double deltaAngle = spread / 6f;
			double offsetAngle;
			int i;
			for (i = 0; i < 3; i++)
			{
				offsetAngle = startAngle + deltaAngle * (i + i * i) / 2f + 32f * i;
				Projectile.NewProjectile(player.Center.X, player.Center.Y, (float)(Math.Sin(offsetAngle) * 2f), (float)(Math.Cos(offsetAngle) * 2f), type, damage, knockBack, Main.myPlayer, 0f, 0f);
				Projectile.NewProjectile(player.Center.X, player.Center.Y, (float)(-Math.Sin(offsetAngle) * 2f), (float)(-Math.Cos(offsetAngle) * 2f), type, damage, knockBack, Main.myPlayer, 0f, 0f);
			}
            return false;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.DemonScythe);
            recipe.AddIngredient(ModContent.ItemType<AscendantSpiritEssence>(), 3);
            recipe.AddIngredient(ModContent.ItemType<CosmiliteBar>(), 8);
            recipe.AddTile(TileID.Bookcases);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
