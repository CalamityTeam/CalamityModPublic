using CalamityMod.CustomRecipes;
using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.DraedonsArsenal;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.DraedonsArsenal
{
    public class MountedScanner : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.DraedonsArsenal";
        public override void SetDefaults()
        {
            CalamityGlobalItem modItem = Item.Calamity();

            Item.width = 26;
            Item.height = 26;
            Item.DamageType = DamageClass.Summon;
            Item.damage = 41;
            Item.knockBack = 2f;
            Item.mana = 10;
            Item.useTime = Item.useAnimation = 24;
            Item.autoReuse = true;

            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.UseSound = SoundID.Item15;
            Item.noMelee = true;

            Item.value = CalamityGlobalItem.Rarity5BuyPrice;
            Item.rare = ItemRarityID.Pink;

            Item.shoot = ModContent.ProjectileType<MountedScannerSummon>();
            Item.shootSpeed = 1f;

            modItem.UsesCharge = true;
            modItem.MaxCharge = 85f;
            modItem.ChargePerUse = 0.85f;
            modItem.ChargePerAltUse = 0f;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            var scanner = Projectile.NewProjectileDirect(source, player.Center, Vector2.Zero, type, damage, knockback, player.whoAmI);
            scanner.originalDamage = Item.damage;

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
            CreateRecipe().
                AddIngredient<MysteriousCircuitry>(15).
                AddIngredient<DubiousPlating>(5).
                AddRecipeGroup("AnyMythrilBar", 10).
                AddIngredient(ItemID.SoulofFright, 20).
                AddCondition(ArsenalTierGatedRecipe.ConstructRecipeCondition(2, out Func<bool> condition), condition).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}
