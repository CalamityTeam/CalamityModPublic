using System;
using System.Linq;
using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Magic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using static CalamityMod.CalamityUtils;

namespace CalamityMod.Items.Weapons.Magic
{
    [LegacyName("WulfrumStaff")]
    public class WulfrumProsthesis : ModItem, IHideFrontArm
    {
        public static readonly SoundStyle ShootSound = new("CalamityMod/Sounds/Item/WulfrumProsthesisShoot") { PitchVariance = 0.1f, Volume = 0.55f };
        public static readonly SoundStyle HitSound = new("CalamityMod/Sounds/Item/WulfrumProsthesisHit") { PitchVariance = 0.1f, Volume = 0.75f , MaxInstances = 3};
        public static readonly SoundStyle SuckSound = new("CalamityMod/Sounds/Item/WulfrumProsthesisSucc") { Volume = 0.5f };
        public static readonly SoundStyle SuckStopSound = new("CalamityMod/Sounds/Item/WulfrumProsthesisSuccStop") { Volume = 0.5f };

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Wulfrum Prosthesis");
            Tooltip.SetDefault("Casts a wulfrum bolt\n" +
                               "Right click to drain mana from creatures in front of you");
            //Lore about how magic is not always a given for everyone, and how some unlucky people sometimes resort to voluntarily cutting their limbs to use magic augmented prosthesis
            //1 : Informs about magic as a narrative thing, 2 : Informs about wulfrum energy being partly magical.
            SacrificeTotal = 1;
        }

        internal static Asset<Texture2D> RealSprite;

        public override string Texture => "CalamityMod/Items/Weapons/Magic/WulfrumProsthesis_Arm";

        public override void SetDefaults()
        {
            Item.damage = 13;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 2;
            Item.width = 34;
            Item.height = 42;
            Item.useTime = 24;
            Item.useAnimation = 24;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 3;
            Item.value = Item.buyPrice(0, 1, 0, 0);
            Item.rare = ItemRarityID.Blue;
            Item.UseSound = ShootSound;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<WulfrumBolt>();
            Item.shootSpeed = 18f;
            Item.holdStyle = 16; //Custom hold style
        }

        public override void HoldItem(Player player)
        {
            player.Calamity().mouseWorldListener = true;
            player.Calamity().rightClickListener = true;
        }

        public override bool CanUseItem(Player player) => !Main.projectile.Any(n => n.active && n.owner == player.whoAmI && n.type == ModContent.ProjectileType<WulfrumManaDrain>());

        public override bool AltFunctionUse(Player player) => true;

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            if (player.altFunctionUse == 2)
                type = ModContent.ProjectileType<WulfrumManaDrain>();
        }

        public override void ModifyManaCost(Player player, ref float reduce, ref float mult)
        {
            if (player.altFunctionUse == 2)
                mult = 0f;
        }

        public override void UseAnimation(Player player)
        {
            Item.UseSound = ShootSound;
            if (player.altFunctionUse == 2)
                Item.UseSound = null;
        }

        public void SetItemInHand(Player player, Rectangle heldItemFrame)
        {
            //Make the player face where they're aiming.
            if (player.Calamity().mouseWorld.X > player.Center.X)
            {
                player.ChangeDir(1);
            }
            else
            {
                player.ChangeDir(-1);
            }

            float animProgress = 1 - player.itemTime / (float)player.itemTimeMax;

            //Default
            Vector2 itemPosition = player.MountedCenter + new Vector2(-2f * player.direction, -1f * player.gravDir);
            float itemRotation = (player.Calamity().mouseWorld - itemPosition).ToRotation();

            //Adjust for animation

            if (animProgress < 0.7f)
                itemPosition -= itemRotation.ToRotationVector2() * (1 - (float)Math.Pow(1 - (0.7f - animProgress) / 0.7f, 4)) * 4f;

            if (animProgress < 0.4f)
                itemRotation += -0.45f * (float)Math.Pow((0.4f - animProgress) / 0.4f, 2) * player.direction * player.gravDir;

            //Shakezzz
            if (player.itemTime == 1 && Main.projectile.Any(n => n.active && n.owner == player.whoAmI && n.type == ModContent.ProjectileType<WulfrumManaDrain>()))
            {
                itemPosition += Main.rand.NextVector2Circular(2f, 2f);
            }


            Vector2 itemSize = new Vector2(28, 14);
            Vector2 itemOrigin = new Vector2(-8, 0);
            CleanHoldStyle(player, itemRotation, itemPosition, itemSize, itemOrigin, true);
        }

        public override void HoldStyle(Player player, Rectangle heldItemFrame) => SetItemInHand(player, heldItemFrame);
        public override void UseStyle(Player player, Rectangle heldItemFrame) => SetItemInHand(player, heldItemFrame);

        public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            if (RealSprite == null)
                RealSprite = ModContent.Request<Texture2D>("CalamityMod/Items/Weapons/Magic/WulfrumProsthesis");

            Texture2D properSprite = RealSprite.Value;

            spriteBatch.DrawNewInventorySprite(properSprite, new Vector2(28f, 14f), position, drawColor, origin, scale, new Vector2(0, -6f));

            return false;
        }

        public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            if (RealSprite == null)
                RealSprite = ModContent.Request<Texture2D>("CalamityMod/Items/Weapons/Magic/WulfrumProsthesis");

            Texture2D properSprite = RealSprite.Value;

            spriteBatch.Draw(properSprite, Item.position - Main.screenPosition, null, lightColor, rotation, properSprite.Size() / 2f, scale, 0, 0);

            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<WulfrumMetalScrap>(10).
                AddIngredient<EnergyCore>().
                AddTile(TileID.Anvils).
                Register();
        }
    }

    public class WulfrumProsthesisPlayer : ModPlayer
    {
        public bool ManaDrainActive = false;

        public override void UpdateDead()
        {
            ManaDrainActive = false;
        }

        public override void PostUpdateMiscEffects()
        {
            if (ManaDrainActive)
            {
                Player.manaRegenBonus = 35;
                Player.manaRegenDelay = 0;
                Player.manaRegenBuff = true;
            }

            ManaDrainActive = false;
        }
    }
}
