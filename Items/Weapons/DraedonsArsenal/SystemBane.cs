using CalamityMod.CustomRecipes;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Weapons.Rogue;
using CalamityMod.Projectiles.DraedonsArsenal;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.DraedonsArsenal
{
    public class SystemBane : RogueWeapon
    {
        public const int MaxDeployedProjectiles = 5;
        
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("System Bane");
            Tooltip.SetDefault("Can be used to quickly send out an electromagnetic blast\n" +
                               "Hurls an unstable device which sticks to the ground and shocks nearby enemies with lightning\n" +
                               "Stealth strikes make the device emit a large, damaging EMP field");
        }

        public override void SafeSetDefaults()
        {
            CalamityGlobalItem modItem = item.Calamity();

            item.damage = 45;
            modItem.rogue = true;
            item.noMelee = true;
            item.noUseGraphic = true;
            item.width = 42;
            item.height = 36;
            item.useTime = 15;
            item.useAnimation = 15;
            item.autoReuse = true;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.knockBack = 4f;
            item.value = CalamityGlobalItem.Rarity8BuyPrice;
            item.rare = ItemRarityID.Red;
            modItem.customRarity = CalamityRarity.DraedonRust;
            item.UseSound = SoundID.Item1;

            item.shootSpeed = 16f;
            item.shoot = ModContent.ProjectileType<SystemBaneProjectile>();

            modItem.UsesCharge = true;
            modItem.MaxCharge = 135f;
            modItem.ChargePerUse = 0.085f;
        }

        public override bool CanUseItem(Player player) => player.ownedProjectileCounts[item.shoot] < MaxDeployedProjectiles;

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            int proj = Projectile.NewProjectile(position, new Vector2(speedX, speedY), type, damage, knockBack, player.whoAmI);
            if (proj.WithinBounds(Main.maxProjectiles))
                Main.projectile[proj].Calamity().stealthStrike = player.Calamity().StealthStrikeAvailable();
            return false;
        }

        public override void SafeModifyTooltips(List<TooltipLine> tooltips) => CalamityGlobalItem.InsertKnowledgeTooltip(tooltips, 3);

        public override void AddRecipes()
        {
            ArsenalTierGatedRecipe recipe = new ArsenalTierGatedRecipe(mod, 3);
            recipe.AddIngredient(ModContent.ItemType<MysteriousCircuitry>(), 20);
            recipe.AddIngredient(ModContent.ItemType<DubiousPlating>(), 10);
            recipe.AddIngredient(ModContent.ItemType<BarofLife>(), 5);
            recipe.AddIngredient(ModContent.ItemType<InfectedArmorPlating>(), 5);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
