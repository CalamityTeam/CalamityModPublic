using System;
using System.Collections.Generic;
using CalamityMod.CustomRecipes;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables.SunkenSea;
using CalamityMod.Projectiles.DraedonsArsenal;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.DraedonsArsenal
{
    public class AqueousHunterDrone : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.DraedonsArsenal";
        public static readonly SoundStyle Fire = new("CalamityMod/Sounds/Item/ShrimpFire");
        public static readonly SoundStyle Hit = new("CalamityMod/Sounds/Item/ShrimpMissileHit");
        public static readonly SoundStyle Sound1 = new("CalamityMod/Sounds/Item/ShrimpSound1");
        public static readonly SoundStyle Sound2 = new("CalamityMod/Sounds/Item/ShrimpSound2");
        public static readonly SoundStyle Surprise = new("CalamityMod/Sounds/Item/ShrimpSurprise");

        public static int ArmorPenetration = 15;
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(ArmorPenetration);

        public override void SetDefaults()
        {
            CalamityGlobalItem modItem = Item.Calamity();

            Item.width = 34;
            Item.height = 32;
            Item.shootSpeed = 10f;
            Item.damage = 24;
            Item.ArmorPenetration = ArmorPenetration;
            Item.mana = 10;
            Item.useAnimation = Item.useTime = 36;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.noMelee = true;
            Item.knockBack = 2.25f;
            Item.value = CalamityGlobalItem.RarityOrangeBuyPrice;
            Item.rare = ItemRarityID.Orange;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<AqueousHunterDroneSummon>();
            Item.shootSpeed = 10f;
            Item.DamageType = DamageClass.Summon;
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Vector2 mouse = player.ClampedMouseWorld();
            Point mouseTileCoords = mouse.ToTileCoordinates();
            if (!CalamityUtils.ParanoidTileRetrieval(mouseTileCoords.X, mouseTileCoords.Y).HasTile)
            {
                int p = Projectile.NewProjectile(source, new Vector2(mouse.X, player.Center.Y - 600), Vector2.Zero, type, damage, knockback, player.whoAmI);
                Main.projectile[p].localAI[2] = player.ownedProjectileCounts[Item.shoot];
                if (Main.projectile.IndexInRange(p))
                    Main.projectile[p].originalDamage = Item.damage;
            }
            return false;
        }
        public override void ModifyTooltips(List<TooltipLine> tooltips) => CalamityGlobalItem.InsertKnowledgeTooltip(tooltips, 1);
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<MysteriousCircuitry>(8).
                AddIngredient<DubiousPlating>(4).
                AddIngredient<AerialiteBar>(4).
                AddIngredient<SeaPrism>(7).
                AddCondition(ArsenalTierGatedRecipe.ConstructRecipeCondition(1, out Func<bool> condition), condition).
                AddTile(TileID.Anvils).
                Register();
        }
    }
}
