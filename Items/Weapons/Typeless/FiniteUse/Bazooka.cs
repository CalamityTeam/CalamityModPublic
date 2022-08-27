using CalamityMod.CalPlayer;
using CalamityMod.Items.Ammo.FiniteUse;
using CalamityMod.Projectiles.Typeless.FiniteUse;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.Items.Weapons.Typeless.FiniteUse
{
    public class Bazooka : ModItem
    {
        public static readonly SoundStyle FireSound = new("CalamityMod/Sounds/Item/BazookaFull");

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Bazooka");
            Tooltip.SetDefault("Uses Grenade Shells\n" +
                "Can be used twice per boss battle");
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.damage = 500;
            Item.width = 66;
            Item.height = 26;
            Item.useTime = 30;
            Item.useAnimation = 30;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 10f;
            Item.value = Item.buyPrice(0, 36, 0, 0);
            Item.rare = ItemRarityID.Pink;
            Item.UseSound = FireSound;
            Item.autoReuse = true;
            Item.shootSpeed = 12f;
            Item.shoot = ModContent.ProjectileType<GrenadeRound>();
            Item.useAmmo = ModContent.ItemType<GrenadeRounds>();
            if (CalamityPlayer.areThereAnyDamnBosses)
                Item.Calamity().timesUsed = 2;
        }

		public override void ModifyResearchSorting(ref ContentSamples.CreativeHelper.ItemGroup itemGroup)
		{
			itemGroup = (ContentSamples.CreativeHelper.ItemGroup)CalamityResearchSorting.ClasslessWeapon;
		}

        public override bool OnPickup(Player player)
        {
            if (CalamityPlayer.areThereAnyDamnBosses)
                Item.Calamity().timesUsed = 2;

            return true;
        }

        public override bool CanUseItem(Player player)
        {
            return Item.Calamity().timesUsed < 2;
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-10, 0);
        }

        public override void UpdateInventory(Player player)
        {
            if (!CalamityPlayer.areThereAnyDamnBosses)
                Item.Calamity().timesUsed = 0;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (CalamityPlayer.areThereAnyDamnBosses)
            {
                player.HeldItem.Calamity().timesUsed++;
                for (int i = 0; i < Main.InventorySlotsTotal; i++)
                {
                    if (player.inventory[i].type == Item.type && player.inventory[i] != player.HeldItem)
                        player.inventory[i].Calamity().timesUsed++;
                }
            }
            return true;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.IllegalGunParts).
                AddRecipeGroup("IronBar", 20).
                AddRecipeGroup("AnyAdamantiteBar", 15).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}
