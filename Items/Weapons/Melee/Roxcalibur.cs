using System.Collections.Generic;
using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Melee;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;


namespace CalamityMod.Items.Weapons.Melee
{
	public class Roxcalibur : ModItem, ILocalizedModType
	{

		public string LocalizationCategory => "Items.Weapons.Melee";

		public static int BaseUseTime = 40;
		public override void SetDefaults()
		{
			Item.damage = 180;
			Item.knockBack = 13f;
            Item.DamageType = DamageClass.Melee;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.UseSound = SoundID.NPCHit42;
			Item.width = 100;
			Item.height = 100;
            Item.useAnimation = Item.useTime = BaseUseTime;
			Item.reuseDelay = 10;
			Item.shoot = ModContent.ProjectileType<RoxcaliburProj>();
			Item.shootSpeed = 4f;
			Item.value = CalamityGlobalItem.Rarity4BuyPrice;
			Item.rare = ItemRarityID.LightRed;
            Item.Calamity().donorItem = true;

            Item.noUseGraphic = true;
            Item.noMelee = true;
            Item.autoReuse = false;
            Item.channel = true;
        }

		public override void ModifyWeaponCrit(Player player, ref float crit)
		{
			crit += 8f;
		}
		public override bool CanUseItem(Player player) => Main.hardMode && player.ownedProjectileCounts[Item.shoot] < 1;
		public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
		{
			Projectile.NewProjectile(source, position, velocity, type, damage, knockback, ai2: player.itemTimeMax); //needed to set ai2
			return false;
		}
		public override void ModifyTooltips(List<TooltipLine> list)
		{
			list.FindAndReplace("[WOF]", Main.hardMode ? string.Empty : this.GetLocalizedValue("LockedInfo") + "\n");
		}

		public override void AddRecipes()
		{
			CreateRecipe()
				.AddIngredient(ItemID.HellstoneBar, 25)
				.AddIngredient(ItemID.SoulofNight, 10)
				.AddIngredient<EssenceofHavoc>(5)
				.AddIngredient(ItemID.Obsidian, 10)
				.AddIngredient(ItemID.StoneBlock, 100)
				.AddIngredient(ItemID.Amethyst, 2)
				.AddTile(TileID.Anvils)
				.Register();
		}
	}
}
