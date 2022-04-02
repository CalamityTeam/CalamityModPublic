using CalamityMod.CustomRecipes;
using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.DraedonsArsenal;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.DraedonsArsenal
{
    public class MountedScanner : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Mounted Scanner");
            Tooltip.SetDefault("Laser technology used in this case for both targeting and defense\n" +
            "Summons a powerful weapon above your head that fires lasers at nearby enemies");
        }

        public override void SetDefaults()
        {
            CalamityGlobalItem modItem = item.Calamity();

            item.width = 26;
            item.height = 26;
            item.summon = true;
            item.damage = 41;
            item.knockBack = 2f;
            item.mana = 10;
            item.useTime = item.useAnimation = 24;
            item.autoReuse = true;

            item.useStyle = ItemUseStyleID.HoldingUp;
            item.UseSound = SoundID.Item15;
            item.noMelee = true;

            item.value = CalamityGlobalItem.Rarity5BuyPrice;
            item.rare = ItemRarityID.Red;
            modItem.customRarity = CalamityRarity.DraedonRust;

            item.shoot = ModContent.ProjectileType<MountedScannerSummon>();
            item.shootSpeed = 1f;

            modItem.UsesCharge = true;
            modItem.MaxCharge = 85f;
            modItem.ChargePerUse = 0.85f;
            modItem.ChargePerAltUse = 0f;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            Projectile.NewProjectile(player.Center, Vector2.Zero, type, damage, knockBack, player.whoAmI);
            int totalOwnedScanners = player.ownedProjectileCounts[type];
            int currentScannerIndex = 0;
            foreach (Projectile projectile in Main.projectile)
            {
                if (!projectile.active)
                    continue;
                if (projectile.type != type)
                    continue;
                if (projectile.owner != player.whoAmI)
                    continue;
                float completionRatio = currentScannerIndex / (float)totalOwnedScanners;

                // ai[0] is the angular offset relative to the projectile's owner.
                // For the first 15 summons, wrap around the player angularly, but not at a perfect angle, a bit like the Dazzling Stabbers when idle.
                // But once the total summon count is greater than 15, just create a perfect circle depending on the total amount of summons.
                if (totalOwnedScanners <= 14)
                {
                    projectile.ai[0] = Utils.AngleLerp(0f, MathHelper.Pi, currentScannerIndex / 15f);
                    if (currentScannerIndex % 2f == 1f)
                        projectile.ai[0] = -Utils.AngleLerp(0f, MathHelper.Pi, (currentScannerIndex + 1) / 15f);
                }
                else
                {
                    projectile.ai[0] = MathHelper.TwoPi / totalOwnedScanners * currentScannerIndex;
                }

                // Add a specific offset so that the scanners spawn above the player at first and not to the side.
                projectile.ai[0] -= MathHelper.PiOver2;
                projectile.netUpdate = true;
                currentScannerIndex++;
            }
            return false;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips) => CalamityGlobalItem.InsertKnowledgeTooltip(tooltips, 2);

        public override void AddRecipes()
        {
            ArsenalTierGatedRecipe recipe = new ArsenalTierGatedRecipe(mod, 2);
            recipe.AddIngredient(ModContent.ItemType<MysteriousCircuitry>(), 15);
            recipe.AddIngredient(ModContent.ItemType<DubiousPlating>(), 5);
            recipe.AddRecipeGroup("AnyMythrilBar", 10);
            recipe.AddIngredient(ItemID.SoulofFright, 20);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
