using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Magic;
using CalamityMod.Rarities;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Magic
{
    [LegacyName("Fabstaff")]
    public class Sylvestaff : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Magic";

        /// <summary>
        ///     The amount by which the staff recoils after firing.
        /// </summary>
        public static float StaffRecoilForce => 0.04f;

        /// <summary>
        ///     The rate at which rays from the staff can release bolts.
        /// </summary>
        public static int RayBoltShootRate => CalamityUtils.SecondsToFrames(0.0667f);

        /// <summary>
        ///     The distance range that targets need to be within relative to a ray's evaluation points in order to shoot bolts.
        /// </summary>
        public static float RayBoltTargetingRange => 272f;

        /// <summary>
        ///     The turn speed interpolant that dictates how fast or slow the staff can move to aim towards the mouse.
        /// </summary>
        public static float TurnSpeedInterpolant => 0.276f;

        /// <summary>
        ///     The sound played when Sylvestaff rays are fired.
        /// </summary>
        public static readonly SoundStyle FireSound = new SoundStyle("CalamityMod/Sounds/Item/SylvestaffFire", 3) with { MaxInstances = 5 };

        /// <summary>
        ///     The sound played when Sylvestaff rays bounce off of tiles.
        /// </summary>
        public static readonly SoundStyle BounceSound = new SoundStyle("CalamityMod/Sounds/Item/SylvestaffProjectileBounce", 3) with { MaxInstances = 5 };

        public override string Texture
        {
            get
            {
                if (WorldGen.SavedOreTiers.Gold == TileID.Gold || Main.gameMenu)
                    return "CalamityMod/Items/Weapons/Magic/SylvestaffGold";

                return "CalamityMod/Items/Weapons/Magic/SylvestaffPlatinum";
            }
        }

        public override void SetStaticDefaults()
        {
            Item.staff[Item.type] = true;
        }

        public override void SetDefaults()
        {
            Item.width = 84;
            Item.height = 84;
            Item.damage = 140;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 30;
            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.channel = true;
            Item.noUseGraphic = true;
            Item.knockBack = 5f;

            Item.value = CalamityGlobalItem.RarityHotPinkBuyPrice;
            Item.rare = ModContent.RarityType<HotPink>();
            Item.Calamity().devItem = true;

            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<SylvestaffHoldout>();
            Item.shootSpeed = 13.5f;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.RainbowRod).
                AddIngredient(ItemID.GenderChangePotion).
                AddRecipeGroup("AnyGoldBar", 5).
                AddIngredient<Necroplasm>(10).
                AddIngredient<ShadowspecBar>(5).
                AddTile<DraedonsForge>().
                Register();
        }

        public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            spriteBatch.Draw(texture, position, frame, drawColor, 0f, origin, scale, SpriteEffects.None, 0);
            return false;
        }

        public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            spriteBatch.Draw(texture, Item.position - Main.screenPosition, null, lightColor, 0f, Vector2.Zero, scale, SpriteEffects.None, 0);
            return false;
        }
    }
}
