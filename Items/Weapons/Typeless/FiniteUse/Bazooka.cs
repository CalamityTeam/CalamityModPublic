using CalamityMod.CalPlayer;
using CalamityMod.Items.Ammo.FiniteUse;
using CalamityMod.Projectiles.Typeless.FiniteUse;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Typeless.FiniteUse
{
    public class Bazooka : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Bazooka");
            Tooltip.SetDefault("Uses Grenade Shells\n" +
                "Can be used twice per boss battle");
        }

        public override void SetDefaults()
        {
            item.damage = 500;
            item.width = 66;
            item.height = 26;
            item.useTime = 30;
            item.useAnimation = 30;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.noMelee = true;
            item.knockBack = 10f;
            item.value = Item.buyPrice(0, 36, 0, 0);
            item.rare = ItemRarityID.Pink;
            item.UseSound = mod.GetLegacySoundSlot(SoundType.Item, "Sounds/Item/BazookaFull");
            item.autoReuse = true;
            item.shootSpeed = 12f;
            item.shoot = ModContent.ProjectileType<GrenadeRound>();
            item.useAmmo = ModContent.ItemType<GrenadeRounds>();
            if (CalamityPlayer.areThereAnyDamnBosses)
                item.Calamity().timesUsed = 2;
        }

        public override bool OnPickup(Player player)
        {
            if (CalamityPlayer.areThereAnyDamnBosses)
                item.Calamity().timesUsed = 2;

            return true;
        }

        public override bool CanUseItem(Player player)
        {
            return item.Calamity().timesUsed < 2;
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-10, 0);
        }

        public override void UpdateInventory(Player player)
        {
            if (!CalamityPlayer.areThereAnyDamnBosses)
                item.Calamity().timesUsed = 0;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            if (CalamityPlayer.areThereAnyDamnBosses)
            {
                player.HeldItem.Calamity().timesUsed++;
                for (int i = 0; i < Main.maxInventory; i++)
                {
                    if (player.inventory[i].type == item.type && player.inventory[i] != player.HeldItem)
                        player.inventory[i].Calamity().timesUsed++;
                }
            }
            return true;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.IllegalGunParts);
            recipe.AddIngredient(ItemID.IronBar, 20);
            recipe.anyIronBar = true;
            recipe.AddRecipeGroup("AnyAdamantiteBar", 15);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
