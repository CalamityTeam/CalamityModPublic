using CalamityMod.CalPlayer;
using CalamityMod.Items.Ammo.FiniteUse;
using CalamityMod.Projectiles.Typeless.FiniteUse;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Typeless.FiniteUse
{
    public class LightningHawk : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Lightning Hawk");
            Tooltip.SetDefault("Uses Magnum Rounds\n" +
                "Can be used thrice per boss battle");
        }

        public override void SetDefaults()
        {
            Item.damage = 400;
            Item.width = 50;
            Item.height = 28;
            Item.useTime = 21;
            Item.useAnimation = 21;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 8f;
            Item.value = Item.buyPrice(0, 48, 0, 0);
            Item.rare = ItemRarityID.LightPurple;
            Item.UseSound = SoundLoader.GetLegacySoundSlot(Mod, "Sounds/Item/Magnum");
            Item.autoReuse = true;
            Item.shootSpeed = 12f;
            Item.shoot = ModContent.ProjectileType<MagnumRound>();
            Item.useAmmo = ModContent.ItemType<MagnumRounds>();
            if (CalamityPlayer.areThereAnyDamnBosses)
                Item.Calamity().timesUsed = 3;
        }

        // Terraria seems to really dislike high crit values in SetDefaults
        public override void ModifyWeaponCrit(Player player, ref int crit) => crit += 56;

        public override bool OnPickup(Player player)
        {
            if (CalamityPlayer.areThereAnyDamnBosses)
                Item.Calamity().timesUsed = 3;

            return true;
        }

        public override bool CanUseItem(Player player) => Item.Calamity().timesUsed < 3;

        public override Vector2? HoldoutOffset() => new Vector2(-5, 0);

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
            CreateRecipe(1).AddIngredient(ModContent.ItemType<Magnum>()).AddIngredient(ItemID.IllegalGunParts).AddIngredient(ItemID.SoulofMight, 20).AddIngredient(ItemID.SoulofSight, 20).AddIngredient(ItemID.SoulofFright, 20).AddIngredient(ItemID.HallowedBar, 10).AddTile(TileID.MythrilAnvil).Register();
        }
    }
}
