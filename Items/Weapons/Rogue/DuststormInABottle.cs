using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Rogue;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Rogue
{
    public class DuststormInABottle : RogueWeapon
    {
        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 24;
            Item.damage = 85;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.useAnimation = 28;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTime = 28;
            Item.knockBack = 4f;
            Item.UseSound = SoundID.Item106;
            Item.autoReuse = true;
            Item.value = CalamityGlobalItem.RarityLimeBuyPrice;
            Item.rare = ItemRarityID.Lime;
            Item.shoot = ModContent.ProjectileType<DuststormInABottleHoldout>();
            Item.shootSpeed = 14f;
            Item.DamageType = RogueDamageClass.Instance;
            Item.channel = true;
        }
        public override void HoldItem(Player player)
        {
            if (player.ownedProjectileCounts[Item.shoot] <= 0)
                player.Calamity().rogueStealth = 0;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<LemonNade>().
                AddIngredient(ItemID.AncientCloth,5).
                AddIngredient<GrandScale>().
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}
