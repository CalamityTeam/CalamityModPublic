using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Melee.Spears;
using CalamityMod.Projectiles.Rogue;
using CalamityMod.Rarities;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Rogue
{
    public class SealedSingularity : RogueWeapon
    {
        /// <summary>
        /// Falloff per target hit simultaneously by the aura. Resets every hit interval
        /// </summary>
        public static float FalloffPerTargetHitByAura => 0.5f;
        public static float FallofPerTargetHitByBomb => 0.2f;

        public override void SetDefaults()
        {
            Item.width = 32;
            Item.height = 32;
            Item.damage = 260;
            Item.DamageType = RogueDamageClass.Instance;
            Item.autoReuse = true;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.useTime = 30;
            Item.useAnimation = 30;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 3;
            Item.value = CalamityGlobalItem.RarityTurquoiseBuyPrice;
            Item.rare = ModContent.RarityType<Turquoise>();
            Item.UseSound = SoundID.Item1;
            Item.channel = true;
            Item.shootSpeed = 13f;
            Item.shoot = ModContent.ProjectileType<SealedSingularityHoldout>();
            Item.Calamity().donorItem = true;
        }

        public override void HoldItem(Player player)
        {
            if (player.ownedProjectileCounts[Item.shoot] <= 0)
                player.Calamity().rogueStealth = 0;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<DuststormInABottle>().
                AddIngredient<DarkPlasma>(3).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}
