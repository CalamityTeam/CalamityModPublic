using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Ranged;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace CalamityMod.Items.Weapons.Ranged
{
    public class TheHive : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Ranged";

        public static float MaxCharge = 90f;
        public static int OriginalUseTime = 34;
        public override void SetDefaults()
        {
            Item.damage = 92;
            Item.DamageType = DamageClass.Ranged;
            Item.crit = 16;
            Item.useAnimation = Item.useTime = OriginalUseTime;
            Item.shoot = ProjectileType<TheHiveHoldout>();
            Item.shootSpeed = 13f;
            Item.knockBack = 3.5f;

            Item.width = 66;
            Item.height = 30;
            Item.noMelee = true;
            Item.autoReuse = true;
            Item.channel = true;
            Item.noUseGraphic = true;
            Item.useAmmo = AmmoID.Rocket;
            Item.value = CalamityGlobalItem.RarityYellowBuyPrice;
            Item.rare = ItemRarityID.Yellow;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.UseSound = new SoundStyle("CalamityMod/Sounds/Item/DudFire") with { Volume = .4f, Pitch = -.9f, PitchVariance = 0.1f };
        }

        public override bool CanUseItem(Player player) => player.ownedProjectileCounts[Item.shoot] == 0;

        // Spawning the holdout won't consume ammo.
        public override bool CanConsumeAmmo(Item ammo, Player player) => player.ownedProjectileCounts[Item.shoot] != 0;

        // Makes the rotation of the mouse around the player sync in multiplayer.
        public override void HoldItem(Player player) => player.Calamity().mouseRotationListener = true;

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Item.channel = true;

            Projectile holdout = Projectile.NewProjectileDirect(source, player.MountedCenter, Vector2.Zero, ProjectileType<TheHiveHoldout>(), 0, 0f, player.whoAmI);

            // 14NOV2024: Ozzatron: clamped mouse position unnecessary, only used for direction
            // We set the rotation to the direction to the mouse so the first frame doesn't appear bugged out.
            holdout.velocity = (player.Calamity().mouseWorld - player.MountedCenter).SafeNormalize(Vector2.Zero);

            return false;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<MineralMortar>().
                AddIngredient<InfectedArmorPlating>(7).
                AddIngredient<PlagueCellCanister>(12).
                AddTile(TileID.MythrilAnvil).
                Register();
        }

        public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI) => Item.DrawItemGlowmaskSingleFrame(spriteBatch, rotation, Request<Texture2D>(Texture + "_Glow").Value);
    }
}
