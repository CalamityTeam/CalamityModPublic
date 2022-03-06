using CalamityMod.CustomRecipes;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables;
using CalamityMod.Items.Weapons.Rogue;
using CalamityMod.Projectiles.DraedonsArsenal;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.DraedonsArsenal
{
    public class TrackingDisk : RogueWeapon
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Tracking Disk");
            Tooltip.SetDefault("A weapon that, as it flies, processes calculations and fires lasers\n" +
                               "Releases a flying disk that fires lasers at nearby enemies\n" +
                               "Stealth strikes allow the disk to fire multiple larger lasers at different targets");
        }
        public override void SafeSetDefaults()
        {
            CalamityGlobalItem modItem = item.Calamity();

            item.damage = 25;
            modItem.rogue = true;

            item.width = 30;
            item.height = 34;
            item.useTime = 42;
            item.useAnimation = 42;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.useTurn = false;
            item.knockBack = 3f;

            item.value = CalamityGlobalItem.Rarity3BuyPrice;
            item.rare = ItemRarityID.Red;
            modItem.customRarity = CalamityRarity.DraedonRust;

            item.noUseGraphic = true;

            item.UseSound = SoundID.Item1;
            item.autoReuse = true;

            item.shoot = ModContent.ProjectileType<TrackingDiskProjectile>();
            item.shootSpeed = 10f;

            modItem.UsesCharge = true;
            modItem.MaxCharge = 50f;
            modItem.ChargePerUse = 0.08f;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            int proj = Projectile.NewProjectile(position, new Vector2(speedX, speedY), type, damage, knockBack, player.whoAmI);
            if (proj.WithinBounds(Main.maxProjectiles))
                Main.projectile[proj].Calamity().stealthStrike = player.Calamity().StealthStrikeAvailable();
            return false;
        }

        public override void SafeModifyTooltips(List<TooltipLine> tooltips) => CalamityGlobalItem.InsertKnowledgeTooltip(tooltips, 1);

        public override void AddRecipes()
        {
            ArsenalTierGatedRecipe recipe = new ArsenalTierGatedRecipe(mod, 1);
            recipe.AddIngredient(ModContent.ItemType<MysteriousCircuitry>(), 5);
            recipe.AddIngredient(ModContent.ItemType<DubiousPlating>(), 7);
            recipe.AddIngredient(ModContent.ItemType<AerialiteBar>(), 4);
            recipe.AddIngredient(ModContent.ItemType<SeaPrism>(), 7);
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
