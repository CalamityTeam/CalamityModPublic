using CalamityMod.Buffs.Summon;
using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Summon;
using CalamityMod.Rarities;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Summon
{
    public class CosmicImmaterializer : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Summon";

        public override void SetStaticDefaults() => ItemID.Sets.StaffMinionSlotsRequired[Type] = 10f;

        public override void SetDefaults()
        {
            Item.width = 170;
            Item.height = 164;
            Item.damage = 560;
            Item.DamageType = DamageClass.Summon;
            Item.mana = 10;
            Item.useAnimation = Item.useTime = 24;
            Item.knockBack = 0.25f;
            Item.buffType = ModContent.BuffType<CosmicEnergy>();
            Item.shoot = ModContent.ProjectileType<CosmicEnergySpiral>();

            Item.UseSound = SoundID.Item60;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.noMelee = true;

            Item.value = CalamityGlobalItem.RarityVioletBuyPrice;
            Item.rare = ModContent.RarityType<ExoticRainbow>();
        }

        public override bool CanUseItem(Player player) => player.ownedProjectileCounts[Item.shoot] <= 0 && player.maxMinions >= 10;

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            player.AddBuff(Item.buffType, 2);
            CalamityUtils.KillShootProjectiles(true, type, player);
            var minion = Projectile.NewProjectileDirect(source, player.ClampedMouseWorld(), Vector2.Zero, type, damage, knockback, player.whoAmI);
            minion.originalDamage = Item.damage;
            return false;
        }

        public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
        {
            Item.DrawItemGlowmaskSingleFrame(spriteBatch, rotation, ModContent.Request<Texture2D>("CalamityMod/Items/Weapons/Summon/CosmicImmaterializerGlow").Value);
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<LegionofCelestia>().
                AddIngredient<EtherealSubjugator>().
                AddIngredient<Cosmilamp>().
                AddIngredient<CalamarisLament>().
                AddIngredient<MiracleMatter>().
                AddTile<DraedonsForge>().
                Register();
        }
    }
}
