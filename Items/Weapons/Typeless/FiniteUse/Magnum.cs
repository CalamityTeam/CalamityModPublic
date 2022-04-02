using CalamityMod.CalPlayer;
using CalamityMod.Items.Ammo.FiniteUse;
using CalamityMod.Projectiles.Typeless.FiniteUse;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Typeless.FiniteUse
{
    public class Magnum : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Magnum");
            Tooltip.SetDefault("Uses Magnum Rounds\n" +
                "Can be used thrice per boss battle");
        }

        public override void SetDefaults()
        {
            item.damage = 80;
            item.width = 52;
            item.height = 28;
            item.useTime = 25;
            item.useAnimation = 25;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.noMelee = true;
            item.knockBack = 8f;
            item.value = Item.buyPrice(0, 2, 0, 0);
            item.rare = ItemRarityID.Green;
            item.UseSound = mod.GetLegacySoundSlot(SoundType.Item, "Sounds/Item/Magnum");
            item.autoReuse = true;
            item.shootSpeed = 12f;
            item.shoot = ModContent.ProjectileType<MagnumRound>();
            item.useAmmo = ModContent.ItemType<MagnumRounds>();
            if (CalamityPlayer.areThereAnyDamnBosses)
                item.Calamity().timesUsed = 3;
        }

        // Terraria seems to really dislike high crit values in SetDefaults
        public override void GetWeaponCrit(Player player, ref int crit) => crit += 46;

        public override bool OnPickup(Player player)
        {
            if (CalamityPlayer.areThereAnyDamnBosses)
                item.Calamity().timesUsed = 3;

            return true;
        }

        public override bool CanUseItem(Player player) => item.Calamity().timesUsed < 3;

        public override Vector2? HoldoutOffset() => new Vector2(-5, 0);

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
            recipe.AddIngredient(ItemID.FlintlockPistol);
            recipe.AddIngredient(ItemID.IronBar, 10);
            recipe.anyIronBar = true;
            recipe.AddIngredient(ItemID.Diamond, 5);
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
